using System.Collections.Generic;
using UnityEngine;

public class DailyModifiers : MonoBehaviour
{
    [Header("Scriptable Object")]
    [Tooltip("Optional. Overrides any manual values with values in provided object. Values not in object will be defaulted to Manual Values")]
    [SerializeField] DayModifierObject ModifiersSO;

    [Header("Task Data")]
    /// <summary>
    /// The list of prefabs that will be the tasks the player must fill out
    /// </summary>
    [SerializeField] internal List<GameObject> TasksToCreate;
    /// <summary>
    /// The amount of time in seconds between one task being added to the player's assigned tasks and the next.
    /// </summary>
    [SerializeField] internal float TimeBetweenTasks = 15f;
    /// <summary>
    /// The amount of time in seconds from the start of a day in the office, until the first new task of the day will be added to the player's assigned tasks.
    /// </summary>
    [SerializeField] internal float TimeBeforeFirstTask = 1f;

    [Header("Anxiety Data")]
    /// <summary>
    /// How much anxiety is passively added to the anxiety meter every fixed update.
    /// </summary>
    [SerializeField] internal float BasicAnxietyAdder = .2f;
    /// <summary>
    /// How much value is removed from the anxiety meter whenever a task is completed successfully (should be negative).
    /// </summary>
    [Range(-100, 0)]
    [SerializeField] internal float TaskSuccessAnxietyModificationValue = -2;
    /// <summary>
    /// How much value is added to the anxiety meter whenever a task is completed unsuccessfully.
    /// </summary>
    [SerializeField] internal float TaskFailAnxietyModificationValue = 10;
    /// <summary>
    /// What the anxiety meter is set to upon returning from the anxiety maze having died from oxygen loss
    /// </summary>
    [SerializeField] internal float MazeFailAnxietyPunishment = 20;

    [Header("Time Data")]
    /// <summary>
    /// How long in seconds an office day should last
    /// </summary>
    [SerializeField] internal float LengthOfDay = 60;
    /// <summary>
    /// How much time to remove from the current time of day if the player fails the maze. (in irl seconds)
    /// </summary>
    [SerializeField] internal int TimeLostOnFail = 60;
    /// <summary>
    /// How much time to remove from the current time of day if the player succeeds the maze. (in irl seconds)
    /// </summary>
    [SerializeField] internal int TimeLostOnSuccess = 30;

    [Header("Misc")]
    /// <summary>
    /// Whether or not the current scene should have the tutorial information.
    /// </summary>
    [SerializeField] internal bool IsTutorial = false;
    void Awake()
    {
        //Sets variables to non empty values in the scriptable object
        if (ModifiersSO != null)
        {
            TasksToCreate = ModifiersSO.TasksToCreate != null ? ModifiersSO.TasksToCreate : TasksToCreate;
            TimeBetweenTasks = ModifiersSO.TimeBetweenTasks != 0.0f ? ModifiersSO.TimeBetweenTasks : TimeBetweenTasks;
            LengthOfDay = ModifiersSO.LengthOfDay != 0.0f ? ModifiersSO.LengthOfDay : LengthOfDay;
            BasicAnxietyAdder = ModifiersSO.BasicAnxietyAdder != 0.0f ? ModifiersSO.BasicAnxietyAdder : BasicAnxietyAdder;
            TimeLostOnFail = ModifiersSO.TimeLostOnFail != 0 ? ModifiersSO.TimeLostOnFail : TimeLostOnFail;
            TimeLostOnSuccess = ModifiersSO.TimeLostOnSuccess != 0 ? ModifiersSO.TimeLostOnSuccess : TimeLostOnSuccess;
            TaskSuccessAnxietyModificationValue = ModifiersSO.TaskSuccessAnxietyModificationValue != 0 ? ModifiersSO.TaskSuccessAnxietyModificationValue : TaskSuccessAnxietyModificationValue;
            TaskFailAnxietyModificationValue = ModifiersSO.TaskFailAnxietyModificationValue != 0 ? ModifiersSO.TaskFailAnxietyModificationValue : TaskFailAnxietyModificationValue;
            MazeFailAnxietyPunishment = ModifiersSO.MazeFailAnxietyPunishment != 0.0f ? ModifiersSO.MazeFailAnxietyPunishment : MazeFailAnxietyPunishment;
            IsTutorial = !ModifiersSO.IsTutorial ? ModifiersSO.IsTutorial : IsTutorial;
        }
    }

    /// <summary>
    /// Returns the list of prefabs that will be the tasks the player must fill out
    /// </summary>
    public List<GameObject> GetTasks()
    {
        return TasksToCreate;
    }

    /// <summary>
    /// Returns the amount of time in seconds between one task being added to the player's assigned tasks and the next.
    /// </summary>
    public float GetTimeBetweenTasks()
    {
        return TimeBetweenTasks;
    }

    /// <summary>
    /// Returns the amount of time in seconds from the start of a day in the office, until the first new task of the day will be added to the player's assigned tasks.
    /// </summary>
    public float GetTimeBeforeFirstTask()
    {
        return TimeBeforeFirstTask;
    }

    /// <summary>
    /// Returns how long in seconds an office day should last
    /// </summary>
    public float GetLengthOfDay()
    {
        return LengthOfDay;
    }

    /// <summary>
    /// Returns how much anxiety is passively added to the anxiety meter every fixed update.
    /// </summary>
    public float GetPassiveAnxietyModifier()
    {
        return BasicAnxietyAdder;
    }

    /// <summary>
    /// Returns the corresponding anxiety level modification value based on if the player succeeded the maze or not
    /// </summary>
    /// <param name="Succeeded">true iff the player successfully complete the anxiety maze</param>
    /// <returns>Either TaskSuccessAnxietyModificationValue or TaskFailAnxietyModificationValue depending on Succeeded</returns>
    public float GetAnxietyModifierOnTaskCompletion(bool Succeeded)
    {
        return Succeeded ? TaskSuccessAnxietyModificationValue : TaskFailAnxietyModificationValue;
    }

    /// <summary>
    /// returns what the anxiety meter is set to upon returning from the anxiety maze having died from oxygen loss
    /// </summary>
    public float GetAnxietyPunishment()
    {
        return MazeFailAnxietyPunishment;
    }

    /// <summary>
    /// Returns the corresponding time loss value based on if the player succeeded the maze or not.
    /// </summary>
    /// <param name="Succeeded">true iff the player successfully complete the anxiety maze</param>
    /// <returns>Either TimeLostOnFail or TimeLostOnSuccess depending on Succeeded.</returns> 
    public float GetTimeLost(bool Succeeded)
    {
        return Succeeded ? TimeLostOnSuccess : TimeLostOnFail;
    }

    /// <summary>
    /// Returns whether or not the current scene should have the tutorial information.
    /// </summary>
    public bool GetIsTutorial()
    {
        return IsTutorial;
    }
}
