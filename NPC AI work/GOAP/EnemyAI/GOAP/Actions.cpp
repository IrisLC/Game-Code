// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/GOAP/Actions.h"

AgentAction::AgentAction(FName name)
{
	Name = name;
}

float AgentAction::GetCost() const {
	if (VariableCost == nullptr) return Cost;

	return (*VariableCost)();
}

void AgentAction::Update(float DeltaTime)
{
	//Check if the action can be performed and update the strategy
	if (Strategy->GetCanPerform())
	{
		Strategy->Update(DeltaTime);
	}

	//Bail out if the strategy is still executing
	if (!Strategy->GetComplete()) return;

	//Apply the effects
	for (auto effect : Effects)
	{
		effect->Evaluate();
	}
}


AgentAction::Builder::Builder(FName name)
{
	Action = new AgentAction(name);
	Action->Cost = 1;
}

AgentAction::Builder& AgentAction::Builder::WithCost(const float* Cost)
{
	Action->Cost = *Cost;
	return *this;
}

AgentAction::Builder& AgentAction::Builder::WithCost(const TFunction<float()>& VariableCost)
{
	Action->VariableCost = VariableCost();
	return *this;
}

AgentAction::Builder& AgentAction::Builder::WithStrategy(IActionStrategies* Strategy)
{
	Action->Strategy = Strategy;
	return *this;
}

AgentAction::Builder& AgentAction::Builder::AddPrecondition(WorldState* Precondition)
{
	Action->Preconditions.Add(Precondition);
	return *this;
}

AgentAction::Builder& AgentAction::Builder::AddEffect(WorldState* Effect)
{
	Action->Effects.Add(Effect);
	return *this;
}

AgentAction* AgentAction::Builder::Build() const
{
	return Action;
}


AgentAction::AgentAction()
{
	Name = FName("");
}

AgentAction::~AgentAction()
{
}
