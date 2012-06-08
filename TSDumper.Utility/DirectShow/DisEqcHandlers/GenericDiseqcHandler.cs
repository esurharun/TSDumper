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

using DirectShowAPI;

using DomainObjects;

namespace DirectShow
{
    internal class GenericDiseqcHandler : DiseqcHandlerBase
    {
        internal override string Description { get { return ("Generic"); } }
        internal override bool CardCapable { get { return (cardCapable); } }

        private readonly Guid guidBdaDigitalDemodulator = new Guid(0xef30f379, 0x985b, 0x4d10, 0xb6, 0x40, 0xa7, 0x9d, 0x5e, 0x4, 0xe1, 0xe0);

        private IBaseFilter tunerFilter;
        private bool cardCapable;

        private int reply;

        /// <summary>
        /// Initializes a new instance of the GenericDiseqcHandler class.
        /// </summary>
        /// <param name="tunerFilter">The tuner filter.</param>
        internal GenericDiseqcHandler(IBaseFilter tunerFilter)
        {
            this.tunerFilter = tunerFilter;

            IPin pin = DsFindPin.ByName(tunerFilter, "MPEG2 Transport");
            if (pin != null)
            {
                IKsPropertySet propertySet = pin as IKsPropertySet;
                if (propertySet != null)
                {
                    KSPropertySupport supported;
                    reply = propertySet.QuerySupported(guidBdaDigitalDemodulator, (int)BdaDigitalModulator.MODULATION_TYPE, out supported);
                    
                    if (reply == 0 && (supported & KSPropertySupport.Set) != 0)
                        cardCapable = true;
                }
            }
        }

        /// <summary>
        /// Sends the diseq command.
        /// </summary>
        /// <param name="tuningSpec">A tuning spec instance.</param>
        /// <param name="port">The Diseqc port ( eg AB).</param>        
        internal override bool SendDiseqcCommand(TuningSpec tuningSpec, string port)
        {
            if (!cardCapable)
                return (false);

            switch (port)
            {
                case "AA":
                    return(sendRangeCommand(0x00));
                case "AB":
                    return(sendRangeCommand(0x01));
                case "BA":
                    return(sendRangeCommand(0x0100));
                case "BB":
                    return(sendRangeCommand(0x0101));
                case "A":
                    return(sendRangeCommand(0x00));
                case "B":
                    return(sendRangeCommand(0x01));
                default:
                    return (false);
            }
        }

        private bool sendRangeCommand(int range)
        {
            object controlNode;
            reply = ((IBDA_Topology)tunerFilter).GetControlNode(0, 1, 0, out controlNode);
            if (reply == 0)
            {
                IBDA_DeviceControl deviceControl = (IBDA_DeviceControl)tunerFilter;
                if (deviceControl != null)
                {
                    if (controlNode != null)
                    {
                        IBDA_FrequencyFilter frequencyFilter = controlNode as IBDA_FrequencyFilter;
                        reply = deviceControl.StartChanges();
                        if (reply == 0)
                        {
                            if (frequencyFilter != null)
                            {
                                Logger.Instance.Write("Generic DiSEqC Handler: Setting range: 0x" + range.ToString("X"));
                                reply = frequencyFilter.put_Range(range);
                                if (reply == 0)
                                {
                                    reply = deviceControl.CheckChanges();
                                    if (reply == 0)
                                    {
                                        reply = deviceControl.CommitChanges();
                                        if (reply == 0)
                                            return (true);

                                        Logger.Instance.Write("Generic DiSEqC Handler: Commit Changes failed with reply 0x" + reply.ToString("X"));
                                        
                                        deviceControl.StartChanges();
                                        deviceControl.CommitChanges();

                                        return (false);
                                    }
                                    else
                                    {                                        
                                        Logger.Instance.Write("Generic DiSEqC Handler: Check Changes failed with reply 0x" + reply.ToString("X"));
                                        return (false);
                                    }
                                }
                                else
                                {
                                    Logger.Instance.Write("Generic DiSEqC Handler: Set Range failed with reply 0x" + reply.ToString("X"));
                                    return (false);
                                }
                            }
                            else
                                Logger.Instance.Write("Generic DiSEqC Handler: failed to get frequency filter interface");
                        }
                        else
                            Logger.Instance.Write("Generic DiSEqC Handler: Start Changes failed with reply 0x" + reply.ToString("X"));
                    }
                    else
                        Logger.Instance.Write("Generic DiSEqC Handler: failed to get control node");
                }
                else
                    Logger.Instance.Write("Generic DiSEqC Handler: failed to get device control interface");
            }
            else
                Logger.Instance.Write("Generic DiSEqC Handler: failed to get control node");

            return (false);
        }
    }
}
