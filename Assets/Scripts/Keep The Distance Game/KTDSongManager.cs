using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KTDSongManager : MonoBehaviour
{
    public static bool onBeat = true;

    [Header("Song Info")]
    public static float BPM = 162;
    public string beatChangesFile;
    public AudioSource dingSource;
    public AudioSource stepSource;
    public AudioSource musicSource;
    public float songBeginning = 0.0f;
    public float songEnding = 0.0f;

    float secondsPerBeat;
    float beatCounter = 0.0f;
    bool songStarted = false;

    KTDPlayerStep player;
    List<KTDNonPlayerStepper> steppers;
    List<float> inputs = new List<float>();
    List<float> beatChanges = new List<float>();

    [Header("SFX Info")]
    public float dingLowPitch = 0.975f;
    public float dingHighPitch = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Calculating seconds per beat
        secondsPerBeat = 60.0f / BPM;

        // Populating the list of steppers
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<KTDPlayerStep>();
        KTDNonPlayerStepper[] steppersArray = (KTDNonPlayerStepper[]) FindObjectsOfType(typeof(KTDNonPlayerStepper));
        steppers = new List<KTDNonPlayerStepper>(steppersArray);

        // Loading the beat changes from the chart file
        LoadBeatChangesFromFile();
    }

    // Update is called once per frame
    void Update()
    {
        // Incrementing the beat counter
        if (songStarted)
        {
            beatCounter += Time.deltaTime / secondsPerBeat;
        }

        // Checking if there is a beat change, changing the beat if so
        if (songStarted && beatChanges.Count > 0)
        {
            float latestBeatChange = beatChanges[0];
            if (beatCounter >= latestBeatChange - (onBeat ? 3.5f : 2.0f))
            {
                ChangeBeat();
                beatChanges.RemoveAt(0);
            }
        }

        // Checking if an input is needed to be hit, stepping the crowd it if so
        if (songStarted && inputs.Count > 0)
        {
            float currentInput = inputs[0];
            if (beatCounter >= currentInput)
            {
                StepAllSteppers();
                inputs.RemoveAt(0);
            }
        }

        // Starting the song on space input
        if (!songStarted && Input.GetKeyDown(KeyCode.Space))
            StartSong();
    }

    // Function to start the song
    void StartSong()
    {
        // Setting the song started variable to true
        songStarted = true;

        // Beginning the song
        musicSource.Play();
        
        // Calling the Begin() function on all of the steppers
        foreach (KTDNonPlayerStepper stepper in steppers)
            stepper.Begin(secondsPerBeat);

        // Calling the begin function on the player
        player.Begin(inputs, songBeginning);
    }

    // Function to load beat changes from a file
    void LoadBeatChangesFromFile()
    {
        // Reading each line of the notes file into a string array
        string[] rawFileContents = File.ReadAllLines(beatChangesFile);

        // Looping through each line of the file
        foreach (string raw in rawFileContents)
        {
            // Trying to parse the current line into a float, continuing if it cannot
            float beatChangeTime;
            if (!float.TryParse(raw, out beatChangeTime)) continue;
            beatChanges.Add(beatChangeTime);
        }

        // Adding an input to the list for each on/off beat
        int beatChangeIndex = 0;
        for (float beatCounter = songBeginning; beatCounter <= songEnding;)
        {
            inputs.Add(beatCounter);
            
            if (beatChangeIndex < beatChanges.Count && beatCounter >= beatChanges[beatChangeIndex] - 1)
            {
                beatChangeIndex++;
                beatCounter -= 0.5f;
            }

            beatCounter += 1.0f;
        }
    }

    // Function to change the beat
    void ChangeBeat()
    {
        // Running the specific corutine for off/on beat
        StartCoroutine(onBeat ? SwitchOffBeat() : SwitchOnBeat());
    }

    // Coroutine to switch to offbeat
    IEnumerator SwitchOffBeat()
    {
        dingSource.pitch = dingLowPitch;
        dingSource.Play();
        yield return new WaitForSeconds(1 * secondsPerBeat);
        dingSource.Play();
        yield return new WaitForSeconds(1 * secondsPerBeat);
        dingSource.Play();
        yield return new WaitForSeconds(1 * secondsPerBeat);
        dingSource.Play();
        yield return new WaitForSeconds(0.5f * secondsPerBeat);
        dingSource.pitch = dingHighPitch;
        dingSource.Play();
        onBeat = false;
    }

    // Coroutine to switch to onbeat
    IEnumerator SwitchOnBeat()
    {
        dingSource.pitch = dingHighPitch;
        dingSource.Play();
        yield return new WaitForSeconds(0.5f * secondsPerBeat);
        dingSource.pitch = dingLowPitch;
        dingSource.Play();
        yield return new WaitForSeconds(0.5f * secondsPerBeat);
        dingSource.pitch = dingHighPitch;
        dingSource.Play();
        yield return new WaitForSeconds(0.5f * secondsPerBeat);
        dingSource.pitch = dingLowPitch;
        dingSource.Play();
        yield return new WaitForSeconds(0.5f * secondsPerBeat);
        dingSource.Play();
        onBeat = true;
    }

    // Function to make each stepper step
    void StepAllSteppers()
    {
        stepSource.Play();

        foreach (KTDNonPlayerStepper stepper in steppers)
            if (stepper.songStarted)
                stepper.Step((beatCounter % 1 < 0.25f || beatCounter % 1 >= 0.75f));
    }
}
