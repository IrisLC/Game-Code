// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/GOAP/GoapPlanner.h"


FActionPlan::FActionPlan(FAgentGoal& Goal, TQueue<TSharedPtr<FAgentAction>>* Actions, float GCost) : agentGoal{&Goal}, TotalCost{GCost}
{
	// Convert the TQueue into a TArray and delete the TQueue
	while (!Actions->IsEmpty())
	{
		TSharedPtr<FAgentAction> action;
		Actions->Dequeue(action);
		ActionQueue.Push(action);
	}
	
	delete Actions;
}

FActionPlan::~FActionPlan()
{
	delete agentGoal;
	delete ActionQueue;
}

FGoapPlanner::FGoapPlanner()
{
}

FGoapPlanner::~FGoapPlanner()
{
}

TSharedPtr<FActionPlan> FGoapPlanner::Plan(TSet<TSharedPtr<FAgentAction>>& Actions, TSet<TSharedPtr<FAgentGoal>>& Goals, FAgentGoal* MostRecentGoal)
{
	// An Array of all goals that are currently valid
	TArray<TSharedPtr<FAgentGoal>> ValidGoals = Goals.Array().FilterByPredicate([](const TSharedPtr<FAgentGoal>& goal){return goal->IsValid();});
	UE_LOG(LogTemp, Warning, TEXT("Number of Valid Goals %d"), ValidGoals.Num());
	// Sort the goals list by priority highest to lowest.
	// The most recently accomplished goal will have its priority slightly reduced so that if we have multiple goals with 
	// the same priority being the highest priority goals then one will not be constantly repeated
	ValidGoals.Sort([MostRecentGoal](const TSharedPtr<FAgentGoal>& a, const TSharedPtr<FAgentGoal>& b)
	{
		float A = a->GetPriority();
		float B = b->GetPriority();
		if (&*a == MostRecentGoal) A -= 0.01f;
		if (&*b == MostRecentGoal) B -= 0.01f;
		return A > B;
	});

	for (TSharedPtr goal : ValidGoals)
	{
		UE_LOG(LogTemp, Warning, TEXT("Checking Goal %s"), *goal->Name.ToString());
		FNode goalNode = FNode(goal->DesiredEffects);
		TSharedPtr<FNode> startNode = FindPath(goalNode, Actions);

		if (startNode.IsValid())
		{
			UE_LOG(LogTemp, Warning, TEXT("Path Found"));
			//We found a Path!
			TQueue<TSharedPtr<FAgentAction>>* ActionQueue = new TQueue<TSharedPtr<FAgentAction>>();
			TSharedPtr<FNode> Curr = startNode;
			//The Parent will be null when we get to the goal node which we don't need in the Queue
			while (Curr->Parent.IsValid())
			{
				ActionQueue->Enqueue(Curr->Action);
				Curr = Curr->Parent;
			}
			float Cost = startNode->GCost;
			
			CleanUpNodes();
			
			return MakeShared<FActionPlan>(FActionPlan{*goal, ActionQueue, Cost});
		}
	}

	UE_LOG(LogTemp, Warning, TEXT("No Plans Found"));
	return nullptr;
}

		

TSharedPtr<FNode> FGoapPlanner::FindPath(FNode& GoalNode, TSet<TSharedPtr<FAgentAction>>& Actions)
{
	UE_LOG(LogTemp, Warning, TEXT("Entering A*"));
	// Start with OpenList only including the GoalNode
	OpenList.Empty();
	TSharedPtr<FNode> CheapestNode = MakeShared<FNode>(GoalNode);
	OpenList.Emplace(CheapestNode);
	check(OpenList.Num() == 1, "OpenList has a node besides GoalNode");
	
	while (OpenList.Num() > 0)
	{
		UE_LOG(LogTemp, Warning, TEXT("Start of loop Open List size is %d"), OpenList.Num());
		//Remove the cheapest node from the list so it doesn't get rechecked
		OpenList.Remove(CheapestNode);
		// Add a new set of Nodes to the OpenList
		FindHelpfulActions(*CheapestNode, Actions);
		UE_LOG(LogTemp, Warning, TEXT("Populated Open List size is %d"), OpenList.Num());
		//Find the node with the cheapest FCost (In the event of a tie the node with the lowest Heuristic will be checked
		if (OpenList.Num() == 0) break;
		CheapestNode = OpenList[0];
		for (int32 i = 1; i < OpenList.Num(); i++)
		{
			if (CheapestNode->FCost < OpenList[i]->FCost
				|| (CheapestNode->FCost == OpenList[i]->FCost && CheapestNode->HCost < OpenList[i]->HCost))
			{
				CheapestNode = OpenList[i];
			}
		}
		
		UE_LOG(LogTemp, Warning, TEXT("Cheapest node is %s"), *CheapestNode->Action->Name.ToString());

		//If Heuristic is 0 then we are done
		if (CheapestNode->HCost == 0)
		{
			UE_LOG(LogTemp, Warning, TEXT("H is 0"));
			CheapestNodeFound = CheapestNode;
			return CheapestNode;
		}
	}

	return nullptr;
}


void FGoapPlanner::FindHelpfulActions(FNode& Parent, TSet<TSharedPtr<FAgentAction>>& Actions)
{
	TArray<TSharedPtr<FNode>> children;

	//Fills the children array with nodes of Actions that have effects that can resolve the list of ProblemStates
	for (auto& action : Actions)
	{
		// if the action has effects that will be beneficial to the goal
		bool helpful = false;
		// check all the effects of an action to see if any of the effects are state changes needed for the goal
		for (TSharedPtr state : action->GetEffects())
		{
			if (Parent.ProblemStates.Contains(state))
			{
				helpful = true;
				break;
			}
		}
		
		if (helpful)
		{
			// The node we plan on adding to the children array
			TSharedPtr<FNode> ToAdd = MakeShared<FNode>(FNode(MakeShared<FNode>(Parent), action));
			// Keeps it from occuring where we are checking two plans where there is a single reordering of the same two actions
			// if there is a node in the open list with the same action that has a parent with the same action as this
			if (OpenList.Num() != 0)
			{
				// Finds a FNode that has the same action as the current node's parent
				TSharedPtr<FNode>* found = OpenList.FindByPredicate(
					[&Parent](const TSharedPtr<FNode>& node){return node->Action == Parent.Action;});
				
				// If the found node's parent is the same action as the current node, and the G and H costs match its the same function just reversed order, so don't add the node
				if (found != nullptr && (*found)->Parent->Action == action &&
					(*found)->GCost == ToAdd->GCost && (*found)->HCost == ToAdd->HCost)
				{
					break;
					
				}
			}
			
			children.Emplace(ToAdd);
			
		}
	}

	OpenList.Append(children);
}

void FGoapPlanner::CleanUpNodes()
{
	for (auto Node : OpenList)
	{
		Node.Reset();
	}
}


FNode::FNode(const TSet<TSharedPtr<FWorldState>>& GoalDesiredEffects) : 
	Parent{nullptr}, Action{nullptr}, GCost{0}, HCost{0}, FCost{0}
{
	++FGoapPlanner::DebugNodeCount;
	UE_LOG(LogTemp, Warning, TEXT("Node Created, Nodes Existing: %d"), FGoapPlanner::DebugNodeCount);
	for (auto effect : GoalDesiredEffects)
	{
		ProblemStates.Emplace(effect);
	}
}

FNode::FNode(TSharedPtr<FNode> parent, TSharedPtr<FAgentAction> action) : Parent{parent}, Action{action}
{
	++FGoapPlanner::DebugNodeCount;
	UE_LOG(LogTemp, Warning, TEXT("Node Created, Nodes Existing: %d"), FGoapPlanner::DebugNodeCount);
	GCost = action->GetCost() + parent->GCost;

	TSet<TSharedPtr<FWorldState>> ExistingProblems = Parent->ProblemStates;
	//Remove any WorldStates that this action would fulfill
	for (const auto& effect : action->GetEffects())
	{
		if (ExistingProblems.Contains(effect))
		{
			ExistingProblems.Remove(effect);
		}
	}

	// Adds every precondition that is not already satisfied to the ProblemStates set
	for (const auto& state : Action->GetPreconditions())
	{
		if (!state->Evaluate())
		{
			ProblemStates.Emplace(state);
		}
	}

	ProblemStates.Append(ExistingProblems);

	HCost = ProblemStates.Num();
	FCost = HCost + GCost;
}

FNode::~FNode()
{
	Parent.Reset();
	
	--FGoapPlanner::DebugNodeCount;
	UE_LOG(LogTemp, Warning, TEXT("Node Destroyed, Nodes Existing: %d"), FGoapPlanner::DebugNodeCount);
}


