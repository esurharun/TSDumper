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

//------------------------------------------------------------------------------
// File: TSMemSink.h
//------------------------------------------------------------------------------

#include "TSMemSourceInterface.h"
#include "RegStore.h"

class CTSMemSinkPin;
class CTSMemSinkFilter;


//  Pin object

class CTSMemSinkPin : public CRenderedInputPin
{
    CTSMemSinkFilter    * const m_pDump;           // Main renderer object
    CCritSec * const m_pReceiveLock;    // Sample critical section
    REFERENCE_TIME m_tLast;             // Last sample receive time

public:

    CTSMemSinkPin(CTSMemSinkFilter *pDump,
                  LPUNKNOWN pUnk,
                  CCritSec *pLock,
                  CCritSec *pReceiveLock,
                  HRESULT *phr);

    // Do something with this media sample
    STDMETHODIMP Receive(IMediaSample *pSample);
    STDMETHODIMP EndOfStream(void);
    STDMETHODIMP ReceiveCanBlock();

    // Check if the pin can support this specific proposed type and format
    HRESULT CheckMediaType(const CMediaType *);

    // Break connection
    HRESULT BreakConnect();

    // Track NewSegment
    STDMETHODIMP NewSegment(REFERENCE_TIME tStart,
                            REFERENCE_TIME tStop,
                            double dRate);
};


//  CTSMemSinkFilter object which has filter and pin members

class CTSMemSinkFilter : public CBaseFilter, public IMemSinkSettings
{
    friend class CTSMemSinkPin;

    CTSMemSinkPin *m_pPin;          // A simple rendered input pin

    CCritSec m_Lock;                // Main renderer critical section
    CCritSec m_ReceiveLock;         // Sublock for received samples

    CPosPassThru *m_pPosition;      // Renderer position controls

    HANDLE   m_hFile;               // Handle to file for dumping

public:

    DECLARE_IUNKNOWN

    CTSMemSinkFilter(LPUNKNOWN pUnk, HRESULT *phr);
    ~CTSMemSinkFilter();

    static CUnknown * WINAPI CreateInstance(LPUNKNOWN punk, HRESULT *phr);

    // Write raw data stream to a file
    HRESULT Write(PBYTE pbData, LONG lDataLength);

	// interface methods
	STDMETHODIMP set_ShareName(LPCOLESTR pszShareName);
	STDMETHODIMP get_IsDataFlowing(BOOL *dataFlowing);
	STDMETHODIMP set_SignalData(long quality, long strength);

private:

	SYSTEMTIME lastData;

    // Overriden to say what interfaces we support where
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);

	HRESULT OpenLogFile(char *logFile);
	HRESULT LogString(char *sz,...);

    // Pin enumeration
    CBasePin * GetPin(int n);
    int GetPinCount();

    // Open and close the file as necessary
    STDMETHODIMP Run(REFERENCE_TIME tStart);
    STDMETHODIMP Pause();
    STDMETHODIMP Stop();

	LPOLESTR m_pShareName;
	int writeLogFile;
};

