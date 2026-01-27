// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
//#include "GoapAgent.h"
#include "Sensor.h"

class AGoapAgent;
/**
 * 
 */
class ENEMYAIWORKSPACE_API WorldState
{
public:
	WorldState(FName name);
	~WorldState();

	FName Name;


	bool Evaluate();

private:
	TFunction<bool()> condition;
	TFunction<FVector()> observedLocation = [] { return FVector::ZeroVector; };

public:
	FVector Location = observedLocation();

	class Builder
	{
	public:
		__readonly WorldState* State;

		Builder(FName name);

		Builder& WithCondition(const TFunction<bool()>& condition);
		Builder& WithLocation(const TFunction<FVector()> observedLocation);

		WorldState* Build();
	};
};

class WSFactory
{
public:
	__readonly AGoapAgent* Agent;

	__readonly TMap<FName, WorldState>* WorldStates;

	WSFactory(AGoapAgent* agent, TMap<FName, WorldState>* worldStates);

	void AddWorldState(const FName Key, const TFunction<bool()>& Condition);
	void AddSensorWorldState(const FName Key, const USensor* sensor);
	void AddLocationWorldState(const FName Key, float Distance, FTransform* LocationCondition);
	void AddLocationWorldState(const FName Key, float Distance, FVector* LocationCondition);

private:
	bool InRangeOf(FVector* pos, float range);
};
