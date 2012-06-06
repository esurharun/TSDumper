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
    /// The class that describes an EIT section.
    /// </summary>
    public class EITSection
    {
        /// <summary>
        /// Get the collection of EIT entries in the section.
        /// </summary>
        public Collection<EITEntry> EITEntries { get { return (eitEntries); } }

        /// <summary>
        /// Get the collection of EIT category records.
        /// </summary>
        public static Collection<CategoryEntry> CategoryEntries { get { return (categoryEntries); } }

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

        private static Collection<CategoryEntry> categoryEntries;

        private int lastIndex;

        /// <summary>
        /// Initialize a new instance of the EITSection class.
        /// </summary>
        public EITSection()
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

            transportStreamID = Utils.Convert2BytesToInt(byteData, lastIndex);
            lastIndex += 2;

            originalNetworkID = Utils.Convert2BytesToInt(byteData, lastIndex);
            lastIndex += 2;

            segmentLastSectionNumber = (int)byteData[lastIndex];
            lastIndex++;

            lastTableID = (int)byteData[lastIndex];
            lastIndex++;

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
                EITEntry eitEntry = new EITEntry();
                eitEntry.Process(byteData, lastIndex);

                if (eitEntry.StartTime != DateTime.MinValue)
                {
                    EPGEntry epgEntry = new EPGEntry();
                    epgEntry.OriginalNetworkID = tvStation.OriginalNetworkID;
                    epgEntry.TransportStreamID = tvStation.TransportStreamID;
                    epgEntry.ServiceID = tvStation.ServiceID;
                    epgEntry.EPGSource = EPGSource.EIT;

                    switch (eitEntry.ComponentTypeAudio)
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

                    if (eitEntry.ComponentTypeVideo > 9)
                        epgEntry.VideoQuality = "HDTV";

                    if (!RunParameters.Instance.Options.Contains("USEDESCASCATEGORY"))
                        epgEntry.EventCategory = getEventCategory(eitEntry.EventName, eitEntry.Description, eitEntry.ContentType, eitEntry.ContentSubType);
                    else
                        epgEntry.EventCategory = eitEntry.ShortDescription;

                    epgEntry.Duration = Utils.RoundTime(eitEntry.Duration);
                    epgEntry.EventID = eitEntry.EventID;
                    epgEntry.EventName = eitEntry.EventName;

                    if (RunParameters.Instance.CountryCode != null)
                    {
                        epgEntry.ParentalRating = ParentalRating.FindRating(RunParameters.Instance.CountryCode, "EIT", (eitEntry.ParentalRating + 3).ToString());
                        epgEntry.MpaaParentalRating = ParentalRating.FindMpaaRating(RunParameters.Instance.CountryCode, "EIT", (eitEntry.ParentalRating + 3).ToString());
                    }
                    else
                    {
                        if (eitEntry.ParentalRating > 11)
                        {
                            epgEntry.ParentalRating = "AO";
                            epgEntry.MpaaParentalRating = "AO";
                        }
                        else
                        {
                            if (eitEntry.ParentalRating > 8)
                            {
                                epgEntry.ParentalRating = "PGR";
                                epgEntry.MpaaParentalRating = "PG";
                            }
                            else
                            {
                                epgEntry.ParentalRating = "G";
                                epgEntry.MpaaParentalRating = "G";
                            }
                        }
                    }

                    epgEntry.RunningStatus = eitEntry.RunningStatus;
                    epgEntry.Scrambled = eitEntry.Scrambled;

                    if (!RunParameters.Instance.Options.Contains("USEDESCASCATEGORY"))
                        epgEntry.ShortDescription = eitEntry.Description;
                    else
                        epgEntry.ShortDescription = eitEntry.ExtendedDescription;

                    epgEntry.StartTime = Utils.RoundTime(TimeOffsetEntry.GetOffsetTime(eitEntry.StartTime));

                    epgEntry.Cast = eitEntry.Cast;
                    epgEntry.Directors = eitEntry.Directors;
                    epgEntry.Date = eitEntry.Year;
                    if (eitEntry.TVRating != null)
                        epgEntry.ParentalRating = eitEntry.TVRating;
                    epgEntry.StarRating = eitEntry.StarRating;

                    if (eitEntry.TVRating != null)
                        epgEntry.ParentalRating = eitEntry.TVRating;

                    setSeriesEpisode(epgEntry, eitEntry);

                    /*if (eitEntry.PreviousPlayDate != null)
                    {
                        try
                        {
                            TimeSpan offset = new TimeSpan(Int32.Parse(eitEntry.PreviousPlayDate) * TimeSpan.TicksPerSecond);
                            epgEntry.PreviousPlayDate = epgEntry.StartTime - offset;
                        }
                        catch (FormatException) { }
                    }*/

                    epgEntry.Country = eitEntry.Country;

                    tvStation.AddEPGEntry(epgEntry);

                    if (titleLogger != null)
                        logTitle(eitEntry.EventName, eitEntry, epgEntry, titleLogger);
                    if (descriptionLogger != null)
                        logTitle(eitEntry.Description, eitEntry, epgEntry, descriptionLogger);

                    if (RunParameters.Instance.DebugIDs.Contains("CATXREF"))
                        updateCategoryEntries(tvStation, eitEntry);
                }

                lastIndex = eitEntry.Index;
            }
        }

        private void setSeriesEpisode(EPGEntry epgEntry, EITEntry eitEntry)
        {
            if (eitEntry.SeriesID == null && eitEntry.SeasonID == null && eitEntry.EpisodeID == null)
                return;

            epgEntry.EpisodeSystemType = "xmltv_ns";

            if (eitEntry.SeriesID != null && eitEntry.SeasonID != null && eitEntry.EpisodeID != null)
            {
                epgEntry.Series = eitEntry.SeriesID;
                epgEntry.Episode = eitEntry.SeasonID;
                epgEntry.PartNumber = eitEntry.EpisodeID;
                return;
            }

            if (eitEntry.SeriesID != null && eitEntry.SeasonID != null)
            {
                epgEntry.Series = eitEntry.SeriesID;
                epgEntry.Episode = eitEntry.SeasonID;
                epgEntry.PartNumber = "0";
                return;
            }
                
            if (eitEntry.SeriesID != null)
            {
                if (eitEntry.EpisodeID != null)
                {
                    epgEntry.Series = eitEntry.SeriesID;
                    epgEntry.Episode = eitEntry.EpisodeID;
                }
                else
                {
                    epgEntry.Series = eitEntry.SeriesID;
                    epgEntry.Episode = "0";
                }

                epgEntry.PartNumber = "0";
                return;
            }
                
            if (eitEntry.SeasonID != null)
            {
                if (eitEntry.EpisodeID != null)
                {
                    epgEntry.Series = eitEntry.SeasonID;
                    epgEntry.Episode = eitEntry.EpisodeID;
                }
                else
                {
                    epgEntry.Series = eitEntry.SeasonID;
                    epgEntry.Episode = "0";
                }

                epgEntry.PartNumber = "0";
                return;
            }
                        
            epgEntry.Series = "0";
            epgEntry.Episode = eitEntry.EpisodeID;
            epgEntry.PartNumber = "0";
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

            return(CustomProgramCategory.FindCategoryDescription(description));            
        }

        private void logTitle(string title, EITEntry eitEntry, EPGEntry epgEntry, Logger logger)
        {
            logger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +            
                epgEntry.StartTime.ToShortDateString() + " " +
                epgEntry.StartTime.ToString("HH:mm") + " - " +
                epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                "Content: " + eitEntry.ContentType + "/" + eitEntry.ContentSubType + " " +
                title);
        }

        private void logDescription(string description, EITEntry eitEntry, EPGEntry epgEntry, Logger logger)
        {
            logger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +
                epgEntry.StartTime.ToShortDateString() + " " +
                epgEntry.StartTime.ToString("HH:mm") + " - " +
                epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                "Content: " + eitEntry.ContentType + "/" + eitEntry.ContentSubType + " " +
                description);
        }

        private void updateCategoryEntries(TVStation tvStation, EITEntry eitEntry)
        {
            if (categoryEntries == null)
                categoryEntries = new Collection<CategoryEntry>();

            CategoryEntry newEntry = new CategoryEntry(tvStation.OriginalNetworkID, tvStation.TransportStreamID, tvStation.ServiceID, eitEntry.StartTime, eitEntry.EventName, eitEntry.ContentType, eitEntry.ContentSubType);

            foreach (CategoryEntry oldEntry in categoryEntries)
            {
                if (oldEntry.NetworkID == newEntry.NetworkID &&
                    oldEntry.TransportStreamID == newEntry.TransportStreamID &&
                    oldEntry.ServiceID == newEntry.ServiceID &&
                    oldEntry.StartTime == newEntry.StartTime)
                    return;

                if (oldEntry.NetworkID > newEntry.NetworkID)
                {
                    categoryEntries.Insert(categoryEntries.IndexOf(oldEntry), newEntry);
                    return;
                }

                if (oldEntry.NetworkID == newEntry.NetworkID &&
                    oldEntry.TransportStreamID > newEntry.TransportStreamID)
                {
                    categoryEntries.Insert(categoryEntries.IndexOf(oldEntry), newEntry);
                    return;
                }

                if (oldEntry.NetworkID == newEntry.NetworkID &&
                    oldEntry.TransportStreamID == newEntry.TransportStreamID &&
                    oldEntry.ServiceID > newEntry.ServiceID)
                {
                    categoryEntries.Insert(categoryEntries.IndexOf(oldEntry), newEntry);
                    return;
                }

                if (oldEntry.NetworkID == newEntry.NetworkID &&
                    oldEntry.TransportStreamID == newEntry.TransportStreamID &&
                    oldEntry.ServiceID == newEntry.ServiceID &&
                    oldEntry.StartTime > newEntry.StartTime)
                {
                    categoryEntries.Insert(categoryEntries.IndexOf(oldEntry), newEntry);
                    return;
                }
            }

            categoryEntries.Add(newEntry);
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "EIT Section: ONID: " + originalNetworkID +
                " TSID: " + transportStreamID +
                " SID: " + serviceID);
        }
    }
}
