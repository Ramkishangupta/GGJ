using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Movement speed of the enemy
    public float moveDuration = 5f; // Duration to move in one direction

    private float moveTimer; // Timer to track movement
    private bool movingForward = true; // Direction of movement
    private Transform player; // Reference to the player's transform

    void Start()
    {
        moveTimer = moveDuration;
    }

    void Update()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        // Update the timer
        moveTimer -= Time.deltaTime;

        // Check if it's time to switch direction
        if (moveTimer <= 0)
        {
            movingForward = !movingForward; // Reverse direction
            moveTimer = moveDuration; // Reset the timer
        }

        // Move the enemy
   Vector2 direction = movingForward ? Vector2.right : Vector2.left;
transform.Translate(direction * moveSpeed * Time.deltaTime);

    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the enemy collides with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerDies(collision.gameObject);
        }
    }

    private void PlayerDies(GameObject player)
    {
        // Handle player's death (e.g., restart level, display game over, etc.)
        Debug.Log("Player has died!");
        // Example: Destroy the player object
        Destroy(player);
    }
}
