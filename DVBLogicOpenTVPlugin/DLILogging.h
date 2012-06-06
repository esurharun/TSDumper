//-----------------------------------------------------------------------------
// Copyright(c) 2008-2009 DVBLogic (info@dvblogic.com)
// All rights reserved
//-----------------------------------------------------------------------------
// File:        mb_srvilogging.h
// Project:     DVBLink SDK
//-----------------------------------------------------------------------------

#pragma once

#include <windows.h>

/*! \file
 */

/*! 
	\enum ELogLevel
	\brief This enum defines logging levels
*/
enum ELogLevel
{
	MBGE_LL_NONE = 0,				/*!< No logging */
	MBGE_LL_ERRORS_AND_WARNINGS,	/*!< Errors and warnings only */
	MBGE_LL_INFO,					/*!< Additional life-cycle information */
	MBGE_LL_EXTENDED_INFO,			/*!< Extended (debug) information */
	MBGE_LL_FORCED_INFO				/*!< This information is always written to a log*/
};

/*!	
	This interface is used for logging
*/
__interface IDLLogging
{
	/*!	
		This function writes message to a log
		\param log_level - specifies the log level of the message
		\param log_str - specifies the log message itself
	*/
	void __stdcall LogMessage(ELogLevel log_level, const wchar_t* log_str);
};

