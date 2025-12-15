// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "PatrollingEnemy.generated.h"

UCLASS()
class CSE386CURRIEJL_API APatrollingEnemy : public ACharacter
{
	GENERATED_BODY()

public:
	// Sets default values for this character's properties
	APatrollingEnemy();
	
protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	//booleans for detecting the player and attacking
	bool PlayerDetected;
	UPROPERTY(BlueprintReadWrite)
	bool CanAttackPlayer;

	// a variable to hold a reference to our player when we need it
	class AControllableCharacter* Player;

	// Collision for detecting the player near the enemy
	UPROPERTY(EditAnywhere)
	class USphereComponent* PlayerCollisionDetection;

	// Collision for detecting if the player can be attacked
	UPROPERTY(EditAnywhere)
	class UBoxComponent* PlayerAttackCollision;

	class APatrollingAIController* AIController;

	void OnAIMoveCompleted(struct FAIRequestID RequestID, const struct FPathFollowingResult& Result);

	UPROPERTY(EditAnywhere)
	float StoppingDistance = 200.f;

	FTimerHandle SeekPlayerTimerHandle;

	UFUNCTION()
	void MoveToPlayer();

	UFUNCTION()
	void SeekPlayer();

	UFUNCTION()
	void StopSeekingPlayer();

	UFUNCTION()
	void OnPlayerDetectedOverlapBegin(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor, class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);

	UFUNCTION()
	void OnPlayerDetectedOverlapEnd(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor, class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex);

	UFUNCTION()
	void OnPlayerAttackOverlapBegin(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor, class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);

	UFUNCTION()
	void OnPlayerAttackOverlapEnd(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor, class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex);

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

};
