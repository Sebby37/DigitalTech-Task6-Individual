using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MaskCue : MonoBehaviour
{
    [Header("Prefab Info")]
    public GameObject breathParticle;
    [SerializeField] Animator playerAnimator;
    List<MaskBreath> breathParticles = new List<MaskBreath>();

    [Header("Song Info")]
    public float BPM = 120;

    [Header("SFX")]
    public AudioSource whooshSound;

    [Header("Scoring")]
    public UnityEvent missEvent;

    float beatsPerSecond;
    float secondsPerBeat;

    // Start is called before the first frame update
    void Start()
    {
        // Setting the BPM
        ChangeBPM(BPM);
    }

    // Update is called once per frame
    void Update()
    {
        // Handling player input
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Playing the block breath animation
            playerAnimator.SetTrigger("Block Breath");

            // Playing the whoosh sound
            whooshSound.Play();

            // Looping through each breath particle to see if it can be blocked, blocking it if so
            foreach (MaskBreath breath in breathParticles.ToArray())
                if (breath != null)
                    if (breath.CanBlockBreath())
                    {
                        breath.BlockBreath();
                        breathParticles.Remove(breath);
                    }
        }

        // Handling missing
        foreach (MaskBreath breath in breathParticles.ToArray())
            if (breath != null && missEvent != null && breath.HasExpired())
            {
                missEvent.Invoke();
                breathParticles.Remove(breath);
            }
    }

    // Function to change the BPM
    public float ChangeBPM(float newBpm)
    {
        BPM = newBpm;
        beatsPerSecond = BPM / 60;
        secondsPerBeat = 60 / BPM;

        // Returning secondsPerBeat for use with the song manager
        return secondsPerBeat;
    }

    // Function to play a cue
    public void PlayCue()
    {
        // Instantiating the new cue
        GameObject newCue = Instantiate(breathParticle, Vector2.zero, Quaternion.identity);

        // Getting the mask breath component and setting it's BPM
        MaskBreath maskBreath = newCue.GetComponent<MaskBreath>();
        maskBreath.SetBPM(BPM);

        // Adding the mask breath to the list
        breathParticles.Add(maskBreath);
    }

    // Function to bob to the beat
    public void BobToBeat()
    {
        // Disabled for now as the bob animation interrupts the block breath animation
        // Also for some reason the timing kinda sucks and it goes out of sync with the song
        //playerAnimator.SetTrigger("Bob");
    }
}
