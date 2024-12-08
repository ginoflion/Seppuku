using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject chapter1Panel;
    [SerializeField] private GameObject chapter2Panel;
    [SerializeField] private GameObject chapter3Panel;
    [SerializeField] private GameObject chapterSelectionPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private List<GameObject> Levels;
    [SerializeField] private GameObject chapter1Button;
    [SerializeField] private GameObject chapter2Button;
    [SerializeField] private GameObject chapter3Button;
    [SerializeField] private GameObject tutorialButton;
    [SerializeField] private GameObject OptionsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject volumePanel;
    [SerializeField] private GameObject generalSettingsPanel;
    private bool isToggleInput = false;
    [SerializeField] private Slider volumeSlider;

    private Dictionary<string, GameObject> optionsPanels;
    private GameObject currentActivePanel;

    private float[] bronzeTimes = { 200f, 260f, 180f, 110f, 115f, 150f, 80f, 75f, 200f };
    private float[] silverTimes = { 170f, 230f, 150f, 90f, 90f, 130f, 70f, 60f, 180f };
    private float[] goldTimes = { 140f, 200f, 120f, 75f, 80f, 100f, 55f, 45f, 160f };

    [SerializeField] private Sprite bronzeMedalSprite;
    [SerializeField] private Sprite silverMedalSprite;
    [SerializeField] private Sprite goldMedalSprite;

    private const int levelStartIndex = 2; // Offset for the first level

    private void Start()
    {
        UnlockLevels();

        optionsPanels = new Dictionary<string, GameObject>
    {
        { "Controls", controlsPanel },
        { "Volume", volumePanel },
        { "GeneralSettings", generalSettingsPanel }
    };
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f);
        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        // Set the default panel to active
        ShowOptionsPanel("GeneralSettings");
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        OptionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        ResetButtonHoverStates(mainMenuPanel);
        OptionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }



    public void Exit()
    {
        Application.Quit();
    }

    public void TutorialLevel(string Tutorial)
    {
        SceneManager.LoadScene(Tutorial);
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        chapterSelectionPanel.SetActive(true);
    }

    public void Chapter1()
    {
        chapterSelectionPanel.SetActive(false);
        chapter1Panel.SetActive(true);
        UpdateChapterCompletionTimes(0, 3);
    }

    private void UnlockLevels()
    {
        int reachedIndex = PlayerPrefs.GetInt("ReachedIndex", levelStartIndex);

        for (int i = 0; i < Levels.Count; i++)
        {
            Levels[i].SetActive(i + levelStartIndex <= reachedIndex);
        }

        chapter2Button.SetActive(IsChapterCompleted(2, 4));
        chapter3Button.SetActive(IsChapterCompleted(5, 7));

        UpdateChapterButtonColor(chapter1Button, 2, 4);
        UpdateChapterButtonColor(chapter2Button, 5, 7);
        UpdateChapterButtonColor(chapter3Button, 8, 10);
    }

    private bool IsChapterCompleted(int startLevel, int endLevel)
    {
        for (int i = startLevel; i <= endLevel; i++)
        {
            string key = "LevelTime_" + i;
            if (PlayerPrefs.GetFloat(key, -1) == -1)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsChapterGold(int startLevel, int endLevel)
    {
        for (int i = startLevel; i <= endLevel; i++)
        {
            string key = "LevelTime_" + i;
            float completionTime = PlayerPrefs.GetFloat(key, -1);

            if (completionTime == -1 || completionTime > goldTimes[i - levelStartIndex])
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateChapterButtonColor(GameObject chapterButton, int startLevel, int endLevel)
    {
        TextMeshProUGUI buttonText = chapterButton.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            if (IsChapterGold(startLevel, endLevel))
            {
                buttonText.color = Color.red; 
            }
           
        }
    }

    public void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.Save();
        }
    }

    public void OpenLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void Chapter2()
    {
        chapterSelectionPanel.SetActive(false);
        chapter2Panel.SetActive(true);
        UpdateChapterCompletionTimes(3, 6);
    }

    public void Chapter3()
    {
        chapterSelectionPanel.SetActive(false);
        chapter3Panel.SetActive(true);
        UpdateChapterCompletionTimes(6, 9);
    }

    private void UpdateChapterCompletionTimes(int startLevel, int endLevel)
    {
        for (int i = startLevel; i < endLevel && i < Levels.Count; i++)
        {
            UpdateCompletionTimeText(i);
        }
    }
    public void ShowOptionsPanel(string panelKey)
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
        }

        if (optionsPanels.ContainsKey(panelKey))
        {
            optionsPanels[panelKey].SetActive(true);
            currentActivePanel = optionsPanels[panelKey];
        }
        else
        {
            generalSettingsPanel.SetActive(true);
            currentActivePanel = generalSettingsPanel;
        }
    }
    public void GoBackToMenu()
    {
        ResetButtonHoverStates(chapterSelectionPanel);
        ResetButtonHoverStates(mainMenuPanel);
        mainMenuPanel.SetActive(true);
        chapterSelectionPanel.SetActive(false);
    }

    public void GoBackToChapterSelectionPanel1()
    {
        ResetButtonHoverStates(chapterSelectionPanel);
        chapter1Panel.SetActive(false);
        chapterSelectionPanel.SetActive(true);
    }

    public void GoBackToChapterSelectionPanel2()
    {
        ResetButtonHoverStates(chapterSelectionPanel);
        chapter2Panel.SetActive(false);
        chapterSelectionPanel.SetActive(true);
    }

    public void GoBackToChapterSelectionPanel3()
    {
        ResetButtonHoverStates(chapterSelectionPanel);
        chapter3Panel.SetActive(false);
        chapterSelectionPanel.SetActive(true);
    }

    public void UpdateCompletionTimeText(int levelId)
    {
        if (levelId < 0 || levelId >= Levels.Count) return;

        TextMeshProUGUI timeText = Levels[levelId].transform.Find("TimeCompletion")?.GetComponent<TextMeshProUGUI>();
        Image medalImage = Levels[levelId].transform.Find("Medal")?.GetComponent<Image>();

        if (timeText != null)
        {
            string key = "LevelTime_" + (levelId + levelStartIndex);
            float completionTime = PlayerPrefs.GetFloat(key, -1);

            if (completionTime >= 0)
            {
                timeText.text = "Best Time: " + completionTime.ToString("F2") + " seconds";
                DisplayMedal(medalImage, levelId + levelStartIndex, completionTime);
            }
            else
            {
                timeText.text = "Best Time: Not Yet Completed";
                medalImage.sprite = null;
            }
        }
    }

    private void ResetButtonHoverStates(GameObject panel)
    {
        HoverHighlightText[] hoverTexts = panel.GetComponentsInChildren<HoverHighlightText>(true);

        foreach (HoverHighlightText hoverText in hoverTexts)
        {
            hoverText.ResetHoverState();
        }
    }

    private void DisplayMedal(Image medalImage, int levelId, float completionTime)
    {
        if (completionTime <= goldTimes[levelId - levelStartIndex])
        {
            medalImage.sprite = goldMedalSprite;
        }
        else if (completionTime <= silverTimes[levelId - levelStartIndex])
        {
            medalImage.sprite = silverMedalSprite;
        }
        else if (completionTime <= bronzeTimes[levelId - levelStartIndex])
        {
            medalImage.sprite = bronzeMedalSprite;
        }
        else
        {
            medalImage.sprite = null;
        }
    }

    public void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void InitializeInputMode(Button button)
    {
        // Read saved input mode from PlayerPrefs, defaulting to 0 (Hold)
        isToggleInput = PlayerPrefs.GetInt("InputMode", 0) == 1;

        // Update the button text to match the saved state
        UpdateInputModeButtonText(button);
    }

    public void ToggleInputMode(Button button)
    {
        // Switch between Hold and Toggle modes
        isToggleInput = !isToggleInput;

        // Update the button text to reflect the change
        UpdateInputModeButtonText(button);

        // Save the new state to PlayerPrefs
        PlayerPrefs.SetInt("InputMode", isToggleInput ? 1 : 0);
        PlayerPrefs.Save();
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
