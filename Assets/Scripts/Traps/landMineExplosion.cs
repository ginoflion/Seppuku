using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class landMineExplosion : MonoBehaviour
{
    private AudioSource explosionSound; // Sound for the explosion effect
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private VisualEffect explosionEffect;

    private void Start()
    {
        explosionSound = GetComponent<AudioSource>();
        explosionEffect = GetComponentInChildren<VisualEffect>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Trigger player death
            Death playerDeath = collision.GetComponent<Death>();
            if (playerDeath != null)
            {
                playerDeath.hit();
            }
            boxCollider.enabled=false;
            spriteRenderer.enabled = false;

            // Play explosion sound if available
            if (explosionSound != null)
            {
                explosionSound.Play();
            }

            // Instantiate explosion effect if available
            if (explosionEffect != null)
            {
               explosionEffect.Play();
            }

        }
    }
}
