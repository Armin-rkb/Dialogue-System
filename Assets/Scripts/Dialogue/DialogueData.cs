using System.Xml;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueData : MonoBehaviour {

    /// <summary>
    /// Story 1(unfinished):JojoDialogue
    /// Story 2: DanganronpaDialogue
    /// </summary>
    [SerializeField] private string dialogueStoryName;

    private XmlLoader xmlLoader = new XmlLoader();
    private XmlNodeList dialogueData;

    private bool isQuestion;
    public bool IsQuestion {
        get { return isQuestion; }
    }
    private bool isLocked;
    public bool IsLocked{
        get { return isLocked; }
    }

    // Xml Data
    private int dialogueID = 0;
    private int destinationID = 0;
    private string dialogueType;
    private string dialogueName;
    public string DialogueName {
        get { return dialogueName; }
    }
    private string dialogueMood;
    public string DialogueMood {
        get { return dialogueMood; }
    }
    private string dialogueLine;
    public string DialogueLine {
        get { return dialogueLine; }
    }

    private void Awake () {
        dialogueData = xmlLoader.LoadXMLdata();
        
        dialogueData = dialogueData[1].SelectSingleNode(dialogueStoryName).SelectNodes(Constants.Dialogue);
    }

    public void GetNextDialogue() {
        
        // Getting our dialogueID and parsing it to an int value. 
        string stringDialogueID = dialogueData[dialogueID].SelectSingleNode(Constants.XmlDialogueID).Value;
        dialogueID = int.Parse(stringDialogueID);
        print("DialogueID: " + dialogueID);

        // Getting our dialogueType 
        string stringDialogueType = dialogueData[dialogueID].SelectSingleNode(Constants.XmlDialogueType).Value;
        dialogueType = stringDialogueType;
        print("Type: " + dialogueType);

        // Getting the portrait of the character 
        string stringDialoguePortrait = dialogueData[dialogueID].SelectSingleNode(Constants.Line).SelectSingleNode(Constants.XmlDialoguePortrait).Value;
        dialogueName = stringDialoguePortrait;
        print("Character: " + dialogueName);

        // Getting the portrait of the character 
        string stringDialogueMood = dialogueData[dialogueID].SelectSingleNode(Constants.Line).SelectSingleNode(Constants.XmlDialogueMood).Value;
        dialogueMood = stringDialogueMood;
        print("Mood: " + dialogueMood);

        // Getting the next dialogueID if its not a question and parsing it to an int value. 
        if (dialogueType == Constants.Normal) { 
            string stringDestinationID = dialogueData[dialogueID].FirstChild.SelectSingleNode(Constants.XmlDialogueDestination).Value;
            destinationID = int.Parse(stringDestinationID);
            print("DestinationID: " + destinationID);
            isQuestion = false;
        } else if (dialogueType == Constants.Question) {
            for (int i = 0; i < dialogueData[dialogueID].SelectSingleNode(Constants.Options).ChildNodes.Count; i++) {
                print("Option: " + dialogueData[dialogueID].SelectSingleNode(Constants.Options).SelectNodes(Constants.Option)[i].InnerText);
                print("Choice: " + dialogueData[dialogueID].SelectSingleNode(Constants.Options).SelectNodes(Constants.Option)[i].SelectSingleNode(Constants.XmlDialogueChoice).Value);
            }
            isQuestion = true;
            isLocked = true;
        }

        // Setting the dialogue line.
        dialogueLine = dialogueData[dialogueID].SelectSingleNode(Constants.Line).InnerText;
        
        print(dialogueLine);

        dialogueID = destinationID;

        print("---------------------------------");
    }

    public void SetButtons(List<Button> buttons, Action callback) {
        // Get the amount of options we have.
        int choiceAmount = dialogueData[dialogueID].SelectSingleNode(Constants.Options).ChildNodes.Count;

        for (int i = 0; i < choiceAmount; i++) {
            // Set the button text
            string buttonText = dialogueData[dialogueID].SelectSingleNode(Constants.Options).SelectNodes(Constants.Option)[i].SelectSingleNode(Constants.XmlDialogueChoice).Value;
            buttons[i].gameObject.GetComponentInChildren<Text>().text = buttonText;

            // Display the correct buttons.
            buttons[i].gameObject.SetActive(true);


            XmlNode buttonNode = dialogueData[dialogueID].SelectSingleNode(Constants.Options).SelectNodes(Constants.Option)[i];
            buttons[i].onClick.AddListener(() => {
                SetButtonDestination(buttonNode);
                callback();
                });
        }

        // After setting the buttons make sure the next line won't be a question.
        isQuestion = false;
    }

    private void SetButtonDestination(XmlNode node) {
        string stringNextDialogueID = node.SelectSingleNode(Constants.XmlDialogueDestination).Value;
         
        dialogueID = int.Parse(stringNextDialogueID); ;
        dialogueType = node.SelectSingleNode(Constants.XmlDialogueType).Value;
        dialogueLine = node.InnerText;
        dialogueName = node.SelectSingleNode(Constants.XmlDialoguePortrait).Value;
        dialogueMood = node.SelectSingleNode(Constants.XmlDialogueMood).Value;

        // Lock the dialogue.
        isLocked = false;
    }
}
