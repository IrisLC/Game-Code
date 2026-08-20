using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object Consisting of 2 components, TaskUIObject and Task
/// TaskUIObject is a gameObject holding the UI elements the player interacts with to do the task (Must be instantaited)
/// Task is an ATask inherited script that handles the logic of the task and the evaluation of the player's actions
/// </summary>
[System.Serializable]
public class TaskPrefab
{
    /// <summary>
    /// a gameObject holding the UI elements the player interacts with to do the task (Must be instantaited)
    /// </summary>
    public GameObject TaskUIObject;
    /// <summary>
    /// A reference to the original prefab the taskUIObject is instantiated from
    /// </summary>
    GameObject UIPrefabReference;
    /// <summary>
    /// an ATask inherited script that handles the logic of the task and the evaluation of the player's actions
    /// </summary>
    public ATask Task;

#if UNITY_EDITOR
    /// <summary>
    /// If Debug.Log commands should be called, modified with DebugCommands
    /// </summary>
    internal static bool isDebugging;
#endif

    /// <summary>
    /// Constructor for TaskPrefab, instantiates a new TaskUIObject
    /// </summary>
    /// <param name="Prefab">the UI elements the player interacts with</param>
    /// <param name="task">the ATask inherited script</param>
    public TaskPrefab(GameObject Prefab, ATask task)
    {
        UIPrefabReference = Prefab;
        Task = task;

        InstantiateTaskUIObject();
    }

    /// <summary>
    /// returns an ATask inherited script that handles the logic of the task and the evaluation of the player's actions
    /// </summary>
    public GameObject GetPrefabReference()
    {
        return UIPrefabReference;
    }

    /// <summary>
    /// Instantiates a gameObject based on the UIPrefabReference, 
    ///  assigns that object to the TaskUIObject and then sets the object to inactive
    /// </summary>
    public void InstantiateTaskUIObject()
    {
        TaskUIObject = MonoBehaviour.Instantiate(UIPrefabReference);
        TaskUIObject.SetActive(false);
    }

    /// <summary>
    /// Given a List of TaskPrefabs and an ATask inhereted script, returns the TaskPrefab holding the given script
    /// </summary>
    /// <param name="list">The list to be search</param>
    /// <param name="targetTask">The ATask inherited script being searched for.</param>
    /// <returns>The TaskPrefab found in the list, null if not found</returns>
    public static TaskPrefab GetTaskInList(List<TaskPrefab> list, ATask targetTask)
    {
        foreach (TaskPrefab task in list)
        {
            if (task.Task.TaskName.Equals(targetTask.TaskName)) return task;
        }
#if UNITY_EDITOR
        if (isDebugging) Debug.Log("None Found");
#endif
        return null;
    }

    /// <summary>
    /// Given a List of TaskPrefabs and gameObject, returns the TaskPrefab holding the given Object
    /// </summary>
    /// <param name="list">The list to be search</param>
    /// <param name="targetPrefab">The gameObject being searched for.</param>
    /// <returns>The TaskPrefab found in the list, null if not found</returns>
    public static TaskPrefab GetTaskInList(List<TaskPrefab> list, GameObject targetPrefab)
    {
        foreach (TaskPrefab tasks in list)
        {
            if (tasks.TaskUIObject == targetPrefab) return tasks;
        }
#if UNITY_EDITOR
        if (isDebugging) Debug.Log("None Found");
#endif
        return null;
    }
}


