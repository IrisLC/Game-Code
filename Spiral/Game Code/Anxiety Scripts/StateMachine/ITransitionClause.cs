
using System;
using UnityEngine;

[Serializable]
public abstract class ITransitionClause
{
    /// <summary>
    /// The condition that will be evaluated to see if a transition can occur
    /// </summary>
    /// <returns>true if the transition can occur</returns>
    public abstract bool Evaluate(IStateMachineUser Parent);
}

/// <summary>
/// Transitions when enemy sees the player
/// </summary>
[Serializable]
public class EnemySeesPlayer : ITransitionClause
{
    public override bool Evaluate(IStateMachineUser Parent)
    {
        EnemyBrain brain = Parent as EnemyBrain;
        return brain.SeesPlayer;
    }
}

/// <summary>
/// Transitions when enemy loses interest in the player
/// </summary>
[Serializable]
public class EnemyLostInterest : ITransitionClause
{
    public override bool Evaluate(IStateMachineUser Parent)
    {
        EnemyBrain brain = Parent as EnemyBrain;
        return !brain.IsInterestedInPlayer;
    }
}