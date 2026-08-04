// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/GOAP/Goals.h"

AgentGoal::AgentGoal(FName name)
{
	Name = name;
}

AgentGoal::~AgentGoal()
{
}

bool AgentGoal::IsValid()
{
	if (ValidationConditions.IsEmpty()) return true;
	
	for (const TFunction<bool()>* func : ValidationConditions)
	{
		TFunction<bool()> Func = *func;
		if (!Func.IsSet()) continue;
		
		if (!Func())
		{
			return false;
		}
	}
	
	return true;
}

AgentGoal::Builder::Builder(FName name)
{
	Goal = new AgentGoal(name);
}

AgentGoal::Builder& AgentGoal::Builder::WithPriority(float priority)
{
	Goal->Priority = priority;
	return *this;
}

AgentGoal::Builder& AgentGoal::Builder::WithDesiredEffect(WorldState* Effect)
{
	Goal->DesiredEffects.Emplace(Effect);
	return *this;
}

AgentGoal::Builder& AgentGoal::Builder::WithValidationCondition(TFunction<bool()>* Condition)
{
	Goal->ValidationConditions.Emplace(Condition);
	return *this;
}

AgentGoal* AgentGoal::Builder::Build()
{
	return Goal;
}
