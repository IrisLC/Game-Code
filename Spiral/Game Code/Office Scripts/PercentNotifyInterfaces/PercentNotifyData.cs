using UnityEngine;

public abstract class PercentNotifyData : ScriptableObject
{
    /// <summary>
    /// What percentage through the day should this be sent?
    /// Negative values indicated immediately
    /// </summary>
    public float PercentThroughDayToSend;
}
