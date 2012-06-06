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
    /// The class that describes a PSIP virtual channel.
    /// </summary>
    public class VirtualChannel
    {
        /// <summary>
        /// Get the frequency the channel was found on.
        /// </summary>
        public int CollectionFrequency { get { return (collectionFrequency); } }
        /// <summary>
        /// Get the short name.
        /// </summary>
        public string ShortName { get { return (shortName); } }
        /// <summary>
        /// Get the major channel number.
        /// </summary>
        public int MajorChannelNumber { get { return (majorChannelNumber); } }
        /// <summary>
        /// Get the minor channel number.
        /// </summary>
        public int MinorChannelNumber { get { return (minorChannelNumber); } }
        /// <summary>
        /// Get the modulation mode.
        /// </summary>
        public int ModulationMode { get { return (modulationMode); } }
        /// <summary>
        /// Get the carrier frequency.
        /// </summary>
        public int Frequency { get { return (frequency); } }
        /// <summary>
        /// Get the transport stream ID (TSID).
        /// </summary>
        public int TransportStreamID { get { return (transportStreamID); } }
        /// <summary>
        /// Get the program number.
        /// </summary>
        public int ProgramNumber { get { return (programNumber); } }
        /// <summary>
        /// Get the ETM location.
        /// </summary>
        public int ETMLocation { get { return (etmLocation); } }
        /// <summary>
        /// Get the access controlled flag.
        /// </summary>
        public bool AccessControlled { get { return (accessControlled); } }
        /// <summary>
        /// Get the hidden flag.
        /// </summary>
        public bool Hidden { get { return (hidden); } }
        /// <summary>
        /// Get the path select flag.
        /// </summary>
        public bool PathSelect { get { return (pathSelect); } }
        /// <summary>
        /// Get the out of band flag.
        /// </summary>
        public bool OutOfBand { get { return (outOfBand); } }
        /// <summary>
        /// Get the hide guide flag.
        /// </summary>
        public bool HideGuide { get { return (hideGuide); } }
        /// <summary>
        /// Get the service type.
        /// </summary>
        public int ServiceType { get { return (serviceType); } }
        /// <summary>
        /// Get the source ID.
        /// </summary>
        public int SourceID { get { return (sourceID); } }
        /// <summary>
        /// Get the collection of descriptors describing this table entry.
        /// </summary>
        internal Collection<DescriptorBase> Descriptors { get { return (descriptors); } }
        /// <summary>
        /// Get the total length of the transport stream data.
        /// </summary>
        public int TotalLength { get { return (totalLength); } }

        /// <summary>
        /// Get the collection of EPG entries for this channel.
        /// </summary>
        public Collection<EPGEntry> EPGCollection
        {
            get
            {
                if (epgCollection == null)
                    epgCollection = new Collection<EPGEntry>();
                return (epgCollection);
            }
        }

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
                    throw (new InvalidOperationException("Virtual Channel: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int collectionFrequency;
        private string shortName;
        private int majorChannelNumber;
        private int minorChannelNumber;
        private int modulationMode;
        private int frequency;
        private int transportStreamID;
        private int programNumber;
        private int etmLocation;
        private bool accessControlled;
        private bool hidden;
        private bool pathSelect;
        private bool outOfBand;
        private bool hideGuide;
        private int serviceType;
        private int sourceID;
        private Collection<DescriptorBase> descriptors;
        private int totalLength;

        private Collection<EPGEntry> epgCollection;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the VirtualChannel class.
        /// </summary>
        /// <param name="collectionFrequency">The frequency the channel was collected on.</param>
        public VirtualChannel(int collectionFrequency) 
        {
            this.collectionFrequency = collectionFrequency;
        }

        /// <summary>
        /// Parse the entry.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the transport stream.</param>
        /// <param name="index">Index of the first byte of the transport stream in the MPEG2 section.</param>
        /// <param name="isCable">True if the entry is for cable; false otherwise.</param>
        internal void Process(byte[] byteData, int index, bool isCable)
        {
            lastIndex = index;

            try
            {
                shortName = Utils.GetUnicodeString(byteData, lastIndex, 14).Replace((char)0x00, '?').Replace("?", "");
                lastIndex += 14;

                majorChannelNumber = (Utils.Convert4BytesToInt(byteData, lastIndex) >> 18) & 0x3ff;
                minorChannelNumber = (Utils.Convert4BytesToInt(byteData, lastIndex) >> 8) & 0x3ff;
                lastIndex += 3;

                modulationMode = (int)byteData[lastIndex];
                lastIndex++;

                frequency = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                transportStreamID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                programNumber = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                etmLocation = byteData[lastIndex] >> 6;
                accessControlled = ((byteData[lastIndex] & 0x20) != 0);
                hidden = ((byteData[lastIndex] & 0x10) != 0);

                if (isCable)
                {
                    pathSelect = ((byteData[lastIndex] & 0x08) != 0);
                    outOfBand = ((byteData[lastIndex] & 0x04) != 0);
                }

                hideGuide = ((byteData[lastIndex] & 0x02) != 0);
                lastIndex++;

                serviceType = byteData[lastIndex] & 0x03f;
                lastIndex++;

                sourceID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;                

                int descriptorLoopLength = ((byteData[lastIndex] & 0x03) * 256) + (int)byteData[lastIndex + 1];
                lastIndex += 2;

                totalLength = (lastIndex - index) + descriptorLoopLength;

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
                throw (new ArgumentOutOfRangeException("The Virtual Channel Table entry message is short"));
            }
        }

        internal bool AddEPGEntry(EPGEntry newEntry)
        {
            foreach (EPGEntry oldEntry in EPGCollection)
            {
                if (newEntry.StartTime == oldEntry.StartTime)
                {
                    EPGCollection.Insert(EPGCollection.IndexOf(oldEntry), newEntry);
                    EPGCollection.Remove(oldEntry);
                    return (false);
                }
                else
                {
                    if (newEntry.StartTime > oldEntry.StartTime && (newEntry.StartTime + newEntry.Duration) <= (oldEntry.StartTime + oldEntry.Duration))
                        return (false);

                    if (newEntry.StartTime < oldEntry.StartTime)
                    {
                        EPGCollection.Insert(EPGCollection.IndexOf(oldEntry), newEntry);
                        return (true);
                    }
                }
            }

            EPGCollection.Add(newEntry);

            return (true);                
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "VIRTUAL CHANNEL: Name: " + shortName +
                " Major ch: " + majorChannelNumber +
                " Minor ch: " + minorChannelNumber +
                " Modulation: " + modulationMode +
                " Frequency: " + frequency +
                " TSID: " + transportStreamID +
                " Prog no: " + programNumber +
                " ETM loc: " + etmLocation +
                " Access ctrl: " + accessControlled +
                " Hidden: " + hidden +
                " Path sel: " + pathSelect +
                " Out of band: " + outOfBand +
                " Hide guide: " + hideGuide +
                " Service type: " + serviceType +
                " Source ID: " + sourceID);

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
