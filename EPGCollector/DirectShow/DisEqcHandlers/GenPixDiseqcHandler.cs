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

using DirectShowAPI;

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The class that controls Diseqc access to a GenPix tuner.
    /// </summary>
    internal class GenPixDiseqcHandler : DiseqcHandlerBase
    {
        internal override string Description { get { return ("GenPix"); } }
        internal override bool CardCapable { get { return (cardCapable); } }

        private readonly Guid bdaTunerExtensionProperties = new Guid(0x0B5221EB, 0xF4C4, 0x4976, 0xB9, 0x59, 0xEF, 0x74, 0x42, 0x74, 0x64, 0xD9);

        private readonly bool cardCapable;
        private readonly IKsPropertySet propertySet;

        private int reply;

        /// <summary>
        /// Diseqc structure for GenPix cards.
        /// </summary>
        [StructLayout(LayoutKind.Sequential), ComVisible(true)]
        private class DiseqcCommand
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] 
            internal byte[] Command;

            internal byte CommandLength;

            internal DiseqcCommand()
            {
                Command = new byte[6];
            }
        }

        /// <summary>
        /// Initializes a new instance of the GenPixDiseqcHandler class.
        /// </summary>
        /// <param name="tunerFilter">The tuner filter.</param>
        internal GenPixDiseqcHandler(IBaseFilter tunerFilter)
        {
            IPin pin = DsFindPin.ByDirection(tunerFilter, PinDirection.Output, 0);
            if (pin != null)
            {
                propertySet = pin as IKsPropertySet;
                if (propertySet != null)
                {
                    KSPropertySupport supported;
                    propertySet.QuerySupported(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_DISEQC, out supported);
                    if ((supported & KSPropertySupport.Set) != 0)
                        cardCapable = true;
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
            IntPtr replyBuffer = Marshal.AllocCoTaskMem(1024);

            for (int index = 0; index < command.Length; ++index)
                Marshal.WriteByte(commandBuffer, index, command[index]);

            DiseqcCommand diseqcCommand = new DiseqcCommand();
            for (int index = 0; index < diseqcCommand.Command.Length; index++)
                diseqcCommand.Command[index] = 0x00;

            for (int index = 0; index < command.Length; ++index)
                diseqcCommand.Command[index] = command[index];
            diseqcCommand.CommandLength = (byte)command.Length;             

            Marshal.StructureToPtr(diseqcCommand, commandBuffer, false);
            int length = Marshal.SizeOf(diseqcCommand);

            Logger.Instance.Write("GenPix DiSEqC Handler: sending command " + ConvertToHex(diseqcCommand.Command));

            reply = propertySet.Set(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_DISEQC, replyBuffer, 32, commandBuffer, length);
            if (reply != 0)
                Logger.Instance.Write("GenPix DiSEqC handler: command failed error code 0x" + reply.ToString("X"));
            else
                Logger.Instance.Write("GenPix DiSEqC handler: command succeeded");

            return (reply == 0);
        }
    }
}

