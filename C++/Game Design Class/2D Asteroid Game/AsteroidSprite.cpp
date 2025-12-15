// Fill out your copyright notice in the Description page of Project Settings.


#include "AsteroidSprite.h"
#include "MediumAsteroidSprite.h"
#include "SpaceShip.h"
#include "SpaceLaser.h"

AAsteroidSprite::AAsteroidSprite()
{
	RootComponent = CreateDefaultSubobject<USceneComponent>(TEXT("ProjectileSceneComponent"));
	//use a sphere as a simple collision representation & set collision radius
	CollisionComp = CreateDefaultSubobject<USphereComponent>(TEXT("SphereComponent"));

	CollisionComp->InitSphereRadius(25.0f);

	//Set the sphere's collision profile nae to "Projectile"
	CollisionComp->BodyInstance.SetCollisionProfileName(TEXT("Asteroid"));
	//Event called when component hits something
	CollisionComp->OnComponentBeginOverlap.AddDynamic(this, &AAsteroidSprite::OnOverlapBegin);
	//Set the root component to be the collision 
	RootComponent = CollisionComp;

	//Create and set up the mesh component to be the imported bullet mesh
	
	static ConstructorHelpers::FObjectFinder<UPaperSprite>Sprite(TEXT("/Game/Assets/Sprites/SpaceAssets/meteor_detailedLarge_Sprite.meteor_detailedLarge_Sprite"));
	if (Sprite.Succeeded()) {
		GetRenderComponent()->SetSprite(Sprite.Object);

		GetRenderComponent()->SetupAttachment(RootComponent);

		UE_LOG(LogTemp, Warning, TEXT("%s"), *GetRenderComponent()->GetSprite()->GetName());
	}

	//Use this to drive the projectile's movement
	PMComp = CreateDefaultSubobject<UProjectileMovementComponent>(TEXT("ProjectileMovementComponent"));
	PMComp->InitialSpeed = 100.f;
	PMComp->MaxSpeed = 100.f;
	PMComp->bRotationFollowsVelocity = true;
	PMComp->bShouldBounce = false;
	PMComp->ProjectileGravityScale = 0.f;

	RootComponent->SetMobility(EComponentMobility::Movable);
	GetRenderComponent()->SetMobility(EComponentMobility::Movable);
}

void AAsteroidSprite::BeginPlay()
{
  Super::BeginPlay();
  
  
}

void AAsteroidSprite::Tick(float DeltaTime)
{
  Super::Tick(DeltaTime);
}

void AAsteroidSprite::OnOverlapBegin(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult) {
	ASpaceShip* SpaceShip = Cast<ASpaceShip>(OtherActor);
	ASpaceLaser* Laser = Cast<ASpaceLaser>(OtherActor);

	if (SpaceShip) {
		SpaceShip->Health--;
		Destroy();
	} else if (Laser) {
		Laser->Destroy();
		FVector SpawnLocation = GetActorLocation();
		FRotator SpawnRotation = GetActorRotation();

		FActorSpawnParameters SpawnParams;
		SpawnParams.Owner = this;
		SpawnParams.Instigator = GetInstigator();

		SpawnRotation.Pitch += 30;

		AMediumAsteroidSprite* spawnedAsteroid = GetWorld()->SpawnActor<AMediumAsteroidSprite>(SpawnLocation, SpawnRotation, SpawnParams);
		
		SpawnRotation.Pitch -= 60;

		AMediumAsteroidSprite* spawnedAsteroid1 = GetWorld()->SpawnActor<AMediumAsteroidSprite>(SpawnLocation, SpawnRotation, SpawnParams);


		Destroy();
	}

	
	
}

