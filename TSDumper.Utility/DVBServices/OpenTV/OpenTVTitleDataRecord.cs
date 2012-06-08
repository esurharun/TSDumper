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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes an OpenTV title data record.
    /// </summary>
    internal class OpenTVTitleDataRecord : OpenTVRecordBase
    {
        /// <summary>
        /// Get the tag value for this record.
        /// </summary>
        public const int TagValue = 0xb5;

        /// <summary>
        /// Get the start time of the event.
        /// </summary>
        public TimeSpan StartTimeOffset { get { return (startTimeOffset); } }
        /// <summary>
        /// Get the duration of the event.
        /// </summary>
        public TimeSpan Duration { get { return (duration); } }
        /// <summary>
        /// Get the theme identification of the event.
        /// </summary>
        public int CategoryID { get { return (categoryID); } }        
        /// <summary>
        /// Get the flags field of the event.
        /// </summary>
        public byte[] Flags { get { return (flags); } }
        /// <summary>
        /// Get the name of the event.
        /// </summary>
        public byte[] EventName { get { return (eventName); } }

        /// <summary>
        /// Get the decompressed event name.
        /// </summary>
        public string DecodedEventName { get { return (SingleTreeDictionaryEntry.DecodeData(eventName)); } }
        
        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the record.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The record has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("OpenTVTitleDataRecord: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private TimeSpan startTimeOffset;
        private TimeSpan duration;
        private int categoryID;
        private byte[] flags;
        private byte[] eventName;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the OpenTVTitleDataRecord class.
        /// </summary>
        internal OpenTVTitleDataRecord() { }

        /// <summary>
        /// Parse the record.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the record.</param>
        /// <param name="index">Index of the first byte of the record data in the MPEG2 section.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                // Source value to 2 second resolution
                startTimeOffset = getTime((byteData[lastIndex] * 512) + (byteData[lastIndex + 1] * 2));
                lastIndex += 2;

                duration = getTime(Utils.Convert2BytesToInt(byteData, lastIndex) * 2);
                lastIndex += 2;

                categoryID = (int)byteData[lastIndex];
                lastIndex++;

                flags = Utils.GetBytes(byteData, lastIndex, 2);
                lastIndex += 2;

                eventName = Utils.GetBytes(byteData, lastIndex, Length - 7);
                lastIndex += Length - 7;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("lastIndex = " + lastIndex));
            }
        }

        private TimeSpan getTime(int totalSeconds)
        {
            int days = totalSeconds / 86400;
            int hours = (totalSeconds - (days * 86400)) / 3600;
            int minutes = (totalSeconds - ((days * 86400) + (hours * 3600))) / 60;
            int seconds = totalSeconds % 60;

            return (new TimeSpan(days, hours, minutes, seconds));
        }

        /// <summary>
        /// Validate the record data fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A record data field is not valid.
        /// </exception>
        internal override void Validate() { }

        /// <summary>
        /// Log the record data fields.
        /// </summary>
        internal override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "");
            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "OPENTV TITLE DATA RECORD: " +
                "Start time: " + startTimeOffset +
                " Duration: " + duration +
                " Category: " + categoryID +
                " Unknown: " + Utils.ConvertToHex(flags) +
                " Name: " + Utils.ConvertToHex(eventName));
        }
    }
}
