using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[Header("Health Settings")]
	public float maxHealth = 100f; // Maximum health value
	public float currentHealth; // Current health value
	public float regenerationRate = 5f; // Health regeneration per second

	[Header("UI Settings")]
	public Slider healthSlider; // Reference to the UI slider for health

	private void Start()
	{
		// Initialize health and update the slider
		currentHealth = maxHealth;
		UpdateHealthUI();
	}

	private void Update()
	{
		// Regenerate health over time
		if (currentHealth < maxHealth && currentHealth >= 1f)
		{
			currentHealth += regenerationRate * Time.deltaTime;
			currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
			UpdateHealthUI();
		}
	}

	/// <summary>
	/// Reduces the player's health by the given amount.
	/// </summary>
	/// <param name="damage">The amount of health to reduce.</param>
	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
		UpdateHealthUI();

		// Optional: Handle player death if health reaches 0
		if (currentHealth <= 2)
		{
			OnPlayerDeath();
		}
	}

	/// <summary>
	/// Updates the health slider UI to reflect the current health.
	/// </summary>
	private void UpdateHealthUI()
	{
		if (healthSlider != null)
		{
			healthSlider.value = currentHealth / maxHealth;
		}
	}

	/// <summary>
	/// Handles logic when the player dies.
	/// </summary>
	private void OnPlayerDeath()
	{
		Debug.Log("Player has died!");
		HoldAndShoot has = GetComponent<HoldAndShoot>();
		has.enabled = false;
		// Add death logic here (e.g., disable player movement, play animation, etc.)
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("enemy"))
		{
			TakeDamage(30);
		}
		if (collision.CompareTag("death"))
		{
			TakeDamage(200);
		}
		if (collision.CompareTag("ball"))
		{
			TakeDamage(80);
		}
	}
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.tag == "death")
		{
			TakeDamage(200);
		}
	}
}
