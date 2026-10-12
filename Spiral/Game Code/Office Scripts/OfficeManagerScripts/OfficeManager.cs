using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//type alias to shorten the long declaration
using OfficeRefs = Referencer.OfficeReferences;

public class OfficeManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    static OfficeManager Instance;
    static GameManager gameManager;


    public TaskManager TaskManager { get => taskManager; }
    [SerializeReference] public TaskManager taskManager;
    public StressManager StressManager { get => stressManager; }
    [SerializeReference] StressManager stressManager;

    /// <summary>
    /// If the office level is the level being actively played
    /// </summary>
    public static bool IsOfficePlaying { get; private set; }

    [SerializeField] float DialRotationSpeed = 1f;

    public static List<IncomingEmailData> ReceivedEmails = new();

    [Header("Time")]
    /// <summary>
    /// How far through the office day we currently are, 0-100%
    /// </summary>
    public float percentageDone;
    /// <summary>
    /// The timer that dictates the length of the day in the office. 
    /// Starts at max and goes down to 0, at which point the day ends.
    /// </summary>
    public static CountdownTimer TimeLeftInDay;

    /// <summary>
    /// The set of values that, when percentageDone hits those values, a notify event will be fired.
    ///     Only includes those that have not been hit yet.
    /// </summary>
    static SortedSet<float> percentNotifiesList = new();
    public delegate void PercentNotifyType(float percent);
    /// <summary>
    /// The event fired when percentageDone hits a requested value, also sends the value that was requested.
    /// </summary>
    public static event PercentNotifyType PercentNotify;

    /*Day Phase Events*/

    /// <summary>
    /// Event called when starting a day in the office
    /// </summary>
    public static event Action OnDayStart;
    /// <summary>
    /// Event called when about to enter an Anxiety Scene from the office
    /// </summary>
    public static event Action OnDayPaused;

    /// <summary>
    /// Event called when returning from Anxiety Scene for methods that need to know if the player succeeded.
    /// 
    /// Bool is true iff the player succeeded the maze.
    /// </summary>
    public static event Action<bool> OnDayResumeWithCompleted;
    /// <summary>
    /// Event called when returning from Anxiety Scene for methods that don't care about success.
    /// </summary>
    public static event Action OnDayResume;
    /// <summary>
    /// Event called when starting a day in the office
    /// </summary>
    public static event Action OnDayEnd;

    public static bool BlockSpiralEnter;
    public static bool SpiralWasBlocked;

    void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Assign this as the OfficeManager that all other scripts will reference
        OfficeRefs.officeManager = this;

        if (taskManager == null)
        {
            taskManager = new TaskManager();
        }
        if (stressManager == null)
        {
            stressManager = new StressManager(this, DialRotationSpeed);
        }

        ReceivedEmails = new();
    }

    void OnEnable()
    {
        ATask.OnTaskFinish += OnTaskComplete;
        IncomingEmailSender.EmailSent += OnReceiveEmail;
        // Ensures we aren't assigning an event call before we create the timer
        if (TimeLeftInDay != null)
        {
            TimeLeftInDay.OnTimerStop += EndDay;
        }
    }

    void Start()
    {
        gameManager = Referencer.gameManager;

        // Only once per game will we not have the timer instantiated,
        if (TimeLeftInDay == null)
        {
            // Timer must be created in start, otherwise risk OfficeRefs.dailyModifiers not being created yet
            TimeLeftInDay = new CountdownTimer(OfficeRefs.dailyModifiers.GetLengthOfDay());
            TimeLeftInDay.OnTimerStop += EndDay;
        }

        if (!IsOfficePlaying)
        {
            StartDay();
        }
    }

    void Update()
    {
        // if we aren't playing in the office then we don't need the any further calls.
        if (!IsOfficePlaying) return;

        // progress the timer.
        TimeLeftInDay.Tick(Time.deltaTime);
        helperTime();

        // If we have completed every available task then end the day as the player has nothing to do
        if (taskManager.CheckTasks(TimeLeftInDay, OfficeRefs.dailyModifiers.GetTimeBetweenTasks()))
        {
            EndDay();
        }

        if (StressManager.isStressMaxed) StressMaxed();
    }

    void FixedUpdate()
    {
        if (IsOfficePlaying)
        {
            // Passive stress gain
            stressManager.PassiveStressUpdate(TimeLeftInDay.Speed);
        }
    }

    // Unsubscribe from Events
    void OnDisable()
    {
        TimeLeftInDay.OnTimerStop -= EndDay;
        ATask.OnTaskFinish -= OnTaskComplete;
        IncomingEmailSender.EmailSent -= OnReceiveEmail;
    }

    void OnDestroy()
    {
        if (Instance != this) return;
        // Allows a new singleton to be made
        Instance = null;

        ReceivedEmails.Clear();
        TimeLeftInDay = null;
        percentNotifiesList.Clear();
    }

    /* Enter and leave office scene methods */

    /// <summary>
    /// Starts a new office day
    /// </summary>
    public void StartDay()
    {
        IsOfficePlaying = true;

        if (TimeLeftInDay != null)
        {
            TimeLeftInDay.Reset(OfficeRefs.dailyModifiers.GetLengthOfDay());
        }
        else
        {
            TimeLeftInDay = new CountdownTimer(OfficeRefs.dailyModifiers.GetLengthOfDay());
            TimeLeftInDay.OnTimerStop += EndDay;
        }

        OnDayStart?.Invoke();



        // Set up the tutorial if we are in the first level
        if (OfficeRefs.dailyModifiers.GetIsTutorial())
        {
            TutorialPrep(OfficeRefs.messageNotification,
                OfficeRefs.OpeningMessage,
                OfficeRefs.officeCamera);
        }

        // Begin the timer
        TimeLeftInDay.Start();
    }

    /// <summary>
    /// Pauses a day in the office, used for going into Anxiety Scenes
    /// </summary>
    void PauseDay()
    {
        IsOfficePlaying = false;

        OnDayPaused?.Invoke();

        // Close the computer and pause the time
        OfficeRefs.Computer.ExitComputer();
        TimeLeftInDay.Pause();

        // Start the blur animation
        OfficeRefs.Blur.SetTrigger("Blur");

    }

    /// <summary>
    /// Returns to office following Anxiety Scene sections
    /// </summary>
    public void ResumeDay()
    {
        IsOfficePlaying = true;
        // Fire the events
        OnDayResume?.Invoke();
        OnDayResumeWithCompleted?.Invoke(gameManager.SucceededMaze);

        // Skip forward in the day depending on performance in Maze
        TimeLeftInDay.Time -= OfficeRefs.dailyModifiers.GetTimeLost(gameManager.SucceededMaze);
        TimeLeftInDay.Resume();

    }

    /// <summary>
    /// Finishes a day in the office
    /// </summary>
    void EndDay()
    {
        IsOfficePlaying = false;
        OnDayEnd?.Invoke();

        // Goes to the current Home Scene
        SceneManager.LoadScene(GameManager.CurrentHomeIndex);

    }

    /* Helper Methods */

    ///<summary>
    /// Sets up popups to teach the player the game
    /// </summary>
    void TutorialPrep(messageNotification messageNotification, GameObject OpeningMessage, OfficeCamera officeCamera)
    {
        messageNotification.isFirstTime = true;
        OpeningMessage.SetActive(true);
        officeCamera.IsFirstTime = true;
    }

    /// <summary>
    /// Series of calls for when the Stress level hits it's cap, transitions into Anxiety Scene
    /// </summary>
    void StressMaxed()
    {
        if (!IsOfficePlaying) return;

        EnterSpiral();
    }

    public void EnterSpiral()
    {
        if (BlockSpiralEnter)
        {
            SpiralWasBlocked = true;
        }

        PauseDay();

        gameManager.StartCoroutine(nameof(GameManager.EnterSpiral));
    }

    /// <summary>
    /// Passes along method call for when a task is complete and changes the stress level based on said completion
    /// </summary>
    void OnTaskComplete(ATask Task, bool Correct, int Weight)
    {
        taskManager.CompletedTask(Task, Correct, Weight);
        StressManager.UpdateStress(OfficeRefs.dailyModifiers.GetStressModifierOnTaskCompletion(Correct) * Weight);
    }

    /// <summary>
    /// To be used when leaving anxiety world
    /// </summary>
    /// <param name="timeToRemove"></param>
    public static void RemoveTime(float timeToRemove)
    {
        TimeLeftInDay.Time -= timeToRemove;
    }

    /// <summary>
    /// Converts from the desending timer to the percentage value
    /// </summary>
    void helperTime()
    {
        // Converts GetTimeAscending from 0-InitialTime to 0-100 and assigns 
        percentageDone = TimeLeftInDay.GetTimeAscending().Remap(0, TimeLeftInDay.InitialTime, 0, 100);
        CheckPercentNotify();
    }

    public static OfficeManager GetOfficeManager() => Instance;

    /// <summary>
    /// Adds a new percentage that, when percentageDone exceeds the given value, will fire a PercentNotify event
    /// </summary>
    /// <param name="percentage">The desired percent to be notified at</param>
    public static void AddPercentNotify(float percentage)
    {
        percentNotifiesList.Add(percentage);
    }

    /// <summary>
    /// Checks to see if any requested percentage notify value has been hit in the past frame, 
    ///     and if so fires a PercentNotify event
    /// </summary>
    static void CheckPercentNotify()
    {
        float percent = percentNotifiesList.Min;
        if (Instance.percentageDone >= percent)
        {
            percentNotifiesList.Remove(percent);
            PercentNotify.Invoke(percent);
        }
    }

    void OnReceiveEmail(IncomingEmailData data, GameObject email)
    {
        ReceivedEmails.Add(data);
    }


#if UNITY_INCLUDE_TESTS
    // For Tests
    internal static void TEST_CallStartDay() => OnDayStart.Invoke();
    internal static void TEST_CallResumeDay() => OnDayResume.Invoke();
    internal static void TEST_CallResumeDayParam(bool complete) => OnDayResumeWithCompleted.Invoke(complete);
    internal static void TEST_CallPauseDay() => OnDayPaused.Invoke();
    internal static void TEST_CallEndDay() => OnDayEnd.Invoke();
    internal static void TEST_ClearEvents()
    {
        OnDayStart = null;
        OnDayResume = null;
        OnDayResumeWithCompleted = null;
        OnDayPaused = null;
        OnDayEnd = null;
    }
#endif
}
