// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GoapAgent.h"
#include "Engine/world.h"
#include "Navigation/PathFollowingComponent.h"
#include "Utility Scripts/Timers.h"

/**
 * This functions more as an interface and will never actually have its own calls
 */
class ENEMYAIWORKSPACE_API IActionStrategies : public TSharedFromThis<IActionStrategies>
{
public:
	/// <summary>
	/// Default constructor
	/// </summary>
	IActionStrategies(AGoapAgent& agent);
	virtual ~IActionStrategies();
	
protected:
	AGoapAgent& Agent;
	/// <summary>
	/// if the Strategy can be performed
	/// </summary>
	bool CanPerform;
	/// <summary>
	/// if the strategy is done
	/// </summary>
	bool IsComplete;
	
	bool IsActive;

public:
	/// <summary>
	/// Code to run at the start of the strategy
	/// </summary>
	virtual void Start() {IsActive = true;}

	/// <summary>
	/// Code to call every tick
	/// </summary>
	/// <param name="DeltaTime">The amount of time that passed between ticks</param>
	virtual void Update(float DeltaTime){UE_LOG(LogTemp, Warning, TEXT("default update run"))};

	/// <summary>
	/// Code to run at the end of the strategy
	/// </summary>
	virtual void Stop() {IsActive = false;}
	
	/// A series of basic getters and setters, added here so child classes can have specific functionality for when an action can be performed or is finished
	
	virtual bool GetComplete() {return IsComplete;}
	virtual void SetComplete(bool Value) {IsComplete = Value;}
	virtual bool GetCanPerform() {return CanPerform;}
	virtual void SetCanPerform(bool Value) {CanPerform = Value;}
};

class FIdleStrategy : public IActionStrategies
{
private:
	bool CanPerform = true; // Agent can always idle
	
	float timerLength;
	
public:
	FTimerManager& TimerManager;
	FTimerHandle Timer;
	
	FIdleStrategy(AGoapAgent& agent, float duration);
	virtual ~FIdleStrategy() override;
	
private:
	void OnTimerStop();
	
public:
	
	virtual void Start() override;
	virtual void Update(float DeltaTime) override;
};

class FWanderStrategy : public IActionStrategies
{
public:
	const UPathFollowingComponent* const PathFollowingComp;
	float WanderRadius;
	
private:
	
	FVector RandomLocation;
public:
	FWanderStrategy(AGoapAgent& Agent, float WanderRadius);
	virtual ~FWanderStrategy() override;
	
	virtual void Start() override;
	virtual void Stop() override;
	
	virtual bool GetCanPerform() override {return !GetComplete();}
	
	void ReachedDestination();
};
