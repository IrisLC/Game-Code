using UnityEngine;

public class Printer : MonoBehaviour
{
    /// <summary>
    /// The spot where paper should be created
    /// </summary>
    [SerializeField] Transform PrintPoint;
    /// <summary>
    /// The paper being created
    /// </summary>
    [SerializeField] GameObject Paper;
    /// <summary>
    /// A distance offset so multiple papers are not created in the same position
    /// </summary>
    public static float PrintOffset;

    void OnEnable()
    {
        // subscribe to print event that passes a sorting spot and a texture
        ComputerScript.PrintPage += Print;
    }

    void OnDisable()
    {
        // unsubscribe from print event
        ComputerScript.PrintPage -= Print;
    }

    /// <summary>
    /// Creates a new Paper object
    /// </summary>
    /// <param name="spot">the SortingSpot the printed paper should go in</param>
    /// <param name="PaperCover">the texture of the paper</param>
    void Print(Paper.SortingSpot spot, Texture PaperCover)
    {
        // Create paper and increase offset
        GameObject CreatedPaper = Instantiate(Paper, PrintPoint.position.Add(y: PrintOffset), PrintPoint.rotation);
        PrintOffset += 0.05f;

        // Set variables
        Paper PaperScript = CreatedPaper.GetComponent<Paper>();

        PaperScript.AssignSpot(spot);
        PaperScript.AssignTexture(PaperCover);
    }
}
