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
using System.Threading;

using DirectShowAPI;

using DomainObjects;

namespace DirectShow
{
    internal class GenPixDVBS2Handler : DVBS2HandlerBase
    {
        internal override string Description { get { return ("GenPix"); } }
        internal override bool DVBS2Capable { get { return (dvbs2Capable); } }

        private readonly Guid bdaTunerExtensionProperties = new Guid(0x0B5221EB, 0xF4C4, 0x4976, 0xB9, 0x59, 0xEF, 0x74, 0x42, 0x74, 0x64, 0xD9);

        private readonly bool dvbs2Capable;
        
        private int reply;

        internal GenPixDVBS2Handler(IBaseFilter tunerFilter)
        {
            IPin pin = DsFindPin.ByDirection(tunerFilter, PinDirection.Output, 0);
            if (pin != null)
            {
                IKsPropertySet propertySet = pin as IKsPropertySet;
                if (propertySet != null)
                {
                    KSPropertySupport supported;
                    propertySet.QuerySupported(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_DISEQC, out supported);
                    if ((supported & KSPropertySupport.Set) != 0)
                        dvbs2Capable = true;
                }
            }
        }

        internal override bool SetDVBS2Parameters(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest)
        {
            if (!dvbs2Capable)
                return (true);

            IBDA_DigitalDemodulator demodulator = FindDemodulator("GenPix", tunerFilter);
            if (demodulator == null)
            {
                Logger.Instance.Write("GenPix DVB-S2 handler: Demodulator not located");
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
                    Logger.Instance.Write("GenPix DVB-S2 handler: Set Modulation Type failed error code 0x" + reply.ToString("X"));
                    return (false);
                }
                else
                    Logger.Instance.Write("GenPix DVB-S2 handler: Modulation type changed to " + modulationType);
            }
            else
                Logger.Instance.Write("GenPix DVB-S2 handler: Modulation type not changed");

            return (true);
        }    
    }
}

