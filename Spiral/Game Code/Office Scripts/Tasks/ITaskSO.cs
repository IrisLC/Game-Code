using UnityEngine;

/// <summary>
/// Parent class for Task Scriptable Objects
/// </summary>
public class TaskSO : ScriptableObject
{
    /// <summary>
    /// The type of the task
    /// </summary>
    public ATask.TaskType taskType;
    /// <summary>
    /// The body text for the task
    /// </summary>
    public string RawText;
    /// <summary>
    /// The name of the task
    /// </summary>
    public string Name;
    /// <summary>
    /// The description of the task, 
    /// note: not all tasks have descriptions, at time of writing (7/28/2026) only emails need descriptions
    /// </summary>
    public string Description;
}
