// Fill out your copyright notice in the Description page of Project Settings.


#include "Utility Scripts/Timers.h"

FTimer::~FTimer()
{
}

FTimer::FTimer(float Value)
{
	InitialTime = Value;
	IsRunning = false;
}

void FTimer::Start()
{
	Time = InitialTime;

	if (!IsRunning)
	{
		IsRunning = true;
		OnTimerStart.Broadcast();
	}
}

void FTimer::Stop()
{
	if (IsRunning)
	{
		IsRunning = false;
		OnTimerStop.Broadcast();
	}
}

/*CountdownTimer*/

CountdownTimer::~CountdownTimer()
{
}

void CountdownTimer::Tick(float DeltaTime)
{
	if (GetIsRunning() && Time > 0)
	{
		Time -= DeltaTime;
	}

	if (GetIsRunning() && Time <= 0)
	{
		Stop();
	}
}

void CountdownTimer::Reset()
{
	Time = InitialTime;
}

void CountdownTimer::Reset(float NewTime)
{
	InitialTime = NewTime;
	Reset();
}

/*StopwatchTimer*/

StopwatchTimer::~StopwatchTimer()
{
}

void StopwatchTimer::Tick(float DeltaTime)
{
	if (GetIsRunning())
	{
		Time += DeltaTime;
	}
}

void StopwatchTimer::Reset()
{
	Time = 0;
}

float StopwatchTimer::GetTime()
{
	return Time;
}
