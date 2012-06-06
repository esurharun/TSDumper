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

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The class that describes a transport stream reader.
    /// </summary>
    public class TSStreamReader : TSReaderBase
    {
        /// <summary>
        /// Get the collection of sections currently held by the reader.
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

        /// <summary>
        /// Get the total number of discontinuities.
        /// </summary>
        public override int Discontinuities
        {
            get
            {
                if (running)
                    CheckOwnership();

                if (pidHandlers == null)
                    return (0);
                else
                {
                    int total = 0;

                    foreach (PidHandler pidHandler in pidHandlers)
                        total += pidHandler.Discontinuities;

                    return (total);
                }
            }
        }

        private Collection<Mpeg2Section> sections;
        private BackgroundWorker backgroundWorker;

        private AutoResetEvent resetEvent = new AutoResetEvent(false);
        private bool running;

        private Collection<byte> tables;
        private int maxSections;

        private bool initialized;
        private IntPtr memoryPointer;
        private IntPtr currentPointer;

        private int maxOffset;
        private int currentOffset;

        private Collection<PidHandler> pidHandlers;

        private TSStreamReader() { }

        /// <summary>
        /// Initialize a new instance of the TSStreamReader class.
        /// </summary>
        /// <param name="maxSections">The maximum number of sections to be buffered by the reader.</param>
        /// <param name="bufferAddress">The address of the memory buffer holding the transport stream.</param>
        public TSStreamReader(int maxSections, IntPtr bufferAddress)
        {
            this.maxSections = maxSections;
            memoryPointer = bufferAddress;
        }

        /// <summary>
        /// Initialize a new instance of the TSStreamReader class filtering by table.
        /// </summary>
        /// <param name="table">The table ID to be filtered.</param>
        /// <param name="maxSections">The maximum number of sections to be buffered by the reader.</param>
        /// <param name="bufferAddress">The address of the memory buffer holding the transport stream.</param>
        public TSStreamReader(byte table, int maxSections, IntPtr bufferAddress) : this(maxSections, bufferAddress)
        {
            tables = new Collection<byte>();
            tables.Add(table);
        }

        /// <summary>
        /// Initialize a new instance of the TSStreamReader class filtering by a list of tables.
        /// </summary>
        /// <param name="tables">The tables to be filtered.</param>
        /// <param name="maxSections">The maximum number of sections to be buffered by the reader.</param>
        /// <param name="bufferAddress">The address of the memory buffer holding the transport stream.</param>
        public TSStreamReader(Collection<byte> tables, int maxSections, IntPtr bufferAddress) : this(maxSections, bufferAddress)
        {
            this.tables = tables;
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
            backgroundWorker.RunWorkerAsync(new BackgroundParameters(maxSections));

            running = true;
        }

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                throw new InvalidOperationException("TS Stream Reader background worker failed - see inner exception", e.Error);
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
                throw (new ArgumentException("Worker thread has been started with an incorrect sender"));

            BackgroundParameters parameters = e.Argument as BackgroundParameters;
            if (parameters == null)
                throw (new ArgumentException("Worker thread has been started with an incorrect parameter"));

            if (RunParameters.IsWindows)
                Thread.CurrentThread.Name = "TS Stream Reader";
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            while (!worker.CancellationPending)
                getSection(worker, parameters);

            resetEvent.Set();
        }

        private void getSection( BackgroundWorker worker, BackgroundParameters parameters)
        {
            Collection<Mpeg2Section> mpeg2Sections = null;

            try
            {
                mpeg2Sections = getNextSection();
                if (mpeg2Sections == null)
                    return;
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }

            Lock("GetSection");

            if (Sections.Count >= parameters.MaxSections)
            {
                bool waitOver = false;

                do
                {
                    Release("GetSectionA");

                    if (worker.CancellationPending)
                        return;

                    Thread.Sleep(100);
                    Lock("GetSectionB");
                    waitOver = Sections.Count < parameters.MaxSections;
                }
                while (!waitOver);
            }

            foreach (Mpeg2Section mpeg2Section in mpeg2Sections)
            {
                if (tables == null || (tables != null && tables.Contains((byte)mpeg2Section.Table)))
                {
                    /*if (Sections.Count < parameters.MaxSections)*/
                        Sections.Add(mpeg2Section);
                }
                /*else
                    Logger.Instance.Write("Section rejected: PID 0x" + mpeg2Section.PID.ToString("X") + " table 0x" + mpeg2Section.Table.ToString("X"));*/
            }
            
            Release("GetSection");
        }

        private Collection<Mpeg2Section> getNextSection()
        {
            if (!initialized)
                initialize();

            if (currentOffset >= maxOffset)
            {
                getMaxOffset();
                return (null);
            }

            byte[] buffer = new byte[188];
            Marshal.Copy(currentPointer, buffer, 0, 188);
            currentPointer = new IntPtr(currentPointer.ToInt64() + 188);
            currentOffset += 188;

            if (RunParameters.Instance.TraceIDs.Contains("TRANSPORTPACKETS"))
                Logger.Instance.Dump("Transport Packet", buffer, buffer.Length);

            TransportPacket transportPacket = new TransportPacket();
            try
            {
                transportPacket.Process(buffer);                
            }
            catch (ArgumentOutOfRangeException)
            {
                Logger.Instance.Write("Transport packet parsing failed at current offset  " + currentOffset +
                    " max offset: " + maxOffset);                
                return (null);
            }

            if (transportPacket.IsNullPacket || transportPacket.ErrorIndicator)
                return (null);

            /*Logger.Instance.Write("Processing PID 0x" + transportPacket.PID.ToString("X") + " from offset " + currentOffset);*/
            PidHandler pidHandler = findPidHandler(transportPacket.PID);
            pidHandler.Process(buffer, transportPacket);

            /*Logger.Instance.Write("PID handler 0x" + pidHandler.Pid.ToString("X") + " has created " + pidHandler.Sections.Count + " sections");*/

            return (pidHandler.Sections);
        }

        private void initialize()
        {
            initialized = true;

            getMaxOffset();

            currentPointer = new IntPtr(memoryPointer.ToInt64() + 136);
            currentOffset = 0;
        }

        private void getMaxOffset()
        {
            maxOffset = Marshal.ReadInt32(memoryPointer);            
        }

        private PidHandler findPidHandler(int pid)
        {
            if (pidHandlers == null)
                pidHandlers = new Collection<PidHandler>();

            foreach (PidHandler pidHandler in pidHandlers)
            {
                if (pidHandler.Pid == pid)
                    return (pidHandler);
            }

            PidHandler newHandler = new PidHandler(pid);
            pidHandlers.Add(newHandler);

            /*Logger.Instance.Write("Created PID handler for pid 0x" + pid.ToString("X"));*/

            return (newHandler);
        }

        internal class BackgroundParameters
        {
            internal int MaxSections { get { return (maxSections); } }

            private int maxSections;

            private BackgroundParameters() { }

            internal BackgroundParameters(int maxSections)
            {
                this.maxSections = maxSections;
            }
        }
    }
}
