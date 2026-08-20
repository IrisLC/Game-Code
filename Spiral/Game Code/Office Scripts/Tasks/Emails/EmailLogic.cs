using System.Collections.Generic;
using UnityEngine;

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
    public List<WordDropBox> AllBoxes;

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
        int count = 0;
        // See how many words are correct
        foreach (WordDropBox box in AllBoxes)
        {
            if (box.CheckWord()) count++;
        }
        NumIncorrectAnswers = AllBoxes.Count - count;
        return NumIncorrectAnswers == 0;
    }

}
