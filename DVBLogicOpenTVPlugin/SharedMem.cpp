////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2011 nzsjb                                           //
//                                                                              //
//  This Program is free software; you can redistribute it and/or modify        //
//  it under the terms of the GNU General Public License as published by        //
//  the Free Software Foundation; either version 2, or (at your option)         //
//  any later version.                                                          //
//                                                                              //
//  This Program is distributed in the hope that it will be useful,             //
//  but WITHOUT ANY WARRANTY; without even the implied warranty of              //
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                //
//  GNU General Public License for more details.                                //
//                                                                              //
//  You should have received a copy of the GNU General Public License           //
//  along with GNU Make; see the file COPYING.  If not, write to                //
//  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.       //
//  http://www.gnu.org/copyleft/gpl.html                                        //
//                                                                              //  
//////////////////////////////////////////////////////////////////////////////////

#include <tchar.h>
#include <stdio.h>

#include "SharedMem.h"

static HANDLE shared_handle = INVALID_HANDLE_VALUE;
static SHARED_DATA *shared_mem = NULL;

int createSharedMemory(int processID, int identity)
{
	/*if (shared_mem != NULL)
		return(0);

	shared_mem = (SHARED_DATA*) malloc(shareDataSize);
	if (shared_mem == NULL)
		return(-1);

	memset(shared_mem, 0, sizeof(SHARED_DATA));
	
	for (int index = 0; index < 32; index++)
		shared_mem->pids[index] = -1;

	return (0);*/

	/*SECURITY_ATTRIBUTES SA_ShMem;
	PSECURITY_DESCRIPTOR pSD_ShMem;

	pSD_ShMem = (PSECURITY_DESCRIPTOR)LocalAlloc(LPTR, SECURITY_DESCRIPTOR_MIN_LENGTH);

	if (pSD_ShMem == NULL)
		return -1;
	if (!InitializeSecurityDescriptor(pSD_ShMem, SECURITY_DESCRIPTOR_REVISION))
		return -2;
	if (!SetSecurityDescriptorDacl(pSD_ShMem, TRUE, (PACL)NULL, FALSE))
		return -3;

	SA_ShMem.nLength = sizeof(SA_ShMem);
	SA_ShMem.lpSecurityDescriptor = pSD_ShMem;
	SA_ShMem.bInheritHandle = TRUE;*/

	TCHAR* fullIdentityString = new TCHAR[64];
	_stprintf_s(fullIdentityString, 64, _T("DVBLogic Plugin Shared Memory %d-%d"), processID, identity);

	/*TCHAR szName[] = TEXT("DVBLogic Plugin Shared Memory");*/
	shared_handle = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(SHARED_DATA), fullIdentityString);

	if (shared_handle == NULL)
	{
		return -4;
	}

	shared_mem = (SHARED_DATA*)MapViewOfFile(shared_handle, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(SHARED_DATA));

	if (shared_mem == NULL)
	{
		CloseHandle(shared_handle);
		return -5;
	}

	memset(shared_mem, 0, sizeof(SHARED_DATA));

	return 0;
}

void closeSharedMemory()
{
	/*if (shared_mem != NULL)
		free(shared_mem);

	shared_mem = NULL;*/

	if (shared_mem != NULL)
	{
		UnmapViewOfFile(shared_mem);
	}

	if (shared_handle != NULL)
	{
		CloseHandle(shared_handle);
	}

	shared_mem = NULL;
	shared_handle = INVALID_HANDLE_VALUE;
}

SHARED_DATA* getSharedMemory()
{
	return (shared_mem);
}

void setSharedMemory(LPVOID memoryBuffer)
{
	shared_mem = (SHARED_DATA*)memoryBuffer;
	memset(shared_mem, 0, 72);
}

BOOL isOpen()
{
	return (shared_mem != NULL);
}
