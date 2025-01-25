using UnityEngine;

public class BalloonMechanism : MonoBehaviour
{
	[Header("Balloon Settings")]
	public GameObject balloonPrefab; // The balloon prefab
	public Transform balloonSpawnPoint; // Where the balloon spawns
	public float maxBalloonSize = 2f; // Maximum size the balloon can grow to
	public float balloonGrowthRate = 1f; // How fast the balloon grows
	public float maxFloatingVelocity = 5f; // Maximum upward velocity when floating
	public float negativeGravityScale = -0.5f; // Gravity scale for floating
	public float balloonLifetime = 3f; // How long the balloon stays after release

	private GameObject spawnedBalloon; // Reference to the spawned balloon
	private bool isBalloonGrowing = false; // Whether the balloon is currently growing
	private float balloonCurrentSize = 0f; // Current size of the balloon

	private Rigidbody2D playerRb; // Player's Rigidbody2D
	private float originalGravityScale; // Original gravity scale of the player

	private void Start()
	{
		// Get the player's Rigidbody2D and store the original gravity scale
		playerRb = GetComponent<Rigidbody2D>();
		originalGravityScale = playerRb.gravityScale;
	}

	private void Update()
	{
		// Balloon mechanism
		if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
		{
			StartBalloon();
		}
		if (Input.GetMouseButton(1)) // Right mouse button held
		{
			UpdateBalloonGrowth();
		}
		if (Input.GetMouseButtonUp(1)) // Right mouse button released
		{
			ReleaseBalloon();
		}

		// Clamp the player's upward velocity when floating
		ClampFloatingVelocity();
	}

	private void StartBalloon()
	{
		// Spawn the balloon as a child of the player
		if (spawnedBalloon == null && balloonPrefab != null)
		{
			spawnedBalloon = Instantiate(balloonPrefab, balloonSpawnPoint.position, Quaternion.identity, transform);
			spawnedBalloon.transform.localScale = Vector3.zero; // Start at zero size
			isBalloonGrowing = true;
			balloonCurrentSize = 0f; // Reset size

			// Set the player's gravity scale to negative for floating
			playerRb.gravityScale = negativeGravityScale;
		}
	}

	private void UpdateBalloonGrowth()
	{
		if (spawnedBalloon != null && isBalloonGrowing)
		{
			// Increment the balloon size
			balloonCurrentSize += balloonGrowthRate * Time.deltaTime;
			spawnedBalloon.transform.localScale = Vector3.one * Mathf.Min(balloonCurrentSize, maxBalloonSize);

			// If the balloon exceeds its max size, pop it
			if (balloonCurrentSize >= maxBalloonSize)
			{
				PopBalloon();
			}
		}
	}

	private void ReleaseBalloon()
	{
		// Stop balloon growth and let it stay for a specific duration
		if (spawnedBalloon != null)
		{
			isBalloonGrowing = false;

			// Restore the player's original gravity scale
			playerRb.gravityScale = originalGravityScale;

			// Let the balloon stay for the defined lifetime, then destroy it
			StartCoroutine(DestroyBalloonAfterTime(balloonLifetime));
		}
	}

	private System.Collections.IEnumerator DestroyBalloonAfterTime(float delay)
	{
		yield return new WaitForSeconds(delay);

		if (spawnedBalloon != null)
		{
			Destroy(spawnedBalloon);
		}
	}

	private void PopBalloon()
	{
		// Destroy the balloon and restore gravity scale when it pops
		if (spawnedBalloon != null)
		{
			Destroy(spawnedBalloon);
			isBalloonGrowing = false;

			// Restore the player's original gravity scale
			playerRb.gravityScale = originalGravityScale;
		}
	}

	private void ClampFloatingVelocity()
	{
		// Clamp the player's upward velocity when floating
		if (playerRb.gravityScale == negativeGravityScale)
		{
			float clampedVelocityY = Mathf.Clamp(playerRb.linearVelocity.y, -Mathf.Infinity, maxFloatingVelocity);
			playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, clampedVelocityY);
		}
	}
}
