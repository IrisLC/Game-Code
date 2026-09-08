// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "AIController.h"
#include "Goals.h"
#include "EnemyAI/AICustomController.h"
#include "GoapAgent.generated.h"

class FActionPlan;
class FGoapPlanner;
class FAgentAction;
/**
 * 
 */
UCLASS()
class ENEMYAIWORKSPACE_API AGoapAgent : public AAICustomController
{
	GENERATED_BODY()

public:
	AGoapAgent();
	~AGoapAgent();
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Locations")
	FVector RestLocation;
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Locations")
	FVector HealthLocation;
	
	UPROPERTY()
	class UNavigationSystemV1* NavSys;
	UPROPERTY()
	UPathFollowingComponent* PathFollowingComp;
	
	UPROPERTY();
	AActor* Target;
	UPROPERTY()
	FVector Destination;
	
	CountdownTimer* CheckTimer;
	bool TimerStopped;
	FTimerManager TimerManager;
	FTimerHandle TimerHandle;
	
	FAgentGoal* CurrentGoal;
	TSharedPtr<FAgentAction> CurrentAction;
	TSharedPtr<FActionPlan> actionPlan;

	
	TMap<FName, FWorldState*> WorldStates;
	TSet<TSharedPtr<FAgentAction>> Actions;
	TSet<TSharedPtr<FAgentGoal>> Goals;
	
	virtual void BeginPlay() override;
	
	virtual void Tick(float DeltaSeconds) override;	 
	
private:
	
	FAgentGoal* LastGoal;
	
	void SetupTimers();
	
	void SetupWorldStates();
	void SetupActions();
	void SetupGoals();
	
	void CalculatePlan();
	
	UFUNCTION()
	void HandleSensorChange();
	bool InRangeOf(FVector& pos, float range);
};
