// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Actions.h"
#include "Goals.h"

/// <summary>
/// A node class that is used by the planner to sort actions
/// </summary>
class Node
{
public:
	Node* Parent;
	AgentAction* Action;
	/// <summary>
	/// The World States that need to be made true for the plan to be valid
	/// </summary>
	TSet<WorldState*> ProblemStates;
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
	Node(TSet<WorldState*> GoalDesiredEffects);

	/// <summary>
	/// Constructor for action nodes
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="action"></param>
	/// <param name="gCost"></param>
	Node(Node* parent, AgentAction* action, float gCost);

	~Node();
};

/// <summary>
/// The plan that the planner creates and passes on to the Agent
/// </summary>
class ActionPlan
{
public:
	/// <summary>
	/// The goal the plan seeks to carry out.
	/// </summary>
	AgentGoal* agentGoal;
	/// <summary>
	/// A queue of all actions that will be taken in order to complete the goal
	/// </summary>
	TQueue<AgentAction*>* ActionQueue;
	/// <summary>
	/// The combined Cost of every action in the queue
	/// 
	/// *note this is not currently used in this implementation but could have use in more detailed implementations
	/// </summary>
	float TotalCost;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="Goal">The goal for the plan</param>
	/// <param name="Actions">The queue of all actions the plan requires</param>
	/// <param name="GCost">The GCost of the first node in the queue</param>
	ActionPlan(AgentGoal* Goal, TQueue<AgentAction*>* Actions, float GCost);
	~ActionPlan();
};

/**
 * 
 */
class ENEMYAIWORKSPACE_API GoapPlanner
{
public:
	/// <summary>
	/// Default constructor
	/// </summary>
	GoapPlanner();
	~GoapPlanner();

	/// <summary>
	/// Sorts the list of valid Goals by their priority and moves through each goal, checking if they 
	/// 1. Do not already have their desired effects met.
	/// 2. Have a series of actions that could be done to reach this goal.
	/// 
	/// If a valid goal is found it will then construct an ActionPlan out of the goal and the actions to meet it.
	/// </summary>
	/// <param name="Actions">The list of all actions the agent can carry out</param>
	/// <param name="Goals">The list of all goals the agent is trying to achieve</param>
	/// <param name="MostRecentGoal">the most recent goal that was successfully accomplished
	/// this will have its priority temporarily lowered so that two goals of the same prioirty can be alternated between</param>
	/// <returns>The ActionPlan created to fulfill a goal. Will return a nullptr if no goal is valid</returns>
	ActionPlan* Plan(TSet<AgentAction*>* Actions, TSet<AgentGoal*> Goals, AgentGoal* MostRecentGoal);

	/// <summary>
	/// uses a modified A* algorithm to sort through all actions trying to find a path from the current world state to the desired world state of the goal
	/// </summary>
	/// <param name="Parent">The goal that is being checked</param>
	/// <param name="Actions">All actions that should be checked</param>
	/// <returns>A node holding the first Action that must be taken in the path, the entire path can be found by following the node's parents</returns>
	Node* FindPath(Node& Parent, TSet<AgentAction*>* Actions);

	/// <summary>
	/// Ensures that with the current world state the list of actions to carry out will have all their 
	/// preconditions met and will get the desired effects of the goal.
	/// </summary>
	/// <param name="CurrentPlan">The Plan that is currently being carried out.</param>
	/// <param name="NextAction">The Action that will be taken if the plan is still valid.</param>
	/// <returns></returns>
	bool ValidateCurrentPlan(const ActionPlan& CurrentPlan, OUT AgentAction& NextAction);

private:
	/// <summary>
	/// The array of Nodes that need to be checked in the A* algorithm.
	/// </summary>
	TArray<Node*> OpenList;

	/// <summary>
	/// Adds all actions that can have an effect on achieving the desired goal to the open list
	/// </summary>
	/// <param name="Parent">The node that will be marked as the parent of the nodes being added for pathfinding purposes</param>
	/// <param name="Actions">All actions that will be checked</param>
	void PopulateOpenList(Node* Parent, TSet<AgentAction*>* Actions);
};
