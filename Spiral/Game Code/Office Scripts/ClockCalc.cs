using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class ClockCalc : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI clockText;
    /// <summary>
    /// Should the time have AM or PM ammended on to the end of the string
    /// 
    /// (Should be false for phone & true for computer)
    /// </summary>
    [SerializeField] bool ShouldHaveSuffix;

    /// <summary>
    /// The hour the day starts at
    /// </summary>
    public const int startHour = 9;
    /// <summary>
    /// The hour (in millitary time) the day ends at
    /// </summary>
    public const int endHour = 17;
    /// <summary>
    /// How many minutes are progressed each step
    /// </summary>
    const int minutesPerStep = 15;

    /// <summary>
    /// How many minutes in game pass from the start to the end of the day
    /// </summary>
    const int totalGameMinutes = (endHour - startHour) * 60;
    /// <summary>
    /// The total number of steps the clock will go through
    /// </summary>
    const int totalSteps = totalGameMinutes / minutesPerStep;

    int currentStep;

    /// <summary>
    /// The starting time of the OfficeManager.TimeLeftInDay
    /// </summary>
    float InitialTime;

    /// <summary>
    /// A reference to OfficeManager.TimeLeftInDay
    /// </summary>
    CountdownTimer TimeLeftInDay;
    /// <summary>
    /// If there is not a timer
    /// </summary>
    bool InvalidTimer;
    /// <summary>
    /// Ensures a Debug Warning is only printed once instead of every frame
    /// </summary>
    bool WarnedOnce;


    void Start()
    {
        // Sets the UI to the base value
        UpdateClockUI(0);

        // Try to get the timer
        TimeLeftInDay = OfficeManager.TimeLeftInDay;

        if (TimeLeftInDay == null)
        {
            InvalidTimer = true;
            return;
        }

        InitialTime = TimeLeftInDay.InitialTime;
    }

    void Update()
    {
        // If the timer was not correctly gotten, try again
        if (InvalidTimer)
        {
            TimeLeftInDay = OfficeManager.TimeLeftInDay;

            if (TimeLeftInDay == null)
            {
                if (!WarnedOnce) Debug.LogWarning("No CountdownTimer Found, ClockCalc will not update");
                WarnedOnce = true;
                return;
            }

            InitialTime = TimeLeftInDay.InitialTime;
            InvalidTimer = false;
        }

        // We shouldn't be able to get here with the timer being null
        Assert.IsNotNull(TimeLeftInDay);

        if (!TimeLeftInDay.isRunning) return;

        UpdateClockUI();
    }

    /// <summary>
    /// Updates the visible clock to the player based on the step the clock should be on 
    /// </summary>
    /// <param name="step">the current clock step</param>
    void UpdateClockUI()
    {

        clockText.text = DisplayCurrentTime(ShouldHaveSuffix);

    }

    void UpdateClockUI(int time)
    {
        if (TimeLeftInDay != null)
        {
            clockText.text = DisplayTimeAtGivenTime(time, ShouldHaveSuffix);
        }
        else
        {
            clockText.text = DisplayTimeAtGivenTime(time, ShouldHaveSuffix);
        }

    }

    public static string DisplayCurrentTime(bool ShouldHaveSuffix = false)
    {

        float currentTime = OfficeManager.TimeLeftInDay.Time;
        // Get time between 0 and 1
        float normalizedTime = Mathf.InverseLerp(OfficeManager.TimeLeftInDay.InitialTime, 0f, currentTime);
        int newStep = Mathf.Min(Mathf.FloorToInt(normalizedTime * totalSteps), totalSteps);

        return CalculateTime(newStep, ShouldHaveSuffix);
    }

    public static string DisplayTimeAtGivenTime(int hours, int minutes, bool ShouldHaveSuffix = false)
    {
        return DisplayTimeAtGivenTime(TimeToMinutes(hours, minutes), ShouldHaveSuffix);
    }

    public static string DisplayTimeAtGivenTime(int minutes, bool ShouldHaveSuffix = false)
    {
        // get minutes since day start
        minutes -= startHour * 60;
        int step = minutes / minutesPerStep;

        return CalculateTime(step, ShouldHaveSuffix);
    }

    static string CalculateTime(int step, bool ShouldHaveSuffix)
    {
        int currentMinutes = (startHour * 60) + (step * minutesPerStep);

        int hour = currentMinutes / 60;
        int minute = currentMinutes % 60;

        string TimeSuffix = hour > 12 ? "PM" : "AM";

        int displayHour = hour > 12 ? hour - 12 : hour;
        if (displayHour == 0) displayHour = 12;

        //Displays time with a variance based on ShouldHaveSuffix
        return ShouldHaveSuffix ? $"{displayHour}:{minute:00} {TimeSuffix}" : $"{displayHour}:{minute:00}";
    }

    public static int GetCurrentMinutes()
    {
        float currentTime = OfficeManager.TimeLeftInDay.Time;
        // Get time between 0 and 1
        float normalizedTime = Mathf.InverseLerp(OfficeManager.TimeLeftInDay.InitialTime, 0f, currentTime);
        int step = Mathf.Min(Mathf.FloorToInt(normalizedTime * totalSteps), totalSteps);
        return (startHour * 60) + (step * minutesPerStep);
    }

    public static int TimeToMinutes(int Hour, int Minute)
    {
        return (Hour * 60) + Minute;
    }
}
