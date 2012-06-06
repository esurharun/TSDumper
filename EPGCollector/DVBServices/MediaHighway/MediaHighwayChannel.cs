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
    /// The class that defines a MediaHighway channel.
    /// </summary>
    public class MediaHighwayChannel : Channel
    {
        /// <summary>
        /// Get or set the channel name.
        /// </summary>
        public string ChannelName
        {
            get { return (channelName); }
            set { channelName = value; }
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
        /// Get the title data for the channel.
        /// </summary>
        public Collection<MediaHighwayTitle> Titles
        {
            get
            {
                if (titles == null)
                    titles = new Collection<MediaHighwayTitle>();
                return (titles);
            }
        }

        private Collection<MediaHighwayTitle> titles;
        
        private string channelName;
        private byte[] unknown;

        /// <summary>
        /// Initialize a new instance of the MediaHighwayChannel class.
        /// </summary>
        public MediaHighwayChannel() { }

        /// <summary>
        /// Add title data to the channel.
        /// </summary>
        /// <param name="newTitle">The title to be added.</param>
        public void AddTitleData(MediaHighwayTitle newTitle)
        {
            foreach (MediaHighwayTitle oldTitle in Titles)
            {
                if (oldTitle.StartTime == newTitle.StartTime)
                    return;

                if (oldTitle.StartTime > newTitle.StartTime)
                {
                    Titles.Insert(Titles.IndexOf(oldTitle), newTitle);
                    return;
                }
            }

            Titles.Add(newTitle);
        }

        /// <summary>
        /// Create the EPG entries from the stored title and summary data.
        /// </summary>
        /// <param name="station">The station that the EPG records are for.</param>
        /// <param name="titleLogger">A Logger instance for the program titles.</param>
        /// <param name="descriptionLogger">A Logger instance for the program descriptions.</param>
        /// <param name="collectionType">The type of collection, MHW1 or MHW2.</param>
        public void ProcessChannelForEPG(TVStation station, Logger titleLogger, Logger descriptionLogger, CollectionType collectionType)
        {
            bool first = true;
            DateTime expectedStartTime = new DateTime();

            foreach (MediaHighwayTitle title in Titles)
            {
                EPGEntry epgEntry = new EPGEntry();
                epgEntry.OriginalNetworkID = OriginalNetworkID;
                epgEntry.TransportStreamID = TransportStreamID;
                epgEntry.ServiceID = ServiceID;
                epgEntry.EventID = title.EventID;

                processEventName(epgEntry, title.EventName);

                MediaHighwaySummary summary = null;

                if (title.SummaryAvailable)
                {
                    summary = findSummary(title.EventID);
                    if (summary != null)
                        processShortDescription(epgEntry, summary.ShortDescription);
                    else
                    {
                        if (RunParameters.Instance.DebugIDs.Contains("MHW2SUMMARYMISSING"))
                            Logger.Instance.Write("Summary missing for event ID " + title.EventID);
                    }
                }                
                if (summary == null)
                    epgEntry.ShortDescription = "No Synopsis Available";

                if (collectionType == CollectionType.MediaHighway1)
                    epgEntry.StartTime = Utils.RoundTime(TimeOffsetEntry.GetOffsetTime(title.StartTime));
                else
                    epgEntry.StartTime = Utils.RoundTime(TimeOffsetEntry.GetAdjustedTime(title.StartTime));
                epgEntry.Duration = Utils.RoundTime(title.Duration);

                epgEntry.EventCategory = getEventCategory(epgEntry.EventName, epgEntry.ShortDescription, title.CategoryID);                

                if (collectionType == CollectionType.MediaHighway1)
                    epgEntry.EPGSource = EPGSource.MediaHighway1;
                else
                    epgEntry.EPGSource = EPGSource.MediaHighway2;

                epgEntry.VideoQuality = getVideoQuality(epgEntry.EventName);

                epgEntry.PreviousPlayDate = title.PreviousPlayDate;
                
                station.AddEPGEntry(epgEntry);

                if (first)
                {
                    expectedStartTime = new DateTime();
                    first = false;
                }
                else
                {
                    if (epgEntry.StartTime < expectedStartTime)
                    {
                        if (titleLogger != null)
                            titleLogger.Write(" ** Overlap In Schedule **");
                    }
                    else
                    {
                        if (RunParameters.Instance.Options.Contains("ACCEPTBREAKS"))
                        {
                            if (epgEntry.StartTime > expectedStartTime + new TimeSpan(0, 5, 0))
                            {
                                if (titleLogger != null)
                                    titleLogger.Write(" ** Gap In Schedule **");
                            }
                        }
                        else
                        {
                            if (epgEntry.StartTime > expectedStartTime)
                            {
                                if (titleLogger != null)
                                    titleLogger.Write(" ** Gap In Schedule **");
                            }
                        }
                    }
                }

                expectedStartTime = epgEntry.StartTime + epgEntry.Duration;

                if (titleLogger != null)
                {
                    if (collectionType == CollectionType.MediaHighway1) 
                        titleLogger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +
                            " Evt ID " + title.EventID +
                            " Cat ID " + title.CategoryID.ToString("00") +
                            " Summary " + title.SummaryAvailable + ":" + (summary != null) + " " +
                            " Orig Day " + title.LogDay +
                            " Orig Hours " + title.LogHours +
                            " YDay " + title.LogYesterday +
                            " Day " + title.Day +
                            " Hours " + title.Hours +
                            " Mins " + title.Minutes + " " +
                            epgEntry.StartTime.ToShortDateString() + " " +
                            epgEntry.StartTime.ToString("HH:mm") + " - " +
                            epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                            title.EventName);
                    else
                        titleLogger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +
                            " Evt ID " + title.EventID +
                            " Cat ID " + title.CategoryID.ToString("000") +
                            " Main cat " + title.MainCategory + 
                            " Sub cat " + title.SubCategory +
                            " Summary " + title.SummaryAvailable + ":" + (summary != null) +
                            " Unknown " + Utils.ConvertToHex(title.Unknown) + " " +
                            epgEntry.StartTime.ToShortDateString() + " " +
                            epgEntry.StartTime.ToString("HH:mm") + " - " +
                            epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                            title.EventName);
                    
                }

                if (descriptionLogger != null && summary != null)
                {
                    if (collectionType == CollectionType.MediaHighway1) 
                        descriptionLogger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +
                            " Evt ID " + title.EventID +
                            " Rpts: " + summary.ReplayCount + " " +
                            epgEntry.StartTime.ToShortDateString() + " " +
                            epgEntry.StartTime.ToString("HH:mm") + " - " +
                            epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                            summary.ShortDescription);
                    else
                        descriptionLogger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +
                            " Evt ID " + title.EventID + " " +
                            " Unknown " + Utils.ConvertToHex(summary.Unknown) + " " +
                            epgEntry.StartTime.ToShortDateString() + " " +
                            epgEntry.StartTime.ToString("HH:mm") + " - " +
                            epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                            summary.ShortDescription);
                }

                if (!RunParameters.Instance.Options.Contains("ACCEPTBREAKS"))
                {
                    if (epgEntry.StartTime.Second != 0)
                    {
                        if (titleLogger != null)
                            titleLogger.Write("** Suspect Start Time **");
                    }
                }
            }
        }

        internal static MediaHighwayChannelTitle FindChannelTitle(int eventID)
        {
            foreach (MediaHighwayChannel channel in MediaHighwayChannel.Channels)
            {
                MediaHighwayTitle title = channel.findTitle(eventID);
                if (title != null)
                    return (new MediaHighwayChannelTitle(channel, title));
            }

            return (null);
        }

        private MediaHighwayTitle findTitle(int eventID)
        {
            foreach (MediaHighwayTitle title in Titles)
            {
                if (title.EventID == eventID)
                    return (title);

                if (title.EventID > eventID)
                    return (null);
            }

            return (null);
        }

        private MediaHighwaySummary findSummary(int eventID)
        {
            foreach (MediaHighwaySummary summary in MediaHighwaySummary.Summaries)
            {
                if (summary.EventID == eventID)
                    return (summary);

                if (summary.EventID > eventID)
                    return (null);
            }

            return (null);
        }

        private string getEventCategory(string title, string description, int categoryID)
        {
            if (categoryID == 0)
                return (getCustomCategory(title, description));

            if (RunParameters.Instance.Options.Contains("CUSTOMCATEGORYOVERRIDE"))
            {
                string customCategory = getCustomCategory(title, description);
                if (customCategory != null)
                    return (customCategory);
            }
            
            MediaHighwayProgramCategory category = MediaHighwayProgramCategory.FindCategory(categoryID);
            if (category != null)
                return (category.Description);
            
            if (RunParameters.Instance.Options.Contains("CUSTOMCATEGORYOVERRIDE"))
                return (null);

            return(getCustomCategory(title, description));
        }

        private string getCustomCategory(string title, string description)
        {
            string category = CustomProgramCategory.FindCategoryDescription(title);
            if (category != null)
                return (category);

            return (CustomProgramCategory.FindCategoryDescription(description));
        }

        private void processShortDescription(EPGEntry epgEntry, string description)
        {
            epgEntry.ShortDescription = description;
            
            epgEntry.Directors = getDirectors(description);
            epgEntry.Cast = getCast(description);
        }

        private string getVideoQuality(string title)
        {
            if (title.Contains("(HD)"))
                return ("HDTV");
            else
                return (null);
        }

        private Collection<string> getDirectors(string description)
        {
            int index1 = description.IndexOf(" Dir: ");
            if (index1 == -1)
                return (null);

            int index2 = description.IndexOf('.', index1);
            if (index2 == -1)
                return(null);

            string directorsString = description.Substring(index1 + 6, index2 - (index1 + 6));
            string[] directorParts = directorsString.Split(new char[] { ',' });

            Collection<string> directors = new Collection<string>();

            foreach (string director in directorParts)
                directors.Add(director.Trim());

            return (directors);
        }

        private Collection<string> getCast(string description)
        {
            string identifier = " Int: ";

            int index1 = description.IndexOf(identifier);
            if (index1 == -1)
            {
                identifier = " Pres: ";
                index1 = description.IndexOf(identifier);
                if (index1 == -1)
                    return (null);                
            }

            int index2 = description.IndexOf('.', index1);
            if (index2 == -1)
                return (null);

            string castString = description.Substring(index1 + identifier.Length, index2 - (index1 + identifier.Length));
            string[] castParts = castString.Split(new char[] { ',' });

            Collection<string> cast = new Collection<string>();

            foreach (string castMember in castParts)
                cast.Add(castMember.Trim());

            return (cast);
        }

        private void processEventName(EPGEntry epgEntry, string eventName)
        {
            epgEntry.EventName = eventName;
        }

        /// <summary>
        /// Get a description of this instance.
        /// </summary>
        /// <returns>A string describing this instance.</returns>
        public override string ToString()
        {
            TVStation station = TVStation.FindStation(OriginalNetworkID, TransportStreamID, ServiceID);
            string stationName;
            if (station != null)
                stationName = station.Name;
            else
                stationName = "** No Station **";

            string unknownString;
            if (unknown == null)
                unknownString = "n/a";
            else
                unknownString = Utils.ConvertToHex(unknown);

            return ("ONID " + OriginalNetworkID +
                " TSID " + TransportStreamID +
                " SID " + ServiceID +
                " Channel ID: " + ChannelID +
                " Unknown: " + unknownString +
                " Name: " + channelName +
                " Station: " + stationName);
        }
    }
}
