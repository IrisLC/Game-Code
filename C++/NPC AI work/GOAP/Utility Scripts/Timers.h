// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

/**
 * 
 */
class ENEMYAIWORKSPACE_API FTimer
{
public:
	virtual ~FTimer();

	float Time;
	float Progress = Time / InitialTime;

	DECLARE_MULTICAST_DELEGATE(OnTimerStartEvent);
	OnTimerStartEvent OnTimerStart;
	DECLARE_MULTICAST_DELEGATE(OnTimerStopEvent);
	OnTimerStopEvent OnTimerStop;

protected:
	float InitialTime;

	FTimer(float Value);

private:
	bool IsRunning;

public:
	void Start();
	void Stop();

	void Resume() { IsRunning = true; }
	void Pause() { IsRunning = false; }

	bool GetIsRunning() { return IsRunning; }

	virtual void Tick(float DeltaTime) = 0;
};

class CountdownTimer : public FTimer
{
public:
	CountdownTimer(float Value) : FTimer(Value)
	{
	};
	virtual ~CountdownTimer() override;

	bool IsFinished = Time <= 0;

	virtual void Tick(float DeltaTime) override;

	void Reset();
	void Reset(float NewTime);
};

class StopwatchTimer : public FTimer
{
public:
	StopwatchTimer() : FTimer(0)
	{
	};
	virtual ~StopwatchTimer() override;

	virtual void Tick(float DeltaTime) override;

	void Reset();
	float GetTime();
};
