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
using System.Reflection;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes the xmltv file.
    /// </summary>
    public sealed class OutputFileXML
    {
        private OutputFileXML() { }

        internal static string Process(string fileName)
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

            Logger.Instance.Write("Creating output file: " + fileName);

            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            if (!OutputFile.UseUnicodeEncoding)
                settings.Encoding = new UTF8Encoding();
            else
                settings.Encoding = new UnicodeEncoding();
            settings.CloseOutput = true;
            
            if (RunParameters.Instance.DebugIDs.Contains("IGNOREXMLCHARS"))
                settings.CheckCharacters = false;

            try
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(fileName, settings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("tv");

                    xmlWriter.WriteAttributeString("generator-info-name", Assembly.GetCallingAssembly().GetName().Name
                        + "/" + Assembly.GetCallingAssembly().GetName().Version.ToString());

                    foreach (TVStation tvStation in TVStation.StationCollection)
                    {
                        if (!tvStation.Excluded && tvStation.EPGCollection.Count != 0)
                            processStationHeader(xmlWriter, tvStation);
                    }

                    foreach (TVStation tvStation in TVStation.StationCollection)
                    {
                        if (!tvStation.Excluded && tvStation.EPGCollection.Count != 0)
                            processStationEPG(xmlWriter, tvStation);
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

            if (RunParameters.Instance.Options.Contains("CREATEBRCHANNELS"))
                OutputFileBladeRunner.Process(fileName);

            if (RunParameters.Instance.Options.Contains("CREATEARCHANNELS"))
                OutputFileAreaRegionChannels.Process(fileName);

            return (null);
        }

        private static void processStationHeader(XmlWriter xmlWriter, TVStation tvStation)
        {
            xmlWriter.WriteStartElement("channel");

            if (tvStation.ChannelID == null)
            {
                if (!RunParameters.Instance.Options.Contains("USECHANNELID"))
                    xmlWriter.WriteAttributeString("id", tvStation.ServiceID.ToString());
                else
                {
                    if (tvStation.LogicalChannelNumber != -1)
                        xmlWriter.WriteAttributeString("id", tvStation.LogicalChannelNumber.ToString());
                    else
                        xmlWriter.WriteAttributeString("id", tvStation.ServiceID.ToString());
                }
            }
            else
                xmlWriter.WriteAttributeString("id", tvStation.ChannelID);


            if (tvStation.NewName == null)
                xmlWriter.WriteElementString("display-name", tvStation.Name);
            else
                xmlWriter.WriteElementString("display-name", tvStation.NewName);

            if (RunParameters.Instance.Options.Contains("USELCN"))
            {
                if (tvStation.ChannelID != null)
                    xmlWriter.WriteElementString("lcn", tvStation.ChannelID);
                else
                {
                    if (tvStation.LogicalChannelNumber != -1)
                        xmlWriter.WriteElementString("lcn", tvStation.LogicalChannelNumber.ToString());
                }
            }

            if (RunParameters.Instance.Options.Contains("USEIMAGE"))
            {
                if (File.Exists(RunParameters.DataDirectory + "\\Images\\" + tvStation.ServiceID + ".png"))
                {
                    xmlWriter.WriteStartElement("icon");
                    xmlWriter.WriteAttributeString("src", "file://" + RunParameters.DataDirectory + "\\Images\\" + tvStation.ServiceID + ".png");
                    xmlWriter.WriteEndElement();
                }
            }

            xmlWriter.WriteEndElement();
        }

        private static void processStationEPG(XmlWriter xmlWriter, TVStation tvStation)
        {
            Regex whitespace = new Regex(@"\s+");

            string channelNumber;
            if (tvStation.ChannelID == null)
            {
                if (!RunParameters.Instance.Options.Contains("USECHANNELID"))
                    channelNumber = tvStation.ServiceID.ToString();
                else
                {
                    if (tvStation.LogicalChannelNumber != -1)
                        channelNumber = tvStation.LogicalChannelNumber.ToString();
                    else
                        channelNumber = tvStation.ServiceID.ToString();
                }
            }
            else
                channelNumber = tvStation.ChannelID;

            for (int index = 0; index < tvStation.EPGCollection.Count; index++)
            {
                EPGEntry epgEntry = tvStation.EPGCollection[index];
                processEPGEntry(xmlWriter, channelNumber, epgEntry);
            }
        }

        private static void processEPGEntry(XmlWriter xmlWriter, string channelNumber, EPGEntry epgEntry)
        {
            Regex whitespace = new Regex(@"\s+");

            xmlWriter.WriteStartElement("programme");

            xmlWriter.WriteAttributeString("start", epgEntry.StartTime.ToString("yyyyMMddHHmmss zzz").Replace(":", ""));
            xmlWriter.WriteAttributeString("stop", (epgEntry.StartTime + epgEntry.Duration).ToString("yyyyMMddHHmmss zzz").Replace(":", ""));

            xmlWriter.WriteAttributeString("channel", channelNumber);

            if (epgEntry.EventName != null)
                xmlWriter.WriteElementString("title", TextTranslator.GetTranslatedText(
                    RunParameters.Instance.InputLanguage,
                    RunParameters.Instance.OutputLanguage,
                    whitespace.Replace(epgEntry.EventName.Trim(), " ")));
            else
                xmlWriter.WriteElementString("title", TextTranslator.GetTranslatedText(
                    RunParameters.Instance.InputLanguage,
                    RunParameters.Instance.OutputLanguage, "No Title"));

            if (!RunParameters.Instance.Options.Contains("USEDVBVIEWER"))
            {
                if (epgEntry.EventSubTitle != null)
                    xmlWriter.WriteElementString("sub-title", TextTranslator.GetTranslatedText(
                        RunParameters.Instance.InputLanguage,
                        RunParameters.Instance.OutputLanguage,
                        whitespace.Replace(epgEntry.EventSubTitle.Trim(), " ")));

                if (epgEntry.ShortDescription != null)
                    xmlWriter.WriteElementString("desc", TextTranslator.GetTranslatedText(
                        RunParameters.Instance.InputLanguage,
                        RunParameters.Instance.OutputLanguage,
                        whitespace.Replace(epgEntry.ShortDescription.Trim(), " ")));
                else
                {
                    if (epgEntry.EventName != null)
                        xmlWriter.WriteElementString("desc", TextTranslator.GetTranslatedText(
                            RunParameters.Instance.InputLanguage,
                            RunParameters.Instance.OutputLanguage,
                            whitespace.Replace(epgEntry.EventName.Trim(), " ")));
                    else
                        xmlWriter.WriteElementString("desc", TextTranslator.GetTranslatedText(
                            RunParameters.Instance.InputLanguage,
                            RunParameters.Instance.OutputLanguage,
                            "No Description"));
                }
            }
            else
            {
                if (epgEntry.ShortDescription != null)
                    xmlWriter.WriteElementString("sub-title", whitespace.Replace(epgEntry.ShortDescription.Trim(), " "));
                else
                {
                    if (epgEntry.EventName != null)
                        xmlWriter.WriteElementString("sub-title", whitespace.Replace(epgEntry.EventName.Trim(), " "));
                    else
                        xmlWriter.WriteElementString("sub-title", "No Description");
                }
            }

            if (epgEntry.EventCategory != null)
            {
                string[] categories = epgEntry.EventCategory.Split(new char[] { ',' });

                foreach (string category in categories)
                    xmlWriter.WriteElementString("category", category);
            }

            if (epgEntry.ParentalRating != null)
            {
                xmlWriter.WriteStartElement("rating");
                if (epgEntry.ParentalRatingSystem != null)
                    xmlWriter.WriteAttributeString("system", epgEntry.ParentalRatingSystem);
                xmlWriter.WriteElementString("value", epgEntry.ParentalRating);
                xmlWriter.WriteEndElement();
            }

            if (epgEntry.StarRating != null)
            {
                xmlWriter.WriteStartElement("star-rating");
                xmlWriter.WriteElementString("value", epgEntry.StarRating);
                xmlWriter.WriteEndElement();
            }

            if (epgEntry.AspectRatio != null || epgEntry.VideoQuality != null)
            {
                xmlWriter.WriteStartElement("video");
                if (epgEntry.AspectRatio != null)
                    xmlWriter.WriteElementString("aspect", epgEntry.AspectRatio);
                if (epgEntry.VideoQuality != null)
                    xmlWriter.WriteElementString("quality", epgEntry.VideoQuality);
                xmlWriter.WriteEndElement();
            }

            if (epgEntry.AudioQuality != null)
            {
                xmlWriter.WriteStartElement("audio");
                xmlWriter.WriteElementString("stereo", epgEntry.AudioQuality);
                xmlWriter.WriteEndElement();
            }

            if (epgEntry.SubTitles != null)
            {
                xmlWriter.WriteStartElement("subtitles");
                xmlWriter.WriteAttributeString("type", epgEntry.SubTitles);
                xmlWriter.WriteEndElement();
            }

            if (epgEntry.EpisodeSystemType != null)
            {
                string series;
                if (epgEntry.Series != null)
                    series = epgEntry.Series;
                else
                    series = "";

                string episode;
                if (epgEntry.Episode != null)
                    episode = epgEntry.Episode;
                else
                    episode = "";

                string partNumber;
                if (epgEntry.PartNumber != null)
                    partNumber = epgEntry.PartNumber;
                else
                    partNumber = "";

                xmlWriter.WriteStartElement("episode-num");
                xmlWriter.WriteAttributeString("system", epgEntry.EpisodeSystemType);

                if (epgEntry.EpisodeSystemParts != 2)
                    xmlWriter.WriteString(series + " . " + episode + " . " + partNumber);
                else
                    xmlWriter.WriteString(series + " . " + episode);

                xmlWriter.WriteEndElement();
            }

            if (epgEntry.PreviousPlayDate != DateTime.MinValue)
            {
                xmlWriter.WriteStartElement("previously-shown");
                xmlWriter.WriteAttributeString("start", epgEntry.PreviousPlayDate.ToString("yyyyMMddHHmmss zzz").Replace(":", ""));
                xmlWriter.WriteEndElement();
            }

            if ((epgEntry.Directors != null && epgEntry.Directors.Count != 0) || (epgEntry.Cast != null && epgEntry.Cast.Count != 0))
            {
                xmlWriter.WriteStartElement("credits");

                if (epgEntry.Directors != null)
                {
                    foreach (string director in epgEntry.Directors)
                        xmlWriter.WriteElementString("director", director.Trim());
                }

                if (epgEntry.Cast != null)
                {
                    foreach (string castMember in epgEntry.Cast)
                        xmlWriter.WriteElementString("actor", castMember.Trim());
                }

                xmlWriter.WriteEndElement();
            }

            if (epgEntry.Date != null)
                xmlWriter.WriteElementString("date", epgEntry.Date);

            if (epgEntry.Country != null)
                xmlWriter.WriteElementString("country", epgEntry.Country);

            xmlWriter.WriteEndElement();
        }

        private static void createBladeRunnerChannelFile(string fileName)
        {
            string bladeRunnerFileName = Path.Combine(Path.GetDirectoryName(fileName), "ChannelInfo.xml");

            try
            {
                Logger.Instance.Write("Deleting any existing version of BladeRunner channel file");
                File.SetAttributes(bladeRunnerFileName, FileAttributes.Normal);
                File.Delete(bladeRunnerFileName);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("File delete exception: " + e.Message);
            }

            Logger.Instance.Write("Creating BladeRunner channel file: " + bladeRunnerFileName);

            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.Encoding = new UTF8Encoding(false);
            settings.CloseOutput = true;

            int channelNumber = 0;

            using (XmlWriter xmlWriter = XmlWriter.Create(bladeRunnerFileName, settings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("NewDataSet");

                foreach (TVStation tvStation in TVStation.StationCollection)
                {
                    if (!tvStation.Excluded && tvStation.EPGCollection.Count != 0)
                    {
                        xmlWriter.WriteStartElement("Channel");

                        if (tvStation.NewName == null)
                            xmlWriter.WriteElementString("Name", tvStation.Name);
                        else
                            xmlWriter.WriteElementString("Name", tvStation.NewName);

                        if (!RunParameters.Instance.Options.Contains("USECHANNELID"))
                            xmlWriter.WriteElementString("channelID", tvStation.ServiceID.ToString());
                        else
                        {
                            if (tvStation.LogicalChannelNumber != -1)
                                xmlWriter.WriteElementString("channelID", tvStation.LogicalChannelNumber.ToString());
                            else
                                xmlWriter.WriteElementString("channelID", tvStation.ServiceID.ToString());
                        }

                        channelNumber++;
                        xmlWriter.WriteElementString("virtualchannel", channelNumber.ToString());

                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();

                xmlWriter.Flush();
                xmlWriter.Close();
            }
        }

        private static void createAreaRegionChannelFile(string fileName)
        {
            string regionChannelFileName = Path.Combine(Path.GetDirectoryName(fileName), "AreaRegionChannelInfo.xml");

            try
            {
                Logger.Instance.Write("Deleting any existing version of Area/Region channel file");
                File.SetAttributes(regionChannelFileName, FileAttributes.Normal);
                File.Delete(regionChannelFileName);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("File delete exception: " + e.Message);
            }

            Logger.Instance.Write("Creating Area/Region channel file: " + regionChannelFileName);

            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.Encoding = new UTF8Encoding(false);
            settings.CloseOutput = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(regionChannelFileName, settings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("areas");

                foreach (Bouquet bouquet in Bouquet.Bouquets)
                {
                    bool writeStart = true;

                    foreach (Region region in bouquet.Regions)
                    {
                        bool include = checkArea(bouquet.BouquetID, region.Code);
                        if (include)
                        {
                            if (writeStart)
                            {
                                xmlWriter.WriteStartElement("area");
                                xmlWriter.WriteAttributeString("id", bouquet.BouquetID.ToString());
                                xmlWriter.WriteAttributeString("name", bouquet.Name.ToString());
                                writeStart = false;
                            }

                            xmlWriter.WriteStartElement("region");
                            xmlWriter.WriteAttributeString("id", region.Code.ToString());

                            foreach (Channel channel in region.GetChannelsInChannelNumberOrder())
                            {
                                {
                                    TVStation station = TVStation.FindStation(channel.OriginalNetworkID, channel.TransportStreamID, channel.ServiceID);
                                    if (station != null)
                                    {
                                        xmlWriter.WriteStartElement("channel");

                                        xmlWriter.WriteAttributeString("id", channel.UserChannel.ToString());
                                        xmlWriter.WriteAttributeString("nid", channel.OriginalNetworkID.ToString());
                                        xmlWriter.WriteAttributeString("tid", channel.TransportStreamID.ToString());
                                        xmlWriter.WriteAttributeString("sid", channel.ServiceID.ToString());

                                        if (station.NewName == null)
                                            xmlWriter.WriteAttributeString("name", station.Name);
                                        else
                                            xmlWriter.WriteAttributeString("name", station.NewName);

                                        xmlWriter.WriteEndElement();
                                    }
                                }
                            }

                            xmlWriter.WriteEndElement();
                        }
                    }

                    if (!writeStart)
                        xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();

                xmlWriter.Flush();
                xmlWriter.Close();
            }
        }

        private static bool checkArea(int bouquet, int region)
        {
            if (RunParameters.Instance.ChannelBouquet == -1)
                return (true);

            if (bouquet != RunParameters.Instance.ChannelBouquet)
                return (false);

            if (RunParameters.Instance.ChannelRegion == -1)
                return (true);

            if (region == 65535)
                return (true);

            return (region == RunParameters.Instance.ChannelRegion);
        }
    }
}
