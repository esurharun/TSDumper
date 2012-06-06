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
                  pDump,                   // Filter
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
STDMETHODIMP CTSMemSinkPin::NewSegment(REFERENCE_TIME tStart,
                                       REFERENCE_TIME tStop,
                                       double dRate)
{
    m_tLast = 0;
    return S_OK;

} // NewSegment

//
//  CTSMemSinkFilter class
//
CTSMemSinkFilter::CTSMemSinkFilter(LPUNKNOWN pUnk, HRESULT *phr) :
	CBaseFilter(NAME("CTSMemSinkFilterFilter"), pUnk, &m_Lock, CLSID_TSMemSinkFilter),
    m_pPin(NULL),
    m_pPosition(NULL),
	m_hFile(INVALID_HANDLE_VALUE),
    m_pShareName(0),
	writeLogFile(0)
{
	ASSERT(phr);

	// get the overrides from the register
	CRegStore *store = new CRegStore("SOFTWARE\\TVSchedulerPro\\TSMemoryShare");
	writeLogFile = store->getInt("writeLog", 0);
	delete store;

	if(writeLogFile == 1)
	{
		OpenLogFile("TSMemSink-");
	}

	// copy base memory share name so if no share name is set we have something to work with
	m_pShareName = new WCHAR[256];
	StringCchCopyW(m_pShareName, 255, L"Global\\$Capture01SHARED$");

    m_pPin = new CTSMemSinkPin(	this,
								GetOwner(),
								&m_Lock,
								&m_ReceiveLock,
								phr);
    if (m_pPin == NULL)
	{
        if (phr)
            *phr = E_OUTOFMEMORY;
        return;
    }

	GetLocalTime(&lastData);
}

// Destructor
CTSMemSinkFilter::~CTSMemSinkFilter()
{
    delete m_pPin;
    delete m_pPosition;
    delete m_pShareName;

	if(m_hFile != INVALID_HANDLE_VALUE)
	{
		CloseHandle(m_hFile);
		m_hFile = INVALID_HANDLE_VALUE;
	}
}

HRESULT CTSMemSinkFilter::OpenLogFile(char *logFile)
{
	char logPath[MAX_PATH] = {0};
	char buffer[MAX_PATH] = {0};
	
	/*
	HRESULT hr = S_FALSE;
	hr = SHGetFolderPath(NULL, CSIDL_COMMON_APPDATA, NULL, 0, buffer);
	if (SUCCEEDED(hr))
	{
		StringCchCat(logPath, MAX_PATH, buffer);
		StringCchCat(logPath, MAX_PATH, "\\Blue Bit Solutions\\TV Scheduler Pro\\log");
		SHCreateDirectoryEx(NULL, logPath, NULL);
	}
	*/

	strcpy_s(logPath, MAX_PATH, ".\\log");
	SHCreateDirectoryEx(NULL, logPath, NULL);

    SYSTEMTIME st;
    GetLocalTime(&st);
	StringCchPrintf(buffer, MAX_PATH, "\\%s%.4d%.2d%.2d-%.2d%.2d%.2d-%.3d.log", logFile, st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond, st.wMilliseconds);
	StringCchCat(logPath, MAX_PATH, buffer);

    // Try to open the log file
    m_hFile = CreateFile((LPCTSTR) logPath,   // The filename
                         GENERIC_WRITE,         // File access
                         FILE_SHARE_READ,       // Share access
                         NULL,                  // Security
                         CREATE_ALWAYS,         // Open flags
                         (DWORD) 0,             // More flags
                         NULL);                 // Template

    if (m_hFile == INVALID_HANDLE_VALUE) 
    {
		//MessageBox(NULL, "Could not create log file", "Error!", MB_OK);
		return S_FALSE;
    }

	return S_OK;
}

//
// GetPin
//
CBasePin * CTSMemSinkFilter::GetPin(int n)
{
    if (n == 0)
	{
        return m_pPin;
    } else
	{
        return NULL;
    }
}

//
// GetPinCount
//
int CTSMemSinkFilter::GetPinCount()
{
    return 1;
}

//
// Stop
//
// Overriden to close the dump file
//
STDMETHODIMP CTSMemSinkFilter::Stop()
{
    CAutoLock cObjectLock(&m_Lock);

	LogString("Closing shared memory\r\n");

	closeSharedMemory();
	
	LogString("Filter Stopped\r\n");

    return CBaseFilter::Stop();
}

//
// Pause
//
// Overriden to open the dump file
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
// Overriden to open the dump file
//
STDMETHODIMP CTSMemSinkFilter::Run(REFERENCE_TIME tStart)
{
    CAutoLock cObjectLock(&m_Lock);

	if(isOpen() == false)
	{
		char convertedShareName[MAX_PATH];
		if(!WideCharToMultiByte(CP_ACP, 0, m_pShareName, -1, convertedShareName, MAX_PATH, 0, 0))
		{
			LogString("Shared memory name not valid.\r\n");
			return ERROR_INVALID_NAME;
		}

		LogString("Creating shared memory name:(%s) size:(%d)\r\n", convertedShareName, shareDataSize);

		int createResult = createSharedMemory(convertedShareName);

		if(createResult != 0)
		{
			LogString("Error creating shared memory\r\n");
		}
	}
	else
	{
		LogString("Shared memory already open\r\n");
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
    
    CTSMemSinkFilter *pNewObject = new CTSMemSinkFilter(punk, phr);
    if (pNewObject == NULL)
	{
        if (phr)
		{
            *phr = E_OUTOFMEMORY;
		}
    }

    return pNewObject;

} // CreateInstance

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

STDMETHODIMP CTSMemSinkFilter::set_ShareName(LPCOLESTR pszShareName)
{
    CheckPointer(pszShareName, E_POINTER);
    if(wcslen(pszShareName) > MAX_PATH)
        return ERROR_FILENAME_EXCED_RANGE;

    // Take a copy of the filename
    size_t len = lstrlenW(pszShareName) + 25;
    m_pShareName = new WCHAR[len];
    if (m_pShareName == 0)
        return E_OUTOFMEMORY;

	StringCchPrintfW(m_pShareName, len, L"%s", pszShareName);

	return NOERROR;
}

STDMETHODIMP CTSMemSinkFilter::get_IsDataFlowing(BOOL *dataFlowing)
{
	/*
	const __int64 nano100SecInWeek=(__int64)10000000*60*60*24*7;
	const __int64 nano100SecInDay=(__int64)10000000*60*60*24;
	const __int64 nano100SecInHour=(__int64)10000000*60*60;
	const __int64 nano100SecInMin=(__int64)10000000*60;
	*/
	const __int64 nano100SecInSec=(__int64)10000000; 

	SYSTEMTIME now;
	GetLocalTime(&now);

	FILETIME lastWrittenFT;
	FILETIME nowTF;

	SystemTimeToFileTime(&lastData, &lastWrittenFT);
	SystemTimeToFileTime(&now, &nowTF);

	__int64 *lastWrittenPT = (__int64*)&lastWrittenFT;
	__int64 *nowPT = (__int64*)&nowTF;

	// take 10 sec off our now time
	//(*nowPT) -= (__int64) 15 * nano100SecInSec;

	__int64 diff = (*nowPT) - (*lastWrittenPT);
	__int64 timeAllowed = ((__int64) 15 * nano100SecInSec);

	if(diff > timeAllowed)
	{
		LogString("No data flowing detected (%d) (%d)!\r\n", (int)diff, (int)timeAllowed);
		*dataFlowing = FALSE;
	}
	else
	{
		LogString("Data flowing (%d) (%d)!\r\n", (int)diff, (int)timeAllowed);
		*dataFlowing = TRUE;
	}

	return NOERROR;
}

STDMETHODIMP CTSMemSinkFilter::set_SignalData(long quality, long strength)
{
	// get state change lock
	CAutoLock cObjectLock(&m_Lock);

	if(isOpen())
	{
		SHARED_DATA *data  = getSharedMemory();
		data->signalQuality = quality;
		data->signalStrength = strength;
		return S_OK;
	}

	return E_FAIL;
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
// Write raw data to the file
//
HRESULT CTSMemSinkFilter::Write(PBYTE pbData, LONG lDataLength)
{
	// get state change lock
	CAutoLock cObjectLock(&m_Lock);

	if(isOpen() == FALSE)
	{
		LogString("Share not open: dropping data\r\n");
		//MessageBox(NULL, "Share not open: dropping data", "TS Sink : Error!", MB_OK);
		return S_OK;
	}

	SHARED_DATA *data  = getSharedMemory();

	if((data->currentPointer + lDataLength) < shareDataSize)
	{
		memcpy((void*)(data->data + data->currentPointer), pbData, lDataLength);
		data->currentPointer += lDataLength;
	}
	else
	{
		int firstCopy = shareDataSize - data->currentPointer;
		memcpy((void*)(data->data + data->currentPointer), pbData, firstCopy);

		// now start from the begining again
		int dataLeft = lDataLength - firstCopy;
		memcpy((void*)(data->data), (pbData + firstCopy), dataLeft);

		data->currentPointer = dataLeft;
		data->loopCount++;

		LogString("Share Full: Rolling over (%d)\r\n", data->loopCount);
	}

	if(lDataLength > 0)
	{
		GetLocalTime(&lastData);
	}

    return S_OK;
}

