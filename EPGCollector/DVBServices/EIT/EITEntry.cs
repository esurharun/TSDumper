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
    /// The class that describes an EIT entry.
    /// </summary>
    public class EITEntry
    {
        /// <summary>
        /// Get the event identification.
        /// </summary>
        public int EventID 
        { 
            get { return (eventID); }
            set { eventID = value; }
        }        
        /// <summary>
        /// Get the event start time.
        /// </summary>
        public DateTime StartTime 
        { 
            get { return (startTime); }
            set { startTime = value; }
        }
        /// <summary>
        /// Get the event duration.
        /// </summary>
        public TimeSpan Duration 
        { 
            get { return (duration); }
            set { duration = value; }
        }
        /// <summary>
        /// Get the running status of the event.
        /// </summary>
        public int RunningStatus { get { return (runningStatus); } }
        /// <summary>
        /// Return true if the event is scrambled; false otherwise.
        /// </summary>
        public bool Scrambled { get { return (scrambled); } }        
        
        /// <summary>
        /// Get the event name.
        /// </summary>
        public string EventName 
        { 
            get { return (eventName); }
            set { eventName = value; }
        }
        /// <summary>
        /// Get the short description for the event.
        /// </summary>
        public string ShortDescription { get { return (shortDescription); } }
        /// <summary>
        /// Get the extended description for the event.
        /// </summary>
        public string ExtendedDescription { get { return (extendedDescription); } }
        /// <summary>
        /// Get the complete description.
        /// </summary>
        public string Description
        {
            get
            {
                if (shortDescription != null)
                {
                    if (extendedDescription == null || extendedDescription == string.Empty)
                        return (shortDescription);
                    else
                    {
                        if (shortDescription.EndsWith("."))
                            return (shortDescription + " " + extendedDescription.Trim());
                        else
                            return (shortDescription + ". " + extendedDescription.Trim());
                    }
                }
                else
                {
                    if (extendedDescription != null && extendedDescription != string.Empty)
                        return (extendedDescription);
                    else
                        return (null);
                }
            }
        }

        /// <summary>
        /// Get the DVB standard (EN 300 468) component type for the video stream.
        /// </summary>
        public int ComponentTypeVideo { get { return (componentTypeVideo); } }
        /// <summary>
        /// Get the DVB standard (EN 300 468) component type for the audio stream.
        /// </summary>
        public int ComponentTypeAudio { get { return (componentTypeAudio); } }
        /// <summary>
        /// Get the DVB standard (EN 300 468) component type for the subtitle stream.
        /// </summary>
        public int ComponentTypeSubtitles { get { return (componentTypeSubtitles); } }

        /// <summary>
        /// Get the DVB standard (EN 300 468) content type.
        /// </summary>
        public int ContentType { get { return (contentType); } }
        /// <summary>
        /// Get the DVB standard (EN 300 468) content subype.
        /// </summary>
        public int ContentSubType { get { return (contentSubType); } }

        /// <summary>
        /// Get the DVB standard (EN 300 468) parental rating.
        /// </summary>
        public int ParentalRating { get { return (parentalRating); } }

        /// <summary>
        /// Get the cast.
        /// </summary>
        public Collection<string> Cast { get { return (cast); } }

        /// <summary>
        /// Get the director.
        /// </summary>
        public Collection<string> Directors { get { return(directors); } } 
        
        /// <summary>
        /// Get the production year.
        /// </summary>
        public string Year { get { return (year); } }

        /// <summary>
        /// Get the star rating.
        /// </summary>
        public string StarRating { get { return (starRating); } }

        /// <summary>
        /// Get the series ID.
        /// </summary>
        public string SeriesID { get { return (seriesID); } }

        /// <summary>
        /// Get the season ID.
        /// </summary>
        public string SeasonID { get { return (seasonID); } }

        /// <summary>
        /// Get the episode ID.
        /// </summary>
        public string EpisodeID { get { return (episodeID); } }

        /// <summary>
        /// Get the TV rating.
        /// </summary>
        public string TVRating { get { return (tvRating); } }

        /// <summary>
        /// Get the previous play date.
        /// </summary>
        public string PreviousPlayDate  { get { return (previousPlayDate); } }

        /// <summary>
        /// Get the country of origin.
        /// </summary>
        public string Country { get { return (country); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the EIT entry.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The entry has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("EITEntry: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int eventID;
        private DateTime startTime;
        private TimeSpan duration;
        private int runningStatus;
        private bool scrambled;

        private string eventName;
        private string shortDescription;
        private string extendedDescription;
        private int componentTypeVideo = -1;
        private int componentTypeAudio = -1;
        private int componentTypeSubtitles = -1;
        private int contentType = -1;
        private int contentSubType = -1;
        private int parentalRating;

        private Collection<string> cast;
        private Collection<string> directors;
        private string year;
        private string starRating;
        private string seriesID;
        private string seasonID;
        private string episodeID;
        private string tvRating;
        private string previousPlayDate;
        private string country;
        
        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the EITEntry class.
        /// </summary>
        public EITEntry() { }

        /// <summary>
        /// Parse the entry.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the entry.</param>
        /// <param name="index">Index of the event identification byte in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                eventID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                getStartTime(byteData, lastIndex);
                lastIndex += 5;

                getDuration(byteData, lastIndex);
                lastIndex += 3;

                runningStatus = (int)(byteData[lastIndex] >> 5);
                scrambled = ((int)byteData[lastIndex] & 0x10) >> 4 == 1;                

                int descriptorLoopLength = ((byteData[lastIndex] & 0x0f) * 256) + (int)byteData[lastIndex + 1];
                lastIndex += 2;

                while (descriptorLoopLength != 0)
                {
                    DescriptorBase descriptor = DescriptorBase.Instance(byteData, lastIndex);

                    if (!descriptor.IsEmpty)
                    {
                        processDescriptor(descriptor);
                        descriptor.LogMessage();

                        lastIndex = descriptor.Index;
                        descriptorLoopLength -= descriptor.TotalLength;
                    }
                    else
                    {
                        lastIndex += DescriptorBase.MinimumDescriptorLength;
                        descriptorLoopLength -= DescriptorBase.MinimumDescriptorLength;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The EIT message is short"));
            }
        }

        private void getStartTime(byte[] byteData, int index)
        {
            if (byteData[index] == 0xff &&
                byteData[index + 1] == 0xff &&
                byteData[index + 2] == 0xff &&
                byteData[index + 3] == 0xff &&
                byteData[index + 4] == 0xff)
            {
                startTime = DateTime.MinValue;
                return;
            }

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
                startTime = utcStartTime.ToLocalTime();
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

        private void getDuration(byte[] byteData, int index)
        {
            int durationHours1 = (int)byteData[index] >> 4;
            int durationHours2 = (int)byteData[index] & 0x0f;
            int durationHours = (durationHours1 * 10) + durationHours2;

            int durationMinutes1 = (int)byteData[index + 1] >> 4;
            int durationMinutes2 = (int)byteData[index + 1] & 0x0f;
            int durationMinutes = (durationMinutes1 * 10) + durationMinutes2;

            int durationSeconds1 = (int)byteData[index + 2] >> 4;
            int durationSeconds2 = (int)byteData[index + 2] & 0x0f;
            int durationSeconds = (durationSeconds1 * 10) + durationSeconds2;

            try
            {
                duration = new TimeSpan(durationHours, durationMinutes, durationSeconds);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The duration time span element(s) are out of range"));
            }
        }

        private void processDescriptor(DescriptorBase descriptor)
        {
            switch (descriptor.Tag)
            {
                case DescriptorBase.ShortEventDescriptorTag:
                    DVBShortEventDescriptor shortEventDescriptor = descriptor as DVBShortEventDescriptor;
                    if (eventName == null)
                        processShortEventDescriptor(shortEventDescriptor);
                    else
                    {
                        if (RunParameters.Instance.InputLanguage == null)
                            processShortEventDescriptor(shortEventDescriptor);
                        else
                        {
                            if (shortEventDescriptor.LanguageCode == RunParameters.Instance.InputLanguage)
                                processShortEventDescriptor(shortEventDescriptor);
                            else
                            {
                                if (shortEventDescriptor.LanguageCode == "eng")
                                    processShortEventDescriptor(shortEventDescriptor);
                            }
                        }
                    }
                    LanguageCode.RegisterUsage(shortEventDescriptor.LanguageCode);
                    break;
                case DescriptorBase.ExtendedEventDescriptorTag:
                    DVBExtendedEventDescriptor extendedEventDescriptor = descriptor as DVBExtendedEventDescriptor;
                    if (extendedDescription == null)
                        processExtendedEventDescriptor(extendedEventDescriptor);
                    else
                    {
                        if (RunParameters.Instance.InputLanguage == null)
                            processExtendedEventDescriptor(extendedEventDescriptor);
                        else
                        {
                            if (extendedEventDescriptor.LanguageCode == RunParameters.Instance.InputLanguage)
                                processExtendedEventDescriptor(extendedEventDescriptor);
                            else
                            {
                                if (extendedEventDescriptor.LanguageCode == "eng")
                                    processExtendedEventDescriptor(extendedEventDescriptor);
                            }
                        }
                    }
                    LanguageCode.RegisterUsage(extendedEventDescriptor.LanguageCode);
                    break;
                case DescriptorBase.ComponentDescriptorTag:
                    if ((descriptor as DVBComponentDescriptor).ComponentTypeVideo != -1)
                        componentTypeVideo = (descriptor as DVBComponentDescriptor).ComponentTypeVideo;
                    if ((descriptor as DVBComponentDescriptor).ComponentTypeAudio != -1)
                        componentTypeAudio = (descriptor as DVBComponentDescriptor).ComponentTypeAudio;
                    if ((descriptor as DVBComponentDescriptor).ComponentTypeSubtitles != -1)
                        componentTypeSubtitles = (descriptor as DVBComponentDescriptor).ComponentTypeSubtitles;
                    break;
                case DescriptorBase.ContentDescriptorTag:
                    contentType = (descriptor as DVBContentDescriptor).ContentType[0];
                    contentSubType = (descriptor as DVBContentDescriptor).ContentSubType[0];
                    break;
                case DescriptorBase.ParentalRatingDescriptorTag:
                    DVBParentalRatingDescriptor parentalRatingDescriptor = descriptor as DVBParentalRatingDescriptor;
                    if (parentalRatingDescriptor.ParentalRatings != null)
                        parentalRating = (descriptor as DVBParentalRatingDescriptor).ParentalRatings[0];
                    break;
                case DescriptorBase.ContentIdentifierDescriptorTag:
                    DVBContentIdentifierDescriptor contentIdentifierDescriptor = descriptor as DVBContentIdentifierDescriptor;
                    if (contentIdentifierDescriptor.IsSeriesLink)
                        seriesID = contentIdentifierDescriptor.ContentReference;
                    else
                    {
                        if (contentIdentifierDescriptor.IsEpisodeLink)
                            episodeID = contentIdentifierDescriptor.ContentReference;
                    }
                    break;
                default:
                    if (RunParameters.Instance.DebugIDs.Contains("UNKNOWNDESCRIPTORS"))
                    {
                        Logger.Instance.Write("Unprocessed EIT descriptor: 0x" + descriptor.Tag.ToString("X"));
                        if (RunParameters.Instance.DebugIDs.Contains("LOGDESCRIPTORDATA"))
                            Logger.Instance.Dump("Descriptor Data", descriptor.Data, descriptor.Data.Length);
                    }
                    break;
            }
        }

        private void processShortEventDescriptor(DVBShortEventDescriptor shortEventDescriptor)
        {
            eventName = shortEventDescriptor.EventName;
            shortDescription = shortEventDescriptor.ShortDescription;
        }

        private void processExtendedEventDescriptor(DVBExtendedEventDescriptor extendedEventDescriptor)
        {
            if (extendedEventDescriptor.DescriptorNumber == 0)
                extendedDescription = extendedEventDescriptor.Text;
            else
                extendedDescription += extendedEventDescriptor.Text;

            if (extendedEventDescriptor.Cast != null)
                cast = extendedEventDescriptor.Cast;
            if (extendedEventDescriptor.Directors != null)
                directors = extendedEventDescriptor.Directors;
            if (extendedEventDescriptor.Year != null)
                year = extendedEventDescriptor.Year;
            if (extendedEventDescriptor.StarRating != null)
                starRating = extendedEventDescriptor.StarRating;
            if (extendedEventDescriptor.SeriesID != null)
                seriesID = extendedEventDescriptor.SeriesID;
            if (extendedEventDescriptor.SeasonID != null)
                seasonID = extendedEventDescriptor.SeasonID;
            if (extendedEventDescriptor.EpisodeID != null)
                episodeID = extendedEventDescriptor.EpisodeID;
            if (extendedEventDescriptor.TVRating != null)
                tvRating = extendedEventDescriptor.TVRating;
            if (extendedEventDescriptor.PreviousPlayDate != null)
                previousPlayDate = extendedEventDescriptor.PreviousPlayDate;
            if (extendedEventDescriptor.Country != null)
                country = extendedEventDescriptor.Country;
        }

        /// <summary>
        /// Validate the entry fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// An entry field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the entry fields.
        /// </summary>
        public void LogMessage() { }
    }
}
