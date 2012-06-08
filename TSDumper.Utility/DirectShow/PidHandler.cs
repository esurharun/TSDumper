///////////////////////////////////////////////////////////////////////////////// 
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

using System.Collections.ObjectModel;

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The class that describes a PID handler.
    /// </summary>
    public class PidHandler
    {
        /// <summary>
        ///  Get the PID processed by this handler.
        /// </summary>
        public int Pid { get { return (pid); } }
        
        /// <summary>
        /// Get the completed sections for the PID.
        /// </summary>
        public Collection<Mpeg2Section> Sections 
        { 
            get 
            {
                empty = true;
                return (sections); 
            } 
        }

        /// <summary>
        /// Get the number of discontinuities encountered.
        /// </summary>
        public int Discontinuities { get { return (discontinuities); } }
        
        private enum handlerStatus
        {
            awaitingStart,
            awaitingLengthByte1,
            awaitingLengthByte2,
            awaitingMoreData
        }

        private int pid;
        private handlerStatus status = handlerStatus.awaitingStart;
        private SIPacket siPacket;
        private int packetIndex;
        private byte table;
        private byte lengthByte1;
        private byte lengthByte2;
        private int length;
        private int continuityCount = -1;
        private int discontinuities;

        private Mpeg2Section mpeg2Section;
        private int sectionIndex;

        private Collection<Mpeg2Section> sections;
        private bool empty;

        private PidHandler() { }

        /// <summary>
        /// Initialize a new instance of the PidHandler class.
        /// </summary>
        /// <param name="pid">The PID to be processed by this handler.</param>
        public PidHandler(int pid)
        {
            this.pid = pid;

            sections = new Collection<Mpeg2Section>();
        }

        /// <summary>
        /// Process a sample.
        /// </summary>
        /// <param name="byteData">The sample data.</param>
        /// <param name="transportPacket">The transport packet header.</param>
        public void Process(byte[] byteData, TransportPacket transportPacket)
        {
            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
            {
                Logger.Instance.Dump(getPidID() + "Data Block", byteData, byteData.Length);
                Logger.Instance.Write(getPidID() + "Status: " + status);                
            }

            if (continuityCount == -1)
                continuityCount = transportPacket.ContinuityCount;
            else
            {
                continuityCount++;
                if (continuityCount > 15)
                    continuityCount = 0;
                if (transportPacket.ContinuityCount != continuityCount)
                {   
                    if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                        Logger.Instance.Write(getPidID() + "Continuity count failure: " + continuityCount + ":" + transportPacket.ContinuityCount);                    
                    continuityCount = transportPacket.ContinuityCount;
                    status = handlerStatus.awaitingStart;
                    discontinuities++;
                }
            }

            if (empty)
            {
                sections.Clear();
                empty = false;
            }

            bool done = false;

            siPacket = new SIPacket();
            siPacket.Process(byteData, transportPacket);

            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLERSI"))
                Logger.Instance.Write(getPidID() + "Created new SI packet");

            switch (status)
            {
                case handlerStatus.awaitingStart:
                    if (!transportPacket.StartIndicator)
                    {
                        if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                            Logger.Instance.Write(getPidID() + "Not start block");
                        return;
                    }

                    packetIndex = siPacket.DataIndex;

                    while (packetIndex < siPacket.ByteData.Length)
                        processNewSection(byteData, transportPacket, sections);

                    break;
                case handlerStatus.awaitingLengthByte1:
                    packetIndex = siPacket.Index;

                    lengthByte1 = siPacket.ByteData[packetIndex];
                    if (packetIndex == siPacket.ByteData.Length - 1)
                    {
                        status = handlerStatus.awaitingLengthByte2;
                        return;
                    }

                    packetIndex++;

                    lengthByte2 = siPacket.ByteData[packetIndex];
                    packetIndex++;

                    length = ((lengthByte1 & 0x0f) * 256) + lengthByte2;

                    done = createMpeg2Section(sections);
                    if (!done)
                        return;

                    status = handlerStatus.awaitingStart;

                    while (packetIndex < siPacket.ByteData.Length)
                        processNewSection(byteData, transportPacket, sections);

                    break;
                case handlerStatus.awaitingLengthByte2:
                    packetIndex = siPacket.Index;

                    lengthByte2 = siPacket.ByteData[packetIndex];
                    packetIndex++;

                    length = ((lengthByte1 & 0x0f) * 256) + lengthByte2;

                    done = createMpeg2Section(sections);
                    if (!done)
                        return;

                    status = handlerStatus.awaitingStart;

                    while (packetIndex < siPacket.ByteData.Length)
                        processNewSection(byteData, transportPacket, sections);

                    break;
                case handlerStatus.awaitingMoreData:
                    packetIndex = siPacket.Index;

                    for (; sectionIndex < length + 3; sectionIndex++)
                    {
                        if (packetIndex == siPacket.ByteData.Length)
                        {
                            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                                Logger.Instance.Write(getPidID() + "Need more data");
                            status = handlerStatus.awaitingMoreData;
                            return;
                        }

                        mpeg2Section.Data[sectionIndex] = siPacket.ByteData[packetIndex];
                        packetIndex++;
                    }

                    if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                        Logger.Instance.Write(getPidID() + "Got table");

                    if ((mpeg2Section.Data[1] & 0x80) != 0)
                    {
                        bool checkCRC = mpeg2Section.CheckCRC();
                        if (!checkCRC)
                        {
                            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                                Logger.Instance.Write(getPidID() + "CRC failed");
                        }
                        else
                        {
                            sections.Add(mpeg2Section);
                            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                                Logger.Instance.Dump(getPidID() + "MPEG2 Section", mpeg2Section.Data, mpeg2Section.Data.Length);
                        }
                    }
                    else
                    {
                        sections.Add(mpeg2Section);
                        if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                            Logger.Instance.Dump(getPidID() + "MPEG2 Section", mpeg2Section.Data, mpeg2Section.Data.Length);
                    }

                    status = handlerStatus.awaitingStart;

                    while (packetIndex < siPacket.ByteData.Length)
                        processNewSection(byteData, transportPacket, sections);

                    break;
                default:
                    break;
            }

            siPacket = null;
        }

        private void processNewSection(byte[] byteData, TransportPacket transportPacket, Collection<Mpeg2Section> sections)
        {
            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                Logger.Instance.Write(getPidID() + "Processing new section");

            if (siPacket == null)
            {
                siPacket = new SIPacket();
                siPacket.Process(byteData, transportPacket);
                packetIndex = siPacket.DataIndex;
            }

            table = siPacket.ByteData[packetIndex];
            if (table == 0xff)
            {
                if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                    Logger.Instance.Write(getPidID() + "Table: 0xff - rest of block ignored");
                packetIndex = siPacket.ByteData.Length;
                status = handlerStatus.awaitingStart;
                return;
            }

            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                Logger.Instance.Write(getPidID() + "Table: 0x" + table.ToString("X"));

            packetIndex++;

            if (packetIndex == siPacket.ByteData.Length)
            {
                status = handlerStatus.awaitingLengthByte1;
                return;
            }

            lengthByte1 = siPacket.ByteData[packetIndex];
            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                Logger.Instance.Write(getPidID() + "Length byte 1: " + lengthByte1);

            packetIndex++;

            if (packetIndex == siPacket.ByteData.Length)
            {
                status = handlerStatus.awaitingLengthByte2;
                return;
            }

            lengthByte2 = siPacket.ByteData[packetIndex];
            packetIndex++;
            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                Logger.Instance.Write(getPidID() + "Length byte 2: " + lengthByte2);

            length = ((lengthByte1 & 0x0f) * 256) + lengthByte2;
            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                Logger.Instance.Write(getPidID() + "Table length: " + length);

            createMpeg2Section(sections);
        }

        private bool createMpeg2Section(Collection<Mpeg2Section> sections)
        {
            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                Logger.Instance.Write(getPidID() + "Creating MPEG2 section");

            mpeg2Section = new Mpeg2Section();

            mpeg2Section.Data = new byte[length + 3];
            mpeg2Section.Length = length + 3;
            mpeg2Section.PID = pid;

            mpeg2Section.Data[0] = table;
            mpeg2Section.Data[1] = lengthByte1;
            mpeg2Section.Data[2] = lengthByte2;

            for (sectionIndex = 3; sectionIndex < length + 3; sectionIndex++)
            {
                if (packetIndex == siPacket.ByteData.Length)
                {
                    if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                        Logger.Instance.Write(getPidID() + "Need more data");
                    status = handlerStatus.awaitingMoreData;
                    return (false);
                }

                mpeg2Section.Data[sectionIndex] = siPacket.ByteData[packetIndex];
                packetIndex++;
            }

            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                Logger.Instance.Write(getPidID() + "Got table");

            if ((mpeg2Section.Data[1] & 0x80) != 0)
            {
                bool checkCRC = mpeg2Section.CheckCRC();
                if (!checkCRC)
                {
                    Logger.Instance.Write(getPidID() + "CRC failed");
                    return (true);
                }
            }

            sections.Add(mpeg2Section);
            if (RunParameters.Instance.TraceIDs.Contains("PIDHANDLER"))
                Logger.Instance.Dump(getPidID() + "MPEG2 Section", mpeg2Section.Data, mpeg2Section.Data.Length);

            return (true);
        }

        private string getPidID()
        {
            return ("PID handler 0x" + pid.ToString("X") + ": ");
        }
    }
}
