using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelEnd : MonoBehaviour
{

    private LevelTimer levelTimer;
    public GameObject levelCompletePanel;
    public GameObject Image;
    public GameObject Timer;
    public TextMeshProUGUI textMeshProUGUI;
    public GameObject Credits;

    // References to player movement and attack scripts
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttack playerAttack;

    private void Start()
    {
        levelTimer = FindObjectOfType<LevelTimer>();
    }

 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UnlockNewLevel();
            Debug.Log("Nível completo!");

            if (levelTimer != null)
            {
                float completionTime = levelTimer.GetCompletionTime();
                SaveCompletionTime(completionTime);
                levelTimer.OnLevelComplete();
                textMeshProUGUI.text = "Time: " + completionTime.ToString("F2");
            }
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }

            // Disable player movement and attack
            if (playerMovement != null) playerMovement.enabled = false;
            if (playerAttack != null) playerAttack.enabled = false;
            Animator animator = playerMovement.GetComponentInParent<Animator>();
            animator.SetInteger("state", 0);
            // Disable all AudioSources attached to the player
            AudioSource[] audioSources = other.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.enabled = false;
            }

            // Display level complete panel and hide unnecessary UI
            levelCompletePanel.SetActive(true);
            Image.SetActive(false);
            Timer.SetActive(false);
        }
    }

    void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }

    void SaveCompletionTime(float completionTime)
    {
        string key = "LevelTime_" + SceneManager.GetActiveScene().buildIndex;

        if (PlayerPrefs.HasKey(key))
        {
            float savedTime = PlayerPrefs.GetFloat(key);

            if (completionTime < savedTime)
            {
                PlayerPrefs.SetFloat(key, completionTime);
                PlayerPrefs.Save();
                Debug.Log("New lower completion time saved: " + completionTime + " seconds");
            }
            else
            {
                Debug.Log("Completion time not saved. Current time: " + completionTime + " seconds is not lower than the saved time: " + savedTime + " seconds");
            }
        }
        else
        {
            PlayerPrefs.SetFloat(key, completionTime);
            PlayerPrefs.Save();
            Debug.Log("First completion time saved: " + completionTime + " seconds");
        }
    }
}
