using UnityEngine;

[CreateAssetMenu(fileName = "PrintableTask", menuName = "Scriptable Objects/PrintableTask")]
public class PrintableTask : ScriptableObject
{
    /// <summary>
    /// Which spot the paper should go into
    /// </summary>
    public Paper.SortingSpot spot;
    /// <summary>
    /// The texture for the front of the paper
    /// </summary>
    public Texture PaperCover;
}
