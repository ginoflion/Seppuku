using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private List<GameObject> enemies;
    public GameObject finishPoint;
    private GameObject boss;
    private GameObject boss_2;
    private GameObject boss_3;
    private BossHealth bossHealth;
    private BossHealth_2 bossHealth_2;
    private KatsuroHealth katsuroHealth;
    private bool allEnemiesDefeated;

    void Start()
    {
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        boss = GameObject.FindGameObjectWithTag("Boss");
        boss_2 = GameObject.FindGameObjectWithTag("Boss_2");
        boss_3 = GameObject.FindGameObjectWithTag("Boss_3");

        if (boss != null)
        {
            bossHealth = boss.GetComponent<BossHealth>();
        }
        if (boss_2 != null)
        {
            bossHealth_2 = boss_2.GetComponent<BossHealth_2>();
        }
        if (boss_3 != null)
        {
            katsuroHealth = boss_3.GetComponentInChildren<KatsuroHealth>();
        }
        allEnemiesDefeated = false;
        finishPoint.SetActive(false);
    }

    void Update()
    {
        CheckEnemies();
    }

    void CheckEnemies()
    {
        allEnemiesDefeated = true;

        // Check if all regular enemies are defeated
        foreach (GameObject enemy in enemies)
        {
            // Check if the enemy still exists and is active in the hierarchy
            if (enemy != null && enemy.activeInHierarchy)
            {
                allEnemiesDefeated = false;
                break;
            }
        }

        // If there's a boss, check both conditions (allEnemiesDefeated and BossDefeated)
        if (bossHealth != null )
        {
            if (allEnemiesDefeated && bossHealth.isDead)
            {
                finishPoint.SetActive(true);
            }
        }
        else if (katsuroHealth != null)
        {
            if (allEnemiesDefeated && katsuroHealth.isDead) // Replace `isDead` with the appropriate property or method
            {
                finishPoint.SetActive(true);
            }
        }
        else if (bossHealth_2 != null)
        {
            if (allEnemiesDefeated && bossHealth_2.isDead) // Replace `isDead` with the appropriate property or method
            {
                finishPoint.SetActive(true);
            }
        }

        // If there's no boss, just check if all enemies are defeated
        else
        {
            if (allEnemiesDefeated)
            {
                finishPoint.SetActive(true);
            }
        }
    }


    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {

            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Acabaste o Jogo");
        }
    }
}
