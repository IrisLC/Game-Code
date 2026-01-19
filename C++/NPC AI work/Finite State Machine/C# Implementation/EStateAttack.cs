using System.Collections;
using UnityEditor;
using UnityEngine;

public class EStateAttack : IState
{
    Enemy_Basic parent;

    public EStateAttack(Enemy_Basic parent)
    {
        this.parent = parent;
    }

    public void OnEnter()
    {
        Debug.Log("Attack Entered");
    }

    public void OnExit()
    {
        Debug.Log("Attack Exited");
    }

    public void Tick()
    {
        Debug.Log("Attack Ticked");
    }
}