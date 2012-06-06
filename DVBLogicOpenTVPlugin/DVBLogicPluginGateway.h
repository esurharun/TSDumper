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

class DVBLogicPluginGateway
{
public:
    DVBLogicPluginGateway();
    ~DVBLogicPluginGateway();

	static DVBLogicPluginGateway* __stdcall GetInstance();

    int __stdcall Init(const wchar_t* working_dir);
    bool __stdcall StartScan(int monitorReference, LPVOID data, char* scan_info);
    bool __stdcall StopScan(int monitorReference);
    EEPGPLuginScanStatus __stdcall GetScanStatus(int monitorReference);
    bool __stdcall GetEPGData(int monitorReference, char* epg_buf, long& epg_buf_size);
	int __stdcall GetProcessID();
	int __stdcall GetMaxSampleSize(int monitorReference);
};
