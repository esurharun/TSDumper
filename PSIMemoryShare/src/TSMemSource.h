////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2010 nzsjb/ukkiwi                                    //
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

/*

#pragma once

#include "TSMemSourceInterface.h"
#include "RegStore.h"

class CTSMemSourcePin : public CSourceStream, public IKsPropertySet, public IAMPushSource
{
friend class CTSMemSourceFilter;

private:

	STDMETHODIMP Set(REFGUID guidPropSet, DWORD dwID, void *pInstanceData, DWORD cbInstanceData, void *pPropData, DWORD cbPropData);
	STDMETHODIMP QuerySupported(REFGUID guidPropSet, DWORD dwPropID, DWORD *pTypeSupport);
	STDMETHODIMP Get(REFGUID guidPropSet, DWORD dwPropID, void *pInstanceData, DWORD cbInstanceData, void *pPropData, DWORD cbPropData, DWORD *pcbReturned);

	//IID_IAMPushSource
	STDMETHODIMP GetPushSourceFlags(ULONG *pFlags);
	STDMETHODIMP SetPushSourceFlags(ULONG Flags);
	STDMETHODIMP SetStreamOffset(REFERENCE_TIME rtOffset);
	STDMETHODIMP GetStreamOffset(REFERENCE_TIME *prtOffset);
	STDMETHODIMP GetMaxStreamOffset(REFERENCE_TIME *prtMaxOffset);
	STDMETHODIMP SetMaxStreamOffset(REFERENCE_TIME rtMaxOffset);	
	STDMETHODIMP GetLatency(REFERENCE_TIME *prtLatency);

    // Overriden to say what interfaces we support where
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);

protected:

    CMediaType m_MediaType;
    CCritSec m_cSharedState;            // Protects our internal state
	
public:

    CTSMemSourcePin(HRESULT *phr, CSource *pFilter);
    ~CTSMemSourcePin();

	DECLARE_IUNKNOWN

    // Override the version that offers exactly one media type
    HRESULT DecideBufferSize(IMemAllocator *pAlloc, ALLOCATOR_PROPERTIES *pRequest);
    HRESULT FillBuffer(IMediaSample *pSample);

    // Support multiple display formats
    HRESULT CheckMediaType(const CMediaType *pMediaType);
    HRESULT GetMediaType(int iPosition, CMediaType *pmt);

	// If the file-writing filter slows the graph down, we just do nothing, which means
	// wait until we're unblocked.
    STDMETHODIMP Notify(IBaseFilter *pSelf, Quality q)
    {
        return E_FAIL;
    }


};

class CTSMemSourceFilter : public CSource, public IMemSourceSettings, public IAMFilterMiscFlags
{

private:
    // Constructor is private because you have to use CreateInstance
    CTSMemSourceFilter(IUnknown *pUnk, HRESULT *phr);
    ~CTSMemSourceFilter();

    CTSMemSourcePin *m_pPin;
	LPOLESTR m_pShareName;
	CCritSec m_Lock;
	HANDLE m_hFile;
	__int64 m_fileCurrentPos;
	__int64 m_loopCount;
	BOOL dataFlowing;
	int fallBehindCount;

    // Overriden to say what interfaces we support where
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);

    // Open and close the file as necessary
    STDMETHODIMP Run(REFERENCE_TIME tStart);
    STDMETHODIMP Pause();
    STDMETHODIMP Stop();

	HRESULT OpenLogFile(char *logFile);
	
    // Pin enumeration
    CBasePin * GetPin(int n);
    int GetPinCount();

	STDMETHODIMP_(ULONG) GetMiscFlags(void);

	int writeLogFile;

public:

	DECLARE_IUNKNOWN

	// FillBuffer
	HRESULT FillBuffer(IMediaSample *pSample);

    static CUnknown * WINAPI CreateInstance(IUnknown *pUnk, HRESULT *phr);  

	// interface methods
	STDMETHODIMP get_IsDataFlowing(BOOL *dataFlowing);
	STDMETHODIMP get_FallBehindCount(int *count);

	HRESULT LogString(char *sz,...);

};*/


