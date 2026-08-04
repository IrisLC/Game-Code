// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "Utility Scripts/Timers.h"
#include "Sensor.generated.h"

/// <summary>
/// An Actor Component that uses a sphereComponent and works in tandem with the WorldState system that can be used to detect the player 
/// </summary>
UCLASS(ClassGroup=(Custom), meta=(BlueprintSpawnableComponent))
class ENEMYAIWORKSPACE_API USensor : public UPrimitiveComponent
{
	GENERATED_BODY()

public:
	// Sets default values for this component's properties
	USensor();

	/// <summary>
	/// The radius of the collider
	/// </summary>
	UPROPERTY(EditAnywhere)
		float DetectionRadius = 5.f;
	/// <summary>
	/// How often to check the location of the target
	/// </summary>
	UPROPERTY(EditAnywhere)
		float TimerInterval = 1.f;

	/// <summary>
	/// The SphereComponent that handles overlaps
	/// </summary>
	UPROPERTY(EditAnywhere)
		class USphereComponent* DetectionRange;

	/// <summary>
	/// The delegate that is called when the player enters or leaves the collider
	/// </summary>
	DECLARE_MULTICAST_DELEGATE(OnTargetChangedEvent);
	/// <summary>
	/// The delegate that is called when the player enters or leaves the collider
	/// </summary>
	OnTargetChangedEvent OnTargetChanged;

	/// <summary>
	/// The position of the target
	/// </summary>
	FVector TargetPosition = Target ? FVector(Target->GetActorLocation()) : FVector::ZeroVector;
	/// <summary>
	/// True iff the Target is within the raedius of the SphereCollider
	/// </summary>
	bool IsTargetInRange = TargetPosition != FVector::ZeroVector;

	/// <summary>
	/// A pointer to the player pawn
	/// </summary>
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
		AActor* Player;
	/// <summary>
	/// A pointer to the sensor being aware of the player. This will only have a value when the Player is within range of the sensor, otherwise will be a nullptr
	/// </summary>
	UPROPERTY()
		AActor* Target;
	/// <summary>
	/// The last known position of the target
	/// </summary>
	FVector LastKnownPosition;

	/// <summary>
	/// A timer that handles when to check for the location of the player
	/// </summary>
	CountdownTimer* Timer;

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

	/// <summary>
	/// Called when the timer hits 0 or an overlap starts or stops with the SphereComponent.
	/// Updates the known location of the target.
	/// </summary>
	/// <param name="Target">The location of the target if the sensor is aware of the player, nullptr otherwise</param>
	void UpdateTargetPosition(AActor* Target = nullptr);

	UFUNCTION()
		void OnOverlapBegin(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor,
		                    class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep,
		                    const FHitResult& SweepResult);
	UFUNCTION()
		void OnOverlapEnd(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor,
		                  class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex);

public:
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType,
	                           FActorComponentTickFunction* ThisTickFunction) override;
};
