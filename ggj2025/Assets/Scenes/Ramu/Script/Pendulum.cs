using UnityEngine;

public class Pendulum : MonoBehaviour
{
    // Reference to the base GameObject
    public Transform baseObject;

    // Amplitude of the oscillation (maximum angle in degrees)
    public float amplitude = 30f;

    // Speed of the oscillation
    public float speed = 2f;

    // Starting offset of the oscillation
    public float startOffset = 0f;

    private float time;

    void Start()
    {
        // Initialize the time variable with the offset
        time = startOffset;
    }

    void Update()
    {
        if (baseObject == null)
        {
            Debug.LogWarning("Base object is not assigned!");
            return;
        }

        // Calculate the oscillation angle using a sine wave
        float angle = amplitude * Mathf.Sin(time * speed);

        // Rotate the pendulum around the base's position
        transform.position = baseObject.position; // Keep the pendulum anchored to the base
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Increment time
        time += Time.deltaTime;
    }
}
