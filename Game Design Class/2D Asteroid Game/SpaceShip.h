// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "PaperCharacter.h"
#include "InputActionValue.h"
#include "InputAction.h"
#include "SpaceShip.generated.h"

/**
 * 
 */
UCLASS()
class CSE386CURRIEJL_API ASpaceShip : public APaperCharacter
{
	GENERATED_BODY()
	
public:
	ASpaceShip();
protected:
	virtual void BeginPlay() override;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Enhanced Input")
	class UInputMappingContext* InputMapping;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Enhanced Input")
	UInputAction* InputMove;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Enhanced Input")
	UInputAction* InputShoot;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FVector ProjectileSpawn;

public:
	virtual void Tick(float DeltaTime) override;
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;
	void Move(const FInputActionValue& Value);
	void Shoot(const FInputActionValue& Value);

	UPROPERTY(BlueprintReadWrite)
	int Health;
};
