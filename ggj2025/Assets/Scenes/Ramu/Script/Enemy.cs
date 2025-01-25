using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject pointA; // First GameObject (Point A)
    public GameObject pointB; // Second GameObject (Point B)
    public float moveDuration = 3f; // Duration to move between points

    private float journeyTime; // Time elapsed during movement
    private bool movingToPointB = true; // Direction flag

    void Start()
    {
        journeyTime = 0f; // Initialize the journey time
    }

    void Update()
    {
        MoveBetweenPoints();
    }

    void MoveBetweenPoints()
    {
        // Check if both points are assigned
        if (pointA != null && pointB != null)
        {
            // Get the start and end points
            Vector3 startPoint = movingToPointB ? pointA.transform.position : pointB.transform.position;
            Vector3 endPoint = movingToPointB ? pointB.transform.position : pointA.transform.position;

            // Calculate the progress as a value between 0 and 1
            journeyTime += Time.deltaTime;
            float journeyFraction = journeyTime / moveDuration;

            // Move the enemy towards the target point using Lerp
            transform.position = Vector3.Lerp(startPoint, endPoint, journeyFraction);

            // Check if the enemy has reached the target point
            if (journeyFraction >= 1f)
            {
                // Reset journey time and switch direction
                journeyTime = 0f;
                movingToPointB = !movingToPointB;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the enemy collides with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.DecreaseHealth(10); // Decrease health by 10
            }
        }
    }
}
