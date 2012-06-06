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

#include "SharedMem.h"

static HANDLE shared_handle = INVALID_HANDLE_VALUE;
static SHARED_DATA *shared_mem = NULL;

int createSharedMemory(char *pShareName)
{
	SECURITY_ATTRIBUTES SA_ShMem;
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
	SA_ShMem.bInheritHandle = TRUE;

	shared_handle = CreateFileMapping(INVALID_HANDLE_VALUE, &SA_ShMem, PAGE_READWRITE, 0, sizeof(SHARED_DATA), pShareName);

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

int openSharedMemory(char *pShareName)
{
   shared_handle = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, pShareName);

   if (shared_handle == NULL) 
   { 
      return -1;
   } 

   shared_mem = (SHARED_DATA*)MapViewOfFile(shared_handle, FILE_MAP_ALL_ACCESS,  0, 0, sizeof(SHARED_DATA)); 

   if (shared_mem == NULL) 
   { 
      return -2;
   }

   return 0;
}

void closeSharedMemory()
{
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
	return shared_mem;
}

BOOL isOpen()
{
	if(shared_mem != NULL && shared_handle != INVALID_HANDLE_VALUE)
		return TRUE;
	else
		return FALSE;

}
