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
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The class that provides data for a plugin from a shared memory block.
    /// </summary>
    public class PluginDataProvider : ISampleDataProvider
    {
        /// <summary>
        /// Open a file mapping object.
        /// </summary>
        /// <param name="dwDesiredAccess">The type of access.</param>
        /// <param name="bInheritHandle">True to allow handle to be inherited; false otherwise.</param>
        /// <param name="lpName">The name of the shared memory.</param>
        /// <returns>A handle to the memory object.</returns>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping(
            int dwDesiredAccess,
            bool bInheritHandle,
            string lpName);

        /// <summary>
        /// Map a shared memory object.
        /// </summary>
        /// <param name="hFileMappingObject">The handle returned by the file mapping API.</param>
        /// <param name="dwDesiredAccess">The access desired.</param>
        /// <param name="dwFileOffsetHigh">The offset high.</param>
        /// <param name="dwFileOffsetLow">The offset low.</param>
        /// <param name="dwNumBytesToMap">The number of bytes to map.</param>
        /// <returns>A pointer to the memory block.</returns>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr MapViewOfFile(
            IntPtr hFileMappingObject,
            int dwDesiredAccess,
            int dwFileOffsetHigh,
            int dwFileOffsetLow,
            int dwNumBytesToMap);

        /// <summary>
        /// Unmap a memory object.
        /// </summary>
        /// <param name="baseAddress">The memory address of the memory object.</param>
        /// <returns>Nonzero if the function succeeds; zero otherwise.</returns>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int UnmapViewOfFile(
            IntPtr baseAddress);

        [Flags]
        private enum fileMapAccess : uint
        {
            fileMapCopy = 0x0001,
            fileMapWrite = 0x0002,
            fileMapRead = 0x0004,
            fileMapAllAccess = 0x001f,
            fileMapExecute = 0x0020
        }       

        /// <summary>
        /// Get the amount of buffer space currently used by the provider.
        /// </summary>
        public int BufferSpaceUsed { get { return (Marshal.ReadInt32(bufferAddress)); } }

        /// <summary>
        /// Get the address of the buffer used by the provider.
        /// </summary>
        public IntPtr BufferAddress { get { return (bufferAddress); } }        

        /// <summary>
        /// Get the frequency the provider is using.
        /// </summary>
        public TuningFrequency Frequency { get { return (tuningFrequency); } }

        /// <summary>
        /// Get the number of sync byte searches.
        /// </summary>
        public int SyncByteSearches { get { return (syncByteSearches); } }
        /// <summary>
        /// Get the number of sync byte searches.
        /// </summary>
        public int SamplesDropped { get { return (samplesDropped); } }
        /// <summary>
        /// Get the maximum sample size.
        /// </summary>
        public int MaximumSampleSize { get { return (maximumSampleSize); } }

        private IntPtr fileMapping;
        private IntPtr bufferAddress;

        private int syncByteSearches = 0;
        private int samplesDropped = 0;
        private int maximumSampleSize = 0;

        private TuningFrequency tuningFrequency;
        private Mutex resourceMutex;

        private const int bufferUsedOffset = 0;
        private const int clearCountOffset = bufferUsedOffset + 4;
        private const int pidListOffset = clearCountOffset + 4;
        private const int dataBufferOffset = pidListOffset + (32 * 4);

        private PluginDataProvider() { }

        /// <summary>
        /// Initialise a new instance of the PluginDataProvider class.
        /// </summary>
        /// <param name="tuningFrequency">The tuning frequency.</param>
        /// <param name="runReference">A unique reference for this run.</param>
        public PluginDataProvider(TuningFrequency tuningFrequency, string runReference)
        {
            this.tuningFrequency = tuningFrequency;

            resourceMutex = new Mutex(false, "DVBLogic Plugin Resource Mutex " + runReference);
        }

        /// <summary>
        /// Start the provider.
        /// </summary>
        /// <param name="runReference">A unique reference identifying the run.</param>
        public string Run(string runReference)
        {
            fileMapping = OpenFileMapping((int)fileMapAccess.fileMapAllAccess, false, "DVBLogic Plugin Shared Memory " + runReference);
            if (fileMapping == null)
                return ("Failed to open file mapping");

            bufferAddress = MapViewOfFile(fileMapping, (int)fileMapAccess.fileMapAllAccess, 0, 0, 0);
            if (fileMapping == null)
                return ("Failed to map view of file");


            return (null);
        }

        /// <summary>
        /// Stop the reader.
        /// </summary>
        public void Stop()
        {
            UnmapViewOfFile(bufferAddress);            
        }

        /// <summary>
        /// Change the PID mappings.
        /// </summary>
        /// <param name="newPid">The new PID to be set.</param>
        public void ChangePidMapping(int newPid)
        {
            ChangePidMapping(new int[] { newPid });
        }

        /// <summary>
        /// Change the PID mappings.
        /// </summary>
        /// <param name="pids">A list of the new PID's to be set.</param>
        public void ChangePidMapping(int[] pids)
        {
            Logger.Instance.Write("Setting " + pids.Length + " pids");

            StringBuilder pidString = new StringBuilder();
            foreach (int pid in pids)
            {
                if (pidString.Length != 0)
                    pidString.Append(", 0x");
                else
                    pidString.Append("0x");

                pidString.Append(pid.ToString("X"));
            }
            Logger.Instance.Write("PID's: " + pidString.ToString());

            bool mutexReply = resourceMutex.WaitOne(5000);
            if (!mutexReply)
                Logger.Instance.Write("Failed to acquire mutex");            

            for (int index = 0; index < 128; index += 4)
                Marshal.WriteInt32(bufferAddress, pidListOffset + index, -1);

            int pidPointer = 0;

            for (int index = 0; index < pids.Length; index++)
            {
                Marshal.WriteInt32(bufferAddress, pidListOffset + pidPointer, pids[index]);
                pidPointer += 4;
            }

            int oldClearCount = Marshal.ReadInt32(bufferAddress, clearCountOffset);
            Marshal.WriteInt32(bufferAddress, clearCountOffset, oldClearCount + 1);            

            Marshal.WriteInt32(bufferAddress, bufferUsedOffset, 0);

            if (mutexReply)
                resourceMutex.ReleaseMutex();
        }
    }
}
