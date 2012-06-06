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
using System.Threading;
using System.ComponentModel;

using DirectShow;
using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that controls the acquisition and processing of SiehFern Info data.
    /// </summary>
    public class SiehFernInfoController : ControllerBase
    {
        /// <summary>
        /// Return true if the EIT data is complete; false otherwise.
        /// </summary>
        public override bool AllDataProcessed { get { return (guideDone); } }

        private TSStreamReader guideReader;

        private bool guideDone = false;

        private TVStation station;
        private DateTime startDate;
        private EPGEntry epgEntry;
        private DateTime lastStartTime;

        /// <summary>
        /// Initialize a new instance of the SiehFernInfoController class.
        /// </summary>
        public SiehFernInfoController() { }

        /// <summary>
        /// Stop acquiring and processing EIT data.
        /// </summary>
        public override void Stop()
        {
            Logger.Instance.Write("Stopping section readers");

            if (guideReader != null)
                guideReader.Stop();

            Logger.Instance.Write("Stopped section readers");
        }

        /// <summary>
        /// Acquire and process Siehfern Info data.
        /// </summary>
        /// <param name="dataProvider">A sample data provider.</param>
        /// <param name="worker">The background worker that is running this collection.</param>
        /// <returns>A CollectorReply code.</returns>
        public override CollectorReply Process(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            /*getChannelSections(dataProvider, worker);*/
            getEPGSections(dataProvider, worker);

            return (CollectorReply.OK);
        }

        private void getChannelSections(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            Logger.Instance.Write("Collecting Channel data", false, true);

            dataProvider.ChangePidMapping(new int[] { 0x711 });

            guideReader = new TSStreamReader(0x3e, 50000, dataProvider.BufferAddress);
            guideReader.Run();

            int lastCount = 0;
            int repeats = 0;

            while (!guideDone)
            {
                if (worker.CancellationPending)
                    return;

                Thread.Sleep(2000);
                Logger.Instance.Write(".", false, false);

                Collection<Mpeg2Section> sections = new Collection<Mpeg2Section>();

                guideReader.Lock("LoadMessages");
                if (guideReader.Sections.Count != 0)
                {
                    foreach (Mpeg2Section section in guideReader.Sections)
                        sections.Add(section);
                    guideReader.Sections.Clear();
                }
                guideReader.Release("LoadMessages");

                if (sections.Count != 0)
                    processChannelSections(sections);

                if (SiehFernInfoChannelSection.Sections.Count == lastCount)
                {
                    repeats++;
                    guideDone = (repeats == RunParameters.Instance.Repeats);
                }
                else
                    repeats = 0;

                lastCount = SiehFernInfoChannelSection.Sections.Count;
            }

            Logger.Instance.Write("", true, false);
            Logger.Instance.Write("Stopping reader");
            guideReader.Stop();

            Logger.Instance.Write("Section count: " + SiehFernInfoChannelSection.Sections.Count + " buffer space used: " + dataProvider.BufferSpaceUsed);
        }

        private void getEPGSections(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            Logger.Instance.Write("Collecting EPG data", false, true);

            dataProvider.ChangePidMapping(new int[] { 0x711 });

            guideReader = new TSStreamReader(0x3e, 50000, dataProvider.BufferAddress);
            guideReader.Run();

            int lastCount = 0;
            int repeats = 0;
            guideDone = false;

            while (!guideDone)
            {
                if (worker.CancellationPending)
                    return;

                Thread.Sleep(2000);
                Logger.Instance.Write(".", false, false);

                Collection<Mpeg2Section> sections = new Collection<Mpeg2Section>();

                guideReader.Lock("LoadMessages");
                if (guideReader.Sections.Count != 0)
                {
                    foreach (Mpeg2Section section in guideReader.Sections)
                        sections.Add(section);
                    guideReader.Sections.Clear();
                }
                guideReader.Release("LoadMessages");

                if (sections.Count != 0)
                    processEPGSections(sections);

                if (SiehFernInfoEPGSection.Sections.Count == lastCount)
                {
                    repeats++;
                    guideDone = (repeats == RunParameters.Instance.Repeats);
                }
                else
                    repeats = 0;

                lastCount = SiehFernInfoEPGSection.Sections.Count;
            }

            Logger.Instance.Write("", true, false);
            Logger.Instance.Write("Stopping reader");
            guideReader.Stop();

            Logger.Instance.Write("Section count: " + SiehFernInfoEPGSection.Sections.Count + " buffer space used: " + dataProvider.BufferSpaceUsed);
        }

        private void processChannelSections(Collection<Mpeg2Section> sections)
        {
            foreach (Mpeg2Section section in sections)
            {
                if (RunParameters.Instance.DebugIDs.Contains("SIEHFERNBLOCKS"))
                    Logger.Instance.Dump("Siehfern Block", section.Data, section.Data.Length);

                try
                {
                    Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();                    
                    mpeg2Header.Process(section.Data);
                    if (mpeg2Header.Current)
                    {
                        if (mpeg2Header.TableIDExtension == 0x1502)
                        {
                            SiehFernInfoChannelSection channelSection = new SiehFernInfoChannelSection();
                            channelSection.Process(section.Data, mpeg2Header);
                            channelSection.LogMessage();

                            bool added = SiehFernInfoChannelSection.AddSection(channelSection);
                            if (added)
                            {
                                if (RunParameters.Instance.DebugIDs.Contains("SIEHFERNCHANNELBLOCKS"))
                                    Logger.Instance.Dump("Siehfern Info Block Type 0x" + mpeg2Header.TableIDExtension.ToString("X"), section.Data, section.Data.Length);
                            }
                        }                        
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Logger.Instance.Write("<e> Error processing SiehFern Info Channel section: " + e.Message);
                }
            }
        }

        private void processEPGSections(Collection<Mpeg2Section> sections)
        {
            foreach (Mpeg2Section section in sections)
            {
                try
                {
                    Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();
                    mpeg2Header.Process(section.Data);
                    if (mpeg2Header.Current)
                    {
                        if (mpeg2Header.TableIDExtension == 0x702)
                        {                            
                            SiehFernInfoEPGSection epgSection = new SiehFernInfoEPGSection();
                            epgSection.Process(section.Data, mpeg2Header);
                            epgSection.LogMessage();

                            bool added = SiehFernInfoEPGSection.AddSection(epgSection);
                            if (added)
                            {
                                if (RunParameters.Instance.DebugIDs.Contains("SIEHFERNEPGBLOCKS"))
                                    Logger.Instance.Dump("Siehfern Info Block Type 0x" + mpeg2Header.TableIDExtension.ToString("X"), section.Data, section.Data.Length);
                            }
                        }
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Logger.Instance.Write("<e> Error processing SiehFern Info EPG section: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Create the EPG entries.
        /// </summary>
        public override void FinishFrequency()
        {
            Logger titleLogger = null;
            Logger descriptionLogger = null;

            if (RunParameters.Instance.DebugIDs.Contains("LOGTITLES"))
                titleLogger = new Logger("EPG Titles.log");
            if (RunParameters.Instance.DebugIDs.Contains("LOGDESCRIPTIONS"))
                descriptionLogger = new Logger("EPG Descriptions.log");

            int byteArraySize = 0;

            foreach (SiehFernInfoEPGSection epgSection in SiehFernInfoEPGSection.Sections)
                byteArraySize += epgSection.Data.Length;

            byte[] epgBuffer = new byte[byteArraySize];

            int putIndex = 0;

            foreach (SiehFernInfoEPGSection epgSection in SiehFernInfoEPGSection.Sections)
            {
                epgSection.Data.CopyTo(epgBuffer, putIndex);
                putIndex += epgSection.Data.Length;
            }

            int getIndex = 0;

            while (epgBuffer[getIndex] != '@')
                getIndex++;

            TVStation currentStation = null;

            while (getIndex < epgBuffer.Length)
            {
                byte[] epgLine = getLine(epgBuffer, getIndex);
                getIndex += epgLine.Length;

                for (int scan = 0; scan < epgLine.Length; scan++)
                {
                    if (epgLine[scan] == 0x0d || epgLine[scan] == 0x0a)
                        epgLine[scan] = (byte)'|';
                    else
                    {
                        if (epgLine[scan] == 0x8a)
                            epgLine[scan] = (byte)' ';
                    }
                }

                string epgText = Utils.GetString(epgLine, 0, epgLine.Length);

                switch (epgText.Substring(0, 3))
                {
                    case "@P:":
                        currentStation = processStation(epgText);
                        TVStation.StationCollection.Add(currentStation);
                        break;
                    case "@E:":
                        processProgramTitle(epgText, titleLogger);
                        break;
                    case "@S:":
                        processProgramDescription(epgText, descriptionLogger);
                        break;
                    default:
                        break;
                }
            }            
        }

        private byte[] getLine(byte[] epgBuffer, int index)
        {
            int length = 0;

            for (int lengthIndex = index + 1; lengthIndex < epgBuffer.Length && epgBuffer[lengthIndex] != '@'; lengthIndex++)
                length++;            

            byte[] outputBytes = new byte[length + 1];

            outputBytes[0] = (byte)'@';
            int outputIndex = 1;

            for (int getIndex = index + 1; getIndex < epgBuffer.Length && epgBuffer[getIndex] != '@'; getIndex++)
            {
                outputBytes[outputIndex] = epgBuffer[getIndex];
                outputIndex++;
            }
            
            return (outputBytes);
        }

        private TVStation processStation(string epgText)
        {
            string[] parts = epgText.Substring(3).Split(new char[] { '(' } );
            string[] stationDefinition = parts[1].Split(new char[] { ')' } );
            string[] stationParts = stationDefinition[0].Split(new char[] { ',' } );

            station = new TVStation(parts[0]);
            station.TransportStreamID = Int32.Parse(stationParts[1].Trim());
            station.ServiceID = Int32.Parse(stationParts[2].Trim());

            string[] dateParts = stationDefinition[1].Split(new char[] { ' ' } );
            string[] dayMonthYear = dateParts[1].Trim().Split(new char[] { '.' } );
            int day = Int32.Parse(dayMonthYear[0]);
            int month = Int32.Parse(dayMonthYear[1]);
            int year = Int32.Parse(dayMonthYear[2]);

            startDate = new DateTime(year, month, day);
            lastStartTime = startDate;

            return(station);
        }

        private void processProgramTitle(string epgText, Logger titleLogger)
        {
            if (station == null)
                return;

            epgEntry = new EPGEntry();
            epgEntry.OriginalNetworkID = station.OriginalNetworkID;
            epgEntry.TransportStreamID = station.TransportStreamID;
            epgEntry.ServiceID = station.ServiceID;
            epgEntry.EPGSource = EPGSource.SiehfernInfo;

            string time = epgText.Substring(3, 5);
            int hours = Int32.Parse(time.Substring(0, 2));
            int minutes = Int32.Parse(time.Substring(3, 2));

            TimeSpan startTime = new TimeSpan(hours, minutes, 0);
            if (startDate + startTime < lastStartTime)
                startTime = startTime.Add(new TimeSpan(24, 0, 0));

            epgEntry.StartTime = Utils.RoundTime(startDate + startTime);

            int separatorIndex = epgText.IndexOf('|');
            if (separatorIndex == -1)
                epgEntry.EventName = epgText.Substring(9);
            else
                epgEntry.EventName = epgText.Substring(9, separatorIndex - 9);

            station.EPGCollection.Add(epgEntry);
            lastStartTime = epgEntry.StartTime;

            if (station.EPGCollection.Count > 1)
            {
                int count = station.EPGCollection.Count;
                if (station.EPGCollection[count - 2].Duration.TotalSeconds == 0)
                    station.EPGCollection[count - 2].Duration = Utils.RoundTime(station.EPGCollection[count - 1].StartTime - station.EPGCollection[count - 2].StartTime);
            }

            if (titleLogger != null)
            {
                titleLogger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +
                    epgEntry.StartTime.ToShortDateString() + " " +
                    epgEntry.StartTime.ToString("HH:mm") + " - " +
                    epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                    epgEntry.EventName + " " +
                    epgText);
            }
        }

        private void processProgramDescription(string epgText, Logger descriptionLogger)
        {
            if (epgEntry == null)
                return;

            int timeIndex = epgText.LastIndexOf(',');
            if (timeIndex == -1)
                return;

            string description = epgText.Substring(3, timeIndex - 3).Trim();
            description = description.Replace(@"\", "");
            description = description.Replace("||", "");
            epgEntry.ShortDescription = description;

            string[] timeParts = epgText.Substring(timeIndex + 1).Replace(@"\", "").Split(new char[] { ' ' } );

            try
            {
                int totalMinutes = Int32.Parse(timeParts[1].Trim());
                int hours = totalMinutes / 60;
                int minutes = totalMinutes % 60;
                epgEntry.Duration = new TimeSpan(hours, minutes, 0);                
            }
            catch (FormatException) { }

            if (descriptionLogger != null)
            {
                descriptionLogger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +
                    epgEntry.StartTime.ToShortDateString() + " " +
                    epgEntry.StartTime.ToString("HH:mm") + " - " +
                    epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                    epgEntry.ShortDescription + " " +
                    epgText);
            }
        }
    }
}
