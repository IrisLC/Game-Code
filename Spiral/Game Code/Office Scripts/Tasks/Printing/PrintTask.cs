using UnityEngine;

/// <summary>
/// Temporary task for testing and showcasing print mechanics. This script should not exist in the same state by game release
/// </summary>
public class PrintTask : ATask
{
    public override TaskType Type { get => TaskType.Print; }
    /// <summary>
    /// The number of blue papers that should be created and sorted properly
    /// </summary>
    [SerializeField] int RequiredBlue;
    /// <summary>
    /// The number of green papers that should be created and sorted properly
    /// </summary>
    [SerializeField] int RequiredGreen;
    /// <summary>
    /// How much paper has been sorted
    /// </summary>
    int GivenPaper;
    /// <summary>
    /// The total number of papers that needs to be sorted
    /// </summary>
    int TotalPieces { get => RequiredBlue + RequiredGreen; }

    /// <summary>
    /// The number of correctly sorted blue papers
    /// </summary>
    int ProvidedBlue;
    /// <summary>
    /// The number of correctly sorted green papers
    /// </summary>
    int ProvidedGreen;

    protected override bool EvaluateTask()
    {
        return ProvidedBlue == RequiredBlue && ProvidedGreen == RequiredGreen;
    }

    /// <summary>
    /// Tells the bins to send info about sorted paper to this task
    /// </summary>
    public void Activate()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("PaperBin"))
        {
            go.GetComponent<Bins>().assignedTask = this;
        }
    }

    /// <summary>
    /// When paper is placed, Bins calls this method with the details of the paper that was sorted
    /// </summary>
    /// <param name="isCorrect">if the paper was correctly placed</param>
    /// <param name="AssignedSpot">the spot of the bin</param>
    public void OnPlace(bool isCorrect, Paper.SortingSpot AssignedSpot)
    {
        GivenPaper++;

        //Left = Green, Right = Blue
        if (AssignedSpot == Paper.SortingSpot.Left && isCorrect)
        {
            ProvidedGreen++;
        }
        else if (AssignedSpot == Paper.SortingSpot.Right && isCorrect)
        {
            ProvidedBlue++;
        }
        // Submit the task if all pieces of paper have been inserted, regardless of if they were done correctly
        if (GivenPaper == TotalPieces)
        {
            Submit();
        }
    }

    public override void Submit()
    {
        Printer.PrintOffset = 0;
        FireEvent(this, EvaluateTask(), 1);
    }


}
