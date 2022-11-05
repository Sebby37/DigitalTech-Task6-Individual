using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

// The BPM change struct
public struct MaskBPMChange
{
    public MaskBPMChange(float _beat, float _newBpm)
    {
        beat = _beat;
        newBpm = _newBpm;
    }
    
    public float beat;
    public float newBpm;
}

public class MaskSongManager : MonoBehaviour
{
    public static float BPM = 120;

    [Header("Song Config")]
    public UnityEvent winEvent;
    public AudioSource musicSource;
    public string notesFile;
    public string bpmChangesFile;

    // The notes and bpm changes
    List<float> notes = new List<float>();
    List<MaskBPMChange> bpmChanges = new List<MaskBPMChange>();
    float lastNote;

    // The cue object
    MaskCue maskCue;

    // Song timing
    bool songStarted = false;
    float currentBeat = 0;
    float secondsPerBeat;
    float bobCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Getting the mask cue component
        maskCue = GetComponent<MaskCue>();

        // Loading the notes and bpm changes from the file
        LoadNotesFromFile();
        LoadBpmChangesFromFile();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (songStarted)
        {
            // Incrementing the current beat counter and bob counter
            bobCounter += Time.deltaTime / secondsPerBeat;
            currentBeat += Time.deltaTime / secondsPerBeat;

            // Bobbing to the beat if one beat has passed
            if (bobCounter >= 1)
            {
                bobCounter = 0;
                maskCue.BobToBeat();
            }

            // Checking if a BPM change is to be done, changing the BPM if so
            foreach (MaskBPMChange change in bpmChanges.ToArray())
                if (currentBeat >= change.beat)
                {
                    print($"{change.beat}, {currentBeat}, {bpmChanges.Count}");
                    ChangeBPM(change.newBpm);
                    bpmChanges.Remove(change);
                }

            // Checking if a cue is needed to be played, playing the cue if so
            foreach (float note in notes.ToArray()) 
                if (currentBeat >= note)
                {
                    maskCue.PlayCue();
                    notes.Remove(note);
                }

            // Ending the song if there are no more notes left and the beat counter is high enough
            if (winEvent != null && notes.Count <= 0 && currentBeat >= lastNote + 16)
                winEvent.Invoke();
        }
    }

    private void Update()
    {
        // Starting the song on space key press
        if (!songStarted && Input.GetKeyDown(KeyCode.Space))
        {
            songStarted = true;
            musicSource.Play();
        }
    }

    private void OnGUI()
    {
        // DEBUG: Drawing the current beat to the screen using OnGUI
        //GUI.TextArea(new Rect(0, 0, 200, 50), $"Current beat: {currentBeat}");
    }

    // Function to change the BPM
    void ChangeBPM(float newBpm)
    {
        BPM = newBpm;
        secondsPerBeat = maskCue.ChangeBPM(newBpm);
        print($"{BPM}, {secondsPerBeat}");
    }

    // Function to load all notes from a file into a list
    void LoadNotesFromFile()
    {
        // Reading each line of the notes file into a string array
        string[] rawFileContents = File.ReadAllLines(notesFile);

        // Looping through each line of the file
        foreach (string raw in rawFileContents)
        {
            // Trying to parse the current line into a float, continuing if it cannot
            float noteTime;
            if (!float.TryParse(raw, out noteTime)) continue;
            notes.Add(noteTime);
        }

        // Setting the last note
        lastNote = notes[notes.Count - 1];
    }

    // Function to load all bpm changes from a file into a list
    void LoadBpmChangesFromFile()
    {
        // Reading each line of the notes file into a string array
        string[] rawFileContents = File.ReadAllLines(bpmChangesFile);

        // Looping through each line of the file
        foreach (string raw in rawFileContents)
        {
            // Continuing if the current line is empty
            if (raw == "") continue;

            // Splitting the current line into an array, continuing if the length is less than 2
            string[] lineContents = raw.Split(' ');
            if (lineContents.Length < 2) continue;

            // Attempting to parse the current line into floats
            float bpmChangeTime;
            float newBpm;

            if (!float.TryParse(lineContents[0], out bpmChangeTime)) continue;
            if (!float.TryParse(lineContents[1], out newBpm)) continue;

            // Saving the newly parsed values into a BPMChange event and adding that to the list of events
            MaskBPMChange bpmChangeEvent = new MaskBPMChange(bpmChangeTime, newBpm);
            bpmChanges.Add(bpmChangeEvent);
        }
        
        // Setting the BPM to the first BPM in the list
        if (bpmChanges.Count > 0) ChangeBPM(bpmChanges[0].newBpm);
    }
}
