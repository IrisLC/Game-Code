using System.Collections.Generic;

public class EmailLogic : ATask
{
    // These enums can be expanded as need be
    // Ways to categorize different groups of words
    public enum WordTypes
    {
        Null,
        Name,
        Notify,//ex. inform, congratulate, break the news
        Pleasantry, //ex. Happy, Regretful, concerned
        Topic, //ex. PTO, Promotion, Let you go
        Result, //ex. Approved, Denied, Pending
        Sendoff, //ex. Sincerely, Best of luck, Congratulations
        Extra //anything else needed
    }


    public override TaskType Type { get => TaskType.Email; }


    /// <summary>
    /// A list of all the WordDropBoxes for the email
    /// </summary>
    public List<EmailWordDropBox> AllBoxes;
    List<EmailWordDropBox> incorrectBoxes = new();

    /// <summary>
    /// The number of incorrectly placed DraggableWords when submitting the task
    /// </summary>
    int NumIncorrectAnswers;
    /// <summary>
    /// The weight of the task when it is completed succesfully
    /// </summary>
    int CorrectWeight = 1;
    /// <summary>
    /// The algorithm for determining the weight of the task based on how badly it was failed
    /// </summary>
    int IncorrectWeight { get => NumIncorrectAnswers / 2; }

    public override void Submit()
    {
        if (EvaluateTask())
        {
            FireEvent(this, true, CorrectWeight);
        }
        else
        {
            FireEvent(this, false, IncorrectWeight);
        }

    }

    protected override bool EvaluateTask()
    {
        // See how many words are correct
        foreach (EmailWordDropBox box in AllBoxes)
        {
            if (!box.CheckWord())
            {
                incorrectBoxes.Add(box);
            }
        }
        NumIncorrectAnswers = incorrectBoxes.Count;
        return NumIncorrectAnswers == 0;
    }

    public override string GetResponse()
    {
        string responseBody = "";

        foreach (EmailWordDropBox box in incorrectBoxes)
        {
            if (box.hasBlock)
            {
                responseBody += CorrectionEmail.EmailCorrection1 + box.Word.Word
                + CorrectionEmail.EmailCorrection2 + EmailDraggableWord.GetCorrectWord(box.RequiredWordType) + "\".\n";
            }
            else
            {
                responseBody += CorrectionEmail.EmailCorrectionBlank1 +
                    EmailDraggableWord.GetCorrectWord(box.RequiredWordType) +
                    CorrectionEmail.EmailCorrectionBlank2 + ".\n";
            }


        }

        return responseBody;
    }
}
