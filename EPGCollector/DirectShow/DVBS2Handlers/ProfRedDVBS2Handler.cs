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
    internal class ProfRedDVBS2Handler : DVBS2HandlerBase
    {
        internal override string Description { get { return ("ProfRed"); } }
        internal override bool DVBS2Capable { get { return (dvbs2Capable); } }

        private readonly Guid bdaTunerExtensionProperties = new Guid(0xfaa8f3e5, 0x31d4, 0x4e41, 0x88, 0xef, 0xd9, 0xeb, 0x71, 0x6f, 0x6e, 0xc9);

        private readonly bool dvbs2Capable;
        
        private int reply;

        internal ProfRedDVBS2Handler(IBaseFilter tunerFilter, Tuner tuner)
        {
            if (tuner.Name.ToUpperInvariant().Contains("TBS"))
                return;

            IPin pin = DsFindPin.ByDirection(tunerFilter, PinDirection.Input, 0);
            if (pin != null)
            {
                IKsPropertySet propertySet = pin as IKsPropertySet;
                if (propertySet != null)
                {
                    KSPropertySupport supported;
                    reply = propertySet.QuerySupported(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_DISEQC, out supported);
                    if (reply == 0)
                        dvbs2Capable = (supported & KSPropertySupport.Get) == KSPropertySupport.Get || (supported & KSPropertySupport.Set) == KSPropertySupport.Set;
                }
            }
        }

        internal override bool SetDVBS2Parameters(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest)
        {
            if (!dvbs2Capable)
                return (true);

            IBDA_DigitalDemodulator demodulator = FindDemodulator("ProfRed", tunerFilter);
            if (demodulator == null)
            {
                Logger.Instance.Write("ProfRed DVB-S2 handler: Demodulator not located");
                return (false);
            }

            ModulationType modulationType = ModulationType.ModNotSet;

            switch (modulationType)
            {
                case ModulationType.Mod8Psk:
                    modulationType = ModulationType.ModBpsk;
                    break;
                default:
                    break;
            }

            if (modulationType != ModulationType.ModNotSet)
            {
                reply = demodulator.put_ModulationType(ref modulationType);
                if (reply != 0)
                {
                    Logger.Instance.Write("ProfRed DVB-S2 handler: Set Modulation Type failed error code 0x" + reply.ToString("X"));
                    return (false);
                }
                else
                    Logger.Instance.Write("ProfRed DVB-S2 handler: Modulation type changed to " + modulationType);
            }
            else
                Logger.Instance.Write("ProfRed DVB-S2 handler: Modulation type not changed");

            return (true);
        }   
    }
}
