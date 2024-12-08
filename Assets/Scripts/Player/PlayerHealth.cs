using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float health = 30f;
    public AudioSource deathSound;
    public Animator animator;
    public float animationDelay = 2f;

    // Respawn system variables
    private Vector2 respawnPoint;
    private Rigidbody2D rb;
    private bool isDead;

    // Reference to LevelTimer
    private LevelTimer levelTimer;
    [SerializeField] private Boss boss;
    private void Start()
    {
        // Find the LevelTimer in the scene
        levelTimer = FindObjectOfType<LevelTimer>();

        // Set the initial respawn point to the player's starting position
        respawnPoint = transform.position;

        // Set up rigidbody and isDead status
        rb = GetComponent<Rigidbody2D>();
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Player took damage. Current health: " + health);

        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        isDead = true;

        // Deduct 5 seconds from the timer
        if (levelTimer != null)
        {
            levelTimer.DeductTimeOnDeath(5f);
        }

        // Play death sound if available
        if (deathSound != null)
        {
            deathSound.Play();
        }

        // Play death animation if animator is set
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        // Stop movement and disable physics temporarily
        rb.velocity = Vector2.zero;
        rb.simulated = false;

        // Start respawn coroutine
        StartCoroutine(RespawnAfterDeath());
    }

    private IEnumerator RespawnAfterDeath()
    {
        yield return new WaitForSeconds(animationDelay);

        // Respawn player at checkpoint
        Respawn();
    }

    private void Respawn()
    {
        // Reset player position and physics
        transform.position = respawnPoint;
        rb.simulated = true;
        isDead = false;

        // Reset animation state if animator is set
        if (animator != null)
        {
            animator.SetBool("isDead", false);
        }

        // Reset boss health and state
        if (boss != null)
        {
            boss.ResetBoss(); // Call the reset method on the boss
        }

        // Reset player health
        health = 30f;

        Debug.Log("Player Respawned at: " + respawnPoint);
    }

    // Method to set a new respawn point (can be called from checkpoints in the level)
    public void SetRespawnPoint(Vector2 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
        Debug.Log("New Respawn Point Set at: " + newRespawnPoint);
    }
}
