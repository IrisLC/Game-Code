using System;
using System.Collections.Generic;
using UnityEngine;

public class DailyModifiers : MonoBehaviour
{
    [Header("Scriptable Object")]
    [Tooltip("Optional. Overrides any manual values with values in provided object. Values not in object will be defaulted to Manual Values")]
    public DayModifierObject ModifiersSO;


    [Header("Task Data")]
    /// <summary>
    /// The list of prefabs that will be the tasks the player must fill out
    /// </summary>
    [SerializeField] internal List<TaskBlock> TasksToCreate;

    /// <summary>
    /// The amount of time in seconds between one task being added to the player's assigned tasks and the next.
    /// </summary>
    [SerializeField] internal float TimeBetweenTasks = 15f;
    /// <summary>
    /// The amount of time in seconds from the start of a day in the office, until the first new task of the day will be added to the player's assigned tasks.
    /// </summary>
    [SerializeField] internal float TimeBeforeFirstTask = 1f;

    [Header("Stress Data")]
    /// <summary>
    /// How much stress is passively added to the stress meter every fixed update.
    /// </summary>
    [SerializeField] internal float BasicStressAdder = .2f;
    /// <summary>
    /// How much value is removed from the stress meter whenever a task is completed successfully (should be negative).
    /// </summary>
    [Range(-100, 0)]
    [SerializeField] internal float TaskSuccessStressModificationValue = -2;
    /// <summary>
    /// How much value is added to the stress meter whenever a task is completed unsuccessfully.
    /// </summary>
    [SerializeField] internal float TaskFailStressModificationValue = 10;
    /// <summary>
    /// What the stress meter is set to upon returning from the stress maze having died from oxygen loss
    /// </summary>
    [SerializeField] internal float MazeFailStressPunishment = 20;

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
            if (ModifiersSO.TasksToCreate != null)
            {
                TasksToCreate = ModifiersSO.TasksToCreate;
            }
            TimeBetweenTasks = ModifiersSO.TimeBetweenTasks != 0.0f ? ModifiersSO.TimeBetweenTasks : TimeBetweenTasks;
            LengthOfDay = ModifiersSO.LengthOfDay != 0.0f ? ModifiersSO.LengthOfDay : LengthOfDay;
            BasicStressAdder = ModifiersSO.BasicStressAdder != 0.0f ? ModifiersSO.BasicStressAdder : BasicStressAdder;
            TimeLostOnFail = ModifiersSO.TimeLostOnFail != 0 ? ModifiersSO.TimeLostOnFail : TimeLostOnFail;
            TimeLostOnSuccess = ModifiersSO.TimeLostOnSuccess != 0 ? ModifiersSO.TimeLostOnSuccess : TimeLostOnSuccess;
            TaskSuccessStressModificationValue = ModifiersSO.TaskSuccessStressModificationValue != 0 ? ModifiersSO.TaskSuccessStressModificationValue : TaskSuccessStressModificationValue;
            TaskFailStressModificationValue = ModifiersSO.TaskFailStressModificationValue != 0 ? ModifiersSO.TaskFailStressModificationValue : TaskFailStressModificationValue;
            MazeFailStressPunishment = ModifiersSO.MazeFailStressPunishment != 0.0f ? ModifiersSO.MazeFailStressPunishment : MazeFailStressPunishment;
            IsTutorial = !ModifiersSO.IsTutorial ? ModifiersSO.IsTutorial : IsTutorial;
        }
    }

    public void setModifiers()
    {
        Awake();
        Debug.Log("Updating Daily Modifier");
    }

    /// <summary>
    /// Returns the list of prefabs that will be the tasks the player must fill out
    /// </summary>
    public List<TaskBlock> GetTasks()
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
    /// Returns how much stress is passively added to the stress meter every fixed update.
    /// </summary>
    public float GetPassiveStressModifier()
    {
        return BasicStressAdder;
    }

    /// <summary>
    /// Returns the corresponding stress level modification value based on if the player succeeded the maze or not
    /// </summary>
    /// <param name="Succeeded">true iff the player successfully complete the anxiety maze</param>
    /// <returns>Either TaskSuccessStressModificationValue or TaskFailStressModificationValue depending on Succeeded</returns>
    public float GetStressModifierOnTaskCompletion(bool Succeeded)
    {
        return Succeeded ? TaskSuccessStressModificationValue : TaskFailStressModificationValue;
    }

    /// <summary>
    /// returns what the stress meter is set to upon returning from the anxiety maze having died from oxygen loss
    /// </summary>
    public float GetStressPunishment()
    {
        return MazeFailStressPunishment;
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
