// Fill out your copyright notice in the Description page of Project Settings.


#include "PatrollingAIController.h"
#include "NavigationSystem.h"

void APatrollingAIController::BeginPlay() {
  Super::BeginPlay();

  NavArea = FNavigationSystem::GetCurrent<UNavigationSystemV1>(this);

  RandomPatrol();
}

void APatrollingAIController::RandomPatrol() {
  // If the NavArea was successfully created, get a random point in the world and move
  //  this Pawn to that location

  if (NavArea) {
    // Parameters
    //  The World we're existing in
    //  The origin point of our travel
    //  The location we want to go to
    //  The radius we would allow a point to be chosen in
    NavArea->K2_GetRandomPointInNavigableRadius(GetWorld(), GetPawn()->GetActorLocation(), RandomLocation, 15000.0f);

    // This function comes from the AIController parent class and moves our pawn
    MoveToLocation(RandomLocation);
  }
}