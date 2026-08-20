using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


public class StateMachine
{

    /// <summary>
    /// The state actively being run
    /// </summary>
    IState currentState;

    /// <summary>
    /// Dictionary of all states handled by the machine sorted by their type
    /// </summary>
    Dictionary<Type, IState> States = new Dictionary<Type, IState>();

    IStateMachineUser Parent;

    /// <summary>
    /// a map of lists containing different transitions based on the type they belong to 
    /// </summary>
    Dictionary<Type, List<StateTransition>> transitions = new Dictionary<Type, List<StateTransition>>();

    /// <summary>
    /// The transitions that are usable from the current state
    /// </summary>
    List<StateTransition> currentTransitions;

    /// <summary>
    /// Transitions without a from state, this is never switched out
    /// </summary>
    List<StateTransition> anyTransitions = new List<StateTransition>();
    /// <summary>
    /// An empty list for use when the current state has no from transitions
    /// </summary>
    static List<StateTransition> noTransitions = new List<StateTransition> { Capacity = 0 };

    /// <summary>
    /// Constructor, creates a full state machine around a series of transitions
    /// </summary>
    /// <param name="parent">The owner of the state machine</param>
    /// <param name="transitionList">The list of transitions to make up the state machine</param>
    public StateMachine(IStateMachineUser parent, List<StateTransition> transitionList)
    {
        Parent = parent;
        // A dictionary of instantiated TransitionClauses sorted by Type. 
        // Used for having one instance of each transition clause per Parent
        Dictionary<Type, ITransitionClause> transitionClauses = new();

        foreach (StateTransition transition in transitionList)
        {
            transition.Parent = Parent;

            if (!States.ContainsKey(transition.ToType))
            {
                transition.ToState.OnInitialize(Parent);
                States.Add(transition.ToType, transition.ToState);
            }

            //Some transitions will not have a from state, those get added to the anyTransition list
            if (transition.FromState == null)
            {
                anyTransitions.Add(transition);
            }
            else
            {
                if (!States.ContainsKey(transition.FromType))
                {
                    transition.FromState.OnInitialize(Parent);
                    States.Add(transition.FromType, transition.FromState);
                }

                //Ensure the dictionary has the state and then add the transition to its list
                transitions.TryAdd(transition.FromType, new List<StateTransition>());
                transitions[transition.FromType].Add(transition);
            }

            if (!transitionClauses.ContainsKey(transition.PredicateType))
            {
                transitionClauses.Add(transition.PredicateType, transition.Predicate);
            }
            else
            {
                transition.Predicate = transitionClauses[transition.PredicateType];
            }
        }
        //Sets the starting state to the initial state
        States.TryGetValue(Parent.GetInitialState().GetType(), out IState initial);
        Assert.IsNotNull(initial, "Initial State does not exist in given transitions");
        SetState(initial);

    }

    /// <summary>
    /// Checks for possible transitions and then calls Tick() on the current state
    /// </summary>
    public void Tick()
    {
        StateTransition transition = GetTransition();

        if (transition != null)
        {
            SetState(States[transition.ToType]);
        }

        currentState?.Tick();
    }

    public void SetState(IState state)
    {
        if (state == currentState)
        {
            return;
        }

        currentState?.OnExit();
        currentState = state;

        // Get the list of transitions for the current state and add it to the current transition
        transitions.TryGetValue(currentState.GetType(), out currentTransitions);
        if (currentTransitions == null)
        {
            // If there was nothing set it to an empty list
            currentTransitions = noTransitions;
        }

        currentState.OnEnter();

    }

    StateTransition GetTransition()
    {
        //Any transitions will take priority over state to state transitions
        foreach (StateTransition transition in anyTransitions)
        {
            if (transition.Evaluate())
            {
                return transition;
            }
        }

        // If there are no valid anyTransitions then we will get here
        foreach (StateTransition transition in currentTransitions)
        {
            if (transition.Evaluate())
            {
                return transition;
            }
        }

        return null;
    }
}