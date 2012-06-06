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

#include <streams.h>

#include "TSMemSource.h"
#include "Guids.h"
#include "SharedMem.h"
#include "TSMemSourceInterface.h"

#include "shlobj.h"

//
// CTSMemSourcePin Class
//

CTSMemSourcePin::CTSMemSourcePin(HRESULT *phr, CSource *pF)
        : CSourceStream(NAME("Push Source Desktop"), phr, pF, L"Out")
{
	DbgLog((LOG_TRACE, 3, TEXT("Filter Loaded")));
}

CTSMemSourcePin::~CTSMemSourcePin()
{   
    DbgLog((LOG_TRACE, 3, TEXT("Data written %d")));
}

//
// GetMediaType
//
HRESULT CTSMemSourcePin::GetMediaType(int iPosition, CMediaType *pmt)
{
    CheckPointer(pmt,E_POINTER);
    CAutoLock cAutoLock(m_pFilter->pStateLock());

	pmt->InitMediaType();
	pmt->SetType      (& MEDIATYPE_Stream);
	pmt->SetSubtype   (& MEDIASUBTYPE_MPEG2_TRANSPORT);

    return NOERROR;
}

//
// CheckMediaType
//
HRESULT CTSMemSourcePin::CheckMediaType(const CMediaType *pMediaType)
{
    CheckPointer(pMediaType,E_POINTER);

	if(MEDIATYPE_Stream == pMediaType->majortype)
	{
		if (MEDIASUBTYPE_MPEG2_TRANSPORT == pMediaType->subtype)
			return S_OK;
	}

	return S_FALSE;
}

//
// DecideBufferSize
//
// This will always be called after the format has been sucessfully
// negotiated. So we have a look at m_mt to see what size image we agreed.
// Then we can ask for buffers of the correct size to contain them.
//
HRESULT CTSMemSourcePin::DecideBufferSize(IMemAllocator *pAlloc,
                                      ALLOCATOR_PROPERTIES *pProperties)
{
    CheckPointer(pAlloc,E_POINTER);
    CheckPointer(pProperties,E_POINTER);

    CAutoLock cAutoLock(m_pFilter->pStateLock());
    HRESULT hr = NOERROR;

    pProperties->cBuffers = 1;
    pProperties->cbBuffer = 188 * 16;

    ASSERT(pProperties->cbBuffer);

    // Ask the allocator to reserve us some sample memory. NOTE: the function
    // can succeed (return NOERROR) but still not have allocated the
    // memory that we requested, so we must check we got whatever we wanted.
    ALLOCATOR_PROPERTIES Actual;
    hr = pAlloc->SetProperties(pProperties, &Actual);
    if(FAILED(hr))
    {
        return hr;
    }

    // Is this allocator unsuitable?
    if(Actual.cbBuffer < pProperties->cbBuffer)
    {
        return E_FAIL;
    }

    return NOERROR;

} // DecideBufferSize

// FillBuffer is called once for every sample in the stream.
HRESULT CTSMemSourcePin::FillBuffer(IMediaSample *pSample)
{
	return ((CTSMemSourceFilter*)m_pFilter)->FillBuffer(pSample);
}

HRESULT CTSMemSourcePin::Set(REFGUID guidPropSet, DWORD dwID, void *pInstanceData, DWORD cbInstanceData, void *pPropData, DWORD cbPropData)
{
    return E_NOTIMPL;
}

// QuerySupported: Query whether the pin supports the specified property.
HRESULT CTSMemSourcePin::QuerySupported(REFGUID guidPropSet, DWORD dwPropID, DWORD *pTypeSupport)
{
    if (guidPropSet != AMPROPSETID_Pin)
        return E_PROP_SET_UNSUPPORTED;
    if (dwPropID != AMPROPERTY_PIN_CATEGORY)
        return E_PROP_ID_UNSUPPORTED;
    if (pTypeSupport)
        // We support getting this property, but not setting it.
        *pTypeSupport = KSPROPERTY_SUPPORT_GET; 
    return S_OK;
}

// Get: Return the pin category (our only property). 
HRESULT CTSMemSourcePin::Get(
    REFGUID guidPropSet,   // Which property set.
    DWORD dwPropID,        // Which property in that set.
    void *pInstanceData,   // Instance data (ignore).
    DWORD cbInstanceData,  // Size of the instance data (ignore).
    void *pPropData,       // Buffer to receive the property data.
    DWORD cbPropData,      // Size of the buffer.
    DWORD *pcbReturned     // Return the size of the property.
)
{
    if (guidPropSet != AMPROPSETID_Pin) 
        return E_PROP_SET_UNSUPPORTED;
    if (dwPropID != AMPROPERTY_PIN_CATEGORY)
        return E_PROP_ID_UNSUPPORTED;
    if (pPropData == NULL && pcbReturned == NULL)
        return E_POINTER;
    if (pcbReturned)
        *pcbReturned = sizeof(GUID);
    if (pPropData == NULL)  // Caller just wants to know the size.
        return S_OK;
    if (cbPropData < sizeof(GUID)) // The buffer is too small.
        return E_UNEXPECTED;

    *(GUID *)pPropData = PIN_CATEGORY_CAPTURE;
    return S_OK;
}

//IAMPushSource
STDMETHODIMP CTSMemSourcePin::GetPushSourceFlags(ULONG *pFlags)
{
	if(pFlags == NULL)
		return E_POINTER;

	//*pFlags = AM_PUSHSOURCECAPS_INTERNAL_RM;
	*pFlags = AM_PUSHSOURCECAPS_NOT_LIVE;
	return S_OK;
}

STDMETHODIMP CTSMemSourcePin::SetPushSourceFlags(ULONG Flags)
{
	return E_NOTIMPL;
}
        
STDMETHODIMP CTSMemSourcePin::SetStreamOffset(REFERENCE_TIME rtOffset)
{
	return E_NOTIMPL;
}
        
STDMETHODIMP CTSMemSourcePin::GetStreamOffset(REFERENCE_TIME *prtOffset)
{
	return E_NOTIMPL;
}
        
STDMETHODIMP CTSMemSourcePin::GetMaxStreamOffset(REFERENCE_TIME *prtMaxOffset)
{
	return E_NOTIMPL;
}
        
STDMETHODIMP CTSMemSourcePin::SetMaxStreamOffset(REFERENCE_TIME rtMaxOffset)
{
	return E_NOTIMPL;
}
        
STDMETHODIMP CTSMemSourcePin::GetLatency(REFERENCE_TIME *prtLatency)
{
	return E_NOTIMPL;
}//IAMPushSource

//
// NonDelegatingQueryInterface
//
// Override this to say what interfaces we support where
//
STDMETHODIMP CTSMemSourcePin::NonDelegatingQueryInterface(REFIID riid, void ** ppv)
{
	CAutoLock cAutoLock(m_pFilter->pStateLock());
    CheckPointer(ppv,E_POINTER);

    // Do we have this interface
    if (riid == IID_IKsPropertySet)
	{
        return GetInterface((IKsPropertySet *) this, ppv);
	}
	else if (riid == IID_IAMPushSource)
	{
		return GetInterface((IAMPushSource*) this, ppv);
	}

    return CSourceStream::NonDelegatingQueryInterface(riid, ppv);

} // NonDelegatingQueryInterface


/**********************************************
 *
 *  CTSMemSourceFilter Class
 *
 **********************************************/

CTSMemSourceFilter::CTSMemSourceFilter(IUnknown *pUnk, HRESULT *phr)
			: CSource(NAME("TSMemoryShareFilter"), pUnk, CLSID_TSMemSourceFilter),
			m_pShareName(0),
			m_fileCurrentPos(-1),
			m_loopCount(-1),
			m_hFile(INVALID_HANDLE_VALUE),
			dataFlowing(FALSE),
			fallBehindCount(0),
			writeLogFile(0)
{

	// get the overrides from the register
	CRegStore *store = new CRegStore("SOFTWARE\\TVSchedulerPro\\TSMemoryShare");
	writeLogFile = store->getInt("writeLog", 0);
	delete store;

	if(writeLogFile == 1)
	{
		OpenLogFile("TSMemSource-");
	}

	// copy base memory share name so if no share name is set we have something to work with
	m_pShareName = new WCHAR[256];
	StringCchCopyW(m_pShareName, 255, L"Global\\$Capture01SHARED$");

    // The pin magically adds itself to our pin array.
    m_pPin = new CTSMemSourcePin(phr, this);

	if (phr)
	{
		if (m_pPin == NULL)
			*phr = E_OUTOFMEMORY;
		else
			*phr = S_OK;
	}  
}


CTSMemSourceFilter::~CTSMemSourceFilter()
{
    delete m_pPin;
	delete m_pShareName;

	if(m_hFile != INVALID_HANDLE_VALUE)
	{
		CloseHandle(m_hFile);
		m_hFile = INVALID_HANDLE_VALUE;
	}
}

STDMETHODIMP CTSMemSourceFilter::set_ShareName(LPCOLESTR pszShareName)
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

STDMETHODIMP CTSMemSourceFilter::get_IsDataFlowing(BOOL *isDataFlowing)
{
	*isDataFlowing = dataFlowing;

	return NOERROR;
}

STDMETHODIMP CTSMemSourceFilter::get_SignalData(long *quality, long *strength)
{
	if(isOpen())
	{
		SHARED_DATA *data = getSharedMemory();

		*quality = data->signalQuality;
		*strength = data->signalStrength;

		return S_OK;
	}

	return E_FAIL;
}

STDMETHODIMP CTSMemSourceFilter::get_FallBehindCount(int *count)
{
	*count = fallBehindCount;
	return S_OK;
}

// get pin count
int CTSMemSourceFilter::GetPinCount()
{
    return 1;
}

//
// GetPin
//
CBasePin* CTSMemSourceFilter::GetPin(int n)
{
    if(n == 0)
	{
        return m_pPin;
    }
	else
	{
        return NULL;
    }
}

//
// Stop
//
// Overriden to close the dump file
//
STDMETHODIMP CTSMemSourceFilter::Stop()
{
	CAutoLock cAutoLockShared(&m_Lock);

	LogString("Closing shared memory\r\n");

	closeSharedMemory();

	LogString("Filter Stopped\r\n");

	return CSource::Stop();
}

//
// Pause
//
// Overriden to open the dump file
//
STDMETHODIMP CTSMemSourceFilter::Pause()
{
	CAutoLock cAutoLockShared(&m_Lock);

	if(isOpen() == FALSE)
	{
		char convertedShareName[MAX_PATH];
		if(!WideCharToMultiByte(CP_ACP, 0, m_pShareName, -1, convertedShareName, MAX_PATH, 0, 0))
		{
			LogString("Shared memory name not valid.\r\n");
			return ERROR_INVALID_NAME;
		}

		LogString("Opening shared memory (%s)\r\n", convertedShareName);

		int openResult = openSharedMemory(convertedShareName);

		if(openResult != 0)
		{
			LogString("Could not open shared memory.\r\n");
			return S_FALSE;
		}
		dataFlowing = TRUE;
	}
	else
	{
		LogString("Shared memory already open\r\n");
	}

	LogString("Filter Paused\r\n");

    return CSource::Pause();
}

//
// Run
//
// Overriden to open the dump file
//
STDMETHODIMP CTSMemSourceFilter::Run(REFERENCE_TIME tStart)
{
	CAutoLock cAutoLockShared(&m_Lock);

	if(isOpen() == FALSE)
	{
		char convertedShareName[MAX_PATH];
		if(!WideCharToMultiByte(CP_ACP, 0, m_pShareName, -1, convertedShareName, MAX_PATH, 0, 0))
		{
			LogString("Shared memory name not valid.\r\n");
			return ERROR_INVALID_NAME;
		}

		LogString("Opening shared memory (%s)\r\n", convertedShareName);

		int openResult = openSharedMemory(convertedShareName);

		if(openResult != 0)
		{
			LogString("Could not open shared memory.\r\n");
			return S_FALSE;
		}
		dataFlowing = TRUE;
	}
	else
	{
		LogString("Shared memory already open\r\n");
	}

	LogString("Filter Running\r\n");

    return CSource::Run(tStart);
}

// Fill the buffer with data
HRESULT CTSMemSourceFilter::FillBuffer(IMediaSample *pSample)
{
	BYTE *pData;
    long cbData;

    CheckPointer(pSample, E_POINTER);

    CAutoLock cAutoLockShared(&m_Lock);

	if(isOpen() == FALSE)
	{
		LogString("Shared mem not open, closing source.\r\n");
		dataFlowing = FALSE;
		return S_FALSE;
	}

	SHARED_DATA *data = getSharedMemory();

    pSample->GetPointer(&pData);
    cbData = pSample->GetSize();

	__int64 curMemPtr = data->currentPointer;
	__int64 loopCount = data->loopCount;

	if(m_fileCurrentPos == -1)
	{
		m_fileCurrentPos = curMemPtr;
	}

	if(m_loopCount == -1)
	{
		m_loopCount = loopCount;
	}

	// detect overlapping, this is where our source graph is lagging behind the sink graph by a full buffer
	// we need to log this, for now that is all we can do.
	BOOL overlapDetected = FALSE;
	if(!overlapDetected && curMemPtr > m_fileCurrentPos && loopCount > m_loopCount)
		overlapDetected = TRUE;
	if(!overlapDetected && curMemPtr < m_fileCurrentPos && loopCount > (m_loopCount + 1))
		overlapDetected = TRUE;

	if(overlapDetected)
	{
		fallBehindCount++;
		LogString("We have been overlapped, this is very bad, our source graph is running to slow.\r\n");
	}


	int bufferSize = 188 * 16;
	__int64 endPoint = m_fileCurrentPos + bufferSize;
	int count = 0;

	while(true)
	{
		// get the current mem writer pointer
		curMemPtr = data->currentPointer;
		loopCount = data->loopCount;

		// if this read will not wrap around to begining we are just going to read a full block of data
		if(endPoint < shareDataSize)
		{
			// if there is enough data available read it and return
			if(endPoint < curMemPtr || curMemPtr < m_fileCurrentPos)
			{
				count = 0;

				memcpy(pData, (void*)(data->data + m_fileCurrentPos), bufferSize);

				m_fileCurrentPos += bufferSize;
				m_loopCount = loopCount;
				return S_OK;
			}
		}
		// we need to read a wrapped block of data
		else
		{
			if(curMemPtr < m_fileCurrentPos)
			{
				LogString("Reading Split data.\r\n");

				__int64 dataAvailable01 = shareDataSize - m_fileCurrentPos;
				__int64 dataAvailable02 = curMemPtr;

				if((dataAvailable01 + dataAvailable02) > bufferSize)
				{
					count = 0;

					memcpy(pData, (void*)(data->data + m_fileCurrentPos), (int)dataAvailable01);
					memcpy((pData + dataAvailable01), (void*)(data->data), (int)(bufferSize - dataAvailable01));

					m_fileCurrentPos = (bufferSize - dataAvailable01);
					m_loopCount = loopCount;
					return S_OK;
				}
			}
		}

		LogString("Not enough data, waiting for more data.\r\n");
		// if not enough data sleep for 1 sec, if we have waited 30 seconds close source
		if(count++ > 30)
		{
			LogString("Waited to long, exiting\r\n");
			// exit after 30 seconds with no new data
			dataFlowing = FALSE;
			return S_FALSE;
		}
		Sleep(1000);
	}
}

HRESULT CTSMemSourceFilter::LogString(char *sz,...)
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

HRESULT CTSMemSourceFilter::OpenLogFile(char *logFile)
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
// NonDelegatingQueryInterface
//
// Override this to say what interfaces we support where
//
STDMETHODIMP CTSMemSourceFilter::NonDelegatingQueryInterface(REFIID riid, void ** ppv)
{
    CheckPointer(ppv,E_POINTER);

    // Do we have this interface
    if (riid == IID_IMemSourceSettings)
	{
        return GetInterface((IMemSourceSettings *) this, ppv);
	}
	else if (riid == IID_IAMFilterMiscFlags)
	{
        return GetInterface((IAMFilterMiscFlags *) this, ppv);
	}

    return CSource::NonDelegatingQueryInterface(riid, ppv);

} // NonDelegatingQueryInterface

CUnknown * WINAPI CTSMemSourceFilter::CreateInstance(IUnknown *pUnk, HRESULT *phr)
{
    CTSMemSourceFilter *pNewFilter = new CTSMemSourceFilter(pUnk, phr);

	if (phr)
	{
		if (pNewFilter == NULL) 
			*phr = E_OUTOFMEMORY;
		else
			*phr = S_OK;
	}
    return pNewFilter;

}

STDMETHODIMP_(ULONG) CTSMemSourceFilter::GetMiscFlags(void)
{
	return AM_FILTER_MISC_FLAGS_IS_SOURCE;
}





