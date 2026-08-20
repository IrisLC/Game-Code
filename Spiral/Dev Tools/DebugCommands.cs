
using UnityEditor;
using UnityEngine;
using System.Reflection;
using UnityEngine.SceneManagement;

public class DebugCommands : EditorWindow
{
    [MenuItem("Tools/Debug Tools")]
    public static void GetWindow()
    {
        GetWindow<DebugCommands>();
    }

    SerializedObject SerializedOfficeManager;
    SerializedProperty SerializedTaskManager;
    SerializedProperty OfficeManagerTimeForNextTask;
    SerializedProperty SerializedAnxietyManager;
    SerializedProperty OfficeManagerAnxietyLevel;

    messageNotification messageNotification;
    MethodInfo StartMom;
    MethodInfo StartFriend;
    float timeToProgress;

    float TimerRate;

    bool EnableDebug;

    bool IsDisplaying;

    void OnEnable()
    {
        SceneManager.sceneLoaded += Activate;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= Activate;
    }

    void Activate(Scene scene, LoadSceneMode mode)
    {
        // Serialize necessary objects for the office scenes
        if (scene.buildIndex == GameManager.CurrentOfficeIndex)
        {
            SerializedOfficeManager = new SerializedObject(Referencer.OfficeReferences.officeManager);
            SerializedTaskManager = SerializedOfficeManager?.FindProperty("taskManager");
            OfficeManagerTimeForNextTask = SerializedTaskManager?.FindPropertyRelative("TimeForNextTask");
            SerializedAnxietyManager = SerializedOfficeManager?.FindProperty("anxietyManager");
            OfficeManagerAnxietyLevel = SerializedAnxietyManager?.FindPropertyRelative("AnxietyLevel");

            messageNotification = Referencer.OfficeReferences.messageNotification;
            StartMom = typeof(messageNotification).GetMethod("StartMom", BindingFlags.Instance | BindingFlags.NonPublic);
            StartFriend = typeof(messageNotification).GetMethod("StartFriend", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        TimerRate = 1;
    }

    void OnGUI()
    {
        GUIStyle CenteredTitle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };

        GUILayout.BeginArea(new Rect(10, 10, position.width - 20, position.height));
        GUILayout.Label("Debug Commands", CenteredTitle);

        GUILayout.Space(10);
        if (GUILayout.Button("Toggle Debug Commands")) EnableDebug = !EnableDebug;
        GUILayout.Space(10);

        if (EnableDebug)
        {
            IsDisplaying = false;

            //If we are in the office and the officeManager is not null
            if (SceneManager.GetActiveScene().buildIndex == GameManager.CurrentOfficeIndex
                 && SerializedOfficeManager != null)
            {
                GUILayout.Label("Office Commands", EditorStyles.label);
                OfficeManagerButtons();
                IsDisplaying = true;
            }
            //If in the maze show these options
            else if (SceneManager.GetActiveScene().buildIndex == GameManager.CurrentMazeIndex && Application.isPlaying)
            {
                GUILayout.Label("Anxiety Commands", EditorStyles.label);
                AnxietyMazeButtons();
                IsDisplaying = true;
            }
            else if (SceneManager.GetActiveScene().buildIndex != 0 && Application.isPlaying)
            {
                Activate(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            }

            if (!IsDisplaying)
            {
                if (Application.isPlaying)
                {
                    GUILayout.Label("Error displaying commands.", CenteredTitle);
                    GUILayout.Label("Either unsupported scene or null reference to required component.", EditorStyles.label);
                }
                else
                {
                    GUILayout.Label("Please start game to display commands", CenteredTitle);
                }
            }

            if (Application.isPlaying)
            {
                DebugLogToggles();
            }

        }
        else
        {
            GUILayout.Label("Debug Commands Off.", CenteredTitle);
        }

        GUILayout.EndArea();
    }

    void OfficeManagerButtons()
    {
        SerializedOfficeManager.Update();
        //Add tasks
        if (GUILayout.Button("Add Next Task"))
        {
            OfficeManagerTimeForNextTask.floatValue = OfficeManager.TimeLeftInDay.Time;
            SerializedOfficeManager.ApplyModifiedProperties();
        }
        //Complete Tasks
        if (Referencer.OfficeReferences.officeManager.TaskManager.OldestTask != null)
        {
            if (GUILayout.Button("Succeed Task"))
            {
                Referencer.OfficeReferences.officeManager.TaskManager.OldestTask.Task.DevCommandSubmitSuccess();
            }

            if (GUILayout.Button("Fail Task"))
            {
                Referencer.OfficeReferences.officeManager.TaskManager.OldestTask.Task.Submit();
            }
            GUILayout.Label("If task was completed successfully will come to true", EditorStyles.miniLabel);
        }


        GUILayout.Space(10);
        // Call next dialogue event
        if (messageNotification != null && (StartMom != null || StartFriend != null))
        {
            if (GUILayout.Button("Call Next Dialogue"))
            {
                if (!CharacterDialogueClass.HasMomDialogueStarted())
                {
                    StartMom?.Invoke(messageNotification, null);
                }
                else
                {
                    StartFriend?.Invoke(messageNotification, null);
                }
            }
        }
        // Change Scenes
        if (GUILayout.Button("Enter Anxiety Scene"))
        {
            OfficeManagerAnxietyLevel.floatValue = 100f;
            SerializedOfficeManager.ApplyModifiedProperties();
        }

        GUILayout.Space(20);
        // Timer Settings
        GUILayout.Label($"Adjust Day Timer Speed: {TimerRate:F2}", EditorStyles.label);
        GUILayout.BeginHorizontal();
        TimerRate = GUILayout.HorizontalSlider(TimerRate, 0, 5);
        if (GUILayout.Button("Reset")) TimerRate = 1;
        GUILayout.EndHorizontal();
        OfficeManager.TimeLeftInDay.Speed = TimerRate;
        // Timer Data
        GUILayout.Label($"Percent through the day: {SerializedOfficeManager.FindProperty("percentageDone").floatValue:F2}", EditorStyles.label);
        GUILayout.Label($"Time Elapsed In Day: {OfficeManager.TimeLeftInDay.GetTimeAscending():F2}", EditorStyles.label);
        GUILayout.Label($"Time Remaining  In Day: {OfficeManager.TimeLeftInDay.Time:F2}", EditorStyles.label);
        // Jump time forward
        GUILayout.Label("Progress Time", EditorStyles.label);
        GUILayout.BeginHorizontal();
        timeToProgress = EditorGUILayout.FloatField(timeToProgress);
        if (GUILayout.Button("Enter")) OfficeManager.TimeLeftInDay.Time -= timeToProgress;
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        // End day
        if (GUILayout.Button("End Day"))
        {
            OfficeManager.TimeLeftInDay.Time = .5f;
        }
    }

    void AnxietyMazeButtons()
    {
        // Leave level
        if (GUILayout.Button("Complete Maze"))
        {
            MazeManager.FinishLevel(true);
        }

        if (GUILayout.Button("Kill Player"))
        {
            MazeManager.FinishLevel(false);
        }

        GUILayout.Space(20);
        // Change speed of oxygen descent
        GUILayout.Label($"Adjust Day Timer Speed: {TimerRate:F2}", EditorStyles.label);
        TimerRate = GUILayout.HorizontalSlider(TimerRate, 0, 5);

        GUILayout.Space(10);

        if (GUILayout.Button("Reset")) TimerRate = 1;
        Referencer.AnxietyReferences.PlayerScript.TimerSpeed = TimerRate;

        GUILayout.Space(10);
        // Debug vision cone toggle
        EnemyBrain.ShowEnemyVisionCones =
            GUILayout.Toggle(EnemyBrain.ShowEnemyVisionCones,
            "Show Enemy Vision Cones In Editor", EditorStyles.toggle);
    }

    void DebugLogToggles()
    {
        GUILayout.Space(10);
        GUILayout.Label("Run in code Debug commands (does not affect functionality of previous commands)");

        GameManager.isDebugging = EditorGUILayout.Toggle("Debug Game Manager", GameManager.isDebugging);
        Interactable.isDebugging = EditorGUILayout.Toggle("Debug Interactables", Interactable.isDebugging);

        GUILayout.Space(10);

        CursorManager.isDebugging = EditorGUILayout.Toggle("Debug Cursor Manager", CursorManager.isDebugging);
        TaskPrefab.isDebugging = EditorGUILayout.Toggle("Debug Task Prefabs", TaskPrefab.isDebugging);

        DialogueManager.isDebugging = EditorGUILayout.Toggle("Debug Dialogue", DialogueManager.isDebugging);
        ContactManager.isDebugging = EditorGUILayout.Toggle("Debug Contact Manager", ContactManager.isDebugging);

        GUILayout.Space(10);

        EnemyBody.isDebugging = EditorGUILayout.Toggle("Debug Enemies", EnemyBody.isDebugging);
    }

    void OnInspectorUpdate()
    {
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}
