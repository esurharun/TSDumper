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
    /// The class that describes MediaHighway2 title data.
    /// </summary>
    public class MediaHighway2TitleData
    {
        /// <summary>
        /// Get the channel identification.
        /// </summary>
        public int ChannelID { get { return (channelID); } }
        /// <summary>
        /// Get the event identification.
        /// </summary>
        public int EventID { get { return (eventID); } }
        /// <summary>
        /// Get the start time of the event.
        /// </summary>
        public DateTime StartTime { get { return (baseDate + new TimeSpan(hours, minutes, 0)); }}
        /// <summary>
        /// Get the duration of the event.
        /// </summary>
        public TimeSpan Duration { get { return (duration); } }
        /// <summary>
        /// Get the theme identification of the event.
        /// </summary>
        public int CategoryID { get { return ((mainCategory << 6) + subCategory); } }
        /// <summary>
        /// Get the theme group of the event.
        /// </summary>
        public int MainCategory { get { return (mainCategory); } }
        /// <summary>
        /// Get the theme subgroup of the event.
        /// </summary>
        public int SubCategory { get { return (subCategory); } }
        /// <summary>
        /// Get the name of the event.
        /// </summary>
        public string EventName { get { return (eventName); } }
        /// <summary>
        /// Get the unknown data.
        /// </summary>
        public byte[] Unknown 
        { 
            get 
            {
                byte[] outputBytes = new byte[unknown0.Length + unknown1.Length + unknown2.Length];
                unknown0.CopyTo(outputBytes, 0);
                unknown1.CopyTo(outputBytes, unknown0.Length);
                unknown2.CopyTo(outputBytes, unknown0.Length + unknown1.Length);
                return (outputBytes); 
            } 
        }
        
        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the title data.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The title data has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("MediaHighway2TitleData: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private byte[] unknown0;
        private int channelID;
        private byte[] unknown1;
        private int mainCategory;
        private DateTime baseDate;
        private int hours;
        private int minutes;
        private byte[] unknown2;
        private TimeSpan duration;
        private string eventName;
        private int subCategory;
        private int eventID;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MediaHighway2TitleData class.
        /// </summary>
        public MediaHighway2TitleData() { }

        /// <summary>
        /// Parse the title data.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the title data.</param>
        /// <param name="index">Index of the first byte of the title data in the MPEG2 section.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                unknown0 = Utils.GetBytes(byteData, 3, 15);

                channelID = (int)byteData[lastIndex] + 1;
                lastIndex++;

                unknown1 = Utils.GetBytes(byteData, lastIndex, 10);
                lastIndex += unknown1.Length;

                mainCategory = byteData[7] & 0x0f;
                
                baseDate = getDate(Utils.Convert2BytesToInt(byteData, lastIndex));
                lastIndex += 2;                

                hours = ((byteData[lastIndex] >> 4) * 10) + (byteData[lastIndex] & 0x0f);
                lastIndex++;

                minutes = ((byteData[lastIndex] >> 4) * 10) + (byteData[lastIndex] & 0x0f);
                lastIndex++;

                unknown2 = Utils.GetBytes(byteData, lastIndex, 1);
                lastIndex += unknown2.Length;

                int durationMinutes = (byteData[lastIndex] << 4)  + (byteData[lastIndex + 1] >> 4);
                duration = new TimeSpan(durationMinutes / 60, durationMinutes % 60, 0);
                lastIndex += 2;

                int eventNameLength = byteData[lastIndex] & 0x3f;
                lastIndex++;

                eventName = Utils.GetString(byteData, lastIndex, eventNameLength, true);
                lastIndex += eventNameLength;

                subCategory = byteData[lastIndex] & 0x3f;
                lastIndex++;

                eventID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("lastIndex = " + lastIndex));
            }
        }

        private DateTime getDate(int mjd)
        {
            int j = mjd + 2400001 + 68569;
            int c = 4 * j / 146097;
            j = j - (146097 * c + 3) / 4;

            int y = 4000 * (j + 1) / 1461001;
            j = j - 1461 * y / 4 + 31;
            int m = 80 * j / 2447;

            int day = j - 2447 * m / 80;
            j = m / 11;
            int month = m + 2 - (12 * j);
            int year = 100 * (c - 49) + y + j;

            return (new DateTime(year, month, day));
        }

        /// <summary>
        /// Validate the title data fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A title data field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the title data fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHW2 TITLE DATA: Channel ID: " + channelID +
                " Unknown0: " + Utils.ConvertToHex(unknown0) +
                " Unknown1: " + Utils.ConvertToHex(unknown1) +
                " Main cat: " + mainCategory +
                " Base date: " + baseDate +
                " Hours: " + hours +
                " Minutes: " + minutes +
                " Unknown2: " + Utils.ConvertToHex(unknown2) +
                " Duration: " + duration +
                " Event name: " + eventName +
                " Sub cat: " + mainCategory +
                " Event ID: " + eventID);
        }
    }
}
