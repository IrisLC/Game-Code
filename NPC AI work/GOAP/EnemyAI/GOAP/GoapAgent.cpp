// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/GOAP/GoapAgent.h"
#include "GoapPlanner.h"
#include "Actions.h"
#include "NavigationSystem.h"
#include "Navigation/PathFollowingComponent.h"

AGoapAgent::AGoapAgent()
{
	Planner = new GoapPlanner();
}

void AGoapAgent::BeginPlay()
{
	Super::BeginPlay();
	
	
	NavSys = FNavigationSystem::GetCurrent<UNavigationSystemV1>(this);
	PathFollowingComp = GetPathFollowingComponent();
	
	SetupTimers();
	
	SetupWorldStates();
	SetupActions();
	SetupGoals();
	
	//If you were to have a sensor labelled "PlayerSensor" you could set it up like this
	//if (PlayerSensor != nullptr) PlayerSensor->OnTargetChanged.AddUObject(this, &AGoapAgent::HandleSensorChange);
}

void AGoapAgent::Tick(float DeltaSeconds)
{
	Super::Tick(DeltaSeconds);
	
	
	// If there is no goal or if enough time has passed then check for replanning
	if (CurrentGoal == nullptr || TimerStopped)
	{
		CheckTimer->Reset();
		CalculatePlan();
	}
	//If we don't have a plan theres nothing left to do
	if (actionPlan == nullptr) return;
		
	if (CurrentAction == nullptr)
	{
		AgentAction& EmptyAction = *new AgentAction();
		//If there are still actions to be performed and the next one has been validated	
		if (!actionPlan->ActionQueue->IsEmpty() && Planner->ValidateCurrentPlan(*actionPlan, EmptyAction))
		{
			CurrentAction = &EmptyAction;
			StopMovement();
			
			CurrentGoal = actionPlan->agentGoal;
			
			CurrentAction->Start();
			
		}
	}
	
	// If we have a plan then execute on it
	if (CurrentAction != nullptr)
	{
		CurrentAction->Update(DeltaSeconds);
		
		// Things to do when the current action ends
		if (CurrentAction->GetComplete())
		{
			CurrentAction->Stop();
			CurrentAction = nullptr;
			
			if (actionPlan->ActionQueue->IsEmpty())
			{
				LastGoal = CurrentGoal;
				CurrentGoal = nullptr;
			}
		}
	}
	
}

void AGoapAgent::SetupTimers()
{
	CheckTimer = new CountdownTimer(.5f);
	CheckTimer->OnTimerStop.AddLambda([this]{TimerStopped = true;});
}

void AGoapAgent::SetupWorldStates()
{
	
	WSFactory Factory = *new WSFactory(this, &WorldStates);
	
	// Place new world states here with
	// Factory.AddWorldState(FName("World State Name"), []{function to be ran that returns a bool});
	// or
	// Factory.AddLocationWorldState(FName("World State Name"), float that holds the distance the agent needs to be to the target, FVector or Transform variable that holds the target's location);
	
}

void AGoapAgent::SetupActions()
{
	//Place new actions here with
	//Actions.Emplace((new AgentAction::Builder(FName("Action Name")))->WithStrategy(new StrategyConstructor).AddEffect(WorldStates.Find(FName("World State Name"))).Build());
	
}

void AGoapAgent::SetupGoals()
{
	//Place new Goals here with
	//Goals.Emplace((new AgentGoal::Builder(FName("Goal Name")))->WithPriority(priorityValue).WithDesiredEffect(WorldStates.Find(FName("World State Name"))).Build());
	
}

void AGoapAgent::CalculatePlan()
{
	TSet<AgentGoal*> GoalsToCheck = Goals;
	
	// If we have a current goal we only need to check goals with a higher priority level
	if (CurrentGoal != nullptr)
	{
		for (auto& Goal : GoalsToCheck)
		{
			if (!Goal->IsValid() || Goal->GetPriority() <= CurrentGoal->GetPriority())
			{
				GoalsToCheck.Remove(Goal);
			}
		}
	}

	ActionPlan* Plan = Planner->Plan(&this->Actions, GoalsToCheck, LastGoal);
	
	if (Plan != nullptr) actionPlan = Plan;
}

void AGoapAgent::HandleSensorChange()
{
	// This is called when the agent becomes aware of the player or loses awareness
	// Forces a replan to occur
	CurrentAction = nullptr;
	CurrentGoal = nullptr;
}

bool AGoapAgent::InRangeOf(FVector* pos, float range)
{
	return FVector::Dist(GetPawn()->GetActorLocation(), *pos) < range;
}
