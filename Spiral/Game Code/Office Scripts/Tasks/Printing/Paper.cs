using UnityEngine;

public class Paper : MonoBehaviour
{
    /// <summary>
    /// The possible spots the paper can go into
    /// </summary>
    public enum SortingSpot { Left, Right }
    /// <summary>
    /// The spot that the paper should go into
    /// </summary>
    SortingSpot sortingSpot;

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
    public void AssignTexture(Texture image)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        renderer.material.SetTexture("_MainTex", image);
    }
}
