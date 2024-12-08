using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject menupausa;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject weapon;
    [SerializeField] private Animator playerAnimator;
    private LevelTimer levelTimer;

    [SerializeField] private CanvasGroup timeWarningCanvasGroup; // Assign in the Inspector
    [SerializeField] private TextMeshProUGUI timeWarningText; // Text component for the warning message
    public GameObject Credits;
    [SerializeField] private GameObject Buttons;
    [SerializeField] private GameObject OptionsPanel;
    [SerializeField] private GameObject Controls;
    [SerializeField] private GameObject volume;
    [SerializeField] private GameObject SettingsTitle;
    [SerializeField] private Slider volumeSlider;
    private bool isToggleInput = false;

    private GameObject currentActivePanel;
    private Dictionary<string, GameObject> panels;
    [SerializeField] private GameObject[] uiElementsToHide;

    private void Start()
    {
        isToggleInput = PlayerPrefs.GetInt("InputMode", 0) == 1;
        Debug.Log("This script is attached to: " + gameObject.name);
        levelTimer = FindObjectOfType<LevelTimer>();
        if (timeWarningCanvasGroup != null)
        {
            timeWarningCanvasGroup.alpha = 0f; // Ensure it's invisible at the start
        }
        panels = new Dictionary<string, GameObject>
        {
            { "Controls", Controls },
            { "Volume", volume },
            { "Settings", SettingsTitle }
        };

        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f); 
        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume; 
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }


        currentActivePanel = SettingsTitle; 

    }
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume); 
        PlayerPrefs.Save();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {

                Resume();
            }
            else
            {

                Pause();
            }
        }
        TimerWarnings();
    }

    public void Pause()
    {
        menupausa.SetActive(true);
        Time.timeScale = 0f;
        playerMovement.enabled = false;
        weapon.SetActive(false);
        GamePaused = true;

        // Stop player animations
        playerAnimator.speed = 0;

        // Hide specified UI elements
        foreach (GameObject uiElement in uiElementsToHide)
        {
            uiElement.SetActive(false);
        }

        // Freeze all physics objects
        FreezePhysics();
    }

   public void Resume()
    {
        menupausa.SetActive(false);
        Time.timeScale = 1f;
        playerMovement.enabled = true;
        weapon.SetActive(true);
        GamePaused = false;

        // Resume player animations
        playerAnimator.speed = 1;

        // Show specified UI elements
        foreach (GameObject uiElement in uiElementsToHide)
        {
            uiElement.SetActive(true);
        }

        // Unfreeze all physics objects
        UnfreezePhysics();
    }

    void FreezePhysics()
    {
        foreach (Rigidbody2D rb in FindObjectsOfType<Rigidbody2D>())
        {
            rb.simulated = false;
        }
    }

    void UnfreezePhysics()
    {
        foreach (Rigidbody2D rb in FindObjectsOfType<Rigidbody2D>())
        {
            rb.simulated = true;
        }
    }




    public void Exit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        playerMovement.enabled = true;
        weapon.SetActive(true);
        SceneManager.LoadScene("MainMenu");
    }

    public void Options()
    {
        Buttons.SetActive(false);      // Hide the main menu buttons
        OptionsPanel.SetActive(true);  // Show the options panel
        ShowPanel("Settings");    // Ensure the SettingsTitle appears by default
    }

    public void OptionsBack()
    {
        Buttons.SetActive(true);       // Show the main menu buttons
        OptionsPanel.SetActive(false); // Hide the options panel
        currentActivePanel.SetActive(false); // Deactivate the currently active panel in options
        currentActivePanel = SettingsTitle;  // Reset to SettingsTitle for the next time
    }

    public void ShowPanel(string panelKey)
    {
        // Deactivate all panels first to prevent overlap
        foreach (var panel in panels.Values)
        {
            panel.SetActive(false);
        }

        // Activate the specified panel
        if (panels.ContainsKey(panelKey))
        {
            panels[panelKey].SetActive(true);
            currentActivePanel = panels[panelKey];
        }
        else if (panelKey == "SettingsTitle")
        {
            SettingsTitle.SetActive(true);
            currentActivePanel = SettingsTitle;
        }
    }



    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void FinishLevel()
    {
        Credits.SetActive(true);
        StartCoroutine(ShowCreditsAndReturnToMainMenu());
    }

    void TimerWarnings()
    {
        // Check if levelTimer is null, exit the function if it is
        if (levelTimer == null)
        {
            return;
        }

        // Check the specific times and trigger the fade effect
        if (levelTimer.timeLeft < 30f && levelTimer.timeLeft > 29f)
        {
            StartCoroutine(FadeInAndOut("30"));
        }
        else if (levelTimer.timeLeft < 15f && levelTimer.timeLeft > 14f)
        {
            StartCoroutine(FadeInAndOut("15"));
        }
        else if (levelTimer.timeLeft < 3f && levelTimer.timeLeft > 2f)
        {
            StartCoroutine(FadeInAndOut("3"));
        }
        else if (levelTimer.timeLeft < 2f && levelTimer.timeLeft > 1f)
        {
            StartCoroutine(FadeInAndOut("2"));
        }
        else if (levelTimer.timeLeft < 1f && levelTimer.timeLeft > 0f)
        {
            StartCoroutine(FadeInAndOut("1"));
        }
    }

    IEnumerator FadeInAndOut(string message)
    {
        timeWarningText.text = message;

        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            timeWarningCanvasGroup.alpha = t;
            yield return null;
        }
        timeWarningCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(0.1f);

        for (float t = 1f; t > 0; t -= Time.deltaTime)
        {
            timeWarningCanvasGroup.alpha = t;
            yield return null;
        }
        timeWarningCanvasGroup.alpha = 0f;
    }

    IEnumerator ShowCreditsAndReturnToMainMenu()
    {
        
        yield return new WaitForSeconds(25f); // Wait for 3 seconds
        SceneManager.LoadScene("MainMenu"); // Load the main menu
    }

    public void ToggleInputMode(Button button)
    {
        // Switch between Hold and Toggle modes
        isToggleInput = !isToggleInput;

        UpdateInputModeButtonText(button);

        PlayerPrefs.SetInt("InputMode", isToggleInput ? 1 : 0);
        PlayerPrefs.Save();

        // Notify GameManager about the change
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.UpdateInputMode(isToggleInput);
        }
    }


    public void UpdateInputModeButtonText(Button button)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = isToggleInput ? "Toggle" : "Hold";
        }
    }
}
