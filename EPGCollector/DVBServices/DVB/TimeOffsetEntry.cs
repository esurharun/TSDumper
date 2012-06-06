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
    /// The class that describes a time offset entry.
    /// </summary>
    public class TimeOffsetEntry
    {
        /// <summary>
        /// Get the collection of time offset entries.
        /// </summary>
        public static Collection<TimeOffsetEntry> TimeOffsets
        {
            get
            {
                if (timeOffsets == null)
                    timeOffsets = new Collection<TimeOffsetEntry>();
                return (timeOffsets);
            }
        }

        /// <summary>
        /// Get or set the current time offset.
        /// </summary>
        public static TimeSpan CurrentTimeOffset 
        { 
            get { return (currentTimeOffset); }
            set { currentTimeOffset = value; }
        }

        /// <summary>
        /// Get or set the next time offset.
        /// </summary>
        public static TimeSpan FutureTimeOffset
        {
            /*get { return(new TimeSpan(13, 0, 0)); }*/
            get { return (futureTimeOffset); }            
            set { futureTimeOffset = value; }
        }

        /// <summary>
        /// Get or set the time that the next time offset will come into force.
        /// </summary>
        public static DateTime TimeOfFutureTimeOffset
        {
            /*get { return(new DateTime(2010, 7, 18, 3, 0, 0)); } */
            get { return (timeOfFutureTimeOffset); }
            set { timeOfFutureTimeOffset = value; }
        }

        /// <summary>
        /// Get or set the country code.
        /// </summary>
        public string CountryCode
        {
            get { return(countryCode); }
            set { countryCode = value; }
        }

        /// <summary>
        /// Get or set the region.
        /// </summary>
        public int Region
        {
            get { return(region); }
            set { region = value; }
        }

        /// <summary>
        /// Get or set the time offset.
        /// </summary>
        public TimeSpan TimeOffset
        {
            get { return(timeOffset); }
            set { timeOffset = value; }
        }

        /// <summary>
        /// Get or set the time of the next change.
        /// </summary>
        public DateTime ChangeTime
        {
            get { return(changeTime); }
            set { changeTime = value; }
        }

        /// <summary>
        /// Get or set the next time offset.
        /// </summary>
        public TimeSpan NextTimeOffset
        {
            get { return(nextTimeOffset); }
            set { nextTimeOffset = value; }
        }

        private string countryCode;
        private int region;
        private TimeSpan timeOffset;
        private DateTime changeTime;
        private TimeSpan nextTimeOffset;

        private static Collection<TimeOffsetEntry> timeOffsets;
        private static TimeSpan currentTimeOffset;
        private static TimeSpan futureTimeOffset;
        private static DateTime timeOfFutureTimeOffset;

        /// <summary>
        /// Initialize a new instance of the TimeOffsetEntry.
        /// </summary>
        public TimeOffsetEntry() { }

        /// <summary>
        /// Add a time offset entry to the collection.
        /// </summary>
        /// <param name="newEntry">The entry to be added.</param>
        public static void AddEntry(TimeOffsetEntry newEntry)
        {
            foreach (TimeOffsetEntry oldEntry in TimeOffsets)
            {
                if (oldEntry.CountryCode == newEntry.CountryCode && oldEntry.Region == newEntry.Region)
                    return;

                if (oldEntry.CountryCode == newEntry.CountryCode)
                {
                    if (oldEntry.Region > newEntry.Region)
                    {
                        TimeOffsets.Insert(TimeOffsets.IndexOf(oldEntry), newEntry);
                        return;
                    }
                }
                else
                {
                    if (oldEntry.CountryCode.CompareTo(newEntry.CountryCode) > 0)
                    {
                        TimeOffsets.Insert(TimeOffsets.IndexOf(oldEntry), newEntry);
                        return;
                    }
                }
            }

            TimeOffsets.Add(newEntry);
        }

        /// <summary>
        /// Find a time offset entry.
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <param name="region">The region code.</param>
        /// <returns>A time offset entry or null if it cannot be located.</returns>
        public static TimeOffsetEntry FindEntry(string countryCode, int region)
        {
            foreach (TimeOffsetEntry entry in TimeOffsets)
            {
                if (entry.CountryCode == countryCode && entry.Region == region)
                    return (entry);
            }

            return (null);
        }

        /// <summary>
        /// Adjust a GMT time using the correct time offset.
        /// </summary>
        /// <param name="gmtStartTime">The GMT time.</param>
        /// <returns>The adjusted time.</returns>
        public static DateTime GetAdjustedTime(DateTime gmtStartTime)
        {
            if (RunParameters.Instance.TimeZoneSet)
            {
                DateTime adjustedDateTime = gmtStartTime + RunParameters.Instance.TimeZone;

                if (adjustedDateTime < RunParameters.Instance.NextTimeZoneChange)
                    return (adjustedDateTime);
                else
                    return (gmtStartTime + RunParameters.Instance.NextTimeZone);
            }
            else
                return(TimeZoneInfo.ConvertTimeFromUtc(gmtStartTime, TimeZoneInfo.Local));
        }

        /// <summary>
        /// Adjust a time using the run parameters time offset.
        /// </summary>
        /// <param name="time">The input time.</param>
        /// <returns>The adjusted time.</returns>
        public static DateTime GetOffsetTime(DateTime time)
        {
            if (RunParameters.Instance.TimeZoneSet)
            {
                DateTime adjustedDateTime = time + RunParameters.Instance.TimeZone;

                if (adjustedDateTime < RunParameters.Instance.NextTimeZoneChange)
                    return (adjustedDateTime);
                else
                    return (time + RunParameters.Instance.NextTimeZone);
            }
            else
                return (time);
        }
    }
}
        