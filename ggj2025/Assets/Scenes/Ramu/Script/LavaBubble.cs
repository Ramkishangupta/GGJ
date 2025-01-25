using UnityEngine;

public class LavaBubble : MonoBehaviour
{
    public int damageAmount = 10; // Damage dealt to the player

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Access the player's health script and decrease health
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.DecreaseHealth(damageAmount);
            }

            // Destroy the bubble upon collision
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            // Destroy the bubble when it collides with other surfaces
            Destroy(gameObject);
        }
    }
}
