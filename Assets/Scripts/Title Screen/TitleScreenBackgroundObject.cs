using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenBackgroundObject : MonoBehaviour
{
    [Header("Sprites")]
    public SpriteRenderer spriteRenderer;
    public List<Sprite> sprites = new List<Sprite>();

    [Header("Movement Behaviour")]
    public float maxSpeed = 4;
    public float angularVelocityRange = Mathf.PI / 4;
    public Vector3 velocity;

    float angularVelocity = 0.0f;

    // Start is called before the first frame update
    [ContextMenu("Reset")]
    void Start()
    {
        // Setting the sprite to a random choice of the list
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];

        // Setting the angular velocity
        angularVelocity = Random.Range(-angularVelocityRange, angularVelocityRange);

        // Setting the object to be destroyed after it leaves the screen

    }

    // Update is called once per frame
    void Update()
    {
        // Rotating based on angluar velocity
        spriteRenderer.gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, angularVelocity) * Time.deltaTime);

        // Translating towards the origin
        transform.Translate(velocity * Time.deltaTime);
    }
}
