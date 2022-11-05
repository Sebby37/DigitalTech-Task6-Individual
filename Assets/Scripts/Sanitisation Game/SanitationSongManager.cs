using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public class SanitationSongManager : MonoBehaviour
{
    [Header("Song Parameters")]
    public float BPM = 94;
    public AudioSource musicSource;
    public string notesFile;
    public UnityEvent winEvent;

    // The cue object
    SanitisationCue sanitisationCue;

    // Notes list
    private List<float> notes = new List<float>();

    // Song timing
    float lastNote;
    bool songStarted = false;
    float currentBeat = 0;
    float secondsPerBeat;

    // Start is called before the first frame update
    void Start()
    {
        // Getting the sanitisation cue handler object and setting it's BPM
        sanitisationCue = GetComponent<SanitisationCue>();
        secondsPerBeat = sanitisationCue.ChangeBPM(BPM);

        // Loading the notes from the file
        LoadNotesFromFile();
    }

    void FixedUpdate()
    {
        // Adding the time in seconds from the last frame divided by seconds per beat to add 1 beat per second to the counter
        if (songStarted)
            currentBeat += Time.deltaTime / secondsPerBeat;

        if (songStarted && notes.Count > 0)
        {
            // Getting the next note and playing it if the current beat is greater than / equal to it
            float nextNote = notes[0];
            if (currentBeat >= nextNote)
            {
                // Checking if the next note is 1 beat after it, dinging if so and playing the full cue if not
                if (notes.Count > 1 && notes[1] == notes[0] + 1)
                    sanitisationCue.Ding();
                else
                    sanitisationCue.PlayCue();

                // Removing the first cue in the list as it has been played
                notes.RemoveAt(0);
            }
        }
        // Winning if there are no more notes left and a sufficient amount of beats have passed
        if (winEvent != null && songStarted && notes.Count == 0 && currentBeat >= lastNote + 12)
            winEvent.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        // Starting the song if space is pressed
        if (Input.GetKeyDown(KeyCode.Space) && !songStarted)
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

    // Function to load the notes from the note file into the list of notes
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

        // Setting the final note
        lastNote = notes[notes.Count - 1];
    }
}
