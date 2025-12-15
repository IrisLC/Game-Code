// Fill out your copyright notice in the Description page of Project Settings.
#include "PatrollingEnemy.h"

#include "ControllableCharacter.h"
#include "Components/SphereComponent.h"
#include "Components/BoxComponent.h"

#include "PAtrollingAIController.h"
#include "Navigation/PathFollowingComponent.h"
#include "AITypes.h"


// Sets default values
APatrollingEnemy::APatrollingEnemy()
{
 	// Set this character to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	PlayerCollisionDetection = CreateDefaultSubobject<USphereComponent>(TEXT("Player Collision Detection"));
	PlayerCollisionDetection->SetupAttachment(RootComponent);

	PlayerAttackCollision = CreateDefaultSubobject<UBoxComponent>(TEXT("Player Attack Collision"));
	PlayerAttackCollision->SetupAttachment(GetMesh(), TEXT("Wolf_-Head"));

	PlayerDetected = false;
	CanAttackPlayer = false;

}

// Called when the game starts or when spawned
void APatrollingEnemy::BeginPlay()
{
	Super::BeginPlay();
	
	//Give our Controller over to our patrolling AIController class and save the reference
	AIController = Cast<APatrollingAIController>(GetController());
	//When we've completed any path following, call OnAIMoveCompleted
	AIController->GetPathFollowingComponent()->OnRequestFinished.AddUObject(this, &APatrollingEnemy::OnAIMoveCompleted);

	//Add links to functions so that our enemy will react to the following events
	PlayerCollisionDetection->OnComponentBeginOverlap.AddDynamic(this, &APatrollingEnemy::OnPlayerDetectedOverlapBegin);

	PlayerCollisionDetection->OnComponentEndOverlap.AddDynamic(this, &APatrollingEnemy::OnPlayerDetectedOverlapEnd);

	PlayerAttackCollision->OnComponentBeginOverlap.AddDynamic(this, &APatrollingEnemy::OnPlayerAttackOverlapBegin);

	PlayerAttackCollision->OnComponentEndOverlap.AddDynamic(this, &APatrollingEnemy::OnPlayerAttackOverlapEnd);
}

// Called every frame
void APatrollingEnemy::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

// Called to bind functionality to input
void APatrollingEnemy::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

}

void APatrollingEnemy::OnAIMoveCompleted(FAIRequestID, const FPathFollowingResult& Result) {
	if (!PlayerDetected) {
		AIController->RandomPatrol();
	}
}

void APatrollingEnemy::MoveToPlayer() {
	if (Player) {
		AIController->MoveToLocation(Player->GetActorLocation(), StoppingDistance, true);
	}

}

void APatrollingEnemy::SeekPlayer() {
	//Move to the player and continue to seek the player out while the timer is running
	MoveToPlayer();
	GetWorld()->GetTimerManager().SetTimer(SeekPlayerTimerHandle, this, &APatrollingEnemy::SeekPlayer, 0.25f, true);

}

void APatrollingEnemy::StopSeekingPlayer() {
	//When we can no longer see the player and it's time to stop looking clear the timer
	GetWorld()->GetTimerManager().ClearTimer(SeekPlayerTimerHandle);
}

void APatrollingEnemy::OnPlayerDetectedOverlapBegin(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult) {
	Player = Cast<AControllableCharacter>(OtherActor);
	if (Player) {
		PlayerDetected = true;
		SeekPlayer();
	}
}

void APatrollingEnemy::OnPlayerDetectedOverlapEnd(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex) {
	Player = Cast<AControllableCharacter>(OtherActor);
	if (Player) {
		PlayerDetected = false;
		StopSeekingPlayer();
		AIController->RandomPatrol();
	}
}

void APatrollingEnemy::OnPlayerAttackOverlapBegin(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBOdyIndex, bool bFromSweep, const FHitResult& SweepResult) {
	Player = Cast<AControllableCharacter>(OtherActor);
	if (Player) {
		PlayerDetected = true;
		CanAttackPlayer = true;
		// deal damage to the player
		UE_LOG(LogTemp, Warning, TEXT("Player Damaged"));
	}
}

void APatrollingEnemy::OnPlayerAttackOverlapEnd(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex) {
	Player = Cast<AControllableCharacter>(OtherActor);
	if (Player) {
		CanAttackPlayer = false;
		SeekPlayer();
	}
}