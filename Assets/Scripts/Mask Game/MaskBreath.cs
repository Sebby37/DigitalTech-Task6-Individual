using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskBreath : MonoBehaviour
{
    static float leniencyMultiplier = 0.2f;

    [Header("Blocking Settings")]
    public AudioClip blockSfx;

    Animator animator;
    AudioSource moveSfx;
    float secondsPerBeat;
    float currentBeatTimer = 1.0f;
    float beatsSinceCreation = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Getting components
        animator = GetComponent<Animator>();
        moveSfx = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Incrementing the beat counter
        currentBeatTimer += Time.deltaTime / secondsPerBeat;
        beatsSinceCreation += Time.deltaTime / secondsPerBeat;

        // Checking if the current beat timer has reached/exceeded 1 beat, if so resetting it and playing the sound
        if (currentBeatTimer >= 1.0f)
        {
            currentBeatTimer = 0.0f;
            moveSfx.Play();
        }

        // Destroying the breath if it has gone beyond 8.5 beats (Well beyond the input)
        if (beatsSinceCreation >= 8.5f) Destroy(gameObject);

        if (CanBlockBreath()) print($"{CanBlockBreath()} {beatsSinceCreation}");
    }

    // Function to set the BPM of the breath
    public void SetBPM(float bpm)
    {
        // Setting the secondsPerBeat
        secondsPerBeat = 60 / bpm;

        // Setting animation variables
        if (animator == null) animator = GetComponent<Animator>();
        animator.SetFloat("Beats Per Second", bpm / 60);
        animator.SetTrigger("Release");
    }

    // Function to check if the player is able to block the breath
    public bool CanBlockBreath()
    {
        return beatsSinceCreation >= 7.0 - leniencyMultiplier && beatsSinceCreation <= 7.0 + leniencyMultiplier;
    }

    // Function to check if the cue has expired
    public bool HasExpired()
    {
        return beatsSinceCreation > 7.0 + leniencyMultiplier;
    }

    // Function to block the breath
    public void BlockBreath()
    {
        // Creating an AudioSource to play the block sfx
        GameObject blockSfxObject = new GameObject("Block Breath SFX");
        AudioSource blockSfxSource = blockSfxObject.AddComponent<AudioSource>();
        blockSfxSource.clip = blockSfx;
        blockSfxSource.Play();

        // Setting the AudioSource to be destroyed after the clip is finished
        Destroy(blockSfxObject, blockSfx.length * 1.1f);

        // Destroying the breath particle
        Destroy(gameObject);
    }
}
