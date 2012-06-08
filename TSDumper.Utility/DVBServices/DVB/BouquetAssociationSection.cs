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
    /// The class that describes a Bouquet Association section.
    /// </summary>
    public class BouquetAssociationSection
    {
        /// <summary>
        /// Get the collection of bouquet association sections.
        /// </summary>
        public static Collection<BouquetAssociationSection> BouquetAssociationSections 
        { 
            get 
            {
                if (bouquetAssociationSections == null)
                    bouquetAssociationSections = new Collection<BouquetAssociationSection>();
                return (bouquetAssociationSections); 
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
        public int BouquetID { get { return (bouquetID); } }
        /// <summary>
        /// Get the collection of bouquet descriptions in the section.
        /// </summary>
        internal Collection<DescriptorBase> BouquetDescriptors { get { return (bouquetDescriptors); } }
        /// <summary>
        /// Get the collection of transport streams in the section.
        /// </summary>
        public Collection<TransportStream> TransportStreams { get { return (transportStreams); } }

        /// <summary>
        /// Get the name of the bouquet.
        /// </summary>
        public string Name
        {
            get
            {
                if (bouquetDescriptors == null)
                    return ("Undefined");
                else
                {
                    foreach (DescriptorBase descriptor in bouquetDescriptors)
                    {
                        DVBBouquetNameDescriptor nameDescriptor = descriptor as DVBBouquetNameDescriptor;
                        if (nameDescriptor != null)
                        {
                            if (nameDescriptor.Name != null)
                                return (nameDescriptor.Name);
                            else
                                return ("** Missing **");
                        }
                    }

                    return ("** Not Present **");
                }
            }
        }

        private int sectionNumber;
        private int lastSectionNumber;
        private int bouquetID;
        private Collection<DescriptorBase> bouquetDescriptors;
        private Collection<TransportStream> transportStreams;

        private static Collection<BouquetAssociationSection> bouquetAssociationSections;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the BouquetAssociationSection class.
        /// </summary>
        internal BouquetAssociationSection()
        {
            bouquetDescriptors = new Collection<DescriptorBase>();
        }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        internal void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;
            bouquetID = mpeg2Header.TableIDExtension;
            sectionNumber = mpeg2Header.SectionNumber;
            lastSectionNumber = mpeg2Header.LastSectionNumber;
            
            int bouquetDescriptorLength = ((byteData[lastIndex] & 0x0f) * 256) + byteData[lastIndex + 1];
            lastIndex += 2;

            if (bouquetDescriptorLength != 0)
            {
                bouquetDescriptors = new Collection<DescriptorBase>();

                while (bouquetDescriptorLength > 0)
                {
                    DescriptorBase descriptor = DescriptorBase.Instance(byteData, lastIndex, Scope.Bouquet);

                    if (!descriptor.IsEmpty)
                    {
                        bouquetDescriptors.Add(descriptor);
                        lastIndex = descriptor.Index;

                        bouquetDescriptorLength -= descriptor.TotalLength;
                    }
                    else
                    {
                        lastIndex += DescriptorBase.MinimumDescriptorLength;
                        bouquetDescriptorLength -= DescriptorBase.MinimumDescriptorLength;
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
                    transportStream.Process(byteData, lastIndex, Scope.Bouquet);
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BOUQUET ASSOCIATION SECTION: Bouquet ID: " + bouquetID);
            
            if (bouquetDescriptors != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (DescriptorBase descriptor in bouquetDescriptors)
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
        /// Process an MPEG2 section from the bouquet association table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        /// <returns>A Bouquet Association instance.</returns>
        public static BouquetAssociationSection ProcessBouquetAssociationTable(byte[] byteData)
        {
            Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();

            try
            {
                mpeg2Header.Process(byteData);

                if (mpeg2Header.Current)
                {
                    BouquetAssociationSection bouquetAssociationSection = new BouquetAssociationSection();

                    bouquetAssociationSection.Process(byteData, mpeg2Header);
                    bouquetAssociationSection.LogMessage();
                    return (bouquetAssociationSection);                    
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Error processing Bouquet Association Section message: " + e.Message);
            }

            return (null);
        }

        /// <summary>
        /// Add a section to the collection.
        /// </summary>
        /// <param name="newSection">The section to be added.</param>
        /// <returns>True if the section was added; false it it already existed in the collection.</returns>
        public static bool AddSection(BouquetAssociationSection newSection)
        {
            foreach (BouquetAssociationSection oldSection in BouquetAssociationSections)
            {
                if (oldSection.BouquetID == newSection.BouquetID && oldSection.SectionNumber == newSection.SectionNumber)
                    return (false);

                if (oldSection.BouquetID == newSection.BouquetID && oldSection.SectionNumber > newSection.SectionNumber)
                {
                    BouquetAssociationSections.Insert(BouquetAssociationSections.IndexOf(oldSection), newSection);
                    return (true);
                }

                if (oldSection.BouquetID > newSection.BouquetID)
                {
                    BouquetAssociationSections.Insert(BouquetAssociationSections.IndexOf(oldSection), newSection);
                    return (true);
                }
            }

            BouquetAssociationSections.Add(newSection);
            
            return (true);
        }

        /// <summary>
        /// Find a specified bouquet.
        /// </summary>
        /// <param name="bouquetID">The ID of the bouquet.</param>
        /// <returns>A BouquetAssociationSection instance or null if the bouquet cannot be located.</returns>
        public static BouquetAssociationSection FindBouquet(int bouquetID)
        {
            foreach (BouquetAssociationSection section in BouquetAssociationSections)
            {
                if (section.BouquetID == bouquetID)
                    return (section);
            }

            return (null);
        }

        /// <summary>
        /// Find the name of a specified bouquet.
        /// </summary>
        /// <param name="bouquetID">The ID of the bouquet.</param>
        /// <returns>The name or null if the bouquet cannot be located.</returns>
        public static string FindBouquetName(int bouquetID)
        {
            BouquetAssociationSection bouquetSection = FindBouquet(bouquetID);
            if (bouquetSection == null)
                return (null);

            return (bouquetSection.Name);
        }
    }
}
