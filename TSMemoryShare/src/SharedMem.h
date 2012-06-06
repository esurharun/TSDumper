/*
* Copyright (c) 2009 Blue Bit Solutions (www.bluebit.com.au)
*
* This file is part of TV Scheduler Pro
* 
* TV Scheduler Pro is free software: you can redistribute it and/or 
* modify it under the terms of the GNU General Public License as published 
* by the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
* 
* TV Scheduler Pro is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
* 
* You should have received a copy of the GNU General Public License
* along with TV Scheduler Pro.
* If not, see <http://www.gnu.org/licenses/>.
*/

#pragma once

#include <windows.h>

#define shareDataSize 15000000
//#define shareDataSize 52428800

typedef struct sharedDataMap
{
	int currentPointer;
	int loopCount;
	int quality;
	long signalQuality;
	long signalStrength;
	BYTE data[shareDataSize];
} SHARED_DATA;

int createSharedMemory(char *pShareName);
int openSharedMemory(char *pShareName);
void closeSharedMemory();
SHARED_DATA* getSharedMemory();
BOOL isOpen();
