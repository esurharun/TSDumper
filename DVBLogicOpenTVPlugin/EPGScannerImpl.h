#pragma once

#include "Inc/TVSIEPGPlugin.h"

class CEPGSampleScanner : public ITVSEPGPlugin
{
public:
    CEPGSampleScanner();
    ~CEPGSampleScanner();

    bool __stdcall Init(SEPGPluginHostInfo* host_info, const wchar_t* working_dir);
    bool __stdcall Term();

    bool __stdcall GetPluginInfo(char* info_buf, long& info_buf_size);

    bool __stdcall Configure(HWND parent_wnd);

    bool __stdcall InitScanner(ITVSEPGPluginHost* host_control);
    bool __stdcall TermScanner();
    bool __stdcall StartScan();
    bool __stdcall StopScan();
    EEPGPLuginScanStatus __stdcall GetScanStatus();

    bool __stdcall GetEPGData(char* epg_buf, long& epg_buf_size);

    bool __stdcall WriteStream(BYTE* pBuffer, int nBufferLength);
};