using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [Header("Game Scenes")]
    public string sanitisationScene;
    public string maskScene;
    public string keepTheDistanceScene;

    [Header("Score Texts")]
    public TextMeshProUGUI sanitisationScore;
    public TextMeshProUGUI maskScore;
    public TextMeshProUGUI keepTheDistanceScore;

    [Header("Background Objects")]
    public GameObject backgroundObject;
    public int maxBackgroundObjects = 12;
    public float backgroundObjectSpawnRadius = 11.0f;
    public float timeBetweenSpawn = 0.5f;

    List<GameObject> backgroundObjects = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        // Setting the score texts
        SetScoreTexts();

        // Beginning the background object creation
        StartCoroutine(CreateBgObjectAfterDelay());
        // TODO: ADD IN A DELAY TO DESTROY THEM OR SOMETHING
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function to switch to the Sanitisation Game
    public void PlaySanitisationGame()
    {
        SceneManager.LoadScene(sanitisationScene);
    }

    // Function to switch to the Mask Game
    public void PlayMaskGame()
    {
        SceneManager.LoadScene(maskScene);
    }

    // Function to switch to the Keep The Distance Game
    public void PlayKeepTheDistanceGame()
    {
        SceneManager.LoadScene(keepTheDistanceScene);
    }

    // Function to open the Covid safety tips site
    public void OpenCovidSafetyTips()
    {
        Application.OpenURL("https://www.health.gov.au/health-alerts/covid-19/protect-yourself-and-others");
    }

    // Function to reset the save data
    public void ResetData()
    {
        DataHandler.ResetData();
        SetScoreTexts();
    }

    // Function to set the score texts
    void SetScoreTexts()
    {
        sanitisationScore.text = ScoreToString(DataHandler.GetScore(Games.Sanitisation));
        maskScore.text = ScoreToString(DataHandler.GetScore(Games.Mask));
        keepTheDistanceScore.text = ScoreToString(DataHandler.GetScore(Games.KeepTheDistance));
    }

    // Function to turn the score into a string
    string ScoreToString(int score)
    {
        return score == 1 ? "Covid Safe!" : "Covid Unsafe";
    }

    // Function to create a background object
    [ContextMenu("Generate Object")]
    void CreateBackgroundObject()
    {
        // Calculating the radial position the object is to be spawned at
        float angle = Random.value * 2 * Mathf.PI;
        Vector2 objectPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * backgroundObjectSpawnRadius;

        // Instantiating the object
        GameObject generatedObject = Instantiate(backgroundObject);
        generatedObject.transform.position = objectPosition;
        TitleScreenBackgroundObject generatedBackgroundObject = generatedObject.GetComponent<TitleScreenBackgroundObject>();

        // Setting the object's velocity
        Vector2 velocity = -objectPosition / backgroundObjectSpawnRadius;
        velocity.x *= Random.Range(0.25f, 1.75f);
        velocity.y *= Random.Range(0.25f, 1.75f);
        generatedBackgroundObject.velocity = velocity;

        // Setting the object to be destroyed after it leaves the screen
        Destroy(generatedObject, (2 * backgroundObjectSpawnRadius) / velocity.magnitude);

        // Adding the object to the list of objects
        backgroundObjects.Add(generatedObject);
    }

    // Coroutine to create a background object
    IEnumerator CreateBgObjectAfterDelay()
    {
        while (true)
        {
            // Delaying between each spawn
            yield return new WaitForSeconds(timeBetweenSpawn * Random.Range(0.75f, 1.25f));

            // Clearing nonexistant GameObjects from the list
            foreach (GameObject backgroundObject in backgroundObjects.ToArray())
                if (backgroundObject == null)
                    backgroundObjects.Remove(backgroundObject);
            
            // Creating a background object if there are not to many already created
            if (backgroundObjects.Count < maxBackgroundObjects)
                CreateBackgroundObject();
        }
    }
}
