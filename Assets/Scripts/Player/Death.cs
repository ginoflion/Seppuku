using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For working with UI components

public class Death : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private BoxCollider2D bc;
    [SerializeField] private Image fadeImage; // Reference to the UI Image for the fade effect
    public AudioSource deathSound;
    private Vector2 PontoRespawn;
    public bool isDead;
    private List<bool> savedEnemyStates;

    // Reference to LevelTimer
    private LevelTimer levelTimer;

    private GameObject[] shurikens;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        isDead = false;

        shurikens = GameObject.FindGameObjectsWithTag("Projectile");
        // Set the initial respawn point to the player's starting position
        PontoRespawn = transform.position;

        // Find the LevelTimer in the scene
        levelTimer = FindObjectOfType<LevelTimer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Projectile"))
        {
            hit();
        }
    }

    public void hit()
    {
        rb.velocity = Vector2.zero; // Stop movement
        rb.bodyType = RigidbodyType2D.Dynamic; // Keep as Dynamic to allow animations or other functions
        rb.simulated = false; // Temporarily disable Rigidbody physics simulation
        bc.enabled = false;
        isDead = true;
        anim.SetBool("isDead", true);
        Debug.Log("Player Hit");

        // Deduct 5 seconds from the timer
        if (levelTimer != null)
        {
            levelTimer.DeductTimeOnDeath(5f);
        }

        if (deathSound != null)
        {
            deathSound.Play();
        }

        StartCoroutine(FadeAndRespawn()); // Start the fade effect before respawn
    }

    public void SaveEnemyStates(List<bool> states)
    {
        savedEnemyStates = new List<bool>(states);
    }

    private IEnumerator FadeAndRespawn()
    {
        yield return Fade(0f, 1f, 1f); // Fade in to full visibility over 1 second
        yield return new WaitForSeconds(1f); // Optional pause at full visibility
        Respawn(); // Respawn the player
        yield return Fade(1f, 0f, 1f); 
    }


    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color; // Use the current color of the image

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            color.a = alpha; // Update the alpha channel
            fadeImage.color = color; // Apply the new color
            yield return null;
        }

        // Ensure the final alpha value is set
        color.a = endAlpha;
        fadeImage.color = color;
    }


    private void Respawn()
    {
        transform.position = PontoRespawn;
        bc.enabled = true;
        rb.simulated = true; // Reactivate physics interactions
        rb.bodyType = RigidbodyType2D.Dynamic;
        isDead = false;
        anim.SetBool("isDead", false);
        anim.Play("Idle");
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject projectile in projectiles)
        {
            Destroy(projectile.gameObject);
        }

        if (enemyManager != null && savedEnemyStates != null)
        {
            enemyManager.SetEnemyStates(savedEnemyStates);
        }

        Debug.Log("Player Respawned at: " + PontoRespawn);
    }

    // Method to set a new respawn point
    public void SetRespawnPoint(Vector2 newRespawnPoint)
    {
        PontoRespawn = newRespawnPoint;
        Debug.Log("New Respawn Point Set at: " + newRespawnPoint);
    }
}
