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
    /// The class that controls Diseqc access to a Conexant chipset tuner.
    /// </summary>
    internal class ConexantDiseqcHandler : DiseqcHandlerBase
    {
        internal override string Description { get { return ("Conexant"); } }
        internal override bool CardCapable { get { return (cardCapable); } }

        private readonly Guid bdaTunerExtensionProperties = new Guid(0xfaa8f3e5, 0x31d4, 0x4e41, 0x88, 0xef, 0xd9, 0xeb, 0x71, 0x6f, 0x6e, 0xc9);

        private readonly bool cardCapable;
        private readonly IKsPropertySet propertySet;

        private const int commandBufferLength = 188;

        private int reply;

        /// <summary>
        /// Initializes a new instance of the ConexantDiseqcHandler class.
        /// </summary>
        /// <param name="tunerFilter">The tuner filter.</param>
        /// <param name="tuner">The tuner.</param>
        internal ConexantDiseqcHandler(IBaseFilter tunerFilter, Tuner tuner)
        {
            if (tuner.Name.ToUpperInvariant().Contains("TBS"))
                return;

            IPin pin = DsFindPin.ByDirection(tunerFilter, PinDirection.Input, 0);
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
            BurstModulationType modulationType;
            if (lnbNumber == 1)                                                   //for simple diseqc switches (i.e. 22KHz tone burst)
                modulationType = BurstModulationType.TONE_BURST_UNMODULATED;
            else
                modulationType = BurstModulationType.TONE_BURST_MODULATED;        //default to tone_burst_modulated

            bool commandReply = sendCommand(GetCommand(lnbNumber, tuningSpec), modulationType);
            if (!commandReply)
                return (false);

            byte[] commandBytes = GetSecondCommand(lnbNumber, tuningSpec);
            if (commandBytes == null)
                return (true);

            Thread.Sleep(150);

            return (sendCommand(commandBytes, BurstModulationType.TONE_BURST_MODULATED));   
        }

        private bool processCommands(string commands)
        {
            string[] commandStrings = commands.Split(new char[] { ':' });

            foreach (string commandString in commandStrings)
            {
                byte[] command = GetCommand(commandString.Trim());
                bool reply = sendCommand(command, BurstModulationType.TONE_BURST_MODULATED);
                if (!reply)
                    return (false);

                Thread.Sleep(150);
            }

            return (true);
        }

        private bool sendCommand(byte[] command, BurstModulationType modulationType)
        {
            IntPtr commandBuffer = Marshal.AllocCoTaskMem(1024);

            for (int index = 0; index < commandBufferLength; ++index)
                Marshal.WriteByte(commandBuffer, index, 0x00);

            for (int index = 0; index < command.Length; ++index)
                Marshal.WriteByte(commandBuffer, index, command[index]);

            Marshal.WriteInt32(commandBuffer, 160, command.Length);             //send message length
            Marshal.WriteInt32(commandBuffer, 164, 0);                          //receive message length
            Marshal.WriteInt32(commandBuffer, 168, 3);                          //amplitude attenuation
            Marshal.WriteInt32(commandBuffer, 172, (byte)modulationType);                        
            Marshal.WriteInt32(commandBuffer, 176, (int)DisEqcVersion.DISEQC_VER_1X);
            Marshal.WriteInt32(commandBuffer, 180, (int)RxMode.RXMODE_NOREPLY);
            Marshal.WriteInt32(commandBuffer, 184, 1);                                  //last_message

            StringBuilder commandString = new StringBuilder("Conexant DiSEqC handler: sending command ");

            byte[] commandBytes = new byte[4];
            for (int index = 0; index < 4; ++index)
                commandBytes[index] = Marshal.ReadByte(commandBuffer, index);
            commandString.Append(ConvertToHex(commandBytes));

            for (int index = 160; index < commandBufferLength; index = (index + 4))
                commandString.Append(" " + Marshal.ReadInt32(commandBuffer, index));
            Logger.Instance.Write(commandString.ToString());

            reply = propertySet.Set(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_DISEQC, commandBuffer, commandBufferLength, commandBuffer, commandBufferLength);
            if (reply != 0)
                Logger.Instance.Write("Conexant DiSEqC handler: command failed error code 0x" + reply.ToString("X"));
            else
                Logger.Instance.Write("Conexant DiSEqC handler: command succeeded");

            return (reply == 0);
        }
    }
}

