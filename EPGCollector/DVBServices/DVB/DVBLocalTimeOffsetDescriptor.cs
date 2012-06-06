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
    /// DVB Local Time Offset descriptor class.
    /// </summary>
    internal class DVBLocalTimeOffsetDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the collection of time offset entries.
        /// </summary>
        public Collection<DVBLocalTimeOffsetEntry> TimeOffsetEntries
        {
            get
            {
                if (timeOffsetEntries == null)
                    timeOffsetEntries = new Collection<DVBLocalTimeOffsetEntry>();
                return (timeOffsetEntries);
            }
        }

        /// <summary>
        /// Get the index of the next byte in the EIT section following this descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("ContentDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private Collection<DVBLocalTimeOffsetEntry> timeOffsetEntries;        

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBLocalTimeOffsetDescriptor class.
        /// </summary>
        internal DVBLocalTimeOffsetDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The mpeg2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the mpeg2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                while (lastIndex < byteData.Length - 4)
                {
                    DVBLocalTimeOffsetEntry entry = new DVBLocalTimeOffsetEntry();

                    entry.CountryCode = Utils.GetString(byteData, lastIndex, 3);
                    lastIndex += 3;

                    entry.Region = (int)byteData[lastIndex] >> 2;
                    entry.OffsetPositive = (byteData[lastIndex] & 0x01) == 0;
                    lastIndex++;

                    int hoursTens = (int)byteData[lastIndex] >> 4;
                    int hoursUnits = (int)byteData[lastIndex] & 0x0f;
                    int minutesTens = (int)byteData[lastIndex + 1] >> 4;
                    int minutesUnits = (int)byteData[lastIndex + 1] & 0x0f;
                    entry.TimeOffset = new TimeSpan((hoursTens * 10) + hoursUnits, (minutesTens * 10) + minutesUnits, 0);
                    lastIndex += 2;

                    entry.ChangeTime = getChangeTime(byteData, lastIndex);
                    lastIndex += 5;

                    int nextHoursTens = (int)byteData[lastIndex] >> 4;
                    int nextHoursUnits = (int)byteData[lastIndex] & 0x0f;
                    int nextMinutesTens = (int)byteData[lastIndex + 1] >> 4;
                    int nextMinutesUnits = (int)byteData[lastIndex + 1] & 0x0f;
                    entry.NextTimeOffset = new TimeSpan((nextHoursTens * 10) + nextHoursUnits, (nextMinutesTens * 10) + nextMinutesUnits, 0);
                    lastIndex += 2;

                    TimeOffsetEntries.Add(entry);
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Local Time Offset Descriptor message is short"));
            }
        }

        private DateTime getChangeTime(byte[] byteData, int index)
        {
            int startDate = Utils.Convert2BytesToInt(byteData, index);

            int year = (int)((startDate - 15078.2) / 365.25);
            int month = (int)(((startDate - 14956.1) - (int)(year * 365.25)) / 30.6001);
            int day = (startDate - 14956) - (int)(year * 365.25) - (int)(month * 30.6001);

            int adjust;

            if (month == 14 || month == 15)
                adjust = 1;
            else
                adjust = 0;

            year = year + 1900 + adjust;
            month = month - 1 - (adjust * 12);

            int hour1 = (int)byteData[index + 2] >> 4;
            int hour2 = (int)byteData[index + 2] & 0x0f;
            int hour = (hour1 * 10) + hour2;

            int minute1 = (int)byteData[index + 3] >> 4;
            int minute2 = (int)byteData[index + 3] & 0x0f;
            int minute = (minute1 * 10) + minute2;

            int second1 = (int)byteData[index + 4] >> 4;
            int second2 = (int)byteData[index + 4] & 0x0f;
            int second = (second1 * 10) + second2;

            try
            {
                DateTime utcStartTime = new DateTime(year, month, day, hour, minute, second);
                return(utcStartTime.ToLocalTime());
            }
            catch (ArgumentOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The start time element(s) are out of range"));
            }
            catch (ArgumentException)
            {
                throw (new ArgumentOutOfRangeException("The start time element(s) result in a start time that is out of range"));
            }
        }

        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal override void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        internal override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB LOCAL TIME OFFSET DESCRIPTOR");

            foreach (DVBLocalTimeOffsetEntry entry in TimeOffsetEntries)
            {
                Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "    DVB LOCAL TIME OFFSET ENTRY:" +
                    " Country code: " + entry.CountryCode +
                    " Region: " + entry.Region +
                    " Polarity: " + entry.OffsetPositive +
                    " Time offset: " + entry.TimeOffset +
                    " Change time: " + entry.ChangeTime +
                    " Next time offset: " + entry.NextTimeOffset);
            }
        }
    }
}
