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
    internal class DigitalEverywhereDVBS2Handler : DVBS2HandlerBase
    {
        internal override string Description { get { return ("DigitalEverywhere"); } }
        internal override bool DVBS2Capable { get { return (dvbs2Capable); } }

        private static readonly Guid ksPropSetID = new Guid(0xab132414, 0xd060, 0x11d0, 0x85, 0x83, 0x00, 0xc0, 0x4f, 0xd9, 0xba, 0xf3);
        private const int ksPropertyLNBControl = 12;

        private readonly bool dvbs2Capable;
        
        private int reply;

        internal DigitalEverywhereDVBS2Handler(IBaseFilter tunerFilter)
        {
            IKsPropertySet propertySet = tunerFilter as IKsPropertySet;
            if (propertySet != null)
            {
                KSPropertySupport supported;
                reply = propertySet.QuerySupported(ksPropSetID, ksPropertyLNBControl, out supported);
                if (reply == 0 && (supported & KSPropertySupport.Set) != 0)
                    dvbs2Capable = true;
            }
        }

        internal override bool SetDVBS2Parameters(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest)
        {
            if (!dvbs2Capable)
                return (true);

            IBDA_DigitalDemodulator demodulator = FindDemodulator("DigitalEverywhere", tunerFilter);
            if (demodulator == null)
            {
                Logger.Instance.Write("DigitalEverywhere DVB-S2 handler: Demodulator not located");
                return (false);
            }

            ModulationType modulationType = ModulationType.ModNotSet;

            switch (tuningSpec.Modulation)
            {
                case Modulation.QPSK:
                    modulationType = ModulationType.ModNbcQpsk;
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
                    Logger.Instance.Write("DigitalEverywhere DVB-S2 handler: Set Modulation Type failed error code 0x" + reply.ToString("X"));
                    return (false);
                }
                else
                    Logger.Instance.Write("DigitalEverywhere DVB-S2 handler: Modulation type changed to " + modulationType);
            }
            else
                Logger.Instance.Write("DigitalEverywhere DVB-S2 handler: Modulation type not changed");

            BinaryConvolutionCodeRate oldCodeRate;
            reply = demodulator.get_InnerFECRate(out oldCodeRate);
            if (reply != 0)
            {
                Logger.Instance.Write("DigitalEverywhere DVB-S2 handler: Get FEC Rate failed error code 0x" + reply.ToString("X"));
                return (false);
            }

            SatelliteFrequency satelliteFrequency = tuningSpec.Frequency as SatelliteFrequency;

            BinaryConvolutionCodeRate newCodeRate = oldCodeRate;

            if (satelliteFrequency.Pilot == DomainObjects.Pilot.Off)
                newCodeRate+= 64;
            else
            {
                if (satelliteFrequency.Pilot == DomainObjects.Pilot.On)
                    newCodeRate+= 128;
            }

            switch (satelliteFrequency.RollOff)
            {
                case DomainObjects.RollOff.RollOff20:
                    newCodeRate += 16;
                    break;
                case DomainObjects.RollOff.RollOff25:
                    newCodeRate += 32;
                    break;
                case DomainObjects.RollOff.RollOff35:
                    newCodeRate += 48;
                    break;
                default:
                    break;
            }

            if (oldCodeRate != newCodeRate)
            {
                reply = demodulator.put_InnerFECRate(newCodeRate);
                if (reply != 0)
                {
                    Logger.Instance.Write("DigitalEverywhere DVB-S2 handler: Set FEC Rate failed error code 0x" + reply.ToString("X"));
                    return (false);
                }

                Logger.Instance.Write("DigitalEverywhere DVB-S2 handler: FEC Rate changed from " + oldCodeRate + " to " + newCodeRate);               
            }

            return (true);
        }
    }
}
