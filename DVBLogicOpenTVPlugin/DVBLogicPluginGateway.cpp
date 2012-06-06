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

#include <string>
#include <tchar.h>
#include <stdio.h>

#include "stdafx.h"
#include "TVSIEPGPlugin.h"
#include "DVBLogicPluginGateway.h"

using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::Remoting;
using namespace System::IO;
using namespace System::Text;
using namespace System::Threading;
using namespace System::Diagnostics;

using namespace DVBLogicPlugin;

const wchar_t* epgDirectory;
static DVBLogicPluginGateway* instance;
static bool first = true;

DVBLogicPluginGateway* __stdcall DVBLogicPluginGateway::GetInstance()
{
	if (first)
	{
		instance = new DVBLogicPluginGateway();
		first = false;
	}

	return(instance);
}

DVBLogicPluginGateway::DVBLogicPluginGateway() { }

DVBLogicPluginGateway::~DVBLogicPluginGateway() { }

String^ __clrcall getBasePath()
{
	String ^fullPath = gcnew String(epgDirectory) + gcnew String("\\EPG Collector Gateway.cfg");
	array<Byte>^ fileBytes = gcnew array<Byte>(256);
	UTF8Encoding^ encoding = gcnew UTF8Encoding(true);
	int readCount = 0;

	try
	{
		FileStream^ fileStream = File::OpenRead(fullPath);
		readCount = fileStream->Read(fileBytes, 0, 256);
		delete (IDisposable^) fileStream;

		String^ identity = encoding->GetString(fileBytes, 0, 9);
		if (identity != "Location=")
			return(nullptr);
	}
	catch (IOException^)
	{
		return(nullptr);
	}	

	return(encoding->GetString(fileBytes, 9, readCount - 9));
}

Assembly^ __clrcall getAssembly()
{
	String^ softwarePath = getBasePath();
	if (softwarePath == nullptr)
		return(nullptr);
	
	return(Assembly::LoadFrom(softwarePath));
}

int __stdcall DVBLogicPluginGateway::Init(const wchar_t* working_dir)
{
	epgDirectory = working_dir;

	Assembly ^assembly = getAssembly();
	if (assembly == nullptr)
		return(false);

	Type ^type = assembly->GetType("DVBLogicPlugin.PluginController");
		
	PropertyInfo ^propertyInfo = type->GetProperty("Instance");
	Object^ pluginController = propertyInfo->GetValue(nullptr, nullptr); 
	
	MethodInfo ^method = type->GetMethod("Init");
	String ^dirString = gcnew String(working_dir);
	String ^basePath = getBasePath();
	Object^ reply = method->Invoke(pluginController, gcnew array<Object^>(2){dirString, basePath});

	return((int)reply);
}

bool __stdcall DVBLogicPluginGateway::StartScan(int monitorReference, LPVOID data, char* scan_info)
{
	IntPtr ^bufferAddress = gcnew IntPtr((LPVOID)data);		

    Assembly ^assembly = getAssembly();
	if (assembly == nullptr)
		return(false);

	Type ^type = assembly->GetType("DVBLogicPlugin.PluginController");
		
	PropertyInfo ^propertyInfo = type->GetProperty("Instance");
	Object^ pluginController = propertyInfo->GetValue(nullptr, nullptr); 
	
	MethodInfo ^method = type->GetMethod("StartScan");
	Object^ reply = method->Invoke(pluginController, gcnew array<Object^> { monitorReference, bufferAddress->ToInt32(), (IntPtr)scan_info } );

	return((bool)reply);
}

bool __stdcall DVBLogicPluginGateway::StopScan(int monitorReference)
{
    Assembly ^assembly = getAssembly();
	if (assembly == nullptr)
		return(false);

	Type ^type = assembly->GetType("DVBLogicPlugin.PluginController");
		
	PropertyInfo ^propertyInfo = type->GetProperty("Instance");
	Object^ pluginController = propertyInfo->GetValue(nullptr, nullptr); 
	
	MethodInfo ^method = type->GetMethod("StopScan");
	Object^ reply = method->Invoke(pluginController, gcnew array<Object^> { monitorReference } );

	return((bool)reply);
}

EEPGPLuginScanStatus __stdcall DVBLogicPluginGateway::GetScanStatus(int monitorReference)
{
	Assembly ^assembly = getAssembly();
	if (assembly == nullptr)
		return(EEPSS_FINISHED_ERROR);

	Type ^type = assembly->GetType("DVBLogicPlugin.PluginController");

	PropertyInfo ^propertyInfo = type->GetProperty("Instance");
	Object^ pluginController = propertyInfo->GetValue(nullptr, nullptr); 
	
	MethodInfo ^method = type->GetMethod("GetScanStatus");
	Object^ status = method->Invoke(pluginController, gcnew array<Object^> { monitorReference } );

	if ((Int32)status == 0)
		return(EEPSS_UNKNOWN);
	if ((Int32)status == 1)
	    return(EEPSS_IN_PROGRESS);
	if ((Int32)status == 2)
		return(EEPSS_FINISHED_ERROR);
	if ((Int32)status == 3)
		return(EEPSS_FINISHED_SUCCESS);
	if ((Int32)status == 4)
		return(EEPSS_FINISHED_ABORTED);
	
	return(EEPSS_UNKNOWN);	
}

bool __stdcall DVBLogicPluginGateway::GetEPGData(int monitorReference, char* epg_buf, long& epg_buf_size)
{
    Assembly ^assembly = getAssembly();
	if (assembly == nullptr)
		return(false);

	Type ^type = assembly->GetType("DVBLogicPlugin.PluginController");
		
	PropertyInfo ^propertyInfo = type->GetProperty("Instance");
	Object^ pluginController = propertyInfo->GetValue(nullptr, nullptr); 
	
	MethodInfo ^method = type->GetMethod("GetEPGData");
	
	Object^ replySize = method->Invoke(pluginController, gcnew array<Object^> { monitorReference, (IntPtr)epg_buf, epg_buf_size } );
	epg_buf_size = (long)(int)replySize;

	return(true);
}

int __stdcall DVBLogicPluginGateway::GetProcessID()
{
	return(Process::GetCurrentProcess()->Id);
}

int __stdcall DVBLogicPluginGateway::GetMaxSampleSize(int monitorReference)
{
	Assembly ^assembly = getAssembly();
	if (assembly == nullptr)
		return(false);

	Type ^type = assembly->GetType("DVBLogicPlugin.PluginController");
		
	PropertyInfo ^propertyInfo = type->GetProperty("Instance");
	Object^ pluginController = propertyInfo->GetValue(nullptr, nullptr); 
	
	MethodInfo ^method = type->GetMethod("GetMaxSampleSize");
	Object^ sampleSize = method->Invoke(pluginController, gcnew array<Object^> { monitorReference } );

	return((int)sampleSize);
}


