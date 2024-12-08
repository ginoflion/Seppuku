using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private float totalTime = 300f;
    [SerializeField] private GameObject TimesUpPanel;
    private float elapsedTime;
    public float timeLeft;

    private bool levelCompleted = false;
    private bool isPaused = false;

    [SerializeField] private TextMeshProUGUI timeWarningText;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
    {
        elapsedTime = 0f;
        timeLeft = totalTime;
    }

    private void Update()
    {
        if (levelCompleted || isPaused)
        {
            return;
        }

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
        }

        elapsedTime += Time.deltaTime;

        if (timeLeft <= 0)
        {
            timeLeft = 0f;
            timeWarningText.gameObject.SetActive(false);
            TimesUpPanel.gameObject.SetActive(true);
        }

        UpdateTimerUI();
    }

    public void OnLevelComplete()
    {
        levelCompleted = true;
        Debug.Log($"Level Completed! Time Left: {timeLeft} seconds, Time Taken: {elapsedTime} seconds");
    }

    public float GetCompletionTime()
    {
        return elapsedTime;
    }

    private void UpdateTimerUI()
    {
        string formattedTimeLeft = FormatTime(timeLeft);
        timerText.text = $"{formattedTimeLeft}";
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int centiseconds = Mathf.FloorToInt((time % 1) * 100);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, centiseconds);
    }

    public void Reset()
    {
        elapsedTime = 0f;
        timeLeft = totalTime;
        levelCompleted = false;
        isPaused = false;
        UpdateTimerUI();
        Debug.Log("Timer reset.");
    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void ResumeTimer()
    {
        isPaused = false;
    }

    // New method to reduce time when player dies
    public void DeductTimeOnDeath(float timeToDeduct)
    {
        timeLeft -= timeToDeduct;
        if (timeLeft < 0) timeLeft = 0;
        UpdateTimerUI();
        Debug.Log($"Time deducted due to death. Time left: {timeLeft} seconds.");
    }
}
