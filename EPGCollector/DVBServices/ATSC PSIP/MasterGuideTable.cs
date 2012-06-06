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
    /// The class that describes a Master Guide Table section.
    /// </summary>
    public class MasterGuideTable
    {
        /// <summary>
        /// Get the protocol version.
        /// </summary>
        public int ProtocolVersion { get { return (protocolVersion); } }
        /// <summary>
        /// Get the collection of table definitions.
        /// </summary>
        public Collection<MasterGuideTableEntry> TableEntries { get { return (tableEntries); } }
        /// <summary>
        /// Get the collection of descriptors.
        /// </summary>
        internal Collection<DescriptorBase> Descriptors { get { return (descriptors); } }

        private int protocolVersion;
        private Collection<MasterGuideTableEntry> tableEntries;
        private Collection<DescriptorBase> descriptors;

        private int lastIndex;

        /// <summary>
        /// Initialize a new instance of the MasterGuideTable class.
        /// </summary>
        public MasterGuideTable()
        {
            Logger.ProtocolIndent = "";
        }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        public void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;            

            protocolVersion = (int)byteData[lastIndex];
            lastIndex++;

            int tableCount = Utils.Convert2BytesToInt(byteData, lastIndex);
            lastIndex+= 2;

            if (tableCount != 0)
            {
                tableEntries = new Collection<MasterGuideTableEntry>();

                while (tableCount != 0)
                {
                    MasterGuideTableEntry tableEntry = new MasterGuideTableEntry();
                    tableEntry.Process(byteData, lastIndex);

                    tableEntries.Add(tableEntry);

                    lastIndex += tableEntry.TotalLength;
                    tableCount--;
                }
            }

            int descriptorLoopLength = ((byteData[lastIndex] & 0x0f) * 256) + (int)byteData[lastIndex + 1];
            lastIndex += 2;

            if (descriptorLoopLength != 0)
            {
                descriptors = new Collection<DescriptorBase>();

                while (descriptorLoopLength != 0)
                {
                    while (descriptorLoopLength != 0)
                    {
                        DescriptorBase descriptor = DescriptorBase.AtscInstance(byteData, lastIndex);
                        descriptors.Add(descriptor);

                        lastIndex = descriptor.Index;
                        descriptorLoopLength -= descriptor.TotalLength;
                    }
                }
            }

            Validate();
        }

        /// <summary>
        /// Validate the entry fields.
        /// </summary>
        public void Validate() { }

        /// <summary>
        /// Log the entry fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MASTER GUIDE TABLE: Version: " + protocolVersion);

            if (tableEntries != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (MasterGuideTableEntry tableEntry in tableEntries)
                    tableEntry.LogMessage();

                Logger.DecrementProtocolIndent();
            }     

            if (descriptors != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (DescriptorBase descriptor in descriptors)
                    descriptor.LogMessage();

                Logger.DecrementProtocolIndent();
            }                
        }

        internal int[] GetETTPids()
        {
            int pidCount = 0;

            foreach (MasterGuideTableEntry tableEntry in TableEntries)
            {
                if (tableEntry.TableType >= 0x200 && tableEntry.TableType <= 0x27f)
                    pidCount++;
            }

            int[] pids = new int[pidCount];

            int index = 0;

            foreach (MasterGuideTableEntry tableEntry in TableEntries)
            {
                if (tableEntry.TableType >= 0x200 && tableEntry.TableType <= 0x27f)
                {
                    pids[index] = tableEntry.Pid;
                    index++;
                }
            }

            return (pids);
        }

        internal int[] GetEITPids()
        {
            int pidCount = 0;

            foreach (MasterGuideTableEntry tableEntry in TableEntries)
            {
                if (tableEntry.TableType >= 0x100 && tableEntry.TableType <= 0x17f)
                    pidCount++;
            }

            int[] pids = new int[pidCount];

            int index = 0;

            foreach (MasterGuideTableEntry tableEntry in TableEntries)
            {
                if (tableEntry.TableType >= 0x100 && tableEntry.TableType <= 0x17f)
                {
                    pids[index] = tableEntry.Pid;
                    index++;
                }
            }

            return (pids);
        }

        internal int[] GetRRTPids()
        {
            int pidCount = 0;

            foreach (MasterGuideTableEntry tableEntry in TableEntries)
            {
                if (tableEntry.TableType >= 0x300 && tableEntry.TableType <= 0x3ff)
                    pidCount++;
            }

            int[] pids = new int[pidCount];

            int index = 0;

            foreach (MasterGuideTableEntry tableEntry in TableEntries)
            {
                if (tableEntry.TableType >= 0x300 && tableEntry.TableType <= 0x3ff)
                {
                    pids[index] = tableEntry.Pid;
                    index++;
                }
            }

            return (pids);
        }

        internal int[] GetRRTRegions()
        {
            int regionCount = 0;

            foreach (MasterGuideTableEntry tableEntry in TableEntries)
            {
                if (tableEntry.TableType >= 0x300 && tableEntry.TableType <= 0x3ff)
                    regionCount++;
            }

            int[] regions = new int[regionCount];

            int index = 0;

            foreach (MasterGuideTableEntry tableEntry in TableEntries)
            {
                if (tableEntry.TableType >= 0x300 && tableEntry.TableType <= 0x3ff)
                {
                    regions[index] = tableEntry.TableType - 0x300;
                    index++;
                }
            }

            return (regions);
        }
    }
}
