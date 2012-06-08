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
using System.Text;
using System.Threading;

using DirectShowAPI;

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The class that sets DVB-S2 parameters for a TBS tuner.
    /// </summary>
    internal class TBSDVBS2Handler : DVBS2HandlerBase
    {
        internal override string Description { get { return ("TBS"); } }
        internal override bool DVBS2Capable { get { return (dvbs2Capable); } }

        private readonly Guid bdaTunerExtensionProperties = new Guid(0xfaa8f3e5, 0x31d4, 0x4e41, 0x88, 0xef, 0xd9, 0xeb, 0x71, 0x6f, 0x6e, 0xc9);

        private readonly bool dvbs2Capable;
        private readonly IKsPropertySet propertySet;

        private bool useGet;
        private bool useSet;

        private const int commandBufferLength = 1024;

        private int reply;

        internal TBSDVBS2Handler(IBaseFilter tunerFilter, Tuner tuner)
        {
            if (!tuner.Name.ToUpperInvariant().Contains("TBS"))
                return;

            IPin pin = DsFindPin.ByDirection(tunerFilter, PinDirection.Input, 0);
            if (pin != null)
            {
                propertySet = pin as IKsPropertySet;
                if (propertySet != null)
                {
                    KSPropertySupport supported;
                    reply = propertySet.QuerySupported(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_NBC_PARAMS, out supported);
                    if (reply == 0)
                    {
                        dvbs2Capable = (supported & KSPropertySupport.Get) == KSPropertySupport.Get || (supported & KSPropertySupport.Set) == KSPropertySupport.Set;

                        if (dvbs2Capable)
                        {
                            useSet = (supported & KSPropertySupport.Set) == KSPropertySupport.Set;
                            useGet = !useSet;
                        }
                    }
                }
            }
        }

        internal override bool SetDVBS2Parameters(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest)
        {
            if (!dvbs2Capable)
                return (true);
            
            IntPtr commandBuffer = Marshal.AllocCoTaskMem(commandBufferLength);

            for (int index = 0; index < commandBufferLength; ++index)
                Marshal.WriteByte(commandBuffer, index, 0x00);

            SatelliteFrequency frequency = tuningSpec.Frequency as SatelliteFrequency;

            switch (frequency.RollOff)
            {
                case DomainObjects.RollOff.RollOff20:
                    Marshal.WriteInt32(commandBuffer, 0, 0);
                    break;
                case DomainObjects.RollOff.RollOff25:
                    Marshal.WriteInt32(commandBuffer, 0, 1);
                    break;
                case DomainObjects.RollOff.RollOff35:
                    Marshal.WriteInt32(commandBuffer, 0, 2);
                    break;
                default:
                    Marshal.WriteInt32(commandBuffer, 0, 0xff);
                    break;
            }

            switch (frequency.Pilot)
            {
                case DomainObjects.Pilot.Off:
                    Marshal.WriteInt32(commandBuffer, 4, 0);
                    break;
                case DomainObjects.Pilot.On:
                    Marshal.WriteInt32(commandBuffer, 4, 1);
                    break;
                default:
                    Marshal.WriteInt32(commandBuffer, 4, 1);
                    break;
            }

            Marshal.WriteInt32(commandBuffer, 8, 2);            // 0 = auto 1 = DVB-S 2 = DVB-S2
            Marshal.WriteInt32(commandBuffer, 12, (int)tuningSpec.NativeFECRate);
            Marshal.WriteInt32(commandBuffer, 16, (int)ModulationType.ModBpsk);            // 

            StringBuilder commandString = new StringBuilder("TBS DVB-S2 handler: sending command 0x");
            byte[] commandBytes = new byte[20];
            for (int index = 0; index < 20; ++index)
            {
                int commandByte = Marshal.ReadByte(commandBuffer, index);
                if (commandByte < 0x10)
                    commandString.Append("0" + commandByte.ToString("X"));
                else
                    commandString.Append(commandByte.ToString("X"));
            }
            Logger.Instance.Write(commandString.ToString());

            string getSet;

            if (useGet)
            {
                int bytesReturned;
                reply = propertySet.Get(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_NBC_PARAMS, commandBuffer, commandBufferLength, commandBuffer, commandBufferLength, out bytesReturned);
                getSet = "Get";
            }
            else
            {
                reply = propertySet.Set(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_NBC_PARAMS, commandBuffer, commandBufferLength, commandBuffer, commandBufferLength);
                getSet = "Set";
            }

            if (reply != 0)
                Logger.Instance.Write("TBS DVB-S2 handler: " + getSet + " command failed error code 0x" + reply.ToString("X"));
            else
                Logger.Instance.Write("TBS DVB-S2 handler: " + getSet + " command succeeded");

            return (reply == 0);
        }
    }
}
