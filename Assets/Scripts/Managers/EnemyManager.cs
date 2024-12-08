using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<Enemy> enemies;

    private void Start()
    {
        enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
    }

    // Method to activate all enemies
    public void ActivateEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.IsDefeated()) // Only activate enemies that are not defeated
            {
                enemy.gameObject.SetActive(true);
            }
        }
    }

    // Method to get enemy defeat states
    public List<bool> GetEnemyStates()
    {
        List<bool> states = new List<bool>();
        foreach (Enemy enemy in enemies)
        {
            states.Add(enemy.IsDefeated());
        }
        return states;
    }

    // Method to set enemy states based on previous saves
    public void SetEnemyStates(List<bool> states)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (i < states.Count && states[i])
            {
                enemies[i].Kill(); // Deactivate the enemy if it was defeated
            }
        }
    }
}
