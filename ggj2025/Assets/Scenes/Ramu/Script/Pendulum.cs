using UnityEngine;

public class Pendulum : MonoBehaviour
{
	public Transform pivot; // The pivot point
	public float oscillationAngle = 45f; // Maximum angle of oscillation
	public float oscillationSpeed = 2f; // Speed of oscillation
	public float offsetAngle = 0f; // Offset angle around which the oscillation occurs

	private float currentAngle; // Current angle of oscillation

	void Start()
	{
		if (pivot == null)
		{
			Debug.LogError("Please assign a pivot Transform.");
		}
	}

	void Update()
	{
		if (pivot == null) return;

		// Calculate the current angle using a sine wave and apply the offset
		currentAngle = offsetAngle + oscillationAngle * Mathf.Sin(Time.time * oscillationSpeed);

		// Rotate the object around the pivot
		transform.position = pivot.position;
		transform.rotation = Quaternion.Euler(0, 0, currentAngle);
	}
}
