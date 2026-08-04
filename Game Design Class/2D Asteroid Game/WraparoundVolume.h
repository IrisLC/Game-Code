// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/TriggerVolume.h"
#include "SpaceShip.h"
#include "WraparoundVolume.generated.h"

/**
 *
 */
UCLASS()
class CSE386CURRIEJL_API AWraparoundVolume : public ATriggerVolume
{
	GENERATED_BODY()

public:
	AWraparoundVolume();

protected:
	virtual void BeginPlay() override;

	UFUNCTION()
	void OnOverlapBegin(class AActor* OverlappedActor, class AActor* OtherActor);
	UFUNCTION()
	void OnOverlapEnd(class AActor* OverlappedActor, class AActor* OtherActor);

	UPROPERTY(EditAnywhere)
	float BoundsX;
	UPROPERTY(EditAnywhere)
	float BoundsY;

	class ASpaceShip* SpaceShip;

	bool PlayerDetected;


public:
	

};
