using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {

    [SerializeField] private DialogueData dialogueData;

    // UI elements
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Text characterName;
    [SerializeField] private string characterPortraitName;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private List<Button> dialogueButtons = new List<Button>();

    // IEnumerator
    private IEnumerator readDialogue;
    private IEnumerator displayDialogueUI;

    // Bool
    private bool dialogueIsRunning = false;
    private bool dialogueDisplayed = false;

    private void Start() {
        // Hiding the button on default.
        DisplayButtons(false);
        displayDialogueUI = DisplayDialogueUI(0.05f, 0.025f, () => {
            dialogueData.GetNextDialogue();
            AdvanceDialogue();
        });
        StartCoroutine(displayDialogueUI);
    }

    private void Update() {
        // Set the next test (Needs to be reworked)
        if (Input.GetMouseButtonDown(0)) {
            if (!dialogueData.IsLocked && !dialogueIsRunning && dialogueDisplayed) {
                // Fetching the data of the next dialogue.
                dialogueData.GetNextDialogue();

                // Setting our new dialogue.
                AdvanceDialogue();
            } else if (dialogueIsRunning) {
                SkipDialogue();
            }
        }
        if (Input.GetMouseButtonDown(1)) {

            // Set the IEnumerators
            displayDialogueUI = DisplayDialogueUI(0.05f, 0.01f);
            StartCoroutine(displayDialogueUI);
        }
    }

    public void AdvanceDialogue() {
        characterName.text = dialogueData.DialogueName;
        // Character portrait needs to get the name+mood
        characterPortraitName = characterName.text + "_" + dialogueData.DialogueMood;
        Sprite currentPortrait = (Sprite)Resources.Load("Characters/" + characterName.text + "/" + characterPortraitName, typeof(Sprite));
        characterPortrait.sprite = currentPortrait;

        // Set the coroutine.
        readDialogue = ReadDialogue(Constants.DialogueReadSpeed);
        StartCoroutine(readDialogue);
    }

    private IEnumerator ReadDialogue(float typeSpeed) {
        dialogueText.text = "";
        dialogueIsRunning = true;

        foreach (char letter in dialogueData.DialogueLine.ToCharArray()) {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typeSpeed);
        }

        // Check if we need to display options buttons.
        if (dialogueData.IsQuestion) {
            ActivateButtons();
        }

        dialogueIsRunning = false;
        StopCoroutine(readDialogue);
    }

    private void SkipDialogue() {
        StopCoroutine(readDialogue);
        dialogueText.text = dialogueData.DialogueLine;
        dialogueIsRunning = false;
        
        // Check if we need to display options buttons.
        if (dialogueData.IsQuestion) {
            ActivateButtons();
        }
    }

    private void ActivateButtons() {
        DisplayButtons(true);
        dialogueData.SetButtons(dialogueButtons, AdvanceDialogue);

        // Making sure the buttons will always hide when pressing on them.
        for (int i = 0; i < dialogueButtons.Count; i++) {
            dialogueButtons[i].onClick.AddListener(() => DisplayButtons(false));
        }
    }
    
    // Showing and hiding our buttons.
    private void DisplayButtons(bool isDisplayed) {
        for (int i = 0; i < dialogueButtons.Count; i++) {
            dialogueButtons[i].gameObject.SetActive(isDisplayed);

            // Removing all listeners on the buttons. 
            dialogueButtons[i].onClick.RemoveAllListeners();
        }
    }

    // Showing and hiding the whole dialogue.
    private IEnumerator DisplayDialogueUI(float fadeAmount, float fadeSpeed, Action callback = null) {
        float targetAlpha = canvasGroup.alpha == 1 ? 0 : 1;
        canvasGroup.interactable = false;
        dialogueDisplayed = false;

        while (true) {
            if (targetAlpha == 0) { 
                if (canvasGroup.alpha > targetAlpha) {
                    canvasGroup.alpha -= fadeAmount;

                    // Check if we are done fading.
                    if (canvasGroup.alpha <= targetAlpha) {
                        StopCoroutine(displayDialogueUI);

                        if (callback != null) {
                            callback();
                        }
                    }
                }
            } else if (targetAlpha == 1) {
                if (canvasGroup.alpha < targetAlpha) {
                    canvasGroup.alpha += fadeAmount;

                    // Check if we are done fading.
                    if (canvasGroup.alpha >= targetAlpha) {
                        dialogueDisplayed = true;
                        canvasGroup.interactable = true;
                        StopCoroutine(displayDialogueUI);

                        if (callback != null) {
                            callback();
                        }
                    }
                }
            }

            yield return new WaitForSeconds(fadeSpeed);
        }
    }

}
