// Autocorrelation.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <iostream>
#include <fstream>
#include <string>


float * resultArr = new float[2];
int * rawData;
int i, j;
float sum, oldSum;
float threshold;
float detectedFreq;
char mState;
float maxSum, ffSum;

float * Autocorrelation(int samplingFrequency, int newSamplesCount, float thresholdValue)
{
	sum = 0;
	mState = 0;
	int detectedPeriod = 0;

	for (i = 0; i < newSamplesCount; i++)
	{
		oldSum = sum;
		sum = 0;

		for (j = 0; j < newSamplesCount - i; j++)
		{
			sum += (rawData[j] - 128) * (rawData[j + i] - 128) / 256;
			if (i == 0)
				maxSum = sum;
		}

		if (mState == 2 && (sum - oldSum) <= 0)
		{
			detectedPeriod = i;
			mState = 3;
			ffSum = sum;
		}

		if (mState == 1 && (sum > threshold) && (sum - oldSum) > 0)
			mState = 2;

		if (i == 0)
		{
			mState = 1;
			threshold = sum * thresholdValue; // 0 < thresholdValue < 1
		}
	}
	
	// Very crude approximation (by interpolation) of correct frequency
	float deltaPhi = ffSum / maxSum * 180;
	float shiftValue = (180 - deltaPhi) / 180;

	detectedFreq = (float)samplingFrequency / ((float)detectedPeriod - shiftValue);
	float periodValueMS = (float)detectedPeriod / (float)samplingFrequency * 1000;

	resultArr[0] = detectedFreq;
	resultArr[1] = periodValueMS;

	delete rawData;

	return resultArr;
}

int ReadInputFile(const char * filePath, int samplesCount)
{
	int buffer;
	std::ifstream dataFile(filePath, std::ios::in);
	rawData = new int[samplesCount];

	samplesCount = 0;

	if (dataFile.is_open())
		while (!dataFile.eof())
			if (dataFile >> buffer) // may be problematic
				rawData[samplesCount++] = buffer;
			else std::cout << buffer << ", iteration: " << samplesCount << std::endl;

	dataFile.close();

	return samplesCount;
}

extern "C" __declspec(dllexport) float * ReadAndProcessSamples(const char* filePath,
	int samplingFrequency,
	int samplesCount,
	float thresholdValue)
{
	int newSamplesCount = ReadInputFile(filePath, samplesCount);
	return Autocorrelation(samplingFrequency, newSamplesCount, thresholdValue);
};