// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Actions.h"
#include "Goals.h"

class FNode : public TSharedFromThis<FNode>
{
public:
	TSharedPtr<FNode> Parent;
	TSharedPtr<FAgentAction> Action;
	/// <summary>
	/// The World States that need to be made true for the plan to be valid
	/// </summary>
	TSet<TSharedPtr<FWorldState>> ProblemStates;
	/// <summary>
	/// The cost of moving to this node (Same variable as cost in other parts of the framework) 
	/// </summary>
	float GCost;
	/// <summary>
	/// The Heuristic, this represents the number of problems for this action
	/// </summary>
	int32 HCost;
	/// <summary>
	/// The combined cost of GCost and HCost
	/// </summary>
	float FCost;
	
	/// <summary>
	/// Constructor for the goal node
	/// </summary>
	/// <param name="GoalDesiredEffects">The World States that the goal is trying to solve</param>
	FNode(const TSet<TSharedPtr<FWorldState>>& GoalDesiredEffects);

	/// <summary>
	/// Constructor for action nodes
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="action"></param>
	FNode(TSharedPtr<FNode> parent, TSharedPtr<FAgentAction> action);

	~FNode();
};

class FActionPlan : public TSharedFromThis<FActionPlan>
{
public:
	FAgentGoal* agentGoal;
	TArray<TSharedPtr<FAgentAction>> ActionQueue;
	float TotalCost;

	FActionPlan(FAgentGoal& Goal, TQueue<TSharedPtr<FAgentAction>>* Actions, float GCost);
	~FActionPlan();
};

/**
 * 
 */
class ENEMYAIWORKSPACE_API FGoapPlanner
{
public:
	FGoapPlanner();
	~FGoapPlanner();
	
	static inline int32 DebugNodeCount = 0;

	static TSharedPtr<FActionPlan> Plan(TSet<TSharedPtr<FAgentAction>>& Actions, TSet<TSharedPtr<FAgentGoal>>& Goals, FAgentGoal* MostRecentGoal);
	static TSharedPtr<FNode> FindPath(FNode& GoalNode, TSet<TSharedPtr<FAgentAction>>& Actions);

private:
	static inline TArray<TSharedPtr<FNode>> OpenList{};
	static inline TSharedPtr<FNode> CheapestNodeFound = nullptr;

	static void FindHelpfulActions(FNode& Parent, TSet<TSharedPtr<FAgentAction>>& Actions);
	
	static void CleanUpNodes();
};
