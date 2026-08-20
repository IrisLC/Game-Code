using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A state for the Enemies. Chooses a random point, walks to that point, and repeats
/// </summary>
[Serializable]
public class EStateWander : IState
{
    /// <summary>
    /// The EnemyBrain calling this State's functions
    /// </summary>
    EnemyBrain Parent;

    /// <summary>
    /// Whether or not this state has found a point to go to
    /// </summary>
    bool hasPoint;
    /// <summary>
    /// The point this state will try to reach
    /// </summary>
    Vector3 Target;

    /// <summary>
    /// How far this state will look for a new point
    /// </summary>
    float Range = 50;

    public void OnInitialize(IStateMachineUser parent)
    {
        Parent = parent as EnemyBrain;
    }

    public void OnEnter()
    {
        TryGetRandomPoint();
        EnemyBrain.MoveBody(Parent, Target);
    }

    public void Tick()
    {
        if (hasPoint)
        {
            // If we reach our target, say we no longer have a point so next tick we will find a new one
            if (Vector3.Distance(Parent.GetParentTransform().position, Target) < 1.5f)
            {
                hasPoint = false;

#if UNITY_EDITOR
                if (EnemyBody.isDebugging) Debug.Log($"EStateWander: {Parent.GetParentTransform().gameObject} At Point");
#endif
            }
        }
        else
        {
            // Get a random point and move towards that point
            if (TryGetRandomPoint())
            {
                EnemyBrain.MoveBody(Parent, Target);
            }
        }
    }

    /// <summary>
    /// Attempts to find a random point within range on the navmesh, setting Target to the found point
    /// </summary>
    /// <returns>True iff a point was successfully found, false otherwise</returns>
    bool TryGetRandomPoint()
    {
        // Due to the way that randomPoint works, we may not always have a valid position, 
        //  so we loop through multiple times, so we can give it multiple tries
        for (int i = 0; i < 30; i++)
        {
            // Find a random point on the map within range, then try to find a nearby Navmesh surface on the point
            Vector3 randomPoint = Parent.GetParentTransform().position + UnityEngine.Random.insideUnitSphere * Range;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                // If we've found a valid position start to move towards it
                hasPoint = true;
                Target = hit.position.With(y: Parent.GetParentTransform().position.y);

#if UNITY_EDITOR
                if (EnemyBody.isDebugging)
                {
                    Debug.Log($"EStateWander: {Parent.GetParentTransform().gameObject} Found Point {randomPoint}");
                }
#endif
                return true;
            }

        }

        return false;
    }
}
