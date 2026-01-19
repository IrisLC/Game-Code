
#pragma once

#include "CoreMinimal.h"
#include "UObject/Object.h"
#include "State.h"
#include "StateMachine.generated.h"

/// <summary>
/// An implmentation of a Finite State Machine based off of Jason Weimann's implementation adapted from C# to C++ by Iris Currie
/// </summary>
UCLASS()
class ENEMYAIWORKSPACE_API UStateMachine : public UObject
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UStateMachine();


private:
	/// <summary>
	/// A nested class for the transition objects that determine how and when states are moved between
	/// </summary>
	class Transition {
	public:
		TFunction<bool()> Condition;

		TScriptInterface<IState>* To;

		Transition(TScriptInterface<IState>* to, TFunction<bool()> condition) {
			To = to;
			Condition = condition;
		}

	};

	/// <summary>
	/// The state actively being run
	/// </summary>
	TScriptInterface<IState>* currentState;

	/// <summary>
	/// a map of Arrays containing different transitions based on the Class they belong to .
	/// </summary>
	TMap<UClass*, TArray<Transition>> transitions;

	/// <summary>
	/// An array containing the transitions the can specifically occur from the active state.
	/// </summary>
	TArray<Transition>* currentTransitions;
	/// <summary>
	/// An array containing transitions that can occur from any state.
	/// </summary>
	TArray<Transition> anyTransitions;

	/// <summary>
	/// Checks if any valid transitions can occur and returns the first valid one found.
	/// 
	/// Note: an AnyTransition will always take precedence over a transition specifically from the active state.
	/// </summary>
	/// <returns></returns>
	Transition* GetTransition();
public:	

	/// <summary>
	/// Functions as the tick/update method for the various states
	/// </summary>
	void SMTick();

	/// <summary>
	/// Sets a new state and does so in the following order:
	/// 1. Runs the exit code for the current state
	/// 2. Sets the new state
	/// 3. Adds the TArray of transitions that apply to the new state to the currentState variable
	/// 4. Runs the enter code for the new state
	/// </summary>
	/// <param name="state">The state that will be transitioned to. Takes a pointer to A UObject that inherits from the IState interface. </param>
	void SetState(TScriptInterface<IState>* state);

	/// <summary>
	/// Adds a transition clause between two defined states.
	/// 
	/// The states take pointers to A UObject that inherits from the IState interface.
	/// </summary>
	/// <param name="from">The state that will be transitioned out of.</param>
	/// <param name="to">The state that will be transitioned to.</param>
	/// <param name="predicate">A Tfunction that returns a boolean, this will be what is checked to decide if the transition can occur.</param>
	void AddTransition(TScriptInterface<IState>* from, TScriptInterface<IState>* to, TFunction<bool()> predicate);

	/// <summary>
	/// Adds a transition that can be done from any state.
	/// </summary>
	/// <param name="state">The state that will be transitioned to. Takes a pointer to A UObject that inherits from the IState interface.</param>
	/// <param name="predicate">A Tfunction that returns a boolean, this will be what is checked to decide if the transition can occur.</param>
	void AddAnyTransition(TScriptInterface<IState>* state, TFunction<bool()> predicate);


};




