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
    protected ComputerScript Computer = Referencer.OfficeReferences.Computer;

    /// <summary>
    /// Opens the tab corresponding to the task's type and pulls up the task
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Computer.tasksWindow.ChangeItem(RelevantTask.TaskUIObject);
    }
}
