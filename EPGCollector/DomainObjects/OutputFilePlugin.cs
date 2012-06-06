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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes the xml DVBLogic output file.
    /// </summary>
    public sealed class OutputFilePlugin
    {
        private OutputFilePlugin() { }

        /// <summary>
        /// Create the file.
        /// </summary>
        /// <param name="fileName">The name of the file to be created.</param>
        /// <returns></returns>
        public static string Process(string fileName)
        {
            try
            {
                Logger.Instance.Write("Deleting any existing version of output file");
                File.SetAttributes(fileName, FileAttributes.Normal);
                File.Delete(fileName);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("File delete exception: " + e.Message);
            }

            Logger.Instance.Write("Creating plugin output file: " + fileName);

            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.Encoding = new UTF8Encoding(false);
            settings.CloseOutput = true;

            try
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(fileName, settings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("EPGInfo");

                    foreach (TVStation tvStation in TVStation.StationCollection)
                    {
                        if (!tvStation.Excluded && tvStation.EPGCollection.Count != 0)
                        {
                            xmlWriter.WriteStartElement("Channel");

                            processStationHeader(xmlWriter, tvStation);
                            processStationEPG(xmlWriter, tvStation);

                            xmlWriter.WriteEndElement();
                        }
                    }

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();

                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
            }
            catch (XmlException ex1)
            {
                return (ex1.Message);
            }
            catch (IOException ex2)
            {
                return (ex2.Message);
            }

            return (null);
        }

        private static void processStationHeader(XmlWriter xmlWriter, TVStation tvStation)
        {
            xmlWriter.WriteAttributeString("Name", tvStation.Name.Trim());

            xmlWriter.WriteAttributeString("ID", tvStation.OriginalNetworkID.ToString() + ":" +
                tvStation.TransportStreamID.ToString() + ":" +
                tvStation.ServiceID.ToString());
            xmlWriter.WriteAttributeString("NID", tvStation.OriginalNetworkID.ToString());
            xmlWriter.WriteAttributeString("TID", tvStation.TransportStreamID.ToString());
            xmlWriter.WriteAttributeString("SID", tvStation.ServiceID.ToString());
            xmlWriter.WriteAttributeString("Count", tvStation.EPGCollection.Count.ToString());
            xmlWriter.WriteAttributeString("FirstStart", getStartTime(tvStation.EPGCollection[0]));
            xmlWriter.WriteAttributeString("LastStart", getStartTime(tvStation.EPGCollection[tvStation.EPGCollection.Count - 1]));
        }

        private static void processStationEPG(XmlWriter xmlWriter, TVStation tvStation)
        {
            xmlWriter.WriteStartElement("dvblink_epg");

            for (int index = 0; index < tvStation.EPGCollection.Count; index++)
            {
                EPGEntry epgEntry = tvStation.EPGCollection[index];

                if (TuningFrequency.HasMHEG5Frequency)
                    checkMidnightBreak(tvStation, epgEntry, index);

                processEPGEntry(xmlWriter, epgEntry);
            }

            xmlWriter.WriteEndElement();
        }

        private static void checkMidnightBreak(TVStation tvStation, EPGEntry currentEntry, int index)
        {
            if (index == tvStation.EPGCollection.Count - 1)
                return;

            EPGEntry nextEntry = tvStation.EPGCollection[index + 1];

            if (currentEntry.EventName != nextEntry.EventName)
                return;

            bool combined = false;
            if (RunParameters.Instance.CountryCode == null)
                combined = checkNZLTimes(currentEntry, nextEntry);
            else
            {
                switch (RunParameters.Instance.CountryCode)
                {
                    case "NZL":
                        combined = checkNZLTimes(currentEntry, nextEntry);
                        break;
                    case "AUS":
                        combined = checkAUSTimes(currentEntry, nextEntry);
                        break;
                    default:
                        break;
                }
            }

            if (combined)
                tvStation.EPGCollection.RemoveAt(index + 1);
        }

        private static bool checkNZLTimes(EPGEntry currentEntry, EPGEntry nextEntry)
        {
            if (!currentEntry.EndsAtMidnight)
                return (false);

            if (!nextEntry.StartsAtMidnight)
                return (false);

            if (currentEntry.StartTime + currentEntry.Duration != nextEntry.StartTime)
                return (false);

            if (nextEntry.Duration > new TimeSpan(3, 0, 0))
                return (false);

            Logger.Instance.Write("Combining " + currentEntry.ScheduleDescription + " with " + nextEntry.ScheduleDescription);
            currentEntry.Duration = currentEntry.Duration + nextEntry.Duration;

            return (true);
        }

        private static bool checkAUSTimes(EPGEntry currentEntry, EPGEntry nextEntry)
        {
            if (!nextEntry.StartsAtMidnight)
                return (false);

            if (currentEntry.StartTime + currentEntry.Duration != nextEntry.StartTime + nextEntry.Duration)
                return (false);

            Logger.Instance.Write("Combining " + currentEntry.ScheduleDescription + " with " + nextEntry.ScheduleDescription);

            return (true);
        }

        private static void processEPGEntry(XmlWriter xmlWriter, EPGEntry epgEntry)
        {
            Regex whitespace = new Regex(@"\s+");

            xmlWriter.WriteStartElement("program");

            if (epgEntry.EventName != null)
                xmlWriter.WriteAttributeString("name", whitespace.Replace(epgEntry.EventName.Trim(), " "));
            else
                xmlWriter.WriteAttributeString("name", "No Title");

            /*TimeSpan timeSpan = epgEntry.StartTime - new DateTime(1970, 1, 1, 0, 0, 0, 0); 
            UInt32 seconds = Convert.ToUInt32(Math.Abs(timeSpan.TotalSeconds));*/
            xmlWriter.WriteElementString("start_time", getStartTime(epgEntry));
            xmlWriter.WriteElementString("duration", epgEntry.Duration.TotalSeconds.ToString());

            if (epgEntry.EventSubTitle != null)
                xmlWriter.WriteElementString("subname", whitespace.Replace(epgEntry.EventSubTitle.Trim(), " "));

            if (epgEntry.ShortDescription != null)
                xmlWriter.WriteElementString("short_desc", whitespace.Replace(epgEntry.ShortDescription.Trim(), " "));
            else
            {
                if (epgEntry.EventName != null)
                    xmlWriter.WriteElementString("short_desc", whitespace.Replace(epgEntry.EventName.Trim(), " "));
                else
                    xmlWriter.WriteElementString("short_desc", "No Description");
            }

            if (epgEntry.EventCategory != null)
            {
                xmlWriter.WriteElementString("categories", "");

                string[] categoryParts = epgEntry.EventCategory.Split(new char[] { ',' });

                foreach (string categoryPart in categoryParts)
                    xmlWriter.WriteElementString("cat_" + categoryPart.Trim(), "");
            }

            /* if (epgEntry.ParentalRating != null)
             {
                 xmlWriter.WriteStartElement("rating");
                 if (epgEntry.ParentalRatingSystem != null)
                     xmlWriter.WriteAttributeString("system", epgEntry.ParentalRatingSystem);
                 xmlWriter.WriteElementString("value", epgEntry.ParentalRating);
                 xmlWriter.WriteEndElement();
             }*/

            if (epgEntry.VideoQuality != null && epgEntry.VideoQuality.ToLowerInvariant() == "hdtv")
                xmlWriter.WriteElementString("hdtv", string.Empty);

            if (epgEntry.EpisodeSystemType != null && epgEntry.EpisodeSystemType == "xmltv_ns")
            {
                if (epgEntry.Series != null)
                    xmlWriter.WriteElementString("season_num", epgEntry.Series);
                if (epgEntry.Episode != null && epgEntry.Episode != "0")
                    xmlWriter.WriteElementString("episode_num", epgEntry.Episode);
            }

            if (epgEntry.Directors != null)
            {
                StringBuilder directorString = new StringBuilder();

                foreach (string director in epgEntry.Directors)
                {
                    if (directorString.Length != 0)
                        directorString.Append("/");
                    directorString.Append(director.Trim());
                }

                xmlWriter.WriteElementString("directors", directorString.ToString());
            }

            if (epgEntry.Cast != null)
            {
                StringBuilder castString = new StringBuilder();

                foreach (string castMember in epgEntry.Cast)
                {
                    if (castString.Length != 0)
                        castString.Append("/");
                    castString.Append(castMember.Trim());
                }

                xmlWriter.WriteElementString("actors", castString.ToString());
            }

            if (epgEntry.Date != null)
                xmlWriter.WriteElementString("year", epgEntry.Date);

            if (epgEntry.PreviousPlayDate != DateTime.MinValue)
                xmlWriter.WriteElementString("repeat", string.Empty);

            xmlWriter.WriteEndElement();
        }

        private static string getStartTime(EPGEntry epgEntry)
        {
            DateTime gmtStartTime = TimeZoneInfo.ConvertTimeToUtc(epgEntry.StartTime);
            TimeSpan timeSpan = gmtStartTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            UInt32 seconds = Convert.ToUInt32(Math.Abs(timeSpan.TotalSeconds));

            return (seconds.ToString());
        }
    }
}
