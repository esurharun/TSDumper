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
// File: Setup.cpp
//------------------------------------------------------------------------------

#include <streams.h>
#include <initguid.h>

#include "Guids.h"
#include "TSMemSource.h"
#include "TSMemSink.h"

/////////////////////////////////////////////////////////////////////////////////////////////////
// TSMemSource Filter and pins
/////////////////////////////////////////////////////////////////////////////////////////////////

// Setup data
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
    L"TS Memory Sink Filter",	// String name
    MERIT_DO_NOT_USE,           // Filter merit
    1,                          // Number pins
    &sinkPin                    // Pin details
};


/////////////////////////////////////////////////////////////////////////////////////////////////
// TSMemSource Filter and pins
/////////////////////////////////////////////////////////////////////////////////////////////////

// Note: It is better to register no media types than to register a partial 
// media type (subtype == GUID_NULL) because that can slow down intelligent connect 
// for everyone else.

// For a specialized source filter like this, it is best to leave out the 
// AMOVIESETUP_FILTER altogether, so that the filter is not available for 
// intelligent connect. Instead, use the CLSID to create the filter or just 
// use 'new' in your application.


// Filter setup data
const AMOVIESETUP_MEDIATYPE sourcePinSubTypes =
{
	&MEDIATYPE_Stream,				// Major type
	&MEDIASUBTYPE_MPEG2_TRANSPORT	// Minor type
};

const AMOVIESETUP_PIN sourcePin = 
{
    L"Output",						// Obsolete, not used.
    FALSE,							// Is this pin rendered?
    TRUE,							// Is it an output pin?
    FALSE,							// Can the filter create zero instances?
    FALSE,							// Does the filter create multiple instances?
    &CLSID_NULL,					// Obsolete.
    NULL,							// Obsolete.
    1,								// Number of media types.
    &sourcePinSubTypes				// Pointer to media types.
};

const AMOVIESETUP_FILTER sourceFilter =
{
    &CLSID_TSMemSourceFilter,		// Filter CLSID
    L"TS Memory Source Filter",		// String name
    MERIT_DO_NOT_USE,				// Filter merit
    1,								// Number pins
    &sourcePin						// Pin details
};

/////////////////////////////////////////////////////////////////////////////////////////////////
// Filter List
/////////////////////////////////////////////////////////////////////////////////////////////////

CFactoryTemplate g_Templates[2] = 
{
	{
		L"TS Memory Sink Filter",			// Name
		&CLSID_TSMemSinkFilter,				// CLSID
		CTSMemSinkFilter::CreateInstance,	// Method to create an instance of MyComponent
		NULL,								// Initialization function
		&sinkFilter							// Set-up information (for filters)
	},
	{
		L"TS Memory Source Filter",			// Name
		&CLSID_TSMemSourceFilter,			// CLSID
		CTSMemSourceFilter::CreateInstance,	// Method to create an instance of MyComponent
		NULL,								// Initialization function
		&sourceFilter						// Set-up information (for filters)
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

