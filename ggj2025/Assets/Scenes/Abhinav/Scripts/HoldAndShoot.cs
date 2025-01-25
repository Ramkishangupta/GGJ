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
	private bool isTrajectoryCanceled = false; // State to track if the trajectory is canceled
	public bool isJumping = false;

	public Transform rotatingObject; // The object that will rotate toward the cursor

	[Header("Prefab and Effects")]
	public GameObject prefab; // The prefab to spawn (balloon)
	public Transform prefabSpawnPosition; // The position where the prefab will spawn
	public GameObject particleEffect; // Particle effect to spawn after destroying the prefab
	private GameObject spawnedPrefab; // Reference to the spawned prefab (balloon)

	public LayerMask groundLayer; // Ground layer to check if player is grounded

	// Cooldown variables
	public float spawnCooldown = 1f; // Time between spawns (in seconds)
	private float lastSpawnTime = 0f; // Time of the last spawn

	private void Update()
	{
		// Update the player's jumping state
		if (rb.linearVelocity.y == 0)
		{
			rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
			isJumping = false;
		}
		else if (rb.linearVelocity.y > 0)
		{
			isJumping = true;
		}

		// Handle right-click input to cancel the trajectory
		if (Input.GetMouseButton(1)) // Right-click cancels trajectory
		{
			CancelTrajectory();
		}

		// Handle left mouse button input for shooting
		if (Input.GetMouseButton(0))
		{
			if (!isTrajectoryCanceled) // Only handle left-click if trajectory is not canceled
			{
				HandleHold();
				// Only enable the LineRenderer if grounded
				if (!isJumping)
				{
					lineRenderer.enabled = true;
				}

				// Spawn and grow the prefab
				HandlePrefabGrowth();
			}
		}
		else
		{
			lineRenderer.enabled = false; // Disable LineRenderer when not holding left-click
		}

		// Rotate the rotating object toward the cursor
		RotateObjectToCursor();

		// Automatically launch if max hold time is reached
		if (isHolding && holdTime >= maxHoldTime)
		{
			LaunchProjectile();
			DestroyPrefabWithEffect();
		}

		if (Input.GetMouseButtonUp(0)) // Left mouse button released
		{
			LaunchProjectile();
			DestroyPrefabWithEffect();
		}

		// Draw the trajectory while holding the mouse button
		if (isHolding)
		{
			DrawTrajectory();
		}

		// Make sure the spawned bubble stays at the correct position relative to the player
		UpdateBubblePosition();
	}

	private void HandleHold()
	{
		// Start holding if not already
		if (!isHolding)
		{
			isHolding = true;
			holdTime = 0f; // Reset hold time
			currentLaunchSpeed = 0f; // Reset launch speed
			initialPosition = transform.position; // Store the initial position of the player

			// Check cooldown before spawning the prefab
			if (Time.time - lastSpawnTime >= spawnCooldown)
			{
				// Spawn the prefab at the specified position (balloon)
				if (spawnedPrefab == null && prefab != null)
				{
					spawnedPrefab = Instantiate(prefab, prefabSpawnPosition.position, Quaternion.identity, transform); // Attach to player
					spawnedPrefab.transform.localScale = Vector3.zero; // Start at zero size

					// Update the last spawn time
					lastSpawnTime = Time.time;
				}
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

	// Cancel the trajectory when the right-click is pressed
	private void CancelTrajectory()
	{
		if (!isTrajectoryCanceled)
		{
			isTrajectoryCanceled = true; // Set the trajectory cancel state
			isHolding = false; // Reset the holding state
			holdTime = 0f; // Reset hold time
			currentLaunchSpeed = 0f; // Reset the launch speed
			lineRenderer.enabled = false; // Disable the trajectory visualization

			// Destroy the spawned prefab if it exists
			if (spawnedPrefab != null)
			{
				Destroy(spawnedPrefab);
			}
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

		// Reset trajectory cancel state once launched
		isTrajectoryCanceled = false;
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

			// Update the trajectory position based on the player's current position
			initialPosition = transform.position; // Keep updating initial position to follow the player
			points[i] = initialPosition + new Vector3(x, y, 0); // Use current position for the trajectory path
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

	// Ensure the balloon stays at the correct spawn position relative to the player
	private void UpdateBubblePosition()
	{
		if (spawnedPrefab != null)
		{
			// Update the position of the spawned bubble relative to the player's position
			spawnedPrefab.transform.position = prefabSpawnPosition.position + transform.position - initialPosition;
		}
	}

	// Ground detection using Raycast to determine if the player is grounded
	private bool IsGrounded()
	{
		// Cast a ray downward from the player's position to check for ground collision
		return Physics2D.Raycast(transform.position, Vector2.down, 0.2f, groundLayer);
	}
}
