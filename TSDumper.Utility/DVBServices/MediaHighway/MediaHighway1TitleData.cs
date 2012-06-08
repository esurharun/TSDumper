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
    /// The class that describes MediaHighway1 title data.
    /// </summary>
    public class MediaHighway1TitleData
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
        public DateTime StartTime { get { return(startTime); } }
        /// <summary>
        /// Get the duration of the event.
        /// </summary>
        public TimeSpan Duration { get { return(duration); } }
        /// <summary>
        /// Get the theme identification of the event.
        /// </summary>
        public int CategoryID { get { return(categoryID); } }
        /// <summary>
        /// Get the name of the event.
        /// </summary>
        public string EventName { get { return(eventName); } }
        /// <summary>
        /// Return true if the summary should be available; false otherwise.
        /// </summary>
        public bool SummaryAvailable { get { return (summaryAvailable); } }

        /// <summary>
        /// Get the day number that was used to generate the start time.
        /// </summary>
        public int Day
        {
            get { return (day); }
            set { day = value; }
        }

        /// <summary>
        /// Get the hours field that generated the start time.
        /// </summary>
        public int Hours
        {
            get { return (hours); }
            set { hours = value; }
        }

        /// <summary>
        /// Get the minutes field that generated the start time.
        /// </summary>
        public int Minutes
        {
            get { return (minutes); }
            set { minutes = value; }
        }

        /// <summary>
        /// Get the log day number that was used to generate the start time.
        /// </summary>
        public int LogDay
        {
            get { return (logDay); }
            set { logDay = value; }
        }

        /// <summary>
        /// Get the log hours field that generated the start time.
        /// </summary>
        public int LogHours
        {
            get { return (logHours); }
            set { logHours = value; }
        }

        /// <summary>
        /// Get the log yesterday field that generated the start time.
        /// </summary>
        public int LogYesterday
        {
            get { return (logYesterday); }
            set { logYesterday = value; }
        }

        /// <summary>
        /// Return true if the entry is empty; false otherwise.
        /// </summary>
        public bool IsEmpty { get { return (empty); } }

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
                    throw (new InvalidOperationException("MediaHighway1TitleData: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int channelID;
        private int categoryID;
        private DateTime startTime;
        private bool summaryAvailable;
        private byte[] unknown1;
        private TimeSpan duration;
        private string eventName;
        private int payPerViewID;
        private int eventID;
        private byte[] unknown2;

        private int day;
        private int hours;
        private int minutes;

        private int logDay;
        private int logHours;
        private int logYesterday;

        private bool empty;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MediaHighway1TitleData class.
        /// </summary>
        public MediaHighway1TitleData() { }

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
                channelID = (int)byteData[lastIndex];
                lastIndex++;

                if (channelID == 255)
                {
                    empty = true;
                    return;
                }

                categoryID = byteData[lastIndex];
                lastIndex++;

                day = byteData[lastIndex] >> 5;
                logDay = day;
                hours = byteData[lastIndex] & 0x1f;
                logHours = hours;
                lastIndex++;

                minutes = byteData[lastIndex] >> 2;

                DateTime yesterdayNow = DateTime.Now - new TimeSpan(1, 0, 0, 0);
                DateTime yesterday = new DateTime(yesterdayNow.Year, yesterdayNow.Month, yesterdayNow.Day);
                logYesterday = (int)yesterday.DayOfWeek;

                if (hours > 15)
                    hours -= 4;
                else
                    if (hours > 7)
                        hours -= 2;
                    else
                        day++;

                if (day > 6)
                    day-= 7;                
                
                day -= (int)yesterday.DayOfWeek;
                
                if (day < 1)
                    day = 7 + day;

                if (day == 1 && hours < 6)
                    day = 8;

                startTime = yesterday + new TimeSpan(day, hours, minutes, 0);

                summaryAvailable = ((byteData[lastIndex] & 0x01) == 1);
                lastIndex++;

                unknown1 = Utils.GetBytes(byteData, lastIndex, 2);
                lastIndex += unknown1.Length;

                int durationMinutes = Utils.Convert2BytesToInt(byteData, lastIndex);
                duration = new TimeSpan(durationMinutes / 60, durationMinutes % 60, 0);
                lastIndex += 2;

                eventName = Utils.GetString(byteData, lastIndex, 23, true);
                lastIndex += 23;

                payPerViewID = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                eventID = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                unknown2 = Utils.GetBytes(byteData, lastIndex, 4);
                lastIndex += unknown2.Length;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("lastIndex = " + lastIndex));
            }
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHW1 TITLE DATA: Channel ID: " + channelID +
                " Category ID: " + categoryID +
                " Day: " + logDay +
                " Hours: " + logHours +
                " Minutes: " + minutes +
                " Start: " + startTime +
                " Summary: " + summaryAvailable +
                " Unknown1: " + Utils.ConvertToHex(unknown1) +
                " Duration: " + duration +
                " Event name: " + eventName +
                " PPV ID: " + payPerViewID + 
                " Event ID: " + eventID +
                " Unknown2: " + Utils.ConvertToHex(unknown2));            
        }
    }
}
