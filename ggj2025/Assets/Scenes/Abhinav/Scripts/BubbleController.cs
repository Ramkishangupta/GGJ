using UnityEngine;

public class BubbleController : MonoBehaviour
{
	[Header("Bubble Settings")]
	public float maxSize = 2f;            // Maximum size of the bubble
	public float growthRate = 2f;         // How fast the bubble grows
	public float popForceMultiplier = 10f; // Force applied to the player when popped
	public float maxProjectileForce = 20f; // Maximum projectile force for better control

	private float currentSize = 0.1f;     // Starting size of the bubble
	private bool isGrowing = true;        // Whether the bubble is still growing
	private Rigidbody2D bubbleRb;         // Reference to the bubble's Rigidbody2D
	private SpriteRenderer spriteRenderer;

	void Start()
	{
		bubbleRb = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		transform.localScale = Vector3.one * currentSize; // Initialize the size
	}

	void Update()
	{
		if (isGrowing)
		{
			// Grow the bubble until it reaches its max size
			currentSize += growthRate * Time.deltaTime;
			if (currentSize >= maxSize)
			{
				currentSize = maxSize;
				isGrowing = false; // Stop growing
				AutoPop(); // Auto-pop if max size is reached
			}
			transform.localScale = Vector3.one * currentSize; // Update the size
		}
	}

	public void Pop(Rigidbody2D playerRb = null)
	{
		if (playerRb != null)
		{
			// Calculate the direction from the bubble to the player
			Vector2 direction = (playerRb.transform.position - transform.position).normalized;

			// Calculate force magnitude based on bubble size
			float forceMagnitude = Mathf.Clamp(popForceMultiplier * currentSize, 0, maxProjectileForce);

			// Apply the force in the opposite direction of the bubble relative to the player
			Vector2 force = direction * forceMagnitude;

			// Apply the force as an impulse to the player (projectile movement)
			playerRb.AddForce(force, ForceMode2D.Impulse);
		}

		// Add a visual effect or sound here (optional)
		Destroy(gameObject); // Destroy the bubble
	}

	void AutoPop()
	{
		// Trigger the Pop method to ensure force is applied to the player
		Pop(FindObjectOfType<PlayerController>().GetComponent<Rigidbody2D>());
	}
}
