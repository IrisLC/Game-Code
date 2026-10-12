using System.Collections.Generic;
using UnityEngine;

public class CoworkerManager : PercentNotifyReceiver<CoworkerSpawnData>
{
    /// <summary>
    /// A collection of GameObjects that will be used for coworker dialogue
    /// Rather than trying to get them every time we spawn a coworker,
    /// they are stored here and referred to upon initialization
    /// </summary>
    [SerializeField] public GameObject messageTextBox;
    [SerializeField] public GameObject speakerNameBox;
    [SerializeField] public Transform messageContainer;
    [SerializeField] public GameObject[] choices;



    /// <summary>
    /// The point in the world to spawn coworkers
    /// </summary>
    [SerializeField] Transform spawnPoint;

    /// <summary>
    /// A queue of coworkers waiting to be spawned.
    /// If we try to spawn a coworker when one is already in the scene, the next coworker to be spawned will be added here
    /// </summary>
    Queue<CoworkerSpawnData> coworkerBuffer = new();
    /// <summary>
    /// The current coworker talking to the player
    /// </summary>
    internal Coworker activeCoworker = null;
    /// <summary>
    /// Whether or not there is a coworker actively talking with the player
    /// </summary>
    public bool isCoworkerActive { get => activeCoworker != null; }

    bool waitForPlayerToSit;

    void Update()
    {
        if (waitForPlayerToSit)
        {
            if (!SitStand.standing)
            {
                SpawnData(coworkerBuffer.Dequeue());
                waitForPlayerToSit = false;
            }
        }
    }

    /// <summary>
    /// Upon the PercentNotify event being fired, checks if it is a percentage this manager has been waiting for. 
    /// If so it spawns the coworker that should be spawned at this time 
    /// (or adds them to the buffer if one is already active). 
    /// </summary>
    /// <param name="percent">the percent through the day that caused a notify event to be fired</param>
    protected override void CheckNotify(float percent)
    {
        if (ToSpawn.TryGetValue(percent, out HashSet<CoworkerSpawnData> list))
        {
            foreach (CoworkerSpawnData coworker in list)
            {
                if (!isCoworkerActive & !SitStand.standing)
                {
                    SpawnData(coworker);
                }
                else
                {
                    if (SitStand.standing) waitForPlayerToSit = true;
                    coworkerBuffer.Enqueue(coworker);
                }
            }

            ToSpawn.Remove(percent);
        }
    }

    /// <summary>
    /// Instantiates a new coworker based on a supplied CoworkerSpawnData
    /// </summary>
    /// <param name="coworker">the info of the coworker to spawn</param>
    protected override void SpawnData(CoworkerSpawnData coworker)
    {
        DataSpawnedToday.Add(coworker);

        GameObject coworkerObject = Instantiate(coworker.CoworkerObject, spawnPoint);
        activeCoworker = coworkerObject.GetComponent<Coworker>();
        activeCoworker.manager = this;
    }



    /// <summary>
    /// Method called by Coworker when it is destroyed.
    /// </summary>
    public void OnCoworkerDelete()
    {
        activeCoworker = null;

        if (coworkerBuffer.Count != 0)
        {
            SpawnData(coworkerBuffer.Dequeue());
        }
    }
}
