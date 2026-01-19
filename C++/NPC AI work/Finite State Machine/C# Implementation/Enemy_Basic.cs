using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class Enemy_Basic : MonoBehaviour
{
    private StateMachine Esm;

    [Header("Debug")]
    [SerializeField] private bool test1;
    [SerializeField] private bool test2;

    void Awake()
    {
        Esm = new StateMachine();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SMSetup();
    }

    void SMSetup()
    {
        EStatePatrol patrol = new EStatePatrol(this);
        EStateAttack attack = new EStateAttack(this);

        Esm.AddTransition(patrol, attack, PToA());
        Esm.AddTransition(attack, patrol, AToP());

        Esm.SetState(patrol);

        Func<bool> PToA() => () => test1;
        Func<bool> AToP() => () => test2;
        Func<bool> startAttacking() => () => seesPlayer && awareness >= 100;
    }

}
