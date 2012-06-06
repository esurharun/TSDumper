#include "stdafx.h"
#include "TVSIEPGPlugin.h"
#include "DVBLogicOpenTVPlugin.h"

static DVBLogicOpenTVPlugin g_EPGScanner;

ITVSEPGPlugin* __stdcall TVSGetEPGPluginIf()
{
    return &g_EPGScanner;
}

void __stdcall TVSReleaseEPGPluginIf(ITVSEPGPlugin* plugin_if)
{
}
