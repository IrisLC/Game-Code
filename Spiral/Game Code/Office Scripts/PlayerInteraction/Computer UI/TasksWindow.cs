using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TasksWindow : ComputerWindow
{
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

    public GameObject WeatherBody;

    public GameObject PTOBody;
    /// <summary>
    /// An enum for the different possible tabs on the computer
    /// </summary>
    [Serializable]
    public enum ComputerPage { Tasks = 0, Emails = 1, Printing = 2, Incidents = 3, Weather = 4, PTO = 5 }

    GameObject ActiveEmail;
    bool isActiveEmailTask;
    GameObject ActiveIR;
    GameObject ActiveWeather;

    List<GameObject> ReceivedEmails = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        TabBodies.Add(TaskBody);
        TabBodies.Add(PrintBody);
        TabBodies.Add(EmailBody);
        TabBodies.Add(IncidentBody);
        TabBodies.Add(WeatherBody);
        //TabBodies.Add(PTOBody);

        ParentAwake();
    }

    void Start()
    {
        Referencer.OfficeReferences.Computer.tasksWindow = this;

        foreach (IncomingEmailData data in OfficeManager.ReceivedEmails)
        {
            GameObject email = IncomingEmailSender.CreateIncomingEmailObject(data);
            IncomingEmailSender.CreateIncomingEmailButton(email, data);
            ReceivedEmails.Add(email);
            email.SetActive(false);
        }
    }

    void OnEnable()
    {
        IncomingEmailSender.EmailSent += OnReceiveEmail;
    }

    void OnDisable()
    {
        IncomingEmailSender.EmailSent -= OnReceiveEmail;
    }

    void OnReceiveEmail(IncomingEmailData data, GameObject email)
    {
        ReceivedEmails.Add(email);
        email.SetActive(false);
    }

    public override void OpenTab(int tabIndex)
    {
        OpenTab((ComputerPage)tabIndex);
    }

    /// <summary>
    /// Sets the given tab to active, whilst setting all other tabs to inactive
    /// </summary>
    /// <param name="Tab">The tab type to open</param>
    public void OpenTab(ComputerPage Tab)
    {
        Debug.Log("Opening window");
        if (TabBodies.Count == 0) return;
        currentTab = (int)Tab;
        ClearPages();

        switch (Tab)
        {
            case ComputerPage.Emails:
                EmailBody.SetActive(true);
                UpdateTask(ActiveEmail, ATask.TaskType.Email);
                break;
            case ComputerPage.Incidents:
                IncidentBody.SetActive(true);
                UpdateTask(ActiveIR, ATask.TaskType.IncidentReport);
                break;
            case ComputerPage.Printing:
                PrintBody.SetActive(true);
                break;
            case ComputerPage.Tasks:
                TaskBody.SetActive(true);
                break;
            case ComputerPage.Weather:
                Debug.Log("Weather");
                WeatherBody.SetActive(true);
                UpdateTask(ActiveWeather, ATask.TaskType.Weather);
                break;
            case ComputerPage.PTO:
                PTOBody.SetActive(true);
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

    public void OpenWeather() => OpenTab(ComputerPage.Weather);

    public void OpenPTO() => OpenTab(ComputerPage.PTO);

    /// <summary>
    /// Goes from one active task to the adjacent active task of the same type, either next or previous.
    /// </summary>
    /// <param name="isIncrementing">true if the player is going to the next element, false if going to previous element</param>
    public void ChangeItem(GameObject gameObject, bool incomingEmail = false)
    {
        ComputerPage page = (ComputerPage)currentTab;

        if (page == ComputerPage.Emails && incomingEmail)
        {
            foreach (GameObject email in TaskManager.GetTaskTypeList(ATask.TaskType.Email))
            {
                email.SetActive(false);
            }
            foreach (GameObject email in ReceivedEmails)
            {
                email.SetActive(false);
            }

            gameObject.SetActive(true);
            ActiveEmail = gameObject;
            isActiveEmailTask = false;
        }
        else
        {
            List<GameObject> TypeList = TaskManager.GetTaskTypeList((ATask.TaskType)page);
            // Grabs the proper index to modify
            int index = TypeList.IndexOf(gameObject);

            UpdateTask((ATask.TaskType)page, ref index);
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
        GameObject active = null;
        for (int i = 0; i < TypeList.Count; i++)
        {
            if (i != taskIndex)
            {
                TypeList[i].SetActive(false);
            }
            else
            {
                TypeList[i].SetActive(true);
                active = TypeList[i];
            }
        }

        switch (taskType)
        {
            case ATask.TaskType.Email:
                ActiveEmail = active;
                isActiveEmailTask = true;
                foreach (GameObject email in ReceivedEmails)
                {
                    email.SetActive(false);
                }
                break;
            case ATask.TaskType.IncidentReport:
                ActiveIR = active;
                break;
            case ATask.TaskType.Weather:
                ActiveWeather = active;
                break;
        }
    }

    /// <summary>
    /// Sets the task corresponding to the given GameObject to active whilst setting all other tasks in the list to inactive.
    /// </summary>
    /// <param name="Prefab">The gameObject being set to active (should already be in a typeList)</param>
    /// <param name="taskType">The type of task being updated</param>
    public void UpdateTask(GameObject Prefab, ATask.TaskType taskType)
    {
        if (taskType == ATask.TaskType.Email && !isActiveEmailTask)
        {
            foreach (GameObject email in TaskManager.GetTaskTypeList(ATask.TaskType.Email))
            {
                email.SetActive(false);
            }
            foreach (GameObject email in ReceivedEmails)
            {
                email.SetActive(false);
            }

            Prefab?.SetActive(true);

            return;
        }

        List<GameObject> TypeList = TaskManager.GetTaskTypeList(taskType);
        if (TypeList.Count == 0) return;

        Assert.IsTrue(TypeList.Contains(Prefab) || Prefab == null, "Prefab not found in TypeList");

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
                ActiveEmail = Prefab != null ? Prefab : TypeList[0];
                isActiveEmailTask = true;
                break;
            case ATask.TaskType.IncidentReport:
                ActiveIR = Prefab != null ? Prefab : TypeList[0];
                break;
            case ATask.TaskType.Print:
                // PrintIndex = TypeList.IndexOf(Prefab);
                break;
            case ATask.TaskType.Weather:
                ActiveWeather = Prefab != null ? Prefab : TypeList[0];
                break;
        }
    }

    /// <summary>
    /// Since weather tasks' submit buttons aren't in the prefab, 
    /// this script is called by the submit buttons on the task window 
    /// and submits the currently active task
    /// </summary>
    /// <param name="AsPredicted">true iff the player said the weather was as predicted</param>
    public void SubmitWeather(bool AsPredicted)
    {
        if (TaskManager.GetTaskTypeList(ATask.TaskType.Weather).Count == 0) return;
        WeatherTask task = ActiveWeather.GetComponent<WeatherTask>();
        task.playerInput = AsPredicted;
        task.Submit();
    }
}
