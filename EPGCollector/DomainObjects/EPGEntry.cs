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

namespace DomainObjects
{
    /// <summary>
    /// The class the describes an EPG record.
    /// </summary>
    public class EPGEntry
    {
        /// <summary>
        /// Get or set the ONID.
        /// </summary>
        public int OriginalNetworkID
        {
            get { return (originalNetworkID); }
            set { originalNetworkID = value; }
        }

        /// <summary>
        /// Get or set the TSID.
        /// </summary>
        public int TransportStreamID
        {
            get { return (transportStreamID); }
            set { transportStreamID = value; }
        }

        /// <summary>
        /// Get or set the SID.
        /// </summary>
        public int ServiceID
        {
            get { return (serviceID); }
            set { serviceID = value; }
        }

        /// <summary>
        /// Get or set the version number.
        /// </summary>
        public int VersionNumber
        {
            get { return (versionNumber); }
            set { versionNumber = value; }
        }

        /// <summary>
        /// Get or set the event ID.
        /// </summary>
        public int EventID
        {
            get { return (eventID); }
            set { eventID = value; }
        }

        /// <summary>
        /// Get or set the program title.
        /// </summary>
        public string EventName
        {
            get { return (eventName); }
            set { eventName = value; }
        }

        /// <summary>
        /// Get or set the program sub-title.
        /// </summary>
        public string EventSubTitle
        {
            get { return (eventSubTitle); }
            set { eventSubTitle = value; }
        }

        /// <summary>
        /// Get or set the start time of the program.
        /// </summary>
        public DateTime StartTime
        {
            get { return (startTime); }
            set { startTime = value; }
        }

        /// <summary>
        /// Get or set the length of the program.
        /// </summary>
        public TimeSpan Duration
        {
            get { return (duration); }
            set { duration = value; }
        }

        /// <summary>
        /// Get or set the running status of the program.
        /// </summary>
        public int RunningStatus
        {
            get { return (runningStatus); }
            set { runningStatus = value; }
        }

        /// <summary>
        /// Return true if the program is encrypted; false otherwise. 
        /// </summary>
        public bool Scrambled
        {
            get { return (scrambled); }
            set { scrambled = value; }
        }

        /// <summary>
        /// Get or set the short description of the program.
        /// </summary>
        public string ShortDescription
        {
            get { return (shortDescription); }
            set { shortDescription = value; }
        }

        /// <summary>
        /// Get or set the full description of the program.
        /// </summary>
        public string ExtendedDescription
        {
            get { return (extendedDescription); }
            set { extendedDescription = value; }
        }

        /// <summary>
        /// Get or set the video quality of the program.
        /// </summary>
        public string VideoQuality
        {
            get { return (videoQuality); }
            set { videoQuality = value; }
        }

        /// <summary>
        /// Get or set the audio quality of the program.
        /// </summary>
        public string AudioQuality
        {
            get { return (audioQuality); }
            set { audioQuality = value; }
        }

        /// <summary>
        /// Get or set the aspect ratio of the program.
        /// </summary>
        public string AspectRatio
        {
            get { return (aspectRatio); }
            set { aspectRatio = value; }
        }

        /// <summary>
        /// Get or set the presence of subtitles for the program.
        /// </summary>
        public string SubTitles
        {
            get { return (subTitles); }
            set { subTitles = value; }
        }

        /// <summary>
        /// Get or set the parental rating of the program.
        /// </summary>
        public string ParentalRating
        {
            get { return (parentalRating); }
            set { parentalRating = value; }
        }

        /// <summary>
        /// Get or set the MPAA parental rating of the program.
        /// </summary>
        public string MpaaParentalRating
        {
            get { return (mpaaParentalRating); }
            set { mpaaParentalRating = value; }
        }

        /// <summary>
        /// Get or set the parental rating system in use.
        /// </summary>
        public string ParentalRatingSystem
        {
            get { return (parentalRatingSystem); }
            set { parentalRatingSystem = value; }
        }

        /// <summary>
        /// Get or set the program category.
        /// </summary>
        public string EventCategory
        {
            get { return (eventCategory); }
            set { eventCategory = value; }
        }

        /// <summary>
        /// Get or set the director of the program.
        /// </summary>
        public Collection<string> Directors
        {
            get { return (directors); }
            set { directors = value; }
        }

        /// <summary>
        /// Get or set the cast of the program.
        /// </summary>
        public Collection<string> Cast
        {
            get { return (cast); }
            set { cast = value; }
        }

        /// <summary>
        /// Get or set the series reference of the program.
        /// </summary>
        public string Series
        {
            get { return (series); }
            set { series = value; }
        }

        /// <summary>
        /// Get or set the episode of the program.
        /// </summary>
        public string Episode
        {
            get { return (episode); }
            set { episode = value; }
        }

        /// <summary>
        /// Get or set the part number within the episode of the program.
        /// </summary>
        public string PartNumber
        {
            get { return (partNumber); }
            set { partNumber = value; }
        }

        /// <summary>
        /// Get or set the scheme used to define the series and episode information.
        /// </summary>
        public string EpisodeSystemType
        {
            get { return (episodeSystemType); }
            set { episodeSystemType = value; }
        }

        /// <summary>
        /// Get or set the number of parts in the scheme used to define the series and episode information.
        /// </summary>
        public int EpisodeSystemParts
        {
            get { return (episodeSystemParts); }
            set { episodeSystemParts = value; }
        }

        /// <summary>
        /// Get or set the season number  of the program.
        /// </summary>
        public int SeasonNumber
        {
            get { return (seasonNumber); }
            set { seasonNumber = value; }
        }

        /// <summary>
        /// Get or set the episode number of the program.
        /// </summary>
        public int EpisodeNumber
        {
            get { return (episodeNumber); }
            set { episodeNumber = value; }
        }

        /// <summary>
        /// Get or set the date of the program.
        /// </summary>
        public string Date
        {
            get { return (date); }
            set { date = value; }
        }

        /// <summary>
        /// Get or set the previous play date of the program.
        /// </summary>
        public DateTime PreviousPlayDate
        {
            get { return (previousPlayDate); }
            set { previousPlayDate = value; }
        }

        /// <summary>
        /// Get or set the country of origin.
        /// </summary>
        public string Country
        {
            get { return (country); }
            set { country = value; }
        }

        /// <summary>
        /// Get or set the star rating of the program.
        /// </summary>
        public string StarRating
        {
            get { return (starRating); }
            set { starRating = value; }
        }

        /// <summary>
        /// Get or set the source of the EPG.
        /// </summary>
        public EPGSource EPGSource
        {
            get { return (epgSource); }
            set { epgSource = value; }
        }

        /// <summary>
        /// Get or set the flag for graphic violence.
        /// </summary>
        public bool HasGraphicViolence
        {
            get { return (hasGraphicViolence); }
            set { hasGraphicViolence = value; }
        }

        /// <summary>
        /// Get or set the flag for graphic language.
        /// </summary>
        public bool HasGraphicLanguage
        {
            get { return (hasGraphicLanguage); }
            set { hasGraphicLanguage = value; }
        }

        /// <summary>
        /// Get or set the flag for strong sexual content.
        /// </summary>
        public bool HasStrongSexualContent
        {
            get { return (hasStrongSexualContent); }
            set { hasStrongSexualContent = value; }
        }

        /// <summary>
        /// Get or set the flag for adult material.
        /// </summary>
        public bool HasAdult
        {
            get { return (hasAdult); }
            set { hasAdult = value; }
        }

        /// <summary>
        /// Get or set the flag for nudity.
        /// </summary>
        public bool HasNudity
        {
            get { return (hasNudity); }
            set { hasNudity = value; }
        }

        /// <summary>
        /// Get or set the PID the program was received on.
        /// </summary>
        public int PID
        {
            get { return (pid); }
            set { pid = value; }
        }

        /// <summary>
        /// Get or set the DVB table the program was received on.
        /// </summary>
        public int Table
        {
            get { return (table); }
            set { table = value; }
        }

        /// <summary>
        /// Get or set the time the program was received.
        /// </summary>
        public DateTime TimeStamp
        {
            get { return (timeStamp); }
            set { timeStamp = value; }
        }

        /// <summary>
        /// Get or set the undefined data associated with the program.
        /// </summary>
        public byte[] UnknownData
        {
            get { return (unknownData); }
            set { unknownData = value; }
        }

        /// <summary>
        /// Get a description of the program.
        /// </summary>
        public string ScheduleDescription
        {
            get
            {
                return (startTime.ToString("HH:mm") + " - " +
                    startTime.Add(Duration).ToString("HH:mm") + " " +
                    eventName);
            }
        }

        /// <summary>
        /// Get a full description of the program.
        /// </summary>
        public string FullScheduleDescription
        {
            get
            {
                return (originalNetworkID.ToString() + ":" + transportStreamID.ToString() + ":" + serviceID.ToString() + " " +
                    startTime.ToShortDateString() + " " +
                    startTime.ToString("HH:mm") + " - " +
                    startTime.Add(Duration).ToString("HH:mm") + " " +
                    eventName);
            }
        }

        /// <summary>
        /// Get a string describing the duration of the program.
        /// </summary>
        public string DurationString { get { return (startTime.ToString("HH:mm") + " - " + startTime.Add(Duration).ToString("HH:mm")); } }

        /// <summary>
        /// Return true if the program starts at midnight; false otherwise.
        /// </summary>
        public bool StartsAtMidnight { get { return (StartTime.Hour == 0 && StartTime.Minute == 0 && StartTime.Second == 0); } }
        /// <summary>
        /// Returns true if the program ends at midnight; false otherwise.
        /// </summary>
        public bool EndsAtMidnight
        {
            get
            {
                DateTime endTime = StartTime + Duration;
                return (endTime.Hour == 0 && endTime.Minute == 0 && endTime.Second == 0);
            }
        }

        private int originalNetworkID = -1;
        private int transportStreamID = -1;
        private int serviceID = -1;

        private int versionNumber = -1;

        private int eventID;
        private string eventName;
        private string eventSubTitle;
        private DateTime startTime;
        private TimeSpan duration;
        private int runningStatus;
        private bool scrambled;
        private string shortDescription;
        private string extendedDescription;
        private string subTitles;
        private string parentalRating;
        private string mpaaParentalRating;
        private string parentalRatingSystem;
        private string eventCategory;
        private string videoQuality;
        private string audioQuality;
        private string aspectRatio;
        private string series;
        private string episode;
        private string partNumber;
        private string episodeSystemType;
        private int episodeSystemParts;
        private int seasonNumber = -1;
        private int episodeNumber = -1;
        private Collection<string> cast;
        private Collection<string> directors;
        private string date;
        private string starRating;
        private DateTime previousPlayDate;
        private string country;
        private bool hasGraphicViolence;
        private bool hasGraphicLanguage;
        private bool hasStrongSexualContent;
        private bool hasAdult;
        private bool hasNudity;

        private int pid;
        private int table;
        private DateTime timeStamp;

        private byte[] unknownData;

        private EPGSource epgSource = EPGSource.MHEG5;

        /// <summary>
        /// Initialize a new instance of the EPGEntry class.
        /// </summary>
        public EPGEntry() { }

        /// <summary>
        /// Get a string representing this instance.
        /// </summary>
        /// <returns>A string description of this instance.</returns>
        public override string ToString()
        {
            return (eventName);
        }

        /// <summary>
        /// Create a copy of this instance.
        /// </summary>
        /// <returns>The replicated instance.</returns>
        public EPGEntry Clone()
        {
            EPGEntry newEntry = new EPGEntry();

            newEntry.originalNetworkID = originalNetworkID;
            newEntry.transportStreamID = transportStreamID;
            newEntry.serviceID = serviceID;
            newEntry.versionNumber = versionNumber;

            newEntry.eventID = eventID;
            newEntry.eventName = eventName;
            newEntry.eventSubTitle = eventSubTitle;
            newEntry.startTime = startTime;
            newEntry.duration = duration;
            newEntry.runningStatus = runningStatus;
            newEntry.scrambled = scrambled;
            newEntry.shortDescription = shortDescription;
            newEntry.extendedDescription = extendedDescription;
            newEntry.subTitles = subTitles;
            newEntry.parentalRating = parentalRating;
            newEntry.mpaaParentalRating = mpaaParentalRating;
            newEntry.parentalRatingSystem = parentalRatingSystem;            
            newEntry.eventCategory = eventCategory;
            newEntry.videoQuality = videoQuality;
            newEntry.audioQuality = audioQuality;
            newEntry.aspectRatio = aspectRatio;
            newEntry.series = series;
            newEntry.episode = episode;
            newEntry.partNumber = partNumber;
            newEntry.episodeSystemType = episodeSystemType;
            newEntry.episodeSystemParts = episodeSystemParts;
            newEntry.seasonNumber = seasonNumber;
            newEntry.episodeNumber = episodeNumber;
            newEntry.cast = cast;
            newEntry.directors = directors;
            newEntry.date = date;
            newEntry.starRating = starRating;
            newEntry.previousPlayDate = previousPlayDate;
            newEntry.country = country;

            newEntry.hasAdult = hasAdult;
            newEntry.hasGraphicLanguage = hasGraphicLanguage;
            newEntry.hasNudity = hasNudity;
            newEntry.hasStrongSexualContent = hasStrongSexualContent;            

            newEntry.pid = pid;
            newEntry.table = table;
            newEntry.timeStamp = timeStamp;

            newEntry.unknownData = unknownData;

            newEntry.epgSource = epgSource;

            return (newEntry);
        }
    }
}
