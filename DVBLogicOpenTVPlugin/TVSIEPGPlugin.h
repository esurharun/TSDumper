//-----------------------------------------------------------------------------
// Copyright(c) 2008-2009 DVBLogic (info@dvblogic.com)
// All rights reserved
//-----------------------------------------------------------------------------
// File:        tvsiepgplugin.h
// Project:     DVBLink SDK
//-----------------------------------------------------------------------------

#pragma once

#include "dlilogging.h"

#pragma pack(push, 1)

/*! 
\enum EEPGPLuginScanStatus
\brief This enum defines possible status values of EPG scanner
*/
enum EEPGPLuginScanStatus
{
    EEPSS_UNKNOWN = 0, /*!< Status is unknown */
    EEPSS_IN_PROGRESS, /*!< EPG scan is in progress */
    EEPSS_FINISHED_ERROR, /*!< EPG scan has finished with error */
    EEPSS_FINISHED_SUCCESS, /*!< EPG scan has finished with success and new EPG information is ready */
    EEPSS_FINISHED_ABORTED /*!< EPG scan was aborted */
};

/*! 
\struct SEPGPluginHostInfo
\brief This tructure defines infortmation that plugin receives from the server
*/
struct SEPGPluginHostInfo
{
    wchar_t host_version[255]; /*!< Server version. Format: x.x.x */
    IDLLogging* log_interface; /*!< Logging interface that plugin can use to write log information */
};

/*! 
\interface ITVSEPGPluginHost
\brief Interface that plugin can use to call functions on the server
*/
__interface ITVSEPGPluginHost
{
    /*! This function is called to add a specific PID to the stream that plugin receives from server
        \param pid - PID to add
        \retval - true if successful
    */
    bool __stdcall AddPid(unsigned short pid);
};

/*! 
\interface ITVSEPGPlugin
\brief Interface that EPG scanning plugin must implement
*/
__interface ITVSEPGPlugin
{
    /*! This function is called by the server to initialize the plugin. It is assumed that after this function
        plugin should be able to process GetPluginInfo and Configure requests. 
        This function does not allocate tuner and does not start the actual scan.
        \param host_info - information about the server
        \param working_dir - EPG plugin's working directory
        \retval - true if plugin was initialized successfully
    */

    bool __stdcall Init(SEPGPluginHostInfo* host_info, const wchar_t* working_dir);

    /*! This function is called to finish the plugin's activities, usually followed by FreeLibrary()
        to unload the plugin's dll
        \retval - true if plugin was uninitialized successfully
    */
    bool __stdcall Term();

    /*! This function is called by the server to retrieve information about plugin. The information is returned as xml-formatted string.
        See sample function implementation for the actually required xml tags.
        \param info_buf - buffer to receive information about plugin
        \param info_buf_size - size of the buffer. If buffer is too small, function should return required size (including terminating zero)
        \retval - true if buffer was filled with info, false otherwise
    */
    bool __stdcall GetPluginInfo(char* info_buf, long& info_buf_size);

    /*! This function is called by the server to present plugin's configuration dialog.
        \param parent_wnd - handle to parent's window
        \retval - true if succesful
    */
    bool __stdcall Configure(HWND parent_wnd);


    /*! This function is called by the server to initialize actual EPG scanning functionality
        \param host_control - host server callback interface
        \retval - true if succesful
    */
    bool __stdcall InitScanner(ITVSEPGPluginHost* host_control);

    /*! This function is called by the server to finish scanning activities. It should release all resources required
        for scanning.
        \retval - true if succesful
    */
    bool __stdcall TermScanner();

    /*! This function is called by the server to let the plugin know that new transponder has been tuned and plugin can start 
        scanning EPG data from the stream.
        \param scan_info - scan parameters in xml format
        \retval - true if succesful
    */
    bool __stdcall StartScan(char* scan_info);

    /*! This function is called by the server to interrupt the ongoing scan. Plugin should as soon as possible stop doing scan 
        in response to this function.
        \retval - true if succesful
    */
    bool __stdcall StopScan();

    /*! This function is called by the server to monitor progress of the scan activity. The expected sequence of state changes is:
        EEPSS_IN_PROGRESS after StartScan() until scan is completed (e.g. EEPSS_FINISHED_xxx).
        \retval - scan status
    */
    EEPGPLuginScanStatus __stdcall GetScanStatus();

    /*! This function is called by the server to retrieve scanned EPG data. The information is returned as xml-formatted string.
        See sample function implementation for the actually required xml tags.
        Function is called by the server after GetScanStatus() returns EEPSS_FINISHED_SUCCESS.
        \param info_buf - buffer to receive information about plugin
        \param info_buf_size - size of the buffer. If buffer is too small, function should return required size (including terminating zero)
        \retval - true if buffer was filled with info, false otherwise
    */
    bool __stdcall GetEPGData(char* epg_buf, long& epg_buf_size);

    /*! This function is called by the server to send transport stream from currently tuned transponder to plugin.
        \param pBuffer - pointer to a stream buffer
        \param nBufferLength - stream buffer size (always multiple of 188 bytes)
        \retval - true if succesful
    */
    bool __stdcall WriteStream(BYTE* pBuffer, int nBufferLength);
};

//
// plugin dll fuctions definitions
//
typedef ITVSEPGPlugin* (__stdcall *GET_EPG_PLUGIN_IF_FUNC)();
typedef void (__stdcall *RELEASE_EPG_PLUGIN_IF_FUNC)(ITVSEPGPlugin* plugin_if);

extern "C" ITVSEPGPlugin* __stdcall TVSGetEPGPluginIf();
extern "C" void __stdcall TVSReleaseEPGPluginIf(ITVSEPGPlugin* plugin_if);

#pragma pack(pop)

