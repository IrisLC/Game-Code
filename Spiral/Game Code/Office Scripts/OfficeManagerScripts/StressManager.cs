using System;
using UnityEngine;



//type alias to shorten the long declaration
using OfficeRefs = Referencer.OfficeReferences;

[Serializable]
public class StressManager
{

    [Header("Stress Values")]
    /// <summary>
    /// How stressed Mallory is
    /// </summary>
    internal static float StressLevel;
    /// <summary>
    /// How high StressLevel can get before entering anxiety scene
    /// </summary>
    const float StressThreshold = 100;


    /// <summary>
    /// A bool that's true if the StressLevel has gotten up to the stress threshold
    /// </summary>
    public static bool isStressMaxed { get => StressLevel >= StressThreshold; }

    /// <summary>
    /// The current stage of stress we are at.
    /// Stress stages are used for choosing the currently active set of possible random events.
    /// Value will be between 1 and TotalStressStages. 
    /// </summary>
    public static int CurrentStressStage { get; private set; }
    /// <summary>
    /// How many stress stages there are.
    /// </summary>
    public const int TotalStressStages = 5;
    /// <summary>
    /// How much stress is needed to get from one stage to the next.
    /// ex. stage 1 = [0, StressStageRange), stage 2 = [StressStageRange, 2 * StressStageRange)...
    /// </summary>
    const float StressStageRange = StressThreshold / TotalStressStages;

    MonoBehaviour Parent;

    const int NumTasksForMultiplierStart = 3;
    const float PerTaskMultiplierIncrease = 0.1f;
    const float PerOverdueMultiplierIncrease = 0.2f;

    public static float PerTaskMultiplier { get; private set; } = 1;
    public static float OverdueMultiplier { get; private set; } = 1;

    public StressManager(MonoBehaviour Parent = null, float dialRotationSpeed = 1)
    {
        OfficeManager.OnDayStart += ResetStress;
        OfficeManager.OnDayEnd += ResetStress;
        OfficeManager.OnDayResumeWithCompleted += OnResumeDay;

        this.Parent = Parent;

        ResetStress();
        //windowController = GameObject.Find("Window").GetComponent<WindowWeatherController>();
    }


    // Destructor, cleans up event subscriptions
    ~StressManager()
    {
        OfficeManager.OnDayStart -= ResetStress;
        OfficeManager.OnDayEnd -= ResetStress;
        OfficeManager.OnDayResumeWithCompleted -= OnResumeDay;
    }

    /// <summary>
    /// Resets the StressLevel to 0
    /// </summary>
    public static void ResetStress()
    {
        StressLevel = 0;
        CurrentStressStage = 0;
    }

    /// <summary>
    /// Resets the StressLevel to a given starting value
    /// </summary>
    /// <param name="startingValue">The value to set the StressLevel to</param>
    public static void ResetStress(float startingValue)
    {
        ResetStress();
        UpdateStress(startingValue);
    }

    /// <summary>
    /// Adds a the PassiveStressModifier found in dailyModifiers to the StressLevel
    /// </summary>
    /// <param name="gameSpeed">The current speed of the day, currently based in OfficeManager.TimeLeftInDay.Speed</param>
    public void PassiveStressUpdate(float gameSpeed)
    {
        PerTaskMultiplier = 1;
        OverdueMultiplier = 1;
        if (Parent != null && Parent is OfficeManager && ((OfficeManager)Parent).TaskManager != null)
        {
            int taskCount = ((OfficeManager)Parent).TaskManager.ActiveTasks.Count;
            // if there're no tasks then there's nothing to stress about
            if (taskCount == 0)
            {
                return;
            }
            else
            {
                if (taskCount >= NumTasksForMultiplierStart) // if we have more than 3 active tasks we stress more 
                {
                    PerTaskMultiplier += taskCount * PerTaskMultiplierIncrease;
                }

                OverdueMultiplier += TaskBlockProcessor.OverdueTasks * PerOverdueMultiplierIncrease;
            }
        }
        UpdateStress(OfficeRefs.dailyModifiers.GetPassiveStressModifier() * PerTaskMultiplier * OverdueMultiplier
            * Time.fixedDeltaTime * gameSpeed);
    }

    /// <summary>
    /// Adds a given value to the StressLevel and rotates the stress dial to match
    /// </summary>
    /// <param name="modificationValue">the value to add</param>
    public static void UpdateStress(float modificationValue)
    {
        if (!OfficeManager.IsOfficePlaying) return;

        StressLevel += modificationValue;

        // Clamp stage between 1 and TotalStressStages
        CurrentStressStage = Math.Clamp((int)(StressLevel / StressStageRange) + 1, 1, TotalStressStages);
    }

    public void OnResumeDay(bool succeeded)
    {
        // If the player succeeded in the maze then their anxiety gets reset, otherwise it incurs a penalty
        if (succeeded)
        {
            ResetStress();
        }
        else
        {
            ResetStress(OfficeRefs.dailyModifiers.GetStressPunishment());
        }
    }

    /// <summary>
    /// Returns the current StressLevel
    /// </summary>
    /// <returns>The current StressLevel</returns>
    public static float GetStress() => StressLevel;


}
