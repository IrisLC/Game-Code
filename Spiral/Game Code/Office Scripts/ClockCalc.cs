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
    const int startHour = 9;
    /// <summary>
    /// The hour (in millitary time) the day ends at
    /// </summary>
    const int endHour = 17;
    /// <summary>
    /// How many minutes are progressed each step
    /// </summary>
    const int minutesPerStep = 15;

    /// <summary>
    /// How many minutes in game pass from the start to the end of the day
    /// </summary>
    int totalGameMinutes => (endHour - startHour) * 60;
    /// <summary>
    /// The total number of steps the clock will go through
    /// </summary>
    int totalSteps => totalGameMinutes / minutesPerStep;

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
                if (WarnedOnce) Debug.LogWarning("No CountdownTimer Found, ClockCalc will not update");
                WarnedOnce = true;
                return;
            }


            InvalidTimer = false;
        }

        // We shouldn't be able to get here with the timer being null
        Assert.IsNotNull(TimeLeftInDay);

        if (!TimeLeftInDay.isRunning) return;

        // Get the current step of the clock
        float currentTime = TimeLeftInDay.Time;

        float normalizedTime = Mathf.InverseLerp(InitialTime, 0f, currentTime);
        int newStep = Mathf.Min(Mathf.FloorToInt(normalizedTime * totalSteps), totalSteps);

        // If we are at a new step
        if (currentStep != newStep)
        {
            UpdateClockUI(newStep);
        }

    }

    /// <summary>
    /// Updates the visible clock to the player based on the step the clock should be on 
    /// </summary>
    /// <param name="step">the current clock step</param>
    void UpdateClockUI(int step)
    {
        currentStep = step;

        int currentMinutes = (startHour * 60) + (step * minutesPerStep);

        int hour = currentMinutes / 60;
        int minute = currentMinutes % 60;

        string TimeSuffix = hour > 12 ? "PM" : "AM";

        int displayHour = hour > 12 ? hour - 12 : hour;
        if (displayHour == 0) displayHour = 12;

        //Displays time with a variance based on ShouldHaveSuffix
        clockText.text = ShouldHaveSuffix ? $"{displayHour}:{minute:00} {TimeSuffix}" : $"{displayHour}:{minute:00}";

    }
}
