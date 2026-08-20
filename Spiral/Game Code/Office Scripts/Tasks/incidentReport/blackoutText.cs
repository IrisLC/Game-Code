using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem;

public class blackoutText : ATask
{
    public override TaskType Type { get => TaskType.IncidentReport; }

    /// <summary>
    /// The text of the incident report
    /// </summary>
    [SerializeField] private TextMeshProUGUI TargetText;
    /// <summary>
    /// The color that the highlight will be
    /// </summary>
    [SerializeField] private string blackoutColorHex = "rgba(0, 0, 0, 0.5)";

    /// <summary>
    /// The unmarked text of the incident report
    /// </summary>
    private string originalText;
    /// <summary>
    /// The indexes of words that are marked
    /// </summary>
    private readonly HashSet<int> blackedOutWordIndexes = new HashSet<int>();

    /// <summary>
    /// The words to start blacked out. 
    ///  Currently a holdover from before the TaskFactory scripts, 
    ///  but kept around as a possible solution for keeping task progress between scenes.
    /// </summary>
    [Header("Debugging to find correct words")]
    [SerializeField] private List<int> startingBlackoutWordIndexes = new List<int>();

    /// <summary>
    /// The words expected to be blacked out.
    /// </summary>
    [Header("Correct Words")]
    [SerializeField] private List<int> ExpectedWordIndexes = new List<int>();

    /// <summary>
    /// The minimum PercentageCorrect that will result in the task being successful
    /// </summary>
    [SerializeField] private float SuccessPercentage;

    /// <summary>
    /// The percentage of correctly blacked out words.
    /// </summary>
    public float percentageCorrect = 0;
    /// <summary>
    /// How much censored words that were not expected to be correct impacts the percentageCorrect
    /// </summary>
    [SerializeField] float extraCountModifier = .5f;

    /// <summary>
    /// The possible states of highlight modification when a clicked mouse is dragged over a highlightable word.
    /// </summary>
    enum HighlightState { Null, Add, Remove }
    /// <summary>
    /// The current state of highlight modification when a clicked mouse is dragged over a highlightable word.
    /// Null means this is the first word being clicked on, at which point the otherstates will be determined. 
    /// </summary>
    HighlightState highlightState = HighlightState.Null;

    /// <summary>
    /// A reference to the lambda for when the interact action is let go of
    /// </summary>
    Action<InputAction.CallbackContext> interactionReleasedLambda;
    /// <summary>
    /// A reference to the lambda for when the mouse is moved
    /// </summary>
    Action<InputAction.CallbackContext> mouseMovedLamda;

    /// <summary>
    /// Sets the starting values for the task, called by the factory
    /// </summary>
    /// <param name="taskName">the name of the task</param>
    /// <param name="targetText">the TextMeshPro component holding the text</param>
    /// <param name="expectedWordIndexes">the indexes of words expected to be censored</param>
    public void AssignTaskValues(string taskName, TextMeshProUGUI targetText, List<int> expectedWordIndexes)
    {
        TaskName = taskName;
        TargetText = targetText;
        ExpectedWordIndexes = expectedWordIndexes;
    }

    void Awake()
    {
        // Setup the context lambdas
        interactionReleasedLambda = (context) => NullHighlightState();
        mouseMovedLamda = (context) => OnMouseMove();

        originalText = TargetText.text;
        // Blackout any words that should start blacked out
        foreach (int index in startingBlackoutWordIndexes)
        {
            blackedOutWordIndexes.Add(index);

            RebuildText(index);
        }
    }

    /// <summary>
    /// Subscribes to the events
    /// </summary>
    void OnEnable()
    {
        GameManager.OfficeInputs.Look.performed += mouseMovedLamda;
        GameManager.OfficeInputs.Interact.started += mouseMovedLamda;
        GameManager.OfficeInputs.Interact.canceled += interactionReleasedLambda;
    }

    /// <summary>
    /// Unsubscribes from the events
    /// </summary>
    void OnDisable()
    {
        GameManager.OfficeInputs.Look.performed -= mouseMovedLamda;
        GameManager.OfficeInputs.Interact.started -= mouseMovedLamda;
        GameManager.OfficeInputs.Interact.canceled -= interactionReleasedLambda;
    }

    /// <summary>
    /// Set the highlightState to null, called when interact is let go of
    /// </summary>
    void NullHighlightState()
    {
        highlightState = HighlightState.Null;
    }

    /// <summary>
    /// When the mouse is moved, checks if it is being clicked, and if so tries to toggle a word
    /// </summary>
    void OnMouseMove()
    {
        if (GameManager.OfficeInputs.Interact.IsPressed())
        {
            TryToggleClickedWord(GameManager.OfficeInputs.Look.ReadValue<Vector2>());
        }
    }

    /// <summary>
    /// Checks for if a highlightable word is being clicked on
    /// </summary>
    /// <param name="clickPosition">the position of the mouse</param>
    private void TryToggleClickedWord(Vector2 clickPosition)
    {
        int wordIndex = TMP_TextUtilities.FindIntersectingWord(TargetText, clickPosition, null);

        if (wordIndex == -1) return;
        // If we've found a word, move to toggling the word
        ToggleWord(wordIndex);
    }

    /// <summary>
    /// Either marks a clicked word or removes it being marked
    /// </summary>
    /// <param name="wordIndex">the index of the clicked word</param>
    public void ToggleWord(int wordIndex)
    {
        // Word is in the wordIndex and mouse is just now being clicked or is removing words
        if (blackedOutWordIndexes.Contains(wordIndex) &&
            (highlightState == HighlightState.Null || highlightState == HighlightState.Remove))
        {
            highlightState = HighlightState.Remove;
            blackedOutWordIndexes.Remove(wordIndex);
            RebuildText(wordIndex);
        }
        // Word is not in the wordIndex and mouse is just now being clicked or we are adding words
        else if (!blackedOutWordIndexes.Contains(wordIndex) &&
            (highlightState == HighlightState.Null || highlightState == HighlightState.Add))
        {
            highlightState = HighlightState.Add;
            blackedOutWordIndexes.Add(wordIndex);
            RebuildText(wordIndex);
        }


    }

    /// <summary>
    /// Adds or removes highlight from a given word
    /// </summary>
    /// <param name="wordIndex">the index of the word being modified</param>
    private void RebuildText(int wordIndex)
    {
        // Get the text info of the text and the wordInfo for the clicked word
        TMP_TextInfo textInfo = TargetText.textInfo;
        TMP_WordInfo wordInfo = textInfo.wordInfo[wordIndex];

        // Check for special adjacency cases
        if (!CombineDotComOrEmail(textInfo, wordIndex, ref wordInfo))
        {
            // Will only ever be a (.com or email) OR (a date or time)
            CombineDatesOrTimes(textInfo, wordIndex, ref wordInfo);
        }

        // Get the first character of the word we are modifying
        int start = wordInfo.firstCharacterIndex;

        StringBuilder sb = new StringBuilder(wordInfo.GetWord());

        int length = wordInfo.characterCount;

        // Splits the text in two around the modified word, storing the characterIndex right before the word and right after the word
        int firstHalfEnd = textInfo.characterInfo[start].index;
        int secondHalfStart = textInfo.characterInfo[start].index + length;

        // If adding highlights insert the mark tags before and after the word
        if (highlightState == HighlightState.Add)
        {
            sb.Insert(length, "</mark>");
            sb.Insert(0, $"<mark={blackoutColorHex}>");
        }
        else
        {
            // If removing highlights we move the ends of the halved text to be before and after the marks.
            // This results in, when everything is recombined, the marks are not included, effectively deleting them. 
            firstHalfEnd -= $"<mark={blackoutColorHex}>".Length;
            secondHalfStart += "</mark>".Length;
        }

        // Get the halves of the text as strings, and recombine them with the constructed modified word
        string FirstHalf = TargetText.text.Substring(0, firstHalfEnd);
        string SecondHalf = TargetText.text.Substring(secondHalfStart);
        TargetText.text = FirstHalf + sb.ToString() + SecondHalf;

        // Forces TargetText to reparse the text to include the mark tags
        TargetText.ForceMeshUpdate(false, true);
    }

    public override void Submit()
    {
        GetMatchPercentage();

        FireEvent(this, EvaluateTask(), 3);
    }

    /// <summary>
    /// Gets and assigns percentageCorrect based on how many words were correctly highlighted
    /// </summary>
    /// <returns>the new value of percentageCorrect</returns>
    public float GetMatchPercentage()
    {
        if (ExpectedWordIndexes == null || ExpectedWordIndexes.Count == 0)
            return 0f;

        int matchCount = 0;
        int extraCount = 0;

        foreach (int index in blackedOutWordIndexes)
        {
            if (ExpectedWordIndexes.Contains(index))
            {
                ++matchCount;
            }
            else
            {
                ++extraCount;
            }
        }

        return percentageCorrect =
            matchCount / (ExpectedWordIndexes.Count + (extraCount * extraCountModifier)) * 100f;
    }

    protected override bool EvaluateTask()
    {
        if (percentageCorrect >= SuccessPercentage)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Combines the three separate words in an email address or two seperate words of a website into one
    /// </summary>
    /// <param name="textInfo"></param>
    /// <param name="wordIndex"></param>
    /// <param name="wordInfo"></param>
    bool CombineDotComOrEmail(TMP_TextInfo textInfo, int wordIndex, ref TMP_WordInfo wordInfo)
    {
        //if the word clicked on is .com then add the previous word
        bool dotComFound = false;
        bool emailFound = false;

        if (wordInfo.GetWord().Equals("com") &&
            CombinePastCharacter(false, '.', textInfo, wordIndex, ref wordInfo))
        {
            --wordIndex;
            dotComFound = true;
        }

        //check if the word is on either side of an \@ and if so add the mirroring word
        if (CombinePastCharacter(false, '@', textInfo, wordIndex, ref wordInfo))
        {
            if (dotComFound) return true;
            emailFound = true;

        }
        else if (CombinePastCharacter(true, '@', textInfo, wordIndex, ref wordInfo))
        {
            emailFound = true;
            ++wordIndex;
        }

        if (dotComFound && !emailFound) return true;

        // if the starting word wasn't already .com then check the end of the word for .com
        if (CombinePastCharacter(true, '.', "com", textInfo, wordIndex, ref wordInfo))
        {
            dotComFound = true;
        }

        return emailFound || dotComFound;
    }

    void CombineDatesOrTimes(TMP_TextInfo textInfo, int wordIndex, ref TMP_WordInfo wordInfo)
    {
        bool TwoNumber = Regex.IsMatch(wordInfo.GetWord(), @"^\d{2}$");
        bool FourNumber = TwoNumber ? false : Regex.IsMatch(wordInfo.GetWord(), @"^\d{4}$");

        if (!TwoNumber && !FourNumber) return;

        // check for time
        if (TwoNumber && (CombinePastCharacter(false, ':', @"^\d{2}$", textInfo, wordIndex, ref wordInfo)
            || CombinePastCharacter(true, ':', @"^\d{2}$", textInfo, wordIndex, ref wordInfo)))
        {
            return;
        }

        //check left and right for dates
        if (CombinePastCharacter(false, '/', @"^\d{2}$", textInfo, wordIndex, ref wordInfo))
        {
            if (FourNumber) --wordIndex;
        }
        else if (!FourNumber && CombinePastCharacter(true, '/', @"^\d{2}$", textInfo, wordIndex, ref wordInfo))
        {
            ++wordIndex;
        }
        else return;

        // if its a year then look for the month
        if (FourNumber)
        {
            CombinePastCharacter(false, '/', @"^\d{2}$", textInfo, wordIndex, ref wordInfo);
        }
        else
        {
            //if its not a year then look for the year
            CombinePastCharacter(true, '/', @"^\d{4}$", textInfo, wordIndex, ref wordInfo);
        }

    }

    /// <summary>
    /// Looks past a provided character and attempts to add the word beyond to the provided wordInfo
    /// </summary>
    /// <param name="checkRight">whether to look to the character on the right or left, true = right</param>
    /// <param name="seperator">the character expected to be inbetween one word and the next</param>
    /// <param name="textInfo"></param>
    /// <param name="wordIndex"></param>
    /// <param name="wordInfo"></param>
    /// <returns>true iff there was a word found past the seperator character</returns>
    bool CombinePastCharacter(bool checkRight, char seperator, TMP_TextInfo textInfo, int wordIndex, ref TMP_WordInfo wordInfo)
    {
        int direction = checkRight ? 1 : -1;
        int index = checkRight ? wordInfo.lastCharacterIndex : wordInfo.firstCharacterIndex;

        if (textInfo.characterInfo[index + direction].character.Equals(seperator))
        {
            if (checkRight)
            {
                wordInfo.lastCharacterIndex = textInfo.wordInfo[wordIndex + direction].lastCharacterIndex;
            }
            else
            {
                wordInfo.firstCharacterIndex = textInfo.wordInfo[wordIndex + direction].firstCharacterIndex;
            }

            wordInfo.characterCount += textInfo.wordInfo[wordIndex + direction].characterCount + 1;

            //Adds or Removes the found word to blackedOutWordIndexes
            if (highlightState == HighlightState.Add)
            {
                blackedOutWordIndexes.Add(wordIndex + direction);
            }
            else
            {
                blackedOutWordIndexes.Remove(wordIndex + direction);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Looks past a provided character and attempts to find an expected word and adds it to the provided wordInfo
    /// </summary>
    /// <param name="checkRight">whether to look to the character on the right or left, true = right</param>
    /// <param name="seperator">the character expected to be inbetween one word and the next</param>
    /// <param name="FollowingString">the word expected to be on the other side of the seperator</param>
    /// <param name="textInfo"></param>
    /// <param name="wordIndex"></param>
    /// <param name="wordInfo"></param>
    /// <returns>true iff there was a word found past the seperator character</returns>
    bool CombinePastCharacter(bool checkRight, char seperator, string FollowingString, TMP_TextInfo textInfo, int wordIndex, ref TMP_WordInfo wordInfo)
    {
        int direction = checkRight ? 1 : -1;
        int index = checkRight ? wordInfo.lastCharacterIndex : wordInfo.firstCharacterIndex;

        if (textInfo.characterInfo[index + direction].character.Equals(seperator) && Regex.IsMatch(textInfo.wordInfo[wordIndex + direction].GetWord(), FollowingString))
        {
            if (checkRight)
            {
                wordInfo.lastCharacterIndex = textInfo.wordInfo[wordIndex + direction].lastCharacterIndex;
            }
            else
            {
                wordInfo.firstCharacterIndex = textInfo.wordInfo[wordIndex + direction].firstCharacterIndex;
            }

            wordInfo.characterCount += textInfo.wordInfo[wordIndex + direction].characterCount + 1;

            //Adds or Removes the found word to blackedOutWordIndexes
            if (highlightState == HighlightState.Add)
            {
                blackedOutWordIndexes.Add(wordIndex + direction);
            }
            else
            {
                blackedOutWordIndexes.Remove(wordIndex + direction);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Resets the report to have no highlights.
    /// </summary>
    public void ClearBlackouts()
    {
        blackedOutWordIndexes.Clear();
        TargetText.text = originalText;
    }
}


