////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2010 nzsjb/ukkiwi                                    //
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using DomainObjects;

using DirectShowAPI;

namespace DirectShow
{
    /// <summary>
    /// The class that controls Diseqc for the Mystique Satix dual.
    /// </summary>
    internal class SatixDualDiseqcHandler : DiseqcHandlerBase
    {
        internal override string Description { get { return ("Mystique Satix Dual"); } }
        internal override bool CardCapable { get { return (cardCapable); } }

        private bool cardCapable;
        private IBDA_DiseqCommand commandInterface;

        /// <summary>
        /// Initializes a new instance of the SatixDualDiseqcHandler class.
        /// </summary>
        /// <param name="tunerFilter">The tuner filter.</param>
        internal SatixDualDiseqcHandler(IBaseFilter tunerFilter)
        {
            IBDA_Topology topology = tunerFilter as IBDA_Topology;
            if (topology == null)
                return;

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
                    if (interfaces[searchIndex] == typeof(IBDA_DiseqCommand).GUID)
                    {
                        if (RunParameters.Instance.TraceIDs.Contains("BDA"))
                            Logger.Instance.Write("BDA DiSEqC interface located for node type " + nodeTypes[nodeTypeIndex]);

                        object controlNode;
                        reply = topology.GetControlNode(0, 1, nodeTypes[nodeTypeIndex], out controlNode);
                        DsError.ThrowExceptionForHR(reply);

                        commandInterface = controlNode as IBDA_DiseqCommand;
                        if (commandInterface == null)
                        {
                            if (RunParameters.Instance.TraceIDs.Contains("BDA"))
                                Logger.Instance.Write("BDA Can't use DiSEqC interface: cast of control node failed");
                        }
                        else
                        {
                            if (RunParameters.Instance.TraceIDs.Contains("BDA"))
                                Logger.Instance.Write("BDA DiSEqC interface available");
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sends the diseq command.
        /// </summary>
        /// <param name="tuningSpec">A tuning spec instance.</param>
        /// <param name="port">The LNB port (eg AB).</param>        
        internal override bool SendDiseqcCommand(TuningSpec tuningSpec, string port)
        {
            if (commandInterface == null)
                return (true);

            int reply = commandInterface.put_EnableDiseqCommands(1);
            if (reply != 0)
                Logger.Instance.Write("DiSEqC enble command failed: error code 0x" + reply.ToString("X"));
            else
            {
                int lnbNumber = GetLnbNumber(port);
                reply = commandInterface.put_DiseqLNBSource(lnbNumber - 1);
                if (reply != 0)
                    Logger.Instance.Write("DiSEqC command failed: error code 0x" + reply.ToString("X"));
            }

            return (reply == 0);
        }
    }
}
