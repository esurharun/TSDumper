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
    /// The class that controls Diseqc access to a DigitalEverywhere tuner.
    /// </summary>
    internal class DigitalEverywhereDiseqcHandler : DiseqcHandlerBase
    {
        internal override string Description { get { return ("DigitalEverywhere"); } }
        internal override bool CardCapable { get { return (cardCapable); } }

        private static readonly Guid ksPropSetID = new Guid(0xab132414, 0xd060, 0x11d0, 0x85, 0x83, 0x00, 0xc0, 0x4f, 0xd9, 0xba, 0xf3);                                                           
        private const int ksPropertyLNBControl = 12;
    
        private readonly bool cardCapable;
        private readonly IKsPropertySet propertySet;

        private const int commandBufferLength = 188;

        private int reply;

        /// <summary>
        /// Initializes a new instance of the DigitalEverywhereDiseqcHandler class.
        /// </summary>
        /// <param name="tunerFilter">The tuner filter.</param>
        internal DigitalEverywhereDiseqcHandler(IBaseFilter tunerFilter)
        {
            propertySet = tunerFilter as IKsPropertySet;
            if (propertySet != null)
            {
                KSPropertySupport supported;
                reply = propertySet.QuerySupported(ksPropSetID, ksPropertyLNBControl, out supported);
                if (reply == 0 && (supported & KSPropertySupport.Set) != 0)
                    cardCapable = true;
            }
        }

        /// <summary>
        /// Sends the diseq command.
        /// </summary>
        /// <param name="tuningSpec">A tuning spec instance.</param>
        /// <param name="port">The LNB port (eg AB).</param>        
        internal override bool SendDiseqcCommand(TuningSpec tuningSpec, string port)
        {
            if (!cardCapable)
                return (true);

            int lnbNumber = GetLnbNumber(port);
            if (lnbNumber != -1)
                return (processPort(lnbNumber, tuningSpec));
            else
                return (processCommands(port));
        }

        private bool processPort(int lnbNumber, TuningSpec tuningSpec)
        {
            bool commandReply = sendCommand(GetCommand(lnbNumber, tuningSpec));
            if (!commandReply)
                return (false);

            byte[] commandBytes = GetSecondCommand(lnbNumber, tuningSpec);
            if (commandBytes == null)
                return (true);

            Thread.Sleep(150);

            return (sendCommand(commandBytes));   
        }

        private bool processCommands(string commands)
        {
            string[] commandStrings = commands.Split(new char[] { ':' });

            foreach (string commandString in commandStrings)
            {
                byte[] command = GetCommand(commandString.Trim());
                bool reply = sendCommand(command);
                if (!reply)
                    return (false);

                Thread.Sleep(150);
            }

            return (true);
        }

        private bool sendCommand(byte[] command)
        {
            IntPtr commandBuffer = Marshal.AllocCoTaskMem(1024);

            for (int index = 0; index < commandBufferLength; ++index)
                Marshal.WriteByte(commandBuffer, index, 0x00);

            Marshal.WriteByte(commandBuffer, 0, 0xFF);                          //Voltage
            Marshal.WriteByte(commandBuffer, 1, 0xFF);                          //ContTone
            Marshal.WriteByte(commandBuffer, 2, 0xFF);                          //Burst
            Marshal.WriteByte(commandBuffer, 3, 0x01);                          //Number of commands
            Marshal.WriteByte(commandBuffer, 4, (byte)command.Length);          //Length of command

            for (int index = 0; index < command.Length; ++index)
                Marshal.WriteByte(commandBuffer, index + 5, command[index]);

            StringBuilder commandString = new StringBuilder("DigitalEverywhere DiSEqC handler: sending command ");

            byte[] commandBytes = new byte[command.Length + 5];
            for (int index = 0; index < commandBytes.Length; ++index)
                commandBytes[index] = Marshal.ReadByte(commandBuffer, index);
            commandString.Append(ConvertToHex(commandBytes));
            Logger.Instance.Write(commandString.ToString());

            reply = propertySet.Set(ksPropSetID, ksPropertyLNBControl, commandBuffer, commandBufferLength, commandBuffer, commandBufferLength);
            if (reply != 0)
                Logger.Instance.Write("DigitalEverywhere DiSEqC handler: command failed error code 0x" + reply.ToString("X"));
            else
                Logger.Instance.Write("DigitalEverywhere DiSEqC handler: command succeeded");

            return (reply == 0);
        }
    }
}
