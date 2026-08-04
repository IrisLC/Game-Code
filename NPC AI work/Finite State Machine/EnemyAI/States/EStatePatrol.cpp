// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/States/EStatePatrol.h"

// Sets default values for this component's properties
UEStatePatrol::UEStatePatrol()
{

}

void UEStatePatrol::OnEnter()
{
	UE_LOG(LogTemp, Warning, TEXT("Patrol Entered"));
}

void UEStatePatrol::SMTick()
{
	UE_LOG(LogTemp, Warning, TEXT("Patrol Ticked"));
}

void UEStatePatrol::OnExit()
{
	UE_LOG(LogTemp, Warning, TEXT("Patrol Left"));
}

