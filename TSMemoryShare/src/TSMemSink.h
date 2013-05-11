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

#include "TSMemSinkInterface.h"
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

    HANDLE   m_hFile;               // Handle to file for logging
	HANDLE   m_hDumpFile;           // Handle to file for dumping data

	int pidList[32];				// list of mapped pids

	int syncSearchCount;			// no of times needed to search for sync byte
	int samplesDropped;				// no of samples dropped
	int sampleOffset;				// The offset to the start of the first TS sample in a block
	BYTE partSample[188];			// Part of TS sample waiting for next block
	int partSampleSize;				// the size of the part sample
	int dumpFileSize;				// the size of the dump file
	int maximumSampleSize;			// the maximum sample size received
	int sampleBufferSize;			// the buffer size in megabytes

public:

    DECLARE_IUNKNOWN

    CTSMemSinkFilter(LPUNKNOWN pUnk, HRESULT *phr, bool logging, char* logFileName, bool dumping, char* dumpFileName, int bufferSize);
    ~CTSMemSinkFilter();

    static CUnknown * WINAPI CreateInstance(LPUNKNOWN punk, HRESULT *phr);

    // Write raw data stream to a file
    HRESULT Write(PBYTE pbData, LONG lDataLength);

	// interface methods
	STDMETHODIMP get_IsDataFlowing(BOOL *dataFlowing);
	STDMETHODIMP clear();
	STDMETHODIMP get_BufferUsed(int *bufferUsed);
	STDMETHODIMP get_BufferAddress(LPVOID *bufferAddress);

	STDMETHODIMP clearPIDs();
	STDMETHODIMP mapPID(int pid);

	STDMETHODIMP get_SyncSearchCount(int *syncSearchCount);
	STDMETHODIMP get_SamplesDropped(int *samplesDropped);
	STDMETHODIMP get_DumpFileSize(int *dumpFileSize);
	STDMETHODIMP get_MaximumSampleSize(int *maximuSampleSize);

	HRESULT LogString(char *sz,...);

private:

	SYSTEMTIME lastData;

    // Overriden to say what interfaces we support where
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);

	HRESULT OpenLogFile(char* fileName);
	HRESULT OpenDumpFile(char* fileName);	

    // Pin enumeration
    CBasePin * GetPin(int n);
    int GetPinCount();

    // Open and close the file as necessary
    STDMETHODIMP Run(REFERENCE_TIME tStart);
    STDMETHODIMP Pause();
    STDMETHODIMP Stop();

	int writeLogFile;
	int writeDumpFile;
};

