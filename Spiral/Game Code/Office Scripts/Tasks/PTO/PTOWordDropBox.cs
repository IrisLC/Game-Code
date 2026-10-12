public class PTOWordDropBox : AWordDropBox
{

    /// <summary>
    /// The WordType that this box expects
    /// </summary>
    public PTORequest.WordTypes RequiredWordType;


    /// <summary>
    /// Checks if the word being held in this box is the correct word.
    /// </summary>
    /// <returns>true iff the Word in the box is of the right type and is the correct word of that type</returns>
    public bool CheckWord()
    {
        // No word is auto incorrect
        if (Word == null) return false;

        if (((PTODraggableWord)Word).WordType == RequiredWordType && Word.IsCorrect)
        {
            return true;
        }

        return false;
    }
}
