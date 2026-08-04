// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GoapAgent.h"
#include "Navigation/PathFollowingComponent.h"
#include "Utility Scripts/Timers.h"

/**
 * This functions as an interface and will never actually have its own calls, rather other classes will expand off of it and add functionality
 */
class ENEMYAIWORKSPACE_API IActionStrategies
{
public:
	/// <summary>
	/// Default constructor
	/// </summary>
	IActionStrategies();
	virtual ~IActionStrategies();
	
protected:
	/// <summary>
	/// if the Strategy can be performed
	/// </summary>
	bool CanPerform;
	/// <summary>
	/// if the strategy is done
	/// </summary>
	bool Complete;

public:
	/// <summary>
	/// Code to run at the start of the strategy
	/// </summary>
	virtual void Start() {};

	/// <summary>
	/// Code to call every tick
	/// </summary>
	/// <param name="DeltaTime">The amount of time that passed between ticks</param>
	virtual void Update(float DeltaTime){};
	/// <summary>
	/// Code to run at the end of the strategy
	/// </summary>
	virtual void Stop(){};
	
	/// A series of basic getters and setters, added here so child classes can have specific functionality in when an action can be performed or is finished

	virtual bool GetComplete() {return Complete;}
	virtual void SetComplete(bool Value) {Complete = Value;}
	virtual bool GetCanPerform() {return CanPerform;}
	virtual void SetCanPerform(bool Value) {CanPerform = Value;}
};

