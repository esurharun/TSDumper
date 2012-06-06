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
    /// The class that describes a Network Information section.
    /// </summary>
    public class NetworkInformationSection
    {
        /// <summary>
        /// Get the collection of network information sections.
        /// </summary>
        public static Collection<NetworkInformationSection> NetworkInformationSections
        {
            get
            {
                if (networkInformationSections == null)
                    networkInformationSections = new Collection<NetworkInformationSection>();
                return (networkInformationSections);
            }
        }

        /// <summary>
        /// Get the section number.
        /// </summary>
        public int SectionNumber { get { return (sectionNumber); } }
        /// <summary>
        /// Get the section number.
        /// </summary>
        public int LastSectionNumber { get { return (lastSectionNumber); } }
        /// <summary>
        /// Get the bouquet identification.
        /// </summary>
        public int NetworkID { get { return (networkID); } }
        /// <summary>
        /// Get the collection of descriptors in the section.
        /// </summary>
        internal Collection<DescriptorBase> NetworkDescriptors { get { return (networkDescriptors); } }
        /// <summary>
        /// Get the collection of transport streams in the section.
        /// </summary>
        public Collection<TransportStream> TransportStreams { get { return (transportStreams); } }

        private int sectionNumber;
        private int lastSectionNumber;
        private int networkID;
        private Collection<DescriptorBase> networkDescriptors;
        private Collection<TransportStream> transportStreams;

        private static Collection<NetworkInformationSection> networkInformationSections;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the NetworkInformationSection class.
        /// </summary>
        internal NetworkInformationSection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        internal void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;
            networkID = mpeg2Header.TableIDExtension;
            sectionNumber = mpeg2Header.SectionNumber;
            lastSectionNumber = mpeg2Header.LastSectionNumber;

            int networkDescriptorLength = ((byteData[lastIndex] & 0x0f) * 256) + byteData[lastIndex + 1];
            lastIndex += 2;

            if (networkDescriptorLength != 0)
            {
                networkDescriptors = new Collection<DescriptorBase>();

                while (networkDescriptorLength > 0)
                {
                    DescriptorBase descriptor = DescriptorBase.Instance(byteData, lastIndex);

                    if (!descriptor.IsEmpty)
                    {
                        networkDescriptors.Add(descriptor);

                        lastIndex = descriptor.Index;
                        networkDescriptorLength -= descriptor.TotalLength;
                    }
                    else
                    {
                        lastIndex += DescriptorBase.MinimumDescriptorLength;
                        networkDescriptorLength -= DescriptorBase.MinimumDescriptorLength;
                    }
                }
            }

            int transportStreamLoopLength = ((byteData[lastIndex] & 0x0f) * 256) + byteData[lastIndex + 1];
            lastIndex += 2;

            if (transportStreamLoopLength != 0)
            {
                transportStreams = new Collection<TransportStream>();

                while (transportStreamLoopLength > 0)
                {
                    TransportStream transportStream = new TransportStream();
                    transportStream.Process(byteData, lastIndex, Scope.All);
                    transportStreams.Add(transportStream);

                    lastIndex = transportStream.Index;
                    transportStreamLoopLength -= transportStream.TotalLength;
                }
            }

            lastIndex += transportStreamLoopLength;
        }

        /// <summary>
        /// Log the section fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "NETWORK INFORMATION SECTION: Network ID: " + networkID);

            if (networkDescriptors != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (DescriptorBase descriptor in networkDescriptors)
                    descriptor.LogMessage();

                Logger.DecrementProtocolIndent();
            }

            if (transportStreams != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (TransportStream transportStream in transportStreams)
                    transportStream.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }

        /// <summary>
        /// Process an MPEG2 section from the network information table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        /// <returns>A Network Information section instance.</returns>
        public static NetworkInformationSection ProcessNetworkInformationTable(byte[] byteData)
        {
            Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();

            try
            {
                mpeg2Header.Process(byteData);

                if (mpeg2Header.Current)
                {
                    NetworkInformationSection networkInformationSection = new NetworkInformationSection();
                    networkInformationSection.Process(byteData, mpeg2Header);

                    if (RunParameters.Instance.DebugIDs.Contains("NITSECTIONS"))
                        networkInformationSection.LogMessage();

                    return (networkInformationSection);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Error processing Network Information Section message: " + e.Message);
            }

            return (null);
        }

        /// <summary>
        /// Add a network information section.
        /// </summary>
        /// <param name="newSection">The section to be added.</param>
        /// <returns>True if the section was added; false otherwise.</returns>
        public static bool AddSection(NetworkInformationSection newSection)
        {
            foreach (NetworkInformationSection oldSection in NetworkInformationSections)
            {
                if (oldSection.NetworkID == newSection.NetworkID && oldSection.SectionNumber == newSection.SectionNumber)
                    return (false);

                if (oldSection.NetworkID == newSection.NetworkID && oldSection.SectionNumber > newSection.SectionNumber)
                {
                    NetworkInformationSections.Insert(NetworkInformationSections.IndexOf(oldSection), newSection);
                    if (Logger.ProtocolLogger != null)
                        Logger.ProtocolLogger.Write("NID " + newSection.NetworkID + " Section " + newSection.SectionNumber + " inserted");
                    return (true);
                }

                if (oldSection.NetworkID > newSection.NetworkID)
                {
                    NetworkInformationSections.Insert(NetworkInformationSections.IndexOf(oldSection), newSection);
                    if (Logger.ProtocolLogger != null)
                        Logger.ProtocolLogger.Write("NID " + newSection.NetworkID + " Section " + newSection.SectionNumber + " inserted");
                    return (true);
                }
            }

            NetworkInformationSections.Add(newSection);
            if (Logger.ProtocolLogger != null)
                Logger.ProtocolLogger.Write("NID " + newSection.NetworkID + " Section " + newSection.SectionNumber + " added");

            return (true);
        }

        /// <summary>
        /// Check all sections have been loaded.
        /// </summary>
        /// <returns>True if all sections have been loaded; false otherwise.</returns>
        public static bool CheckAllLoaded()
        {
            if (NetworkInformationSections.Count == 0)
                return (false);

            int lastNetworkID = 0;
            int highestSectionNumber = 0;
            int expectedSectionNumber = 0;

            foreach (NetworkInformationSection section in NetworkInformationSections)
            {
                if (lastNetworkID == 0)
                {
                    if (section.SectionNumber != 0)
                        return (false);
                    else
                    {
                        lastNetworkID = section.NetworkID;
                        highestSectionNumber = section.LastSectionNumber;
                        expectedSectionNumber = 1;
                    }
                }
                else
                {
                    if (section.NetworkID == lastNetworkID)
                    {
                        if (section.SectionNumber != expectedSectionNumber)
                            return (false);
                        else
                            expectedSectionNumber++;
                    }
                    else
                    {
                        if (expectedSectionNumber - 1 != highestSectionNumber)
                            return (false);
                        else
                        {
                            if (section.SectionNumber != 0)
                                return (false);
                            else
                            {
                                lastNetworkID = section.NetworkID;
                                highestSectionNumber = section.LastSectionNumber;
                                expectedSectionNumber = 1;
                            }
                        }
                    }
                }
            }

            return (expectedSectionNumber - 1 == highestSectionNumber);
        }

        /// <summary>
        /// Get the frequency for a transport stream.
        /// </summary>
        /// <param name="originalNetworkID">The network ID.</param>
        /// <param name="transportStreamID">The transport stream ID</param>
        /// <returns>The frequency or zero if the transport stream cannot be located.</returns>
        public static int GetFrequency(int originalNetworkID, int transportStreamID)
        {
            if (networkInformationSections == null)
                return (0);

            foreach (NetworkInformationSection section in NetworkInformationSections)
            {
                if (section.NetworkID == originalNetworkID)
                {
                    if (section.TransportStreams != null)
                    {
                        foreach (TransportStream stream in section.TransportStreams)
                        {
                            if (stream.TransportStreamID == transportStreamID && stream.Descriptors != null)
                            {
                                foreach (DescriptorBase descriptor in stream.Descriptors)
                                {
                                    DVBDeliverySystemDescriptor deliveryDescriptor = descriptor as DVBDeliverySystemDescriptor;
                                    if (deliveryDescriptor != null)
                                        return (deliveryDescriptor.Frequency);
                                }
                            }
                        }
                    }
                }
            }

            return (0);
        }
    }
}
