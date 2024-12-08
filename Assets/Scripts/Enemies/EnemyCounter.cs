using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    private int numberEnemies = 0;
    [SerializeField] private TextMeshProUGUI enemiesCounter;
    [SerializeField] private GameObject finishLineArrow; // Reference to the arrow GameObject

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the arrow is hidden at the start
        if (finishLineArrow != null)
        {
            finishLineArrow.SetActive(false);
        }

        // Initial count of enemies
        UpdateEnemyCount();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemyCount();
    }

    private void UpdateEnemyCount()
    {
        // Find all game objects tagged as "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Initialize the enemy count with active enemies
        numberEnemies = enemies.Length;

        // Check bosses and their health status
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        GameObject boss_2 = GameObject.FindGameObjectWithTag("Boss_2");
        GameObject boss_3 = GameObject.FindGameObjectWithTag("Boss_3");

        if (boss != null)
        {
            BossHealth bossHealth = boss.GetComponent<BossHealth>();
            if (bossHealth != null && !bossHealth.isDead)
            {
                numberEnemies += 1; // Count the boss if not dead
            }
        }

        if (boss_2 != null)
        {
            BossHealth_2 bossHealth_2 = boss_2.GetComponent<BossHealth_2>();
            if (bossHealth_2 != null && !bossHealth_2.isDead)
            {
                numberEnemies += 1; // Count the second boss if not dead
            }
        }

        if (boss_3 != null)
        {
            KatsuroHealth katsuroHealth = boss_3.GetComponentInChildren<KatsuroHealth>();
            if (katsuroHealth != null && !katsuroHealth.isDead)
            {
                numberEnemies += 1; // Count Katsuro if not dead
            }
        }

        // Update the UI text
        if (numberEnemies == 0)
        {
            enemiesCounter.text = "No enemies remaining";

            // Show the finish line arrow when enemies are gone
            if (finishLineArrow != null)
            {
                finishLineArrow.SetActive(true);
            }
        }
        else
        {
            enemiesCounter.text = "Enemies Remaining: " + numberEnemies.ToString();

            // Hide the finish line arrow when there are still enemies
            if (finishLineArrow != null)
            {
                finishLineArrow.SetActive(false);
            }
        }
    }
}
