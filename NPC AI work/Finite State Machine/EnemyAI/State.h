
#pragma once

#include "CoreMinimal.h"
#include "UObject/Interface.h"
#include "State.generated.h"

// This class does not need to be modified.
UINTERFACE(MinimalAPI)
class UState : public UInterface
{
	GENERATED_BODY()
};

/// <summary>
/// The interface for states used in the custom FSM based off of Jason Weimann's implementation adapted from C# to C++ by Iris Currie.
/// 
/// Note: this is a true interface and as such does not need a .cpp file
/// </summary>
class ENEMYAIWORKSPACE_API IState
{
	GENERATED_BODY()

	// Add interface functions to this class. This is the class that will be inherited to implement this interface.
public:
	/// <summary>
	/// The code to run every tick the state is active.
	/// </summary>
	virtual void SMTick() = 0;
	/// <summary>
	/// The code to run when the state is entered into.
	/// </summary>
	virtual void OnEnter() = 0;
	/// <summary>
	/// The code to run when the state is left.
	/// </summary>
	virtual void OnExit() = 0;

};
