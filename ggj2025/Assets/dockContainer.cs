using UnityEngine;

public class dockContainer : MonoBehaviour
{
	public Transform pointA; // First point
	public Transform pointB; // Second point
	public float speed = 2f; // Movement speed

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

		// Move the platform towards the target point
		transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

		// Check if the platform has reached the target point
		if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
		{
			// Switch to the other point
			targetPoint = targetPoint == pointA ? pointB : pointA;
		}
	}
}