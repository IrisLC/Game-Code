// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/GOAP/Actions.h"

FAgentAction::FAgentAction(const FName& name) : Name(name), Cost(1), VariableCost(nullptr)
{
}

FAgentAction::FAgentAction() : Name(FName("")), Cost(1), VariableCost(nullptr)
{
}

float FAgentAction::GetCost() const {
	if (!VariableCost) return Cost;

	return (*VariableCost)();
}

void FAgentAction::Tick(float DeltaTime) const
{
	//Check if the action can be performed and update the strategy
	if (Strategy->GetCanPerform())
	{
		Strategy->Update(DeltaTime);
	}

	//Bail out if the strategy is still executing
	if (!Strategy->GetComplete()) return;

	//TODO: Remove if possible. 
	/* //Apply the effects
	for (auto effect : Effects)
	{
		effect->Evaluate();
	}
	*/
}

FAgentAction::~FAgentAction()
{
	Strategy.Reset();
	VariableCost.Reset();
}


FAgentAction::Builder::Builder(const FName& name) : Action(new FAgentAction(name))
{
}

FAgentAction::Builder& FAgentAction::Builder::WithCost(float Cost)
{
	Action->Cost = Cost;
	return *this;
}

FAgentAction::Builder& FAgentAction::Builder::WithCost( TFunction<float()>& VariableCost)
{
	Action->VariableCost = MakeShareable<TFunction<float()>>(&VariableCost);
	return *this;
}

FAgentAction::Builder& FAgentAction::Builder::WithStrategy(IActionStrategies& Strategy)
{
	Action->Strategy = MakeShareable<IActionStrategies>(&Strategy);
	return *this;
}

FAgentAction::Builder& FAgentAction::Builder::AddPrecondition(FWorldState& Precondition)
{
	Action->Preconditions.Add(MakeShareable<FWorldState>(&Precondition));
	return *this;
}

FAgentAction::Builder& FAgentAction::Builder::AddPrecondition(TSet<TSharedPtr<FWorldState>>& Preconditions)
{
	for (auto& Precondition : Preconditions)
	{
		AddPrecondition(*Precondition);
	}
	return *this;
}

FAgentAction::Builder& FAgentAction::Builder::AddEffect(FWorldState& Effect)
{
	Action->Effects.Add(MakeShareable<FWorldState>(&Effect));
	return *this;
}

FAgentAction* FAgentAction::Builder::Build() const
{
	return Action;
}
