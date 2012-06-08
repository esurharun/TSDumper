////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2012 nzsjb, Harun Esur                                           //
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

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

using DirectShowAPI;

using DomainObjects;

namespace DirectShow
{
    internal class TechnoTrendDVBS2Handler : DVBS2HandlerBase
    {
        internal override string Description { get { return ("TechnoTrend"); } }
        internal override bool DVBS2Capable { get { return (dvbs2Capable); } }

        private const string LBDG2_NAME = "TechnoTrend BDA/DVB Capture";
        private const string LBDG2_NAME_C_TUNER = "TechnoTrend BDA/DVB-C Tuner";
        private const string LBDG2_NAME_S_TUNER = "TechnoTrend BDA/DVB-S Tuner";
        private const string LBDG2_NAME_T_TUNER = "TechnoTrend BDA/DVB-T Tuner";
        private const string LBDG2_NAME_NEW = "ttBudget2 BDA DVB Capture";
        private const string LBDG2_NAME_C_TUNER_NEW = "ttBudget2 BDA DVB-C Tuner";
        private const string LBDG2_NAME_S_TUNER_NEW = "ttBudget2 BDA DVB-S Tuner";
        private const string LBDG2_NAME_T_TUNER_NEW = "ttBudget2 BDA DVB-T Tuner";
        private const string LBUDGET3NAME = "TTHybridTV BDA Digital Capture";
        private const string LBUDGET3NAME_TUNER = "TTHybridTV BDA DVBT Tuner";
        private const string LBUDGET3NAME_ATSC_TUNER = "TTHybridTV BDA ATSC Tuner";
        private const string LBUDGET3NAME_TUNER_ANLG = "TTHybridTV BDA Analog TV Tuner";
        private const string LBUDGET3NAME_ANLG = "TTHybridTV BDA Analog Capture";
        private const string LUSB2BDA_DVB_NAME = "USB 2.0 BDA DVB Capture";
        private const string LUSB2BDA_DSS_NAME = "USB 2.0 BDA DSS Capture";
        private const string LUSB2BDA_DSS_NAME_TUNER = "USB 2.0 BDA DSS Tuner";
        private const string LUSB2BDA_DVB_NAME_C_TUNER = "USB 2.0 BDA DVB-C Tuner";
        private const string LUSB2BDA_DVB_NAME_S_TUNER = "USB 2.0 BDA DVB-S Tuner";
        private const string LUSB2BDA_DVB_NAME_S_TUNER_FAKE = "USB 2.0 BDA (DVB-T Fake) DVB-T Tuner";
        private const string LUSB2BDA_DVB_NAME_T_TUNER = "USB 2.0 BDA DVB-T Tuner";
        private const string LUSB2BDA_DVBS_NAME_PIN = "Pinnacle PCTV 4XXe Capture";
        private const string LUSB2BDA_DVBS_NAME_PIN_TUNER = "Pinnacle PCTV 4XXe Tuner";

        private enum deviceCategory
        {
            UNKNOWN = 0,
            BUDGET_2,
            BUDGET_3,
            USB_2,
            USB_2_PINNACLE,
            USB_2_DSS,
            PREMIUM
        }

        private readonly bool dvbs2Capable;

        private int reply;

        internal TechnoTrendDVBS2Handler(IBaseFilter tunerFilter)
        {
            deviceCategory category = getDeviceType(tunerFilter);
            if (category == deviceCategory.UNKNOWN)
                return;

            dvbs2Capable = true;
        }

        private deviceCategory getDeviceType(IBaseFilter tunerFilter)
        {
            FilterInfo info;

            if (tunerFilter.QueryFilterInfo(out info) == 0)
            {
                switch (info.achName)
                {
                    case LBDG2_NAME_C_TUNER:
                    case LBDG2_NAME_S_TUNER:
                    case LBDG2_NAME_T_TUNER:
                    case LBDG2_NAME_C_TUNER_NEW:
                    case LBDG2_NAME_S_TUNER_NEW:
                    case LBDG2_NAME_T_TUNER_NEW:
                        return (deviceCategory.BUDGET_2);
                    case LBUDGET3NAME_TUNER:
                    case LBUDGET3NAME_ATSC_TUNER:
                        return (deviceCategory.BUDGET_3);
                    case LUSB2BDA_DVB_NAME_C_TUNER:
                    case LUSB2BDA_DVB_NAME_S_TUNER:
                    case LUSB2BDA_DVB_NAME_T_TUNER:
                        return (deviceCategory.USB_2);
                    case LUSB2BDA_DVBS_NAME_PIN_TUNER:
                        return (deviceCategory.USB_2_PINNACLE);
                    default:
                        return (deviceCategory.UNKNOWN);
                }
            }

            return (deviceCategory.UNKNOWN);
        }

        internal override bool SetDVBS2Parameters(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest)
        {
            if (!dvbs2Capable)
                return (true);

            IBDA_DigitalDemodulator demodulator = FindDemodulator("TechnoTrend", tunerFilter);
            if (demodulator == null)
            {
                Logger.Instance.Write("TechnoTrend DVB-S2 handler: Demodulator not located");
                return (false);
            }

            ModulationType modulationType = ModulationType.ModNotSet;

            switch (modulationType)
            {
                case ModulationType.ModQpsk:
                    modulationType = ModulationType.Mod8Vsb;
                    break;
                case ModulationType.Mod8Psk:
                    modulationType = ModulationType.Mod8Vsb;
                    break;
                case ModulationType.Mod16Apsk:
                    modulationType = ModulationType.Mod16Vsb;
                    break;
                case ModulationType.Mod32Apsk:
                    modulationType = ModulationType.ModOqpsk;
                    break;
                default:
                    break;
            }

            if (modulationType != ModulationType.ModNotSet)
            {
                reply = demodulator.put_ModulationType(ref modulationType);
                if (reply != 0)
                {
                    Logger.Instance.Write("TechnoTrend DVB-S2 handler: Set Modulation Type failed error code 0x" + reply.ToString("X"));
                    return (false);
                }
                else
                    Logger.Instance.Write("TechnoTrend DVB-S2 handler: Modulation type changed to " + modulationType);
            }
            else
                Logger.Instance.Write("TechnoTrend DVB-S2 handler: Modulation type not changed");

            return (true);
        }
    }
}
