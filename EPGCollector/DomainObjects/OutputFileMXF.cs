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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace DomainObjects
{
    /// <summary>
    /// The class that creates an MXF file for import to 7MC.
    /// </summary>
    public sealed class OutputFileMXF
    {
        private static string actualFileName;

        private static Collection<KeywordGroup> groups;
        private static Collection<string> people;
        private static Collection<string> series;
        private static Collection<int> guideImages;

        private static string importName;
        private static string importReference;

        private static Process importProcess;
        private static bool importExited;

        private OutputFileMXF() { }

        /// <summary>
        /// Create the MXF file.
        /// </summary>
        /// <returns>An error message if the process fails; null otherwise.</returns>
        public static string Process()
        {
            string clearReply = WMCUtility.Run("clear series data", "CLEARSERIES");
            if (clearReply != null)
                Logger.Instance.Write("<e> " + clearReply);

            actualFileName = Path.Combine(RunParameters.DataDirectory, "TVGuide.mxf");

            try
            {
                Logger.Instance.Write("Deleting any existing version of output file");
                File.SetAttributes(actualFileName, FileAttributes.Normal);
                File.Delete(actualFileName);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("File delete exception: " + e.Message);
            }

            if (RunParameters.Instance.WMCImportName != null)
                importName = RunParameters.Instance.WMCImportName;

            if (importName == null)
                importName = "EPG Collector";
            importReference = importName.Replace(" ", string.Empty);
            Logger.Instance.Write("Import name set to '" + importName + "'");

            Logger.Instance.Write("Creating output file: " + actualFileName);

            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            if (!OutputFile.UseUnicodeEncoding)
                settings.Encoding = new UTF8Encoding();
            else
                settings.Encoding = new UnicodeEncoding();
            settings.CloseOutput = true;

            try
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(actualFileName, settings))
                {
                    xmlWriter.WriteStartDocument();

                    xmlWriter.WriteStartElement("MXF");
                    xmlWriter.WriteAttributeString("xmlns", "sql", null, "urn:schemas-microsoft-com:XML-sql");
                    xmlWriter.WriteAttributeString("xmlns", "xsi", null, @"http://www.w3.org/2001/XMLSchema-instance");

                    xmlWriter.WriteStartElement("Assembly");
                    xmlWriter.WriteAttributeString("name", "mcepg");
                    xmlWriter.WriteAttributeString("version", "6.0.6000.0");
                    xmlWriter.WriteAttributeString("cultureInfo", "");
                    xmlWriter.WriteAttributeString("publicKey", "0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9");
                    xmlWriter.WriteStartElement("NameSpace");
                    xmlWriter.WriteAttributeString("name", "Microsoft.MediaCenter.Guide");

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "Lineup");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "Channel");
                    xmlWriter.WriteAttributeString("parentFieldName", "lineup");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "Service");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "ScheduleEntry");
                    xmlWriter.WriteAttributeString("groupName", "ScheduleEntries");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "Program");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "Keyword");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "KeywordGroup");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "Person");
                    xmlWriter.WriteAttributeString("groupName", "People");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "ActorRole");
                    xmlWriter.WriteAttributeString("parentFieldName", "program");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "DirectorRole");
                    xmlWriter.WriteAttributeString("parentFieldName", "program");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "WriterRole");
                    xmlWriter.WriteAttributeString("parentFieldName", "program");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "HostRole");
                    xmlWriter.WriteAttributeString("parentFieldName", "program");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "GuestActorRole");
                    xmlWriter.WriteAttributeString("parentFieldName", "program");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "ProducerRole");
                    xmlWriter.WriteAttributeString("parentFieldName", "program");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "GuideImage");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "Affiliate");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "SeriesInfo");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "Season");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Assembly");
                    xmlWriter.WriteAttributeString("name", "mcstore");
                    xmlWriter.WriteAttributeString("version", "6.0.6000.0");
                    xmlWriter.WriteAttributeString("cultureInfo", "");
                    xmlWriter.WriteAttributeString("publicKey", "0024000004800000940000000602000000240000525341310004000001000100B5FC90E7027F67871E773A8FDE8938C81DD402BA65B9201D60593E96C492651E889CC13F1415EBB53FAC1131AE0BD333C5EE6021672D9718EA31A8AEBD0DA0072F25D87DBA6FC90FFD598ED4DA35E44C398C454307E8E33B8426143DAEC9F596836F97C8F74750E5975C64E2189F45DEF46B2A2B1247ADC3652BF5C308055DA9");
                    xmlWriter.WriteStartElement("NameSpace");
                    xmlWriter.WriteAttributeString("name", "Microsoft.MediaCenter.Store");

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "Provider");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Type");
                    xmlWriter.WriteAttributeString("name", "UId");
                    xmlWriter.WriteAttributeString("parentFieldName", "target");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Providers");
                    xmlWriter.WriteStartElement("Provider");
                    xmlWriter.WriteAttributeString("id", "provider1");
                    xmlWriter.WriteAttributeString("name", importReference);
                    xmlWriter.WriteAttributeString("displayName", importName);
                    xmlWriter.WriteAttributeString("copyright", "");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("With");
                    xmlWriter.WriteAttributeString("provider", "provider1");

                    xmlWriter.WriteStartElement("Keywords");
                    processKeywords(xmlWriter);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("KeywordGroups");
                    processKeywordGroups(xmlWriter);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("GuideImages");
                    processGuideImages(xmlWriter);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("People");
                    processPeople(xmlWriter);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("SeriesInfos");
                    processSeries(xmlWriter);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Seasons");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Programs");
                    processPrograms(xmlWriter);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Affiliates");
                    processAffiliates(xmlWriter);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Services");
                    processServices(xmlWriter);
                    xmlWriter.WriteEndElement();

                    processSchedules(xmlWriter);

                    xmlWriter.WriteStartElement("Lineups");
                    processLineUps(xmlWriter);
                    xmlWriter.WriteEndElement();

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

            string reply = runImportUtility(actualFileName);
            if (reply != null)
                return (reply);

            if (RunParameters.Instance.Options.Contains("CREATEBRCHANNELS"))
                OutputFileBladeRunner.Process(actualFileName);

            if (RunParameters.Instance.Options.Contains("CREATEARCHANNELS"))
                OutputFileAreaRegionChannels.Process(actualFileName);

            return (null);
        }

        private static void processKeywords(XmlWriter xmlWriter)
        {
            groups = new Collection<KeywordGroup>();
            groups.Add(new KeywordGroup("General"));

            foreach (TVStation station in TVStation.StationCollection)
            {
                if (!station.Excluded)
                {
                    foreach (EPGEntry epgEntry in station.EPGCollection)
                    {
                        if (epgEntry.EventCategory != null)
                            processCategory(xmlWriter, groups, epgEntry.EventCategory); 
                    }
                }
            }

            foreach (KeywordGroup group in groups)
            {
                xmlWriter.WriteStartElement("Keyword");
                xmlWriter.WriteAttributeString("id", "k" + ((groups.IndexOf(group) + 1)));
                xmlWriter.WriteAttributeString("word", group.Name.Trim());
                xmlWriter.WriteEndElement();

                foreach (string keyword in group.Keywords)
                {
                    xmlWriter.WriteStartElement("Keyword");
                    xmlWriter.WriteAttributeString("id", "k" + (((groups.IndexOf(group) + 1) * 100) + group.Keywords.IndexOf(keyword)));
                    xmlWriter.WriteAttributeString("word", keyword.Trim());
                    xmlWriter.WriteEndElement();
                }
            }
        }

        private static void processCategory(XmlWriter xmlWriter, Collection<KeywordGroup> groups, string category)
        {
            string[] parts = removeSpecialCategories(category);
            if (parts == null)
                return;

            if (parts.Length == 1)
            {                
                foreach (KeywordGroup keywordGroup in groups)
                {
                    if (keywordGroup.Name == parts[0])
                        return;
                }

                KeywordGroup singleGroup = new KeywordGroup(parts[0]);
                singleGroup.Keywords.Add("All");
                groups.Add(singleGroup);
                return;
            }

            foreach (KeywordGroup group in groups)
            {
                if (group.Name == parts[0])
                {
                    for (int index = 1; index < parts.Length; index++)
                    {
                        bool keywordFound = false;

                        foreach (string keyword in group.Keywords)
                        {
                            if (keyword == parts[index])
                                keywordFound = true;
                        }

                        if (!keywordFound)
                        {
                            if (group.Keywords.Count == 0)
                                group.Keywords.Add("All");
                            group.Keywords.Add(parts[index]);
                        }
                    }

                    return;
                }
            }

            KeywordGroup newGroup = new KeywordGroup(parts[0]);
            newGroup.Keywords.Add("All");

            for (int partAddIndex = 1; partAddIndex < parts.Length; partAddIndex++)
                newGroup.Keywords.Add(parts[partAddIndex]);

            groups.Add(newGroup);            
        }

        private static string[] removeSpecialCategories(string category)
        {
            string[] parts = category.Split(new string[] { "," }, StringSplitOptions.None);

            int specialCategoryCount = 0;

            foreach (string part in parts)
            {
                string specialCategory = getSpecialCategory(part);
                if (specialCategory != null)
                    specialCategoryCount++;
            }

            if (specialCategoryCount == parts.Length)
                return (null);

            string[] editedParts = new string[parts.Length - specialCategoryCount];
            int index = 0;

            foreach (string part in parts)
            {
                string specialCategory = getSpecialCategory(part);
                if (specialCategory == null)
                {
                    editedParts[index] = part;
                    index++;
                }

            }

            return (editedParts);
        }

        private static void processKeywordGroups(XmlWriter xmlWriter)
        {
            int groupNumber = 1;

            foreach (KeywordGroup group in groups)
            {
                xmlWriter.WriteStartElement("KeywordGroup");
                xmlWriter.WriteAttributeString("uid", "!KeywordGroup!k-" + group.Name.ToLowerInvariant().Replace(' ', '-'));
                xmlWriter.WriteAttributeString("groupName", "k" + groupNumber);

                StringBuilder keywordString = new StringBuilder();
                int keywordNumber = 0;

                foreach (string keyword in group.Keywords)
                {
                    if (keywordString.Length != 0)
                        keywordString.Append(",");
                    keywordString.Append("k" + ((groupNumber * 100) + keywordNumber));

                    keywordNumber++;
                }

                xmlWriter.WriteAttributeString("keywords", keywordString.ToString());
                xmlWriter.WriteEndElement();

                groupNumber++;
            }
        }

        private static void processGuideImages(XmlWriter xmlWriter)
        {
            string imageDirectory = Path.Combine(RunParameters.DataDirectory, "Images") + Path.DirectorySeparatorChar;

            if (!Directory.Exists(imageDirectory))
                return;

            guideImages = new Collection<int>();

            DirectoryInfo directoryInfo = new DirectoryInfo(imageDirectory);

            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                if (fileInfo.Extension.ToLowerInvariant() == ".png")
                {
                    string serviceID = fileInfo.Name.Remove(fileInfo.Name.Length - 4);

                    try 
                    { 
                        guideImages.Add(Int32.Parse(serviceID));

                        xmlWriter.WriteStartElement("GuideImage");
                        xmlWriter.WriteAttributeString("id", "i" + guideImages.Count);
                        xmlWriter.WriteAttributeString("uid", "!Image!SID" + serviceID);
                        xmlWriter.WriteAttributeString("imageUrl", "file://" + fileInfo.FullName);                        
                        xmlWriter.WriteEndElement();
                    }
                    catch (ArgumentException) { }
                    catch (ArithmeticException) { }                    
                }
            }            
        }

        private static void processPeople(XmlWriter xmlWriter)
        {
            people = new Collection<string>();

            foreach (TVStation station in TVStation.StationCollection)
            {
                if (!station.Excluded)
                {
                    foreach (EPGEntry epgEntry in station.EPGCollection)
                    {
                        if (epgEntry.Cast != null)
                        {
                            foreach (string person in epgEntry.Cast)
                                processPerson(xmlWriter, people, person);
                        }

                        if (epgEntry.Directors != null)
                        {
                            foreach (string person in epgEntry.Directors)
                                processPerson(xmlWriter, people, person);
                        }
                    }
                }
            }
        }

        private static void processPerson(XmlWriter xmlWriter, Collection<string> people, string newPerson)
        {
            foreach (string existingPerson in people)
            {
                if (existingPerson == newPerson)
                    return;
            }

            people.Add(newPerson);

            xmlWriter.WriteStartElement("Person");
            xmlWriter.WriteAttributeString("id", "prs" + people.Count);
            xmlWriter.WriteAttributeString("name", newPerson.Trim());
            xmlWriter.WriteAttributeString("uid", "!Person!" + newPerson.Trim());
            xmlWriter.WriteEndElement();
        }

        private static void processSeries(XmlWriter xmlWriter)
        {
            if (RunParameters.Instance.Options.Contains("ALLSERIES"))
                return;

            series = new Collection<string>();

            foreach (TVStation station in TVStation.StationCollection)
            {
                if (!station.Excluded)
                {
                    foreach (EPGEntry epgEntry in station.EPGCollection)
                    {
                        if (epgEntry.EpisodeSystemType != null)
                        {
                            string seriesLink = processEpisode(xmlWriter, series, epgEntry);
                            if (seriesLink != null)
                            {
                                xmlWriter.WriteStartElement("SeriesInfo");
                                xmlWriter.WriteAttributeString("id", "si" + series.Count);
                                xmlWriter.WriteAttributeString("uid", "!Series!" + seriesLink);
                                xmlWriter.WriteAttributeString("title", epgEntry.EventName);
                                xmlWriter.WriteAttributeString("description", epgEntry.EventName);
                                xmlWriter.WriteAttributeString("startAirdate", "2000-01-01T00:00:00");
                                xmlWriter.WriteAttributeString("endAirdate", "1900-01-01T00:00:00");
                                xmlWriter.WriteEndElement();
                            }
                        }
                    }
                }
            }
        }

        private static string processEpisode(XmlWriter xmlWriter, Collection<string> series, EPGEntry epgEntry)
        {
            if (epgEntry.EpisodeSystemType == null || epgEntry.Series == null)
                return(null);

            string newSeriesLink = getSeriesLink(epgEntry);

            foreach (string oldSeriesLink in series)
            {
                if (oldSeriesLink == newSeriesLink)
                    return (null);
            }

            series.Add(newSeriesLink);

            return (newSeriesLink);
        }

        private static void processPrograms(XmlWriter xmlWriter)
        {
            int programNumber = 1;

            foreach (TVStation station in TVStation.StationCollection)
            {
                if (!station.Excluded)
                {
                    foreach (EPGEntry epgEntry in station.EPGCollection)
                    {
                        xmlWriter.WriteStartElement("Program");
                        xmlWriter.WriteAttributeString("id", "prg" + programNumber);
                        xmlWriter.WriteAttributeString("uid", "!Program!" + (epgEntry.OriginalNetworkID + ":" +
                            epgEntry.TransportStreamID + ":" +
                            epgEntry.ServiceID +
                            TimeZoneInfo.ConvertTimeToUtc(epgEntry.StartTime).ToString()).Replace(" ", "").Replace(":", "").Replace("/", "").Replace(".", ""));
                        
                        xmlWriter.WriteAttributeString("title", epgEntry.EventName);
                        xmlWriter.WriteAttributeString("description", epgEntry.ShortDescription);
                        if (epgEntry.EventSubTitle != null)
                            xmlWriter.WriteAttributeString("episodeTitle", epgEntry.EventSubTitle);

                        if (epgEntry.HasAdult)
                            xmlWriter.WriteAttributeString("hasAdult", "1");
                        if (epgEntry.HasGraphicLanguage)
                            xmlWriter.WriteAttributeString("hasGraphicLanguage", "1");
                        if (epgEntry.HasGraphicViolence)
                            xmlWriter.WriteAttributeString("hasGraphicViolence", "1");
                        if (epgEntry.HasNudity)
                            xmlWriter.WriteAttributeString("hasNudity", "1");
                        if (epgEntry.HasStrongSexualContent)
                            xmlWriter.WriteAttributeString("hasStrongSexualContent", "1");

                        if (epgEntry.MpaaParentalRating != null)
                        {
                            switch (epgEntry.MpaaParentalRating)
                            {
                                case "G":
                                    xmlWriter.WriteAttributeString("mpaaRating", "1");
                                    break;
                                case "PG":
                                    xmlWriter.WriteAttributeString("mpaaRating", "2");
                                    break;
                                case "PG13":
                                    xmlWriter.WriteAttributeString("mpaaRating", "3");
                                    break;
                                case "R":
                                    xmlWriter.WriteAttributeString("mpaaRating", "4");
                                    break;
                                case "NC17":
                                    xmlWriter.WriteAttributeString("mpaaRating", "5");
                                    break;
                                case "X":
                                    xmlWriter.WriteAttributeString("mpaaRating", "6");
                                    break;
                                case "NR":
                                    xmlWriter.WriteAttributeString("mpaaRating", "7");
                                    break;
                                case "AO":
                                    xmlWriter.WriteAttributeString("mpaaRating", "8");
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (epgEntry.EventCategory != null)
                            processCategoryKeywords(xmlWriter, epgEntry.EventCategory);                        

                        if (epgEntry.Date != null)
                            xmlWriter.WriteAttributeString("year", epgEntry.Date);

                        if (epgEntry.SeasonNumber != -1)
                            xmlWriter.WriteAttributeString("seasonNumber", epgEntry.SeasonNumber.ToString());
                        if (epgEntry.EpisodeNumber != -1)
                            xmlWriter.WriteAttributeString("episodeNumber", epgEntry.EpisodeNumber.ToString());                        

                        if (epgEntry.PreviousPlayDate != DateTime.MinValue)
                            xmlWriter.WriteAttributeString("originalAirdate", convertDateTimeToString(epgEntry.PreviousPlayDate));

                        if (RunParameters.Instance.Options.Contains("ALLSERIES"))
                            xmlWriter.WriteAttributeString("isSeries", "1");                            
                        else
                            processSeries(xmlWriter, epgEntry);

                        if (epgEntry.StarRating != null)
                        {
                            switch (epgEntry.StarRating)
                            {
                                case "*":
                                    xmlWriter.WriteAttributeString("halfStars", "2");
                                    break;
                                case "*+":
                                    xmlWriter.WriteAttributeString("halfStars", "3");
                                    break;
                                case "**":
                                    xmlWriter.WriteAttributeString("halfStars", "4");
                                    break;
                                case "**+":
                                    xmlWriter.WriteAttributeString("halfStars", "5");
                                    break;
                                case "***":
                                    xmlWriter.WriteAttributeString("halfStars", "6");
                                    break;
                                case "***+":
                                    xmlWriter.WriteAttributeString("halfStars", "7");
                                    break;
                                case "****":
                                    xmlWriter.WriteAttributeString("halfStars", "8");
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (epgEntry.Cast != null && epgEntry.Cast.Count != 0)
                            processCast(xmlWriter, epgEntry.Cast);

                        if (epgEntry.Directors != null && epgEntry.Directors.Count != 0)
                            processDirectors(xmlWriter, epgEntry.Directors);                        
                        
                        xmlWriter.WriteEndElement();

                        programNumber++;
                    }
                }
            }
        }

        private static void processCategoryKeywords(XmlWriter xmlWriter, string category)
        {
            string[] parts = processSpecialCategories(xmlWriter, category);
            if (parts == null)
                return;

            if (parts.Length < 2)
                return;

            StringBuilder keywordString = new StringBuilder();    

            int groupNumber = 1;            

            foreach (KeywordGroup group in groups)
            {
                if (group.Name == parts[0])
                {                    
                    keywordString.Append("k" + groupNumber);

                    int keywordNumber = groupNumber * 100;

                    for (int keywordIndex = 1; keywordIndex < group.Keywords.Count; keywordIndex++)
                    {
                        keywordNumber++;

                        for (int partsIndex = 1; partsIndex < parts.Length; partsIndex++)
                        {
                            if (group.Keywords[keywordIndex] == parts[partsIndex])
                                keywordString.Append(",k" + keywordNumber);
                        }
                    }
                    xmlWriter.WriteAttributeString("keywords", keywordString.ToString());
                    return;
                }
                groupNumber++;
            }            
        }

        private static string[] processSpecialCategories(XmlWriter xmlWriter, string category)
        {
            string[] parts = category.Split(new string[] { "," }, StringSplitOptions.None);

            int specialCategoryCount = 0;

            foreach (string part in parts)
            {
                string specialCategory = getSpecialCategory(part);
                if (specialCategory != null)
                {
                    xmlWriter.WriteAttributeString(specialCategory, "1");
                    specialCategoryCount++;
                }
            }

            if (specialCategoryCount == parts.Length)
                return (null);

            string[] editedParts = new string[parts.Length - specialCategoryCount];
            int index = 0;

            foreach (string part in parts)
            {
                string specialCategory = getSpecialCategory(part);
                if (specialCategory == null)
                {
                    editedParts[index] = part;
                    index++;
                }

            }

            return (editedParts);
        }

        private static string getSpecialCategory(string category)
        {
            switch (category.ToUpperInvariant())
            {
                case "ISMOVIE":
                    return ("isMovie");
                case "ISSPECIAL":
                    return ("isSpecial");
                case "ISSPORTS":
                    return ("isSports");
                case "ISNEWS":
                    return ("isNews");
                case "ISKIDS":
                    return ("isKids");
                case "ISREALITY":
                    return ("isReality");
                default:
                    return (null);
            }
        }

        private static void addSpecialCategory(Collection<string> specialCategories, string newCategory)
        {
            foreach (string oldCategory in specialCategories)
            {
                if (oldCategory == newCategory)
                    return;
            }

            specialCategories.Add(newCategory);
        }

        private static void processCast(XmlWriter xmlWriter, Collection<string> cast)
        {
            if (people == null)
                return;

            int rank = 1;

            foreach (string actor in cast)
            {
                xmlWriter.WriteStartElement("ActorRole");
                xmlWriter.WriteAttributeString("person", "prs" + (people.IndexOf(actor) + 1));
                xmlWriter.WriteAttributeString("rank", rank.ToString());
                xmlWriter.WriteEndElement();

                rank++;
            }
        }

        private static void processDirectors(XmlWriter xmlWriter, Collection<string> directors)
        {
            if (people == null)
                return;

            int rank = 1;

            foreach (string director in directors)
            {
                xmlWriter.WriteStartElement("DirectorRole");
                xmlWriter.WriteAttributeString("person", "prs" + (people.IndexOf(director) + 1));
                xmlWriter.WriteAttributeString("rank", rank.ToString());
                xmlWriter.WriteEndElement();

                rank++;
            }
        }

        private static void processSeries(XmlWriter xmlWriter, EPGEntry epgEntry)
        {
            if (epgEntry.EpisodeSystemType == null || epgEntry.Series == null)
                return;

            string seriesLink = getSeriesLink(epgEntry);

            foreach (string oldSeriesLink in series)
            {
                if (oldSeriesLink == seriesLink)
                {
                    xmlWriter.WriteAttributeString("isSeries", "1");
                    xmlWriter.WriteAttributeString("series", "si" + (series.IndexOf(oldSeriesLink) + 1).ToString());
                    return;
                }
            }
        }

        private static void processAffiliates(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Affiliate");
            xmlWriter.WriteAttributeString("name", importName);
            xmlWriter.WriteAttributeString("uid", "!Affiliate!" + importReference);
            xmlWriter.WriteEndElement();
        }

        private static void processServices(XmlWriter xmlWriter)
        {
            foreach (TVStation station in TVStation.StationCollection)
            {
                if (!station.Excluded)
                {
                    xmlWriter.WriteStartElement("Service");
                    xmlWriter.WriteAttributeString("id", "s" + (TVStation.StationCollection.IndexOf(station) + 1));
                    xmlWriter.WriteAttributeString("uid", "!Service!" + station.OriginalNetworkID + ":" + 
                        station.TransportStreamID + ":" + 
                        station.ServiceID);
                    xmlWriter.WriteAttributeString("name", station.Name);
                    xmlWriter.WriteAttributeString("callSign", station.Name);
                    xmlWriter.WriteAttributeString("affiliate", "!Affiliate!" + importReference);

                    if (guideImages != null)
                    {
                        int imageIndex = 1;

                        foreach (int imageServiceID in guideImages)
                        {
                            if (imageServiceID == station.ServiceID)
                            {
                                xmlWriter.WriteAttributeString("logoImage", "i" + imageIndex.ToString());
                                break;
                            }

                            imageIndex++;
                        }
                    }

                    xmlWriter.WriteEndElement();
                }
            }
        }

        private static void processSchedules(XmlWriter xmlWriter)
        {
            int programNumber = 1;

            foreach (TVStation station in TVStation.StationCollection)
            {
                if (!station.Excluded)
                {
                    xmlWriter.WriteStartElement("ScheduleEntries");
                    xmlWriter.WriteAttributeString("service", "s" + (TVStation.StationCollection.IndexOf(station) + 1));
                    
                    foreach (EPGEntry epgEntry in station.EPGCollection)
                    {
                        xmlWriter.WriteStartElement("ScheduleEntry");
                        xmlWriter.WriteAttributeString("program", "prg" + programNumber);
                        xmlWriter.WriteAttributeString("startTime", convertDateTimeToString(epgEntry.StartTime));
                        xmlWriter.WriteAttributeString("duration", epgEntry.Duration.TotalSeconds.ToString());

                        if (epgEntry.VideoQuality != null && epgEntry.VideoQuality.ToLowerInvariant() == "hdtv")
                            xmlWriter.WriteAttributeString("isHdtv", "true");

                        if (epgEntry.AudioQuality != null)
                        {
                            switch (epgEntry.AudioQuality.ToLowerInvariant())
                            {
                                case "mono":
                                    xmlWriter.WriteAttributeString("audioFormat", "1");
                                    break;
                                case "stereo":
                                    xmlWriter.WriteAttributeString("audioFormat", "2");
                                    break;
                                case "dolby":
                                case "surround":
                                    xmlWriter.WriteAttributeString("audioFormat", "3");
                                    break;
                                case "dolby digital":
                                    xmlWriter.WriteAttributeString("audioFormat", "4");
                                    break;
                                default:
                                    break;
                            }
                        }

                        xmlWriter.WriteEndElement();

                        programNumber++;
                    }

                    xmlWriter.WriteEndElement();
                }
            }
        }

        private static void processLineUps(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Lineup");
            xmlWriter.WriteAttributeString("id", "l1");
            xmlWriter.WriteAttributeString("uid", "!Lineup!" + importName);
            xmlWriter.WriteAttributeString("name", importName);
            xmlWriter.WriteAttributeString("primaryProvider", "!MCLineup!MainLineup");

            xmlWriter.WriteStartElement("channels");

            foreach (TVStation station in TVStation.StationCollection)
            {
                if (!station.Excluded)
                {
                    xmlWriter.WriteStartElement("Channel");
                    if (station.WMCUniqueID != null)
                        xmlWriter.WriteAttributeString("uid", station.WMCUniqueID);
                    else
                        xmlWriter.WriteAttributeString("uid", "!Channel!EPGCollector!" + station.OriginalNetworkID + ":" +
                            station.TransportStreamID + ":" +
                            station.ServiceID);
                    xmlWriter.WriteAttributeString("lineup", "l1");
                    xmlWriter.WriteAttributeString("service", "s" + (TVStation.StationCollection.IndexOf(station) + 1));
                    
                    if (RunParameters.Instance.Options.Contains("AUTOMAPEPG"))
                    {
                        if (station.WMCMatchName != null)
                            xmlWriter.WriteAttributeString("matchName", station.WMCMatchName);
                    }
                    xmlWriter.WriteEndElement();
                }
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        private static string runImportUtility(string fileName)
        {
            string runDirectory = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "ehome");
            Logger.Instance.Write("Running Windows Media Centre import utility LoadMXF from " + runDirectory);
 
            importProcess = new Process();

            importProcess.StartInfo.FileName = Path.Combine(runDirectory, "LoadMXF.exe");
            importProcess.StartInfo.WorkingDirectory = runDirectory + Path.DirectorySeparatorChar;
            importProcess.StartInfo.Arguments = @"-v -i " + '"' + fileName + '"';
            importProcess.StartInfo.UseShellExecute = false;
            importProcess.StartInfo.CreateNoWindow = true;
            importProcess.StartInfo.RedirectStandardOutput = true;
            importProcess.StartInfo.RedirectStandardError = true;
            importProcess.EnableRaisingEvents = true;
            importProcess.OutputDataReceived += new DataReceivedEventHandler(importProcessOutputDataReceived);
            importProcess.ErrorDataReceived += new DataReceivedEventHandler(importProcessErrorDataReceived);
            importProcess.Exited += new EventHandler(importProcessExited);

            try
            {
                importProcess.Start();

                importProcess.BeginOutputReadLine();
                importProcess.BeginErrorReadLine();

                while (!importExited)
                    Thread.Sleep(500);

                Logger.Instance.Write("Windows Media Centre import utility LoadMXF has completed: exit code " + importProcess.ExitCode);
                if (importProcess.ExitCode == 0)
                    return (null);
                else
                    return ("Failed to load Windows Media Centre data: reply code " + importProcess.ExitCode);
            }
            catch (Exception e)
            {
                Logger.Instance.Write("<e> Failed to run the Windows Media Centre import utility LoadMXF");
                Logger.Instance.Write("<e> " + e.Message);
                return ("Failed to load Windows Media Centre data due to an exception");
            }
        }

        private static void importProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            Logger.Instance.Write("LoadMXF message: " + e.Data);
        }

        private static void importProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            Logger.Instance.Write("<e> LoadMXF error: " + e.Data);
        }

        private static void importProcessExited(object sender, EventArgs e)
        {
            importExited = true;
        }

        private static string convertDateTimeToString(DateTime dateTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(dateTime);
            
            return(utcTime.Date.ToString("yyyy-MM-dd") + "T" +
                utcTime.Hour.ToString("00") + ":" +
                utcTime.Minute.ToString("00") + ":" +
                utcTime.Second.ToString("00"));
        }

        private static string getSeriesLink(EPGEntry epgEntry)
        {
            return (epgEntry.OriginalNetworkID + "-" + epgEntry.TransportStreamID + "-" + epgEntry.ServiceID + "-" + epgEntry.EventName + "-" + epgEntry.Series);
        }

        internal class KeywordGroup
        {
            internal string Name { get { return(name); } }
            internal Collection<string> Keywords { get { return (keywords); } }

            private string name;
            private Collection<string> keywords;

            internal KeywordGroup(string name)
            {
                this.name = name;
                keywords = new Collection<string>();
            }
        }
    }
}
