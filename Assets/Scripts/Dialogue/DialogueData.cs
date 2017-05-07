using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueData : MonoBehaviour {

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

    private void Start () {
        dialogueData = xmlLoader.LoadXMLdata();

        // Pick the senario for our dialogue (Needs to be reworked).
        dialogueData = dialogueData[1].FirstChild.SelectNodes(Constants.Dialogue);
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

    public void SetButtons(List<Button> buttons) {
        for (int i = 0; i < dialogueData[dialogueID].SelectSingleNode(Constants.Options).ChildNodes.Count; i++) {
            string buttonText = dialogueData[dialogueID].SelectSingleNode(Constants.Options).SelectNodes(Constants.Option)[i].SelectSingleNode(Constants.XmlDialogueChoice).Value;
            buttons[i].gameObject.GetComponentInChildren<Text>().text = buttonText;

            XmlNode buttonNode = dialogueData[dialogueID].SelectSingleNode(Constants.Options).SelectNodes(Constants.Option)[i];
            buttons[i].onClick.AddListener(() => SetButtonDestination(buttonNode));
            // TODO: Remove EventListener.
        }
    }

    private void SetButtonDestination(XmlNode node) {
        string stringNextDialogueID = node.SelectSingleNode(Constants.XmlDialogueDestination).Value;
         
        dialogueID = int.Parse(stringNextDialogueID); ;
        dialogueLine = node.InnerText;
        
        // Lock the dialogue.
        isLocked = false;

        Debug.Log("Buttons Set");
    }
}
