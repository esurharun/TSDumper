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

#pragma once

#ifdef __cplusplus
extern "C"
{
#endif

	// {34D4180C-44EF-4907-8372-82FAB7A36767}
	DEFINE_GUID(IID_IMemSinkSettings, 
	0x34d4180c, 0x44ef, 0x4907, 0x83, 0x72, 0x82, 0xfa, 0xb7, 0xa3, 0x67, 0x67);
	struct __declspec(uuid("{34D4180C-44EF-4907-8372-82FAB7A36767}")) IMemSinkSettings;

    DECLARE_INTERFACE_(IMemSinkSettings, IUnknown)
    {
        STDMETHOD(get_IsDataFlowing) (THIS_ BOOL *dataFlowing) PURE;
		STDMETHOD(clear) () PURE;
		STDMETHOD(get_BufferUsed) (THIS_ int *bufferUsed) PURE;	
		STDMETHOD(get_BufferAddress) (THIS_ LPVOID *bufferAddress) PURE;

		STDMETHOD(clearPIDs) () PURE;
		STDMETHOD(mapPID) (int pid) PURE;

		STDMETHOD(get_SyncSearchCount) (THIS_ int *syncSearchCount) PURE;
		STDMETHOD(get_SamplesDropped) (THIS_ int *samplesDropped) PURE;	
		STDMETHOD(get_DumpFileSize) (THIS_ int *samplesDropped) PURE;
		STDMETHOD(get_MaximumSampleSize) (THIS_ int *maximumSampleSize) PURE;	
    };

#ifdef __cplusplus
}
#endif
