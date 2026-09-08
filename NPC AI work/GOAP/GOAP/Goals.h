// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "WorldStates.h"

/**
 * The goals the agent will try to carry out
 */
class ENEMYAIWORKSPACE_API FAgentGoal : public TSharedFromThis<FAgentGoal>
{
public:
	/// <summary>
	/// Default constructor
	/// </summary>
	/// <param name="name">The name the Goal will be referred to as</param>
	FAgentGoal(const FName& name);
	~FAgentGoal();

	/// <summary>
	/// The name the Goal will be referred to as.
	/// </summary>
	FName Name;
	/// <summary>
	/// The WorldStates that will be changed when the goal is successfully carried out
	/// </summary>
	TSet<TSharedPtr<FWorldState>> DesiredEffects;
	
	/// <summary>
	/// Checks to see if any validation conditions are keeping the Goal from being checked. If no validation conditions exist will return true.
	/// </summary>
	/// <returns>ture iff the Goal is valid</returns>
	bool IsValid();

private:
	/// <summary>
	/// How much the goal should be prioritized in relation to the other goals.
	/// </summary>
	float Priority;
	
	/// <summary>
	/// Gives the opportunity for some goals to only be able to be checked under certain conditions.
	/// This is not necessary and by default will return true, but adding conditions reduces the number of 
	/// Goals that are checked by the planner.
	/// 
	/// ex. An Attack goal is only valid if the AI knows the target exists
	/// </summary>
	TSet<TSharedPtr<TFunction<bool()>>> ValidationConditions;
public:
	/// <summary>
	/// Returns the priority of the goal in relation to other goals. Higher Priorities will be checked first.
	/// </summary>
	/// <returns>the priority of the goal</returns>
	float GetPriority() const { return Priority; };

	/// <summary>
	/// A class to easily create new Goals.
	/// 
	/// Using this usually takes the form of (new AgentGoal::Builder())->WithPriority().WithDesiredEffect().Build();
	/// All calls in this format should end with Build()
	/// </summary>
	class Builder
	{
	public:
		/// <summary>
		/// The goal being built
		/// </summary>
		FAgentGoal* const Goal;

		/// <summary>
		/// Creates a new Goal.
		/// </summary>
		/// <param name="name">The name to set for the goal</param>
		Builder(const FName& name);

		/// <summary>
		/// Assigns a priority to the goal
		/// </summary>
		/// <param name="priority"> How much the goal should be prioritized in relation to the other goals. Higher Priorities will be checked first.</param>
		/// <returns>A reference to the builder instance.</returns>
		Builder& WithPriority(float priority);
		/// <summary>
		/// Adds a WorldState that the Goal seeks to be true and will be true when the goal is successfully carried out.
		/// </summary>
		/// <param name="Effect">a WorldState that the Goal seeks to be true</param>
		/// <returns>A reference to the builder instance.</returns>
		Builder& WithDesiredEffect(FWorldState& Effect);
		/// <summary>
		/// Gives the opportunity for some goals to only be able to be checked under certain conditions.
		/// This is not necessary and by default will return true, but adding conditions reduces the number of 
		/// Goals that are checked by the planner.
		/// </summary>
		/// <param name="Condition">The Function to check</param>
		/// <returns>A reference to the builder instance.</returns>
		Builder& WithValidationCondition(TFunction<bool()>& Condition);

		/// <summary>
		/// Builds the Goal.
		/// </summary>
		/// <returns>The Goal that has been created</returns>
		FAgentGoal* Build() const;
	};
};
