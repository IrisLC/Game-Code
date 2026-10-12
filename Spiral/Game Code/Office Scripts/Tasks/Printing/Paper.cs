using UnityEngine;
using UnityEngine.UI;

public class Paper : ATask
{

    /// <summary>
    /// The possible spots the paper can go into
    /// </summary>
    public enum SortingSpot { Correct, Incorrect }
    /// <summary>
    /// The spot that the paper should go into
    /// </summary>
    SortingSpot sortingSpot;

    [SerializeField] RawImage cover;

    public override TaskType Type { get => TaskType.Print; }
    bool correctlyPlaced;
    /// <summary>
    /// To be used for feedback
    /// </summary>
    Paper.SortingSpot spotPlaced;

    protected override bool EvaluateTask()
    {
        return correctlyPlaced;
    }

    /// <summary>
    /// When paper is placed, Bins calls this method with the details of the paper that was sorted
    /// </summary>
    /// <param name="isCorrect">if the paper was correctly placed</param>
    /// <param name="AssignedSpot">the spot of the bin</param>
    public void OnPlace(bool isCorrect, Paper.SortingSpot AssignedSpot)
    {
        correctlyPlaced = isCorrect;
        spotPlaced = AssignedSpot;
        Submit();
    }

    public override void Submit()
    {
        Printer.PrintOffset -= Printer.PrintOffsetDelta;
        FireEvent(this, EvaluateTask(), 1);
    }

    /// <summary>
    /// Checks if the paper was placed in the right spot
    /// </summary>
    /// <param name="PlacedSpot">the spot the paper was placed in</param>
    /// <returns>true if in the right spot</returns>
    public bool CheckRightSpot(SortingSpot PlacedSpot) => PlacedSpot == sortingSpot;

    /// <summary>
    /// Sets sortingSpot
    /// </summary>
    /// <param name="spot">the spot that the paper should go</param>
    public void AssignSpot(SortingSpot spot) => sortingSpot = spot;

    /// <summary>
    /// Assigns the given texture to the renderer component of the gameObject
    /// </summary>
    /// <param name="image">the texture being assigned</param>
    public void AssignTexture(RenderTexture renderTexture)
    {
        Texture2D snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        RenderTexture temp = RenderTexture.active;
        RenderTexture.active = renderTexture;

        snapshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        snapshot.Apply();

        cover.texture = snapshot;

        RenderTexture.active = temp;
    }

    public void AssignName(string name)
    {
        TaskName = name;
    }

    public override string GetResponse()
    {
        return CorrectionEmail.PrintCorrection1 + spotPlaced.ToString() + " reports, "
            + CorrectionEmail.PrintCorrection2 + spotPlaced.ToString() + " reports\n";
    }

}
