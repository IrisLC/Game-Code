// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "AIController.h"
#include "StateMachine.h"
#include "States/EStatePatrol.h"
#include "States/EStateAttack.h"
#include "Enemy_Soldier_AIController.generated.h"

/**
 * 
 */
UCLASS()
class ENEMYAIWORKSPACE_API AEnemy_Soldier_AIController : public AAIController
{
	GENERATED_BODY()

public:
	AEnemy_Soldier_AIController();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Debug")
	bool test1;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Debug")
	bool test2;
	
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "AI")
	UStateMachine* sm;

	UFUNCTION(BlueprintCallable)
	void AITick();

protected:
	virtual void BeginPlay() override;
	virtual void OnPossess(APawn* InPawn) override;

	

private:
	/// <summary>
	/// A pointer for the patrol state used in the state machine
	/// </summary>
	UEStatePatrol* patrol;
	/// <summary>
	/// A pointer for the attack state used in the state machine
	/// </summary>
	UEStateAttack* attack;

	/// <summary>
	/// The reference for the patrol state so that the state can be used in the state machine
	/// </summary>
	TScriptInterface<IState> patrolPtr;
	/// <summary>
	/// The reference for the attack state so that the state can be used in the state machine
	/// </summary>
	TScriptInterface<IState> attackPtr;

	/// <summary>
	/// A method to handle the creation of the state machine and its states.
	/// </summary>
	void StateMachineSetup();
	
};
