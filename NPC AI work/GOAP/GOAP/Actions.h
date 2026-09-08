// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "WorldStates.h"
#include "Strategies.h"

/// <summary>
/// Object class for actions the agent can take.
/// </summary>
class ENEMYAIWORKSPACE_API FAgentAction : public TSharedFromThis<FAgentAction>
{
public:
	/// <summary>
	/// Default constructor, makes an action with a name of "" and no other variables assigned. Use this only for creating placeholders.
	/// </summary>
	FAgentAction();
	~FAgentAction();
	
	/// <summary>
	/// The constructor that should be used when making new actions for an agent.
	/// </summary>
	/// <param name="name">The name to set for the action.</param>
	FAgentAction(const FName& name);

	/// <summary>
	/// The name of the action.
	/// </summary>
	const FName Name;

private:
	/// <summary>
	/// The Set of WorldStates that need to be true for the action to be started.
	/// </summary>
	TSet<TSharedPtr<FWorldState>> Preconditions;
	/// <summary>
	/// The WorldStates that will be changed by the completion of the action.
	/// </summary>
	TSet<TSharedPtr<FWorldState>> Effects;
	
	/// <summary>
	/// The cost of doing this action, used in the planner to give more weight to certain actions.
	/// 1 by default.
	/// </summary>
	float Cost;
	/// <summary>
	/// A pointer to a TFunction that allows for the setting of a cost that changes depending on different situations.
	/// </summary>
	TSharedPtr<TFunction<float()>> VariableCost;

	/// <summary>
	/// The strategy the agent will use to carry out the action.
	/// </summary>
	TSharedPtr<IActionStrategies> Strategy;

public:
	/// <summary>
	/// Returns the cost for doing the action.
	/// </summary>
	/// <returns>the cost for doing the action.</returns>
	float GetCost() const;
	
	/// <summary>
	/// Returns if the strategy has finished executing the action.
	/// </summary>
	/// <returns>true iff the Strategy for carrying out the action is finished.</returns>
	bool GetComplete() const { return Strategy->GetComplete(); }
	
	TSet<TSharedPtr<FWorldState>>& GetPreconditions() {return Preconditions;}
	TSet<TSharedPtr<FWorldState>>& GetEffects() {return Effects;}

	/// <summary>
	/// Code to run when the action is entered into.
	/// </summary>
	void Start() const { Strategy->Start(); }
	
	/// <summary>
	/// The code that is run every tick.
	/// </summary>
	/// <param name="DeltaTime">The amount of time between ticks.</param>
	void Tick(float DeltaTime) const;
	
	/// <summary>
	/// The code that is run when the action is done executing.
	/// </summary>
	void Stop() const { Strategy->Stop(); }

	/// <summary>
	/// A class to easily create new Actions.
	/// 
	/// Using this usually takes the form of AgentAction::Builder().WithStrategy().AddEffect)).Build()
	/// All calls in this format should end with Build()
	/// </summary>
	class Builder
	{
	public:
		/// <summary>
		/// The action being built.
		/// </summary>
		FAgentAction* const Action;

		/// <summary>
		/// Creates a new Action.
		/// </summary>
		/// <param name="name">The name to set for the action</param>
		Builder(const FName& name);

		/// <summary>
		/// Gives a fixed cost to the Action being built.
		/// </summary>
		/// <param name="Cost">The cost to assign to the Action.</param>
		/// <returns>A reference to the builder instance.</returns>
		Builder& WithCost(float Cost);
		
		/// <summary>
		/// Gives a variable cost to the action being built, use this if the cost of carrying out an action should be changed depending on the situation
		/// </summary>
		/// <param name="VariableCost">A TFunction that will be evaluated when checking the cost of the action</param>
		/// <returns>A reference to the builder instance.</returns>
		Builder& WithCost(TFunction<float()>& VariableCost);
		
		/// <summary>
		/// Assigns the strategy the agent will use to carry out the action.
		/// </summary>
		/// <param name="Strategy">The strategy to be used</param>
		/// <returns>A reference to the builder instance.</returns>
		Builder& WithStrategy(IActionStrategies& Strategy);
		
		/// <summary>
		/// Adds a WorldState that needs to be true for the action to be started to the set of preconditions.
		/// </summary>
		/// <param name="Precondition">	A WorldState that needs to be true for the action to be started.</param>
		/// <returns>A reference to the builder instance.</returns>
		Builder& AddPrecondition(FWorldState& Precondition);

		/// <summary>
		/// Takes a set of WorldStates that needs to be true for the action to be started
		/// and adds them to the set of preconditions
		/// </summary>
		/// <param name="Preconditions">The TSet of WorldStates that needs to be true for the action to be started.</param>
		/// <returns>A reference to the builder instance.</returns>
		Builder& AddPrecondition(TSet<TSharedPtr<FWorldState>>& Preconditions);
		
		/// <summary>
		/// Adds a WorldState that will be changed when the action finishes.
		/// </summary>
		/// <param name="Effect">A WorldState that will be changed when the action finishes.</param>
		/// <returns>A reference to the builder instance.</returns>
		Builder& AddEffect(FWorldState& Effect);

		FAgentAction* Build() const;

	};
};
