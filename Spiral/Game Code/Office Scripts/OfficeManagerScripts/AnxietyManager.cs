using System;
using System.Collections;
using UnityEngine;



//type alias to shorten the long declaration
using OfficeRefs = Referencer.OfficeReferences;

[Serializable]
public class AnxietyManager
{

    [Header("Anxiety Values")]
    /// <summary>
    /// How anxious Mallory is
    /// </summary>
    [SerializeField] float AnxietyLevel;
    /// <summary>
    /// How high AnxietyLevel can get before entering anxiety scene
    /// </summary>
    const float AnxietyThreshold = 100;
    /// <summary>
    /// The speed at which the anxiety meter dial rotates to show new level
    /// </summary>
    float DialRotationSpeed;
    /// <summary>
    /// A bool that's true if the AnxietyLevel has gotten up to the Anxiety threshold
    /// </summary>
    public bool isAnxietyMaxed { get => AnxietyLevel >= AnxietyThreshold; }

    Coroutine RotateDialCoroutine;

    MonoBehaviour Parent;

    public AnxietyManager(MonoBehaviour Parent = null, float dialRotationSpeed = 1)
    {
        OfficeManager.OnDayStart += ResetAnxietyDial;
        OfficeManager.OnDayPaused += OnPauseDay;
        OfficeManager.OnDayResumeWithCompleted += OnResumeDay;

        this.Parent = Parent;
        DialRotationSpeed = dialRotationSpeed;
    }


    // Destructor, cleans up event subscriptions
    ~AnxietyManager()
    {
        OfficeManager.OnDayStart -= ResetAnxietyDial;
        OfficeManager.OnDayPaused -= OnPauseDay;
        OfficeManager.OnDayResumeWithCompleted -= OnResumeDay;
    }

    /// <summary>
    /// Resets the AnxietyLevel to 0
    /// </summary>
    public void ResetAnxiety()
    {
        ResetAnxiety(0);
    }

    /// <summary>
    /// Resets the AnxietyLevel to a given starting value
    /// </summary>
    /// <param name="startingValue">The value to set the AnxietyLevel to</param>
    public void ResetAnxiety(float startingValue)
    {
        UpdateAnxiety(-AnxietyLevel + startingValue);
    }

    /// <summary>
    /// Adds a the PassiveAnxietyModifier found in dailyModifiers to the AnxietyLevel
    /// </summary>
    /// <param name="gameSpeed">The current speed of the day, currently based in OfficeManager.TimeLeftInDay.Speed</param>
    public void PassiveAnxietyUpdate(float gameSpeed)
    {
        UpdateAnxiety(OfficeRefs.dailyModifiers.GetPassiveAnxietyModifier() * Time.fixedDeltaTime * gameSpeed);
    }

    /// <summary>
    /// Adds a given value to the AnxietyLevel and rotates the anxiety dial to match
    /// </summary>
    /// <param name="modificationValue">the value to add</param>
    public void UpdateAnxiety(float modificationValue)
    {
        AnxietyLevel += modificationValue;
        // Go from the current AnxietyLevel to the angle that the dial should be at
        float dialRotation = AnxietyLevel.Remap(0, AnxietyThreshold, 90, -90);

        if (Parent == null)
        {
            return;
        }

        //Restart the dial to rotate to the new value
        if (RotateDialCoroutine != null)
        {
            Parent.StopCoroutine(RotateDialCoroutine);
        }

        RotateDialCoroutine = Parent.StartCoroutine(RotateDial(dialRotation));
    }

    /// <summary>
    /// Rotates the anxiety dial to a new position so as to match the current AnxietyLevel
    /// </summary>
    /// <param name="targetRotation">The z rotation that the dial should move to</param>
    IEnumerator RotateDial(float targetRotation)
    {
        Quaternion targetQuaternion = Quaternion.Euler(0, 0, targetRotation);

        //Loops until the dial has gotten to the right value
        while (OfficeRefs.AnxietyDial.rectTransform.rotation != targetQuaternion)
        {
            // Gets a rotation moving from the dial's current rotation, towards the target rotation, 
            // at a speed based on the provided DialRotationSpeed
            Quaternion newRotation = Quaternion.RotateTowards(OfficeRefs.AnxietyDial.rectTransform.rotation,
                targetQuaternion, DialRotationSpeed * Time.deltaTime);

            OfficeRefs.AnxietyDial.rectTransform.rotation = newRotation;

            // Waits till next frame
            yield return null;
        }
    }

    /// <summary>
    /// The actions to perform when the day gets paused (entering Anxiety Scene)
    /// </summary>
    public void OnPauseDay()
    {
        if (Parent != null)
        {
            Parent.StopCoroutine(RotateDialCoroutine);
        }
    }

    public void OnResumeDay(bool succeeded)
    {
        ResetAnxietyDial();

        // If the player succeeded in the maze then their anxiety gets reset, otherwise it incurs a penalty
        if (succeeded)
        {
            ResetAnxiety();
        }
        else
        {
            ResetAnxiety(OfficeRefs.dailyModifiers.GetAnxietyPunishment());
        }
    }

    /// <summary>
    /// Returns the current AnxietyLevel
    /// </summary>
    /// <returns>The current AnxietyLevel</returns>
    public float GetAnxiety() => AnxietyLevel;

    /// <summary>
    /// Resets the AnxietyDial to the furthest left position
    /// </summary>
    public static void ResetAnxietyDial()
    {
        if (OfficeRefs.AnxietyDial != null)
        {
            OfficeRefs.AnxietyDial.rectTransform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }


}
