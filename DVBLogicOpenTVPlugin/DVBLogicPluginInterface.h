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

#include "TVSIEPGPlugin.h"

class DVBLogicPluginInterface : public ITVSEPGPlugin
{
public:
    DVBLogicPluginInterface();
    ~DVBLogicPluginInterface();

    bool __stdcall Init(SEPGPluginHostInfo* host_info, const wchar_t* working_dir);
    bool __stdcall Term();

    bool __stdcall GetPluginInfo(char* info_buf, long& info_buf_size);

    bool __stdcall Configure(HWND parent_wnd);

    bool __stdcall InitScanner(ITVSEPGPluginHost* host_control);
    bool __stdcall TermScanner();
    bool __stdcall StartScan(char* scan_info);
    bool __stdcall StopScan();
    EEPGPLuginScanStatus __stdcall GetScanStatus();

    bool __stdcall GetEPGData(char* epg_buf, long& epg_buf_size);

    bool __stdcall WriteStream(BYTE* pBuffer, int nBufferLength);

	void __stdcall LogString(char *sz);

	int __stdcall GetMaxSampleSize();
};
