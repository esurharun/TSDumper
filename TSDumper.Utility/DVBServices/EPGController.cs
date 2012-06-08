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
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Xml;

using DomainObjects;
using DirectShow;

namespace DVBServices
{
    /// <summary>
    /// The class that controls the collection of EPG data.
    /// </summary>
    public sealed class EPGController
    {
        /// <summary>
        /// Get an instance of the EPGController class.
        /// </summary>
        public static EPGController Instance
        {
            get
            {
                if (instance == null)
                    instance = new EPGController();
                return (instance);
            }
        }

        private static Collection<IEPGCollector> collectors;

        /// <summary>
        /// The end of collection event.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event parameters.</param>
        public delegate void ProcessCompleteHandler(object sender, EventArgs e);
        /// <summary>
        /// The handler for the end of collection event.
        /// </summary>
        public static event ProcessCompleteHandler ProcessComplete;

        /// <summary>
        /// Returns true if collection is in progress; false otherwise.
        /// </summary>
        public bool Running 
        { 
            get 
            {
                if (epgWorker == null)
                    return (false);
                else                    
                    return (running); 
            } 
        }

        /// <summary>
        /// Return true if all data was collected; false otherwise.
        /// </summary>
        public bool DataComplete
        {
            get
            {
                if (collector != null)
                    return (collector.AllDataProcessed);
                else
                    return (false);
            }
        }

        private static EPGController instance;
        private BackgroundWorker epgWorker;
        private AutoResetEvent resetEvent = new AutoResetEvent(false);
        private bool running;        
        
        private static DateTime lastUpdateTime;

        private IEPGCollector collector;

        private Collection<RecordedProgram> recordedPrograms;

        private EPGController() 
        {
            lastUpdateTime = DateTime.Now;
        }

        /// <summary>
        /// Carry out the end of run processing.
        /// </summary>
        public void FinishRun()
        {
            if (collectors == null)
                return;

            foreach (IEPGCollector collector in collectors)
                collector.FinishRun();

            createDuplicatedChannels();

            loadWMCData();
            createWMCRelatedData();
            createOffsetChannelData();
            checkCollectionRepeats();

            createSatelliteIni();

            if (RunParameters.Instance.DebugIDs.Contains("CATXREF"))
                produceCategoryAnalysis();
        }

        /// <summary>
        /// Start the collection process.
        /// </summary>
        /// <param name="dataProvider">A sample data provider.</param>
        /// <param name="tuningFrequency">The tuning frequency.</param>
        public void Run(ISampleDataProvider dataProvider, TuningFrequency tuningFrequency)
        {
            epgWorker = new BackgroundWorker();
            epgWorker.WorkerSupportsCancellation = true;
            epgWorker.DoWork += new DoWorkEventHandler(epgWorkerDoWork);
            epgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(epgWorkerRunWorkerCompleted);
            epgWorker.RunWorkerAsync(new BackgroundParameters(dataProvider, tuningFrequency));

            running = true;
        }

        /// <summary>
        /// Stop the collection process.
        /// </summary>
        public void Stop()
        {
            Logger.Instance.Write("Stop request received");

            if (running)
            {
                Logger.Instance.Write("Stopping background worker thread");
                epgWorker.CancelAsync();
                bool reply = resetEvent.WaitOne(new TimeSpan(0, 0, 45));
                if (!reply)
                    Logger.Instance.Write("Failed to stop background worker thread");
                running = false;
            }

            Logger.Instance.Write("Stop request processed");
        }

        private void epgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                throw new InvalidOperationException("EPG Controller background worker failed - see inner exception", e.Error);

            if (ProcessComplete != null)
                ProcessComplete(this, new EventArgs());
        }

        private void epgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker == null)
                throw (new ArgumentException("EPG worker thread has been started with an incorrecect sender"));

            BackgroundParameters parameters = e.Argument as BackgroundParameters;
            if (parameters == null)
                throw (new ArgumentException("EPG worker thread has been started with incorrect parameters"));

            if (RunParameters.IsWindows)
                Thread.CurrentThread.Name = "EPG Controller Worker Thread";
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            collector = getCollector(parameters.Frequency);          

            CollectorReply reply = collector.Process(parameters.DataProvider, worker);                
            collector.Stop();
            collector.FinishFrequency();

            Logger.Instance.Write("Sample stats: Sync byte searches: " + parameters.DataProvider.SyncByteSearches +
                " Samples dropped: " + parameters.DataProvider.SamplesDropped +
                " Max sample size: " + parameters.DataProvider.MaximumSampleSize);

            e.Cancel = true;

            Logger.Instance.Write("Background worker thread finished");

            resetEvent.Set();
        }

        private IEPGCollector getCollector(TuningFrequency frequency)
        {
            IEPGCollector collector;

            switch (frequency.CollectionType)
            {
                case CollectionType.EIT:
                    collector = new EITController();
                    Logger.Instance.Write("EPG delivery system is EIT on frequency " + frequency.Frequency);
                    break;
                case CollectionType.MHEG5:
                    collector = new DSMCCController();
                    Logger.Instance.Write("EPG delivery system is MHEG5 on frequency " + frequency.Frequency);
                    break;
                case CollectionType.OpenTV:
                    collector = new OpenTVController();
                    Logger.Instance.Write("EPG delivery system is OpenTV on frequency " + frequency.Frequency);
                    break;
                case CollectionType.MediaHighway1:
                    collector = new MediaHighway1Controller();
                    Logger.Instance.Write("EPG delivery system is MediaHighway1 on frequency " + frequency.Frequency);
                    break;
                case CollectionType.MediaHighway2:
                    collector = new MediaHighway2Controller();
                    Logger.Instance.Write("EPG delivery system is MediaHighway2 on frequency " + frequency.Frequency);
                    break;
                case CollectionType.FreeSat:
                    collector = new FreeSatController();
                    Logger.Instance.Write("EPG delivery system is FreeSat on frequency " + frequency.Frequency);
                    break;
                case CollectionType.PSIP:
                    collector = new AtscPsipController();
                    Logger.Instance.Write("EPG delivery system is ATSC PSIP on frequency " + frequency.Frequency);
                    break;
                case CollectionType.DishNetwork:
                    collector = new DishNetworkController();
                    Logger.Instance.Write("EPG delivery system is Dish Network on frequency " + frequency.Frequency);
                    break;
                case CollectionType.BellTV:
                    collector = new BellTVController();
                    Logger.Instance.Write("EPG delivery system is Bell TV on frequency " + frequency.Frequency);
                    break;
                case CollectionType.SiehfernInfo:
                    collector = new SiehFernInfoController();
                    Logger.Instance.Write("EPG delivery system is SiehFern Info on frequency " + frequency.Frequency);
                    break;                
                default:
                    throw (new InvalidEnumArgumentException("CollectionType"));
            }

            addCollectorToList(collector);

            return (collector);
        }

        private void addCollectorToList(IEPGCollector newCollector)
        {
            if (collectors == null)
                collectors = new Collection<IEPGCollector>();

            foreach (IEPGCollector oldCollector in collectors)
            {
                if (oldCollector.GetType().FullName == newCollector.GetType().FullName)
                    return;
            }

            collectors.Add(newCollector);
        }

        private void createDuplicatedChannels()
        {
            if (!RunParameters.Instance.Options.Contains("DUPLICATESAMECHANNELS"))
                return;

            Logger.Instance.Write("Checking if channel duplication needed");

            Collection<TVStation> sortedStations = new Collection<TVStation>();

            foreach (TVStation unsortedStation in TVStation.StationCollection)
            {
                if (!unsortedStation.Excluded)
                    addStationInSequence(sortedStations, unsortedStation);
            }

            TVStation currentStation = null;

            foreach (TVStation station in sortedStations)
            {
                if (currentStation == null)
                {
                    if (station.EPGCollection.Count != 0)
                        currentStation = station;
                }
                else
                {
                    if (station.Name == currentStation.Name)
                    {
                        /*if (station.EPGCollection.Count == 0)*/
                        {
                            Logger.Instance.Write("Generating data for " + station.Name + " (" + station.FullID + ") from " + currentStation.Name + "(" + currentStation.FullID + ")");
                            station.EPGCollection.Clear();

                            foreach (EPGEntry epgEntry in currentStation.EPGCollection)
                            {
                                EPGEntry newEntry = epgEntry.Clone();
                                newEntry.OriginalNetworkID = station.OriginalNetworkID;
                                newEntry.TransportStreamID = station.TransportStreamID;
                                newEntry.ServiceID = station.ServiceID;
                                station.AddEPGEntry(newEntry);
                            }
                        }
                    }
                    else
                    {
                        if (station.EPGCollection.Count != 0)
                            currentStation = station;
                    }
                }
            }

            Logger.Instance.Write("Channel duplication finished");
        }

        private void addStationInSequence(Collection<TVStation> sortedStations, TVStation newStation)
        {
            foreach (TVStation currentStation in sortedStations)
            {
                int sequence = currentStation.Name.CompareTo(newStation.Name);

                if (sequence > 0)
                {
                    sortedStations.Insert(sortedStations.IndexOf(currentStation), newStation);
                    return;
                }
                else
                {
                    if (sequence == 0)
                    {
                        if (currentStation.EPGCollection.Count == 0)
                        {
                            sortedStations.Insert(sortedStations.IndexOf(currentStation), newStation);
                            return;
                        }
                    }
                }
            }

            sortedStations.Add(newStation);
        }

        private void loadWMCData()
        {
            if (RunParameters.Instance.Options.Contains("WMCIMPORT") || RunParameters.Instance.Options.Contains("CHECKFORREPEATS"))
            {
                bool wmcInstalled = checkWMCInstalled();
                if (wmcInstalled)
                {
                    string exportReply = WMCUtility.Run("export data", "EXPORTDATA");
                    if (exportReply != null)
                        Logger.Instance.Write("<e> " + exportReply);
                    else
                    {
                        string loadReply = loadExportedData();
                        if (loadReply != null)
                            Logger.Instance.Write("<e> " + loadReply);
                    }
                }
                else
                    Logger.Instance.Write("Windows Media Center not installed - no data loaded");
            }
        }

        private bool checkWMCInstalled()
        {
            string mcStorePath = Path.Combine(Environment.GetEnvironmentVariable("windir"), Path.Combine("ehome", "mcstore.dll"));
            Logger.Instance.Write("Checking Windows Media Center installed using " + mcStorePath);

            return (File.Exists(mcStorePath));
        }

        private string loadExportedData()
        {
            XmlReader reader = null;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            WMCChannel channel = null;
            
            try
            {
                reader = XmlReader.Create(Path.Combine(RunParameters.DataDirectory, "WMC Export.xml"), settings);
            }
            catch (IOException)
            {
                return("Failed to open " + Path.Combine(RunParameters.DataDirectory, "WMC Export.xml"));
            }

            try
            {
                while (!reader.EOF)
                {
                    reader.Read();
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "channel":
                                channel = new WMCChannel();
                                channel.Load(reader);
                                break;
                            case "dvbTuningInfo":
                                channel.LoadDVBTuningInfo(reader);
                                break;
                            case "atscTuningInfo":
                                channel.LoadATSCTuningInfo(reader);
                                break;
                            case "recording":
                                WMCRecording recording = new WMCRecording();
                                recording.Load(reader);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (XmlException e)
            {
                return("Failed to load export file: " + e.Message);                
            }
            catch (IOException e)
            {
                Logger.Instance.Write("Failed to load export file: " + e.Message);                
            }

            if (reader != null)
                reader.Close();

            return (null);
        }

        private void createWMCRelatedData()
        {
            if (!RunParameters.Instance.Options.Contains("WMCIMPORT"))
                return;

            if (RunParameters.Instance.DebugIDs.Contains("NOWMC"))
                return;

            Logger.Instance.WriteSeparator("Start Of Windows Media Center Import Specific Processes");
            createWMCChannelInfo();
            Logger.Instance.WriteSeparator("End Of Windows Media Center Import Specific Processes");
        }

        private void createWMCChannelInfo()
        {
            Logger.Instance.Write("Setting the unique ID's and match names for channels");

            Collection<TVStation> manualMatchList = new Collection<TVStation>();

            if (WMCChannel.Channels == null || WMCChannel.Channels.Count == 0)
            {
                Logger.Instance.Write("No merged channels loaded from database");
                return;
            }

            if (RunParameters.Instance.DebugIDs.Contains("LOGMERGEDCHANNELS"))
            {
                foreach (WMCChannel channel in WMCChannel.Channels)
                    Logger.Instance.Write("Merged channel: " + channel.ChannelNumber + " " + channel.CallSign);
            }

            foreach (TVStation station in TVStation.StationCollection)
            {
                if (!station.Excluded && station.EPGCollection.Count != 0 && station.WMCUniqueID == null && station.WMCMatchName == null)
                    processStation(station, manualMatchList);
            }

            foreach (TVStation station in manualMatchList)
                Logger.Instance.Write("Station " + station.Name + " (" + station.FullID + ") match name not set - use WMC Edit Channel to link EPG to channel ");        
        }

        private void processStation(TVStation station, Collection<TVStation> manualMatchList)
        {
            foreach (WMCChannel channel in WMCChannel.Channels)
            {
                foreach (WMCTuningInfo tuningInfo in channel.TuningInfos)
                {
                    WMCDVBTuningInfo dvbTuningInfo = tuningInfo as WMCDVBTuningInfo;
                    if (dvbTuningInfo != null)
                    {
                        if (station.OriginalNetworkID == dvbTuningInfo.ONID &&
                            station.TransportStreamID == dvbTuningInfo.TSID &&
                            station.ServiceID == dvbTuningInfo.SID)
                        {
                            setWMCStationInfo(station, channel, "tuning info");

                            if (manualMatchList.Contains(station))
                                manualMatchList.Remove(station);

                            return;
                        }
                    }
                    else
                    {
                        WMCATSCTuningInfo atscTuningInfo = tuningInfo as WMCATSCTuningInfo;
                        if (atscTuningInfo != null)
                        {
                            if (station.OriginalNetworkID == atscTuningInfo.Frequency &&
                                station.TransportStreamID == atscTuningInfo.MajorChannel &&
                                station.ServiceID == atscTuningInfo.MinorChannel)
                            {
                                setWMCStationInfo(station, channel, "tuning info");
                                
                                if (manualMatchList.Contains(station))
                                    manualMatchList.Remove(station);

                                return;
                            }
                        }
                    }
                }                
            }

            foreach (WMCChannel channel in WMCChannel.Channels)
            {
                if (station.Name == channel.CallSign)
                {
                    setWMCStationInfo(station, channel, "call sign");

                    if (manualMatchList.Contains(station))
                        manualMatchList.Remove(station);

                    return;
                }
            }

            /*foreach (WMCChannel channel in WMCChannel.Channels)
            {
                MatchSpec matchSpec = new MatchSpec(channel.MatchName);

                if (station.OriginalNetworkID == matchSpec.OriginalNetworkID && station.TransportStreamID == matchSpec.TransportStreamID && station.ServiceID == matchSpec.ServiceID)
                {
                    setWMCStationInfo(station, channel, "match name");
                   
                    if (manualMatchList.Contains(station))
                        manualMatchList.Remove(station);

                    return;
                }
            }*/



            if (!manualMatchList.Contains(station))
                manualMatchList.Add(station);
        }

        private void setWMCStationInfo(TVStation station, WMCChannel channel, string matchMethod)
        {
            if (RunParameters.Instance.Options.Contains("AUTOMAPEPG"))
            {
                station.WMCUniqueID = channel.Uid;
                station.WMCMatchName = channel.MatchName;
            }

            string uniqueID;
            string matchName;

            if (station.WMCUniqueID != null)
                uniqueID = station.WMCUniqueID;
            else
                uniqueID = "undefined";

            if (station.WMCMatchName != null)
                matchName = station.WMCMatchName;
            else
                matchName = "undefined";

            Logger.Instance.Write("Station " + station.Name + " (" + station.FullID + ") unique ID " + uniqueID + " match name " + matchName + " set using " + matchMethod);
        }

        private void checkCollectionRepeats()
        {
            if (!RunParameters.Instance.Options.Contains("CHECKFORREPEATS"))
                return;

            Logger.Instance.Write("Checking for repeats");

            checkWMCRecordings();
            checkCollectionData();
        }

        private void checkWMCRecordings()
        {
            if (WMCRecording.Recordings == null || WMCRecording.Recordings.Count == 0)
            {
                Logger.Instance.Write("No recordings loaded from Windows Media Center database");
                return;
            }

            Logger.Instance.Write("Checking Windows Media Center recordings for repeats");            

            recordedPrograms = new Collection<RecordedProgram>();

            foreach (WMCRecording recording in WMCRecording.Recordings)
            {
                RecordedProgram recordedProgram = new RecordedProgram(recording.Title);
                recordedProgram.Description = recording.Description;
                if (recording.StartTime != DateTime.MinValue)
                    recordedProgram.Date = recording.StartTime;
                else
                    recordedProgram.Date = new DateTime(2011, 1, 1);

                recordedPrograms.Add(recordedProgram);
            }

            foreach (RecordedProgram recordedProgram in recordedPrograms)
                Logger.Instance.Write("Recorded program: Title: " + recordedProgram.Title + " Description: " + recordedProgram.Description + " Date: " + recordedProgram.Date);

            Logger.Instance.Write("Matching collection data against existing recordings");
            
            foreach (TVStation station in TVStation.StationCollection)
            {
                if (!station.Excluded)
                {
                    foreach (EPGEntry epgEntry in station.EPGCollection)
                    {
                        if (epgEntry.PreviousPlayDate == DateTime.MinValue)
                        {
                            bool exclude = checkForExcludedProgram(epgEntry.EventName, epgEntry.ShortDescription);
                            if (!exclude)
                            {
                                RecordedProgram recording = findRecordedProgram(epgEntry.EventName, epgEntry.ShortDescription);
                                if (recording != null && recording.Date < epgEntry.StartTime)
                                    epgEntry.PreviousPlayDate = recording.Date;
                            }
                        }
                    }
                }
            }
        }

        private RecordedProgram findRecordedProgram(string title, string description)
        {
            string editedTitle = removePhrases(title);
            string editedDescription = removePhrases(description);

            foreach (RecordedProgram recordedProgram in recordedPrograms)
            {
                string recordedTitle = removePhrases(recordedProgram.Title);
                string recordedDescription = removePhrases(recordedProgram.Description);

                if (recordedTitle == editedTitle && recordedDescription == editedDescription)
                    return (recordedProgram);
            }

            return (null);
        }

        private void checkCollectionData()
        {
            Logger.Instance.Write("Checking collection data for repeats");

            SortedList<EPGEntryKey, EPGEntry> sortedList = new SortedList<EPGEntryKey, EPGEntry>();

            foreach (TVStation station in TVStation.StationCollection)
            {
                if (!station.Excluded)
                {
                    foreach (EPGEntry epgEntry in station.EPGCollection)
                    {
                        if (epgEntry.PreviousPlayDate == DateTime.MinValue)
                        {
                            bool exclude = checkForExcludedProgram(epgEntry.EventName, epgEntry.ShortDescription);
                            if (!exclude)
                            {
                                EPGEntryKey entryKey = new EPGEntryKey(removePhrases(epgEntry.EventName),
                                    removePhrases(epgEntry.ShortDescription),
                                    epgEntry.StartTime);

                                bool added = false;

                                while (!added)
                                {
                                    try
                                    {
                                        sortedList.Add(entryKey, epgEntry);
                                        added = true;
                                    }
                                    catch (ArgumentException)
                                    {
                                        entryKey.SequenceNumber++;
                                    }
                                }
                            }
                            else
                                Logger.Instance.Write("Excluded from repeat checking: " + epgEntry.FullScheduleDescription);
                        }
                    }
                }
            }

            EPGEntryKey lastKey = null;
            int flaggedCount = 0;

            foreach (KeyValuePair<EPGEntryKey, EPGEntry> keyValue in sortedList)
            {
                EPGEntryKey entryKey = keyValue.Key as EPGEntryKey;
                EPGEntry entry = keyValue.Value as EPGEntry;

                if (lastKey == null)
                    lastKey = entryKey;
                else
                {
                    if (entryKey.SequenceNumber != 0)
                    {
                        entry.PreviousPlayDate = lastKey.StartTime;
                        flaggedCount++;
                    }
                    else
                    {
                        if (entryKey.Title == lastKey.Title && entryKey.Description == lastKey.Description)
                        {
                            entry.PreviousPlayDate = lastKey.StartTime;
                            flaggedCount++;
                        }
                        else
                            lastKey = entryKey;
                    }
                }
            }

            Logger.Instance.Write("Number of repeats flagged = " + flaggedCount);
        }

        private bool checkForExcludedProgram(string title, string description)
        {
            if (RepeatExclusion.Exclusions == null)
                return (false);

            foreach (RepeatExclusion exclusion in RepeatExclusion.Exclusions)
            {
                bool titleReply;
                bool descriptionReply;

                if (exclusion.Title.Length != 0)
                    titleReply = checkForMatchingText(title, exclusion.Title);
                else
                    titleReply = true;

                if (exclusion.Description.Length != 0)
                    descriptionReply = checkForMatchingText(description, exclusion.Description);
                else
                    descriptionReply = true;

                if (titleReply && descriptionReply)
                    return (true);
            }

            return (false);
        }

        private bool checkForMatchingText(string programText, string repeatText)
        {
            string lowerCaseProgramText = programText.ToLower();

            int matchMethod;
            string matchString;

            if (repeatText.StartsWith("<"))
            {
                if (repeatText.EndsWith(">"))
                {
                    matchMethod = 1;
                    matchString = (repeatText.Substring(1, repeatText.Length - 2)).ToLower();
                }
                else
                {
                    matchMethod = 2;
                    matchString = (repeatText.Substring(1)).ToLower();
                }
            }
            else
            {
                if (repeatText.EndsWith(">"))
                {
                    matchMethod = 3;
                    matchString = repeatText.Substring(0, repeatText.Length - 1).ToLower();
                }
                else
                {
                    matchMethod = 0;
                    matchString = repeatText.ToLower();
                }
            }

            switch (matchMethod)
            {
                case 0:
                    if (lowerCaseProgramText == matchString)
                        return (true);
                    break;
                case 1:
                    if (lowerCaseProgramText.Contains(matchString))
                        return (true);
                    break;
                case 2:
                    if (lowerCaseProgramText.StartsWith(matchString))
                        return (true);
                    break;
                case 3:
                    if (lowerCaseProgramText.EndsWith(matchString))
                        return (true);
                    break;
                default:
                    break;
            }

            return (false);
        }

        private string removePhrases(string text)
        {
            if (text == null)
                return (null);

            string editedText = text;

            if (RepeatExclusion.PhrasesToIgnore != null)
            {
                foreach (string phrase in RepeatExclusion.PhrasesToIgnore)
                {
                    editedText = editedText.Replace(phrase, "");
                    editedText = editedText.Replace(phrase, "");
                }
            }
            else
            {
                editedText = editedText.Replace("(R)", "");
                editedText = editedText.Replace("(REPEAT)", "");
                editedText = editedText.Replace("(Repeat)", "");
            }

            return (editedText.Trim());
        }

        private void createOffsetChannelData()
        {
            if (TimeOffsetChannel.Channels.Count == 0)
                return;

            Logger.Instance.Write("Processing " + TimeOffsetChannel.Channels.Count + " time shifted channel(s)");

            foreach (TimeOffsetChannel timeOffsetChannel in TimeOffsetChannel.Channels)
                processOffsetChannel(timeOffsetChannel);

            Logger.Instance.Write("Finished processing time shifted channels");
        }

        private void processOffsetChannel(TimeOffsetChannel timeOffsetChannel)
        {
            Logger.Instance.Write("Processing time shift channel " + timeOffsetChannel.SourceChannel.Name + " to " + timeOffsetChannel.DestinationChannel.Name);

            TVStation sourceStation = TVStation.FindStation(timeOffsetChannel.SourceChannel.OriginalNetworkID,
                timeOffsetChannel.SourceChannel.TransportStreamID,
                timeOffsetChannel.SourceChannel.ServiceID);
            if (sourceStation == null)
            {
                Logger.Instance.Write("<e> Source channel not located: " + timeOffsetChannel.SourceChannel.Name);
                return;
            }

            TVStation destinationStation = TVStation.FindStation(timeOffsetChannel.DestinationChannel.OriginalNetworkID,
                timeOffsetChannel.DestinationChannel.TransportStreamID,
                timeOffsetChannel.DestinationChannel.ServiceID);
            if (destinationStation == null)
            {
                Logger.Instance.Write("<e> Destination channel not located: " + timeOffsetChannel.DestinationChannel.Name);
                return;
            }

            if (destinationStation.EPGCollection.Count != 0)
            {
                destinationStation.EPGCollection.Clear();
                Logger.Instance.Write("Destination channel existing EPG data cleared: " + timeOffsetChannel.DestinationChannel.Name);
            }

            foreach (EPGEntry epgEntry in sourceStation.EPGCollection)
            {
                EPGEntry newEntry = epgEntry.Clone();
                newEntry.StartTime += new TimeSpan(timeOffsetChannel.Offset, 0, 0);
                destinationStation.EPGCollection.Add(newEntry);
            }

            Logger.Instance.Write("Created time shifted channel " + timeOffsetChannel.DestinationChannel.Name + " with " +
                destinationStation.EPGCollection.Count + " EPG entries");
        }

        private void produceCategoryAnalysis()
        {
            if (EITSection.CategoryEntries == null || OpenTVChannel.CategoryEntries == null)
                return;

            Collection<CategoryReference> categoryReferences = new Collection<CategoryReference>();

            foreach (CategoryEntry openTVCategoryEntry in OpenTVChannel.CategoryEntries)
            {
                CategoryEntry eitCategoryEntry = findEITCategoryEntry(openTVCategoryEntry);
                if (eitCategoryEntry != null)
                {
                    CategoryReference categoryReference = findCategoryReference(categoryReferences, openTVCategoryEntry.Category);
                    addEITCategoryEntry(categoryReference, eitCategoryEntry.Category, eitCategoryEntry.SubCategory, eitCategoryEntry.EventName);
                }

            }

            Logger.Instance.WriteSeparator("OpenTV/EIT Category Analysis");

            foreach (CategoryReference categoryReference in categoryReferences)
            {
                if (categoryReference.EITCategories.Count == 0)
                    Logger.Instance.Write("OpenTV category " + categoryReference.OpenTVCategory + " has no EIT equivalent");
                else
                {
                    StringBuilder eitString = new StringBuilder();

                    foreach (EITCategory eitCategory in categoryReference.EITCategories)
                    {
                        if (eitString.Length != 0)
                            eitString.Append(", ");
                        eitString.Append(eitCategory.ToString());
                    }

                    Logger.Instance.Write("OpenTV category: " + categoryReference.OpenTVCategory + " EIT equivalent: " + eitString);
                }
            }
        }

        private CategoryEntry findEITCategoryEntry(CategoryEntry openTVCategoryEntry)
        {
            foreach (CategoryEntry categoryEntry in EITSection.CategoryEntries)
            {
                if (categoryEntry.NetworkID == openTVCategoryEntry.NetworkID &&
                    categoryEntry.TransportStreamID == openTVCategoryEntry.TransportStreamID &&
                    categoryEntry.ServiceID == openTVCategoryEntry.ServiceID &&
                    categoryEntry.StartTime == openTVCategoryEntry.StartTime)
                    return (categoryEntry);
            }

            return (null);
        }

        private CategoryReference findCategoryReference(Collection<CategoryReference> categoryReferences, int category)
        {
            foreach (CategoryReference categoryReference in categoryReferences)
            {
                if (categoryReference.OpenTVCategory == category)
                    return (categoryReference);
            }

            CategoryReference newReference = new CategoryReference(category);

            foreach (CategoryReference categoryReference in categoryReferences)
            {
                if (categoryReference.OpenTVCategory > category)
                {
                    categoryReferences.Insert(categoryReferences.IndexOf(categoryReference), newReference);
                    return (newReference);
                }
            }

            categoryReferences.Add(newReference);

            return (newReference);
        }

        private void addEITCategoryEntry(CategoryReference categoryReference, int category, int subCategory, string eventName)
        {
            foreach (EITCategory eitCategory in categoryReference.EITCategories)
            {
                if (eitCategory.Category == category && eitCategory.SubCategory == subCategory)
                    return;
            }

            categoryReference.EITCategories.Add(new EITCategory(category, subCategory, eventName));
        }

        private void createSatelliteIni()
        {
            if (!RunParameters.Instance.DebugIDs.Contains("CREATESATINI"))
                return;

            Collection<SatelliteEntry> satelliteEntries = new Collection<SatelliteEntry>();

            foreach (NetworkInformationSection networkInformationSection in NetworkInformationSection.NetworkInformationSections)
            {
                foreach (TransportStream transportStream in networkInformationSection.TransportStreams)
                {
                    foreach (DescriptorBase descriptor in transportStream.Descriptors)
                    {
                        DVBSatelliteDeliverySystemDescriptor satelliteDescriptor = descriptor as DVBSatelliteDeliverySystemDescriptor;
                        if (satelliteDescriptor != null)
                        {
                            bool added = false;

                            foreach (SatelliteEntry satelliteEntry in satelliteEntries)
                            {
                                if (satelliteEntry.OrbitalPosition == satelliteDescriptor.OrbitalPosition && satelliteEntry.East == satelliteDescriptor.EastFlag)
                                {
                                    satelliteEntry.Add(satelliteDescriptor);
                                    added = true;
                                    break;
                                }
                                
                            }

                            if (!added)
                            {
                                SatelliteEntry newSatelliteEntry = new SatelliteEntry();
                                newSatelliteEntry.Add(satelliteDescriptor);
                                satelliteEntries.Add(newSatelliteEntry);
                            }
                        }
                    }
                }
            }

            foreach (SatelliteEntry satelliteEntry in satelliteEntries)
            {
                string fileName = Path.Combine(RunParameters.DataDirectory, satelliteEntry.OrbitalPosition.ToString().PadLeft(4, '0') + ".ini");

                try
                {
                    if (File.Exists(fileName))
                    {
                        if (File.Exists(fileName + ".bak"))
                        {
                            File.SetAttributes(fileName + ".bak", FileAttributes.Normal);
                            File.Delete(fileName + ".bak");
                        }

                        File.Copy(fileName, fileName + ".bak");
                        File.SetAttributes(fileName + ".bak", FileAttributes.ReadOnly);

                        File.SetAttributes(fileName, FileAttributes.Normal);
                    }

                    FileStream fileStream = new FileStream(fileName, FileMode.Create);
                    StreamWriter streamWriter = new StreamWriter(fileStream);

                    streamWriter.WriteLine(";    [ Created by EPG Collector " + DateTime.Now);
                    streamWriter.WriteLine("");

                    streamWriter.WriteLine("[SATTYPE]");
                    streamWriter.WriteLine("1=" + satelliteEntry.OrbitalPosition.ToString().PadLeft(4, '0'));

                    string orbitalPosition;
                    if (satelliteEntry.East)
                        orbitalPosition = "2=" + (satelliteEntry.OrbitalPosition / 10) + "." + (satelliteEntry.OrbitalPosition % 10) + "E";
                    else
                        orbitalPosition = "2=" + (satelliteEntry.OrbitalPosition / 10) + "." + (satelliteEntry.OrbitalPosition % 10) + "W";
                    streamWriter.WriteLine(orbitalPosition);

                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("[DVB]");
                    streamWriter.WriteLine("0=" + satelliteEntry.SatelliteDescriptors.Count);

                    int lineNumber = 0;

                    foreach (DVBSatelliteDeliverySystemDescriptor satelliteDescriptor in satelliteEntry.SatelliteDescriptors)
                    {
                        lineNumber++;
                        
                        string polarization = getPolarization(satelliteDescriptor.Polarization);
                        string innerFec = getInnerFec(satelliteDescriptor.InnerFEC);
                        string modulationType = getModulationType(satelliteDescriptor.ModulationType);

                        string modulationSystem;
                        if (!satelliteDescriptor.S2Flag)
                            modulationSystem = "DVB-S";
                        else
                            modulationSystem = "S2";

                        streamWriter.WriteLine(lineNumber + "=" + (satelliteDescriptor.Frequency / 100) + "," +
                            polarization + "," +
                            (satelliteDescriptor.SymbolRate / 10) + "," +
                            innerFec + "," +
                            modulationSystem + "," +
                            modulationType);
                    }                     

                    streamWriter.Close();
                    fileStream.Close();
                }
                catch (IOException e)
                {
                    Logger.Instance.Write("<E> Exception creating satellite ini file:");
                    Logger.Instance.Write("<E> " + e.Message);
                    return;
                }
            }
        }

        private string getPolarization(int polarization)
        {
            switch (polarization)
            {
                case 0:
                    return ("H");
                case 1:
                    return ("V");
                case 2:
                    return ("L");
                case 3:
                    return ("R");
                default:
                    return ("H");
            }
        }

        private string getInnerFec(int innerFec)
        {
            switch (innerFec)
            {
                case 1:
                    return ("12");
                case 2:
                    return ("23");
                case 3:
                    return ("34");
                case 4:
                    return ("56");
                case 5:
                    return ("78");
                case 6:
                    return ("89");
                case 7:
                    return ("35");
                case 8:
                    return ("45");
                case 9:
                    return ("91");
                default:
                    return ("12");
            }
        }

        private string getModulationType(int modulationType)
        {
            switch (modulationType)
            {
                case 0:
                    return ("AUTO");
                case 1:
                    return ("QPSK");
                case 2:
                    return ("8PSK");
                case 3:
                    return ("16QAM");
                default:
                    return ("QPSK");
            }
        }

        private class SatelliteEntry
        {
            internal int OrbitalPosition { get { return (orbitalPosition); } }
            internal bool East { get { return (east); } }
            internal Collection<DVBSatelliteDeliverySystemDescriptor> SatelliteDescriptors { get { return (satelliteDescriptors); } }

            private int orbitalPosition;
            private bool east;
            private Collection<DVBSatelliteDeliverySystemDescriptor> satelliteDescriptors;

            internal SatelliteEntry() { }

            internal void Add(DVBSatelliteDeliverySystemDescriptor newDescriptor)
            {
                if (satelliteDescriptors == null)
                {
                    satelliteDescriptors = new Collection<DVBSatelliteDeliverySystemDescriptor>();
                    orbitalPosition = newDescriptor.OrbitalPosition;
                    east = newDescriptor.EastFlag;
                }

                foreach (DVBSatelliteDeliverySystemDescriptor oldDescriptor in satelliteDescriptors)
                {
                    if (oldDescriptor.Frequency == newDescriptor.Frequency && oldDescriptor.Polarization == newDescriptor.Polarization)
                        return;

                    if (oldDescriptor.Frequency > newDescriptor.Frequency)
                    {
                        satelliteDescriptors.Insert(satelliteDescriptors.IndexOf(oldDescriptor), newDescriptor);
                        return;
                    }
                }

                satelliteDescriptors.Add(newDescriptor);
            }
        }

        private class BackgroundParameters
        {
            internal ISampleDataProvider DataProvider { get { return (dataProvider); } }
            internal TuningFrequency Frequency { get { return (tuningFrequency); } }

            private ISampleDataProvider dataProvider;
            private TuningFrequency tuningFrequency;

            private BackgroundParameters() { }

            internal BackgroundParameters(ISampleDataProvider dataProvider, TuningFrequency tuningFrequency)
            {
                this.dataProvider = dataProvider;
                this.tuningFrequency = tuningFrequency;
            }
        }

        private class CategoryReference
        {
            internal int OpenTVCategory { get { return (openTVCategory); } }

            internal Collection<EITCategory> EITCategories 
            { 
                get 
                {
                    if (eitCategories == null)
                        eitCategories = new Collection<EITCategory>();

                    return(eitCategories); 
                } 
            }            

            private int openTVCategory;
            private Collection<EITCategory> eitCategories;

            private CategoryReference() { }

            internal CategoryReference(int openTVCategory)
            {
                this.openTVCategory = openTVCategory;
            }
        }

        private class EITCategory
        {
            internal int Category { get { return (category); } }
            internal int SubCategory { get { return (subCategory); } }            

            private int category;
            private int subCategory;
            private string eventName;

            private EITCategory() { }

            internal EITCategory(int category, int subCategory, string eventName)
            {
                this.category = category;
                this.subCategory = subCategory;
                this.eventName = eventName;
            }

            public override string ToString()
            {
                if (eventName != null)
                    return (category + "/" + subCategory + " " + eventName);                
                else
                    return (category + "/" + subCategory + " No Event Name"); 
            }
        }

        private class RecordedProgram
        {
            internal string Title { get { return (title); } }

            internal string Description
            {
                get { return (description); }
                set { description = value; }
            }

            internal DateTime Date
            {
                get { return (date); }
                set { date = value; }
            }

            private string title;
            private string description;
            private DateTime date;

            private RecordedProgram() { }

            internal RecordedProgram(string title)
            {
                this.title = title;
            }
        }

        private class MatchSpec
        {
            internal int OriginalNetworkID { get { return(originalNetworkID); } }
            internal int TransportStreamID { get { return (transportStreamID); } }
            internal int ServiceID { get { return (serviceID); } }

            private int originalNetworkID = -1;
            private int transportStreamID = -1;
            private int serviceID = -1;

            private MatchSpec() { }

            internal MatchSpec(string matchName)
            {
                if (matchName.StartsWith("!Channel!EPGCollector!"))
                    setFromCollector(matchName);
                else
                    setFromMatchName(matchName);
            }

            private void setFromCollector(string matchName)
            {
                string[] majorParts = matchName.Split(new char[] { '!' });
                if (majorParts.Length != 4)
                    return;

                string[] minorParts = majorParts[3].Split(new char[] { ':' });
                if (minorParts.Length != 3)
                    return;

                try
                {
                    originalNetworkID = Int32.Parse(minorParts[0]);
                    transportStreamID = Int32.Parse(minorParts[1]);
                    serviceID = Int32.Parse(minorParts[2]);   
                }
                catch (FormatException) { }
                catch (ArithmeticException) { }    
            }

            private void setFromMatchName(string matchName)
            {
                string[] matchNameParts = matchName.Split(new char[] { ':' });

                if (matchNameParts.Length > 1)
                {
                    originalNetworkID = -1;
                    transportStreamID = -1;
                    serviceID = -1;

                    try
                    {
                        switch (matchNameParts[0])
                        {
                            case "DVBS":
                                if (matchNameParts.Length == 6)
                                {
                                    originalNetworkID = Int32.Parse(matchNameParts[3]);
                                    transportStreamID = Int32.Parse(matchNameParts[4]);
                                    serviceID = Int32.Parse(matchNameParts[5]);                                    
                                }
                                break;
                            case "DVBT":
                                if (matchNameParts.Length == 4)
                                {
                                    originalNetworkID = Int32.Parse(matchNameParts[1]);
                                    transportStreamID = Int32.Parse(matchNameParts[2]);
                                    if (!matchNameParts[3].Contains("|"))
                                        serviceID = Int32.Parse(matchNameParts[3]);
                                    else
                                    {
                                        int index = matchNameParts[3].IndexOf("|");
                                        serviceID = Int32.Parse(matchNameParts[3].Substring(0, index));
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    catch (FormatException) { }
                    catch (ArithmeticException) { }                    
                }
            }
        }

        private class EPGEntryKey : IComparable
        {
            internal string Title { get { return (title); } }
            internal string Description { get { return (description); } }
            internal DateTime StartTime { get { return (startTime); } }

            internal int SequenceNumber
            {
                get { return (sequenceNumber); }
                set { sequenceNumber = value; }
            }

            private string title;
            private string description;
            private DateTime startTime;
            private int sequenceNumber;

            private EPGEntryKey() { }

            internal EPGEntryKey(string title, string description, DateTime startTime)
            {
                this.title = title;
                this.description = description;
                this.startTime = startTime;
            }

            public int CompareTo(object other)
            {
                EPGEntryKey otherKey = other as EPGEntryKey;
                if (otherKey == null)
                    return (1);

                switch (string.Compare(title, otherKey.Title, StringComparison.CurrentCulture))
                {
                    case -1:
                        return(-1);
                    case 1:
                        return(1);
                    default:
                        switch (string.Compare(description, otherKey.description, StringComparison.CurrentCulture))
                        {
                            case -1:
                                return(-1);
                            case 1:
                                return(1);
                            default:
                                switch (DateTime.Compare(startTime, otherKey.StartTime))
                                {
                                    case -1:
                                        return(-1);
                                    case 1:
                                        return(1);
                                    default:
                                        if (sequenceNumber == otherKey.SequenceNumber)
                                            return(0);
                                        else
                                        {
                                            if (sequenceNumber < otherKey.SequenceNumber)
                                                return(-1);
                                            else
                                                return(1);
                                        }
                                }
                        }
                }
            }
        }

        private class DatabaseVersionNumber
        {
            internal int MajorVersion { get { return (majorVersion); } }
            internal int MinorVersion { get { return (minorVersion); } }

            private int majorVersion = -1;
            private int minorVersion = -1;

            private DatabaseVersionNumber() { }

            internal DatabaseVersionNumber(string fileName)
            {
                string identifier = "mcepg";

                string[] majorParts = fileName.Split(new char[] { '-' });
                if (majorParts.Length != 2)
                    return;

                int index1 = majorParts[0].LastIndexOf(identifier);
                if (index1 == -1)
                    return;

                majorVersion = Int32.Parse(majorParts[0].Substring(index1 + identifier.Length));

                string[] minorParts = majorParts[1].Split(new char[] { '.' });
                if (minorParts.Length != 2)
                    return;

                minorVersion = Int32.Parse(minorParts[0]);
            }
        }
    }
}
