// Fill out your copyright notice in the Description page of Project Settings.


#include "SpaceLaser.h"

// Sets default values
ASpaceLaser::ASpaceLaser()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

	RootComponent = CreateDefaultSubobject<USceneComponent>(TEXT("ProjectileSceneComponent"));
	//use a sphere as a simple collision representation & set collision radius
	CollisionComp = CreateDefaultSubobject<USphereComponent>(TEXT("SphereComponent"));

	CollisionComp->InitSphereRadius(5.0f);

	//Set the sphere's collision profile nae to "Projectile"
	CollisionComp->BodyInstance.SetCollisionProfileName(TEXT("SpaceProjectile"));
	//Set the root component to be the collision 
	RootComponent = CollisionComp;

	//Create and set up the mesh component to be the imported bullet mesh
	SpriteComp = CreateDefaultSubobject<UPaperSpriteComponent>(TEXT("Sprite"));
	static ConstructorHelpers::FObjectFinder<UPaperSprite>Sprite(TEXT("/Game/Assets/Sprites/SpaceAssets/Default/ship_B_Sprite.ship_B_Sprite"));
	if (Sprite.Succeeded()) {
		SpriteComp->SetSprite(Sprite.Object);


		SpriteComp->SetRelativeRotation(FRotator(-90.f, 0.f, 0.f));
		SpriteComp->SetRelativeScale3D(FVector(.5f, .5f, .5f));

		SpriteComp->SetupAttachment(RootComponent);
	}

	//Use this to drive the projectile's movement
	PMComp = CreateDefaultSubobject<UProjectileMovementComponent>(TEXT("ProjectileMovementComponent"));
	PMComp->InitialSpeed = 1000.f;
	PMComp->MaxSpeed = 1000.f;
	PMComp->bRotationFollowsVelocity = true;
	PMComp->bShouldBounce = false;
	PMComp->ProjectileGravityScale = 0.f;
	InitialLifeSpan = 3.f;

	RootComponent->SetMobility(EComponentMobility::Movable);
	SpriteComp->SetMobility(EComponentMobility::Movable);

}

// Called when the game starts or when spawned
void ASpaceLaser::BeginPlay()
{
	Super::BeginPlay();
	
	UE_LOG(LogTemp, Warning, TEXT("Test"));
}

// Called every frame
void ASpaceLaser::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

}

void ASpaceLaser::FireInDirection(const FVector& ShootDirection) {
	PMComp->Velocity = ShootDirection * PMComp->InitialSpeed;
}

