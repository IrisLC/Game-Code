using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    // The state actively being run
    private IState currentState;
    //Dictionaries are basically maps
    // a map of lists containing different transitions based on the type they belong to 
    private Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> currentTransitions = new List<Transition>();
    // Transitions coming from any state, this is never switched out
    private List<Transition> anyTransitions = new List<Transition>();
    // An empty list for quickly creating a new transition list
    private static List<Transition> emptyTransitions = new List<Transition>(capacity:0);


    public void tick() {
        Transition transition = GetTransition();

        if(transition != null) {
            SetState(transition.To);
        }

        currentState?.Tick();
    }

    public void SetState(IState state) {
        if (state == currentState) {
            return;
        }
        
        currentState?.OnExit();
        currentState = state;

        // Get the list of transitions for the current state and add it to the current transition
        transitions.TryGetValue(currentState.GetType(), out currentTransitions);
        if(currentTransitions == null) {
            // If there was nothing make a new list
            currentTransitions = emptyTransitions;
        }
        currentState.OnEnter();
        
    }

    public void AddTransition(IState from, IState to, Func<bool> predicate) {

        if(transitions.TryGetValue(from.GetType(), out var currList) == false) {
            // If there is no current list for the from transition then make a new one
            // then add it to the dictionary
            currList = new List<Transition>();
            transitions.Add(from.GetType(), currList);
        }

        // Create a transition and add it to the list
        currList.Add(new Transition(to, predicate));
    }

    public void AddAnyTransition(IState state, Func<bool> predicate) {
        anyTransitions.Add(new Transition(state, predicate));
    }

    private class Transition {
        public Func<bool> Condition;
        public IState To;

        public Transition(IState to, Func<bool> condition) {
            To = to;
            Condition = condition;
        }
    }

    private Transition GetTransition() {

        foreach(Transition transition in anyTransitions) {
            if(transition.Condition()) {
                return transition;
            }
        }

        // If there are no valid anyTransitions then you will get here
        foreach(Transition transition in currentTransitions) {
            if(transition.Condition()) {
                return transition;
            }
        }

        return null;
    }
}
