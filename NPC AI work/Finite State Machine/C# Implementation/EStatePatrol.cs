using System;
using UnityEngine;
using UnityEngine.AI;

public class EStatePatrol : IState
{
    Enemy_Basic parent;

    public EStatePatrol(Enemy_Basic parent)
    {
        this.parent = parent;
    }
    public void OnEnter()
    {
        Debug.Log("Patrol Entered");
    }

    public void OnExit()
    {
        Debug.Log("Patrol Exited");
    }

    public void Tick()
    {
        Debug.Log("Patrol Ticked");

    }

}