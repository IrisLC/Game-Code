using UnityEngine;
using UnityEngine.Assertions;

public abstract class ATask : MonoBehaviour
{

    /// <summary>
    /// The delegate for when a Task is finished, be it right or wrong
    /// </summary>
    /// <param name="Correct">True iff the task was completed correctly.</param>
    /// <param name="Weight">The importance of the task, 
    ///     allows for not all tasks to be treated the same value wise.</param>
    public delegate void TaskFinished(ATask Task, bool Correct, int Weight);
    /// <summary>
    /// The event called when a task is finished
    /// </summary>
    public static event TaskFinished OnTaskFinish;
    /// <summary>
    /// The name of the task
    /// </summary>
    public string TaskName;

    /// <summary>
    /// The possible kinds of tasks
    /// </summary>
    public enum TaskType { Null, Email, IncidentReport, Print }

    /// <summary>
    /// The type of task that this task is
    /// </summary>
    public virtual TaskType Type { get => TaskType.Null; }

    /// <summary>
    /// Method called when the task is done and ready for evaluation
    /// </summary>
    public virtual void Submit()
    {
        EvaluateTask();
    }

    /// <summary>
    /// To be called by Submit
    /// Checks to see if the task was done succesfully.
    /// </summary>
    /// <returns>true iff the task was successful</returns>
    protected abstract bool EvaluateTask();
    /// <summary>
    /// Dev command that automatically successfully completes a task. Should not be called in normal scripts
    /// </summary>
    internal void DevCommandSubmitSuccess() => FireEvent(this, true, 1);

    /// <summary>
    /// Invokes the event for when a Task is finished, be it right or wrong
    /// </summary>
    /// <param name="Correct">True iff the task was completed correctly.</param>
    /// <param name="Weight">The importance of the task, 
    ///     allows for not all tasks to be treated the same value wise.</param>
    protected static void FireEvent(ATask task, bool Correct, int Weight) => OnTaskFinish.Invoke(task, Correct, Weight);
}
