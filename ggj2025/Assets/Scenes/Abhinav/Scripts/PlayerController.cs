using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Player Settings")]
	public float moveSpeed = 5f;               // Movement speed for left and right movement
	public GameObject bubblePrefab;           // Prefab of the bubble (just for shooting visuals)
	public Transform gunTransform;            // The entire gun object to rotate
	public float launchForce = 10f;            // Force applied to the player when shot
	public float maxProjectileForce = 20f;    // Max force for better control
	public int trajectoryPoints = 50;          // Number of points to display in trajectory line

	private Rigidbody2D rb;                   // Player's Rigidbody2D for applying force
	private LineRenderer lineRenderer;        // LineRenderer to draw the trajectory
	private Vector2 launchDirection;          // Direction of the applied force
	private Vector2 initialVelocity;          // Initial velocity when the player is shot

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		lineRenderer = GetComponent<LineRenderer>();

		// Initialize the LineRenderer
		lineRenderer.positionCount = 0; // Start with no points
	}

	void Update()
	{
		// Handle player movement
		float moveInput = Input.GetAxis("Horizontal");
		rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

		// Rotate gun to face mouse position
		RotateGunToMouse();

		// Handle bubble shooting and trajectory line
		if (Input.GetMouseButtonDown(0))
		{
			ShootBubble();
			ShowTrajectory(); // Show trajectory path when shooting
		}

		// Clear trajectory line after shooting
		if (Input.GetMouseButtonUp(0))
		{
			ClearTrajectory();
		}
	}

	void RotateGunToMouse()
	{
		// Get mouse position in world space
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = 0; // Ensure it's in 2D space

		// Calculate the direction from the gun to the mouse
		Vector2 direction = (mousePosition - gunTransform.position).normalized;

		// Calculate the rotation angle
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		// Apply the rotation to the gun
		gunTransform.rotation = Quaternion.Euler(0, 0, angle);
	}

	void ShootBubble()
	{
		// Instantiate the bubble at the gun's position
		Instantiate(bubblePrefab, gunTransform.position, gunTransform.rotation);

		// Calculate the direction of the shot
		launchDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

		// Apply the force to the player's Rigidbody2D in the direction of the mouse
		initialVelocity = launchDirection * launchForce;
		rb.linearVelocity = initialVelocity; // Set initial velocity directly
	}

	void ShowTrajectory()
	{
		// Set LineRenderer settings
		lineRenderer.positionCount = trajectoryPoints;  // We will draw 'trajectoryPoints' number of points

		// Calculate and display the trajectory points
		Vector2 startPosition = rb.position;
		Vector2 velocity = initialVelocity;

		for (int i = 0; i < trajectoryPoints; i++)
		{
			float time = i * 0.1f; // Time intervals for each point
			Vector2 trajectoryPoint = startPosition + velocity * time + 0.5f * Physics2D.gravity * time * time;
			lineRenderer.SetPosition(i, trajectoryPoint);
		}
	}

	void ClearTrajectory()
	{
		// Clear the trajectory line after shooting
		lineRenderer.positionCount = 0;
	}
}
