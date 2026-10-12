using UnityEngine;
using UnityEngine.Assertions;

public class Printer : MonoBehaviour
{
    static Printer instance;
    /// <summary>
    /// The spot where paper should be created
    /// </summary>
    [SerializeField] Transform PrintPoint;
    /// <summary>
    /// The paper being created
    /// </summary>
    [SerializeField] GameObject PaperObject;
    /// <summary>
    /// A distance offset so multiple papers are not created in the same position
    /// </summary>
    public static float PrintOffset;
    public static float PrintOffsetDelta { get; private set; } = 0.05f;
    static RenderTexture renderTexture;

    void Awake()
    {
        instance = this;
        renderTexture = Resources.Load<RenderTexture>("RenderTextures/PaperPrint");
        Assert.IsNotNull(renderTexture);
    }

    /// <summary>
    /// Creates a new Paper object
    /// </summary>
    /// <param name="spot">the SortingSpot the printed paper should go in</param>
    /// <param name="PaperCover">the texture of the paper</param>
    public void Print()
    {
        if (SubmittedIRQueue.IsEmpty) return;

        bool WasCorrect = SubmittedIRQueue.CurrentCompletion;

        Paper.SortingSpot spot;
        if (WasCorrect)
        {
            spot = Paper.SortingSpot.Correct;
        }
        else
        {
            spot = Paper.SortingSpot.Incorrect;
        }

        // Create paper and increase offset
        GameObject CreatedPaper = Instantiate(instance.PaperObject, instance.PrintPoint, false);
        CreatedPaper.transform.position = instance.PrintPoint.position.Add(y: PrintOffset);
        PrintOffset += PrintOffsetDelta;

        // Set variables
        Paper PaperScript = CreatedPaper.GetComponent<Paper>();

        PaperScript.AssignSpot(spot);
        PaperScript.AssignTexture(renderTexture);
        PaperScript.AssignName(SubmittedIRQueue.CurrentName);

        SubmittedIRQueue.RemoveIncidentReport();
    }
}
