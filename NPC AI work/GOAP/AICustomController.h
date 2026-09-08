// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "AIController.h"
#include "AICustomController.generated.h"

UCLASS()
class ENEMYAIWORKSPACE_API AAICustomController : public AAIController
{
	GENERATED_BODY()

public:
	// Sets default values for this actor's properties
	AAICustomController();

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	
	bool bIsMoveCompleted;
	bool bWasMoveCompletedSuccessfully;

public:
	DECLARE_MULTICAST_DELEGATE(CustomAIMoveCompleted);
	CustomAIMoveCompleted MoveCompleted;
	
	
	// Called every frame
	virtual void Tick(float DeltaTime) override;
	
	virtual FPathFollowingRequestResult MoveTo(const FAIMoveRequest& MoveRequest, FNavPathSharedPtr* OutPath = nullptr) override;
	virtual void OnMoveCompleted(FAIRequestID RequestID, EPathFollowingResult::Type Result) override;
	
	bool GetMoveCompleted() const {return bIsMoveCompleted;}
	bool GetMoveCompletedSuccessfully() const {return bIsMoveCompleted && bWasMoveCompletedSuccessfully;}
};
