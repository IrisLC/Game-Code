using System;
using UnityEngine;

/// <summary>
/// Monobehavior for coworker actions, holds actions for informing other scripts that the coworker has spawned or despawned.
/// </summary>
/// TODO: Add coworker dialogue as well as a cause for despawning
public class Coworker : MonoBehaviour
{
    /// <summary>
    /// Event fired when a coworker spawns
    /// </summary>
    public static event Action<GameObject> CoworkerSpawn;
    /// <summary>
    /// the CoworkerManager that spawned this coworker
    /// </summary>
    public CoworkerManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // turn off the computer
        if (Referencer.OfficeReferences.Computer.IsComputerOn)
        {
            Referencer.OfficeReferences.Computer.ExitComputer();
        }

        manager = Referencer.OfficeReferences.coworkerManager;

        CoworkerSpawn.Invoke(gameObject);

        SitStand.DisableStanding = true;
        OfficeManager.BlockSpiralEnter = true;

    }

    void OnDestroy()
    {
        manager.OnCoworkerDelete();
        SitStand.DisableStanding = false;
        OfficeManager.BlockSpiralEnter = false;

        if (OfficeManager.SpiralWasBlocked)
        {
            OfficeManager.GetOfficeManager().EnterSpiral();
        }
    }
}
