// Fill out your copyright notice in the Description page of Project Settings.


#include "SmallAsteroidSprite.h"
#include "SpaceShip.h"
#include "SpaceLaser.h"

ASmallAsteroidSprite::ASmallAsteroidSprite()
{
	RootComponent = CreateDefaultSubobject<USceneComponent>(TEXT("ProjectileSceneComponent"));
	//use a sphere as a simple collision representation & set collision radius
	CollisionComp = CreateDefaultSubobject<USphereComponent>(TEXT("SphereComponent"));

	CollisionComp->InitSphereRadius(5.f);

	//Set the sphere's collision profile nae to "Projectile"
	CollisionComp->BodyInstance.SetCollisionProfileName(TEXT("Asteroid"));
	//Event called when component hits something
	CollisionComp->OnComponentBeginOverlap.AddDynamic(this, &ASmallAsteroidSprite::OnOverlapBegin);
	//Set the root component to be the collision 
	RootComponent = CollisionComp;

	//Create and set up the mesh component to be the imported bullet mesh

	static ConstructorHelpers::FObjectFinder<UPaperSprite>Sprite(TEXT("/Game/Assets/Sprites/SpaceAssets/star_tiny_Sprite.star_tiny_Sprite"));
	if (Sprite.Succeeded()) {
		GetRenderComponent()->SetSprite(Sprite.Object);

		GetRenderComponent()->SetupAttachment(RootComponent);

		UE_LOG(LogTemp, Warning, TEXT("Success"));
	}

	//Use this to drive the projectile's movement
	PMComp = CreateDefaultSubobject<UProjectileMovementComponent>(TEXT("ProjectileMovementComponent"));
	PMComp->InitialSpeed = 300.f;
	PMComp->MaxSpeed = 300.f;
	PMComp->bRotationFollowsVelocity = true;
	PMComp->bShouldBounce = false;
	PMComp->ProjectileGravityScale = 0.f;


	RootComponent->SetMobility(EComponentMobility::Movable);
	GetRenderComponent()->SetMobility(EComponentMobility::Movable);
}

void ASmallAsteroidSprite::BeginPlay()
{
	Super::BeginPlay();


}

void ASmallAsteroidSprite::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

void ASmallAsteroidSprite::OnOverlapBegin(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult) {
	ASpaceShip* SpaceShip = Cast<ASpaceShip>(OtherActor);

	ASpaceLaser* Laser = Cast<ASpaceLaser>(OtherActor);

	if (SpaceShip) {
		SpaceShip->Health--;
		Destroy();
	}
	else if (Laser) {
		Laser->Destroy();
		Destroy();
	}
}
