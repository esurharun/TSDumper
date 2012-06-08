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
using System.Collections.ObjectModel;
using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes an FreeSat section.
    /// </summary>
    class FreeSatSection
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
        /// Initialize a new instance of the FreeSatSection class.
        /// </summary>
        public FreeSatSection()
        {
            eitEntries = new Collection<EITEntry>();
            Logger.ProtocolIndent = "";

            if (RunParameters.Instance.DebugIDs.Contains("LOGTITLES") && titleLogger == null)
                titleLogger = new Logger("EPG Titles.log");
            if (RunParameters.Instance.DebugIDs.Contains("LOGDESCRIPTIONS")&& descriptionLogger == null)
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
                throw (new ArgumentOutOfRangeException("The FreeSat EIT section is short"));
            }

            TVStation tvStation = TVStation.FindStation(originalNetworkID, transportStreamID, serviceID);
            if (tvStation == null)
                return;

            bool newSection = tvStation.AddMapEntry(mpeg2Header.TableID, mpeg2Header.SectionNumber, lastTableID, mpeg2Header.LastSectionNumber, segmentLastSectionNumber);
            if (!newSection)
                return;

            while (lastIndex < byteData.Length - 4)
            {
                FreeSatEntry freeSatEntry = new FreeSatEntry();
                freeSatEntry.Process(byteData, lastIndex);

                EPGEntry epgEntry = new EPGEntry();
                epgEntry.OriginalNetworkID = tvStation.OriginalNetworkID;
                epgEntry.TransportStreamID = tvStation.TransportStreamID;
                epgEntry.ServiceID = tvStation.ServiceID;
                epgEntry.EPGSource = EPGSource.FreeSat;

                switch (freeSatEntry.ComponentTypeAudio)
                {
                    case 3:
                        epgEntry.AudioQuality = "stereo";
                        break;
                    case 5:
                        epgEntry.AudioQuality = "dolby digital";
                        break;
                    default:
                        break;
                }

                if (freeSatEntry.ComponentTypeVideo > 9)
                    epgEntry.VideoQuality = "HDTV";

                epgEntry.Duration = Utils.RoundTime(freeSatEntry.Duration);
                epgEntry.EventID = freeSatEntry.EventID;
                epgEntry.EventName = freeSatEntry.EventName;

                if (freeSatEntry.ParentalRating > 11)
                    epgEntry.ParentalRating = "AO";
                else
                {
                    if (freeSatEntry.ParentalRating > 8)
                        epgEntry.ParentalRating = "PGR";
                    else
                        epgEntry.ParentalRating = "G";
                }

                setSeriesEpisode(epgEntry, freeSatEntry);                

                epgEntry.RunningStatus = freeSatEntry.RunningStatus;
                epgEntry.Scrambled = freeSatEntry.Scrambled;
                epgEntry.ShortDescription = freeSatEntry.ShortDescription;
                epgEntry.StartTime = Utils.RoundTime(TimeOffsetEntry.GetOffsetTime(freeSatEntry.StartTime));

                epgEntry.EventCategory = getEventCategory(epgEntry.EventName, epgEntry.ShortDescription, freeSatEntry.ContentType, freeSatEntry.ContentSubType);
                
                tvStation.AddEPGEntry(epgEntry);

                if (titleLogger != null)
                    logTitle(freeSatEntry.EventName, epgEntry, titleLogger);
                if (descriptionLogger != null)
                    logDescription(freeSatEntry.ShortDescription, epgEntry, descriptionLogger);

                lastIndex = freeSatEntry.Index;
            }
        }

        private string getEventCategory(string title, string description, int contentType, int contentSubType)
        {
            if (contentType == -1 || contentSubType == -1)
                return (getCustomCategory(title, description));

            if (RunParameters.Instance.Options.Contains("CUSTOMCATEGORYOVERRIDE"))
            {
                string customCategory = getCustomCategory(title, description);
                if (customCategory != null)
                    return (customCategory);
            }

            EITProgramContent contentEntry;

            if (RunParameters.Instance.Options.Contains("USECONTENTSUBTYPE"))
            {
                contentEntry = EITProgramContent.FindContent(contentType, contentSubType);
                if (contentEntry != null)
                {
                    if (contentEntry.SampleEvent == null)
                        contentEntry.SampleEvent = title;
                    contentEntry.UsedCount++;
                    return (contentEntry.Description);
                }
            }

            contentEntry = EITProgramContent.FindContent(contentType, 0);
            if (contentEntry != null)
            {
                if (contentEntry.SampleEvent == null)
                    contentEntry.SampleEvent = title;
                contentEntry.UsedCount++;
                return (contentEntry.Description);
            }

            EITProgramContent.AddUndefinedContent(contentType, contentSubType, "", title);

            if (RunParameters.Instance.Options.Contains("CUSTOMCATEGORYOVERRIDE"))
                return (null);

            return (getCustomCategory(title, description));
        }

        private string getCustomCategory(string title, string description)
        {
            string category = CustomProgramCategory.FindCategoryDescription(title);
            if (category != null)
                return (category);

            return (CustomProgramCategory.FindCategoryDescription(description));
        }

        private void setSeriesEpisode(EPGEntry epgEntry, FreeSatEntry freeSatEntry)
        {
            if (freeSatEntry.SeriesID == null && freeSatEntry.SeasonID == null && freeSatEntry.EpisodeID == null)
                return;

            if (RunParameters.Instance.Options.Contains("USENUMERICCRID"))
            {
                processCRIDNumeric(epgEntry, freeSatEntry.SeriesID, freeSatEntry.SeasonID, freeSatEntry.EpisodeID);
                return;
            }

            if (RunParameters.Instance.Options.Contains("USEBSEPG"))
            {
                processCRIDBSEPG(epgEntry, freeSatEntry.SeriesID, freeSatEntry.SeasonID, freeSatEntry.EpisodeID);
                return;
            }
        }

        private void processCRIDNumeric(EPGEntry epgEntry, string seriesID, string seasonID, string episodeID)
        {
            epgEntry.EpisodeSystemType = "xmltv_ns";
            epgEntry.EpisodeSystemParts = 3;

            if (seriesID != null && seasonID != null && episodeID != null)
            {
                epgEntry.Series = ((uint)seriesID.GetHashCode()).ToString();
                epgEntry.Episode = ((uint)seasonID.GetHashCode()).ToString();
                epgEntry.PartNumber = ((uint)episodeID.GetHashCode()).ToString();
                return;
            }

            if (seriesID != null && seasonID != null)
            {
                epgEntry.Series = ((uint)seriesID.GetHashCode()).ToString();
                epgEntry.Episode = ((uint)seasonID.GetHashCode()).ToString();
                return;
            }

            if (seriesID != null)
            {
                if (episodeID != null)
                {
                    epgEntry.Series = ((uint)seriesID.GetHashCode()).ToString();
                    epgEntry.Episode = ((uint)episodeID.GetHashCode()).ToString();
                }
                else
                    epgEntry.Series = ((uint)seriesID.GetHashCode()).ToString();

                return;
            }

            if (seasonID != null)
            {
                if (episodeID != null)
                {
                    epgEntry.Series = ((uint)seasonID.GetHashCode()).ToString();
                    epgEntry.Episode = ((uint)episodeID.GetHashCode()).ToString();
                }
                else
                    epgEntry.Series = ((uint)seasonID.GetHashCode()).ToString();

                return;
            }

            epgEntry.Episode = ((uint)episodeID.GetHashCode()).ToString();            
        }

        private void processCRIDBSEPG(EPGEntry epgEntry, string seriesID, string seasonID, string episodeID)
        {
            epgEntry.EpisodeSystemType = "bsepg-epid";
            epgEntry.EpisodeSystemParts = 2;

            string series = "SE-";
            string episode = "EP-";

            if (seriesID != null && seasonID != null && episodeID != null)
            {
                epgEntry.Series = series + ((uint)seasonID.GetHashCode()).ToString();
                epgEntry.Episode = episode + ((uint)episodeID.GetHashCode()).ToString();                
                return;
            }

            if (seriesID != null && seasonID != null)
            {
                epgEntry.Series = series + ((uint)seriesID.GetHashCode()).ToString();
                epgEntry.Episode = episode + ((uint)seasonID.GetHashCode()).ToString();                
                return;
            }

            if (seriesID != null)
            {
                if (episodeID != null)
                {
                    epgEntry.Series = series + ((uint)seriesID.GetHashCode()).ToString();
                    epgEntry.Episode = episode + ((uint)episodeID.GetHashCode()).ToString();
                }
                else
                {
                    epgEntry.Series = series + ((uint)seriesID.GetHashCode()).ToString();
                    epgEntry.Episode = episode;
                }

                return;
            }

            if (seasonID != null)
            {
                if (episodeID != null)
                {
                    epgEntry.Series = series + ((uint)seasonID.GetHashCode()).ToString();
                    epgEntry.Episode = episode + ((uint)episodeID.GetHashCode()).ToString();
                }
                else
                {
                    epgEntry.Series = series + ((uint)seasonID.GetHashCode()).ToString();
                    epgEntry.Episode = episode;
                }

                return;
            }

            epgEntry.Series = series;
            epgEntry.Episode = episode + ((uint)episodeID.GetHashCode()).ToString();            
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "FREESAT Section: ONID: " + originalNetworkID +
                " TSID: " + transportStreamID +
                " SID: " + serviceID);
        }
    }
}
