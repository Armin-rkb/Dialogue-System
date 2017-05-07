using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {

    [SerializeField] private DialogueData dialogueData;

    // UI elements
    [SerializeField] private Text dialogueText;
    [SerializeField] private Text characterName;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private List<Button> dialogueButtons = new List<Button>();

    private void Start() {
        // Hiding the button on default.
        DisplayButtons(false);

        // Making sure the buttons will always hide when pressing on them.
        for (int i = 0; i < dialogueButtons.Count; i++) { 
            dialogueButtons[i].onClick.AddListener(() => DisplayButtons(false));
        }
    }

    private void Update() {
        // Set the next test (Needs to be reworked)
        if (Input.GetMouseButtonDown(0) && !dialogueData.IsLocked) {
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue() {
        dialogueData.GetNextDialogue();

        dialogueText.text = dialogueData.DialogueLine;
        characterName.text = dialogueData.DialogueName;
        // Character portrait needs to get the name+mood

        if (dialogueData.IsQuestion) {
            DisplayButtons(true);
            dialogueData.SetButtons(dialogueButtons);
        }
    }

    private void DisplayButtons(bool isDisplayed) {
        for (int i = 0; i < dialogueButtons.Count; i++) {
            dialogueButtons[i].gameObject.SetActive(isDisplayed);
        }
    }

}
