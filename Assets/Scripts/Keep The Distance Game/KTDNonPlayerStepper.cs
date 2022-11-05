using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KTDNonPlayerStepper : MonoBehaviour
{
    public bool songStarted = false;
    
    Animator animator;
    float secondsPerbeat;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function to run the begin animation
    public void Begin(float _secondsPerbeat)
    {
        secondsPerbeat = _secondsPerbeat;
        StartCoroutine(Intro());
    }

    // Function to play the intro bops
    IEnumerator Intro()
    {
        // Setting the animator's seconds per beat variable
        animator.SetFloat("Beats Per Second", (60 / secondsPerbeat) / 60);

        // Doing 6 bops with 2 beats between them
        for (int i = 0; i < 6; i++)
        {
            animator.SetTrigger("Bob");
            yield return new WaitForSeconds(2 * secondsPerbeat);
        }

        // Doing 4 bops with 1 beat between them
        for (int i = 0; i < 4; i++)
        {
            animator.SetTrigger("Bob");
            yield return new WaitForSeconds(secondsPerbeat);
        }

        // Starting the song
        songStarted = true;
    }

    // Function to step
    public void Step(bool onBeat)
    {
        animator.SetBool("On Beat", onBeat);
        animator.SetTrigger("Step");
    }
}
