using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmailSO", menuName = "Scriptable Objects/EmailSO")]
public class EmailSO : TaskSO
{
    /// <summary>
    /// The RequiredWord for each drop box
    /// </summary>
    public List<EmailLogic.WordTypes> BoxRequiredWords;
    /// <summary>
    /// The list of word bank entries, see WordBankDictionaryEntry struct declaration below for the reason for this interstitial list.
    /// </summary>
    public List<WordBankDictionaryEntry> wordBankEntriesList = new List<WordBankDictionaryEntry>();

    /// <summary>
    /// The dictionary of word bank entries that will be passed to the TaskFactory
    /// </summary>
    public Dictionary<string, EmailTaskFactory.DraggableWordInfo> WordBankWords = new Dictionary<string, EmailTaskFactory.DraggableWordInfo>();

    /// <summary>
    /// Assigns values to all stored variables
    /// </summary>
    /// <param name="rawText">the body text of the email</param>
    /// <param name="name">the name of the task</param>
    /// <param name="description">the description given to the player as to the purpose of the email</param>
    /// <param name="boxRequiredWords">the list of requiredWords for each drop box</param>
    /// <param name="wordBankWords">The dictionary of word bank entries that will be passed to the TaskFactory, 
    ///                               stored as a list of WordBankDictionaryEntry</param>
    public void Create(string rawText, string name, string description,
        List<EmailLogic.WordTypes> boxRequiredWords,
        Dictionary<string, EmailTaskFactory.DraggableWordInfo> wordBankWords)
    {
        taskType = ATask.TaskType.Email;

        RawText = rawText;
        Name = name;
        Description = description;
        BoxRequiredWords = boxRequiredWords;

        // Converts the WordBankWords dictionary into the wordBankEntriesList List
        foreach (string s in wordBankWords.Keys)
        {
            WordBankDictionaryEntry entry = new WordBankDictionaryEntry
            {
                key = s,
                value = wordBankWords[s]
            };

            wordBankEntriesList.Add(entry);
        }
    }

    /// <summary>
    /// Converts the wordBankEntriesList list back into a dictionary. Call right before creating the prefab in the EmailTaskFactory
    /// </summary>
    /// <returns>the EmailSO</returns>
    public EmailSO Initialize()
    {
        foreach (WordBankDictionaryEntry entry in wordBankEntriesList)
        {
            WordBankWords.Add(entry.key, entry.value);
        }

        return this;
    }
}

/// <summary>
/// For the task creator and factory it is easier to store the word bank words as a dictionary, 
///  but a dictionary cannot be serialized, so this struct exists as a stopgap between the creator and factory. 
///  Serialization allows for modification of assets in the editor, but also is a requirement for data being 
///  stored after runtime in a scriptable object.
/// </summary>
[Serializable]
public struct WordBankDictionaryEntry
{
    public string key;
    public EmailTaskFactory.DraggableWordInfo value;
}
