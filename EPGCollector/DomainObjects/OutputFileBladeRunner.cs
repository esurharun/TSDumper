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
    internal sealed class OutputFileBladeRunner
    {
        private static string actualFileName;

        private OutputFileBladeRunner() { }

        internal static void Process(string fileName)
        {
            actualFileName = Path.Combine(Path.GetDirectoryName(fileName), "ChannelInfo.xml");

            try
            {
                Logger.Instance.Write("Deleting any existing version of BladeRunner channel file");
                File.SetAttributes(actualFileName, FileAttributes.Normal);
                File.Delete(actualFileName);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("File delete exception: " + e.Message);
            }

            Logger.Instance.Write("Creating BladeRunner channel file: " + actualFileName);

            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.Encoding = new UTF8Encoding(false);
            settings.CloseOutput = true;

            int channelNumber = 0;

            using (XmlWriter xmlWriter = XmlWriter.Create(actualFileName, settings))
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
                            if (tvStation.ChannelID != null)
                                xmlWriter.WriteElementString("channelID", tvStation.ChannelID);
                            {
                                if (tvStation.LogicalChannelNumber != -1)
                                    xmlWriter.WriteElementString("channelID", tvStation.LogicalChannelNumber.ToString());
                                else
                                    xmlWriter.WriteElementString("channelID", tvStation.ServiceID.ToString());
                            }
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
    }
}
