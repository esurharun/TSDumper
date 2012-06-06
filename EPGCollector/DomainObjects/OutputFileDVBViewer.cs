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
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

//susing DVBViewerServer;

namespace DomainObjects
{
    /// <summary>
    /// The DVBViewer output class.
    /// </summary>
    public class OutputFileDVBViewer
    {
        private static int epgCount;

        private static string basicRequest = @"http://127.0.0.1:";
        private static WebRequest webRequest;

        /// <summary>
        /// Create the DVBViewer EPG entries.
        /// </summary>
        /// <returns>Null if the process succeeeded; an error message otherwise.</returns>
        public static string Process()
        {
            if (RunParameters.Instance.Options.Contains("DVBVIEWERIMPORT"))
                return (processDVBViewer());
            else
                return (processRecordingService());
        }

        private static string processDVBViewer()
        {
            Logger.Instance.Write("Importing data to DVBViewer");

           /* DVBViewer dvbViewer;

            try
            {
                dvbViewer = (DVBViewer)Marshal.GetActiveObject("DVBViewerServer.DVBViewer");
            }
            catch (COMException)
            {
                return ("DVBViewer COM server not available");
            }

            IEPGManager epgManager = dvbViewer.EPGManager;
            if (epgManager == null)
                return ("Cannot get IEPGManager interface");

            IEPGAddBuffer epgAddBuffer = epgManager.AddEPG();

            foreach (TVStation tvStation in TVStation.StationCollection)
            {
                if (!tvStation.Excluded && tvStation.EPGCollection.Count != 0)
                    processStationEPG(epgAddBuffer, tvStation);
            }*/

            return (null);
        }

//        private static void processStationEPG(IEPGAddBuffer epgAddBuffer, TVStation tvStation)
//        {
//            foreach (EPGEntry epgEntry in tvStation.EPGCollection)
//            {
//                IEPGItem newItem = epgAddBuffer.NewItem();
//                newItem.SetEPGEventID(epgEntry.ServiceID, epgEntry.TransportStreamID);
//                newItem.EventID = epgEntry.EventID;
//                
//                if (epgEntry.EventName != null)
//                    newItem.Title = epgEntry.EventName;
//                else
//                    newItem.Title = "No Title";
//
//                if (epgEntry.ShortDescription != null)
//                    newItem.Event = epgEntry.ShortDescription;
//                else
//                {
//                    if (epgEntry.EventName != null)
//                        newItem.Event = epgEntry.EventName;
//                    else
//                        newItem.Event = "No Description";
//                }
//                
//                newItem.Event = epgEntry.ShortDescription;
//                newItem.Description = string.Empty;
//                newItem.Time = epgEntry.StartTime;
//                newItem.Duration = new DateTime(1899, 12, 30) + epgEntry.Duration; 
//
//                if (epgEntry.EventCategory != null)
//                {
//                    try
//                    {
//                        newItem.Content = Int32.Parse(epgEntry.EventCategory.Trim());
//                    }
//                    catch (FormatException)
//                    {
//                        newItem.Content = 0;
//                    }
//                    catch (ArithmeticException)
//                    {
//                        newItem.Content = 0;
//                    }
//                }
//                else
//                    newItem.Content = 0;                
//                                          
//                epgAddBuffer.Add(newItem);
//
//                epgCount++;
//            }
//
//            epgAddBuffer.Commit();
//        }

        private static string processRecordingService()
        {
            Logger.Instance.Write("Importing data to DVBViewer Recording Service");

            string port = "8089";

            foreach (string optionID in RunParameters.Instance.Options)
            {
                if (optionID.StartsWith("DVBVIEWERRECSVCPORT-"))
                {
                    string[] parts = optionID.Split(new char[] { '-' });
                    port = parts[1].Trim();
                }
            }

            string destination = basicRequest + port;
            Logger.Instance.Write("Recording Service address is " + destination);

            if (RunParameters.Instance.Options.Contains("DVBVIEWERCLEAR"))
            {
                Logger.Instance.Write("Clearing  DVBViewer EPG data");
                string clearResponse = sendGetRequest(destination + @"/index.html?epg_clear=true");
            }

            Logger.Instance.Write("DVBViewer Recording Service import starting");

            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.Encoding = new UTF8Encoding(false);
            settings.CloseOutput = false;

            MemoryStream memoryStream = new MemoryStream();

            try
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("epg");
                    xmlWriter.WriteAttributeString("Ver", "1");

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

            string sendResponse = sendPostRequest(memoryStream, destination + "/cgi-bin/EPGimport");

            Logger.Instance.Write("DVBViewer Recording Service import finished");

            return (null);
        }

        private static void processStationEPG(XmlWriter xmlWriter, TVStation tvStation)
        {
            foreach (EPGEntry epgEntry in tvStation.EPGCollection)
            {
                xmlWriter.WriteStartElement("programme");

                xmlWriter.WriteAttributeString("start", epgEntry.StartTime.ToString("yyyyMMddHHmmss").Replace(":", ""));
                xmlWriter.WriteAttributeString("stop", (epgEntry.StartTime + epgEntry.Duration).ToString("yyyyMMddHHmmss").Replace(":", ""));
                xmlWriter.WriteAttributeString("channel", ((tvStation.TransportStreamID << 16) + tvStation.ServiceID).ToString());

                xmlWriter.WriteElementString("eventid", epgEntry.EventID.ToString());

                if (epgEntry.EventCategory != null)
                {
                    try
                    {
                        xmlWriter.WriteElementString("content", Int32.Parse(epgEntry.EventCategory.Trim()).ToString());
                    }
                    catch (FormatException) 
                    {
                        xmlWriter.WriteElementString("content", "48");
                    }
                    catch (ArithmeticException) 
                    {
                        xmlWriter.WriteElementString("content", "48");
                    }
                }
                else
                    xmlWriter.WriteElementString("content", "48");
                
                xmlWriter.WriteElementString("charset", "255");

                xmlWriter.WriteStartElement("titles");
                xmlWriter.WriteStartElement("title");
                if (epgEntry.EventName != null)
                    xmlWriter.WriteValue(epgEntry.EventName);
                else
                    xmlWriter.WriteValue("No Title");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("events");
                xmlWriter.WriteStartElement("event");                

                if (epgEntry.ShortDescription != null)
                    xmlWriter.WriteValue(epgEntry.ShortDescription);
                else
                {
                    if (epgEntry.EventName != null)
                        xmlWriter.WriteValue(epgEntry.EventName);
                    else
                        xmlWriter.WriteValue("No Description");
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();

                epgCount++;
            }
        }

        private static string sendGetRequest(string httpRequest)
        {
            webRequest = WebRequest.Create(httpRequest);
            webRequest.ContentType = "text/html";
            webRequest.Timeout = 180000;
            ((HttpWebRequest)webRequest).UserAgent = "HBS";

            HttpWebResponse webResponse = null;

            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException)
            {
                if (webResponse != null)
                    webResponse.Close();
                return (null);
            }

            Stream receiveStream = webResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            StreamReader readStream = new StreamReader(receiveStream, encode);
            string buffer = readStream.ReadToEnd();

            readStream.Close();
            webResponse.Close();

            return (buffer);
        }

        private static string sendPostRequest(MemoryStream memoryStream, string httpRequest)
        {
            WebRequest webRequest = WebRequest.Create(httpRequest);
            webRequest.Method = "POST";
            webRequest.ContentType = "text/html";
            webRequest.ContentLength = memoryStream.Length;
            webRequest.Timeout = 180000;
            ((HttpWebRequest)webRequest).UserAgent = "HBS";

            byte[] dataBuffer = new byte[memoryStream.Length];
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.Read(dataBuffer, 0, (int)memoryStream.Length);

            Stream stream = webRequest.GetRequestStream();
            stream.Write(dataBuffer, 0, dataBuffer.Length);
            stream.Close();
            memoryStream.Close();

            HttpWebResponse webResponse = null;

            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException)
            {
                if (webResponse != null)
                    webResponse.Close();
                return (null);
            }

            Stream receiveStream = webResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            StreamReader readStream = new StreamReader(receiveStream, encode);
            string buffer = readStream.ReadToEnd();

            readStream.Close();
            webResponse.Close();

            return (buffer);
        }
    }
}
