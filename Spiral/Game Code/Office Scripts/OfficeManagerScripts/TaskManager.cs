using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

using OfficeRefs = Referencer.OfficeReferences;

[Serializable]
public class TaskManager
{
    [Header("Tasks")]
    /// <summary>
    /// The tasks that the player can see
    /// </summary>
    public List<TaskPrefab> ActiveTasks { get; private set; } = new List<TaskPrefab>();
    /// <summary>
    /// The tasks that have yet to be given to the player that day
    /// </summary>
    internal List<TaskPrefab> UnassignedTasks = new List<TaskPrefab>();
    /// <summary>
    /// The buttons on the TODO list that will take the player to the task
    /// </summary>
    List<GameObject> Buttons = new List<GameObject>();
    /// <summary>
    /// The list holding all currently active email tasks
    /// </summary>
    static List<GameObject> Emails = new List<GameObject>();
    /// <summary>
    /// The list holding all currently active print tasks
    /// </summary>
    static List<GameObject> Prints = new List<GameObject>();
    /// <summary>
    /// The list holding all currently active incident report tasks
    /// </summary>
    static List<GameObject> IRs = new List<GameObject>();
    /// <summary>
    /// The number of activeTasks the player has left
    /// </summary>
    public int RemainingTasks { get => ActiveTasks.Count; }
    /// <summary>
    /// The number of tasks the player has finished today
    /// </summary>
    public int finishedTasks;
    /// <summary>
    /// The total number of tasks the player needs to finish today
    /// </summary>
    public int TotalTasksForDay;
    /// <summary>
    /// The oldest added task, for use with the Dev. Tools
    /// </summary>
    internal TaskPrefab OldestTask { get => ActiveTasks.Count == 0 ? null : ActiveTasks[0]; }

    /// <summary>
    /// The time at which the next task will be assigned
    /// </summary>
    public float TimeForNextTask;

    // Subscribe to events
    public TaskManager()
    {
        // Create the tasks
        OfficeManager.OnDayStart += DaySetup;
        // The lists will be repopulated when we resume the day so we clear the lists to prevent duplicates
        OfficeManager.OnDayPaused += ClearLists;
        // Recreates tasks that had been active before entering the Anxiety Scene
        OfficeManager.OnDayResume += RecreateTasks;
    }

    // Destructor, Unsubscribe from events
    ~TaskManager()
    {
        OfficeManager.OnDayStart -= DaySetup;
        OfficeManager.OnDayPaused -= ClearLists;
        OfficeManager.OnDayResume -= RecreateTasks;
    }

    /// <summary>
    /// The methods needing to be done at the start of an office day
    /// </summary>
    void DaySetup()
    {
        CreateTasks(OfficeRefs.dailyModifiers.GetTasks());

        //For if there are unfinished tasks from previous days
        if (ActiveTasks.Count != 0)
        {
            RecreateTasks();
        }

        TimeForNextTask = OfficeManager.TimeLeftInDay.InitialTime - OfficeRefs.dailyModifiers.GetTimeBeforeFirstTask();

    }

    /// <summary>
    /// Takes a List of task GameObjects and creates TaskPrefabs around them, adding said TaskPrefabs to UnassignedTasks.
    /// </summary>
    /// <param name="Tasks">The list of Prefabs to create tasks from.</param>
    internal void CreateTasks(List<GameObject> Tasks)
    {
        foreach (GameObject Prefab in Tasks)
        {
            ATask task = Prefab.GetComponentInChildren<ATask>();
            Assert.IsNotNull(task, $"GameObject {Prefab.name} must have an ATask component.");

            UnassignedTasks.Add(new TaskPrefab(Prefab, task));
        }

        TotalTasksForDay = UnassignedTasks.Count + ActiveTasks.Count;
    }

    /// <summary>
    /// Makes a new instance of each task in ActiveTasks, used for tasks the player has seen but 
    /// hadn't completed yet before leaving the office.
    /// </summary>
    public void RecreateTasks()
    {
        if (ActiveTasks.Count == 0) return;

        // Loop around ActiveTasks, recreating each one, until we've completed a single loop
        for (int i = 0; i < ActiveTasks.Count; ++i)
        {
            AddTask(ActiveTasks[0]);
            // Task will be put back at the end
            ActiveTasks.Remove(ActiveTasks[0]);
        }
    }

    /// <summary>
    /// Takes a given task or the first task in UnassignedTasks, and makes it into a completable task by the player.
    /// 
    /// Moves the task from UnassignedTasks, to ActiveTasks.
    /// </summary>
    /// <param name="task">A TaskPrefab to make the task around, if none provided will use the first value in UnassignedTasks</param>
    internal void AddTask(TaskPrefab task = null)
    {
        if (task == null)
        {
            // If no tasks in UnassignedTasks then don't move forward
            if (UnassignedTasks.Count <= 0) return;

            // Get the first unassigned task
            task = UnassignedTasks[0];
            UnassignedTasks.Remove(task);
        }

        if (task.TaskUIObject.IsDestroyed())
        {
            task.InstantiateTaskUIObject();
        }

        ActiveTasks.Add(task);

        // Gets the transform of the gameObject the task will be a child of, and the List that will hold a reference to the task
        Transform transform = GetTaskHolder(task.Task.Type);
        List<GameObject> TypeList = GetTaskTypeList(task.Task.Type);

        Assert.IsNotNull(transform, "Null Transform, Invalid Task Type found");
        Assert.IsNotNull(TypeList, "Null List, Invalid Task Type found");

        //Assign the task to the right parent, and add it to the relevant List
        task.TaskUIObject.transform.SetParent(transform, false);
        TypeList.Add(task.TaskUIObject);


        //Create the Button in the todoList
        CreateButton(task, OfficeRefs.ButtonPrefab, OfficeRefs.TodoList);

        task.TaskUIObject.SetActive(false);
    }

    /// <summary>
    /// Creates the button on the task screen that the player can click on to go to the given task
    /// </summary>
    /// <param name="task">the TaskPrefab the button will point to</param>
    /// <param name="buttonPrefab">the template around which to build the button</param>
    /// <param name="buttonHolder">the GameObject the button will be a child of</param>
    void CreateButton(TaskPrefab task, GameObject buttonPrefab, GameObject buttonHolder)
    {
        if (buttonPrefab == null) return;
        // Create the button as a child of the buttonHolder and add it to the button list
        GameObject button = UnityEngine.Object.Instantiate(buttonPrefab, buttonHolder.transform, false);
        Buttons.Add(button);

        // Get the TaskListButton component on the button object and assign the task to it
        TaskListButton buttonScript = button.GetComponent<TaskListButton>();
        Assert.IsNotNull(buttonScript, "buttonPrefab must have a TaskListButton component");
        buttonScript.RelevantTask = task;

        // Add the name of the task to the text of the button
        button.GetComponentInChildren<TextMeshProUGUI>().text = task.Task.TaskName;
    }

    /// <summary>
    /// Removes a completed task from the game.
    /// </summary>
    /// <param name="task">The task that was completed.</param>
    /// <param name="Correct">Whether or not the task was completed correctly.</param>
    /// <param name="Weight">The weight of the task, determines how much success or failiure impacts AnxietyLevels</param>
    public void CompletedTask(ATask task, bool Correct, int Weight)
    {
        // Play the Incorrect animation if the player failed the task
        if (!Correct)
        {
            OfficeRefs.Computer.ExitComputer();
            OfficeRefs.ScreenFlash.SetTrigger("ScreenFlash");
        }

        // Removes the TaskPrefab from ActiveTasks
        TaskPrefab prefab = TaskPrefab.GetTaskInList(ActiveTasks, task);
        ActiveTasks.Remove(prefab);

        // Destroy the Button in the to-do list
        foreach (GameObject Button in Buttons)
        {
            if (Button.GetComponent<TaskListButton>().RelevantTask.Task.TaskName.Equals(prefab.Task.TaskName))
            {
                UnityEngine.Object.Destroy(Button);
                Buttons.Remove(Button);
                break;
            }
        }

        // Remove the button from the List holding it, and destroy the gameObject
        GetTaskTypeList(prefab.Task.Type).Remove(prefab.TaskUIObject);
        UnityEngine.Object.Destroy(prefab.TaskUIObject);

        ++finishedTasks;
    }

    //// <summary>
    /// Checks to see 
    /// 1. if all tasks have been created in which case it will end the day early
    /// 2. It is time for the next class to be added
    /// </summary>
    /// <returns>true iff every task has been completed</returns>
    public bool CheckTasks(CountdownTimer timeLeftInDay, float timeBetweenTasks)
    {
        // End the day if every task has been completed
        if (UnassignedTasks.Count == 0 && RemainingTasks == 0)
        {
            return true;
        }

        // Add the next task if it is the right time.
        if (timeLeftInDay.Time < TimeForNextTask)
        {
            AddTask();

            TimeForNextTask = timeLeftInDay.Time - timeBetweenTasks;
        }

        return false;
    }

    /// <summary>
    /// Clears all the GameObject lists that are part of this class
    /// </summary>
    public void ClearLists()
    {
        Emails.Clear();
        IRs.Clear();
        Prints.Clear();
        Buttons.Clear();
    }

    /// <summary>
    /// Gives a task List based on the provided TaskType
    /// </summary>
    /// <param name="type">the TaskType whose list you want</param>
    /// <returns>The list corresponded to the provided type</returns>
    public static List<GameObject> GetTaskTypeList(ATask.TaskType type)
    {
        switch (type)
        {
            case ATask.TaskType.Email:
                return Emails;

            case ATask.TaskType.IncidentReport:
                return IRs;

            case ATask.TaskType.Print:
                return Prints;

            default:
                Debug.LogError("Unexpected Type Given, Null Type List Returned");
                return null;
        }
    }

    public static Transform GetTaskHolder(ATask.TaskType type)
    {
        switch (type)
        {
            case ATask.TaskType.Email:
                return OfficeRefs.EmailHolder.transform;

            case ATask.TaskType.IncidentReport:
                return OfficeRefs.IRHolder.transform;

            case ATask.TaskType.Print:
                return OfficeRefs.PrinterHolder.transform;

            default:
                return null;
        }
    }
}
