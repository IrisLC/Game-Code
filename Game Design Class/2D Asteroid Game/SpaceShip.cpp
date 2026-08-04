// Fill out your copyright notice in the Description page of Project Settings.


#include "SpaceShip.h"
#include "InputMappingContext.h"
#include "EnhancedInputSubsystems.h"
#include "EnhancedInputComponent.h"
#include "SpaceLaser.h"

ASpaceShip::ASpaceShip()
{
}

void ASpaceShip::BeginPlay()
{
  Super::BeginPlay();
	Health = 3;
}

void ASpaceShip::Tick(float DeltaTime)
{
  Super::Tick(DeltaTime);
}

void ASpaceShip::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

	//Get the player controller
	APlayerController* PC = Cast<APlayerController>(GetController());

	//Get the local player subsystems
	UEnhancedInputLocalPlayerSubsystem* Subsystem =
		ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(PC->GetLocalPlayer());

	//Clear out existing mapping, and add our mapping
	Subsystem->ClearAllMappings();
	Subsystem->AddMappingContext(InputMapping, 0);

	//Get the EnhancedInputComponent
	UEnhancedInputComponent* PEI = Cast<UEnhancedInputComponent>(PlayerInputComponent);
	//Bind the actions
	PEI->BindAction(InputMove, ETriggerEvent::Triggered, this, &ASpaceShip::Move);
	PEI->BindAction(InputShoot, ETriggerEvent::Triggered, this, &ASpaceShip::Shoot);

}

void ASpaceShip::Move(const FInputActionValue& Value){
	if (Controller != nullptr) {
		FVector2D MoveValue = Value.Get<FVector2D>();
		FVector PlayerLocation = GetActorLocation();

		if (MoveValue.X != 0.0f) {
			AddActorLocalRotation(FRotator(MoveValue.X * 5, 0, 0));
		}

		if (MoveValue.Y != 0.f) {
			PlayerLocation += GetActorForwardVector() * MoveValue.Y * 5;
			SetActorLocation(PlayerLocation);
		}
	}
 }

void ASpaceShip::Shoot(const FInputActionValue& Value)
{

	//Set ProjectileSpawn to spawn projectiles slightly in front of the character
	ProjectileSpawn.Set(20.f, 0.f, 0.f);

	//Transform ProjectileSpawn from local space to world space
	FVector SpawnLocation = GetActorLocation() +
		FTransform(GetActorRotation()).TransformVector(ProjectileSpawn);

	// Skew the aim to be slightly upwards
	FRotator SpawnRotation = GetActorRotation();

	//Give the projectile some metadata
	FActorSpawnParameters SpawnParams;
	SpawnParams.Owner = this;
	SpawnParams.Instigator = GetInstigator();

	//Spawn the projectile using our locations and metadata
	ASpaceLaser* Bullet = GetWorld()->SpawnActor<ASpaceLaser>(SpawnLocation, SpawnRotation, SpawnParams);

	if (Bullet) {
		//set the projectile's initial trajectory.
		FVector LaunchDirection = SpawnRotation.Vector();
		Bullet->FireInDirection(LaunchDirection);
	}
}
