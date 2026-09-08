// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
//#include "GoapAgent.h"
#include "Sensor.h"

class AGoapAgent;
/**
 * Variables that define aspects about the world, return true when the aspect is a specific way, false otherwise
 */
class ENEMYAIWORKSPACE_API FWorldState : public TSharedFromThis<FWorldState>
{
public:
	/// <summary>
	/// Default constructor
	/// </summary>
	/// <param name="name">the name to refer to the WorldState by</param>
	FWorldState(const FName& name);
	~FWorldState();

	/// <summary>
	/// the name to refer to the WorldState by
	/// </summary>
	 const FName Name;

	/// <summary>
	/// Checks if the WorldState's condition is true.
	/// </summary>
	/// <returns> true iff the game world aligns with the condition</returns>
	bool Evaluate() const;

private:
	/// <summary>
	/// A function that returns a boolean about the state of game world
	/// </summary>
	TSharedPtr<TFunction<bool()>> Condition;
	/// <summary>
	/// A function that returns an FVector. Used to check for specific locations in the game world.
	/// </summary>
	TSharedPtr<TFunction<FVector()>> ObservedLocation 
		= MakeShared<TFunction<FVector()>>([] { return FVector::ZeroVector; });

public:
	/// <summary>
	/// A location in the game world that can be checked for an Agent's proximity
	/// </summary>
	FVector Location = (*ObservedLocation)();

	/// <summary>
	/// A class to create new WorldStates.
	/// </summary>
	class Builder
	{
	public:
		/// <summary>
		/// The WorldState being created
		/// </summary>
		FWorldState* const State;

		/// <summary>
		/// Creates a new State.
		/// </summary>
		/// <param name="name">The name to set for the State</param>
		Builder(const FName& name);

		/// <summary>
		/// Adds a condition that will be checked for to see if it is true by Evaluate()
		/// </summary>
		/// <param name="condition">The function to check</param>
		/// <returns>A reference to the builder class instance.</returns>
		Builder& WithCondition(const TFunction<bool()>& condition);
		/// <summary>
		/// Adds a location that will be checked for proximity by Evaluate()
		/// </summary>
		/// <param name="observedLocation">A function that returns an FVector, the FVector is the location that will be checked</param>
		/// <returns>A reference to the builder class instance.</returns>
		Builder& WithLocation(const TFunction<FVector()>& observedLocation);
		
		/// <summary>
		/// Builds the WorldState.
		/// </summary>
		/// <returns>The WorldState that has been created</returns>
		FWorldState* Build() const;
	};
};

/// <summary>
/// A class to quickly create specific WorldStates and assign them to a TMap
/// </summary>
class FWSFactory
{
public:
	/// <summary>
	/// The AIController for the agent these states are being built for.
	/// </summary>
	const AGoapAgent& Agent;

	/// <summary>
	/// Constructor for the WSFactory class
	/// </summary>
	/// <param name="agent">The AIController handling these WorldStates</param>
	/// <param name="worldStates">A Map of WorldStates and the names that correlate to a state></param>
	FWSFactory(AGoapAgent& agent, TMap<FName, FWorldState*>& worldStates);

	/// <summary>
	/// Creates a WorldState that checks against a specific condition to see if it's true.
	/// </summary>
	/// <param name="Key">The name for the WorldState</param>
	/// <param name="Condition">The function to be checked</param>
	void AddWorldState(const FName& Key, const TFunction<bool()>& Condition) const;
	/// <summary>
	/// Creates a WorldState that relies on the Sensor class to evaluate the condition
	/// </summary>
	/// <param name="Key">The name for the WorldState</param>
	/// <param name="sensor">The sensor that will be used for evaluations</param>
	void AddSensorWorldState(const FName& Key, const USensor& sensor) const;
	/// <summary>
	/// Creates a WorldState that checks the Agent's location against a FVector to see if they are close enough
	/// </summary>
	/// <param name="Key">The name for the WorldState</param>
	/// <param name="Distance">The minimum distance the Agent can be from the location for the evaluation to be false</param>
	/// <param name="LocationCondition">The Transform that holds the Location being checked against</param>
	void AddLocationWorldState(const FName& Key, float Distance, const FTransform& LocationCondition) const;
	/// <summary>
	/// Creates a WorldState that checks the Agent's location against a FVector to see if they are close enough
	/// </summary>
	/// <param name="Key">The name for the WorldState</param>
	/// <param name="Distance">The minimum distance the Agent can be from the location for the evaluation to be false</param>
	/// <param name="LocationCondition">The Location being checked against</param>
	void AddLocationWorldState(const FName& Key, float Distance, const FVector& LocationCondition) const;

private:
	/// <summary>
	/// A TMap that uses the name of the WorldState as the key and holds the WorldState as the value.
	/// </summary>
	TMap<FName, FWorldState*>& WorldStates;
	
	/// <summary>
	/// Checks if the Agent is in range of the target position
	/// </summary>
	/// <param name="pos">the position of the target</param>
	/// <param name="range">The minimum distance the Agent can be from the location for the evaluation to be false</param>
	/// <returns>true iff the Agent is in range</returns>
	bool InRangeOf(const FVector& pos, float range) const;
};
