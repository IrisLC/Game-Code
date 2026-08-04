// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Components/SphereComponent.h"
#include "PaperSpriteComponent.h"
#include "PaperSprite.h"
#include "GameFramework/ProjectileMovementComponent.h"
#include "SpaceLaser.generated.h"


UCLASS()
class CSE386CURRIEJL_API ASpaceLaser : public AActor
{
	GENERATED_BODY()
	
public:	
	// Sets default values for this actor's properties
	ASpaceLaser();
	//Function that initializees the projectile's velocity in the shoot direction
	void FireInDirection(const FVector& ShootDirection);
	

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	//Collision Component
	UPROPERTY(VisibleDefaultsOnly)
	USphereComponent* CollisionComp;

	//Static mesh displaying bullet
	UPROPERTY(VisibleDefaultsOnly)
	UPaperSpriteComponent* SpriteComp;

	//Component that we can use to make actor move
	UPROPERTY(VisibleAnywhere)
	UProjectileMovementComponent* PMComp;

	UPROPERTY(EditAnywhere)
	class UNiagaraSystem* ParticleBurst;

public:
	// Called every frame
	virtual void Tick(float DeltaTime) override;



};
