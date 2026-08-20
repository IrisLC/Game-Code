using System;
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
    [SerializeReference] TaskManager taskManager;
    public AnxietyManager AnxietyManager { get => anxietyManager; }
    [SerializeReference] AnxietyManager anxietyManager;

    /// <summary>
    /// If the office level is the level being actively played
    /// </summary>
    bool IsOfficePlaying;

    [SerializeField] float DialRotationSpeed = 1f;

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

        taskManager = new TaskManager();
        anxietyManager = new AnxietyManager(this, DialRotationSpeed);
    }

    void OnEnable()
    {
        ATask.OnTaskFinish += OnTaskComplete;
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

        StartDay();
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

        if (anxietyManager.isAnxietyMaxed) AnxietyMaxed();
    }

    void FixedUpdate()
    {
        if (IsOfficePlaying)
        {
            // Passive anxiety gain
            anxietyManager.PassiveAnxietyUpdate(TimeLeftInDay.Speed);
        }
    }

    // Unsubscribe from Events
    void OnDisable()
    {
        TimeLeftInDay.OnTimerStop -= EndDay;
        ATask.OnTaskFinish -= OnTaskComplete;
    }

    void OnDestroy()
    {
        // Allows a new singleton to be made
        if (Instance == this) Instance = null;
    }

    /* Enter and leave office scene methods */

    /// <summary>
    /// Starts a new office day
    /// </summary>
    public void StartDay()
    {
        if (TimeLeftInDay != null)
        {
            TimeLeftInDay.Reset(OfficeRefs.dailyModifiers.GetLengthOfDay());
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

        IsOfficePlaying = true;
    }

    /// <summary>
    /// Pauses a day in the office, used for going into Anxiety Scenes
    /// </summary>
    void PauseDay()
    {
        OnDayPaused?.Invoke();

        // Close the computer and pause the time
        OfficeRefs.Computer.ExitComputer();
        TimeLeftInDay.Pause();

        // Start the blur animation
        OfficeRefs.Blur.SetTrigger("Blur");

        IsOfficePlaying = false;
    }

    /// <summary>
    /// Returns to office following Anxiety Maze sections
    /// </summary>
    public void ResumeDay()
    {
        // Fire the events
        OnDayResume?.Invoke();
        OnDayResumeWithCompleted?.Invoke(gameManager.SucceededMaze);

        // Skip forward in the day depending on performance in Maze
        TimeLeftInDay.Time -= OfficeRefs.dailyModifiers.GetTimeLost(gameManager.SucceededMaze);
        TimeLeftInDay.Resume();

        IsOfficePlaying = true;
    }

    /// <summary>
    /// Finishes a day in the office
    /// </summary>
    void EndDay()
    {
        OnDayEnd?.Invoke();

        // Goes to the current Home Scene
        SceneManager.LoadScene(GameManager.CurrentHomeIndex);

        IsOfficePlaying = false;
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
    /// Series of calls for when the Anxiety level hits it's cap, transitions into Anxiety Scene
    /// </summary>
    void AnxietyMaxed()
    {
        if (!IsOfficePlaying) return;
        PauseDay();

        gameManager.StartCoroutine(nameof(GameManager.EnterAnxietyAttack));
    }

    /// <summary>
    /// Passes along method call for when a task is complete and changes the anxiety level based on said completion
    /// </summary>
    void OnTaskComplete(ATask Task, bool Correct, int Weight)
    {
        taskManager.CompletedTask(Task, Correct, Weight);
        anxietyManager.UpdateAnxiety(OfficeRefs.dailyModifiers.GetAnxietyModifierOnTaskCompletion(Correct) * Weight);
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
    }

    public static OfficeManager GetOfficeManager() => Instance;

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
