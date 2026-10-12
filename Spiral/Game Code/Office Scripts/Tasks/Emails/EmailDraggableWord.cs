using System.Collections.Generic;

public class EmailDraggableWord : ADraggableWord
{
    /// <summary>
    /// The wordType that this word represents
    /// </summary>
    public EmailLogic.WordTypes WordType;

    static Dictionary<EmailLogic.WordTypes, string> ActiveCorrectWords = new();

    protected override void OnEnable()
    {
        base.OnEnable();

        if (IsCorrect)
        {
            if (!ActiveCorrectWords.TryAdd(WordType, Word))
            {
                ActiveCorrectWords[WordType] = Word;
            }
        }
    }

    public static string GetCorrectWord(EmailLogic.WordTypes wordType)
    {
        return ActiveCorrectWords[wordType];
    }
}

