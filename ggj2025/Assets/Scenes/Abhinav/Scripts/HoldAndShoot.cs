using UnityEngine;

public class HoldAndShoot : MonoBehaviour
{
	public Rigidbody2D rb; // Rigidbody2D of the object
	public LineRenderer lineRenderer; // LineRenderer for trajectory visualization

	public float maxLaunchSpeed = 20f; // Maximum speed the object can reach
	public float launchSpeedIncreaseRate = 5f; // Rate at which speed increases per second
	public float maxHoldTime = 3f; // Maximum time the mouse button can be held
	public float gravityScale = 1f; // Optional gravity scale for trajectory
	public int numberOfPoints = 20; // Number of points to render the trajectory

	private float currentLaunchSpeed = 0f; // Current speed increasing with hold time
	private float holdTime = 0f; // Time the mouse button has been held
	private Vector3 initialPosition; // Position where the launch starts
	private Vector2 launchDirection; // Direction of the launch

	private bool isHolding = false; // Whether the mouse is being held
	public bool isJumping = false;

	public Transform rotatingObject; // The object that will rotate toward the cursor

	[Header("Prefab and Effects")]
	public GameObject prefab; // The prefab to spawn
	public Transform prefabSpawnPosition; // The position where the prefab will spawn
	public GameObject particleEffect; // Particle effect to spawn after destroying the prefab
	private GameObject spawnedPrefab; // Reference to the spawned prefab

	private void Update()
	{
		if (rb.linearVelocity.y == 0)
		{
			rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
			isJumping = false;
		}
		else if (rb.linearVelocity.y > 0)
		{
			isJumping = true;
		}
		if (Input.GetMouseButton(1) && rb.linearVelocity.y == 0)
		{
			initialPosition = transform.position;
		}
		// Handle mouse button input
		if (Input.GetMouseButton(0) && isJumping == false)
		{
			HandleHold();
			lineRenderer.enabled = true;

			// Spawn and grow the prefab
			HandlePrefabGrowth();
		}
		else
		{
			lineRenderer.enabled = false;
		}

		// Rotate the rotating object toward the cursor
		RotateObjectToCursor();

		// Automatically launch if max hold time is reached
		if (isHolding && holdTime >= maxHoldTime && isJumping == false)
		{
			LaunchProjectile();
			DestroyPrefabWithEffect();
		}

		if (Input.GetMouseButtonUp(0))
		{
			LaunchProjectile();
			DestroyPrefabWithEffect();
		}

		// Draw the trajectory while holding the mouse button
		if (isHolding)
		{
			DrawTrajectory();
		}
	}

	private void HandleHold()
	{
		// Start holding if not already
		if (!isHolding)
		{
			isHolding = true;
			holdTime = 0f; // Reset hold time
			currentLaunchSpeed = 0f; // Reset launch speed
			initialPosition = transform.position; // Store the initial position

			// Spawn the prefab at the specified position
			if (spawnedPrefab == null && prefab != null)
			{
				spawnedPrefab = Instantiate(prefab, prefabSpawnPosition.position, Quaternion.identity);
				spawnedPrefab.transform.localScale = Vector3.zero; // Start at zero size
			}
		}

		// Update hold time and launch speed
		holdTime += Time.deltaTime;
		currentLaunchSpeed = Mathf.Min(holdTime * launchSpeedIncreaseRate, maxLaunchSpeed);

		// Calculate the launch direction based on the cursor position
		Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		cursorPosition.z = 0f; // Ignore z-axis
		launchDirection = (transform.position - cursorPosition).normalized; // Opposite to cursor
	}

	private void HandlePrefabGrowth()
	{
		if (spawnedPrefab != null)
		{
			// Calculate the scale factor based on hold time
			float scaleFactor = Mathf.Min(holdTime / maxHoldTime, 1f); // Clamp between 0 and 1
			spawnedPrefab.transform.localScale = Vector3.one * scaleFactor; // Scale uniformly
		}
	}

	private void DestroyPrefabWithEffect()
	{
		if (spawnedPrefab != null)
		{
			// Destroy the prefab
			Destroy(spawnedPrefab);

			// Spawn particle effect at prefab's position
			if (particleEffect != null)
			{
				Instantiate(particleEffect, prefabSpawnPosition.position, Quaternion.identity);
			}
		}
	}

	private void LaunchProjectile()
	{
		if (isHolding)
		{
			// Apply force in the calculated launch direction with the current speed
			rb.AddForce(launchDirection * currentLaunchSpeed, ForceMode2D.Impulse);

			// Reset holding state
			isHolding = false;
			holdTime = 0f; // Reset hold time for the next launch
		}
	}

	private void DrawTrajectory()
	{
		// Set up the trajectory points array
		Vector3[] points = new Vector3[numberOfPoints];

		// Calculate the initial velocity components
		Vector2 velocity = launchDirection * currentLaunchSpeed;

		// Loop to calculate the trajectory path at different time steps
		for (int i = 0; i < numberOfPoints; i++)
		{
			float t = i / (float)(numberOfPoints - 1); // Time factor from 0 to 1
			float x = velocity.x * t; // X position based on velocity
			float y = velocity.y * t - 0.5f * gravityScale * Mathf.Abs(Physics2D.gravity.y) * t * t; // Y position with gravity

			// Add the object's initial position to the trajectory point
			points[i] = initialPosition + new Vector3(x, y, 0); // Use initial position for the trajectory path
		}

		// Set the LineRenderer's positions to the trajectory points
		lineRenderer.positionCount = points.Length;
		lineRenderer.SetPositions(points);
	}

	private void RotateObjectToCursor()
	{
		// Get the cursor position in world space
		Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		cursorPosition.z = 0f; // Ignore the z-axis

		// Calculate the direction from the rotating object to the cursor
		Vector2 direction = cursorPosition - rotatingObject.position;

		// Calculate the angle in degrees and set the rotation of the rotating object
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		rotatingObject.rotation = Quaternion.Euler(0f, 0f, angle + 180);
	}
}
