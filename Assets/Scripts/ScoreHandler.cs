using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    public static string menuScene = "Menu Screen";

    [Header("Config")]
    public string game;
    public int missesLeft = 3;
    public AudioSource missSource;
    public List<Image> images = new List<Image>(); // This is ordered from low to high (ie: low = 0, med = 1, high = 2)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Returning to the menu if the escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(menuScene);
    }

    // Function to miss
    [ContextMenu("Trigger Miss")]
    public void Miss()
    {
        // Setting the current health images to black out
        images[missesLeft - 1].color = Color.black;

        // Playing the miss SFX
        missSource.Play();
        
        // Subtracting the amount of misses left
        missesLeft --;

        // Losing if there are no more misses left
        if (missesLeft <= 0)
            Lose();
    }

    // Function to lose
    public void Lose()
    {
        DataHandler.SetScore(game, 0);
        SceneManager.LoadScene(menuScene);
    }

    // Function to win
    public void Win()
    {
        DataHandler.SetScore(game, 1);
        SceneManager.LoadScene(menuScene);
    }
}
