using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Using an enum for the cue state handling
public enum SanitisationCueState
{
    Ding,
    Press,
    BeginPlayerInput,
    EndPlayerInput
}

public class SanitisationCue : MonoBehaviour
{
    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip missSFX;
    public AudioClip dingSFX;
    public AudioClip pressSFX;
    public AudioClip sanitiserSFX;

    [Header("Song Objects")]
    public Animator songObjectAnimator;
    public GameObject elevatorButton;
    public GameObject hand;
    public GameObject sanitiser;

    [Header("Missing")]
    public UnityEvent missEvent;

    // Cue variables
    float cueTime = 0;
    bool cuePlaying = false;
    bool playerCanInput = false;
    SanitisationCueState? currentCue = null;

    // Timing variables
    float BPM;
    float secondsPerBeat;
    float beatsPerSecond;
    float leniencyMultiplier = 0.2f; // Is multiplied by secondsPerBeat and is the window in which the player can input, starting from -value and ending at +value

    // Start is called before the first frame update
    void Start()
    {
        ChangeBPM(BPM);
    }

    // FixedUpdate is called prior to physics calculations
    private void FixedUpdate()
    {
        if (cuePlaying)
        {
            // Adding the time in seconds from the previous frame divided by the seconds per beat
            // This basically adds 1 beat per second to the cue counter
            cueTime += Time.deltaTime / secondsPerBeat;

            // Sound and animation management for the cue
            switch (currentCue)
            {
                case SanitisationCueState.Ding:
                    if (cueTime >= 0)
                    {
                        PlaySFX(dingSFX);
                        songObjectAnimator.SetTrigger("Ding");
                        currentCue = SanitisationCueState.Press;
                    }
                    break;
                case SanitisationCueState.Press:
                    if (cueTime >= 1)
                    {
                        PlaySFX(pressSFX);
                        songObjectAnimator.SetTrigger("Press");
                        currentCue = SanitisationCueState.BeginPlayerInput;
                    }
                    break;
                case SanitisationCueState.BeginPlayerInput:
                    if (cueTime >= 2 - leniencyMultiplier)
                    {
                        // Begin player input
                        playerCanInput = true;
                        songObjectAnimator.SetTrigger("Open Hand");
                        currentCue = SanitisationCueState.EndPlayerInput;
                    }
                    break;
                case SanitisationCueState.EndPlayerInput:
                    if (cueTime >= 2 + leniencyMultiplier && playerCanInput)
                    {
                        // Player has missed
                        if (missEvent != null)
                            missEvent.Invoke();
                        PlaySFX(missSFX);
                        songObjectAnimator.SetTrigger("Miss");
                        StopCue();
                    }
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {   
        // Player input check
        if (playerCanInput)
        {
            // Player has hit the note
            if (Input.GetKeyDown(KeyCode.Z))
            {
                // Play hand sanitiser squirt animation
                songObjectAnimator.SetTrigger("Sanitiser Squirt");
                PlaySFX(sanitiserSFX);
                StopCue();
            }
        }
    }

    // Function called when a cue is to be played
    public void PlayCue()
    {
        cueTime = 0;
        cuePlaying = true;
        currentCue = SanitisationCueState.Ding;
    }

    // Function called when a cue is to be stopped
    void StopCue()
    {
        cueTime = 0;
        cuePlaying = false;
        currentCue = null;
        playerCanInput = false;
    }

    // Function to change the BPM
    public float ChangeBPM(float newBPM)
    {
        BPM = newBPM;
        secondsPerBeat = 60 / BPM;
        beatsPerSecond = BPM / 60;
        songObjectAnimator.SetFloat("Beats Per Second", beatsPerSecond);

        return secondsPerBeat;
    }

    // Function to play SFX as an audioclip at the SFX Source
    void PlaySFX(AudioClip sfx)
    {
        sfxSource.clip = sfx;
        sfxSource.Play();
    }

    // Function simply to do the Ding sfx
    public void Ding()
    {
        PlaySFX(dingSFX);
        songObjectAnimator.SetTrigger("Ding");
    }
}
