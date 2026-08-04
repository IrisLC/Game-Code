// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "WorldStates.h"

/**
 * 
 */
class ENEMYAIWORKSPACE_API AgentGoal
{
public:
	AgentGoal(FName name);
	~AgentGoal();

	FName Name;
	TSet<WorldState*> DesiredEffects;
	
	bool IsValid();

private:
	float Priority;
	
	/// <summary>
	/// Gives the opportunity for some goals to only be able to be checked under certain conditions.
	/// This is not necessary and by default will return true, but adding conditions reduces the number of 
	/// Goals that are checked by the planner.
	/// 
	/// ex. An Attack goal is only valid if the AI knows the target exists
	/// </summary>
	TSet<TFunction<bool()>*> ValidationConditions;
public:
	float GetPriority() const { return Priority; };

	class Builder
	{
	public:
		__readonly AgentGoal* Goal;

		Builder(FName name);

		Builder& WithPriority(float priority);
		Builder& WithDesiredEffect(WorldState* Effect);
		Builder& WithValidationCondition(TFunction<bool()>* Condition);
		AgentGoal* Build();
	};
};
