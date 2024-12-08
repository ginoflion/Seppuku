using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private GameObject dialoguePanelStart;
    [SerializeField] private GameObject dialoguePanelFinish;
    private bool isToggleInput = false;

    private bool bulletTimeActive = false;

    private Collider2D triggerCollider; // Reference to the Collider2D component

    private void Start()
    {
        isToggleInput = PlayerPrefs.GetInt("InputMode", 0) == 1;
        Debug.LogWarning("Input" + isToggleInput);
        triggerCollider = GetComponent<Collider2D>(); // Get the Collider2D attached to this GameObject
    }

    private void Update()
    {
        if (isToggleInput) // Toggle mode
        {
            if (Input.GetButtonDown("BulletTime"))
            {
                bulletTimeActive = !bulletTimeActive;
                if (bulletTimeActive)
                {
                    timeManager.StartBulletTime();
                }
                else
                {
                    timeManager.StopBulletTime();
                }
            }
        }
        else // Hold mode
        {
            if (Input.GetButton("BulletTime"))
            {
                timeManager.StartBulletTime();
            }
            else
            {
                timeManager.StopBulletTime();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialoguePanelStart.SetActive(true); // Activate the start dialogue panel
            triggerCollider.enabled = false; // Disable the trigger collider on the GameManager itself
        }
    }

    public void OnBossDefeated()
    {
        StartCoroutine(ShowFinishDialogueWithDelay());
    }

    private IEnumerator ShowFinishDialogueWithDelay()
    {
        yield return new WaitForSeconds(3); // Wait for 3 seconds
        dialoguePanelFinish.SetActive(true); // Activate the finish dialogue panel
    }
    public void UpdateInputMode(bool newInputMode)
    {
        isToggleInput = newInputMode;
        Debug.LogWarning("Input mode updated to: " + (isToggleInput ? "Toggle" : "Hold"));
    }
}
