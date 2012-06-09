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

#include <streams.h>
#include <initguid.h>

#include "Guids.h"
#include "TSMemSource.h"
#include "TSMemSink.h"

/////////////////////////////////////////////////////////////////////////////////////////////////
// TSMemSource Filter and pins
/////////////////////////////////////////////////////////////////////////////////////////////////

const AMOVIESETUP_MEDIATYPE sinkPinSubTypes =
{
    &MEDIATYPE_NULL,            // Major type
    &MEDIASUBTYPE_NULL          // Minor type
};

const AMOVIESETUP_PIN sinkPin =
{
    L"Input",                   // Pin string name
    FALSE,                      // Is it rendered
    FALSE,                      // Is it an output
    FALSE,                      // Allowed none
    FALSE,                      // Likewise many
    &CLSID_NULL,                // Connects to filter
    L"Output",                  // Connects to pin
    1,                          // Number of types
    &sinkPinSubTypes            // Pin information
};

const AMOVIESETUP_FILTER sinkFilter =
{
    &CLSID_TSMemSinkFilter,     // Filter CLSID
    L"PSI Memory Sink Filter",	// String name
    MERIT_DO_NOT_USE,           // Filter merit
    1,                          // Number pins
    &sinkPin                    // Pin details
};

/////////////////////////////////////////////////////////////////////////////////////////////////
// Filter List
/////////////////////////////////////////////////////////////////////////////////////////////////

CFactoryTemplate g_Templates[1] = 
{
	{
		L"PSI Memory Sink Filter",			// Name
		&CLSID_TSMemSinkFilter,				// CLSID
		CTSMemSinkFilter::CreateInstance,	// Method to create an instance
		NULL,								// Initialization function
		&sinkFilter							// Set-up information for filter
	},	
};

int g_cTemplates = sizeof(g_Templates) / sizeof(g_Templates[0]);    


/////////////////////////////////////////////////////////////////////////////////////////////////
// DLL export functions
/////////////////////////////////////////////////////////////////////////////////////////////////

STDAPI DllRegisterServer()
{
    return AMovieDllRegisterServer2( TRUE );
}

STDAPI DllUnregisterServer()
{
    return AMovieDllRegisterServer2( FALSE );
}

//
// DllEntryPoint
//
extern "C" BOOL WINAPI DllEntryPoint(HINSTANCE, ULONG, LPVOID);

BOOL APIENTRY DllMain(HANDLE hModule, 
                      DWORD  dwReason, 
                      LPVOID lpReserved)
{
	return DllEntryPoint((HINSTANCE)(hModule), dwReason, lpReserved);
}

