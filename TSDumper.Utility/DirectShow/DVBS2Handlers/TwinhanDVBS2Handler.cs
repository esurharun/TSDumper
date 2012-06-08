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
    internal class TwinhanDVBS2Handler : DVBS2HandlerBase
    {
        internal override string Description { get { return ("Twinhan/TechniSat"); } }
        internal override bool DVBS2Capable { get { return (dvbs2Capable); } }

        private IBaseFilter captureFilter;

        private readonly bool dvbs2Capable;

        private readonly Guid THBDA_TUNER = new Guid("E5644CC4-17A1-4eed-BD90-74FDA1D65423");
        private readonly Guid GUID_THBDA_CMD = new Guid("255E0082-2017-4b03-90F8-856A62CB3D67");

        private readonly uint THBDA_IOCTL_CHECK_INTERFACE = 0xaa0001e4;
        
        private int reply;

        public TwinhanDVBS2Handler(IBaseFilter filter)
        {
            captureFilter = filter;

            if (filter != null)
                dvbs2Capable = checkTwinhanInterface();
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

        internal override bool SetDVBS2Parameters(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest)
        {
            if (!dvbs2Capable)
                return (true);

            IBDA_DigitalDemodulator demodulator = FindDemodulator("Twinhan/TechniSat", tunerFilter);
            if (demodulator == null)
            {
                Logger.Instance.Write("Twinhan/TechniSat DVB-S2 handler: Demodulator not located");
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
                    Logger.Instance.Write("Twinhan/TechniSat DVB-S2 handler: Set Modulation Type failed error code 0x" + reply.ToString("X"));
                    return (false);
                }
                else
                    Logger.Instance.Write("Twinhan/TechniSat DVB-S2 handler: Modulation type changed to " + modulationType);
            }
            else
                Logger.Instance.Write("Twinhan/TechniSat DVB-S2 handler: Modulation type not changed");

            return (true);
        }            
    }
}
