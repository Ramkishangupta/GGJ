using UnityEngine;

public class MovingPlatformUD : MonoBehaviour
{
	public Transform pointA; // Transform for the starting point
	public Transform pointB; // Transform for the ending point
	public float speed = 2f; // Movement speed of the platform

	private Transform targetPoint; // The current target point
	private bool isPlayerOnPlatform = false; // Tracks if the player is on the platform

	void Start()
	{
		if (pointA == null || pointB == null)
		{
			Debug.LogError("Please assign both pointA and pointB Transforms.");
			return;
		}

		// Initialize the platform to start at pointA
		transform.position = pointA.position;

		// Set the initial target point
		targetPoint = pointA;
	}

	void Update()
	{
		if (pointA == null || pointB == null) return;

		// Determine target point based on player presence
		if (isPlayerOnPlatform)
		{
			targetPoint = pointB; // Move to pointB when the player is on the platform
		}
		else
		{
			targetPoint = pointA; // Move back to pointA when the player leaves
		}

		// Move the platform towards the current target point
		transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Check if the collided object is the player
		if (collision.collider.CompareTag("Player"))
		{
			// Set the player as a child of the platform so it moves with it
			collision.collider.transform.SetParent(transform);

			// Indicate the player is on the platform
			isPlayerOnPlatform = true;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		// Detach the player from the platform when they leave it
		if (collision.collider.CompareTag("Player"))
		{
			collision.collider.transform.SetParent(null);

			// Indicate the player is no longer on the platform
			isPlayerOnPlatform = false;
		}
	}
}
