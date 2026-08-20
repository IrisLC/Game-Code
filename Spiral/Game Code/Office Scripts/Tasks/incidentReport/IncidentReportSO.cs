using UnityEngine;

[CreateAssetMenu(fileName = "IncidentReportSO", menuName = "Scriptable Objects/IncidentReportSO")]
public class IncidentReportSO : TaskSO
{
    /// <summary>
    /// Assigns values to all stored variables.
    /// 
    /// Sets description to empty
    /// </summary>
    /// <param name="rawText">the body text of the report</param>
    /// <param name="name">the name of the task</param>
    public void Create(string rawText, string name)
    {
        taskType = ATask.TaskType.IncidentReport;

        RawText = rawText;
        Name = name;
        Description = "";
    }
}
