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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a master guide table entry.
    /// </summary>
    public class MasterGuideTableEntry
    {
        /// <summary>
        /// Get the table type.
        /// </summary>
        public int TableType { get { return (tableType); } }
        /// <summary>
        /// Get the table's PID.
        /// </summary>
        public int Pid { get { return (pid); } }
        /// <summary>
        /// Get the version number.
        /// </summary>
        public int Version { get { return (version); } }
        /// <summary>
        /// Get the number of bytes.
        /// </summary>
        public int ByteCount { get { return (byteCount); } }
        /// <summary>
        /// Get the collection of descriptors describing this table entry.
        /// </summary>
        internal Collection<DescriptorBase> Descriptors { get { return (descriptors); } }
        /// <summary>
        /// Get the total length of the transport stream data.
        /// </summary>
        public int TotalLength { get { return (totalLength); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the transport stream.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The transport stream has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("Master Guide Table Entry: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int tableType;
        private int pid;
        private int version;
        private int byteCount;
        private Collection<DescriptorBase> descriptors;
        private int totalLength;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MasterGuideTableEntry class.
        /// </summary>
        public MasterGuideTableEntry() { }

        /// <summary>
        /// Parse the entry.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the transport stream.</param>
        /// <param name="index">Index of the first byte of the transport stream in the MPEG2 section.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                tableType = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                pid = ((byteData[lastIndex] & 0x1f) * 256) + byteData[lastIndex + 1];
                lastIndex += 2;

                version = byteData[lastIndex] & 0x01f;
                lastIndex++;

                byteCount = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                int descriptorLoopLength = ((byteData[lastIndex] & 0x0f) * 256) + (int)byteData[lastIndex + 1];
                lastIndex += 2;

                totalLength = descriptorLoopLength + 11;

                if (descriptorLoopLength != 0)
                {
                    descriptors = new Collection<DescriptorBase>();

                    while (descriptorLoopLength != 0)
                    {
                        DescriptorBase descriptor = DescriptorBase.AtscInstance(byteData, lastIndex);
                        descriptors.Add(descriptor);

                        lastIndex = descriptor.Index;
                        descriptorLoopLength -= descriptor.TotalLength;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The Master Guide Table entry message is short"));
            }
        }

        /// <summary>
        /// Validate the fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MASTER GUIDE TABLE ENTRY: Table type: " + tableType +
                " PID: " + pid +
                " Version: " + version +
                " Byte ct: " + byteCount);

            if (descriptors != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (DescriptorBase descriptor in descriptors)
                    descriptor.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
