using System;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "StateTransition", menuName = "StateMachine/StateTransition")]
public class StateTransition : ScriptableObject
{
    /// <summary>
    /// The IState being transitioned from, not necessary for all transitions
    /// </summary>
    [SerializeReference] public IState FromState;
    /// <summary>
    /// The IState being transitioned to. Must be non-null.
    /// </summary>
    [SerializeReference] public IState ToState;
    /// <summary>
    /// The Clause that will be evaluated before a transition may occur. Must be non-null.
    /// </summary>
    [SerializeReference] public ITransitionClause Predicate;
    public IStateMachineUser Parent;

    /// <summary>
    /// The type of the from state
    /// </summary>
    public Type FromType { get => FromState.GetType(); }
    /// <summary>
    /// The type of the to state
    /// </summary>
    public Type ToType { get => ToState.GetType(); }
    public Type PredicateType { get => Predicate.GetType(); }

    void Awake()
    {
        Assert.IsNotNull(ToState);
        Assert.IsNotNull(Predicate);

    }

    /// <summary>
    /// Evaluates the condition of the Transition's Predicate clause
    /// </summary>
    /// <returns>true if the clause allows a transition</returns>
    public bool Evaluate()
    {
        return Predicate.Evaluate(Parent);
    }
}