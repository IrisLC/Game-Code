// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "AIController.h"
#include "Goals.h"
#include "GoapAgent.generated.h"

class ActionPlan;
class GoapPlanner;
class AgentAction;
/**
 * The AIController that runs the GOAP framework
 */
UCLASS()
class ENEMYAIWORKSPACE_API AGoapAgent : public AAIController
{
	GENERATED_BODY()

public:
	AGoapAgent();
	
	/*
		Some implementations may have variables for locations or sensors to be used in various WorldStates here
	*/
	
	/// <summary>
	/// The navigation system used to move around the world
	/// </summary>
	UPROPERTY()
	class UNavigationSystemV1* NavSys;
	/// <summary>
	/// The PathFollowingComponent of this Controller
	/// </summary>
	UPROPERTY()
	UPathFollowingComponent* PathFollowingComp;
	
	/// <summary>
	/// A timer that handles when to check if there is a more optimal plan that the agent could be following
	/// </summary>
	CountdownTimer* CheckTimer;
	bool TimerStopped;
	
	/// <summary>
	/// The current goal the agent is trying to complete
	/// </summary>
	AgentGoal* CurrentGoal;
	/// <summary>
	/// The action that is actively being carried out
	/// </summary>
	AgentAction* CurrentAction;
	/// <summary>
	/// The plan to get to the currentGoal
	/// </summary>
	ActionPlan* actionPlan;

	/// <summary>
	/// A map of all WorldStates this Agent is aware of
	/// </summary>
	TMap<FName, WorldState> WorldStates;
	/// <summary>
	/// A set of all Actions this Agent is able to take
	/// </summary>
	TSet<AgentAction*> Actions;
	/// <summary>
	/// A set of all goals this agent is trying to carry out
	/// </summary>
	TSet<AgentGoal*> Goals;
	
	
	/// <summary>
	/// The planner being used
	/// </summary>
	GoapPlanner* Planner;
	
	virtual void BeginPlay() override;
	
	virtual void Tick(float DeltaSeconds) override;	 
	
private:
	
	/// <summary>
	/// The last successfully carried out goal
	/// </summary>
	AgentGoal* LastGoal;
	
	/// <summary>
	/// Creates and activates any timers used by the agent
	/// </summary>
	void SetupTimers();
	
	/// <summary>
	/// Creates all WorldState variables
	/// </summary>
	void SetupWorldStates();
	/// <summary>
	/// Creates all Action variables
	/// </summary>
	void SetupActions();
	/// <summary>
	/// Creates all goal variables
	/// </summary>
	void SetupGoals();
	
	/// <summary>
	/// finds the best plan for the agent to carry out
	/// </summary>
	void CalculatePlan();
	
	/// <summary>
	/// Called when a sensor's target changes
	/// </summary>
	UFUNCTION()
	void HandleSensorChange();
	/// <summary>
	/// Checks if the Agent is in range of the target position
	/// </summary>
	/// <param name="pos">the position of the target</param>
	/// <param name="range">The minimum distance the Agent can be from the location for the evaluation to be false</param>
	/// <returns>true iff the Agent is in range</returns>
	bool InRangeOf(FVector* pos, float range);
};
