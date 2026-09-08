// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/GOAP/WorldStates.h"

#include "GoapAgent.h"

FWorldState::FWorldState(const FName& name) : Name(name) {}

FWorldState::~FWorldState()
{
	Condition.Reset();
	ObservedLocation.Reset();
}

bool FWorldState::Evaluate() const
{
	return (*Condition)();
}

FWorldState::Builder::Builder(const FName& name) : State(new FWorldState(name))
{
}

FWorldState::Builder& FWorldState::Builder::WithCondition(const TFunction<bool()>& condition)
{
	State->Condition = MakeShared<TFunction<bool()>>(condition);
	return *this;
}

FWorldState::Builder& FWorldState::Builder::WithLocation(const TFunction<FVector()>& observedLocation)
{
	State->ObservedLocation = MakeShared<TFunction<FVector()>>(observedLocation);
	return *this;
}

FWorldState* FWorldState::Builder::Build() const
{
	return State;
}


FWSFactory::FWSFactory(AGoapAgent& agent, TMap<FName, FWorldState*>& worldStates) : Agent(agent), WorldStates(worldStates)
{
}

void FWSFactory::AddWorldState(const FName& Key, const TFunction<bool()>& Condition) const
{
	FWorldState* ws = FWorldState::Builder(Key)
	                 .WithCondition(Condition)
	                 .Build();

	
	WorldStates.Emplace(Key, ws);
	
}

void FWSFactory::AddSensorWorldState(const FName& Key, const USensor& sensor) const
{
	const TFunction<bool()> condition = [&] { return sensor.IsTargetInRange; };
	TFunction<FVector()> location = [&] { return sensor.TargetPosition; };

	FWorldState* ws = FWorldState::Builder(Key)
	                 .WithCondition(condition)
	                 .WithLocation(location)
	                 .Build();

	WorldStates.Emplace(Key, ws);
}

void FWSFactory::AddLocationWorldState(const FName& Key, float Distance, const FTransform& LocationCondition) const
{
	AddLocationWorldState(Key, Distance, LocationCondition.GetLocation());
}

void FWSFactory::AddLocationWorldState(const FName& Key, float Distance, const FVector& LocationCondition) const
{
	TFunction<FVector()> locationCon = [&] { return LocationCondition; };
	TFunction<bool()> condition = [&] { return InRangeOf(LocationCondition, Distance); };

	FWorldState* ws = FWorldState::Builder(Key)
	                 .WithCondition(condition)
	                 .WithLocation(locationCon)
	                 .Build();

	WorldStates.Emplace(Key, ws);
}

bool FWSFactory::InRangeOf(const FVector& pos, float range) const
{
	return FVector::Dist(Agent.GetPawn()->GetActorLocation(), pos) < range;
}
