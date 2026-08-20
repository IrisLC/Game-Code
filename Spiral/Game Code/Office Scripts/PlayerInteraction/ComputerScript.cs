using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class ComputerScript : MonoBehaviour
{
    /// <summary>
    /// The button to close the computer
    /// </summary>
    public GameObject ExitButton;
    /// <summary>
    /// The GameObject that holds the list of tasks that the player has been assigned.
    /// </summary>
    public GameObject TaskBody;
    /// <summary>
    /// The GameObject that holds the print tasks.
    /// </summary>
    public GameObject PrintBody;
    /// <summary>
    /// The GameObject that holds the email tasks.
    /// </summary>
    public GameObject EmailBody;
    /// <summary>
    /// The GameObject that holds the incident report tasks.
    /// </summary>
    public GameObject IncidentBody;

    /// <summary>
    /// An enum for the different possible tabs on the computer
    /// </summary>
    [Serializable]
    public enum ComputerPage { Tasks, Emails, Printing, Incidents }

    /// <summary>
    /// An event that fires when the print button is pressed, that carries info for the printing paper object
    /// </summary>
    public static event Action<Paper.SortingSpot, Texture> PrintPage;
    /// <summary>
    /// The actively opened page
    /// </summary>
    ComputerPage currPage;

    /// <summary>
    /// Which index in the email TaskTypeList (found in TaskManager) the actively opened email task is at
    /// </summary>
    int EmailIndex;
    /// <summary>
    /// Which index in the incident report TaskTypeList (found in TaskManager) the actively opened incident report task is at
    /// </summary>
    int IRIndex;
    /// <summary>
    /// Which index in the print TaskTypeList (found in TaskManager) the actively opened print task is at
    /// </summary>
    int PrintIndex;

    /// <summary>
    /// Whether or not the computer is on
    /// </summary>
    public bool IsComputerOn { get; private set; }

    /// <summary>
    /// A type of event fired when the computer is turned on or off.
    /// </summary>
    public delegate void ComputerModified();
    /// <summary>
    /// The event fired when the computer is turned on.
    /// </summary>
    public static event ComputerModified ComputerTurnedOn;
    /// <summary>
    /// The event fired when the computer is turned off.
    /// </summary>
    public static event ComputerModified ComputerTurnedOff;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenTab(ComputerPage.Tasks);
    }

    /// <summary>
    /// Method that handles behavior when the players closes the computer
    /// </summary>
    public void ExitComputer()
    {
        IsComputerOn = false;
        Referencer.OfficeReferences.ComputerUI.gameObject.SetActive(false);
        ComputerTurnedOff?.Invoke();
    }

    /// <summary>
    /// Method that handles behavior when the players opens the computer
    /// </summary>
    public void OpenComputer()
    {
        IsComputerOn = true;
        Referencer.OfficeReferences.ComputerUI.gameObject.SetActive(true);
        ComputerTurnedOn?.Invoke();
    }

    /// <summary>
    /// Sets the given tab to active, whilst setting all other tabs to inactive
    /// </summary>
    /// <param name="Tab">The tab type to open</param>
    public void OpenTab(ComputerPage Tab)
    {
        currPage = Tab;
        ClearPages();

        switch (Tab)
        {
            case ComputerPage.Emails:
                EmailBody.SetActive(true);
                UpdateTask(ATask.TaskType.Email, ref EmailIndex);
                break;
            case ComputerPage.Incidents:
                IncidentBody.SetActive(true);
                UpdateTask(ATask.TaskType.IncidentReport, ref IRIndex);
                break;
            case ComputerPage.Printing:
                PrintBody.SetActive(true);
                UpdateTask(ATask.TaskType.Print, ref PrintIndex);
                break;
            case ComputerPage.Tasks:
                TaskBody.SetActive(true);
                break;
            default:
                Debug.LogWarning("Tab type is not implemented");
                break;
        }
    }
    /// <summary>
    /// Opens the task tab
    /// </summary>
    public void OpenTasks() => OpenTab(ComputerPage.Tasks);
    /// <summary>
    /// Opens the email tab
    /// </summary>
    public void OpenEmails() => OpenTab(ComputerPage.Emails);
    /// <summary>
    /// Opens the incident report tab
    /// </summary>
    public void OpenIncidents() => OpenTab(ComputerPage.Incidents);
    /// <summary>
    /// Opens the printing tab
    /// </summary>
    public void OpenPrinting() => OpenTab(ComputerPage.Printing);

    /// <summary>
    /// Sets all tabs to inactive
    /// </summary>
    public void ClearPages()
    {
        TaskBody.SetActive(false);
        PrintBody.SetActive(false);
        EmailBody.SetActive(false);
        IncidentBody.SetActive(false);
    }

    /// <summary>
    /// Goes from one active task to the adjacent active task of the same type, either next or previous.
    /// </summary>
    /// <param name="isIncrementing">true if the player is going to the next element, false if going to previous element</param>
    public void ChangeItem(bool isIncrementing)
    {
        int modifier = isIncrementing ? 1 : -1;

        List<GameObject> TypeList;

        switch (currPage)
        {
            case ComputerPage.Emails:
                EmailIndex += modifier;

                TypeList = TaskManager.GetTaskTypeList(ATask.TaskType.Email);

                if (isIncrementing && EmailIndex <= TypeList.Count)
                {
                    EmailIndex = 0;
                }
                else if (!isIncrementing && EmailIndex < 0)
                {
                    EmailIndex = TypeList.Count - 1;
                }

                UpdateTask(ATask.TaskType.Email, ref EmailIndex);
                break;

            case ComputerPage.Incidents:
                IRIndex += modifier;

                TypeList = TaskManager.GetTaskTypeList(ATask.TaskType.IncidentReport);

                if (isIncrementing && IRIndex <= TypeList.Count)
                {
                    IRIndex = 0;
                }
                else if (!isIncrementing && IRIndex < 0)
                {
                    IRIndex = TypeList.Count - 1;
                }

                UpdateTask(ATask.TaskType.IncidentReport, ref IRIndex);
                break;
            case ComputerPage.Printing:
                PrintIndex += modifier;

                TypeList = TaskManager.GetTaskTypeList(ATask.TaskType.Print);

                if (isIncrementing && PrintIndex <= TypeList.Count)
                {
                    PrintIndex = 0;
                }
                else if (!isIncrementing && PrintIndex < 0)
                {
                    PrintIndex = TypeList.Count - 1;
                }

                UpdateTask(ATask.TaskType.Print, ref PrintIndex);
                break;
            default:
                Debug.LogWarning("ActivateComputer.ChangeItem called on unexpected page");
                break;
        }

    }

    /// <summary>
    /// Sets the task found at the given index of the relevant TypeList to active whilst setting all other tasks in the list to inactive.
    /// </summary>
    /// <param name="taskType">The type of task being updated</param>
    /// <param name="taskIndex">The index of the TypeList for the corresponding task</param>
    void UpdateTask(ATask.TaskType taskType, ref int taskIndex)
    {
        List<GameObject> TypeList = TaskManager.GetTaskTypeList(taskType);
        if (TypeList.Count == 0) return;
        taskIndex = Mathf.Clamp(taskIndex, 0, TypeList.Count - 1);
        for (int i = 0; i < TypeList.Count; i++)
        {
            if (i != taskIndex)
            {
                TypeList[i].SetActive(false);
            }
            else
            {
                TypeList[i].SetActive(true);
            }
        }
    }

    /// <summary>
    /// Sets the task corresponding to the given GameObject to active whilst setting all other tasks in the list to inactive.
    /// </summary>
    /// <param name="Prefab">The gameObject being set to active (should already be in a typeList)</param>
    /// <param name="taskType">The type of task being updated</param>
    public void UpdateTask(GameObject Prefab, ATask.TaskType taskType)
    {
        List<GameObject> TypeList = TaskManager.GetTaskTypeList(taskType);
        if (TypeList.Count == 0) return;

        Assert.IsTrue(TypeList.Contains(Prefab), "Prefab not found in TypeList");

        for (int i = 0; i < TypeList.Count; i++)
        {
            if (Prefab != TypeList[i])
            {
                TypeList[i].SetActive(false);
            }
            else
            {
                TypeList[i].SetActive(true);
            }
        }

        switch (taskType)
        {
            case ATask.TaskType.Email:
                EmailIndex = TypeList.IndexOf(Prefab);
                break;
            case ATask.TaskType.IncidentReport:
                IRIndex = TypeList.IndexOf(Prefab);
                break;
            case ATask.TaskType.Print:
                PrintIndex = TypeList.IndexOf(Prefab);
                break;
        }
    }

    /// <summary>
    /// Invokes the PrintPage event based on a provided PrintableTask object
    /// </summary>
    /// <param name="task">the task from which to print the paper</param>
    public void PrintPaper(PrintableTask task)
    {
        PrintPage?.Invoke(task.spot, task.PaperCover);
    }
}
