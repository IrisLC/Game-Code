// Fill out your copyright notice in the Description page of Project Settings.


#include "MediumAsteroidSprite.h"
#include "SpaceShip.h"
#include "SmallAsteroidSprite.h"
#include "SpaceLaser.h"

AMediumAsteroidSprite::AMediumAsteroidSprite()
{
	RootComponent = CreateDefaultSubobject<USceneComponent>(TEXT("ProjectileSceneComponent"));
	//use a sphere as a simple collision representation & set collision radius
	CollisionComp = CreateDefaultSubobject<USphereComponent>(TEXT("SphereComponent"));

	CollisionComp->InitSphereRadius(18.f);

	//Set the sphere's collision profile nae to "Projectile"
	CollisionComp->BodyInstance.SetCollisionProfileName(TEXT("Asteroid"));
	//Event called when component hits something
	CollisionComp->OnComponentBeginOverlap.AddDynamic(this, &AMediumAsteroidSprite::OnOverlapBegin);
	//Set the root component to be the collision 
	RootComponent = CollisionComp;

	//Create and set up the mesh component to be the imported bullet mesh
	
	static ConstructorHelpers::FObjectFinder<UPaperSprite>Sprite(TEXT("/Game/Assets/Sprites/SpaceAssets/meteor_small_Sprite.meteor_small_Sprite"));
	if (Sprite.Succeeded()) {
		GetRenderComponent()->SetSprite(Sprite.Object);
		
		GetRenderComponent()->SetupAttachment(RootComponent);

		UE_LOG(LogTemp, Warning, TEXT("Success"));
	}

	//Use this to drive the projectile's movement
	PMComp = CreateDefaultSubobject<UProjectileMovementComponent>(TEXT("ProjectileMovementComponent"));
	PMComp->InitialSpeed = 200.f;
	PMComp->MaxSpeed = 200.f;
	PMComp->bRotationFollowsVelocity = true;
	PMComp->bShouldBounce = false;
	PMComp->ProjectileGravityScale = 0.f;


	RootComponent->SetMobility(EComponentMobility::Movable);
	GetRenderComponent()->SetMobility(EComponentMobility::Movable);
}

void AMediumAsteroidSprite::BeginPlay()
{
	Super::BeginPlay();


}

void AMediumAsteroidSprite::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

void AMediumAsteroidSprite::OnOverlapBegin(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult) {
	ASpaceShip* SpaceShip = Cast<ASpaceShip>(OtherActor);
	ASpaceLaser* Laser = Cast<ASpaceLaser>(OtherActor);

	if (SpaceShip) {
		SpaceShip->Health--;
		Destroy();
	}
	else if (Laser) {
		Laser->Destroy();
		FVector SpawnLocation = GetActorLocation();
		FRotator SpawnRotation = GetActorRotation();

		FActorSpawnParameters SpawnParams;
		SpawnParams.Owner = this;
		SpawnParams.Instigator = GetInstigator();

		SpawnRotation.Pitch += 30;

		ASmallAsteroidSprite* spawnedAsteroid = GetWorld()->SpawnActor<ASmallAsteroidSprite>(SpawnLocation, SpawnRotation, SpawnParams);

		SpawnRotation.Pitch -= 60;

		ASmallAsteroidSprite* spawnedAsteroid1 = GetWorld()->SpawnActor<ASmallAsteroidSprite>(SpawnLocation, SpawnRotation, SpawnParams);
		Destroy();
	}
}

