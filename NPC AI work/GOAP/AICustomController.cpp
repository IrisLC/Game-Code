// Fill out your copyright notice in the Description page of Project Settings.


#include "AICustomController.h"

#include "Navigation/PathFollowingComponent.h"


// Sets default values
AAICustomController::AAICustomController()
{
	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;
}

// Called when the game starts or when spawned
void AAICustomController::BeginPlay()
{
	Super::BeginPlay();
	
}

// Called every frame
void AAICustomController::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

FPathFollowingRequestResult AAICustomController::MoveTo(const FAIMoveRequest& MoveRequest, FNavPathSharedPtr* OutPath)
{
	bIsMoveCompleted = false;
	bWasMoveCompletedSuccessfully = false;
	
	return Super::MoveTo(MoveRequest, OutPath);
}

void AAICustomController::OnMoveCompleted(FAIRequestID RequestID, EPathFollowingResult::Type Result)
{
	Super::OnMoveCompleted(RequestID, Result);
	
	bIsMoveCompleted = true;
	
	if (Result == EPathFollowingResult::Type::Success)
	{
		bWasMoveCompletedSuccessfully = true;
	}
	
	if (MoveCompleted.IsBound())
	{
		MoveCompleted.Broadcast();
	}
}

