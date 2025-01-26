using UnityEngine;

public class AIEnemyPatrol : MonoBehaviour
{
	public Transform pointA; // First patrol point
	public Transform pointB; // Second patrol point
	public float speed = 2f; // Movement speed
	public bool flipX = true; // Option to flip X axis
	public bool flipY = false; // Option to flip Y axis

	private Transform targetPoint;

	void Start()
	{
		if (pointA == null || pointB == null)
		{
			Debug.LogError("Please assign both pointA and pointB transforms.");
			return;
		}

		// Start moving towards pointA initially
		targetPoint = pointB;
	}

	void Update()
	{
		if (pointA == null || pointB == null) return;

		// Move the enemy towards the target point
		transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

		// Flip sprite direction based on movement direction (if applicable)
		Vector3 scale = transform.localScale;

		// Flip X axis if needed
		if (flipX)
		{
			scale.x = transform.position.x < targetPoint.position.x ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
		}

		// Flip Y axis if needed
		if (flipY)
		{
			scale.y = transform.position.y < targetPoint.position.y ? Mathf.Abs(scale.y) : -Mathf.Abs(scale.y);
		}

		transform.localScale = scale;

		// Check if the enemy has reached the target point
		if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
		{
			// Switch to the other point
			targetPoint = targetPoint == pointA ? pointB : pointA;
		}
	}
}
