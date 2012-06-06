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
using System.Threading;

using DomainObjects;

using DirectShowAPI;

namespace DirectShow
{
    /// <summary>
    /// The class that controls Diseqc using the Windows 7 API interface.
    /// </summary>
    internal class Win7APIDiseqcHandler : DiseqcHandlerBase
    {
        internal override string Description { get { return ("Win7 API"); } }
        internal override bool CardCapable { get { return (commandInterface != null); } }

        private IBDA_DiseqCommand commandInterface;
        private IBaseFilter tunerFilter;

        private int reply;

        /// <summary>
        /// Initializes a new instance of the Win7APIDiseqcHandler class.
        /// </summary>
        /// <param name="tunerFilter">The tuner filter.</param>
        internal Win7APIDiseqcHandler(IBaseFilter tunerFilter)
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

        private bool disableDiseqcCommands(IBaseFilter tunerFilter)
        {
            IBDA_DeviceControl deviceControl = (IBDA_DeviceControl)tunerFilter;
            if (deviceControl != null)
            {
                reply = deviceControl.StartChanges();
                if (reply == 0)
                {
                    Logger.Instance.Write("Win7API DiSEqC handler: disabling driver diseqc commands");
                    reply = commandInterface.put_EnableDiseqCommands(0x00);
                    if (reply != 0)
                        Logger.Instance.Write("Win7API DiSEqC handler: Put Enable command failed: error code 0x" + reply.ToString("X"));
                    else
                    {
                        reply = deviceControl.CheckChanges();
                        if (reply == 0)
                        {
                            reply = deviceControl.CommitChanges();
                            if (reply == 0)
                                return (true);

                            Logger.Instance.Write("Win7API DiSEqC Handler: Commit Changes failed with reply 0x" + reply.ToString("X"));

                            deviceControl.StartChanges();
                            deviceControl.CommitChanges();
                        }
                        else
                            Logger.Instance.Write("Win7API DiSEqC Handler: Check Changes failed with reply 0x" + reply.ToString("X"));                        
                    }
                }
                else
                    Logger.Instance.Write("Win7API DiSEqC Handler: Start Changes failed with reply 0x" + reply.ToString("X"));
            }
            else
                Logger.Instance.Write("Win7API DiSEqC Handler: failed to get device control interface");            

            return (false);
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

            disableDiseqcCommands(tunerFilter);

            int lnbNumber = GetLnbNumber(port);
            if (lnbNumber != -1)
                return(processPort(lnbNumber, tuningSpec));
            else
                return(processCommands(port));
        }

        private bool processPort(int lnbNumber,TuningSpec tuningSpec)
        {
            bool commandReply = sendCommand(1, GetCommand(lnbNumber, tuningSpec));
            if (!commandReply)
                return (false);

            byte[] commandBytes = GetSecondCommand(lnbNumber, tuningSpec);
            if (commandBytes == null)
                return (true);

            return (sendCommand(2, commandBytes));             
        }

        private bool processCommands(string commands)
        {
            string[] commandStrings = commands.Split(new char[] { ':' });

            int requestID = 0;

            foreach (string commandString in commandStrings)
            {
                byte[] commandBytes = GetCommand(commandString.Trim());

                bool sendReply = sendCommand(requestID, commandBytes);
                if (!sendReply)
                    return (false);                

                Thread.Sleep(150);

                requestID++;
            }

            return (true);
        }

        private bool sendCommand(int requestID, byte[] commandBytes)
        {
            Logger.Instance.Write("Win7API DiSEqC handler: sending command " + ConvertToHex(commandBytes));
            
            int sendReply = commandInterface.put_DiseqSendCommand(requestID, commandBytes.Length, ref commandBytes[0]);
            if (reply != 0)
            {
                Logger.Instance.Write("Win7API DiSEqC handler: Put Send command failed: error code 0x" + reply.ToString("X"));
                return (false);
            }
            else
            {
                Logger.Instance.Write("Win7API DiSEqC handler: Put Send command succeeded");
                return (true);
            }
        }
    }
}
