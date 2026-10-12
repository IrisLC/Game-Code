using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "TaskBlock", menuName = "Scriptable Objects/TaskBlock")]
public class TaskBlock : ScriptableObject
{
    public List<GameObject> TaskList;
    public HashSet<TaskPrefab> Tasks = new();

    [Header("Due Dates")]
    public GameManager.Days DayDue;
    [Range(ClockCalc.startHour, ClockCalc.endHour)]
    public int HourDue;
    [Range(0, 59)]
    public int MinuteDue;


    public void Init()
    {
        foreach (GameObject go in TaskList)
        {
            ATask task = go.GetComponentInChildren<ATask>();
            Assert.IsNotNull(task, $"GameObject {go.name} must have an ATask component.");
            TaskPrefab taskPrefab = new TaskPrefab(go, task);
            Tasks.Add(taskPrefab);
        }
    }
}
