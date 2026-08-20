using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A state for the Enemies. Chases the player whilst this state is active
/// </summary>
[Serializable]
public class EStateChase : IState
{
    /// <summary>
    /// The EnemyBrain calling this State's functions
    /// </summary>
    EnemyBrain Parent;
    /// <summary>
    /// The timer for when to recalculate the path towards the player
    /// </summary>
    CountdownTimer RepathTimer;
    /// <summary>
    /// If it is time to repath 
    /// </summary>
    bool ShouldRepath;


    public void OnInitialize(IStateMachineUser parent)
    {
        Parent = parent as EnemyBrain;

        // We will recalculate the navMesh path every .2 seconds
        RepathTimer = new CountdownTimer(.2f);
        RepathTimer.OnTimerStop += () => ShouldRepath = true;
    }

    public void OnEnter()
    {
        ShouldRepath = true;

        // start the timer
        RepathTimer.Reset();
        RepathTimer.Start();

#if UNITY_EDITOR
        if (EnemyBody.isDebugging) Debug.Log($"EStateChase: {Parent.GetParentTransform().gameObject} Chase");
#endif
    }

    public void OnExit()
    {
        RepathTimer.Pause();

#if UNITY_EDITOR
        if (EnemyBody.isDebugging) Debug.Log($"EStateChase: {Parent.GetParentTransform().gameObject} Leave Chase");
#endif
    }

    public void Tick()
    {
        // Updates the timer, then checks to see if we should recalculate the path, and does so if it should
        RepathTimer.Tick(Time.deltaTime);

        if (ShouldRepath)
        {
            EnemyBrain.MoveBody(Parent, Parent.Player.transform.position);
            ShouldRepath = false;
            RepathTimer.Reset();
            RepathTimer.Start();
        }
    }


}
