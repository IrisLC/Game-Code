using System;
using UnityEngine;

/// <summary>
/// Interface for scripts that can be controlled by either inputs or a state machine
/// </summary>
public interface IControllable
{

    /// <summary>
    /// Moves towards a given target
    /// </summary>
    /// <param name="Target">The location to move to</param>
    abstract void Move(Vector3 Target);
    /// <summary>
    /// Runs Attack Logic
    /// </summary>
    void Attack();
    /// <summary>
    /// Gets the transform of the IControllable object
    /// </summary>
    Transform GetTransform();

}
