using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A button on the home page that shows the tasks the player has left to do, clicking on it will open the task
/// </summary>
public class TaskListButton : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// The task this button corresponds to
    /// </summary>
    public TaskPrefab RelevantTask;
    /// <summary>
    /// The computer script
    /// </summary>
    ComputerScript Computer = Referencer.OfficeReferences.Computer;

    /// <summary>
    /// Opens the tab corresponding to the task's type and pulls up the task
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (RelevantTask.Task.Type)
        {
            case ATask.TaskType.Email:
                Computer.OpenTab(ComputerScript.ComputerPage.Emails);
                break;

            case ATask.TaskType.IncidentReport:
                Computer.OpenTab(ComputerScript.ComputerPage.Incidents);
                break;

            case ATask.TaskType.Print:
                Computer.OpenTab(ComputerScript.ComputerPage.Printing);
                break;
        }

        Computer.UpdateTask(RelevantTask.TaskUIObject, RelevantTask.Task.Type);
    }
}
