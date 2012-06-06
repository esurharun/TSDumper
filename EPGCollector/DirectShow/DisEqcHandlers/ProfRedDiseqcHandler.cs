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
    /// The class that controls Diseqc access to a ProfRed tuner.
    /// </summary>
    internal class ProfRedDiseqcHandler : DiseqcHandlerBase
    {
        internal override string Description { get { return ("ProfRed/TBS"); } }
        internal override bool CardCapable { get { return (cardCapable); } }
        
        private readonly Guid bdaTunerExtensionProperties = new Guid(0xfaa8f3e5, 0x31d4, 0x4e41, 0x88, 0xef, 0xd9, 0xeb, 0x71, 0x6f, 0x6e, 0xc9);

        private readonly bool cardCapable;
        private readonly IKsPropertySet propertySet;

        private bool useGet;
        private bool useSet;

        private const int commandBufferLength = 188;

        private int reply;

        /// <summary>
        /// Initializes a new instance of the ProfRedDiseqcHandler class.
        /// </summary>
        /// <param name="tunerFilter">The tuner filter.</param>
        internal ProfRedDiseqcHandler(IBaseFilter tunerFilter)
        {
            IPin pin = DsFindPin.ByDirection(tunerFilter, PinDirection.Input, 0);
            if (pin != null)
            {
                propertySet = pin as IKsPropertySet;
                if (propertySet != null)
                {
                    KSPropertySupport supported;
                    reply = propertySet.QuerySupported(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_DISEQC, out supported);
                    if (reply == 0)
                    {
                        cardCapable = (supported & KSPropertySupport.Get) == KSPropertySupport.Get || (supported & KSPropertySupport.Set) == KSPropertySupport.Set;

                        if (cardCapable)
                        {
                            useSet = (supported & KSPropertySupport.Set) == KSPropertySupport.Set;
                            useGet = !useSet; 
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

            byte toneDataBurst = 2;
            if (lnbNumber == 1)                                                   // mo tone/data burst is the default
                toneDataBurst = 0;                                                // tone burst for simple A
            else
            {
                if (lnbNumber == 2)
                    toneDataBurst = 1;                                            // data burst for simple B
            }

            byte[] commandBytes = GetCommand(lnbNumber, tuningSpec);
            
            bool commandReply = sendCommand(commandBytes, modulationType, toneDataBurst);
            if (!commandReply)
                return (false);

            commandReply = sendCommand(commandBytes, modulationType, toneDataBurst);
            if (!commandReply)
                return (false);

            commandBytes = GetSecondCommand(lnbNumber, tuningSpec);
            if (commandBytes == null)
                return (true);

            Thread.Sleep(150);
            commandReply = sendCommand(commandBytes, modulationType, toneDataBurst);
            if (commandReply)
            {
                Thread.Sleep(150);
                commandReply = sendCommand(commandBytes, modulationType, toneDataBurst);
            }
            
            return (commandReply);
        }

        private bool processCommands(string commands)
        {
            string[] commandStrings = commands.Split(new char[] { ':' });

            foreach (string commandString in commandStrings)
            {
                byte[] command = GetCommand(commandString.Trim());
                bool reply = sendCommand(command, BurstModulationType.TONE_BURST_MODULATED, 2);
                if (!reply)
                    return (false);

                Thread.Sleep(150);
            }

            return (true);
        }

        private bool sendCommand(byte[] command, BurstModulationType modulationType, byte toneDataBurst)
        {
            IntPtr commandBuffer = Marshal.AllocCoTaskMem(1024);

            for (int index = 0; index < commandBufferLength; ++index)
                Marshal.WriteByte(commandBuffer, index, 0x00);

            for (int index = 0; index < command.Length; ++index)
                Marshal.WriteByte(commandBuffer, index, command[index]);

            if (useGet)
            {
                Marshal.WriteByte(commandBuffer, 151, (byte)command.Length);        //send message length
                Marshal.WriteByte(commandBuffer, 161, 0);                           //receive message length            
                Marshal.WriteInt32(commandBuffer, 162, (int)modulationType);        //phantom LNB burst - simple A/AA = unmodulated else modulated 
                Marshal.WriteInt32(commandBuffer, 166, 2);                          //phantom no reply 
                Marshal.WriteInt32(commandBuffer, 170, 3);                          //command mode
                Marshal.WriteByte(commandBuffer, 174, 0);                           //22hz off 
                Marshal.WriteByte(commandBuffer, 175, toneDataBurst);               //tone/data
                Marshal.WriteByte(commandBuffer, 176, 0);                           //parity errors
                Marshal.WriteByte(commandBuffer, 177, 0);                           //reply errors 
                Marshal.WriteInt32(commandBuffer, 178, 1);                          //last message
                Marshal.WriteInt32(commandBuffer, 182, 0);                          //lnb power
            }
            else
                Marshal.WriteByte(commandBuffer, 10, (byte)command.Length);         //send message length
            
            StringBuilder commandString = new StringBuilder("ProfRed/TBS DiSEqC handler: sending command ");

            byte[] commandBytes = new byte[4];
            for (int index = 0; index < command.Length; ++index)
                commandBytes[index] = Marshal.ReadByte(commandBuffer, index);
            commandString.Append(ConvertToHex(commandBytes));

            for (int index = 151; index < commandBufferLength; index++)
                commandString.Append(" " + Marshal.ReadByte(commandBuffer, index).ToString("X"));
            Logger.Instance.Write(commandString.ToString());

            string getSetString;

            if (useGet)
            {
                int bytesReturned;
                reply = propertySet.Get(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_DISEQC, commandBuffer, 188, commandBuffer, 188, out bytesReturned);
                getSetString = "Get";
            }
            else
            {
                reply = propertySet.Set(bdaTunerExtensionProperties, (int)BdaTunerExtension.KSPROPERTY_BDA_DISEQC, commandBuffer, 11, commandBuffer, 11);
                getSetString = "Set";
            }

            if (reply != 0)
                Logger.Instance.Write("ProfRed/TBS DiSEqC handler: " + getSetString + " command failed error code 0x" + reply.ToString("X"));
            else
                Logger.Instance.Write("ProfRed/TBS DiSEqC handler: " + getSetString + " command succeeded");

            return (reply == 0);
        }
    }
}
