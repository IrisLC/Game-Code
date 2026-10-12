using System;
using System.Collections;
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
    internal List<TaskBlock> UnassignedTasks = new();
    internal List<TaskBlock> AssignedBlocks = new();

    HashSet<string> CompletedTasks = new();

    List<string> PrintTasks = new();
    /// <summary>
    /// The buttons on the TODO list that will take the player to the task
    /// </summary>
    List<GameObject> Buttons = new List<GameObject>();
    /// <summary>
    /// The list holding all currently active email tasks
    /// </summary>
    static List<GameObject> Emails = new List<GameObject>();
    /// <summary>
    /// The list holding all currently active incident report tasks
    /// </summary>
    static List<GameObject> IRs = new List<GameObject>();
    static List<GameObject> Weathers = new List<GameObject>();
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
    public int TotalTasksForDay { get => GetUnassignedTasksNum() + ActiveTasks.Count + PrintTasks.Count + finishedTasks; }
    /// <summary>
    /// The oldest added task, for use with the Dev. Tools
    /// </summary>
    internal TaskPrefab OldestTask { get => ActiveTasks.Count == 0 ? null : ActiveTasks[0]; }

    /// <summary>
    /// The time at which the next task will be assigned
    /// </summary>
    public float TimeForNextTask;

    public CorrectionEmail correctionEmail = null;

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

        Emails.Clear();
        IRs.Clear();
        Weathers.Clear();
    }

    /// <summary>
    /// The methods needing to be done at the start of an office day
    /// </summary>
    void DaySetup()
    {
        finishedTasks = 0;
        CreateTasks(OfficeRefs.dailyModifiers.GetTasks());

        //For if there are unfinished tasks from previous days
        if (ActiveTasks.Count != 0)
        {
            ClearLists();
            RecreateTasks();
        }

        TimeForNextTask = OfficeManager.TimeLeftInDay.InitialTime - OfficeRefs.dailyModifiers.GetTimeBeforeFirstTask();

    }

    /// <summary>
    /// Takes a List of task GameObjects and creates TaskPrefabs around them, adding said TaskPrefabs to UnassignedTasks.
    /// </summary>
    /// <param name="Tasks">The list of Prefabs to create tasks from.</param>
    internal void CreateTasks(List<TaskBlock> Tasks)
    {
        foreach (TaskBlock block in Tasks)
        {
            block.Init();

            if (block.Tasks.Count != 0)
            {
                UnassignedTasks.Add(block);
            }
        }

    }

    /// <summary>
    /// Makes a new instance of each task in ActiveTasks, used for tasks the player has seen but 
    /// hadn't completed yet before leaving the office.
    /// </summary>
    public void RecreateTasks()
    {
        if (AssignedBlocks.Count == 0) return;
        int count = AssignedBlocks.Count;

        // Loop around ActiveTasks, recreating each one, until we've completed a single loop
        for (int i = 0; i < count; ++i)
        {
            AddTaskByBlock(null, AssignedBlocks[0]);

            // Task will be put back at the end
            AssignedBlocks.Remove(AssignedBlocks[0]);
        }
    }

    /// <summary>
    /// Takes a given task or the first task in UnassignedTasks, and makes it into a completable task by the player.
    /// 
    /// Moves the task from UnassignedTasks, to ActiveTasks.
    /// </summary>
    /// <param name="task">A TaskPrefab to make the task around, if none provided will use the first value in UnassignedTasks</param>
    internal void AddTaskByBlock(TaskPrefab task = null, TaskBlock block = null)
    {
        if (task == null && block == null)
        {
            // If no tasks in UnassignedTasks then don't move forward
            if (UnassignedTasks.Count <= 0) return;

            // Get the first unassigned task
            block = UnassignedTasks[0];
            UnassignedTasks.Remove(block);
            AssignedBlocks.Add(block);
        }
        else if (task != null)
        {
            block = FindBlock(task);
        }

        foreach (TaskPrefab taskInBlock in block.Tasks)
        {
            AddTask(taskInBlock);
        }

        GameObject toDoBLock = MonoBehaviour.Instantiate(Resources.Load("toDoBlock") as GameObject, OfficeRefs.TodoList.transform);
        toDoBLock.GetComponent<TaskBlockProcessor>().Init(block);
        foreach (string name in CompletedTasks)
        {
            toDoBLock.GetComponent<TaskBlockProcessor>().CrossOffTask(name);
        }
    }
    internal void AddTask(TaskPrefab task)
    {

        if (task.TaskUIObject.IsDestroyed())
        {
            task.InstantiateTaskUIObject();
        }

        if (CompletedTasks.Contains(task.Task.TaskName)) return;

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
        CreateButton(task, OfficeRefs.ButtonPrefab, GetButtonHolder(task.Task.Type));

        if (task.Task.Type == ATask.TaskType.IncidentReport)
        {
            PrintTasks.Add(task.Task.TaskName);
        }

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
        //Buttons.Add(button);

        // Get the TaskListButton component on the button object and assign the task to it
        TaskListButton buttonScript = button.GetComponent<TaskListButton>();
        Assert.IsNotNull(buttonScript, "buttonPrefab must have a TaskListButton component");
        buttonScript.RelevantTask = task;

        // Add the name of the task to the text of the button
        button.GetComponentInChildren<TextMeshProUGUI>().text = task.Task.TaskName;
        task.TaskUIObject.GetComponent<ATask>().SetButton(button);
    }

    /// <summary>
    /// Removes a completed task from the game.
    /// </summary>
    /// <param name="task">The task that was completed.</param>
    /// <param name="Correct">Whether or not the task was completed correctly.</param>
    /// <param name="Weight">The weight of the task, determines how much success or failiure impacts StressLevels</param>
    public void CompletedTask(ATask task, bool Correct, int Weight)
    {
        // Play the Incorrect animation if the player failed the task
        if (!Correct)
        {
            OfficeRefs.ScreenFlash.SetTrigger("ScreenFlash");
        }

        if (task.Type == ATask.TaskType.Print)
        {
            PrintTasks.Remove(task.name);
            ++finishedTasks;
            return;
        }

        // Removes the TaskPrefab from ActiveTasks
        TaskPrefab prefab = TaskPrefab.GetTaskInList(ActiveTasks, task);
        ActiveTasks.Remove(prefab);

        CompletedTasks.Add(task.TaskName);

        // Remove the button from the List holding it, and destroy the gameObject
        GetTaskTypeList(prefab.Task.Type).Remove(prefab.TaskUIObject);
        if (correctionEmail != null)
        {
            OfficeManager.GetOfficeManager().StartCoroutine(DelayDestroy(prefab));
        }
        else
        {
            UnityEngine.Object.Destroy(prefab.TaskUIObject);
        }

        ++finishedTasks;
    }

    /// <summary>
    /// Delays the destruction of a taskprefab by one frame to ensure a CorrectionEmail is still able to access its data
    /// </summary>
    IEnumerator DelayDestroy(TaskPrefab prefab)
    {

        yield return null;
        UnityEngine.Object.Destroy(prefab.TaskUIObject);
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
            AddTaskByBlock();

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
        Buttons.Clear();
        PrintTasks.Clear();
        Weathers.Clear();
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
            case ATask.TaskType.Weather:
                return Weathers;
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

            case ATask.TaskType.Weather:
                return OfficeRefs.WeatherHolder.transform;

            default:
                return null;
        }
    }

    public static GameObject GetButtonHolder(ATask.TaskType type)
    {
        switch (type)
        {
            case ATask.TaskType.Email:
                return OfficeRefs.OutgoingEmailButtonHolder;

            case ATask.TaskType.IncidentReport:
                return OfficeRefs.IncidentReportButtonHolder;

            case ATask.TaskType.Weather:
                return OfficeRefs.WeatherButtonHolder;

            default:
                return null;
        }
    }

    int GetUnassignedTasksNum()
    {
        int count = 0;
        foreach (TaskBlock block in UnassignedTasks)
        {
            count += block.Tasks.Count;
        }
        return count;
    }

    TaskBlock FindBlock(TaskPrefab taskPrefab)
    {
        foreach (TaskBlock block in UnassignedTasks)
        {
            if (block.Tasks.Contains(taskPrefab))
            {
                return block;
            }
        }

        foreach (TaskBlock block in AssignedBlocks)
        {
            if (block.Tasks.Contains(taskPrefab))
            {
                return block;
            }
        }

        return null;
    }

}
