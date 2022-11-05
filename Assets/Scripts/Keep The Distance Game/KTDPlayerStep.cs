using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KTDPlayerStep : MonoBehaviour
{
    [Header("Song Info")]
    public float BPM = 162;

    bool onBeat = true;
    bool songStarted = false;
    float beatCounter = 0.0f;
    float secondsPerbeat;

    Animator animator;
    List<float> inputs = new List<float>();

    [Header("Inputs")]
    public UnityEvent winEvent;
    public UnityEvent missEvent;
    public float beatLeniency = 0.25f;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip stepSfx;
    public AudioClip missSfx;
    public AudioClip cowbellSfx;
    
    // Start is called before the first frame update
    void Start()
    {
        // Setting the BPM and beat counter
        BPM = KTDSongManager.BPM;

        // Setting animation variables
        animator = GetComponent<Animator>();
        secondsPerbeat = 60 / BPM;
        animator.SetFloat("Beats Per Second", BPM / 60);
    }

    // Update is called once per frame
    void Update()
    {
        // Incrementing the beat timer and current beat timer
        if (songStarted)
        {
            beatCounter += Time.deltaTime / secondsPerbeat;
        }

        // Figuring out whether the player is currently on/off beat
        onBeat = (beatCounter % 1 < 0.25f || beatCounter % 1 >= 0.75f);

        if (!songStarted && Input.GetKeyDown(KeyCode.Space)) StartCoroutine(Intro());

        // Input key press logic
        bool stepButtonPressed = Input.GetKeyDown(KeyCode.Z);

        // Player step Logic
        if (songStarted && stepButtonPressed)
        {
            // Setting animation variables and triggering step animation
            animator.SetBool("On Beat", onBeat);
            animator.SetTrigger("Step");

            // Playing the step sfx
            PlaySFX(stepSfx, randomisePitch: true);
        }

        // Hitting inputs logic
        if (inputs.Count > 0)
        {
            // Getting the latest input
            float latestInput = inputs[0];

            // Missing if the beat counter is greater than the latest input's leniency window
            // This will only be reached if the latest input hasn't been removed from the list when hit
            if (beatCounter > latestInput + beatLeniency)
            {
                Miss();
                inputs.RemoveAt(0);
            }

            // Checking if the step key was pressed within the latest input's leniency window, removing it if so
            if (stepButtonPressed && (
                beatCounter >= latestInput - beatLeniency &&
                beatCounter <= latestInput + beatLeniency
            ))
                inputs.RemoveAt(0);
        }
        // If there is only 1 input left, then the player wins as the song is basically done
        if (inputs.Count == 1)
            winEvent.Invoke();
    }

    // Function to play the intro bops
    IEnumerator Intro()
    {
        // Doing 6 bops with 2 beats between them
        for (int i = 0; i < 6; i++)
        {
            // Playing a cowbell sound if i + 1 > 4
            if (i + 1 > 4)
                PlaySFX(cowbellSfx);
            
            animator.SetTrigger("Bob");
            yield return new WaitForSeconds(2 * secondsPerbeat);
        }

        // Doing 4 bops with 1 beat between them
        for (int i = 0; i < 4; i++)
        {
            PlaySFX(cowbellSfx);
            animator.SetTrigger("Bob");
            yield return new WaitForSeconds(secondsPerbeat);
        }

        // Starting the song
        songStarted = true;
    }
    
    // Function to play a sound effect
    void PlaySFX(AudioClip sfx, bool randomisePitch = false)
    {
        // Returning if the audiosource is null
        if (audioSource == null) 
            return;

        // Setting the pitch randomly to make the sfx sound less repetitive
        audioSource.pitch = randomisePitch ? Random.Range(1.0f, 1.1f) : 1.0f;

        // Setting the clip to the SFX and playing the clip
        audioSource.clip = sfx;
        audioSource.Play();
    }

    // Function to start a song
    public void Begin(List<float> _inputs, float songBeginning)
    {
        inputs = new List<float>(_inputs);
        beatCounter = songBeginning;
        StartCoroutine(Intro());
    }

    // Function to miss
    void Miss()
    {
        PlaySFX(missSfx);
        animator.SetTrigger("Miss");
        if (missEvent != null) missEvent.Invoke();
    }
}
