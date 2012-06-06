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
    /// The class that describes a Bell TV section.
    /// </summary>
    public class BellTVSection
    {
        /// <summary>
        /// Get the collection of EIT enteries in the section.
        /// </summary>
        public Collection<EITEntry> EITEntries { get { return (eitEntries); } }

        /// <summary>
        /// Get the original network identification (ONID).
        /// </summary>
        public int OriginalNetworkID { get { return (originalNetworkID); } }
        /// <summary>
        /// Get the transport stream identification (TSID).
        /// </summary>
        public int TransportStreamID { get { return (transportStreamID); } }
        /// <summary>
        /// Get the service identification (SID).
        /// </summary>
        public int ServiceID { get { return (serviceID); } }

        /// <summary>
        /// Get the identification of the last table for the EIT section.
        /// </summary>
        public int LastTableID { get { return (lastTableID); } }
        /// <summary>
        /// Get the segment last section number for the EIT section.
        /// </summary>
        public int SegmentLastSectionNumber { get { return (segmentLastSectionNumber); } }

        private Collection<EITEntry> eitEntries;

        private int transportStreamID;
        private int originalNetworkID;
        private int serviceID;

        private int segmentLastSectionNumber;
        private int lastTableID;

        private static Logger titleLogger;
        private static Logger descriptionLogger;

        private int lastIndex;

        /// <summary>
        /// Initialize a new instance of the BellTVSection class.
        /// </summary>
        public BellTVSection()
        {
            eitEntries = new Collection<EITEntry>();
            Logger.ProtocolIndent = "";

            if (RunParameters.Instance.DebugIDs.Contains("LOGTITLES") && titleLogger == null)
                titleLogger = new Logger("EPG Titles.log");
            if (RunParameters.Instance.DebugIDs.Contains("LOGDESCRIPTIONS") && descriptionLogger == null)
                descriptionLogger = new Logger("EPG Descriptions.log");
        }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        public void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;
            serviceID = mpeg2Header.TableIDExtension;

            try
            {
                transportStreamID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                originalNetworkID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                segmentLastSectionNumber = (int)byteData[lastIndex];
                lastIndex++;

                lastTableID = (int)byteData[lastIndex];
                lastIndex++;
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The Bell TV section is short"));
            }

            TVStation tvStation = TVStation.FindStation(originalNetworkID, transportStreamID, serviceID);
            if (tvStation == null)
            {
                if (!RunParameters.Instance.DebugIDs.Contains("CREATESTATIONS"))
                    return;
                else
                {
                    tvStation = new TVStation("Auto Generated Station: " + originalNetworkID + ":" + transportStreamID + ":" + serviceID);
                    tvStation.OriginalNetworkID = originalNetworkID;
                    tvStation.TransportStreamID = transportStreamID;
                    tvStation.ServiceID = serviceID;

                    TVStation.StationCollection.Add(tvStation);
                }
            }

            bool newSection = tvStation.AddMapEntry(mpeg2Header.TableID, mpeg2Header.SectionNumber, lastTableID, mpeg2Header.LastSectionNumber, segmentLastSectionNumber);
            if (!newSection)
                return;

            while (lastIndex < byteData.Length - 4)
            {
                BellTVEntry bellTVEntry = new BellTVEntry();
                bellTVEntry.Process(byteData, lastIndex, mpeg2Header.TableID);

                EPGEntry epgEntry = new EPGEntry();
                epgEntry.OriginalNetworkID = tvStation.OriginalNetworkID;
                epgEntry.TransportStreamID = tvStation.TransportStreamID;
                epgEntry.ServiceID = tvStation.ServiceID;
                epgEntry.EPGSource = EPGSource.BellTV;

                if (bellTVEntry.HighDefinition)
                    epgEntry.VideoQuality = "HDTV";
                if (bellTVEntry.ClosedCaptions)
                    epgEntry.SubTitles = "teletext";
                if (bellTVEntry.Stereo)
                    epgEntry.AudioQuality = "stereo";

                epgEntry.Duration = Utils.RoundTime(bellTVEntry.Duration);
                epgEntry.EventID = bellTVEntry.EventID;
                epgEntry.EventName = bellTVEntry.EventName;
                
                getParentalRating(epgEntry, bellTVEntry);

                epgEntry.RunningStatus = bellTVEntry.RunningStatus;
                epgEntry.Scrambled = bellTVEntry.Scrambled;
                epgEntry.ShortDescription = bellTVEntry.ShortDescription;
                if (bellTVEntry.SubTitle != bellTVEntry.EventName)
                    epgEntry.EventSubTitle = bellTVEntry.SubTitle;
                epgEntry.StartTime = Utils.RoundTime(TimeOffsetEntry.GetOffsetTime(bellTVEntry.StartTime));

                epgEntry.EventCategory = getEventCategory(epgEntry.EventName, epgEntry.ShortDescription, bellTVEntry.ContentType, bellTVEntry.ContentSubType);

                epgEntry.StarRating = getStarRating(bellTVEntry);
                epgEntry.Date = bellTVEntry.Date;
                epgEntry.Cast = bellTVEntry.Cast;

                getSeriesEpisode(epgEntry, bellTVEntry.Series, bellTVEntry.Episode);

                epgEntry.HasGraphicLanguage = bellTVEntry.HasStrongLanguage;
                epgEntry.HasStrongSexualContent = bellTVEntry.HasSexualContent;
                epgEntry.HasGraphicViolence = bellTVEntry.HasViolence;
                epgEntry.HasNudity = bellTVEntry.HasNudity;

                epgEntry.PreviousPlayDate = bellTVEntry.OriginalAirDate;

                tvStation.AddEPGEntry(epgEntry);

                if (titleLogger != null)
                    logTitle(bellTVEntry.EventName, epgEntry, titleLogger);
                if (descriptionLogger != null)
                {
                    if (!RunParameters.Instance.DebugIDs.Contains("LOGORIGINAL"))
                        logDescription(bellTVEntry.ShortDescription, epgEntry, descriptionLogger);
                    else
                        logDescription(bellTVEntry.OriginalDescription, epgEntry, descriptionLogger);
                }

                lastIndex = bellTVEntry.Index;
            }
        }

        private string getEventCategory(string title, string description, int contentType, int contentSubType)
        {
            if (contentType == -1 || contentSubType == -1)
                return (getCustomCategory(title, description));

            if (contentType == 0 && contentSubType == 0)
                return (getCustomCategory(title, description));

            if (contentType == 0 && contentSubType == 255)
                return (getCustomCategory(title, description));

            if (RunParameters.Instance.Options.Contains("CUSTOMCATEGORYOVERRIDE"))
            {
                string customCategory = getCustomCategory(title, description);
                if (customCategory != null)
                    return (customCategory);
            }

            BellTVProgramCategory mainCategoryEntry = BellTVProgramCategory.FindCategory(contentType, 0);
            if (mainCategoryEntry == null)
            {
                BellTVProgramCategory.AddUndefinedCategory(contentType, 0, "", title);

                if (RunParameters.Instance.Options.Contains("CUSTOMCATEGORYOVERRIDE"))
                    return (null);

                return (getCustomCategory(title, description));
            }

            if (mainCategoryEntry.SampleEvent == null)
                mainCategoryEntry.SampleEvent = title;
            mainCategoryEntry.UsedCount++;
            return (mainCategoryEntry.Description);            
        }

        private string getCustomCategory(string title, string description)
        {
            string category = CustomProgramCategory.FindCategoryDescription(title);
            if (category != null)
                return (category);

            return (CustomProgramCategory.FindCategoryDescription(description));
        }

        private void getParentalRating(EPGEntry epgEntry, BellTVEntry entry)
        {
            epgEntry.ParentalRating = ParentalRating.FindRating("USA", "DISHNETWORK", entry.ParentalRating.ToString());
            epgEntry.MpaaParentalRating = ParentalRating.FindMpaaRating("USA", "DISHNETWORK", entry.ParentalRating.ToString());
            epgEntry.ParentalRatingSystem = ParentalRating.FindSystem("USA", "DISHNETWORK", entry.ParentalRating.ToString());

            /*switch (entry.ParentalRating)
            {
                case 0:
                    return (null);
                case 1:
                    return ("G");
                case 2:
                    return ("PG");
                case 3:
                    return ("PG-13");
                case 4:
                    return ("R");
                case 5:
                    return ("NR/AO");
                case 6:
                    return (null);
                case 7:
                    return ("NC-17");
                default:
                    return (null);
            }*/
        }

        private string getStarRating(BellTVEntry entry)
        {
            switch (entry.StarRating)
            {
                case 0:
                    return (null);
                case 1:
                    return ("*");
                case 2:
                    return ("*+");
                case 3:
                    return ("**");
                case 4:
                    return ("**+");
                case 5:
                    return ("***");
                case 6:
                    return ("***+");
                case 7:
                    return ("****");
                default:
                    return (null);
            }
        }

        private void getSeriesEpisode(EPGEntry epgEntry, int series, int episode)
        {
            if (series < 1 && episode < 1)
                return;

            epgEntry.EpisodeSystemType = "xmltv_ns";

            if (series > 0)
                epgEntry.Series = series.ToString();

            if (episode > 0)
                epgEntry.Episode = episode.ToString();
        }

        private void logTitle(string title, EPGEntry epgEntry, Logger logger)
        {
            string episodeInfo;

            if (RunParameters.Instance.DebugIDs.Contains("LOGEPISODEINFO"))
            {
                if (epgEntry.EpisodeSystemType != null)
                    episodeInfo = epgEntry.Series + ":" + epgEntry.Episode + ":" + epgEntry.PartNumber;
                else
                    episodeInfo = string.Empty;
            }
            else
                episodeInfo = string.Empty;

            logger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +
                epgEntry.StartTime.ToShortDateString() + " " +
                epgEntry.StartTime.ToString("HH:mm") + " - " +
                epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                title + " " +
                episodeInfo);
        }

        private void logDescription(string description, EPGEntry epgEntry, Logger logger)
        {
            logger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +
                epgEntry.StartTime.ToShortDateString() + " " +
                epgEntry.StartTime.ToString("HH:mm") + " - " +
                epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                description);
        }

        /// <summary>
        /// Validate the entry fields.
        /// </summary>
        public void Validate() { }

        /// <summary>
        /// Log the entry fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BELL TV SECTION: ONID: " + originalNetworkID +
                " TSID: " + transportStreamID +
                " SID: " + serviceID);
        }
    }
}
