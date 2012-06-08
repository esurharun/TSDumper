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
using System.Threading;
using System.ComponentModel;
using System.Text;
using System.IO;

using DirectShow;
using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The base class for the collector controllers.
    /// </summary>
    public abstract class ControllerBase : IEPGCollector
    {
        /// <summary>
        /// Get the bouquet reader.
        /// </summary>
        protected TSStreamReader BouquetReader { get { return (bouquetReader); } }

        /// <summary>
        /// Get the time offset reader.
        /// </summary>
        protected TSStreamReader TimeOffsetReader { get { return (timeOffsetReader); } }

        /// <summary>
        /// Return true if all data has been processed; false otherwise.
        /// </summary>
        public abstract bool AllDataProcessed { get; }

        private TSStreamReader bouquetReader;
        private TSStreamReader timeOffsetReader;

        private static bool omitTimeZoneSections;

        private static bool stationCacheLoaded;

        /// <summary>
        /// Initialise a new instance of the ControllerBase class.
        /// </summary>
        public ControllerBase() { }

        /// <summary>
        /// Collect EPG data.
        /// </summary>
        /// <param name="dataProvider">The provider for the data samples.</param>
        /// <param name="worker">The BackgroundWorker instance running the collection.</param>        
        /// <returns>A CollectorReply code.</returns>
        public abstract CollectorReply Process(ISampleDataProvider dataProvider, BackgroundWorker worker);

        /// <summary>
        /// Get the stations using the standard SDT pid.
        /// </summary>
        /// <param name="dataProvider">The sample data provider.</param>
        /// <param name="worker">The BackgroundWorker instance running the collection.</param>
        protected void GetStationData(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            GetStationData(dataProvider, worker, new int[] { BDAGraph.SdtPid });
        }

        /// <summary>
        /// Get the stations using specified pid's.
        /// </summary>
        /// <param name="dataProvider">The sample data provider.</param>
        /// <param name="worker">The BackgroundWorker instance running the collection.</param>
        /// <param name="pids">An array of pid's to search.</param>
        protected void GetStationData(ISampleDataProvider dataProvider, BackgroundWorker worker, int[] pids)
        {
            Collection<TVStation> stations;

            if (!RunParameters.Instance.Options.Contains("USESTOREDSTATIONINFO"))
            {
                FrequencyScanner frequencyScanner = new FrequencyScanner(dataProvider, pids, true, worker);
                frequencyScanner.UseActualFrequency = false;
                stations = frequencyScanner.FindTVStations();
                if (worker.CancellationPending)
                    return;
            }
            else
            {
                if (!stationCacheLoaded)
                {
                    stations = TVStation.Load(Path.Combine(RunParameters.DataDirectory, "Station Cache.xml"));
                    if (stations == null)
                        return;

                    setMHEG5Pid(dataProvider, stations);
                    stationCacheLoaded = true;
                }
                else
                {
                    setMHEG5Pid(dataProvider, TVStation.StationCollection);
                    return;
                }
            }

            foreach (TVStation tvStation in stations)
            {
                bool include = checkChannelFilters(tvStation);

                if (include)
                {
                    TVStation existingStation = TVStation.FindStation(tvStation.OriginalNetworkID, tvStation.TransportStreamID, tvStation.ServiceID);
                    if (existingStation == null)
                    {
                        tvStation.CollectionType = dataProvider.Frequency.CollectionType;
                        bool added = TVStation.AddStation(tvStation);
                        if (added)
                            Logger.Instance.Write("Included station: " + getStationDescription(tvStation));
                    }
                    else
                    {
                        if (!existingStation.Excluded)
                        {
                            existingStation.Update(tvStation);
                            Logger.Instance.Write("Included station: " + getStationDescription(tvStation));
                        }
                        else
                            Logger.Instance.Write("Excluded station: " + getStationDescription(tvStation));
                    }
                }
                else
                    Logger.Instance.Write("Excluded station: " + getStationDescription(tvStation));

            }

            Logger.Instance.Write("Station count now: " + TVStation.StationCollection.Count);
        }

        private bool checkChannelFilters(TVStation station)
        {
            if (RunParameters.Instance.MaxService != -1 && station.ServiceID > RunParameters.Instance.MaxService)
                return (false);

            if (ChannelFilterEntry.ChannelFilters == null)
                return (true);

            ChannelFilterEntry filterEntry = ChannelFilterEntry.FindEntry(station.OriginalNetworkID, station.TransportStreamID, station.ServiceID);
            if (filterEntry != null)
                return (true);

            filterEntry = ChannelFilterEntry.FindEntry(station.OriginalNetworkID, station.TransportStreamID);
            if (filterEntry != null)
                return (true);

            filterEntry = ChannelFilterEntry.FindEntry(station.OriginalNetworkID);
            if (filterEntry != null)
                return (true);  

            return (false);
        }

        private void setMHEG5Pid(ISampleDataProvider dataProvider, Collection<TVStation> stations)
        {
            if (dataProvider.Frequency.CollectionType != CollectionType.MHEG5)
                return;

            foreach (TVStation station in stations)
            {
                if (dataProvider.Frequency.DSMCCPid == 0)
                    dataProvider.Frequency.DSMCCPid = station.DSMCCPID;

                if (!station.Excluded && station.DSMCCPID != 0 && station.Frequency == dataProvider.Frequency.Frequency)
                    dataProvider.Frequency.DSMCCPid = station.DSMCCPID;
            }
        }

        private string getStationDescription(TVStation station)
        {
            StringBuilder description = new StringBuilder(station.FixedLengthName);

            description.Append(" (");

            description.Append(station.FullID);
            description.Append(" Type: " + station.ServiceType);
            description.Append(" " + (station.Encrypted ? "Encrypted" : "Clear"));

            string epg;

            if (station.NextFollowingAvailable)
            {
                if (station.ScheduleAvailable)
                    epg = "NN&S";
                else
                    epg = "NN";
            }
            else
            {
                if (station.ScheduleAvailable)
                    epg = "S";
                else
                    epg = "None";
            }

            description.Append(" EPG: " + epg);
            description.Append(")");
            
            return (description.ToString());
        }

        /// <summary>
        /// Get the bouquet data from the standard PID.
        /// </summary>
        /// <param name="dataProvider">The sample data provider.</param>
        /// <param name="worker">The BackgroundWorker instance running the collection.</param>
        protected void GetBouquetSections(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            GetBouquetSections(dataProvider, worker, new int[] { 0x11 } );
        }

        /// <summary>
        /// Get the bouquet data.
        /// </summary>
        /// <param name="dataProvider">The sample data provider.</param>
        /// <param name="worker">The BackgroundWorker instance running the collection.</param>
        /// <param name="pids">The PID's to scan.</param>
        protected void GetBouquetSections(ISampleDataProvider dataProvider, BackgroundWorker worker, int[] pids)
        {
            BouquetAssociationSection.BouquetAssociationSections.Clear();

            dataProvider.ChangePidMapping(pids);

            Logger.Instance.Write("Collecting channel data", false, true);

            bouquetReader = new TSStreamReader(0x4a, 2000, dataProvider.BufferAddress);            
            Channel.Channels.Clear();

            bouquetReader.Run();

            int lastCount = 0;
            int repeats = 0;

            bool bouquetSectionsDone = false;

            while (!bouquetSectionsDone)
            {
                if (worker.CancellationPending)
                    return;

                Thread.Sleep(2000);
                Logger.Instance.Write(".", false, false);

                Collection<Mpeg2Section> sections = new Collection<Mpeg2Section>();

                bouquetReader.Lock("ProcessOpenTVSections");
                if (bouquetReader.Sections.Count != 0)
                {
                    foreach (Mpeg2Section section in bouquetReader.Sections)
                        sections.Add(section);
                    bouquetReader.Sections.Clear();
                }
                bouquetReader.Release("ProcessOpenTVSections");

                if (sections.Count != 0)
                    ProcessBouquetSections(sections);

                if (OpenTVChannel.Channels.Count == lastCount)
                {
                    repeats++;
                    bouquetSectionsDone = (repeats == RunParameters.Instance.Repeats);
                }
                else
                    repeats = 0;

                lastCount = OpenTVChannel.Channels.Count;
            }

            Logger.Instance.Write("", true, false);
            Logger.Instance.Write("Stopping bouquet reader");
            bouquetReader.Stop();

            Logger.Instance.Write("Channel count: " + OpenTVChannel.Channels.Count + " buffer space used: " + dataProvider.BufferSpaceUsed + " discontinuities: " + bouquetReader.Discontinuities);
        }

        /// <summary>
        /// Process the bouquet data.
        /// </summary>
        /// <param name="sections">A collection of MPEG2 sections containing the bouquet data.</param>
        protected virtual void ProcessBouquetSections(Collection<Mpeg2Section> sections) { }

        /// <summary>
        /// Get the time offset data.
        /// </summary>
        /// <param name="dataProvider">The sample data provider.</param>
        /// <param name="worker">The BackgroundWorker instance running the collection.</param>
        protected void GetTimeOffsetSections(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            if (omitTimeZoneSections)
                return;

            if (RunParameters.Instance.TimeZoneSet)
            {
                TimeOffsetEntry.CurrentTimeOffset = RunParameters.Instance.TimeZone;
                Logger.Instance.Write("Local time offset set from Timezone ini parameter");
                omitTimeZoneSections = true;
                return;
            }

            dataProvider.ChangePidMapping(new int[] { 0x14 });

            Logger.Instance.Write("Collecting time zone data", false, true);

            timeOffsetReader = new TSStreamReader(0x73, 50000, dataProvider.BufferAddress);
            timeOffsetReader.Run();

            bool timeOffsetSectionsDone = false;

            while (!timeOffsetSectionsDone)
            {
                if (worker.CancellationPending)
                    return;

                Thread.Sleep(2000);
                Logger.Instance.Write(".", false, false);

                Collection<Mpeg2Section> sections = new Collection<Mpeg2Section>();

                timeOffsetReader.Lock("ProcessOpenTVSections");
                if (timeOffsetReader.Sections.Count != 0)
                {
                    foreach (Mpeg2Section section in timeOffsetReader.Sections)
                        sections.Add(section);
                    timeOffsetReader.Sections.Clear();
                }
                timeOffsetReader.Release("ProcessOpenTVSections");

                if (sections.Count != 0)
                    processTimeOffsetSections(sections);

                timeOffsetSectionsDone = (TimeOffsetEntry.TimeOffsets.Count != 0);
            }

            Logger.Instance.Write("", true, false);

            foreach (TimeOffsetEntry timeOffsetEntry in TimeOffsetEntry.TimeOffsets)
                Logger.Instance.Write("Time offset: " + timeOffsetEntry.CountryCode + " region " + timeOffsetEntry.Region +
                    " offset " + timeOffsetEntry.TimeOffset + " next offset: " + timeOffsetEntry.NextTimeOffset +
                    " date: " + timeOffsetEntry.ChangeTime);

            Logger.Instance.Write("Stopping time offset reader");
            timeOffsetReader.Stop();

            setTimeOffset();

            Logger.Instance.Write("Time zone count: " + TimeOffsetEntry.TimeOffsets.Count + " buffer space used: " + dataProvider.BufferSpaceUsed);
        }

        private void processTimeOffsetSections(Collection<Mpeg2Section> sections)
        {
            foreach (Mpeg2Section section in sections)
            {
                TimeOffsetSection timeOffsetSection = TimeOffsetSection.ProcessTimeOffsetTable(section.Data);
                if (timeOffsetSection != null)
                {
                    if (timeOffsetSection.Descriptors != null)
                    {
                        foreach (DescriptorBase descriptor in timeOffsetSection.Descriptors)
                        {
                            DVBLocalTimeOffsetDescriptor timeOffsetDescriptor = descriptor as DVBLocalTimeOffsetDescriptor;
                            if (timeOffsetDescriptor != null)
                            {
                                foreach (DVBLocalTimeOffsetEntry entry in timeOffsetDescriptor.TimeOffsetEntries)
                                {
                                    TimeOffsetEntry offsetEntry = new TimeOffsetEntry();
                                    offsetEntry.CountryCode = entry.CountryCode;
                                    offsetEntry.Region = entry.Region;

                                    if (entry.OffsetPositive)
                                    {
                                        offsetEntry.TimeOffset = entry.TimeOffset;
                                        offsetEntry.NextTimeOffset = entry.NextTimeOffset;
                                    }
                                    else
                                    {
                                        offsetEntry.TimeOffset = new TimeSpan() - entry.TimeOffset;
                                        offsetEntry.NextTimeOffset = new TimeSpan() - entry.NextTimeOffset;
                                    }

                                    offsetEntry.ChangeTime = entry.ChangeTime;

                                    TimeOffsetEntry.AddEntry(offsetEntry);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void setTimeOffset()
        { 
            if (TimeOffsetEntry.TimeOffsets.Count == 0)
            {
                TimeOffsetEntry.CurrentTimeOffset = new TimeSpan();
                TimeOffsetEntry.FutureTimeOffset = new TimeSpan();
                TimeOffsetEntry.TimeOfFutureTimeOffset = new DateTime();
                Logger.Instance.Write("No local time offset in effect");
                return;
            }

            if (TimeOffsetEntry.TimeOffsets.Count == 1)
            {
                TimeOffsetEntry.CurrentTimeOffset = TimeOffsetEntry.TimeOffsets[0].TimeOffset;
                TimeOffsetEntry.FutureTimeOffset = TimeOffsetEntry.TimeOffsets[0].NextTimeOffset;
                TimeOffsetEntry.TimeOfFutureTimeOffset = TimeOffsetEntry.TimeOffsets[0].ChangeTime;
                Logger.Instance.Write("Local time offset set to " + TimeOffsetEntry.TimeOffsets[0].TimeOffset +
                    " for country " + TimeOffsetEntry.TimeOffsets[0].CountryCode +
                    " region " + TimeOffsetEntry.TimeOffsets[0].Region);
                Logger.Instance.Write("Time offset will change to " + TimeOffsetEntry.TimeOffsets[0].NextTimeOffset +
                    " at " + TimeOffsetEntry.TimeOffsets[0].ChangeTime);
                return;
            }

            if (RunParameters.Instance.CountryCode != null)
            {
                TimeOffsetEntry offsetEntry = TimeOffsetEntry.FindEntry(RunParameters.Instance.CountryCode, RunParameters.Instance.Region);
                if (offsetEntry != null)
                {
                    TimeOffsetEntry.CurrentTimeOffset = offsetEntry.TimeOffset;
                    TimeOffsetEntry.FutureTimeOffset = offsetEntry.NextTimeOffset;
                    TimeOffsetEntry.TimeOfFutureTimeOffset = offsetEntry.ChangeTime;
                    Logger.Instance.Write("Local time offset set to " + offsetEntry.TimeOffset +
                        " for country " + offsetEntry.CountryCode +
                        " region " + offsetEntry.Region);
                    Logger.Instance.Write("Time offset will change to " + offsetEntry.NextTimeOffset +
                    " at " + offsetEntry.ChangeTime);
                }
                else
                {
                    TimeOffsetEntry.CurrentTimeOffset = new TimeSpan();
                    TimeOffsetEntry.FutureTimeOffset = new TimeSpan();
                    TimeOffsetEntry.TimeOfFutureTimeOffset = new DateTime();
                    Logger.Instance.Write("No local time offset in effect");
                }
            }
            else
            {
                TimeOffsetEntry.CurrentTimeOffset = new TimeSpan();
                TimeOffsetEntry.FutureTimeOffset = new TimeSpan();
                TimeOffsetEntry.TimeOfFutureTimeOffset = new DateTime();
                Logger.Instance.Write("No local time offset in effect");
            }
        }

        /// <summary>
        /// Stop the collection.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Carry out the processing necessary at the end of processing a frequency.
        /// </summary>
        public virtual void FinishFrequency() { }

        /// <summary>
        /// Carry out the processing necessary when all frequencies have been processed.
        /// </summary>
        public virtual void FinishRun() { }
    }
}
