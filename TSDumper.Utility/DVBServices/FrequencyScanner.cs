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

using DomainObjects;
using DirectShow;

namespace DVBServices
{
    /// <summary>
    /// The class the describes the frequency scanner that locates stations.
    /// </summary>
    public class FrequencyScanner
    {
        /// <summary>
        /// Get or set whether to use the Network Information sections to set the actual station frequency.
        /// </summary>
        public bool UseActualFrequency
        {
            get { return (useActualFrequency); }
            set { useActualFrequency = value; }
        }

        private const int serviceTypeDigitalTV = 1;
        private const int serviceTypeDigitalRadio = 2;
        private const int serviceTypeHDTV = 17;

        private const int runningStatusRunning = 4;

        private const int streamTypeVideo = 2;
        private const int streamTypeAudio = 4;
        private const int streamTypeDSMCCUserToNetwork = 11;

        private ISampleDataProvider dataProvider;        

        private int[] pids;
        private bool searchOtherTable = true;
        private BackgroundWorker worker;

        private bool useActualFrequency;

        /// <summary>
        /// Initialize a new instance of the FrequencyScanner class.
        /// </summary>
        /// <param name="dataProvider">A sample data provider.</param>
        public FrequencyScanner(ISampleDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;

            pids = new int[] { BDAGraph.SdtPid } ;
        }

        /// <summary>
        /// Initialize a new instance of the FrequencyScanner class.
        /// </summary>
        /// <param name="dataProvider">A sample data provider.</param>
        /// <param name="worker">An optional background worker instance. Can be null.</param>
        public FrequencyScanner(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            this.dataProvider = dataProvider;
            this.worker = worker;

            pids = new int[] { BDAGraph.SdtPid };
        }

        /// <summary>
        /// Initialize a new instance of the FrequencyScanner.
        /// </summary>
        /// <param name="dataProvider">A sample data provider.</param>
        /// <param name="pids">A collection of PID's to be searched.</param>
        public FrequencyScanner(ISampleDataProvider dataProvider, int[] pids) : this(dataProvider)
        {
            this.pids = pids;
        }

        /// <summary>
        /// Initialize a new instance of the FrequencyScanner.
        /// </summary>
        /// <param name="dataProvider">A sample data provider.</param>
        /// <param name="pids">A collection of PID's to be searched.</param>
        /// <param name="searchOtherTable">True to include the 'other' stations; false otherwise</param>
        public FrequencyScanner(ISampleDataProvider dataProvider, int[] pids, bool searchOtherTable) : this(dataProvider, pids)
        {
            this.searchOtherTable = searchOtherTable;
        }

        /// <summary>
        /// Initialize a new instance of the FrequencyScanner.
        /// </summary>
        /// <param name="dataProvider">A sample data provider.</param>
        /// <param name="pids">A collection of PID's to be searched.</param>
        /// <param name="searchOtherTable">True to include the 'other' stations; false otherwise</param>
        /// <param name="worker">An optional background worker instance. Can be null.</param>
        public FrequencyScanner(ISampleDataProvider dataProvider, int[] pids, bool searchOtherTable, BackgroundWorker worker) : this(dataProvider, pids, searchOtherTable)
        {
            this.worker = worker;
        }

        /// <summary>
        /// Find TV stations.
        /// </summary>
        /// <returns>A collection of TV stations.</returns>
        public Collection<TVStation> FindTVStations()
        {
            if (dataProvider.Frequency.TunerType != TunerType.ATSC)
                return (findDvbStations());
            else
                return (findAtscStations());
        }

        private Collection<TVStation> findDvbStations()
        {
            if (useActualFrequency || RunParameters.Instance.DebugIDs.Contains("CREATESATINI"))
                processNITSections(worker);

            dataProvider.ChangePidMapping(pids);

            if (!RunParameters.Instance.TraceIDs.Contains("BDA"))
                Logger.Instance.Write("Collecting station data", false, true);
            else
                Logger.Instance.Write("Collecting station data");

            Collection<TVStation> tvStations = new Collection<TVStation>();            

            Collection<byte> tables = new Collection<byte>();
            tables.Add(BDAGraph.SdtTable);
            if (searchOtherTable)
                tables.Add(BDAGraph.SdtOtherTable);

            TSStreamReader stationReader = stationReader = new TSStreamReader(tables, 2000, dataProvider.BufferAddress);
            stationReader.Run();

            Collection<Mpeg2Section> sections = null;

            int lastCount = 0;
            int repeats = 0;
            bool done = false;

            while (!done)
            {
                if (worker != null && worker.CancellationPending)
                {
                    stationReader.Stop();
                    return (null);
                }

                Thread.Sleep(2000);

                if (!RunParameters.Instance.TraceIDs.Contains("BDA"))
                    Logger.Instance.Write(".", false, false);
                else
                    Logger.Instance.Write("BDA Buffer space used " + dataProvider.BufferSpaceUsed);
                
                sections = new Collection<Mpeg2Section>();

                stationReader.Lock("ProcessSDTSections");
                if (stationReader.Sections.Count != 0)
                {
                    foreach (Mpeg2Section section in stationReader.Sections)
                        sections.Add(section);
                    stationReader.Sections.Clear();
                }
                stationReader.Release("ProcessSDTSections");

                foreach (Mpeg2Section section in sections)
                {
                    ServiceDescriptionSection serviceDescriptionSection = ServiceDescriptionSection.ProcessServiceDescriptionTable(section.Data);
                    if (serviceDescriptionSection != null)
                        processServiceDescriptionSection(serviceDescriptionSection, tvStations, dataProvider.Frequency);
                }

                if (tvStations.Count == lastCount)
                {
                    repeats++;
                    done = (repeats == RunParameters.Instance.Repeats);
                }
                else
                    repeats = 0;

                lastCount = tvStations.Count;
            }

            if (!RunParameters.Instance.TraceIDs.Contains("BDA"))
                Logger.Instance.Write("", true, false);

            Logger.Instance.Write("Stopping station reader for frequency " + dataProvider.Frequency);
            stationReader.Stop();

            int bufferSpaceUsed = dataProvider.BufferSpaceUsed;

            if (dataProvider.Frequency.CollectionType == CollectionType.MHEG5 && tvStations.Count != 0)
            {
                int debugPid = -1;

                foreach (string debugID in RunParameters.Instance.DebugIDs)
                {
                    if (debugID.StartsWith("MHEG5PID-"))
                    {
                        string[] parts = debugID.Split(new char[] { '-' });
                        debugPid = Int32.Parse(parts[1]);
                    }
                }

                if (debugPid == -1)
                {
                    Logger.Instance.Write("Collecting PAT/PMT data to locate MHEG5 pid(s)");

                    processPATSections(tvStations);

                    foreach (TVStation station in tvStations)
                    {
                        if (!station.Excluded && station.DSMCCPID != 0)
                            dataProvider.Frequency.DSMCCPid = station.DSMCCPID;
                    }
                }
                else
                    dataProvider.Frequency.DSMCCPid = debugPid;
            }

            Logger.Instance.Write("Stations: " + tvStations.Count + " buffer space used: " + bufferSpaceUsed + " discontinuities: " + stationReader.Discontinuities);   

            return (tvStations);
        }

        private void processPATSections(Collection<TVStation> tvStations)
        {
            dataProvider.ChangePidMapping(new int[] { BDAGraph.PatPid } );

            TSReaderBase patReader = new TSStreamReader(BDAGraph.PatTable, 2000, dataProvider.BufferAddress);
            patReader.Run();

            Thread.Sleep(2000);

            Collection<Mpeg2Section> sections = new Collection<Mpeg2Section>();            

            patReader.Lock("ProcessPATSections");
            if (patReader.Sections.Count != 0)
            {
                foreach (Mpeg2Section section in patReader.Sections)
                    sections.Add(section);
                patReader.Sections.Clear();
            }
            patReader.Release("ProcessPATSections");

            patReader.Stop();

            Collection<int> sectionNumbers = new Collection<int>();

            if (sections == null)
            {
                Logger.Instance.Write("No PAT sections received");
                return;
            }
            else
            {
                foreach (Mpeg2Section section in sections)
                {
                    ProgramAssociationSection programAssociationSection = ProgramAssociationSection.ProcessProgramAssociationTable(section.Data);
                    if (programAssociationSection != null)
                    {
                        if (!sectionNumbers.Contains(programAssociationSection.SectionNumber))
                        {
                            processProgramAssociationSection(programAssociationSection, tvStations);
                            sectionNumbers.Add(programAssociationSection.SectionNumber);
                        }
                    }
                }
            }
        }

        private void processServiceDescriptionSection(ServiceDescriptionSection serviceDescriptionSection, Collection<TVStation> tvStations, TuningFrequency frequency)
        {
            foreach (ServiceDescription serviceDescription in serviceDescriptionSection.ServiceDescriptions)
            {
                bool processStation = checkServiceInfo(serviceDescription);

                if (processStation)
                {
                    TVStation tvStation = new TVStation(serviceDescription.ServiceName);
                    tvStation.ProviderName = serviceDescription.ProviderName;
                    if (useActualFrequency)
                    {
                        tvStation.Frequency = NetworkInformationSection.GetFrequency(serviceDescriptionSection.OriginalNetworkID, serviceDescriptionSection.TransportStreamID) * 10;
                        if (tvStation.Frequency == 0)
                        {
                            tvStation.Frequency = frequency.Frequency;
                            Logger.Instance.Write("Station : " + tvStation.Name + " not found in Network Information Table");
                        }
                    }
                    else
                        tvStation.Frequency = frequency.Frequency;

                    tvStation.OriginalNetworkID = serviceDescriptionSection.OriginalNetworkID;
                    tvStation.TransportStreamID = serviceDescriptionSection.TransportStreamID;
                    tvStation.ServiceID = serviceDescription.ServiceID;
                    tvStation.Encrypted = serviceDescription.Scrambled;
                    tvStation.ServiceType = serviceDescription.ServiceType;
                    tvStation.ScheduleAvailable = serviceDescription.EITSchedule;
                    tvStation.NextFollowingAvailable = serviceDescription.EITPresentFollowing;

                    tvStation.TunerType = frequency.TunerType;
                    if (frequency.TunerType == TunerType.Satellite)
                    {
                        Satellite satellite = ((SatelliteFrequency)frequency).Provider as Satellite;
                        if (satellite != null)
                            tvStation.Satellite = satellite;
                    }

                    if (RunParameters.Instance.Options.Contains("USECHANNELID"))
                    {
                        if (serviceDescription.ChannelNumber != -1)
                            tvStation.OriginalChannelNumber = serviceDescription.ChannelNumber;
                    }

                    addStation(tvStations, tvStation);
                }
            }
        }

        private bool checkServiceInfo(ServiceDescription serviceDescription)
        {
            if (serviceDescription.ServiceType == 0)
                return (false);

            if (RunParameters.Instance.Options.Contains("PROCESSALLSTATIONS"))
                return (true);

            return (serviceDescription.ServiceType != 0x0c);
        }

        private void addStation(Collection<TVStation> tvStations, TVStation newStation)
        {
            foreach (TVStation oldStation in tvStations)
            {
                if (oldStation.OriginalNetworkID == newStation.OriginalNetworkID &&
                    oldStation.TransportStreamID == newStation.TransportStreamID &&
                    oldStation.ServiceID == newStation.ServiceID)
                {
                    oldStation.Frequency = newStation.Frequency;
                    oldStation.Name = newStation.Name;
                    return;
                }
            }

            tvStations.Add(newStation);
        }

        private void processProgramAssociationSection(ProgramAssociationSection programAssociationSection, Collection<TVStation> tvStations)
        {
            foreach (ProgramInfo programInfo in programAssociationSection.ProgramInfos)
            {
                if (programInfo.ProgramNumber != 0)
                    processProgramInfo(programInfo, tvStations);
            }
        }

        private void processProgramInfo(ProgramInfo programInfo, Collection<TVStation> tvStations)
        {
            dataProvider.ChangePidMapping(new int[] { programInfo.ProgramID } );

            TSReaderBase pmtReader = new TSStreamReader(BDAGraph.PmtTable, 2000, dataProvider.BufferAddress);
            pmtReader.Run();
            
            Collection<Mpeg2Section> sections = new Collection<Mpeg2Section>();

            int repeats = 0;
            bool done = false;

            while (!done)
            {
                Thread.Sleep(10);
                                
                pmtReader.Lock("ProcessPMTSections");
                if (pmtReader.Sections.Count != 0)
                {
                    foreach (Mpeg2Section section in pmtReader.Sections)
                        sections.Add(section);
                    pmtReader.Sections.Clear();
                }
                pmtReader.Release("ProcessPMTSections");

                done = (sections.Count != 0);

                if (!done)
                {
                    repeats++;
                    done = (repeats == 500);
                }
            }        

            pmtReader.Stop();

            if (sections.Count == 0)
            {
                Logger.Instance.Write("No PMT sections received");
                return;
            }
            else
            {
                foreach (Mpeg2Section section in sections)
                {
                    ProgramMapSection programMapSection = ProgramMapSection.ProcessProgramMapTable(section.Data);
                    processProgramMapSection(programMapSection, programInfo, tvStations);
                }
            }
        }

        private void processProgramMapSection(ProgramMapSection programMapSection, ProgramInfo programInfo, Collection<TVStation> tvStations)
        {
            TVStation tvStation = findTVStation(programInfo.ProgramNumber, tvStations);
            if (tvStation == null)
                return;

            foreach (StreamInfo streamInfo in programMapSection.StreamInfos)
            {
                if (streamInfo.StreamType == streamTypeVideo)
                    tvStation.VideoPID = streamInfo.ProgramID;
                if (streamInfo.StreamType == streamTypeAudio)
                    tvStation.AudioPID = streamInfo.ProgramID;
                if (streamInfo.StreamType == streamTypeDSMCCUserToNetwork)
                    tvStation.DSMCCPID = streamInfo.ProgramID;
            }
        }

        private TVStation findTVStation(int serviceID, Collection<TVStation> tvStations)
        {
            foreach (TVStation tvStation in tvStations)
            {
                if (tvStation.ServiceID == serviceID)
                    return (tvStation);
            }

            return (null);
        }

        private void processNITSections(BackgroundWorker worker)
        {
            if (!RunParameters.Instance.TraceIDs.Contains("BDA"))
                Logger.Instance.Write("Collecting network information data", false, true);
            else
                Logger.Instance.Write("Collecting network information data");

            dataProvider.ChangePidMapping(new int[] { BDAGraph.NitPid });

            Collection<byte> tables = new Collection<byte>();
            tables.Add(0x40);
            tables.Add(0x41);
            TSReaderBase nitReader = new TSStreamReader(tables, 50000, dataProvider.BufferAddress);
            nitReader.Run();            

            bool done = false;
            int lastCount = 0;
            int repeats = 0;

            while (!done)
            {
                if (worker != null && worker.CancellationPending)
                {
                    nitReader.Stop();
                    return;
                }

                Thread.Sleep(2000);

                if (!RunParameters.Instance.TraceIDs.Contains("BDA"))
                    Logger.Instance.Write(".", false, false);
                else
                    Logger.Instance.Write("Buffer space used " + dataProvider.BufferSpaceUsed);

                Collection<Mpeg2Section> sections = new Collection<Mpeg2Section>();

                nitReader.Lock("ProcessNITSections");
                if (nitReader.Sections.Count != 0)
                {
                    foreach (Mpeg2Section section in nitReader.Sections)
                        sections.Add(section);
                    nitReader.Sections.Clear();
                }
                nitReader.Release("ProcessNITSections");

                foreach (Mpeg2Section section in sections)
                {
                    NetworkInformationSection networkInformationSection = NetworkInformationSection.ProcessNetworkInformationTable(section.Data);
                    if (networkInformationSection != null)
                        NetworkInformationSection.AddSection(networkInformationSection);
                }

                if (NetworkInformationSection.NetworkInformationSections.Count == lastCount)
                {
                    repeats++;
                    done = (repeats == RunParameters.Instance.Repeats);
                }
                else
                    repeats = 0;

                lastCount = NetworkInformationSection.NetworkInformationSections.Count;
            }

            if (!RunParameters.Instance.TraceIDs.Contains("BDA"))
                Logger.Instance.Write("", true, false);

            Logger.Instance.Write("Stopping network information reader for frequency " + dataProvider.Frequency);
            nitReader.Stop();            
        }

        private Collection<TVStation> findAtscStations()
        {
            Logger.Instance.Write("Collecting ATSC Channel data", false, true);

            Collection<TVStation> tvStations = new Collection<TVStation>();

            VirtualChannelTable.Clear();

            dataProvider.ChangePidMapping(new int[] { 0x1ffb });

            Collection<byte> tables = new Collection<byte>();
            tables.Add(0xc8);
            tables.Add(0xc9);
            TSStreamReader guideReader = new TSStreamReader(tables, 50000, dataProvider.BufferAddress);
            guideReader.Run();

            int repeats = 0;
            bool done = false;

            while (!done)
            {
                if (worker.CancellationPending)
                    return (tvStations);

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
                    processVirtualChannelTable(sections, dataProvider.Frequency.Frequency);

                done = VirtualChannelTable.Complete;
                if (!done)
                {
                    repeats++;
                    done = (repeats == RunParameters.Instance.Repeats);
                }
            }

            Logger.Instance.Write("", true, false);
            Logger.Instance.Write("Stopping reader");
            guideReader.Stop();

            foreach (VirtualChannel channel in VirtualChannelTable.Channels)
            {
                TVStation station = new TVStation(channel.ShortName);
                station.StationType = TVStationType.Atsc;
                station.OriginalNetworkID = channel.CollectionFrequency;
                station.TransportStreamID = channel.MajorChannelNumber;
                station.ServiceID = channel.MinorChannelNumber;
                tvStations.Add(station);
            }

            return (tvStations);
        }

        private void processVirtualChannelTable(Collection<Mpeg2Section> sections, int frequency)
        {
            foreach (Mpeg2Section section in sections)
            {
                if (RunParameters.Instance.DebugIDs.Contains("VIRTUALCHANNELTABLE"))
                    Logger.Instance.Dump("PSIP Virtual Channel Table", section.Data, section.Data.Length);

                try
                {
                    Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();
                    mpeg2Header.Process(section.Data);
                    if (mpeg2Header.Current)
                    {
                        VirtualChannelTable virtualChannelTable = new VirtualChannelTable();
                        virtualChannelTable.Process(section.Data, mpeg2Header, (mpeg2Header.TableID == 0xc9), frequency);
                        VirtualChannelTable.AddSectionNumber(mpeg2Header.SectionNumber);                        
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Logger.Instance.Write("PSIP error: " + e.Message);
                }
            }
        }
    }
}
