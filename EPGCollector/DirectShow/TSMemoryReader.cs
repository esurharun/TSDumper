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
using System.Collections.ObjectModel;
using System.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The class that describes a transport stream memory reader.
    /// </summary>
    public class TSMemoryReader : TSReaderBase
    {
        [DllImport("Kernel32.dll")]
        internal static extern int OpenFileMappingA(
            int desiredAccess,
            bool inherit,
            StringBuilder name);

        [DllImport("Kernel32.dll")]
        internal static extern IntPtr MapViewOfFile(
            int handle,
            int desiredAccess,
            int offsetLow,
            int offsetHigh,
            long numberOfBytes);

        private static int sectionMapWrite = 0x02;
        private static int sectionMapRead = 0x04;
        
        private static int fileMapWrite = sectionMapWrite;
        private static int fileMapRead = sectionMapRead;
        
        /// <summary>
        /// Get the PID.
        /// </summary>
        public short PID { get { return (pid); } }
        /// <summary>
        /// Get the table ID.
        /// </summary>
        public byte Table { get { return (table); } }

        /// <summary>
        /// Get the sections loaded by the reader.
        /// </summary>
        public override Collection<Mpeg2Section> Sections
        {
            get
            {
                CheckOwnership();
                if (sections == null)
                    sections = new Collection<Mpeg2Section>();
                return (sections);
            }
        }

        private Collection<Mpeg2Section> sections;
        private BackgroundWorker backgroundWorker;

        private AutoResetEvent resetEvent = new AutoResetEvent(false);
        private bool running;

        private short pid;
        private byte table;
        private int maxSections;

        private string fileName;
        private int memoryHandle;
        private IntPtr memoryPointer;
        private int currentPointer;

        private SIPacket siPacket;
        private Mpeg2Section mpeg2Section;
        private int packetIndex;
        private bool eof;

        private byte[] buffer = new byte[15 * (1024 * 1024)];
        private int maxOffset;
        private int currentOffset;

        private TSMemoryReader() { }

        /// <summary>
        /// Initialize a new instance of the TSMemoryReader class.
        /// </summary>
        /// <param name="pid">The PID.</param>
        /// <param name="table">The table ID.</param>
        /// <param name="maxSections">The maximum number of sections to be loaded.</param>
        public TSMemoryReader(short pid, byte table, int maxSections)
        {
            this.pid = pid;
            this.table = table;
            this.maxSections = maxSections;

            fileName = "PID" + pid.ToString("X") + ".ts";
        }

        /// <summary>
        /// Start the reader.
        /// </summary>
        public override void Run()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(workerDoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerCompleted);
            backgroundWorker.RunWorkerAsync(new BackgroundParameters(pid, table, maxSections));

            running = true;
        }

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                throw new InvalidOperationException("TS File Reader background worker failed - see inner exception", e.Error);
        }

        /// <summary>
        /// Stop the reader.
        /// </summary>
        public override void Stop()
        {
            if (running)
            {
                backgroundWorker.CancelAsync();
                bool reply = resetEvent.WaitOne(new TimeSpan(0, 0, 40));
                running = false;
            }
        }

        private void workerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker == null)
                throw (new ArgumentException("Worker thread has been started with an incorrecect sender"));

            BackgroundParameters parameters = e.Argument as BackgroundParameters;
            if (parameters == null)
                throw (new ArgumentException("Worker thread has been started with an incorrect parameter"));

            if (RunParameters.IsWindows)
                Thread.CurrentThread.Name = "TS File Reader for PID " + parameters.PID + " Table " + parameters.Table;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            while (!worker.CancellationPending)
                getSection(parameters);

            resetEvent.Set();
        }

        private void getSection(BackgroundParameters parameters)
        {
            if (siPacket == null)
            {
                siPacket = getPacket(true);
                if (siPacket == null)
                    return;

                if (parameters.Table != 0xff && parameters.Table != siPacket.ByteData[siPacket.DataIndex])
                {
                    siPacket = null;
                    return;
                }

                packetIndex = siPacket.DataIndex;
            }

            if (packetIndex > siPacket.ByteData.Length - 1)
            {
                siPacket = getPacket(false);
                if (siPacket == null)
                {
                    Thread.Sleep(500);
                    return;
                }
                packetIndex = siPacket.DataIndex;
            }

            byte table = siPacket.ByteData[packetIndex];
            if (table == 0xff)
            {
                siPacket = null;
                return;
            }

            if (packetIndex == siPacket.ByteData.Length - 1)
            {
                siPacket = getPacket(false);
                if (siPacket == null)
                {
                    Thread.Sleep(500);
                    return;
                }
                packetIndex = siPacket.Index;
            }
            else
                packetIndex++;

            byte lengthByte1 = siPacket.ByteData[packetIndex];

            if (packetIndex == siPacket.ByteData.Length - 1)
            {
                siPacket = getPacket(false);
                if (siPacket == null)
                {
                    Thread.Sleep(500);
                    return;
                }
                packetIndex = siPacket.Index;
            }
            else
                packetIndex++;

            byte lengthByte2 = siPacket.ByteData[packetIndex];
            packetIndex++;

            int length = ((lengthByte1 & 0x0f) * 256) + lengthByte2;

            mpeg2Section = new Mpeg2Section();

            mpeg2Section.Data = new byte[length + 3];
            mpeg2Section.Length = length + 3;

            mpeg2Section.Data[0] = table;
            mpeg2Section.Data[1] = lengthByte1;
            mpeg2Section.Data[2] = lengthByte2;

            for (int index = 3; index < length + 3; index++)
            {
                if (packetIndex == siPacket.ByteData.Length)
                {
                    siPacket = getPacket(false);
                    if (siPacket == null)
                    {
                        Thread.Sleep(500);
                        return;
                    }
                    packetIndex = siPacket.Index;
                }
                mpeg2Section.Data[index] = siPacket.ByteData[packetIndex];
                packetIndex++;
            }

            if ((mpeg2Section.Data[1] & 0x80) == 1)
            {
                bool checkCRC = mpeg2Section.CheckCRC();
                if (!checkCRC)
                    return;
            }

            Logger.Instance.Dump("MPEG2 Section", mpeg2Section.Data, mpeg2Section.Data.Length);

            Lock("GetSection");
            if (Sections.Count < parameters.MaxSections)
                Sections.Add(mpeg2Section);
            Release("GetSection");
        }

        private SIPacket getPacket(bool needPayloadStart)
        {
            if (eof)
                return (null);

            SIPacket siPacket = null;
            bool done = false;

            while (!done)
            {
                if (memoryHandle == 0)
                {
                    bool reply = getMappedMemory();
                    if (!reply)
                    {
                        eof = true;
                        return(null);
                    }                    
                }

                if (currentOffset > maxOffset)
                    return (null);

                byte[] buffer = new byte[188];
                for (int index = 0; index < 188; index++)
                    buffer[index] = this.buffer[currentPointer + index];

                currentPointer += 188;
                currentOffset += 188;

                TransportPacket transportPacket = new TransportPacket();
                transportPacket.Process(buffer);

                if (transportPacket.PID == pid)
                {
                    Logger.Instance.Dump("File Packet", buffer, buffer.Length);

                    if (!transportPacket.ErrorIndicator && !transportPacket.IsNullPacket)
                    {
                        if (needPayloadStart)
                        {
                            if (transportPacket.StartIndicator)
                                done = true;
                        }
                        else
                            done = true;

                        if (done)
                        {
                            siPacket = new SIPacket();
                            siPacket.Process(buffer, transportPacket);
                        }
                    }
                }
            }

            return (siPacket);
        }

        private bool getMappedMemory()
        {
            StringBuilder fileName = new StringBuilder(@"Global\$Capture01SHARED$");

            memoryHandle = OpenFileMappingA(
                fileMapRead,
                false,
                fileName);

            if (memoryHandle == 0)
                return (false);

            memoryPointer = MapViewOfFile(memoryHandle, fileMapRead, 0, 0, 0);
            if (memoryPointer == IntPtr.Zero)
                return (false);

            Marshal.Copy(memoryPointer, buffer, 0, 10 * (1024 * 1024));

            maxOffset = (int)buffer[3];
            maxOffset = (maxOffset * 256) + (int)buffer[2];
            maxOffset = (maxOffset * 256) + (int)buffer[1];
            maxOffset = (maxOffset * 256) + (int)buffer[0];
            
            currentPointer = 12;
            currentOffset = 0;

            Logger.Instance.Dump("Memory Buffer", buffer, 2048);

            return (true);
        }

        internal class BackgroundParameters
        {
            internal short PID { get { return (pid); } }
            internal byte Table { get { return (table); } }
            internal int MaxSections { get { return (maxSections); } }

            private short pid;
            private byte table;
            private int maxSections;

            private BackgroundParameters() { }

            internal BackgroundParameters(short pid, byte table, int maxSections)
            {
                this.pid = pid;
                this.table = table;
                this.maxSections = maxSections;
            }
        }
    }
}
