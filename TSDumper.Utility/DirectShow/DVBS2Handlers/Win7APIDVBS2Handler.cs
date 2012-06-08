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

using DomainObjects;

using DirectShowAPI;

namespace DirectShow
{
    // <summary>
    /// The class that sets DVB-S2 parameters using the Windows 7 API interface.
    /// </summary>
    internal class Win7APIDVBS2Handler : DVBS2HandlerBase
    {
        internal override string Description { get { return ("Win7 API"); } }
        internal override bool DVBS2Capable { get { return (commandInterface != null); } }

        private IBDA_DigitalDemodulator2 commandInterface;
        private IBaseFilter tunerFilter;

        private int reply;

        internal Win7APIDVBS2Handler(IBaseFilter tunerFilter)
        {
            IBDA_Topology topology = tunerFilter as IBDA_Topology;
            if (topology == null)
                return;

            this.tunerFilter = tunerFilter;

            int nodeTypeCount = 0;
            int[] nodeTypes = new int[10];

            int reply = topology.GetNodeTypes(out nodeTypeCount, 10, nodeTypes);
            DsError.ThrowExceptionForHR(reply);

            for (int nodeTypeIndex = 0; nodeTypeIndex < nodeTypeCount; nodeTypeIndex++)
            {
                int interfaceCount;
                Guid[] interfaces = new Guid[32];
                reply = topology.GetNodeInterfaces(nodeTypes[nodeTypeIndex], out interfaceCount, 32, interfaces);
                DsError.ThrowExceptionForHR(reply);

                for (int searchIndex = 0; searchIndex < interfaceCount; searchIndex++)
                {
                    if (interfaces[searchIndex] == typeof(IBDA_DigitalDemodulator2).GUID)
                    {
                        if (RunParameters.Instance.TraceIDs.Contains("BDA"))
                            Logger.Instance.Write("BDA DVB-S2 interface located for node type " + nodeTypes[nodeTypeIndex]);

                        object controlNode;
                        reply = topology.GetControlNode(0, 1, nodeTypes[nodeTypeIndex], out controlNode);
                        DsError.ThrowExceptionForHR(reply);

                        commandInterface = controlNode as IBDA_DigitalDemodulator2;
                        if (commandInterface == null)
                        {
                            if (RunParameters.Instance.TraceIDs.Contains("BDA"))
                                Logger.Instance.Write("BDA Can't use DVB-S2 interface: cast of control node failed");
                        }
                        else
                        {
                            if (RunParameters.Instance.TraceIDs.Contains("BDA"))
                                Logger.Instance.Write("BDA DVB-S2 interface available");                            
                            return;
                        }                        
                    }
                }
            }
        }

        /// <summary>
        internal override bool SetDVBS2Parameters(TuningSpec tuningSpec, IBaseFilter tunerFilter, ITuneRequest tuneRequest)
        {
            if (commandInterface == null)
                return (false);

            SatelliteFrequency frequency = tuningSpec.Frequency as SatelliteFrequency;

            DirectShowAPI.Pilot pilot = DsUtils.GetNativePilot(frequency.Pilot);
            reply = commandInterface.put_Pilot(pilot);
            if (reply != 0)
                Logger.Instance.Write("Win7API DVB-S2 handler: Set Pilot command failed error code 0x" + reply.ToString("X"));
            else
            {
                Logger.Instance.Write("Win7API DVB-S2 handler: Set Pilot command succeeded");

                DirectShowAPI.RollOff rollOff = DsUtils.GetNativeRollOff(frequency.RollOff);
                reply = commandInterface.put_RollOff(ref rollOff);
                if (reply != 0)
                    Logger.Instance.Write("Win7API DVB-S2 handler: Set Rolloff command failed error code 0x" + reply.ToString("X"));
                else
                    Logger.Instance.Write("Win7API DVB-S2 handler: Set Rolloff command succeeded");
            }

            return (reply == 0);
        }
    }
}
