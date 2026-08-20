using UnityEngine;
using UnityEngine.UI;

public class TaskProgressBar : MonoBehaviour
{
    /// <summary>
    /// The bar image
    /// </summary>
    public Image taskBar;
    /// <summary>
    /// The number of completed tasks
    /// </summary>
    int FinishedTasks;

    /// <summary>
    /// Subscribes so GetCurrentFill will be called wheenver a task is completed
    /// </summary>
    void Awake()
    {
        ATask.OnTaskFinish += GetCurrentFill;
    }

    void OnDestroy()
    {
        ATask.OnTaskFinish -= GetCurrentFill;
    }

    /// <summary>
    /// Fills the progress bar based on the percentage of total tasks that have been completed
    /// </summary>
    // Parameters are just so event subscription works
    void GetCurrentFill(ATask Task, bool Correct, int Weight)
    {
        ++FinishedTasks;

        float fillAmount = (float)FinishedTasks
            / Referencer.OfficeReferences.officeManager.TaskManager.TotalTasksForDay;

        taskBar.fillAmount = fillAmount;
    }
}
