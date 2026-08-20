using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A Scriptable object that can be slotted into a DailyModifier component. 
///  Allows for asset saving of changes to modifiers throughout days.
/// </summary>
[CreateAssetMenu(fileName = "DayModifierObject", menuName = "Scriptable Objects/DayModifierObject")]
public class DayModifierObject : ScriptableObject
{
    /// <summary>
    /// The list of prefabs that will be the tasks the player must fill out
    /// </summary>
    public List<GameObject> TasksToCreate;
    /// <summary>
    /// The amount of time in seconds between one task being added to the player's assigned tasks and the next.
    /// </summary>
    public float TimeBetweenTasks;
    /// <summary>
    /// The amount of time in seconds from the start of a day in the office, until the first new task of the day will be added to the player's assigned tasks.
    /// </summary>
    public float TimeTillFirstTask;
    /// <summary>
    /// How much anxiety is passively added to the anxiety meter every fixed update.
    /// </summary>
    public float BasicAnxietyAdder;

    /// /// <summary>
    /// How much time to remove from the current time of day if the player fails the maze. (in irl seconds)
    /// </summary>
    public int TimeLostOnFail;
    /// /// <summary>
    /// How much time to remove from the current time of day if the player completes the maze. (in irl seconds)
    /// </summary>
    public int TimeLostOnSuccess;
    /// <summary>
    /// How much value is removed from the anxiety meter whenever a task is completed successfully (should be negative).
    /// </summary>
    public float TaskSuccessAnxietyModificationValue;
    /// <summary>
    /// How much value is added to the anxiety meter whenever a task is completed unsuccessfully.
    /// </summary>
    public float TaskFailAnxietyModificationValue;
    /// /// <summary>
    /// What the anxiety meter is set to upon returning from the anxiety maze having died from oxygen loss
    /// </summary>
    public float MazeFailAnxietyPunishment;
    /// <summary>
    /// How long in seconds an office day should last
    /// </summary>
    public float LengthOfDay;
    /// <summary>
    /// Whether or not the current scene should have the tutorial information.
    /// </summary>
    public bool IsTutorial;
}
