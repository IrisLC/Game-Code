// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/States/EStateAttack.h"

// Sets default values for this component's properties
UEStateAttack::UEStateAttack()
{

}

void UEStateAttack::OnEnter()
{
	UE_LOG(LogTemp, Warning, TEXT("Attack Entered"));
}

void UEStateAttack::SMTick()
{
	UE_LOG(LogTemp, Warning, TEXT("Attack Ticked"));
}

void UEStateAttack::OnExit()
{
	UE_LOG(LogTemp, Warning, TEXT("Attack Left"));
}

