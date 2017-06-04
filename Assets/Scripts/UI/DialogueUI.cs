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
    [SerializeField] private ButtonUI buttonUI;
    [SerializeField] private Image skipButton;
    [SerializeField] private string characterPortraitName;
    [SerializeField] private RawImage characterPortrait;

    // IEnumerator
    private IEnumerator readDialogue;
    private IEnumerator displayDialogueUI;

    // Bool
    private bool dialogueIsRunning = false;
    private bool dialogueDisplayed = false;

    private void Start() {
        // Hiding all the button on default.
        buttonUI.DisplayButtons(false);
        characterPortrait.canvasRenderer.SetAlpha(0);

        // Show to first dialogue line.
        displayDialogueUI = DisplayDialogueUI(0.05f, 0.025f, () => {
            dialogueData.GetNextDialogue();
            characterPortrait.canvasRenderer.SetAlpha(1);
            AdvanceDialogue();
        });
        StartCoroutine(displayDialogueUI);
    }

    private void Update() {
        // Set the next dialogue
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
    }

    public void AdvanceDialogue() {
        characterName.text = dialogueData.DialogueName;
        // Load and display the image of our current speaker.
        ShowCharacter();

        // Set the coroutine.
        readDialogue = ReadDialogue(Constants.DialogueReadSpeed);
        StartCoroutine(readDialogue);
    }

    private IEnumerator ReadDialogue(float typeSpeed) {
        skipButton.enabled = false;
        dialogueText.text = "";
        dialogueIsRunning = true;

        foreach (char letter in dialogueData.DialogueLine.ToCharArray()) {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typeSpeed);
        }
        skipButton.enabled = true;

        // Check if we need to display options buttons.
        if (dialogueData.IsQuestion) {
            skipButton.enabled = false;
            buttonUI.DialogueButtons = dialogueData.SetButtons(buttonUI.DialogueButtons, AdvanceDialogue);
            buttonUI.HideOnClick();
        }

        dialogueIsRunning = false;
        StopCoroutine(readDialogue);
    }

    private void SkipDialogue() {
        StopCoroutine(readDialogue);
        dialogueText.text = dialogueData.DialogueLine;
        dialogueIsRunning = false;
        skipButton.enabled = true;

        // Check if we need to display options buttons.
        if (dialogueData.IsQuestion) {
            buttonUI.DialogueButtons = dialogueData.SetButtons(buttonUI.DialogueButtons, AdvanceDialogue);
            buttonUI.HideOnClick();
        }
    }

    private void ShowCharacter()
    {
        // Character portrait needs to get the name+mood
        characterPortraitName = characterName.text + "_" + dialogueData.DialogueMood;
        Texture currentPortrait = (Texture)Resources.Load("Characters/" + characterName.text + "/" + characterPortraitName, typeof(Texture));
        characterPortrait.texture = currentPortrait;
        characterPortrait.SetNativeSize();
        characterPortrait.canvasRenderer.SetAlpha(0);
        characterPortrait.CrossFadeAlpha(1, 0.25f, false);
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
