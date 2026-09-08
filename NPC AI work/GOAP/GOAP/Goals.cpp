// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/GOAP/Goals.h"

FAgentGoal::FAgentGoal(const FName& name) : Name(name), Priority(1)
{
}

FAgentGoal::~FAgentGoal()
{
}

bool FAgentGoal::IsValid()
{
	if (ValidationConditions.IsEmpty()) return true;
	
	for (const TSharedPtr func : ValidationConditions)
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

FAgentGoal::Builder::Builder(const FName& name) : Goal(new FAgentGoal(name))
{
}

FAgentGoal::Builder& FAgentGoal::Builder::WithPriority(float priority)
{
	Goal->Priority = priority;
	return *this;
}

FAgentGoal::Builder& FAgentGoal::Builder::WithDesiredEffect(FWorldState& Effect)
{
	Goal->DesiredEffects.Emplace(Effect.AsShared());
	return *this;
}

FAgentGoal::Builder& FAgentGoal::Builder::WithValidationCondition(TFunction<bool()>& Condition)
{
	Goal->ValidationConditions.Emplace(MakeShareable<TFunction<bool()>>(&Condition));
	return *this;
}

FAgentGoal* FAgentGoal::Builder::Build() const
{
	return Goal;
}
