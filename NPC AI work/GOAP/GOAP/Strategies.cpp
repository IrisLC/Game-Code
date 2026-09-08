// Fill out your copyright notice in the Description page of Project Settings.


#include "Strategies.h"
#include "Engine/World.h"
#include "NavigationSystem.h"


IActionStrategies::IActionStrategies(AGoapAgent& agent) : Agent{agent}, CanPerform{true}, IsComplete{false}, IsActive{false}
{
}

IActionStrategies::~IActionStrategies()
{
}

FIdleStrategy::FIdleStrategy(AGoapAgent& agent, float duration) : IActionStrategies{agent}, TimerManager{agent.GetWorldTimerManager()}
{
	timerLength = duration;
}

FIdleStrategy::~FIdleStrategy()
{
	TimerManager.ClearTimer(Timer);
}

void FIdleStrategy::Start()
{
	IActionStrategies::Start();
	IsComplete = false;
	TimerManager.SetTimer(Timer, FTimerDelegate::CreateRaw(this, &FIdleStrategy::OnTimerStop), timerLength, false);
}

void FIdleStrategy::Update(float DeltaTime)
{
	UE_LOG(LogTemp, Warning, TEXT("Time remaining %f"), TimerManager.GetTimerRemaining(Timer));
}

void FIdleStrategy::OnTimerStop()
{
	TimerManager.ClearTimer(Timer);
	IsComplete = true;
}

FWanderStrategy::FWanderStrategy(AGoapAgent& Agent, float WanderRadius)
	: IActionStrategies{Agent}, PathFollowingComp{Agent.PathFollowingComp}, WanderRadius{WanderRadius}
{
	this->Agent.MoveCompleted.AddRaw(this, &FWanderStrategy::ReachedDestination);
}

FWanderStrategy::~FWanderStrategy()
{
	Agent.MoveCompleted.RemoveAll(this);
}

void FWanderStrategy::Start()
{
	IActionStrategies::Start();
	
	
	if (Agent.NavSys)
	{
		Agent.NavSys->K2_GetRandomLocationInNavigableRadius
		(Agent.GetWorld(), PathFollowingComp->GetCurrentNavLocation(), RandomLocation, WanderRadius);
		RandomLocation.Z = Agent.GetPawn()->GetActorLocation().Z;
		Agent.MoveToLocation(RandomLocation);
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("No nav system"));
		return;
	}
	
	IsComplete = false;
}

void FWanderStrategy::Stop()
{
	IActionStrategies::Stop();
}

void FWanderStrategy::ReachedDestination()
{
	if (IsActive) IsComplete = true;
}

