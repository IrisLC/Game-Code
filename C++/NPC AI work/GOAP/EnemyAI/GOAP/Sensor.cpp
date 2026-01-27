// Fill out your copyright notice in the Description page of Project Settings.


#include "EnemyAI/GOAP/Sensor.h"

#include "Components/SphereComponent.h"

// Sets default values for this component's properties
USensor::USensor()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

	DetectionRange = CreateDefaultSubobject<USphereComponent>(TEXT("DetectionRange"));
	DetectionRange->SetupAttachment(GetAttachParent());

	DetectionRange->SetSphereRadius(DetectionRadius);

	// ...
}


// Called when the game starts
void USensor::BeginPlay()
{
	Super::BeginPlay();


	// ...

	Timer = new CountdownTimer(TimerInterval);

	Timer->OnTimerStart.AddLambda([this]()
	{
		UpdateTargetPosition(Target);
		Timer->Start();
	});

	Player = GetWorld()->GetFirstPlayerController()->GetPawn();

	DetectionRange->OnComponentBeginOverlap.AddDynamic(this, &USensor::OnOverlapBegin);
	DetectionRange->OnComponentEndOverlap.AddDynamic(this, &USensor::OnOverlapEnd);
}

void USensor::UpdateTargetPosition(AActor* target)
{
	Target = target;

	if (IsTargetInRange &&
		(LastKnownPosition != FVector(target->GetActorLocation()) || LastKnownPosition != FVector::ZeroVector))
	{
		LastKnownPosition = TargetPosition;
		OnTargetChanged.Broadcast();
	}
}

void USensor::OnOverlapBegin(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor,
                             class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep,
                             const FHitResult& SweepResult)
{
	if (OtherActor != Player) return;
	UpdateTargetPosition(OtherActor);
}

void USensor::OnOverlapEnd(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor,
                           class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex)
{
	if (OtherActor != Player) return;
	UpdateTargetPosition();
}


// Called every frame
void USensor::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	Timer->Tick(DeltaTime);
	// ...
}
