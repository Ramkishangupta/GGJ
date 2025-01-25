using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public GameObject lavaBubblePrefab; // Prefab for the lava bubble
    public Transform spawnPoint; // Point where the bubbles are spawned
    public float spawnInterval = 2f; // Time interval between bubble releases
    public float bubbleSpeed = 5f; // Speed at which the bubbles move

    private float nextSpawnTime;

    void Update()
    {
        ReleaseLavaBubbles();
    }

    void ReleaseLavaBubbles()
    {
        if (Time.time >= nextSpawnTime)
        {
            Debug.Log("Spawning Lava Bubble");

            GameObject bubble = Instantiate(lavaBubblePrefab, spawnPoint.position, Quaternion.identity);

            Rigidbody rb = bubble.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = new Vector3(bubbleSpeed, 0f, 0f);
                Debug.Log("Bubble moving at velocity: " + rb.velocity);
            }
            else
            {
                Debug.LogError("Rigidbody missing on lavaBubblePrefab");
            }

            // Destroy the bubble after 5 seconds
            Destroy(bubble, 5f);

            nextSpawnTime = Time.time + spawnInterval;
        }
    }
}

