using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class for classes that want to use OfficeManager's PercentNotify methods to trigger/spawn data throughout the day
/// </summary>
/// <typeparam name="T">A scriptable object inheriting from PercentNotifyData</typeparam>
public abstract class PercentNotifyReceiver<T> : MonoBehaviour where T : PercentNotifyData
{
    /// <summary>
    /// The list of data that will be spawned today, used for adding data in inspector.
    /// DO NOT USE OUTSIDE OF AWAKE
    /// </summary>
    [SerializeField] protected List<T> toSpawnToday = new();
    /// <summary>
    /// A dictionary for every data object that is meant to be spawned, 
    /// the key is the time when they should spawn.
    /// toSpawnToday list is converted into this on start for more efficient lookup and removal
    /// </summary>
    protected static Dictionary<float, HashSet<T>> ToSpawn = new();

    protected static HashSet<T> DataSpawnedToday = new();

    /// <summary>
    /// Converts toSpawnToday entries into ToSpawn entries
    /// </summary>
    protected virtual void Awake()
    {
        foreach (T data in toSpawnToday)
        {
            // If we've already spawned the entry today don't add it
            if (DataSpawnedToday.Contains(data))
            {
                continue;
            }

            AddDataToDictionary(data);
        }

        toSpawnToday.Clear();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        foreach (float percent in ToSpawn.Keys)
        {
            SubscribeToNotify(percent);
        }
    }

    /// <summary>
    /// Adds a new data entry to ToSpawn, and creates a new list if no other entries exist
    /// </summary>
    /// <param name="data"></param>
    protected static void AddDataToDictionary(T data)
    {
        float percent = data.PercentThroughDayToSend;
        // If we've not already added something at this percent, create a new entry
        if (!ToSpawn.ContainsKey(percent))
        {
            ToSpawn.Add(percent, new());
        }
        // add to the list of things to spawn at this percentage
        ToSpawn[percent].Add(data);
    }

    protected static void SubscribeToNotify(float percent)
    {
        OfficeManager.AddPercentNotify(percent);
    }

    protected virtual void OnEnable()
    {
        OfficeManager.PercentNotify += CheckNotify;
        OfficeManager.OnDayEnd += ClearDataSpawnedToday;
    }

    protected virtual void OnDisable()
    {
        OfficeManager.PercentNotify -= CheckNotify;
        OfficeManager.OnDayEnd -= ClearDataSpawnedToday;
    }

    /// <summary>
    /// Upon the PercentNotify event being fired, checks if it is a percentage this receiver has been waiting for. 
    /// If so it spawns the data that should be spawned at this time 
    /// </summary>
    /// <param name="percent">the percent through the day that caused a notify event to be fired</param>
    protected virtual void CheckNotify(float percent)
    {
        if (ToSpawn.TryGetValue(percent, out HashSet<T> list))
        {
            foreach (T data in list)
            {
                SpawnData(data);
            }
            ToSpawn.Remove(percent);
        }
    }

    /// <summary>
    /// Handles what is to be done with the data when it hits the correct percent through the day.
    /// Note: must include DataSpawnedToday.Add(data);
    /// </summary>
    /// <param name="data"></param>
    protected abstract void SpawnData(T data);

    protected static void ClearDataSpawnedToday() => DataSpawnedToday.Clear();
}
