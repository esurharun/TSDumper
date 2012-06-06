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
    /// The class that controls Diseqc access to a Tevii tuner.
    /// </summary>
    internal class TeviiDiseqcHandler : DiseqcHandlerBase
    {
        internal override string Description { get { return ("Tevii"); } }
        internal override bool CardCapable { get { return (cardCapable); } }

        /// <summary>
        /// Enumerate devices in system.
        /// </summary>
        /// <returns>Number of devices.</returns>
        [DllImport("TeVii.dll", EntryPoint = "FindDevices", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FindDevices();

        /// <summary>
        /// Get the device path.
        /// </summary>
        /// <param name="deviceNumber">The device index.</param>
        /// <returns>The device path or null for an invalid device number.</returns>
        [DllImport("TeVii.dll", EntryPoint = "GetDevicePath", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetDevicePath(int deviceNumber);

        /// <summary>
        /// Get the device name.
        /// </summary>
        /// <param name="deviceNumber">The device index.</param>
        /// <returns>The device name or null for an invalid device number.</returns>
        [DllImport("TeVii.dll", EntryPoint = "GetDeviceName", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetDeviceName(int deviceNumber);

        /// <summary>
        /// Open device.  
        /// </summary>
        /// <param name="deviceNumber">The device index.</param>
        /// <param name="func">Capture function which will receive stream.</param>
        /// <param name="lParam">Application defined parameter which will be passed to capture function</param>
        /// <returns>Non-zero on success; zero otherwise.</returns>
        [DllImport("TeVii.dll", EntryPoint = "OpenDevice", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 OpenDevice(int deviceNumber, IntPtr func, IntPtr lParam);

        /// <summary>
        /// Close device
        /// </summary>
        /// <param name="deviceNumber">The device index.</param>
        /// <returns>Non-zero on success; zero otherwise.</returns>
        [DllImport("TeVii.dll", EntryPoint = "CloseDevice", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 CloseDevice(int deviceNumber);

        /// <summary>
        /// Send DiSEqC message.
        /// </summary>
        /// <param name="deviceNumber">The device index.</param>
        /// <param name="Data">The command to be sent.</param>
        /// <param name="Len">Length of data.</param>
        /// <param name="Repeats">Number of repeats.</param>
        /// <param name="Flg">Non-zero means replace first byte (0xE0) of DiSEqC message with 0xE1 on second and following repeats.</param>
        /// <returns>Non-zero on success; zero otherwise.</returns>
        [DllImport("TeVii.dll", EntryPoint = "SendDiSEqC", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 SendDiSEqC(int deviceNumber, byte[] Data, Int32 Len, Int32 Repeats, Int32 Flg);

        private int deviceNumber;
       
        private bool cardCapable;

        private int reply;

        /// <summary>
        /// Initializes a new instance of the TeviiDiseqcHandler class.
        /// </summary>
        /// <param name="tunerFilter">The tuner filter.</param>
        /// <param name="tunerSpec">The current tuner.</param>
        internal TeviiDiseqcHandler(IBaseFilter tunerFilter, Tuner tunerSpec)
        {
            Int32 deviceCount = FindDevices();
            if (deviceCount == 0)
                return;

            Logger.Instance.Write("Tevii DiSEqC handler: Found " + deviceCount + " devices"); 

            for (int index = 0; index < deviceCount; index++)
            {
                IntPtr devicePathAddress = GetDevicePath(index);
                string devicePath = Marshal.PtrToStringAnsi(devicePathAddress);
                Logger.Instance.Write("Tevii DiSEqC handler: Device " + index + " Path " + devicePath);

                if (devicePath == tunerSpec.Path)
                {
                    cardCapable = true;
                    deviceNumber = index;
                    break;
                }
            }

            if (!cardCapable)
            {
                Logger.Instance.Write("Tevii DiSEqC handler: No device with the correct path found");
                return;
            }

            Logger.Instance.Write("Tevii DiSEqC handler: Opening device " + deviceNumber);

            reply = OpenDevice(deviceNumber, IntPtr.Zero, IntPtr.Zero);
            if (reply != 0)
                Logger.Instance.Write("Tevii DiSEqC handler: Opened device " + deviceNumber);
            else
            {
                Logger.Instance.Write("Tevii DiSEqC handler: Failed to open device " + deviceNumber);
                cardCapable = false;
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
            bool commandReply = sendCommand(GetCommand(lnbNumber, tuningSpec), false);
            if (!commandReply)
            {
                CloseDevice(deviceNumber);
                return (false);
            }

            byte[] commandBytes = GetSecondCommand(lnbNumber, tuningSpec);
            if (commandBytes == null)
            {
                CloseDevice(deviceNumber);
                return (true);
            }

            Thread.Sleep(150);

            return (sendCommand(commandBytes, true));  
        }

        private bool processCommands(string commands)
        {
            string[] commandStrings = commands.Split(new char[] { ':' });

            int index = 0;
            
            foreach (string commandString in commandStrings)
            {
                byte[] command = GetCommand(commandString.Trim());
                bool reply = sendCommand(command, index + 1 == commandStrings.Length);
                if (!reply)
                    return (false);

                index++;

                Thread.Sleep(150);
            }

            return (true);
        }

        private bool sendCommand(byte[] command, bool closeDevice)
        {
            Logger.Instance.Write("Tevii DiSEqC handler: sending command " + ConvertToHex(command));

            reply = SendDiSEqC(deviceNumber, command, 4, 0, 0);
            if (reply == 0)
                Logger.Instance.Write("Tevii DiSEqC handler: command failed");
            else
                Logger.Instance.Write("Tevii DiSEqC handler: command succeeded");

            if (closeDevice)
            {
                Logger.Instance.Write("Tevii DiSEqC handler: closing device");
                int closeReply = CloseDevice(deviceNumber);
                if (closeReply == 0)
                    Logger.Instance.Write("Tevii DiSEqC handler: close failed");
                else
                    Logger.Instance.Write("Tevii DiSEqC handler: close succeeded");

                cardCapable = false;
            }

            return (reply != 0);
        }
    }
}
