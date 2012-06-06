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
// File: Guids.h
//------------------------------------------------------------------------------

#pragma once

#ifndef __GUIDS_DEFINED
#define __GUIDS_DEFINED

// source filter GUID
// {92FE2AAA-262B-4c9b-A204-7BE9C1067A3C}
DEFINE_GUID(CLSID_TSMemSourceFilter, 
0x92fe2aaa, 0x262b, 0x4c9b, 0xa2, 0x4, 0x7b, 0xe9, 0xc1, 0x6, 0x7a, 0x3c);

// sink filter GUID
// {A1E74C38-7A02-459a-9EE4-1597AE1849A8}
DEFINE_GUID(CLSID_TSMemSinkFilter, 
0xa1e74c38, 0x7a02, 0x459a, 0x9e, 0xe4, 0x15, 0x97, 0xae, 0x18, 0x49, 0xa8);

#endif
