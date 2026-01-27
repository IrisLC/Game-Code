// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/GOAP/WorldStates.h"

#include "GoapAgent.h"

WorldState::WorldState(FName name)
{
	Name = name;
}

WorldState::~WorldState()
{
}

bool WorldState::Evaluate()
{
	return condition();
}

WorldState::Builder::Builder(FName name)
{
	State = new WorldState(name);
}

WorldState::Builder& WorldState::Builder::WithCondition(const TFunction<bool()>& condition)
{
	State->condition = condition;
	return *this;
}

WorldState::Builder& WorldState::Builder::WithLocation(const TFunction<FVector()> observedLocation)
{
	State->observedLocation = observedLocation;
	return *this;
}

WorldState* WorldState::Builder::Build()
{
	return State;
}


WSFactory::WSFactory(AGoapAgent* agent, TMap<FName, WorldState>* worldStates) 
{
	Agent = agent;
	WorldStates = worldStates;
}

void WSFactory::AddWorldState(const FName Key, const TFunction<bool()>& Condition)
{
	WorldState* ws = (new WorldState::Builder(Key))
	                 ->WithCondition(Condition)
	                 .Build();

	
	WorldStates->Emplace(Key, *ws);
	
}

void WSFactory::AddSensorWorldState(const FName Key, const USensor* sensor)
{
	TFunction<bool()> con = [&] { return sensor->IsTargetInRange; };
	TFunction<FVector()> loc = [&] { return sensor->TargetPosition; };

	WorldState* ws = (new WorldState::Builder(Key))
	                 ->WithCondition(con)
	                 .WithLocation(loc)
	                 .Build();

	WorldStates->Emplace(Key, *ws);
}

void WSFactory::AddLocationWorldState(const FName Key, float Distance, FTransform* LocationCondition)
{
	//Converts the TVector returned from GetLocation into an FVector
	FVector loc = FVector::ZeroVector + LocationCondition->GetLocation();
	AddLocationWorldState(Key, Distance, &loc);
}

void WSFactory::AddLocationWorldState(const FName Key, float Distance, FVector* LocationCondition)
{
	TFunction<FVector()> locationCon = [&] { return *LocationCondition; };
	TFunction<bool()> condition = [&] { return InRangeOf(LocationCondition, Distance); };

	WorldState* ws = (new WorldState::Builder(Key))
	                 ->WithCondition(condition)
	                 .WithLocation(locationCon)
	                 .Build();

	WorldStates->Emplace(Key, *ws);
}

bool WSFactory::InRangeOf(FVector* pos, float range)
{
	return FVector::Dist(Agent->GetPawn()->GetActorLocation(), *pos) < range;
}
