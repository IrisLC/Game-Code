using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

//TODO: add logic so boxes can't go off the side of the screen
public class EmailTaskFactory
{
    /// <summary>
    /// The GameObject that will serve as the framework around which the email will be constructed
    /// </summary>
    GameObject EmailTemplate = AssetDatabase.LoadAssetAtPath<GameObject>
        ("Assets/Prefabs/UI/Tasks/Templates/Email/EmailTemplate.prefab");
    /// <summary>
    /// The GameObject that will serve as the framework for all the boxes inserted throughout the email
    /// </summary>
    GameObject WordDropBoxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>
        ("Assets/Prefabs/UI/Tasks/Templates/Email/EmailWordDropboxTemplate.prefab");
    /// <summary>
    /// The GameObject that will serve as the framework for all the words in the word bank
    /// </summary>
    GameObject DraggableWordPrefab = AssetDatabase.LoadAssetAtPath<GameObject>
        ("Assets/Prefabs/UI/Tasks/Templates/Email/EmailDraggableWordTemplate.prefab");

    /// <summary>
    /// The parent GameObject for all the others, assigned as the Instantiated EmailTemplate
    /// </summary>
    GameObject Container;
    /// <summary>
    /// The GameObject holding the textMeshComponent (the TextMeshPro component that will be the email's text)
    /// </summary>
    GameObject TextObject;
    /// <summary>
    /// The GameObject for the WordBank
    /// </summary>
    GameObject WordBank;

    /// <summary>
    /// The TextMeshPro component that will be the email's text 
    /// </summary>
    TextMeshProUGUI textMeshComponent;

    /// <summary>
    /// The index for the TextObject in the EmailTemplate
    /// </summary>
    const int TextObjectIndex = 0;
    /// <summary>
    /// The index for the WordBank in the EmailTemplate
    /// </summary>
    const int WordBankIndex = 1;
    /// <summary>
    /// The index for the directions in the EmailTemplate
    /// </summary>
    const int DirectionsIndex = 3;

    /// <summary>
    /// The string that marks where every WordDropBox prefab will go
    /// </summary>
    public const string BoxTag = "<box>"; // if tag is changed to not be surrounded by <>, some code will need to be changed, ctrl+f //CHANGE IF REMOVING <> FROM TAG

    /// <summary>
    /// The width of the BoxTag text
    /// </summary>
    float BoxTagLength;

    /// <summary>
    /// The list of all created WordDropBox components
    /// </summary>
    List<WordDropBox> BoxInstances = new List<WordDropBox>();
    /// <summary>
    /// The ordered list of every EmailLogic.WordTypes enum for the WordDropBoxes ([0] is the first box, final entry is the last box)
    /// </summary>
    List<EmailLogic.WordTypes> BoxWordTypes;

    /// <summary>
    /// The width of the WordDropBox prefab, used for <space> tags in the text so that WordDropBoxes are not covering text
    /// </summary>
    float boxWidth { get => WordDropBoxPrefab.transform.GetChild(TextObjectIndex).GetComponent<RectTransform>().rect.width; }
    /// <summary>
    /// The RectTransform of Container GameObject
    /// </summary>
    RectTransform ContainerRect;
    /// <summary>
    /// The attached EmailLogic script
    /// </summary>
    EmailLogic logic;

    /// <summary>
    /// The ScriptableObject providing the data for the email
    /// </summary>
    EmailSO emailSO;

    /// <summary>
    /// A struct containing an EmailLogic.WordType and a bool representing 2/3 of the necessary data in a DraggableWord.
    /// 
    /// EmailLogic.WordType WordType represents the kind of work that the word represents.
    /// bool IsCorrect represents if the word is the correct instance of the EmailLogic.WordType for the email.
    /// 
    /// Used so a Dictionary can hold the data for a DraggableWord.
    /// </summary>
    [Serializable]
    public struct DraggableWordInfo
    {
        public EmailLogic.WordTypes WordType;
        public bool IsCorrect;
    }

    /// <summary>
    /// Creates an Email Task out of a provided ScriptableObject.
    /// 
    /// Must be called during run time in an Office scene
    /// </summary>
    /// <param name="email">The ScriptableObject providing the data for the email</param>
    /// <returns>The created GameObject for the Task</returns>
    public GameObject Create(EmailSO email)
    {
        emailSO = email;

        Assert.IsTrue(emailSO.WordBankWords.Count != 0);

        //Create the EmailTemplate framework and get its components
        Container = GameObject.Instantiate(EmailTemplate);
        logic = Container.GetComponent<EmailLogic>();
        ContainerRect = Container.GetComponent<RectTransform>();

        // Set the containers parent to be the local EmailHolder object
        Container.transform.SetParent(Referencer.OfficeReferences.EmailHolder.transform, false);
        ContainerRect.ForceUpdateRectTransforms();

        // Get the text gameObject, and assign the Email's text
        TextObject = Container.transform.GetChild(TextObjectIndex).gameObject;
        textMeshComponent = TextObject.GetComponent<TextMeshProUGUI>();
        textMeshComponent.text = email.RawText;

        // Generate the Drop Boxes
        BoxWordTypes = email.BoxRequiredWords;

        // Add spacing for the boxes
        textMeshComponent.ForceMeshUpdate(true, true);
        BoxTagLength = textMeshComponent.GetPreferredValues(BoxTag).x;
        textMeshComponent.text = textMeshComponent.text.Replace(BoxTag, $"{BoxTag} <space={boxWidth - BoxTagLength:F3}>");
        textMeshComponent.ForceMeshUpdate(true, true);

        // Create the boxes
        AddBoxes();

        // Remove the BoxTags
        textMeshComponent.text = textMeshComponent.text.Replace($"{BoxTag} <space={boxWidth - BoxTagLength:F3}>", $"<space={boxWidth:F3}>|");

        // Create the word bank
        WordBank = Container.transform.GetChild(WordBankIndex).gameObject;
        AddWordBank(email.WordBankWords);

        // Assign the text fields
        logic.TaskName = email.Name;
        Container.transform.GetChild(DirectionsIndex).gameObject.GetComponent<TextMeshProUGUI>().text = email.Description;

        return Container;
    }

    /// <summary>
    /// Creates the boxes/slots throughout the email that the given words will be placed into
    /// </summary>
    void AddBoxes()
    {
        // The number of times the BoxTag has been found
        int count = 0;

        // Loop through each word in the given text
        foreach (TMP_WordInfo wordInfo in textMeshComponent.textInfo.wordInfo)
        {
            if (wordInfo.lastCharacterIndex >= textMeshComponent.textInfo.characterInfo.Length - 1)
            {
                Debug.LogError("Reached end of textMeshComponentText. Please verify all WordDropBoxes were created in final prefab. Aaron, contact Iris");
                return;
            }
            // Get the word and check if it is equal to the BoxTag without the <>
            string word = wordInfo.GetWord();
            if (!word.Equals(BoxTag.Substring(1, BoxTag.Length - 2))) continue; //CHANGE IF REMOVING <> FROM TAG

            // get the character on either side of the word (this gets the <> of the tag)
            word = textMeshComponent.textInfo.characterInfo[wordInfo.firstCharacterIndex - 1].character + word
                + textMeshComponent.textInfo.characterInfo[wordInfo.lastCharacterIndex + 1].character; //CHANGE IF REMOVING <> FROM TAG

            if (word.Equals(BoxTag)) //BoxTag has been found
            {
                // Get the middle left position of the first character in the Tag
                // (Averages the topLeft and bottomLeft values)
                Vector3 Position = (textMeshComponent.textInfo.characterInfo[wordInfo.lastCharacterIndex + 1].topRight
                    + textMeshComponent.textInfo.characterInfo[wordInfo.lastCharacterIndex + 1].bottomRight) / 2;

                // Create a new WordBoxPrefab as a child of the textMeshComponent 
                // and set its position to the before-found middle left position.
                GameObject box = GameObject.Instantiate(WordDropBoxPrefab, textMeshComponent.transform, false);
                box.GetComponent<RectTransform>().anchoredPosition = Position;

                // Gets the WordDropBox component of the newly made GameObject
                WordDropBox wordDropBox = box.transform.GetChild(0).GetComponent<WordDropBox>();

                // Values for Dynamic movement and resizing due to resizing of the screen
                wordDropBox.TextContainer = textMeshComponent;
                wordDropBox.boxWordLastIndex = wordInfo.firstCharacterIndex - 1 - ((BoxTag.Length) * count); // Since the BoxTags will be removed, adjusts the index counts accordingly 
                wordDropBox.RequiredWordType = BoxWordTypes[count];
                // Adds the newly created WordDropBox component to BoxInstances 
                BoxInstances.Add(wordDropBox);

                // Increments the count and checks if all BoxTags have been found
                if (++count >= emailSO.BoxRequiredWords.Count) break;
            }
        }

        logic.AllBoxes = BoxInstances;
    }

    /// <summary>
    /// Creates the bank of words that the player can drag into the boxes throughout the email
    /// </summary>
    /// <param name="WordBankWords">A dictionary containing the required data for creating a word bank entry, 
    ///                             that being a string, an EmailLogic.WordType, and a bool.</param>
    void AddWordBank(Dictionary<string, DraggableWordInfo> WordBankWords)
    {
        // Loops through every Word based on the text that will be on the word
        foreach (string text in WordBankWords.Keys)
        {
            // Create a new GameObject as a child of the WordBank
            GameObject dragWord = GameObject.Instantiate(DraggableWordPrefab, WordBank.transform, false);

            DraggableWord draggableWord = dragWord.GetComponent<DraggableWord>();

            // Gets the corresponding DraggableWordInfo struct from the dictionary and assigns its values to the 
            // DraggableWord component in the newly created GameObject
            DraggableWordInfo info = WordBankWords[text];
            draggableWord.WordType = info.WordType;
            draggableWord.IsCorrect = info.IsCorrect;

            // Gives the DraggableWord a reference to TextMeshProUGUI component to allow for dynamic resizing
            draggableWord.EmailText = textMeshComponent;

            // Gives the child text object the necessary text for the word
            dragWord.transform.GetChild(TextObjectIndex).gameObject.GetComponent<TextMeshProUGUI>().text = text;
        }

        //WordBank.GetComponent<RectTransform>().ForceUpdateRectTransforms();
    }


}
