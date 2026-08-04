// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PaperSpriteActor.h"
#include "Components/SphereComponent.h"
#include "PaperSpriteComponent.h"
#include "PaperSprite.h"
#include "GameFramework/ProjectileMovementComponent.h"
#include "MediumAsteroidSprite.generated.h"

/**
 *
 */
UCLASS()
class CSE386CURRIEJL_API AMediumAsteroidSprite : public APaperSpriteActor
{
	GENERATED_BODY()

public:
	//Constructor
	AMediumAsteroidSprite();
protected:
	//Called when the game starts or when spawned
	virtual void BeginPlay() override;

	UPROPERTY(VisibleDefaultsOnly)
	USphereComponent* CollisionComp;


	//Component that we can use to make actor move
	UPROPERTY(VisibleAnywhere)
	UProjectileMovementComponent* PMComp;


public:


	virtual void Tick(float DeltaTime) override;

	UFUNCTION()
	void OnOverlapBegin(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);
};
