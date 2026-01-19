
#include "EnemyAI/StateMachine.h"


// Sets default values for this component's properties
UStateMachine::UStateMachine()
{
}

/// <summary>
/// Functions as the tick/update method for the various states
/// </summary>
void UStateMachine::SMTick() {
	Transition* transition = GetTransition();

	if (transition != nullptr) {
		SetState(transition->To);
	}

	if (currentState != nullptr) {
		currentState->GetInterface()->SMTick();
	}
}

/// <summary>
/// Sets a new state and does so in the following order:
/// 1. Runs the exit code for the current state
/// 2. Sets the new state
/// 3. Adds the TArray of transitions that apply to the new state to the currentState variable
/// 4. Runs the enter code for the new state
/// </summary>
/// <param name="state">The state that will be transitioned to. Takes a pointer to A UObject that inherits from the IState interface. </param>
void UStateMachine::SetState(TScriptInterface<IState>* state)
{
	if (state == currentState) {
		return;
	}

	if (currentState != nullptr) {
		currentState->GetInterface()->OnExit();
	}

	currentState = state;

	//Get the list of transitions for the current state and add it to the current transition
	currentTransitions = transitions.Find(currentState->GetObject()->GetClass());

	currentState->GetInterface()->OnEnter();

}

/// <summary>
/// Adds a transition clause between two defined states.
/// 
/// The states take pointers to A UObject that inherits from the IState interface.
/// </summary>
/// <param name="from">The state that will be transitioned out of.</param>
/// <param name="to">The state that will be transitioned to.</param>
/// <param name="predicate">A Tfunction that returns a boolean, this will be what is checked to decide if the transition can occur.</param>
void UStateMachine::AddTransition(TScriptInterface<IState>* from, TScriptInterface<IState>* to, TFunction<bool()> predicate)
{
	UClass* fromClass = from->GetObject()->GetClass();

	if (!transitions.Contains(fromClass)) {
		transitions.Emplace(fromClass);
	}

	TArray<UStateMachine::Transition>& currList = transitions[fromClass];


	// Create a transition and add it to the array

	currList.Add(UStateMachine::Transition(to, predicate));

}

/// <summary>
/// Adds a transition that can be done from any state.
/// </summary>
/// <param name="state">The state that will be transitioned to. Takes a pointer to A UObject that inherits from the IState interface.</param>
/// <param name="predicate">A Tfunction that returns a boolean, this will be what is checked to decide if the transition can occur.</param>
void UStateMachine::AddAnyTransition(TScriptInterface<IState>* state, TFunction<bool()> predicate)
{
	anyTransitions.Emplace(UStateMachine::Transition(state, predicate));
}

/// <summary>
/// Checks if any valid transitions can occur and returns the first valid one found.
/// 
/// Note: an AnyTransition will always take precedence over a transition between two specified states
/// </summary>
/// <returns></returns>
UStateMachine::Transition* UStateMachine::GetTransition()
{
	for (Transition& transition : anyTransitions) {
		if (transition.Condition()) {
			return &transition;
		}
	}

	//if there are no valid anyTransitions then we will get here

	for (Transition& transition : *currentTransitions) {
		if (transition.Condition()) {
			return &transition;
		}
	}

	return nullptr;
}



