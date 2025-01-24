using UnityEngine;

public class TrajectoryTest : MonoBehaviour
{
	public Rigidbody2D rb;
	public LineRenderer lineRenderer;
	public float launchSpeed = 10f;  // Initial speed for the projectile
	public float launchAngle = 45f;  // Launch angle in degrees
	public int numberOfPoints = 20;  // Number of points to render the trajectory
	public float gravityScale = 1f;  // Optional gravity scale to adjust gravity effect
	private Vector3 initialPosition;

	public bool isJumping = false;
	private void Start()
	{
		// Launch the object with the specified force

		// Draw the trajectory using LineRenderer
	}
	private void Update()
	{
		if(rb.linearVelocity.y == 0)
		{
			rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
			isJumping = false ;
		}
		else if(rb.linearVelocity.y > 0)
		{
			isJumping = true ;
		}
		if (Input.GetMouseButton(1) && rb.linearVelocity.y == 0)
		{
			initialPosition = transform.position;

		}

		DrawTrajectory();

		if (Input.GetMouseButtonDown(0))
		{
			LaunchProjectile();

		}
	}
	void LaunchProjectile()
	{
		// Convert angle to radians
		float angleRad = launchAngle * Mathf.Deg2Rad;

		// Calculate initial velocity components
		Vector2 initialVelocity = new Vector2(Mathf.Cos(angleRad) * launchSpeed, Mathf.Sin(angleRad) * launchSpeed);

		// Apply the force to the Rigidbody2D
		rb.AddForce(initialVelocity, ForceMode2D.Impulse);


	}

	void DrawTrajectory()
	{
		// Set up the trajectory points array
		Vector3[] points = new Vector3[numberOfPoints];

		// Calculate the initial velocity components
		float angleRad = launchAngle * Mathf.Deg2Rad;
		Vector2 velocity = new Vector2(Mathf.Cos(angleRad) * launchSpeed, Mathf.Sin(angleRad) * launchSpeed);

		// Loop to calculate the trajectory path at different time steps
		for (int i = 0; i < numberOfPoints; i++)
		{
			float t = i / (float)(numberOfPoints - 1);  // Time factor from 0 to 1
			float x = velocity.x * t;  // X position based on velocity
			float y = velocity.y * t - 0.5f * gravityScale * Mathf.Abs(Physics2D.gravity.y) * t * t;  // Y position with gravity

			// Add the object's initial position to the trajectory point
			points[i] = initialPosition + new Vector3(x, y, 0);  // Use initial position for the trajectory path
		}

		// Set the LineRenderer's positions to the trajectory points
		lineRenderer.positionCount = points.Length;
		lineRenderer.SetPositions(points);
	}
}
