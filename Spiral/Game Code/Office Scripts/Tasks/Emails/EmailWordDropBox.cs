using UnityEngine;

public class EmailWordDropBox : AWordDropBox
{
    /// <summary>
    /// The WordType that this box expects
    /// </summary>
    public EmailLogic.WordTypes RequiredWordType;
    /// <summary>
    /// The index of the '|' character that this box ties its location to. 
    ///  Index is in terms of the TextContainer.textInfo.characterInfo
    /// </summary>
    public int boxWordLastIndex;

    void Update()
    {
        if (TextContainer && TextContainer.textInfo.characterInfo.Length > 8)
        {
            Vector3 Position = (TextContainer.textInfo.characterInfo[boxWordLastIndex].topRight
                + TextContainer.textInfo.characterInfo[boxWordLastIndex].bottomRight) / 2;

            parentRectTransform.anchoredPosition = Position;
        }
    }


    /// <summary>
    /// Checks if the word being held in this box is the correct word.
    /// </summary>
    /// <returns>true iff the Word in the box is of the right type and is the correct word of that type</returns>
    public bool CheckWord()
    {
        // No word is auto incorrect
        if (Word == null) return false;

        if (((EmailDraggableWord)Word).WordType == RequiredWordType && Word.IsCorrect)
        {
            return true;
        }

        return false;
    }
}
