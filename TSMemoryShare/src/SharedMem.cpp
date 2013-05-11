////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2013 nzsjb                                           //
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

#include "SharedMem.h"

static int sharedDataSize = 0;
static SHARED_DATA *shared_mem = NULL;

int createSharedMemory(int bufferSize)
{
	sharedDataSize = bufferSize * 1024 * 1024;

	shared_mem = (SHARED_DATA*) malloc(sharedDataSize);
	if (shared_mem == NULL)
		return(-1);

	memset(shared_mem, 0, sharedDataSize);

	return (0);
}

void closeSharedMemory()
{
	if (shared_mem != NULL)
		free(shared_mem);

	shared_mem = NULL;
}

SHARED_DATA* getSharedMemory()
{
	return (shared_mem);
}

void setSharedMemory(LPVOID memoryBuffer)
{
	shared_mem = (SHARED_DATA*)memoryBuffer;
	memset(shared_mem, 0, 12);
}

BOOL isOpen()
{
	return (shared_mem != NULL);
}

int getSharedDataSize()
{
	return(sharedDataSize);
}
