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
    /// The class that describes an event information table entry.
    /// </summary>
    public class EventInformationTableEntry
    {
        /// <summary>
        /// Get the event ID.
        /// </summary>
        public int EventID { get { return (eventID); } }
        /// <summary>
        /// Get the start time.
        /// </summary>
        public DateTime StartTime { get { return (startTime); } }
        /// <summary>
        /// Get the ETM location.
        /// </summary>
        public int ETMLocation { get { return (etmLocation); } }
        /// <summary>
        /// Get the duration.
        /// </summary>
        public TimeSpan Duration { get { return (duration); } }
        /// <summary>
        /// Get the event name.
        /// </summary>
        public MultipleString EventName { get { return (eventName); } }
        /// <summary>
        /// Get the collection of descriptors describing this table entry.
        /// </summary>
        internal Collection<DescriptorBase> Descriptors { get { return (descriptors); } }
        /// <summary>
        /// Get the total length of the entry.
        /// </summary>
        public int TotalLength { get { return (totalLength); } }

        /// <summary>
        /// Get the parental rating.
        /// </summary>
        public string ParentalRating
        {
            get
            {
                if (descriptors == null)
                    return (null);

                foreach (DescriptorBase descriptor in descriptors)
                {
                    ContentAdvisoryDescriptor contentAdvisoryDescriptor = descriptor as ContentAdvisoryDescriptor;
                    if (contentAdvisoryDescriptor != null)
                    {
                        if (contentAdvisoryDescriptor.Regions != null)
                        {
                            if (contentAdvisoryDescriptor.Regions[0].Description != null)
                                return (contentAdvisoryDescriptor.Regions[0].Description.ToString());
                        }
                    }
                }

                return (null);
            }
        }

        /// <summary>
        /// Get the audio quality.
        /// </summary>
        public string AudioQuality
        {
            get
            {
                if (descriptors == null)
                    return (null);

                foreach (DescriptorBase descriptor in descriptors)
                {
                    AC3AudioDescriptor audioDescriptor = descriptor as AC3AudioDescriptor;
                    if (audioDescriptor != null)
                    {
                        switch (audioDescriptor.NumberOfChannels)
                        {
                            case 1:
                                return ("mono");
                            case 2:
                                return ("stereo");
                            case 6:
                                return ("dolby digital");
                            default:
                                return (null);
                        }
                    }
                }

                return (null);
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
                    throw (new InvalidOperationException("Event Information Table Entry: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int eventID;
        private DateTime startTime;
        private int etmLocation;
        private TimeSpan duration;
        private MultipleString eventName;
        private Collection<DescriptorBase> descriptors;
        private int totalLength;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the EventInformationTableEntry class.
        /// </summary>
        public EventInformationTableEntry() { }

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
                eventID = ((byteData[lastIndex] & 0x3f) * 256) + byteData[lastIndex + 1];
                lastIndex += 2;

                startTime = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc) + new TimeSpan(Utils.Convert4BytesToInt(byteData, lastIndex) * TimeSpan.TicksPerSecond);
                lastIndex+= 4;

                etmLocation = (byteData[lastIndex] &0x30) >> 4;
                duration = new TimeSpan(
                    ((byteData[lastIndex] & 0x0f) * 16384) + 
                    ((byteData[lastIndex + 1] * 256) +
                    byteData[lastIndex + 2]) 
                    * TimeSpan.TicksPerSecond);
                lastIndex += 3;

                int titleLength = (int)byteData[lastIndex];
                lastIndex++;

                if (titleLength != 0)
                {
                    eventName = new MultipleString();
                    eventName.Process(byteData, lastIndex);                    
                    lastIndex += titleLength;
                }

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
                throw (new ArgumentOutOfRangeException("The Event Information Table entry message is short"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "EVENT INFORMATION TABLE ENTRY: Event ID: " + eventID +
                " Start time: " + startTime +
                " ETM loc: " + etmLocation +
                " Duration: " + duration);

            if (eventName != null)
            {
                Logger.IncrementProtocolIndent();
                eventName.LogMessage();
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
    }
}
