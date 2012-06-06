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
using System.Text;
using System.Threading;

using DirectShowAPI;

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The class that sets DVB-S2 parameters for a Hauppauge tuner.
    /// </summary>
    internal class HauppaugeDVBS2Handler : DVBS2HandlerBase
    {
        internal override string Description { get { return ("Hauppauge"); } }
        internal override bool DVBS2Capable { get { return (dvbs2Capable); } }

        private readonly Guid bdaTunerExtensionProperties = new Guid(0xfaa8f3e5, 0x31d4, 0x4e41, 0x88, 0xef, 0x00, 0xa0, 0xc9, 0xf2, 0x1f, 0xc7);

        private readonly bool dvbs2Capable;
        private readonly IKsPropertySet propertySet;

        private int reply;

        internal HauppaugeDVBS2Handler(IBaseFilter tunerFilter)
        {
            IPin pin = DsFindPin.ByDirection(tunerFilter, PinDirection.Input, 0);
            if (pin != null)
            {
                propertySet = pin as IKsPropertySet;
                if (propertySet != null)
                {
                    KSPropertySupport supported;
                    reply = propertySet.QuerySupported(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_PILOT, out supported);
                    if (reply == 0 && (supported & KSPropertySupport.Set) != 0)
                        dvbs2Capable = true;
                }
            }
        }

        internal override bool SetDVBS2Parameters(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest)
        {
            if (!dvbs2Capable)
                return (true);

            IBDA_DigitalDemodulator demodulator = FindDemodulator("Hauppauge", tunerFilter);
            if (demodulator == null)
            {
                Logger.Instance.Write("Hauppauge DVB-S2 handler: Demodulator not located");
                return (false);
            }

            ModulationType modulationType = ModulationType.ModNotSet;

            switch (tuningSpec.Modulation)
            {
                case Modulation.QPSK:
                    BinaryConvolutionCodeRate codeRate;
                    reply = demodulator.get_InnerFECRate(out codeRate);
                    if (reply != 0)
                    {
                        Logger.Instance.Write("Hauppauge DVB-S2 handler: Get FEC rate failed error code 0x" + reply.ToString("X"));
                        return (false);
                    }

                    if (codeRate == BinaryConvolutionCodeRate.Rate9_10)
                        modulationType = ModulationType.Mod32Qam;                        
                    else
                    {
                        if (codeRate == BinaryConvolutionCodeRate.Rate8_9)
                            modulationType = ModulationType.Mod16Qam;
                        else
                            modulationType = ModulationType.ModBpsk;                            
                    }
                    break;
                case Modulation.PSK8:
                    modulationType = ModulationType.ModNbc8Psk;
                    break;
                default:
                    break;
            }

            if (modulationType != ModulationType.ModNotSet)
            {
                reply = demodulator.put_ModulationType(ref modulationType);
                if (reply != 0)
                {
                    Logger.Instance.Write("Hauppauge DVB-S2 handler: Set Modulation Type failed error code 0x" + reply.ToString("X"));
                    return (false);
                }
                else
                    Logger.Instance.Write("Hauppauge DVB-S2 handler: Modulation type changed to " + modulationType);
            }
            else
                Logger.Instance.Write("Hauppauge DVB-S2 handler: Modulation type not changed");

            IntPtr commandBuffer = Marshal.AllocCoTaskMem(1024);
            IntPtr instanceBuffer = Marshal.AllocCoTaskMem(1024);

            SatelliteFrequency frequency = tuningSpec.Frequency as SatelliteFrequency;

            string setting;

            if (frequency.SymbolRate != 30000)
            {
                Marshal.WriteInt32(commandBuffer, (int)DsUtils.GetNativePilot(frequency.Pilot));
                setting = frequency.Pilot.ToString();
            }
            else
            {
                Logger.Instance.Write("Hauppauge DVB-S2 handler: Setting pilot to off for symbol rate of 30000");
                Marshal.WriteInt32(commandBuffer, (int)DirectShowAPI.Pilot.Off);
                setting = "Off";
            }
            reply = propertySet.Set(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_PILOT, instanceBuffer, 32, commandBuffer, 4);
            if (reply != 0)
            {
                Logger.Instance.Write("Hauppauge DVB-S2 handler: Set Pilot command failed error code 0x" + reply.ToString("X"));
                return (false);
            }
            else
            {
                Logger.Instance.Write("Hauppauge DVB-S2 handler: Pilot set to " + setting);

                Marshal.WriteInt32(commandBuffer, (int)DsUtils.GetNativeRollOff(frequency.RollOff));
                reply = propertySet.Set(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_ROLL_OFF, instanceBuffer, 32, commandBuffer, 4);
                if (reply != 0)
                {
                    Logger.Instance.Write("Hauppauge DVB-S2 handler: Set Rolloff command failed error code 0x" + reply.ToString("X"));
                    return (false);
                }
                else
                    Logger.Instance.Write("Hauppauge DVB-S2 handler: Rolloff set to " + frequency.RollOff);
            }

            return (true);
        }

        internal override bool ClearDVBS2Parameters(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest)
        {
            if (!dvbs2Capable)
                return (true);

            IBDA_DigitalDemodulator demodulator = FindDemodulator("Hauppauge", tunerFilter);
            if (demodulator == null)
            {
                Logger.Instance.Write("Hauppauge DVB-S2 handler: Demodulator not located");
                return (false);
            }

            ModulationType modulationType = tuningSpec.NativeModulation;            
            reply = demodulator.put_ModulationType(ref modulationType);
            if (reply != 0)
            {
                Logger.Instance.Write("Hauppauge DVB-S2 handler: Set Modulation Type failed error code 0x" + reply.ToString("X"));
                return (false);
            }
            else
                Logger.Instance.Write("Hauppauge DVB-S2 handler: Modulation type set to " + modulationType);            

            IntPtr commandBuffer = Marshal.AllocCoTaskMem(1024);
            IntPtr instanceBuffer = Marshal.AllocCoTaskMem(1024);

            SatelliteFrequency frequency = tuningSpec.Frequency as SatelliteFrequency;
            
            Marshal.WriteInt32(commandBuffer, (int)DirectShowAPI.Pilot.NotSet);
            reply = propertySet.Set(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_PILOT, instanceBuffer, 32, commandBuffer, 4);
            if (reply != 0)
                Logger.Instance.Write("Hauppauge DVB-S2 handler: Set Pilot command failed error code 0x" + reply.ToString("X"));
            else
                Logger.Instance.Write("Hauppauge DVB-S2 handler: Pilot set to off");
            
            return (reply == 0);
        }
    }
}
