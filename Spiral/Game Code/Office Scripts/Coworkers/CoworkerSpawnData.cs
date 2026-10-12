using UnityEngine;

[CreateAssetMenu(fileName = "CoworkerSpawnData", menuName = "Scriptable Objects/CoworkerData")]
public class CoworkerSpawnData : PercentNotifyData
{
    /// <summary>
    /// The gameObject representing the coworker in the world.
    /// </summary>
    public GameObject CoworkerObject;

}
