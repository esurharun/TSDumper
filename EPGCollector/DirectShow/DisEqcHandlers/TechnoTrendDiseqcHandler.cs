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

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

using DirectShowAPI;

using DomainObjects;

namespace DirectShow
{
    internal class TechnoTrendDiseqcHandler : DiseqcHandlerBase
    {
        internal override string Description { get { return ("TechnoTrend"); } }
        internal override bool CardCapable { get { return (cardCapable); } }

        /// <summary>
        /// Open hardware.
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="deviceIdentifier"></param>
        /// <returns>handle to opened device</returns>
        [DllImport("ttBdaDrvApi_Dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr bdaapiOpenHWIdx(deviceCategory deviceType, Int32 deviceIdentifier);

        /// <summary>
        /// Close API
        /// </summary>
        /// <param name="device"></param>
        [DllImport("ttBdaDrvApi_Dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        private static extern void bdaapiClose(IntPtr device);

        /// <summary>
        /// Diseqc
        /// </summary>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="repeat"></param>
        /// <param name="toneburst"></param>
        /// <param name="polarity"></param>
        /// <returns></returns>
        [DllImport("ttBdaDrvApi_Dll.dll", EntryPoint = "bdaapiSetDiSEqCMsg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int bdaapiSetDiSEqCMsg(IntPtr device, IntPtr data, byte length, byte repeat, byte toneburst, int polarity);

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
        
        private IntPtr handle;
        private readonly bool cardCapable;

        private int reply;

        /// <summary>
        /// Initialize a new instance of the TechnoTrendDiseqcHandler class.
        /// </summary>
        /// <param name="tunerFilter">tunerfilter</param>
        public TechnoTrendDiseqcHandler(IBaseFilter tunerFilter)
        {
            deviceCategory category = getDeviceType(tunerFilter);
            if (category == deviceCategory.UNKNOWN)
                return;

            handle = bdaapiOpenHWIdx(getDeviceType(tunerFilter), getDeviceID(tunerFilter));
            cardCapable = (handle.ToInt32() != -1);               
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
                        return(deviceCategory.BUDGET_2);
                    case LBUDGET3NAME_TUNER:
                    case LBUDGET3NAME_ATSC_TUNER:
                        return(deviceCategory.BUDGET_3);
                    case LUSB2BDA_DVB_NAME_C_TUNER:
                    case LUSB2BDA_DVB_NAME_S_TUNER:
                    case LUSB2BDA_DVB_NAME_T_TUNER:
                        return(deviceCategory.USB_2);
                    case LUSB2BDA_DVBS_NAME_PIN_TUNER:
                        return(deviceCategory.USB_2_PINNACLE);
                    default:
                        return(deviceCategory.UNKNOWN);
                }
            }

            return (deviceCategory.UNKNOWN);
        }

        private int getDeviceID(IBaseFilter tunerFilter)
        {
            IKsPin pKsPin = DsFindPin.ByDirection(tunerFilter, PinDirection.Output, 0) as IKsPin;
            if (pKsPin != null)
            {
                IntPtr raw;
                reply = pKsPin.KsQueryMediums(out raw);
                DsError.ThrowExceptionForHR(reply);
                
                try
                {
                    RegPinMedium rpm;

                    IntPtr addr = new IntPtr(raw.ToInt32() + 8);
                    rpm = (RegPinMedium)Marshal.PtrToStructure(addr, typeof(RegPinMedium));
                    return((int)rpm.dw1);
                }
                finally
                {
                    if (IntPtr.Zero != raw) 
                        Marshal.FreeCoTaskMem(raw);
                }
            }
            
            return (-1);
        }

        internal override bool SendDiseqcCommand(TuningSpec tuningSpec, string port)
        {
            if (!cardCapable)
                return (true);

            int lnbNumber = GetLnbNumber(port);
            if (lnbNumber != -1)
                return (processPort(lnbNumber, tuningSpec));
            else
                return (processCommands(port, tuningSpec));
        }

        private bool processPort(int lnbNumber, TuningSpec tuningSpec)
        {
            bool commandReply = sendCommand(GetCommand(lnbNumber, tuningSpec), tuningSpec.NativeSignalPolarization);
            if (!commandReply)
                return (false);

            byte[] commandBytes = GetSecondCommand(lnbNumber, tuningSpec);
            if (commandBytes == null)
                return (true);

            Thread.Sleep(150);

            return (sendCommand(commandBytes, tuningSpec.NativeSignalPolarization));
        }

        private bool processCommands(string commands, TuningSpec tuningSpec)
        {
            string[] commandStrings = commands.Split(new char[] { ':' });

            foreach (string commandString in commandStrings)
            {
                byte[] command = GetCommand(commandString.Trim());
                bool reply = sendCommand(command, tuningSpec.NativeSignalPolarization);
                if (!reply)
                    return (false);

                Thread.Sleep(150);
            }

            return (true);
        }

        /// <summary>
        /// Sends the DiSEqC command.
        /// </summary>
        /// <param name="command">The DiSEqC command.</param>
        /// <param name="polarisation">The horizontal or vertical polarisation..</param>
        /// <returns>True if succeeded: false otherwise.</returns>
        private bool sendCommand(byte[] command, Polarisation polarisation)
        {
            IntPtr commandBuffer = Marshal.AllocCoTaskMem(1024);

            for (int index = 0; index < command.Length; ++index)
                Marshal.WriteByte(commandBuffer, index, command[index]);

            Logger.Instance.Write("TechnoTrend DiSEqC handler: sending command " + ConvertToHex(command));
            
            int reply = bdaapiSetDiSEqCMsg(handle, commandBuffer, (byte)command.Length, 1, 0, (short)polarisation);
            if (reply != 0)
                Logger.Instance.Write("TechnoTrend DiSEqC handler: command failed error code 0x" + reply.ToString("X"));
            else
                Logger.Instance.Write("TechnoTrend DiSEqC handler: command succeeded");
            
            return (reply == 0);
        }
    }
}
