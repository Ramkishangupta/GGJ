using UnityEngine;

public class OneSidedPlatform : MonoBehaviour
{
	private Collider2D platformCollider;

	void Start()
	{
		// Get the Collider2D component of the platform
		platformCollider = GetComponent<Collider2D>();
		if (platformCollider == null)
		{
			Debug.LogError("No Collider2D found on the platform!");
		}
	}

	void Update()
	{
		if (platformCollider == null) return;

		// Get the player's position relative to the platform
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player == null)
		{
			Debug.LogError("Player with tag 'Player' not found!");
			return;
		}

		Transform playerTransform = player.transform;
		Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();

		// If the player's position is below the platform and moving upwards, disable collisions
		if (playerTransform.position.y < transform.position.y && playerRigidbody.linearVelocity.y > 0)
		{
			platformCollider.enabled = false; // Temporarily disable collision
		}
		else
		{
			platformCollider.enabled = true; // Enable collision when above the platform
		}
	}
}
