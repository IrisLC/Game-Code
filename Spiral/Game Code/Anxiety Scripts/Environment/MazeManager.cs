using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeManager : MonoBehaviour
{
    /// <summary>
    /// Ends the maze with a call to the gameManager
    /// </summary>
    /// <param name="Succeeded">if the player made it to the goal</param>
    public static void FinishLevel(bool Succeeded)
    {
        if (GameManager.CurrentLevelIndex != GameManager.CurrentMazeIndex) return;


        Referencer.gameManager.SucceededMaze = Succeeded;
        GameManager.LoadOffice();
    }
}
