using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class CheckpoinManager : MonoBehaviour
{
    [SerializeField] private Death playerDeath;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField]private VisualEffect effect;
    private Light2D lights;

    private void Start()
    {
        lights = GetComponent<Light2D>();
        lights.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            effect.Play();
            lights.enabled = true;
            playerDeath.SetRespawnPoint(transform.position);

            // Save enemy states
            if (enemyManager != null)
            {
                List<bool> enemyStates = enemyManager.GetEnemyStates();
                playerDeath.SaveEnemyStates(enemyStates);
            }

            Debug.Log("Checkpoint Reached at: " + transform.position);
        }
    }
}
