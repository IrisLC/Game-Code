using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Editor script for creating TaskSO ScriptableObjects from entered data. Part 1 of automated task creation (part 2 is TaskPrefabCreator)
/// </summary>
public class TaskCreator : EditorWindow
{
    [MenuItem("Tools/Task Creator")]
    public static void GetWindow()
    {
        GetWindow<TaskCreator>();
    }

    /// <summary>
    /// The last selected task to create, allows for values to be reset if changing the type
    /// </summary>
    ATask.TaskType LastSelectedTask = ATask.TaskType.Null;
    /// <summary>
    /// The currently selected task type to create
    /// </summary>
    ATask.TaskType taskType = ATask.TaskType.Null;
    /// <summary>
    /// Default value for the name of the task
    /// </summary>
    const string DefaultTaskNameString = "Enter a Unique Name for Task";
    /// <summary>
    /// Name of the task
    /// </summary>
    string TaskName = DefaultTaskNameString;
    /// <summary>
    /// Default value for the Text body that will hold the task's main language
    /// </summary>
    const string DefaultTextInputString = "Enter Formatted Task Text Here";
    /// <summary>
    /// Text body that will hold the task's main language
    /// </summary>
    string InputtedText = DefaultTextInputString;
    /// <summary>
    /// Previously submitted InputtedText, used to check for changes to InputtedText
    /// </summary>
    string lastInputtedText = DefaultTextInputString;
    /// <summary>
    /// Default value for DescriptionText
    /// </summary>
    const string DefaultDescriptionString = "Enter a description of the task";
    /// <summary>
    /// The task's description
    /// </summary>
    string DescriptionText = DefaultDescriptionString;
    /// <summary>
    /// Text to be displayed in the summary screen
    /// </summary>
    string DisplayText;

    /// <summary>
    /// If InputtedText has been submitted
    /// </summary>
    bool TextSubmitted;

    /// <summary>
    /// Number of entries in various arrays (max one array per taskType)
    /// </summary>
    int numEntries = 0;

    /// <summary>
    /// What step of the taskCreation is the player on
    /// </summary>
    int step = 0;
    /// <summary>
    /// The highest step that the player can currently go
    /// </summary>
    int MaxStep = 0;
    /// <summary>
    /// A value used for if there's a step that the player cannot currently go past that is not the MaxStep
    /// </summary>
    int restrictedStep = -1;


    List<TaskSO> TasksToCreate = new List<TaskSO>();
    EmailTaskFactory emailFactory;
    IncidentReportTaskFactory IRFactory;

    const string FilePath = "Assets/Prefabs/UI/Tasks";

    void OnGUI()
    {
        SetStyles();

        taskType = (ATask.TaskType)EditorGUILayout.EnumPopup("Task To Create", taskType);

        GUILayout.Space(20);

        switch (taskType)
        {
            case ATask.TaskType.Email:
                GUILayout.Label("Email:");
                EmailTaskGUI();
                break;

            case ATask.TaskType.IncidentReport:
                GUILayout.Label("Incident Report:");
                IRTaskGUI();
                break;

            case ATask.TaskType.Print:
                GUILayout.Label("Print:");
                PrintTaskGUI();
                break;

            case ATask.TaskType.Null:
                GUILayout.Label("Prefab Creator (Final Step)");
                PrefabCreation();
                break;
            default:
                GUILayout.Label("Invalid Task");
                break;
        }

        GUILayout.Space(20);

        StartHorizontalFlexibleSpace();
        if (step > 0 && GUILayout.Button("Back", GUILayout.Width(50))) step--;
        if (step != restrictedStep && step != MaxStep && GUILayout.Button("Next", GUILayout.Width(50))) step++;
        EndHorizontalFlexibleSpace();

        // Reset values if changing to a new type of task
        if (LastSelectedTask != taskType)
        {
            TextSubmitted = false;
            LastSelectedTask = taskType;
            step = 0;
            MaxStep = 0;
            numEntries = 0;

            InputtedText = DefaultTextInputString;
            lastInputtedText = DefaultTextInputString;
            DisplayText = "";
        }
    }

    // Task Creations
    #region 

    // Emails
    #region
    EmailLogic.WordTypes[] BoxInputs;
    string[] ContextStrings;
    const string defaultTextString = "Enter Text For Wordbank Entry";
    private class WordBankWordData
    {
        public string Text = defaultTextString;
        public EmailLogic.WordTypes type = EmailLogic.WordTypes.Null;
        public bool IsCorrect = false;
    }
    List<WordBankWordData> DataList;

    Dictionary<string, EmailTaskFactory.DraggableWordInfo> WordBankEntries;
    internal void EmailTaskGUI()
    {
        switch (step)
        {
            case 0:
                Email_EnterText();
                break;
            case 1:
                Email_SetBoxValues();
                StartHorizontalFlexibleSpace();

                EndHorizontalFlexibleSpace();
                break;
            case 2:
                Email_EnterWordBank();
                break;

            default:
                Email_Summary();
                break;
        }

        GUILayout.Space(20);

        StartHorizontalFlexibleSpace();
        if (step > 0 && GUILayout.Button("Back", GUILayout.Width(50))) step--;
        if (step != restrictedStep && step != MaxStep && GUILayout.Button("Next", GUILayout.Width(50))) step++;
        EndHorizontalFlexibleSpace();
    }

    internal void Email_EnterText()
    {
        // Get the 3 main values
        TaskName = EditorGUILayout.TextField(TaskName);
        InputtedText = EditorGUILayout.TextArea(InputtedText);
        DescriptionText = EditorGUILayout.TextArea(DescriptionText);

        //Ensures Changes are saved before continuing
        if (InputtedText != lastInputtedText)
        {
            restrictedStep = 0;
            TextSubmitted = false;
            lastInputtedText = InputtedText;
        }

        StartHorizontalFlexibleSpace();
        // Submit button
        if (GUILayout.Button("Submit Text", GUILayout.Width(80)))
        {
            restrictedStep = -1;
            TextSubmitted = true;

            Email_SetArraysFromText();

            SetHigherMaxStep(1);
            ++step;
        }
        EndHorizontalFlexibleSpace();
    }

    /// <summary>
    /// Creates a new instance of BoxInputs based on the number of BoxTags.
    /// Also fills the ContextStrings array with the text preceding each tag 
    /// </summary>
    internal void Email_SetArraysFromText()
    {
        string[] brokenText = InputtedText.Split(EmailTaskFactory.BoxTag);
        int count = brokenText.Length - 1;

        BoxInputs = new EmailLogic.WordTypes[count];
        ContextStrings = new string[count];

        for (int i = 0; i < count; i++)
        {
            string prefix = "...";

            if (i == 0) prefix = "";


            string text = brokenText[i];

            const int MaxLength = 20;

            int IndexesToTake = text.Length >= MaxLength ? MaxLength : text.Length;

            ContextStrings[i] = prefix + text.Substring(text.Length - IndexesToTake, IndexesToTake);
        }
    }

    /// <summary>
    /// Gets and sets the values for each WordDropBox, filling BoxInputs in the process
    /// </summary>
    internal void Email_SetBoxValues()
    {
        if (!TextSubmitted)
        {
            //Should never get here but just in case
            GUILayout.Label("Please go back and submit text changes");
            return;
        }

        for (int i = 0; i < BoxInputs.Length; ++i)
        {
            GUIContent Label = new GUIContent("Select a type for box " + i, ContextStrings[i]);
            BoxInputs[i] = (EmailLogic.WordTypes)EditorGUILayout.EnumPopup(Label, BoxInputs[i]);
        }

        // ensure every box has been assigned a type
        // TODO: Check against duplicate types
        if (!BoxInputs.Contains(EmailLogic.WordTypes.Null))
        {
            if (numEntries <= BoxInputs.Length) numEntries = BoxInputs.Length;
            // if DataList hasn't already been set, make a new one, keeps from overriding already written word bank entries
            if (MaxStep == 1)
            {
                DataList = new List<WordBankWordData>(numEntries);
            }
            SetHigherMaxStep(2);
        }
    }

    /// <summary>
    /// Gets and sets the values for each word bank entry, filling WordBankEntries in the process
    /// </summary>
    internal void Email_EnterWordBank()
    {
        // Buttons to increase or decrease the number of word bank entries
        StartHorizontalFlexibleSpace();
        if (GUILayout.Button("Add Word", GUILayout.Width(80))) ++numEntries;
        if (numEntries > BoxInputs.Length && GUILayout.Button("Remove Word", GUILayout.Width(80))) --numEntries;
        EndHorizontalFlexibleSpace();

        GUILayout.Space(10);

        // Adds or removes DataList indecies based on numEntries
        if (DataList.Count < numEntries)
        {
            DataList.Add(new WordBankWordData());
        }
        else if (DataList.Count > numEntries)
        {
            DataList.RemoveAt(numEntries);
        }

        // bool used to check if a text entry is the default text or is empty
        bool containsDefault = false;
        // the number of IsCorrect entries
        int numCorrect = 0;
        // A set of EmailLogic.WordTypes that have a corresponding word bank entry where IsCorrect is true
        HashSet<EmailLogic.WordTypes> CorrectTypes = new HashSet<EmailLogic.WordTypes>();
        // bool used to check if multiple of the same WordType have a correct entry
        bool UniqueCorrect = true;

        for (int i = 0; i < DataList.Count; ++i)
        {
            // Make the input areas for each word to be filled in
            //Outputs to: [Enter text for word bank entry] Word Type [Null] O Is Correct Word
            StartHorizontalFlexibleSpace();
            DataList[i].Text = GUILayout.TextField(DataList[i].Text, GUILayout.Width(160));
            GUILayout.Label("Word Type");
            DataList[i].type = (EmailLogic.WordTypes)EditorGUILayout.EnumPopup(DataList[i].type, GUILayout.MaxWidth(160));
            DataList[i].IsCorrect = GUILayout.Toggle(DataList[i].IsCorrect, "Is Correct Word", GUILayout.MaxWidth(160));
            EndHorizontalFlexibleSpace();

            // Check if any box has not had text entered in, if that is the case it will stop the user from progressing
            if (DataList[i].Text == defaultTextString || DataList[i].Text == "")
            {
                containsDefault = true;
            }

            if (DataList[i].IsCorrect)
            {
                ++numCorrect;
                // Check for if a correct WordType already exists
                if (!CorrectTypes.Add(DataList[i].type))
                {
                    UniqueCorrect = false;
                }
            }
        }

        // Doesn't display the Submit Button unless the various checked prerequisites are met
        if (!containsDefault && UniqueCorrect && numCorrect == BoxInputs.Length && GUILayout.Button("Submit Word Bank"))
        {
            Dictionary<string, EmailTaskFactory.DraggableWordInfo> Entries = new Dictionary<string, EmailTaskFactory.DraggableWordInfo>();

            // Transfers data from a WordBankWordData to the entries dictionary so that we can use a dictionary's 
            // TryAdd method to check for duplicates
            foreach (WordBankWordData data in DataList)
            {
                EmailTaskFactory.DraggableWordInfo info =
                    new EmailTaskFactory.DraggableWordInfo { WordType = data.type, IsCorrect = data.IsCorrect };

                if (!Entries.TryAdd(data.Text, info))
                {
                    Debug.LogError("Duplicate Text Entries");
                    return;
                }
            }

            SetHigherMaxStep(3);
            ++step;

            WordBankEntries = Entries;
            FormatDisplayText();
            return;
        }

        // Various error message labels.
        if (containsDefault)
        {
            GUILayout.Label("Please enter text for all words");
        }

        if (numCorrect > BoxInputs.Length)
        {
            GUILayout.Label("Too many correct options, Required Correct Options " + BoxInputs.Length);
        }
        else if (numCorrect < BoxInputs.Length)
        {
            GUILayout.Label("Not enough correct options, Required Correct Options " + BoxInputs.Length);
        }

        if (!UniqueCorrect)
        {
            GUILayout.Label("Cannot have two instances of the same word type be correct");
        }
    }

    /// <summary>
    /// Formats the displayText to be the output of a correctly filled out task
    /// </summary>
    private void FormatDisplayText()
    {
        DisplayText = InputtedText;
        // A regex containing the BoxTag, used for one by one replacement later
        Regex regex = new Regex(EmailTaskFactory.BoxTag);
        // Loops through each box and WorkdBankEntries key, 
        // replacing each instance of the BoxTag with the text of the word that goes there.
        for (int i = 0; i < BoxInputs.Length; ++i)
        {
            foreach (string key in WordBankEntries.Keys)
            {
                if (BoxInputs[i] == WordBankEntries[key].WordType && WordBankEntries[key].IsCorrect)
                {
                    // Replaces the first instance of the BoxTag with the text of the correct word bank entry.
                    DisplayText = regex.Replace(DisplayText, key, 1);
                }
            }
        }
    }

    /// <summary>
    /// Displays the submit button and a summary of the entered data
    /// </summary>
    internal void Email_Summary()
    {
        EditorGUILayout.LabelField("Summary: ");
        EditorGUILayout.LabelField("Name: " + TaskName, EditorStyles.textField);
        EditorGUILayout.LabelField("Description: " + DescriptionText, EditorStyles.textField);
        EditorGUILayout.LabelField("Successful Email: \n" + DisplayText, EditorStyles.textArea);

        if (GUILayout.Button("Submit Email Task", GUILayout.Width(160)))
        {
            CreateEmail(InputtedText, TaskName, DescriptionText, new List<EmailLogic.WordTypes>(BoxInputs), WordBankEntries);

            step = 0;
            MaxStep = 0;
        }
    }
    #endregion

    // Incident Reports
    #region 
    string[] censoredWords;

    internal void IRTaskGUI()
    {
        switch (step)
        {
            case 0:
                IR_EnterText();
                break;

            default:
                IR_Summary();
                break;
        }

        GUILayout.Space(20);

        StartHorizontalFlexibleSpace();
        if (step > 0 && GUILayout.Button("Back", GUILayout.Width(50))) step--;
        if (step != restrictedStep && step != MaxStep && GUILayout.Button("Next", GUILayout.Width(50))) step++;
        EndHorizontalFlexibleSpace();
    }

    internal void IR_EnterText()
    {
        // Get the 3 main values
        TaskName = EditorGUILayout.TextField(TaskName);
        InputtedText = EditorGUILayout.TextArea(InputtedText);

        //Ensures Changes are saved before continuing
        if (InputtedText != lastInputtedText)
        {
            restrictedStep = 0;
            TextSubmitted = false;
            lastInputtedText = InputtedText;
        }

        StartHorizontalFlexibleSpace();
        // Submit button
        if (GUILayout.Button("Submit Text", GUILayout.Width(80)))
        {
            restrictedStep = -1;
            TextSubmitted = true;

            IR_ParseText();
            IR_DisplayText();

            SetHigherMaxStep(1);
            ++step;
        }
        EndHorizontalFlexibleSpace();
    }

    void IR_ParseText()
    {
        MatchCollection foundTags = Regex.Matches(InputtedText, @$"{IncidentReportTaskFactory.CensorTagStart}.*?{IncidentReportTaskFactory.CensorTagEnd}");
        censoredWords = foundTags.Select(m => m.Value).ToArray();

        for (int i = 0; i < censoredWords.Length; ++i)
        {
            censoredWords[i] = censoredWords[i].Substring(IncidentReportTaskFactory.CensorTagStart.Length, censoredWords[i].Length - IncidentReportTaskFactory.CensorTagStart.Length - IncidentReportTaskFactory.CensorTagEnd.Length);
        }
    }

    void IR_DisplayText()
    {
        string regexTag = @$"{IncidentReportTaskFactory.CensorTagStart}.*?{IncidentReportTaskFactory.CensorTagEnd}";

        DisplayText = Regex.Replace(InputtedText, regexTag, "");
    }

    /// <summary>
    /// Displays the submit button and a summary of the entered data
    /// </summary>
    internal void IR_Summary()
    {
        EditorGUILayout.LabelField("Summary: ");
        EditorGUILayout.LabelField("Name: " + TaskName, EditorStyles.textField);
        EditorGUILayout.LabelField("Successful Incident Report: \n" + DisplayText, EditorStyles.textArea);
        EditorGUILayout.LabelField("Censored Words: \n" + string.Join(',', censoredWords), EditorStyles.textArea);

        if (GUILayout.Button("Submit IR Task", GUILayout.Width(160)))
        {
            CreateIR(InputtedText, TaskName);

            step = 0;
            MaxStep = 0;
        }
    }
    #endregion


    void PrintTaskGUI()
    {
        GUILayout.Label("Not Implemented");
    }

    #endregion

    // ScriptableObject Asset Creation 
    #region 
    /// <summary>
    /// Creates an EmailSO ScriptableObject and saves it to the Assets/Prefabs/UI/Tasks/ScriptableObjects folder
    /// </summary>
    /// <param name="RawText">The text of the email</param>
    /// <param name="Name">the name of the email</param>
    /// <param name="Description">the description of the email</param>
    /// <param name="BoxRequiredWords">The required word types for each WordDropBox</param>
    /// <param name="WordBankWords">The dictionary of data for creating the DraggableWords in the word bank</param>
    void CreateEmail(string RawText, string Name, string Description,
        List<EmailLogic.WordTypes> BoxRequiredWords, Dictionary<string, EmailTaskFactory.DraggableWordInfo> WordBankWords)
    {
        EmailSO emailSO = ScriptableObject.CreateInstance<EmailSO>();
        emailSO.Create(RawText, Name, Description, BoxRequiredWords, WordBankWords);

        AssetDatabase.CreateAsset(emailSO, "Assets/Prefabs/UI/Tasks/ScriptableObjects/" + Name + ".asset");
        AssetDatabase.SaveAssets();
    }

    void CreateIR(string rawText, string name)
    {
        IncidentReportSO incidentReportSO = ScriptableObject.CreateInstance<IncidentReportSO>();
        incidentReportSO.Create(rawText, name);

        AssetDatabase.CreateAsset(incidentReportSO, "Assets/Prefabs/UI/Tasks/ScriptableObjects/" + name + ".asset");
        AssetDatabase.SaveAssets();
    }
    #endregion

    // Prefab Creation
    #region

    void PrefabCreation()
    {
        GUILayout.Label("Add ITaskSO Scriptable Objects To Create");

        StartHorizontalFlexibleSpace();
        if (GUILayout.Button("Add Object", GUILayout.Width(90))) ++numEntries;
        if (numEntries > 0 && GUILayout.Button("Remove Object", GUILayout.Width(90))) --numEntries;
        EndHorizontalFlexibleSpace();

        GUILayout.Space(10);

        if (TasksToCreate.Count < numEntries)
        {
            TasksToCreate.Add(null);
        }
        else if (TasksToCreate.Count > numEntries)
        {
            TasksToCreate.RemoveAt(numEntries);
        }


        for (int i = 0; i < TasksToCreate.Count; ++i)
        {
            TasksToCreate[i] = (TaskSO)EditorGUILayout.ObjectField(TasksToCreate[i], typeof(TaskSO), true);
        }

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Create"))
            {
                for (int i = 0; i < TasksToCreate.Count; ++i)
                {
                    Instantiate(TasksToCreate[i]);
                    TasksToCreate[i] = null;
                }

                TasksToCreate = new List<TaskSO>();
                numEntries = 1;
            }
        }
        else
        {
            GUILayout.Label("Game Must Be Playing To Create");
        }
    }

    void Instantiate(TaskSO task)
    {
        switch (task.taskType)
        {
            case ATask.TaskType.Email:
                InstantiateEmail((EmailSO)task);
                break;

            case ATask.TaskType.IncidentReport:
                InstantiateIR((IncidentReportSO)task);
                break;

            case ATask.TaskType.Print:

                break;
            default:

                break;
        }
    }

    void InstantiateEmail(EmailSO email)
    {
        if (emailFactory == null) emailFactory = new EmailTaskFactory();

        GameObject prefab = emailFactory.Create(email.Initialize());
        PrefabUtility.SaveAsPrefabAsset(prefab, FilePath + "/Emails/" + email.name + ".prefab");
    }

    void InstantiateIR(IncidentReportSO ir)
    {
        if (IRFactory == null) IRFactory = new IncidentReportTaskFactory();

        GameObject prefab = IRFactory.Create(ir);
        PrefabUtility.SaveAsPrefabAsset(prefab, FilePath + "/IncidentReports/" + ir.name + ".prefab");
    }
    #endregion

    // Helper Methods
    #region 
    /// <summary>
    /// Helper method to fill with any custom style modifications, called at the start of OnGUI
    /// </summary>
    void SetStyles()
    {
        EditorStyles.textField.wordWrap = true;
    }

    /// <summary>
    /// Helper method to combine the start of a horizontal group and a flexible space group into one call
    /// </summary>
    void StartHorizontalFlexibleSpace()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
    }

    /// <summary>
    /// Helper method to combine the end of a horizontal group and a flexible space group into one call
    /// </summary>
    void EndHorizontalFlexibleSpace()
    {
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    #endregion

    void SetHigherMaxStep(int value) => MaxStep = MaxStep > value ? MaxStep : value;
}