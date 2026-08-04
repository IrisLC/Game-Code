// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/Enemy_Soldier_AIController.h"


AEnemy_Soldier_AIController::AEnemy_Soldier_AIController()
{
  
}

void AEnemy_Soldier_AIController::BeginPlay()
{
}

void AEnemy_Soldier_AIController::OnPossess(APawn* InPawn)
{
  Super::OnPossess(InPawn);

  StateMachineSetup();
}

void AEnemy_Soldier_AIController::AITick()
{
  UE_LOG(LogTemp, Warning, TEXT("AITick Entered"));
  if (sm == nullptr) return;
  sm->SMTick();
}

/// <summary>
/// A method to handle the creation of the state machine and its states in the following order:
/// 1. Instantiates the state machine and the states with this controller as the attached outer.
/// 2. Creates the TScriptInterfaces so the states can be used in the state machine methods that require the use of the IState interface.
/// 3. Creates the TFunctions that are used as the predicates for transitions between states.
/// 4. Uses the created TScriptInterfaces and TFunctions to create the transitions between specific states.
/// 5. Sets the initial state that will be used for the controller. 
/// </summary>
void AEnemy_Soldier_AIController::StateMachineSetup()
{
  // 1.
  sm = NewObject<UStateMachine>(this);
  patrol = NewObject<UEStatePatrol>(this);
  attack = NewObject<UEStateAttack>(this);

  // 2.
  patrolPtr = TScriptInterface<IState>(patrol);
  attackPtr = TScriptInterface<IState>(attack);

  // 3.
  TFunction<bool()> PToA = [&]() { return test1; };
  TFunction<bool()> AToP = [&]() { return test2; };

  // 4.
  sm->AddTransition(&patrolPtr, &attackPtr, PToA);
  sm->AddTransition(&attackPtr, &patrolPtr, AToP);

  // 5.
  sm->SetState(&patrolPtr);

}


