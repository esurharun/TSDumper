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

namespace DVBServices
{
    /// <summary>
    /// The class that describes a MediaHighway title.
    /// </summary>
    public class MediaHighwayTitle
    {
        /// <summary>
        /// Get or set the event identification.
        /// </summary>
        public int EventID 
        { 
            get { return (eventID); }
            set { eventID = value; }
        }
        /// <summary>
        /// Get or set the start time of the event.
        /// </summary>
        public DateTime StartTime 
        { 
            get { return (startTime); }
            set { startTime = value; }
        }
        /// <summary>
        /// Get or set the duration of the event.
        /// </summary>
        public TimeSpan Duration 
        { 
            get { return (duration); }
            set  {duration = value; } 
        }
        /// <summary>
        /// Get or set the theme identification of the event.
        /// </summary>
        public int CategoryID 
        { 
            get { return (categoryID); }
            set { categoryID = value; }
        }
        /// <summary>
        /// Get or set the main theme identification of the event.
        /// </summary>
        public int MainCategory
        {
            get { return (mainCategory); }
            set { mainCategory = value; }
        }
        /// <summary>
        /// Get or set the sub theme identification of the event.
        /// </summary>
        public int SubCategory
        {
            get { return (subCategory); }
            set { subCategory = value; }
        }
        /// <summary>
        /// Get or set the name of the event.
        /// </summary>
        public string EventName 
        { 
            get { return (eventName); }
            set { eventName = value; }
        }
        /// <summary>
        /// Return true if the summary should be available; false otherwise.
        /// </summary>
        public bool SummaryAvailable 
        { 
            get { return (summaryAvailable); }
            set { summaryAvailable = value; }
        }

        /// <summary>
        /// Get or set the day number that was used to generate the start time.
        /// </summary>
        public int Day
        {
            get { return (day); }
            set { day = value; }
        }

        /// <summary>
        /// Get or set the hours field that generated the start time.
        /// </summary>
        public int Hours
        {
            get { return (hours); }
            set { hours = value; }
        }

        /// <summary>
        /// Get or set the minutes field that generated the start time.
        /// </summary>
        public int Minutes
        {
            get { return (minutes); }
            set { minutes = value; }
        }

        /// <summary>
        /// Get or set the log day number that was used to generate the start time.
        /// </summary>
        public int LogDay
        {
            get { return (logDay); }
            set { logDay = value; }
        }

        /// <summary>
        /// Get or set the log hours field that generated the start time.
        /// </summary>
        public int LogHours
        {
            get { return (logHours); }
            set { logHours = value; }
        }

        /// <summary>
        /// Get or set the log yesterday field that generated the start time.
        /// </summary>
        public int LogYesterday
        {
            get { return (logYesterday); }
            set { logYesterday = value; }
        }

        /// <summary>
        /// Get or set the unknown data.
        /// </summary>
        public byte[] Unknown
        {
            get { return (unknown); }
            set { unknown = value; }
        }

        /// <summary>
        /// Get or set the previous play date.
        /// </summary>
        public DateTime PreviousPlayDate
        {
            get { return (previousPlayDate); }
            set { previousPlayDate = value; }
        }

        private int categoryID;
        private int mainCategory;
        private int subCategory;
        private DateTime startTime;
        private bool summaryAvailable;
        private TimeSpan duration;
        private string eventName;
        private int eventID;

        private int day;
        private int hours;
        private int minutes;

        private int logDay;
        private int logHours;
        private int logYesterday;

        private byte[] unknown;

        private DateTime previousPlayDate;

        /// <summary>
        /// Initialize a new instance of the MediaHighwayTitle class.
        /// </summary>
        public MediaHighwayTitle() { }        
    }
}
