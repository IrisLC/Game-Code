// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/GOAP/GoapAgent.h"
#include "GoapPlanner.h"
#include "Actions.h"
#include "NavigationSystem.h"
#include "Navigation/PathFollowingComponent.h"

AGoapAgent::AGoapAgent()
{
}

AGoapAgent::~AGoapAgent()
{
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
	
	UE_LOG(LogTemp, Warning, TEXT("Goap Agent Has Plan with goal %s"), *(actionPlan->agentGoal->Name.ToString()));
	
	if (CurrentAction == nullptr)
	{
		//If there are still actions to be performed	
		if (!actionPlan->ActionQueue.IsEmpty())
		{
			StopMovement();
			
			CurrentAction = actionPlan->ActionQueue[0];
			actionPlan->ActionQueue.RemoveAt(0);
			
			CurrentGoal = actionPlan->agentGoal;
			
			CurrentAction->Start();
		}
	}
	
	// If we have a plan then execute on it
	if (CurrentAction != nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("Acting on Action %s"), *CurrentAction->Name.ToString());
		CurrentAction->Tick(DeltaSeconds);
		
		// Things to do when the current action ends
		if (CurrentAction->GetComplete())
		{
			UE_LOG(LogTemp, Warning, TEXT("Action %s complete"), *CurrentAction->Name.ToString());
			CurrentAction->Stop();
			CurrentAction = nullptr;
			
			if (actionPlan->ActionQueue.IsEmpty())
			{
				UE_LOG(LogTemp, Warning, TEXT("Goal %s complete"), *actionPlan->agentGoal->Name.ToString());

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
	FWSFactory Factory = FWSFactory(*this, WorldStates);
	
	Factory.AddWorldState(FName("Nothing"), []{return false;});
	
	Factory.AddWorldState(FName("AgentIdle"), [this]{return !PathFollowingComp->HasValidPath();});
	
	Factory.AddWorldState(FName("AgentMoving"), [this]{return PathFollowingComp->HasValidPath();});
	
	Factory.AddLocationWorldState(FName("AgentAtHealthLocation"), 3.f, HealthLocation);

	Factory.AddLocationWorldState(FName("AgentAtRestLocation"), 3.f, RestLocation);
}

void AGoapAgent::SetupActions()
{
	Actions.Emplace(MakeShareable<FAgentAction>(FAgentAction::Builder(FName("Idle"))
		.WithStrategy(*new FIdleStrategy(*this, 1))
		.AddEffect(*WorldStates[FName("Nothing")])
		.Build()));
	
	Actions.Emplace( MakeShareable<FAgentAction>(FAgentAction::Builder(FName("Wander Around"))
		.WithStrategy(*new FWanderStrategy(*this, 500))
		.AddEffect(*WorldStates[FName("AgentMoving")])
		.Build()));
}

void AGoapAgent::SetupGoals()
{
	Goals.Emplace(FAgentGoal::Builder(FName("Do Nothing")).
		WithPriority(1)
		.WithDesiredEffect(*WorldStates[FName("Nothing")])
		.Build());
	
	Goals.Emplace(FAgentGoal::Builder(FName("Wander"))
		.WithPriority(1)
		.WithDesiredEffect(*WorldStates[FName("AgentMoving")])
		.Build());
}

void AGoapAgent::CalculatePlan()
{
	UE_LOG(LogTemp, Warning, TEXT("Goap Agent Calculating Plan"));
	TSet<TSharedPtr<FAgentGoal>> GoalsToCheck = Goals;
	
	UE_LOG(LogTemp, Warning, TEXT("Number of Goals To Check %d"), GoalsToCheck.Num());
	// If we have a current goal we only need to check goals with a higher priority level
	if (CurrentGoal != nullptr)
	{
		for (auto& Goal : GoalsToCheck)
		{
			if (!Goal.IsValid() || Goal->GetPriority() <= CurrentGoal->GetPriority())
			{
				GoalsToCheck.Remove(Goal);
			}
		}
	}

	TSharedPtr<FActionPlan> Plan = FGoapPlanner::Plan(this->Actions, GoalsToCheck, LastGoal);
	if (Plan) UE_LOG(LogTemp, Warning, TEXT("Goal is %s"), *(Plan->agentGoal->Name.ToString()));
	
	if (Plan != nullptr) actionPlan = Plan;
}

void AGoapAgent::HandleSensorChange()
{
	// This is called when the agent becomes aware of the player or loses awareness
	// Forces a replan to occur
	CurrentAction = nullptr;
	CurrentGoal = nullptr;
}

bool AGoapAgent::InRangeOf(FVector& pos, float range)
{
	return FVector::Dist(GetPawn()->GetActorLocation(), pos) < range;
}
