using UnityEngine;

public class HoldAndShoot : MonoBehaviour
{
	public Rigidbody2D rb; // Rigidbody2D of the object
	public LineRenderer lineRenderer; // LineRenderer for trajectory visualization

	public float maxLaunchSpeed = 20f; // Maximum speed the object can reach
	public float launchSpeedIncreaseRate = 5f; // Rate at which speed increases per second (normal condition)
	public float minLaunchSpeedIncreaseRate = 2f; // Rate at which speed increases per second (when Shift is held)
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

	[Header("Movement")]
	public float movSpeed = 5f;
	public bool isGrounded = false;

	// Anti-spamming mechanism variables (Cooldown for the next launch)
	public float cooldownTime = 0.5f; // Time the player needs to wait before attempting the next launch
	private float nextLaunchTime = 0f; // Time the player can attempt the next launch

	private FuelManager fuelManager; // Reference to the FuelManager script
	private Animator animator;

	private void Start()
	{
		fuelManager = GetComponent<FuelManager>(); // Get the FuelManager script
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		float hor = Input.GetAxisRaw("Horizontal");

		// Handle movement when grounded
		if (isGrounded)
		{
			transform.position += new Vector3(hor * movSpeed * Time.deltaTime, 0f, 0f);
			if (hor != 0)
			{
				animator.SetBool("move", true);

			}
			else if (hor == 0)
			{
				animator.SetBool("move", false);

			}
		}
		else
		{
		}

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

		// Handle right-click input to cancel the trajectory (Only if hold time > cooldown time)
		if (Input.GetMouseButton(1) && holdTime >= cooldownTime) // Right-click cancels trajectory if enough time passed
		{
			CancelTrajectory();
		}

		// Handle left mouse button input for shooting
		if (Input.GetMouseButton(0) && Time.time >= nextLaunchTime) // Check if cooldown has passed
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

			// Spawn the prefab at the specified position (balloon)
			if (spawnedPrefab == null && prefab != null)
			{
				spawnedPrefab = Instantiate(prefab, prefabSpawnPosition.position, Quaternion.identity, transform); // Attach to player
				spawnedPrefab.transform.localScale = Vector3.zero; // Start at zero size
			}
		}

		// Check if Shift is held down to use minLaunchSpeedIncreaseRate
		float speedIncreaseRate = Input.GetKey(KeyCode.LeftShift) ? minLaunchSpeedIncreaseRate : launchSpeedIncreaseRate;

		// Update hold time
		holdTime += Time.deltaTime;

		// Get the max allowed launch speed based on fuel
		float maxAllowedLaunchSpeed = fuelManager.GetMaxAllowedLaunchSpeed(maxLaunchSpeed);

		// Cap the current launch speed based on maxAllowedLaunchSpeed
		currentLaunchSpeed = Mathf.Min(holdTime * speedIncreaseRate, maxAllowedLaunchSpeed);

		// Calculate the launch direction based on the cursor position
		Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		cursorPosition.z = 0f; // Ignore z-axis
		launchDirection = (transform.position - cursorPosition).normalized; // Opposite to cursor
	}

	private void LaunchProjectile()
	{
		if (isHolding)
		{
			// Check fuel level before resetting velocity
			if (fuelManager.GetFuelPercentage() > 10f) // Only reset velocity if fuel > 10%
			{
				rb.linearVelocity = Vector2.zero; // Reset velocity to ignore existing force
			}

			// Apply force in the calculated launch direction with the current speed
			rb.AddForce(launchDirection * currentLaunchSpeed, ForceMode2D.Impulse);
			AudioSource aas = GetComponent<AudioSource>();
			aas.Play();


			// Consume fuel based on the actual launch speed
			fuelManager.ConsumeFuel(currentLaunchSpeed, maxLaunchSpeed);

			// Reset holding state
			isHolding = false;
			holdTime = 0f; // Reset hold time for the next launch
		}

		// Reset trajectory cancel state once launched
		isTrajectoryCanceled = false;

		// Set the time for the next possible launch (cooldown time)
		nextLaunchTime = Time.time + cooldownTime;
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

	private void CancelTrajectory()
	{
		if (!isTrajectoryCanceled && holdTime >= cooldownTime)
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

			// Spawn particle effect at prefab's position (bubble burst)
			if (particleEffect != null)
			{
				Instantiate(particleEffect, prefabSpawnPosition.position, Quaternion.identity);
			}
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
		cursorPosition.z = 0f; // Ignore z-axis

		// Calculate the direction from the rotating object to the cursor
		Vector2 direction = cursorPosition - rotatingObject.position;

		// Calculate the angle in degrees and set the rotation of the rotating object
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		rotatingObject.rotation = Quaternion.Euler(0f, 0f, angle);
	}

	private void UpdateBubblePosition()
	{
		if (spawnedPrefab != null)
		{
			// Update the position of the spawned bubble relative to the player's position
			spawnedPrefab.transform.position = prefabSpawnPosition.position + transform.position - initialPosition;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		isGrounded = true;
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		isGrounded = false;
	}
}
