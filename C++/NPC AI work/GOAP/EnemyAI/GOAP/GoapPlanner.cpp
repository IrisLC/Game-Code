// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/GOAP/GoapPlanner.h"


ActionPlan::ActionPlan(AgentGoal* Goal, TQueue<AgentAction*>* Actions, float GCost)
{
	agentGoal = Goal;
	ActionQueue = Actions;
	TotalCost = GCost;
}

ActionPlan::~ActionPlan()
{
}

GoapPlanner::GoapPlanner()
{
}

GoapPlanner::~GoapPlanner()
{
}

ActionPlan* GoapPlanner::Plan(TSet<AgentAction*>* Actions, TSet<AgentGoal*> Goals, AgentGoal* MostRecentGoal)
{
	// An Array of all goals that are currently valid
	TArray<AgentGoal*> ValidGoals = Goals.Array().FilterByPredicate([](AgentGoal* goal){return goal->IsValid();});
	// Sort the goals list by priority highest to lowest.
	// The most recently accomplished goal will have its priority slightly reduced so that if we have multiple goals with 
	// the same priority being the highest priority goals then one will not be constantly repeated
	ValidGoals.Sort([MostRecentGoal](AgentGoal& a, AgentGoal& b)
	{
		float A = a.GetPriority();
		float B = b.GetPriority();
		if (&a == MostRecentGoal) A -= 0.01f;
		if (&b == MostRecentGoal) B -= 0.01f;
		return A > B;
	});

	for (auto& goal : ValidGoals)
	{
		Node* startNode = FindPath(*new Node(goal->DesiredEffects), Actions);

		if (startNode != nullptr)
		{
			//We found a Path!
			TQueue<AgentAction*>* ActionQueue = new TQueue<AgentAction*>();
			Node* Curr = startNode;
			//The Parent will be null when we get to the goal node which we don't need in the Queue
			while (Curr->Parent != nullptr)
			{
				ActionQueue->Enqueue(Curr->Action);
				Curr = Curr->Parent;
			}
			return (new ActionPlan(goal, ActionQueue, startNode->GCost));
		}
	}

	return nullptr;
}

		

Node* GoapPlanner::FindPath(Node& GoalNode, TSet<AgentAction*>* Actions)
{
	OpenList.Empty();
	Node* CheapestNode = &GoalNode;
	OpenList.Emplace(CheapestNode);
	while (OpenList.Num() > 0)
	{
		//Remove the cheapest node from the list so it doesn't get rechecked
		OpenList.Remove(CheapestNode);
		PopulateOpenList(CheapestNode, Actions);
		//Find the node with the cheapest FCost (In the event of a tie the node with the lowest Heuristic will be checked
		if (OpenList.Num() <= 0) break;
		CheapestNode = OpenList[0];
		for (int32 i = 1; i < OpenList.Num(); i++)
		{
			if (CheapestNode->FCost < OpenList[i]->FCost
				|| (CheapestNode->FCost == OpenList[i]->FCost && CheapestNode->HCost < OpenList[i]->HCost))
			{
				CheapestNode = OpenList[i];
			}
		}
		

		//If Heuristic is 0 then we are done
		if (CheapestNode->HCost == 0)
		{
			return CheapestNode;
		}
	}

	return nullptr;
}


bool GoapPlanner::ValidateCurrentPlan(const ActionPlan& CurrentPlan, OUT AgentAction& NextAction)
{
	AgentAction* StartingAction;
	CurrentPlan.ActionQueue->Dequeue(OUT StartingAction);
	AgentAction* CurrentAction = StartingAction;
	
	// The Effects of every action that has already been checked
	TSet<WorldState*>* Effects = new TSet<WorldState*>();
	
	do{
		// Convert the TSet to a TArray so it can be filtered through then filter out any Preconditions that are already 
		// met by the current world state.
		// This leaves us with an array of all World States that are not already satiated by the current state of the 
		// world that need to be true for the action to be valid.
		if (!CurrentAction->Preconditions.IsEmpty())
		{
			TArray<WorldState*> NotCurrentlyFulfilled = CurrentAction->Preconditions.Array()
			.FilterByPredicate([](WorldState* w){return !w->Evaluate();});
		
			for (auto& state : NotCurrentlyFulfilled)
			{
				// If the Precondition is not met by a state that comes before it then the entire plan is invalid
				//if (!Effects->Contains(state)) return false;
			}
		}
		
		//Add all the effects of the current action to the set of effects as for future actions that are checked these 
		//will be assumed to have already occured
		Effects->Append(CurrentAction->Effects);
		
		// Move the current Action to the back of the Queue and get the next one
		CurrentPlan.ActionQueue->Enqueue(CurrentAction);
		CurrentPlan.ActionQueue->Dequeue(OUT CurrentAction);
		
		//This will be true when the queue has been entirely looped through
	} while (CurrentAction != StartingAction);
	
	// If we get to this point then set the current action to be returned to the user as it will have been Dequeued 
	// And return true as the Goal will still work
	NextAction = *CurrentAction;
	return true;
}

void GoapPlanner::PopulateOpenList(Node* Parent, TSet<AgentAction*>* Actions)
{
	TArray<Node*> children;

	//Fills the children array with nodes of Actions that have effects that can resolve the list of ProblemStates
	for (auto& action : *Actions)
	{
		bool helpful = false;
		for (auto& state : action->Effects)
		{
			if (Parent->ProblemStates.Contains(state))
			{
				helpful = true;
				break;
			}
		}
		if (helpful)
		{
			Node* ToAdd = new Node(Parent, action, action->GetCost());
			// Keeps it from occuring where we are checking two plans where there is a single reordering of the same two actions
			// If there is a node in the open list with the same action that has a parent with the same action as this
			if (OpenList.Num() != 0)
			{
				Node* found = *OpenList.FindByPredicate([Parent](Node* n){return n->Action == Parent->Action;});
				
				if (found != nullptr && found->Parent->Action == action)
				{
					if (found->GCost == ToAdd->GCost && found->HCost == ToAdd->HCost)
					{
						return;
					}
				}
			}
			
			
			children.Emplace(ToAdd);
			
		}
	}

	OpenList.Append(children);
}



Node::Node(TSet<WorldState*> GoalDesiredEffects)
{
	Parent = nullptr;
	ProblemStates = GoalDesiredEffects;
}

Node::Node(Node* parent, AgentAction* action, float gCost)
{
	Parent = parent;
	Action = action;
	GCost = action->GetCost() + parent->GCost;

	TSet<WorldState*> ExistingProblems = Parent->ProblemStates;
	//Remove any WorldStates that this action would fulfill
	for (auto effect : action->Effects)
	{
		if (ExistingProblems.Contains(effect))
		{
			ExistingProblems.Remove(effect);
		}
	}

	// Adds every precondition that is not already satisfied to the ProblemStates set
	for (auto& state : Action->Preconditions)
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

Node::~Node()
{
}
