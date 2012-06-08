////////////////////////////////////////////////////////////////////////////////// 
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

using System;
using System.Collections.ObjectModel;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a Virtual Channel Table section.
    /// </summary>
    public class VirtualChannelTable
    {
        /// <summary>
        /// Get the collection of Virtual Channels in the section.
        /// </summary>
        public static Collection<VirtualChannel> Channels 
        { 
            get 
            {
                if (channels == null)
                    channels = new Collection<VirtualChannel>();
                return (channels); 
            } 
        }

        /// <summary>
        /// Return true if all sections have been received; false otherwise.
        /// </summary>
        public static bool Complete
        {
            get
            {
                if (sectionNumbers == null || sectionNumbers.Count == 0)
                    return(false);

                int expectedNumber = 0;

                foreach (int sectionNumber in sectionNumbers)
                {
                    if (sectionNumber != expectedNumber)
                        return (false);

                    expectedNumber++;
                }

                return (sectionNumbers[sectionNumbers.Count - 1] == lastSectionNumber);
            }
        }

        /// <summary>
        /// Get the total number of EPG entries.
        /// </summary>
        public static int EPGCount
        {
            get
            {
                int totalCount = 0;

                foreach (VirtualChannel channel in Channels)
                    totalCount += channel.EPGCollection.Count;
                
                return (totalCount);
            }
        }

        /// <summary>
        /// Get the protocol version.
        /// </summary>
        public int ProtocolVersion { get { return (protocolVersion); } }
        /// <summary>
        /// Get the collection of descriptors.
        /// </summary>
        internal Collection<DescriptorBase> Descriptors { get { return (descriptors); } }

        private static Collection<VirtualChannel> channels;
        private static Collection<int> sectionNumbers;
        private static int lastSectionNumber = -1;
        
        private int protocolVersion;
        private Collection<DescriptorBase> descriptors;

        private int lastIndex;

        /// <summary>
        /// Initialize a new instance of the VirtualChannelTable class.
        /// </summary>
        public VirtualChannelTable()
        {
            Logger.ProtocolIndent = "";
        }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        /// <param name="isCable">True is the section is a virtual cable section;false otherwise.</param>
        /// <param name="frequency">The frequency being processed.</param>
        public void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header, bool isCable, int frequency)
        {
            lastIndex = mpeg2Header.Index;
            lastSectionNumber = mpeg2Header.LastSectionNumber;

            protocolVersion = (int)byteData[lastIndex];
            lastIndex++;

            int channelCount = (int)byteData[lastIndex];
            lastIndex++;

            if (channelCount != 0)
            {
                if (channels == null)
                    channels = new Collection<VirtualChannel>();

                while (channelCount != 0)
                {
                    VirtualChannel channel = new VirtualChannel(frequency);
                    channel.Process(byteData, lastIndex, isCable);

                    addChannel(channel);

                    lastIndex += channel.TotalLength;
                    channelCount--;
                }
            }

            int descriptorLoopLength = ((byteData[lastIndex] & 0x03) * 256) + (int)byteData[lastIndex + 1];
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

        private void addChannel(VirtualChannel newChannel)
        {
            foreach (VirtualChannel oldChannel in Channels)
            {
                if (oldChannel.CollectionFrequency == newChannel.CollectionFrequency && oldChannel.SourceID == newChannel.SourceID)
                    return;
            }

            Channels.Add(newChannel);
        }

        internal static void AddSectionNumber(int newSectionNumber)
        {
            if (sectionNumbers == null)
                sectionNumbers = new Collection<int>();

            foreach (int oldSectionNumber in sectionNumbers)
            {
                if (oldSectionNumber == newSectionNumber)
                    return;

                if (oldSectionNumber > newSectionNumber)
                {
                    sectionNumbers.Insert(sectionNumbers.IndexOf(oldSectionNumber), newSectionNumber);
                    return;
                }
            }

            sectionNumbers.Add(newSectionNumber);
        }

        internal static VirtualChannel FindChannel(int frequency, int sourceID)
        {
            foreach (VirtualChannel channel in Channels)
            {
                if (channel.CollectionFrequency == frequency && channel.SourceID == sourceID)
                    return (channel);
            }

            return (null);
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "VIRTUAL CHANNEL TABLE: Version: " + protocolVersion);

            if (channels != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (VirtualChannel channelEntry in channels)
                    channelEntry.LogMessage();

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

        internal static void Clear()
        {
            channels = null;
            sectionNumbers = null;
            lastSectionNumber = -1;
        }
    }
}
