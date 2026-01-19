// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UObject/Object.h"
#include "../State.h"
#include "EStateAttack.generated.h"


UCLASS()
class ENEMYAIWORKSPACE_API UEStateAttack : public UObject, public IState
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UEStateAttack();

	void OnEnter() override;

	void SMTick() override;

	void OnExit() override;
		
};
