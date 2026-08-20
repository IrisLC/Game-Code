using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class IncidentReportTaskFactory
{
    /// <summary>
    /// The GameObject that will serve as the framework around which the email will be constructed
    /// </summary>
    GameObject IRTemplate = AssetDatabase.LoadAssetAtPath<GameObject>
        ("Assets/Prefabs/UI/Tasks/Templates/IncidentReportTemplate.prefab");

    /// <summary>
    /// The parent GameObject for all the others, assigned as the Instantiated EmailTemplate
    /// </summary>
    GameObject Container;
    /// <summary>
    /// The GameObject holding the textMeshComponent (the TextMeshPro component that will be the email's text)
    /// </summary>
    GameObject TextObject;
    /// <summary>
    /// The TextMeshPro component that will be the email's text 
    /// </summary>
    TextMeshProUGUI textMeshComponent;

    /// <summary>
    /// The index for the TextObject in the EmailTemplate
    /// </summary>
    const int TextObjectIndex = 0;
    /// <summary>
    /// The RectTransform of Container GameObject
    /// </summary>
    RectTransform ContainerRect;
    /// <summary>
    /// The attached blackoutText script
    /// </summary>
    blackoutText logic;
    /// <summary>
    /// The ScriptableObject providing the data for the email
    /// </summary>
    IncidentReportSO ir;

    /// <summary>
    /// The string that marks where every WordDropBox prefab will go
    /// </summary>
    public const string CensorTagStart = "<censor>"; // if tag is changed to not be surrounded by <>, some code will need to be changed, ctrl+f //CHANGE IF REMOVING <> FROM TAG
    public const string CensorTagEnd = "</censor>";

    List<int> censoredWordIndexes;

    public GameObject Create(IncidentReportSO IRSO)
    {
        ir = IRSO;

        censoredWordIndexes = new List<int>();

        Container = GameObject.Instantiate(IRTemplate);
        logic = Container.GetComponent<blackoutText>();
        ContainerRect = Container.GetComponent<RectTransform>();

        // Set the containers parent to be the local IRHolder object
        Container.transform.SetParent(Referencer.OfficeReferences.IRHolder.transform, false);
        ContainerRect.ForceUpdateRectTransforms();

        // Get the text gameObject, and assign the IR's text
        TextObject = Container.transform.GetChild(TextObjectIndex).gameObject;
        textMeshComponent = TextObject.GetComponent<TextMeshProUGUI>();
        textMeshComponent.text = ir.RawText;
        textMeshComponent.ForceMeshUpdate(true, true);

        FindIndexes();
        logic.AssignTaskValues(ir.name, textMeshComponent, censoredWordIndexes);

        return Container;
    }

    void FindIndexes()
    {
        bool inTag = false;

        for (int i = 0; i < textMeshComponent.textInfo.wordInfo.Length; i++)
        {
            if (i > textMeshComponent.textInfo.wordCount) break;

            TMP_WordInfo wordInfo = textMeshComponent.textInfo.wordInfo[i];

            // Get the word and check if it is equal to the CensorTags without the <> (or </>)
            string word = wordInfo.GetWord();

            if (!inTag)
            {
                if (!word.Equals(CensorTagStart.Substring(1, CensorTagStart.Length - 2))) continue; //CHANGE IF REMOVING <> FROM TAG

                // get the character on either side of the word (this gets the <> of the tag)
                word = textMeshComponent.textInfo.characterInfo[wordInfo.firstCharacterIndex - 1].character + word
                    + textMeshComponent.textInfo.characterInfo[wordInfo.lastCharacterIndex + 1].character; //CHANGE IF REMOVING <> FROM TAG

                if (word.Equals(CensorTagStart))
                {
                    inTag = true;
                    --i;
                    StripTag(true, wordInfo);
                }

                //either we've found a start tag or we've found a word that is the same as the contents of the tag, 
                // either way we are done with this word
                continue;
            }
            else
            {
                // first checks to see if we've found the end tag, if so go to the next word, otherwise add that word's index to the list
                if (word.Equals(CensorTagEnd.Substring(2, CensorTagEnd.Length - 3)))
                {
                    // get the character on either side of the word (this gets the </> of the tag)
                    string borderedWord = "" + textMeshComponent.textInfo.characterInfo[wordInfo.firstCharacterIndex - 2].character
                        + textMeshComponent.textInfo.characterInfo[wordInfo.firstCharacterIndex - 1].character + word
                        + textMeshComponent.textInfo.characterInfo[wordInfo.lastCharacterIndex + 1].character; //CHANGE IF REMOVING </> FROM TAG


                    if (borderedWord.Equals(CensorTagEnd))
                    {
                        inTag = false;
                        --i;
                        StripTag(false, wordInfo);
                        continue;
                    }
                }
            }

            //if we are here then we are in between the start and end tags 
            censoredWordIndexes.Add(i);
        }
    }

    void StripTag(bool startTag, TMP_WordInfo wordInfo)
    {
        int startOffset = startTag ? 1 : 2;
        int tagLength = startTag ? CensorTagStart.Length : CensorTagEnd.Length;

        int firstIndex = textMeshComponent.textInfo.characterInfo[wordInfo.firstCharacterIndex - startOffset].index;

        textMeshComponent.text = textMeshComponent.text.Remove(firstIndex, tagLength);

        textMeshComponent.ForceMeshUpdate(true, true);
    }
}
