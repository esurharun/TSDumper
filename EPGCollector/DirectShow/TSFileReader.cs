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
using System.IO;
using System.Threading;
using System.ComponentModel;

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The class that describes a transport stream file reader.
    /// </summary>
    public class TSFileReader : TSReaderBase
    {
        /// <summary>
        /// Get the collection of sections read.
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

        private Collection<short> pids;
        private Collection<byte> tables;
        private int maxSections;    

        private string fileName;
        private FileStream fileStream;

        private SIPacket siPacket;
        private Mpeg2Section mpeg2Section;
        private int packetIndex;
        private bool eof;
        private int blockCount;
        private int continuityCount;

        private TSFileReader() { }

        /// <summary>
        /// Initialize a new instance of the TSFileReader class.
        /// </summary>
        /// <param name="maxSections">The maximum number of sections to be loaded.</param>
        /// <param name="fileName">The name of the file holding the transport stream.</param>
        public TSFileReader(int maxSections, string fileName)
        {
            this.maxSections = maxSections;
            this.fileName = fileName;
        }

        /// <summary>
        /// Initialize a new instance of the TSFileReader class.
        /// </summary>
        /// <param name="pid">The PID to be read.</param>
        /// <param name="maxSections">The maximum number of sections to be loaded.</param>
        /// <param name="fileName">The name of the file holding the transport stream.</param>
        public TSFileReader(short pid, int maxSections, string fileName) : this(maxSections, fileName)
        {
            pids = new Collection<short>();
            pids.Add(pid);
        }

        /// <summary>
        /// Initialize a new instance of the TSFileReader class filtering by a list of PID's.
        /// </summary>
        /// <param name="pids">The PID's to be filtered.</param>
        /// <param name="maxSections">The maximum number of sections to be buffered by the reader.</param>
        /// <param name="fileName">The name of the file holding the transport stream.</param>
        public TSFileReader(Collection<short> pids, int maxSections, string fileName) : this(maxSections, fileName)
        {
            this.pids = pids;
        }

        /// <summary>
        /// Initialize a new instance of the TSFileReader class.
        /// </summary>
        /// <param name="table">The table to be read.</param>
        /// <param name="maxSections">The maximum number of sections to be loaded.</param>
        /// <param name="fileName">The name of the file holding the transport stream.</param>
        public TSFileReader(byte table, int maxSections, string fileName) : this(maxSections, fileName)
        {
            tables = new Collection<byte>();
            tables.Add(table);         
        }

        /// <summary>
        /// Initialize a new instance of the TSFileReader class filtering by a list of tables.
        /// </summary>
        /// <param name="tables">The tables to be filtered.</param>
        /// <param name="maxSections">The maximum number of sections to be buffered by the reader.</param>
        /// <param name="fileName">The name of the file holding the transport stream.</param>
        public TSFileReader(Collection<byte> tables, int maxSections, string fileName) : this(maxSections, fileName)
        {
            this.tables = tables;
        }

        /// <summary>
        /// Initialize a new instance of the TSFileReader class.
        /// </summary>
        /// <param name="pid">The PID to filter on.</param>
        /// <param name="table">The table to be read.</param>
        /// <param name="maxSections">The maximum number of sections to be loaded.</param>
        /// <param name="fileName">The name of the file holding the transport stream.</param>
        public TSFileReader(short pid, byte table, int maxSections, string fileName) : this(maxSections, fileName)
        {
            pids = new Collection<short>();
            pids.Add(pid);

            tables = new Collection<byte>();
            tables.Add(table);
        }

        /// <summary>
        /// Initialize a new instance of the TSFileReader class.
        /// </summary>
        /// <param name="pid">The PID to filter on.</param>
        /// <param name="tables">The tables to be read.</param>
        /// <param name="maxSections">The maximum number of sections to be loaded.</param>
        /// <param name="fileName">The name of the file holding the transport stream.</param>
        public TSFileReader(short pid, Collection<byte> tables, int maxSections, string fileName) : this(maxSections, fileName)
        {
            pids = new Collection<short>();
            pids.Add(pid);

            this.tables = tables;
        }

        /// <summary>
        /// Initialize a new instance of the TSFileReader class.
        /// </summary>
        /// <param name="pids">The PID's to filter on.</param>
        /// <param name="table">The table to be read.</param>
        /// <param name="maxSections">The maximum number of sections to be loaded.</param>
        /// <param name="fileName">The name of the file holding the transport stream.</param>
        public TSFileReader(Collection<short> pids, byte table, int maxSections, string fileName) : this(maxSections, fileName)
        {
            this.pids = pids;

            tables = new Collection<byte>();
            tables.Add(table);
        }

        /// <summary>
        /// Initialize a new instance of the TSFileReader class.
        /// </summary>
        /// <param name="pids">The PID's to filter on.</param>
        /// <param name="tables">The table to be read.</param>
        /// <param name="maxSections">The maximum number of sections to be loaded.</param>
        /// <param name="fileName">The name of the file holding the transport stream.</param>
        public TSFileReader(Collection<short> pids, Collection<byte> tables, int maxSections, string fileName) : this(maxSections, fileName)
        {
            this.pids = pids;
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

            /*Logger.Instance.Write("Blocks read: : " + blockCount);*/
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
                Thread.CurrentThread.Name = "TS File Reader";
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            while (!worker.CancellationPending)
                getSection(parameters);

            if (fileStream != null)
                fileStream.Close();

            resetEvent.Set();
        }

        private void getSection(BackgroundParameters parameters)
        {
            if (siPacket == null)
            {
                siPacket = getPacket(true);
                if (siPacket == null)
                {
                    Thread.Sleep(500);
                    return;
                }

                packetIndex = siPacket.DataIndex;
            }

            if (packetIndex > siPacket.ByteData.Length - 1)
            {
                siPacket = null;
                return;
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

            if (length < 2)
            {
                packetIndex += length;
                return;
            }

            if (tables != null && !tables.Contains(table))
            {
                packetIndex += length;
                return;
            }

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
                if (fileStream == null)
                    fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                byte[] buffer = new byte[188];
                int readCount = fileStream.Read(buffer, 0, 188);
                if (readCount < 188)
                {
                    fileStream.Close();
                    eof = true;
                    return (null);
                }

                blockCount++;
                
                if (RunParameters.Instance.TraceIDs.Contains("TSFILEPACKETS"))
                    Logger.Instance.Dump("File Packet " + blockCount, buffer, buffer.Length);
                
                TransportPacket transportPacket = new TransportPacket();

                try
                {
                    transportPacket.Process(buffer);

                    if (!transportPacket.ErrorIndicator && !transportPacket.IsNullPacket)
                    {
                        if (pids == null || (pids != null && pids.Contains((short)transportPacket.PID)))
                        {
                            if (RunParameters.Instance.TraceIDs.Contains("TSPIDPACKETS"))
                                Logger.Instance.Dump("File Packet", buffer, buffer.Length);

                            if (needPayloadStart)
                            {
                                if (transportPacket.StartIndicator)
                                {
                                    continuityCount = transportPacket.ContinuityCount;
                                    done = true;
                                }
                            }
                            else
                            {
                                if (continuityCount == 15)
                                    continuityCount = 0;
                                else
                                    continuityCount++;

                                if (transportPacket.ContinuityCount == continuityCount)
                                    done = true;
                                else
                                {
                                    if (RunParameters.Instance.TraceIDs.Contains("CONTINUITYERRORS"))
                                        Logger.Instance.Write("Continuity error: expected " + continuityCount + " got " + transportPacket.ContinuityCount);
                                    needPayloadStart = true;
                                    return (null);
                                }
                            }

                            if (done)
                            {
                                siPacket = new SIPacket();
                                siPacket.Process(buffer, transportPacket);
                            }
                        }
                    }
                }
                catch (ArgumentOutOfRangeException) 
                {
                    /*Logger.Instance.Write("Failed to parse transport packet");*/
                }
            }      

            return (siPacket);
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
