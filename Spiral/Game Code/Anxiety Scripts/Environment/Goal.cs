using UnityEngine;

/// <summary>
/// The goal in the maze the player is trying to reach
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class Goal : MonoBehaviour
{

    GameObject Player;

    void Start()
    {
        Player = Referencer.AnxietyReferences.PlayerGameObject;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            MazeManager.FinishLevel(true);
        }
    }
}
