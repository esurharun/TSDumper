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

#include <windows.h>
#include <commdlg.h>
#include <streams.h>
#include <initguid.h>
#include <strsafe.h>

#include "Guids.h"
#include "TSMemSink.h"
#include "SharedMem.h"
#include "shlobj.h"

//
//  Definition of CTSMemSinkPin
//
CTSMemSinkPin::CTSMemSinkPin(CTSMemSinkFilter *pDump,
	LPUNKNOWN pUnk,
    CCritSec *pLock,
    CCritSec *pReceiveLock,
    HRESULT *phr) :

    CRenderedInputPin(NAME("CTSMemSinkPin"),
		pDump,                     // Filter
        pLock,                     // Locking
        phr,                       // Return code
        L"Input"),                 // Pin name
		m_pReceiveLock(pReceiveLock),
		m_pDump(pDump),
		m_tLast(0)
{
}

//
// CheckMediaType
//
// Check if the pin can support this specific proposed type and format
//
HRESULT CTSMemSinkPin::CheckMediaType(const CMediaType *)
{
    return S_OK;
}

//
// BreakConnect
//
// Break a connection
//
HRESULT CTSMemSinkPin::BreakConnect()
{
    if (m_pDump->m_pPosition != NULL)
	{
        m_pDump->m_pPosition->ForceRefresh();
    }

    return CRenderedInputPin::BreakConnect();
}

//
// ReceiveCanBlock
//
// We don't hold up source threads on Receive
//
STDMETHODIMP CTSMemSinkPin::ReceiveCanBlock()
{
    return S_FALSE;
}

//
// Receive
//
// Do something with this media sample
//
STDMETHODIMP CTSMemSinkPin::Receive(IMediaSample *pSample)
{
	CheckPointer(pSample,E_POINTER);

	// get writer lock
    CAutoLock lock(m_pReceiveLock);

    PBYTE pbData;

    REFERENCE_TIME tStart, tStop;
    pSample->GetTime(&tStart, &tStop);

    m_tLast = tStart;

    // Copy the data to the file

    HRESULT hr = pSample->GetPointer(&pbData);
    if (FAILED(hr))
	{
        return hr;
    }

    return m_pDump->Write(pbData, pSample->GetActualDataLength());
}

//
// EndOfStream
//
STDMETHODIMP CTSMemSinkPin::EndOfStream(void)
{
    CAutoLock lock(m_pReceiveLock);
    return CRenderedInputPin::EndOfStream();

} // EndOfStream

//
// NewSegment
//
// Called when we are seeked
//
STDMETHODIMP CTSMemSinkPin::NewSegment(REFERENCE_TIME tStart, REFERENCE_TIME tStop, double dRate)
{
    m_tLast = 0;
    return (S_OK);

} 

//
//  CTSMemSinkFilter class
//
CTSMemSinkFilter::CTSMemSinkFilter(LPUNKNOWN pUnk, HRESULT *phr, bool logging, char* logFileName, bool dumping, char* dumpFileName) :
	CBaseFilter(NAME("PSIMemorySinkFilter"), pUnk, &m_Lock, CLSID_TSMemSinkFilter),
    m_pPin(NULL),
    m_pPosition(NULL),
	m_hFile(INVALID_HANDLE_VALUE),
	m_hDumpFile(INVALID_HANDLE_VALUE),
    writeLogFile(0),
	writeDumpFile(0),
	pidList(),
	syncSearchCount(0),
	samplesDropped(0),
	sampleOffset(0),
	partSample(),
	partSampleSize(0),
	dumpFileSize(0),
	maximumSampleSize(0)

{
	ASSERT(phr);

	writeLogFile = logging;
	writeDumpFile = dumping;
	
	if(writeLogFile == 1)
		OpenLogFile(logFileName);
	if (writeDumpFile == 1)
		OpenDumpFile(dumpFileName);

	m_pPin = new CTSMemSinkPin(this, GetOwner(), &m_Lock, &m_ReceiveLock, phr);
    if (m_pPin == NULL)
	{
        if (phr)
            *phr = E_OUTOFMEMORY;
        return;
    }

	memset(pidList, -1, sizeof(pidList));

	GetLocalTime(&lastData);
}

//
// Destructor
//
CTSMemSinkFilter::~CTSMemSinkFilter()
{
    delete m_pPin;
    delete m_pPosition;

	if(m_hDumpFile != INVALID_HANDLE_VALUE)
	{
		LogString("Destructor called - closing dump file\r\n");
		CloseHandle(m_hDumpFile);
		m_hDumpFile = INVALID_HANDLE_VALUE;
	}
    
	if(m_hFile != INVALID_HANDLE_VALUE)
	{
		LogString("Destructor called - closing log file\r\n");
		CloseHandle(m_hFile);
		m_hFile = INVALID_HANDLE_VALUE;
	}
}

HRESULT CTSMemSinkFilter::OpenLogFile(char* fileName)
{
	m_hFile = CreateFile(fileName,	// The filename
		GENERIC_WRITE,				// File access
        FILE_SHARE_READ,			// Share access
        NULL,						// Security
        CREATE_ALWAYS,				// Open flags
        (DWORD) 0,					// More flags
        NULL);						// Template
    if (m_hFile == INVALID_HANDLE_VALUE) 
		return (S_FALSE);

	return (S_OK);
}

HRESULT CTSMemSinkFilter::OpenDumpFile(char* fileName)
{
	m_hDumpFile = CreateFile(fileName,	// The filename
		GENERIC_WRITE,				// File access
        FILE_SHARE_READ,			// Share access
        NULL,						// Security
        CREATE_ALWAYS,				// Open flags
        (DWORD) 0,					// More flags
        NULL);						// Template
    if (m_hDumpFile == INVALID_HANDLE_VALUE) 
		return (S_FALSE);

	return (S_OK);
}

//
// GetPin
//
CBasePin * CTSMemSinkFilter::GetPin(int n)
{
    if (n == 0)
		return (m_pPin);
    else
		return (NULL);
}

//
// GetPinCount
//
int CTSMemSinkFilter::GetPinCount()
{
    return (1);
}

//
// Stop
//
// Overriden to close the memory buffer
//
STDMETHODIMP CTSMemSinkFilter::Stop()
{
    CAutoLock cObjectLock(&m_Lock);

	LogString("Stop called - closing memory buffer\r\n");
	closeSharedMemory();

	if(m_hDumpFile != INVALID_HANDLE_VALUE)
	{
		LogString("Stop called - closing dump file\r\n");
		CloseHandle(m_hDumpFile);
		m_hDumpFile = INVALID_HANDLE_VALUE;
	}

	LogString("Filter Stopped\r\n");	

    return CBaseFilter::Stop();
}

//
// Pause
//
// Overriden to log event.
//
STDMETHODIMP CTSMemSinkFilter::Pause()
{
	CAutoLock cObjectLock(&m_Lock);

	LogString("Filter Paused\r\n");

    return CBaseFilter::Pause();
}

//
// Run
//
// Overriden to create memory buffer
//
STDMETHODIMP CTSMemSinkFilter::Run(REFERENCE_TIME tStart)
{
    CAutoLock cObjectLock(&m_Lock);

	if(isOpen() == false)
	{
		LogString("Creating memory buffer: size (%d) bytes\r\n", shareDataSize);

		int createResult = createSharedMemory();

		if(createResult != 0)
			LogString("Error creating memory buffer\r\n");
		else
			LogString("Memory buffer created\r\n");
	}
	else
	{
		LogString("Memory buffer already exists\r\n");
	}

	LogString("Filter Running\r\n");

    return CBaseFilter::Run(tStart);
}

//
// CreateInstance
//
// Provide the way for COM to create a dump filter
//
CUnknown * WINAPI CTSMemSinkFilter::CreateInstance(LPUNKNOWN punk, HRESULT *phr)
{
	ASSERT(phr);
    
    CTSMemSinkFilter *pNewObject = new CTSMemSinkFilter(punk, phr, false, "PSI Memory Filter.log", false, "PSI Memory Dump.ts");
    if (pNewObject == NULL)
	{
        if (phr)
		{
            *phr = E_OUTOFMEMORY;
		}
    }
	
    return pNewObject;

} 

//
// NonDelegatingQueryInterface
//
// Override this to say what interfaces we support where
//
STDMETHODIMP CTSMemSinkFilter::NonDelegatingQueryInterface(REFIID riid, void ** ppv)
{
    CheckPointer(ppv,E_POINTER);
    CAutoLock lock(&m_Lock);

    // Do we have this interface
    if (riid == IID_IMemSinkSettings)
	{
        return GetInterface((IMemSinkSettings *) this, ppv);
	}
    else if (riid == IID_IMediaPosition || riid == IID_IMediaSeeking)
	{
        if (m_pPosition == NULL) 
        {

            HRESULT hr = S_OK;
            m_pPosition = new CPosPassThru(NAME("Dump Pass Through"),
                                           (IUnknown *) GetOwner(),
                                           (HRESULT *) &hr, m_pPin);
            if (m_pPosition == NULL) 
                return E_OUTOFMEMORY;

            if (FAILED(hr)) 
            {
                delete m_pPosition;
                m_pPosition = NULL;
                return hr;
            }
        }

        return m_pPosition->NonDelegatingQueryInterface(riid, ppv);
    } 


    return CBaseFilter::NonDelegatingQueryInterface(riid, ppv);

} // NonDelegatingQueryInterface

STDMETHODIMP CTSMemSinkFilter::get_IsDataFlowing(BOOL *dataFlowing)
{
	const __int64 nano100SecInSec=(__int64)10000000; 

	SYSTEMTIME now;
	GetLocalTime(&now);

	FILETIME lastWrittenFT;
	FILETIME nowTF;

	SystemTimeToFileTime(&lastData, &lastWrittenFT);
	SystemTimeToFileTime(&now, &nowTF);

	__int64 *lastWrittenPT = (__int64*)&lastWrittenFT;
	__int64 *nowPT = (__int64*)&nowTF;

	__int64 diff = (*nowPT) - (*lastWrittenPT);
	__int64 timeAllowed = ((__int64) 15 * nano100SecInSec);

	if(diff > timeAllowed)
	{
		LogString("No data flowing detected (%d) (%d)!\r\n", (int)diff, (int)timeAllowed);
		*dataFlowing = FALSE;
	}
	else
	{
		LogString("Data flowing (%d) (%d)\r\n", (int)diff, (int)timeAllowed);
		*dataFlowing = TRUE;
	}

	return NOERROR;
}

STDMETHODIMP CTSMemSinkFilter::clear()
{
	LogString("Clear memory buffer requested\r\n");

	CAutoLock cObjectLock(&m_Lock);

	if(isOpen() == FALSE)
	{
		LogString("Memory buffer not open: ignoring clear request\r\n");
		return S_OK;
	}

	SHARED_DATA *data  = getSharedMemory();
	data->currentPointer = 0;

	partSampleSize = 0;

	LogString("Memory buffer cleared\r\n");

	return NOERROR;
}

STDMETHODIMP CTSMemSinkFilter::clearPIDs()
{
	LogString("Clear pid mapping requested\r\n");

	CAutoLock cObjectLock(&m_Lock);

	pidList[0] = -1;

	LogString("Pid mapping cleared\r\n");

	return NOERROR;
}

STDMETHODIMP CTSMemSinkFilter::mapPID(int pid)
{
	LogString("Mapping PID (%d)\r\n", pid);

	CAutoLock cObjectLock(&m_Lock);

	for (int index = 0; index < sizeof(pidList); index++)
	{
		if (pidList[index] == -1)
		{
			pidList[index] = pid;
			if (index + 1 < sizeof(pidList))
				pidList[index + 1] = -1;
			LogString("Pid mapped\r\n");
			return NOERROR;
		}
	}

	LogString("Pid map full\r\n");
	return ERROR;
}

STDMETHODIMP CTSMemSinkFilter::get_BufferUsed(int *bufferUsed)
{
	CAutoLock cObjectLock(&m_Lock);

	if(isOpen() == FALSE)
	{
		*bufferUsed = -1;
		return S_OK;
	}

	SHARED_DATA *data  = getSharedMemory();
	*bufferUsed = data->currentPointer;

	return S_OK;
}

STDMETHODIMP CTSMemSinkFilter::get_BufferAddress(LPVOID *bufferAddress)
{
	CAutoLock cObjectLock(&m_Lock);

	if(isOpen() == FALSE)
		return S_FALSE;

	SHARED_DATA *data  = getSharedMemory();

	*bufferAddress = data;

	return S_OK;
}

STDMETHODIMP CTSMemSinkFilter::get_SyncSearchCount(int *searchCount)
{
	*searchCount = syncSearchCount;
	return S_OK;
}

STDMETHODIMP CTSMemSinkFilter::get_SamplesDropped(int *dropped)
{
	*dropped = samplesDropped;
	return S_OK;
}

STDMETHODIMP CTSMemSinkFilter::get_DumpFileSize(int *fileSize)
{
	*fileSize = dumpFileSize;
	return S_OK;
}

STDMETHODIMP CTSMemSinkFilter::get_MaximumSampleSize(int *maxSampleSize)
{
	*maxSampleSize = maximumSampleSize;
	return S_OK;
}

HRESULT CTSMemSinkFilter::LogString(char *sz,...)
{
    // If the file has already been closed, don't continue
    if (m_hFile == INVALID_HANDLE_VALUE)
	{
        return S_FALSE;
    }

	char mainBuff[300];
    char vaBuff[256];
	
    va_list va;
    va_start(va, sz);
	StringCchVPrintf(vaBuff, 256, sz, va);
    va_end(va);

    SYSTEMTIME st;
    GetLocalTime(&st);
	StringCchPrintf(mainBuff, 300, "%.4d/%.2d/%.2d %.2d:%.2d:%.2d.%.3d - %s", st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond, st.wMilliseconds, vaBuff);

	DWORD dwWritten;

	WriteFile(m_hFile, (PVOID)mainBuff, (DWORD)strlen(mainBuff), &dwWritten, NULL);
	
	return S_OK;
}

//
// Write
//
// Write raw data to the memory buffer
//
HRESULT CTSMemSinkFilter::Write(PBYTE pbData, LONG lDataLength)
{	
	// get state change lock
	CAutoLock cObjectLock(&m_Lock);

	if (lDataLength == 0)
		return(S_OK);

	LogString("Sample size: (%d)\r\n", lDataLength);

	if (lDataLength > maximumSampleSize)
		maximumSampleSize = lDataLength;

	SHARED_DATA *data  = getSharedMemory();
	if (data == NULL)
	{
		LogString("Memory buffer not open: block dropped\r\n");
		return S_OK;
	}

	if (pidList[0] == -1)
	{
		LogString("No PID filtering requested - block output unscanned\r\n");

		if (m_hDumpFile != INVALID_HANDLE_VALUE)
		{
			DWORD dwWritten;
			WriteFile(m_hDumpFile, (PVOID)pbData, lDataLength, &dwWritten, NULL);
			dumpFileSize+= dwWritten;
			return S_OK;
		}

		if((data->currentPointer + lDataLength) < shareDataSize)
		{
			memcpy((void*)(data->data + data->currentPointer), pbData, lDataLength);
			data->currentPointer += lDataLength;
			return S_OK;
		}
		else
		{
			LogString("Memory buffer full: complete block dropped\r\n");
			return S_OK;
		}
	}

	int processedCount = 0;
	int bufferPointer = 0;	

	if (partSampleSize != 0)
	{
		LogString("Sample broken over blocks\r\n");

		if (m_hDumpFile != INVALID_HANDLE_VALUE)
		{
			DWORD dwWritten;
			WriteFile(m_hDumpFile, (PVOID)partSample, partSampleSize, &dwWritten, NULL);
			dumpFileSize+= dwWritten;
			WriteFile(m_hDumpFile, (PVOID)pbData, 188 - partSampleSize, &dwWritten, NULL);
			dumpFileSize+= dwWritten;
		}
			
		if((data->currentPointer + 188) < shareDataSize)
		{
			LogString("Storing first part %d bytes at offset %d\r\n", partSampleSize, data->currentPointer);
			memcpy((void*)(data->data + data->currentPointer), partSample, partSampleSize);
			data->currentPointer += partSampleSize;
			LogString("Storing second part %d bytes at offset %d\r\n", 188 - partSampleSize, data->currentPointer);
			memcpy((void*)(data->data + data->currentPointer), pbData, 188 - partSampleSize);
			data->currentPointer += 188 - partSampleSize;
			bufferPointer = 188 - partSampleSize;
		}
		else
		{
			LogString("Memory buffer full: remainder of block dropped\r\n");
			return S_OK;
		}
	}

	for (; bufferPointer <= lDataLength - 188; bufferPointer += 188)
	{
		LogString("Pointer and length: %d %d\r\n", bufferPointer, lDataLength);
		
		if (*(pbData + bufferPointer) == 0x47)
		{		
			int pid = (((*(pbData + bufferPointer + 1)) & 0x1f) * 256)  + *(pbData + bufferPointer + 2);

			bool found = false;
		
			if (pidList[0] != -1)
			{
				/*LogString("Searching pid list for PID (%d)\r\n", pid);*/

				bool done = false;			

				for (int index = 0; index < sizeof(pidList) && !done; index++)
				{
					if (pidList[index] == -1)
						done = true;
					else
					{
						if (pidList[index] == pid)
						{
							found = true;
							done = true;
						}
					}
				}
			}
			else
				found = true;

			if (found)
			{
				if (m_hDumpFile != INVALID_HANDLE_VALUE)
				{
					DWORD dwWritten;
					WriteFile(m_hDumpFile, (PVOID)(pbData + bufferPointer), 188, &dwWritten, NULL);
					dumpFileSize+= dwWritten;
				}

				if((data->currentPointer + 188) < shareDataSize)
				{
					memcpy((void*)(data->data + data->currentPointer), pbData + bufferPointer, 188);
					data->currentPointer += 188;
				}
				else
				{
					LogString("Memory buffer full: remainder of block dropped\r\n");
					return S_OK;
				}
		
				/*LogString("Sample written: offset %d size %d bytes\r\n", bufferPointer, 188);*/
				GetLocalTime(&lastData);

				processedCount++;		
			}
		}
		else
		{
			LogString("Looking for next sync byte\r\n");
			syncSearchCount++;

			sampleOffset = 0;

			for (int index = 0; index < 188 && (bufferPointer + index < lDataLength && sampleOffset == 0); index++)
			{
				if (*(pbData + bufferPointer + index) == 0x47)
				{
					if (bufferPointer + index + 188 + 188 < lDataLength)
					{
						if (*(pbData + bufferPointer + index + 188) == 0x47)
						{
							LogString("Next sync byte at: %d\r\n", (bufferPointer + index));
							sampleOffset = bufferPointer + index;
							bufferPointer = bufferPointer + index - 188;														
						}
					}
				}
			}

			if (sampleOffset == 0)
			{
				LogString("Rest of block dropped (sync byte missing): %d\r\n", (lDataLength / 188) - processedCount);
				samplesDropped++;
				partSampleSize = 0;
				return S_OK;
			}
		}
	}

	if (bufferPointer != lDataLength)
	{
		partSampleSize = lDataLength - bufferPointer;
		LogString("Storing part sample: size %d offset %d\r\n", partSampleSize, bufferPointer);
		memcpy(partSample, pbData + bufferPointer, partSampleSize);		
	}
	else
	{
		LogString("No part sample\r\n");
		partSampleSize = 0;
	}

	return S_OK;
}

extern "C" {
	__declspec(dllexport) HRESULT __cdecl CreatePSIMemoryFilter(IGraphBuilder *graphBuilder, bool logging, char* logFileName, bool dumping, char* dumpFileName)
	{
		CTSMemSinkFilter *m_pSink = new CTSMemSinkFilter(NULL, NULL, logging, logFileName, dumping, dumpFileName);
		m_pSink->AddRef();

		IBaseFilter* pSinkFilter;
		HRESULT hr = m_pSink->QueryInterface(IID_IBaseFilter, (void**)&pSinkFilter);
		if (SUCCEEDED(hr)) 
		{
			hr = graphBuilder->AddFilter(pSinkFilter, L"PSI Memory Filter");
			pSinkFilter->Release();	    // corresponds to QueryInterface
		}

		return(S_OK);
	}
}



