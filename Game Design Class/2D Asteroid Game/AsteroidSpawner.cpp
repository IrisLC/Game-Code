// Fill out your copyright notice in the Description page of Project Settings.


#include "AsteroidSpawner.h"
#include "AsteroidSprite.h"

// Sets default values
AAsteroidSpawner::AAsteroidSpawner()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void AAsteroidSpawner::BeginPlay()
{
	Super::BeginPlay();

	GetWorld()->GetTimerManager().SetTimer(TimerHandle, this, &AAsteroidSpawner::SpawnActor, 5.f, true);
	
}

// Called every frame
void AAsteroidSpawner::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void AAsteroidSpawner::SpawnActor() {

	FVector SpawnLocation = GetActorLocation();
	FRotator SpawnRotation = FRotator(FMath::RandRange(0.f, 360.f), 0, 0);

	SpawnLocation.X += FMath::RandRange(-450.f, 450.f);
	SpawnLocation.Z += FMath::RandRange(-250.f, 250.f);

	

	GetWorld()->SpawnActor<AAsteroidSprite>(SpawnLocation, SpawnRotation);
}

