// Fill out your copyright notice in the Description page of Project Settings.

#include "WraparoundVolume.h"
#include "SpaceLaser.h"


AWraparoundVolume::AWraparoundVolume()
{
}

void AWraparoundVolume::BeginPlay()
{
  Super::BeginPlay();

  OnActorBeginOverlap.AddDynamic(this, &AWraparoundVolume::OnOverlapBegin);
  OnActorEndOverlap.AddDynamic(this, &AWraparoundVolume::OnOverlapEnd);
}

void AWraparoundVolume::OnOverlapBegin(class AActor* OverlappedActor, class AActor* OtherActor) {
  SpaceShip = Cast<ASpaceShip>(OtherActor);

  if (SpaceShip) {
    PlayerDetected = true;
  }
}

void AWraparoundVolume::OnOverlapEnd(class AActor* OverlappedActor, class AActor* OtherActor) {
    
    ASpaceLaser* Laser = Cast<ASpaceLaser>(OtherActor);

    if (Laser) {
      return;
    }
    

    FVector ActorLocation = OtherActor->GetActorLocation();
    if (ActorLocation.X > BoundsX || ActorLocation.X < -BoundsX) {
      ActorLocation.X *= -1;
    } 
    if (ActorLocation.Z > BoundsY || ActorLocation.Z < -BoundsY) {
      ActorLocation.Z *= -1;
    }

    OtherActor->SetActorLocation(ActorLocation);
  
}
