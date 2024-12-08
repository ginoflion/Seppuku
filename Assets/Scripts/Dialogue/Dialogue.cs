using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;         // For the dialogue text
    public TextMeshProUGUI speakerComponent;      // For the speaker's name
    public DialogueLine[] lines;                  // Array of DialogueLine to hold speaker and text
    public float textSpeed;
    public static bool dialogueComplete = false;

    private int index;
    private bool isDialogueActive = false;

    // References to other components
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private LevelTimer levelTimer;
    private Animator playerAnimator;

    public GameObject[] uiElementsToHide;

    private Dictionary<string, Sprite> characterPortraits;

    void Start()
    {


        playerMovement = FindObjectOfType<PlayerMovement>();
        playerAttack = FindObjectOfType<PlayerAttack>();

        levelTimer = FindObjectOfType<LevelTimer>();

        // Assuming PlayerMovement has an Animator component on the same GameObject
        if (playerMovement != null)
        {
            playerAnimator = playerMovement.GetComponent<Animator>();
        }

        textComponent.text = string.Empty;
        speakerComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (isDialogueActive && Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index].text)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index].text;
            }
        }
    }

    void StartDialogue()
    {
        if (playerMovement != null)
        {
            playerMovement.SetDialogueState(true); // Notify PlayerMovement to stop footsteps
            playerMovement.enabled = false;
        }

        if (playerAttack != null) playerAttack.enabled = false;
        if (levelTimer != null) levelTimer.PauseTimer();

        if (playerAnimator != null) playerAnimator.enabled = false;

        foreach (GameObject uiElement in uiElementsToHide)
        {
            uiElement.SetActive(false);
        }

        index = 0;
        isDialogueActive = true;
        UpdateSpeakerAndText();
        StartCoroutine(TypeLine());
    }

    void UpdateSpeakerAndText()
    {
        // Check if the speaker's name is "Rei" to apply the red color
        if (lines[index].speaker == "Noburo" || lines[index].speaker == "Rei" || lines[index].speaker == "Katsuro")
        {
            // Wrap the speaker name with a rich text color tag for red
            speakerComponent.text = $"<color=red>{lines[index].speaker}</color>";
        }
        else
        {
            // Default color for other charactersº
            speakerComponent.text = lines[index].speaker;
        }

        textComponent.text = string.Empty; // Clear text before typing
    }


    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            UpdateSpeakerAndText();
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueComplete = true;
        isDialogueActive = false;

        if (playerMovement != null)
        {
            playerMovement.SetDialogueState(false); // Notify PlayerMovement to resume footsteps if needed
            playerMovement.enabled = true;
        }

        if (playerAttack != null) playerAttack.enabled = true;
        if (levelTimer != null) levelTimer.ResumeTimer();

        if (playerAnimator != null) playerAnimator.enabled = true;

        foreach (GameObject uiElement in uiElementsToHide)
        {
            uiElement.SetActive(true);
        }

        gameObject.SetActive(false);

        GameObject bossObject_1 = GameObject.FindWithTag("Boss");
        if (bossObject_1 != null)
        {
            Boss boss = bossObject_1.GetComponent<Boss>();
            if (boss != null)
            {
                boss.ActivateBoss();
            }
        }
        GameObject bossObject_2 = GameObject.FindWithTag("Boss_2");
        if (bossObject_2 != null)
        {
            Boss_2 boss_2 = bossObject_2.GetComponent<Boss_2>();
            if (boss_2 != null)
            {
                boss_2.ActivateBoss();
            }
        }
        GameObject bossObject_3 = GameObject.FindWithTag("Boss_3");
        if (bossObject_3 != null)
        {
            KatsuroActivate boss3 = bossObject_3.GetComponent<KatsuroActivate>();
            if (boss3 != null)
            {
                boss3.ActivateBoss();
            }
        }
    }
}

// A new struct to hold speaker and text
[System.Serializable]
public struct DialogueLine
{
    public string speaker;
    public string text;
}
