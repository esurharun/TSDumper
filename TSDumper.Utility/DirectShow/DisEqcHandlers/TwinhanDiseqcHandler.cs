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

using DirectShowAPI;

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The class that controls Diseqc access to a Twinhan tuner.
    /// </summary>
    internal class TwinhanDiseqcHandler : DiseqcHandlerBase
    {
        internal override string Description { get { return ("Twinhan/TechniSat"); } }
        internal override bool CardCapable { get { return (cardCapable); } }        
        
        private IBaseFilter captureFilter;

        private readonly bool cardCapable;
        
        private IntPtr ptrDiseqc;
        private IntPtr ptrDwBytesReturned;
        private IntPtr thbdaBuf;
        private IntPtr ptrOutBuffer;
        private IntPtr ptrOutBuffer2;
        
        private readonly Guid THBDA_TUNER = new Guid("E5644CC4-17A1-4eed-BD90-74FDA1D65423");
        private readonly Guid GUID_THBDA_CMD = new Guid("255E0082-2017-4b03-90F8-856A62CB3D67");
        
        private readonly uint THBDA_IOCTL_CHECK_INTERFACE = 0xaa0001e4;
        private readonly uint THBDA_IOCTL_SET_DiSEqC = 0xaa0001a0;          //CTL_CODE(THBDA_IO_INDEX, 104, METHOD_BUFFERED, FILE_ANY_ACCESS) 
        private readonly uint THBDA_IOCTL_SET_LNB_DATA = 0xaa000200;        //CTL_CODE(THBDA_IO_INDEX, 128, METHOD_BUFFERED, FILE_ANY_ACCESS) 

        private int reply;
                
        public TwinhanDiseqcHandler(IBaseFilter filter)
        {
            captureFilter = filter;

            ptrDwBytesReturned = Marshal.AllocCoTaskMem(20);
            thbdaBuf = Marshal.AllocCoTaskMem(8192);
            ptrOutBuffer = Marshal.AllocCoTaskMem(8192);
            ptrOutBuffer2 = Marshal.AllocCoTaskMem(8192);
            ptrDiseqc = Marshal.AllocCoTaskMem(8192);
                        
            if (filter != null)
                cardCapable = checkTwinhanInterface();
        }

        private bool checkTwinhanInterface()
        {
            bool success = false;

            IntPtr ptrDwBytesReturned = Marshal.AllocCoTaskMem(4);

            try
            {
                int thbdaLen = 0x28;

                IntPtr thbdaBuf = Marshal.AllocCoTaskMem(thbdaLen);

                try
                {
                    Marshal.WriteInt32(thbdaBuf, 0, 0x255e0082);                            
                    Marshal.WriteInt16(thbdaBuf, 4, 0x2017);
                    Marshal.WriteInt16(thbdaBuf, 6, 0x4b03);
                    Marshal.WriteByte(thbdaBuf, 8, 0x90);
                    Marshal.WriteByte(thbdaBuf, 9, 0xf8);
                    Marshal.WriteByte(thbdaBuf, 10, 0x85);
                    Marshal.WriteByte(thbdaBuf, 11, 0x6a);
                    Marshal.WriteByte(thbdaBuf, 12, 0x62);
                    Marshal.WriteByte(thbdaBuf, 13, 0xcb);
                    Marshal.WriteByte(thbdaBuf, 14, 0x3d);
                    Marshal.WriteByte(thbdaBuf, 15, 0x67);
                    Marshal.WriteInt32(thbdaBuf, 16, (int)THBDA_IOCTL_CHECK_INTERFACE);     //control code
                    Marshal.WriteInt32(thbdaBuf, 20, (int)IntPtr.Zero);
                    Marshal.WriteInt32(thbdaBuf, 24, 0);
                    Marshal.WriteInt32(thbdaBuf, 28, (int)IntPtr.Zero);
                    Marshal.WriteInt32(thbdaBuf, 32, 0);
                    Marshal.WriteInt32(thbdaBuf, 36, (int)ptrDwBytesReturned);

                    IPin pin = DsFindPin.ByDirection(captureFilter, PinDirection.Input, 0);
                    if (pin != null)
                    {
                        IKsPropertySet propertySet = pin as IKsPropertySet;
                        if (propertySet != null)
                        {
                            reply = propertySet.Set(THBDA_TUNER, 0, thbdaBuf, thbdaLen, thbdaBuf, thbdaLen);
                            if (reply == 0)
                                success = true;
                            Marshal.ReleaseComObject(propertySet);
                        }

                        Marshal.ReleaseComObject(pin);
                    }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(thbdaBuf);
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptrDwBytesReturned);
            }

            return (success);
        }

        internal override bool SendDiseqcCommand(TuningSpec tuningSpec, string port)
        {
            byte disEqcPort = 0;

            switch (port)
            {
                case "A":                   
                    disEqcPort = 1;
                    break;
                case "B":                   
                    disEqcPort = 2;
                    break;
                case "AA":                  
                    disEqcPort = 1;
                    break;
                case "AB":                  
                    disEqcPort = 2;
                    break;
                case "BA":                  
                    disEqcPort = 3;
                    break;
                case "BB":                  
                    disEqcPort = 4;
                    break;
            }

            byte turnon22Khz = 0;
            bool isHiBand = false;

            if (tuningSpec.Frequency.Frequency >= (((SatelliteFrequency)tuningSpec.Frequency).SatelliteDish.LNBSwitchFrequency))
            {
                isHiBand = true;
                turnon22Khz = 2;
                
            }
            else
                turnon22Khz = 1;
                
            Logger.Instance.Write("Twinhan/TechniSat high band: " + isHiBand + " 22Khz: " + turnon22Khz);

            setLnbData(true, turnon22Khz, disEqcPort, ((SatelliteFrequency)tuningSpec.Frequency).SatelliteDish);

            return(sendDiseqcCommand(disEqcPort, isHiBand, tuningSpec));
        }

        private void setLnbData(bool lnbPower, int turnon22Khz, int disEqcPort, SatelliteDish satelliteDish)
        {
            int thbdaLen = 0x28;
            int disEqcLen = 20;
            
            Marshal.WriteByte(ptrDiseqc, 0, (byte)(lnbPower ? 1 : 0));                                  // 0: LNB_POWER
            Marshal.WriteByte(ptrDiseqc, 1, 0);                                                         // 1: Tone_Data_Burst (Tone_Data_OFF:0 | Tone_Burst_ON:1 | Data_Burst_ON:2)
            Marshal.WriteByte(ptrDiseqc, 2, 0);
            Marshal.WriteByte(ptrDiseqc, 3, 0);
            Marshal.WriteInt32(ptrDiseqc, 4, satelliteDish.LNBLowBandFrequency / 1000);     // 4: LNBLOF LowBand MHz
            Marshal.WriteInt32(ptrDiseqc, 8, satelliteDish.LNBHighBandFrequency / 1000);    // 8: LNBLOF HighBand MHz
            Marshal.WriteInt32(ptrDiseqc, 12, satelliteDish.LNBSwitchFrequency / 1000);     //12: LNBLOF HiLoSW MHz
            Marshal.WriteByte(ptrDiseqc, 16, (byte)turnon22Khz);                                        //16: f22K_Output (F22K_Output_HiLo:0 | F22K_Output_Off:1 | F22K_Output_On:2
            Marshal.WriteByte(ptrDiseqc, 17, (byte)disEqcPort);                                         //17: DiSEqC_Port
            Marshal.WriteByte(ptrDiseqc, 18, 0);
            Marshal.WriteByte(ptrDiseqc, 19, 0);
            Marshal.WriteInt32(thbdaBuf, 0, 0x255e0082);                                    //GUID_THBDA_CMD
            Marshal.WriteInt16(thbdaBuf, 4, 0x2017);
            Marshal.WriteInt16(thbdaBuf, 6, 0x4b03);
            Marshal.WriteByte(thbdaBuf, 8, 0x90);
            Marshal.WriteByte(thbdaBuf, 9, 0xf8);
            Marshal.WriteByte(thbdaBuf, 10, 0x85);
            Marshal.WriteByte(thbdaBuf, 11, 0x6a);
            Marshal.WriteByte(thbdaBuf, 12, 0x62);
            Marshal.WriteByte(thbdaBuf, 13, 0xcb);
            Marshal.WriteByte(thbdaBuf, 14, 0x3d);
            Marshal.WriteByte(thbdaBuf, 15, 0x67);
            Marshal.WriteInt32(thbdaBuf, 16, (int)THBDA_IOCTL_SET_LNB_DATA);                    //dwIoControlCode
            Marshal.WriteInt32(thbdaBuf, 20, (int)ptrDiseqc.ToInt32());                         //lpInBuffer
            Marshal.WriteInt32(thbdaBuf, 24, disEqcLen);                                        //nInBufferSize
            Marshal.WriteInt32(thbdaBuf, 28, (int)IntPtr.Zero);                                 //lpOutBuffer
            Marshal.WriteInt32(thbdaBuf, 32, 0);                                                //nOutBufferSize
            Marshal.WriteInt32(thbdaBuf, 36, (int)ptrDwBytesReturned);                          //lpBytesReturned

            IPin pin = DsFindPin.ByDirection(captureFilter, PinDirection.Input, 0);
            if (pin != null)
            {
                IKsPropertySet propertySet = pin as IKsPropertySet;
                if (propertySet != null)
                {
                    reply = propertySet.Set(THBDA_TUNER, 0, ptrOutBuffer2, 0x18, thbdaBuf, thbdaLen);
                    if (reply != 0)
                        Logger.Instance.Write("TwinHan/TechniSat SetLNB failed 0x" + reply.ToString("X"));
                    else
                        Logger.Instance.Write("TwinHan/TechniSat SetLNB OK 0x" + reply.ToString("X"));

                    Marshal.ReleaseComObject(propertySet);
                }

                Marshal.ReleaseComObject(pin);
            }
        }

        private bool sendDiseqcCommand(byte disEqcPort, bool isHiBand, TuningSpec tuningSpec)
        {
            //bit 0	(1)	: 0=low band, 1 = hi band
            //bit 1 (2) : 0=vertical, 1 = horizontal
            //bit 3 (4) : 0=satellite position A, 1=satellite position B
            //bit 4 (8) : 0=switch option A, 1=switch option  B
            // LNB    option  position
            // 1        A         A
            // 2        A         B
            // 3        B         A
            // 4        B         B

            bool isHorizontal = ((tuningSpec.NativeSignalPolarization == Polarisation.LinearH) || (tuningSpec.NativeSignalPolarization == Polarisation.CircularL));
            
            byte command = 0xf0;
            command |= (byte)(isHiBand ? 1 : 0);
            command |= (byte)((isHorizontal) ? 2 : 0);
            command |= (byte)((disEqcPort - 1) << 2);
            
            byte[] commandBytes = new byte[4];
            commandBytes[0] = 0xe0;
            commandBytes[1] = 0x10;
            commandBytes[2] = 0x38;
            commandBytes[3] = command;
            
            return(sendCommand(commandBytes));
        }

        /// <summary>
        /// Sends the Diseqc command.
        /// </summary>
        /// <param name="command">The Diseqc command byte string.</param>
        /// <returns>True if succeeded; false otherwise.</returns>
        private bool sendCommand(byte[] command)
        {
            int thbdaLen = 0x28;
            int disEqcLen = 16;

            for (int index = 0; index < 12; ++index)
                Marshal.WriteByte(ptrDiseqc, 4 + index, 0);

            Marshal.WriteInt32(ptrDiseqc, 0, (int)command.Length);          //command len
            
            for (int index = 0; index < command.Length; ++index)
                Marshal.WriteByte(ptrDiseqc, 4 + index, command[index]);
            
            string line = "";
            for (int index = 0; index < disEqcLen; ++index)
            {
                byte commandByte = Marshal.ReadByte(ptrDiseqc, index);
                line += String.Format("{0:X} ", commandByte);
            }

            Marshal.WriteInt32(thbdaBuf, 0, 0x255e0082);                    //GUID_THBDA_CMD
            Marshal.WriteInt16(thbdaBuf, 4, 0x2017);
            Marshal.WriteInt16(thbdaBuf, 6, 0x4b03);
            Marshal.WriteByte(thbdaBuf, 8, 0x90);
            Marshal.WriteByte(thbdaBuf, 9, 0xf8);
            Marshal.WriteByte(thbdaBuf, 10, 0x85);
            Marshal.WriteByte(thbdaBuf, 11, 0x6a);
            Marshal.WriteByte(thbdaBuf, 12, 0x62);
            Marshal.WriteByte(thbdaBuf, 13, 0xcb);
            Marshal.WriteByte(thbdaBuf, 14, 0x3d);
            Marshal.WriteByte(thbdaBuf, 15, 0x67);
            Marshal.WriteInt32(thbdaBuf, 16, (int)THBDA_IOCTL_SET_DiSEqC);  //dwIoControlCode
            Marshal.WriteInt32(thbdaBuf, 20, (int)ptrDiseqc.ToInt32());     //lpInBuffer
            Marshal.WriteInt32(thbdaBuf, 24, disEqcLen);                    //nInBufferSize
            Marshal.WriteInt32(thbdaBuf, 28, (int)IntPtr.Zero);             //lpOutBuffer
            Marshal.WriteInt32(thbdaBuf, 32, 0);                            //nOutBufferSize
            Marshal.WriteInt32(thbdaBuf, 36, (int)ptrDwBytesReturned);      //lpBytesReturned

            Logger.Instance.Write("TwinHan/TechniSat DiSEqC handler: sending command " + line);

            bool success = false;
            
            IPin pin = DsFindPin.ByDirection(captureFilter, PinDirection.Input, 0);
            if (pin != null)
            {
                IKsPropertySet propertySet = pin as IKsPropertySet;
                if (propertySet != null)
                {
                    reply = propertySet.Set(THBDA_TUNER, 0, ptrOutBuffer2, 0x18, thbdaBuf, thbdaLen);
                    if (reply != 0)
                        Logger.Instance.Write("TwinHan/TechniSat DiSEqC handler: command failed error code 0x" + reply.ToString("X"));
                    else
                    {
                        Logger.Instance.Write("TwinHan/TechniSat DiSEqC handler: command succeeded");
                        success = true;
                    }

                    Marshal.ReleaseComObject(propertySet);
                }

                Marshal.ReleaseComObject(pin);
            }

            return (success);
        }
    }
}
