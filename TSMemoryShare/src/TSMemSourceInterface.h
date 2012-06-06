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

#pragma once


#ifdef __cplusplus
extern "C"
{
#endif

	// {2A0E8A16-D2DA-404c-89D1-3BDCED37DC12}
	DEFINE_GUID(IID_IMemSinkSettings, 
	0x2a0e8a16, 0xd2da, 0x404c, 0x89, 0xd1, 0x3b, 0xdc, 0xed, 0x37, 0xdc, 0x12);
	struct __declspec(uuid("{2A0E8A16-D2DA-404c-89D1-3BDCED37DC12}")) IMemSinkSettings;

    DECLARE_INTERFACE_(IMemSinkSettings, IUnknown)
    {
        STDMETHOD(set_ShareName) (THIS_ LPCOLESTR pszShareName) PURE;
		STDMETHOD(get_IsDataFlowing) (THIS_ BOOL *dataFlowing) PURE;
		STDMETHOD(set_SignalData) (THIS_ long quality, long strength) PURE;
    };

	// {0678ADA1-A3FC-422e-9366-52B87BDA49A4}
	DEFINE_GUID(IID_IMemSourceSettings, 
	0x678ada1, 0xa3fc, 0x422e, 0x93, 0x66, 0x52, 0xb8, 0x7b, 0xda, 0x49, 0xa4);
	struct __declspec(uuid("{0678ADA1-A3FC-422e-9366-52B87BDA49A4}")) IMemSourceSettings;

    DECLARE_INTERFACE_(IMemSourceSettings, IUnknown)
    {
        STDMETHOD(set_ShareName) (THIS_ LPCOLESTR pszShareName) PURE;
		STDMETHOD(get_IsDataFlowing) (THIS_ BOOL *dataFlowing) PURE;
		STDMETHOD(get_SignalData) (THIS_ long *quality, long *strength) PURE;
		STDMETHOD(get_FallBehindCount) (THIS_ int *count) PURE;
    };

#ifdef __cplusplus
}
#endif
