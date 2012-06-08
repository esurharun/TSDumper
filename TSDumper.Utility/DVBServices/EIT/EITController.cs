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
using System.IO;

using DirectShow;
using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that controls the acquisition and processing of EIT data.
    /// </summary>
    public class EITController : ControllerBase
    {
        /// <summary>
        /// Return true if the EIT data is complete; false otherwise.
        /// </summary>
        public override bool AllDataProcessed { get { return (eitSectionsDone);  } }

        private TSStreamReader eitReader;

        private bool eitSectionsDone = false;
        private int eitChannels;
        private int openTVChannels;

        /// <summary>
        /// Initialize a new instance of the EITController class.
        /// </summary>
        public EITController() { }

        /// <summary>
        /// Stop acquiring and processing EIT data.
        /// </summary>
        public override void Stop()
        {
            Logger.Instance.Write("Stopping section readers");

            if (BouquetReader != null)
                BouquetReader.Stop();

            if (eitReader != null)
                eitReader.Stop();

            Logger.Instance.Write("Stopped section readers");
        }

        /// <summary>
        /// Acquire and process EIT data.
        /// </summary>
        /// <param name="dataProvider">A sample data provider.</param>
        /// <param name="worker">The background worker that is running this collection.</param>
        /// <returns>A CollectorReply code.</returns>
        public override CollectorReply Process(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            EITProgramContent.Load();
            CustomProgramCategory.Load();
            ParentalRating.Load();

            if (RunParameters.Instance.Options.Contains("USEFREESATTABLES"))
                MultiTreeDictionaryEntry.Load(Path.Combine(RunParameters.ConfigDirectory, "Huffman Dictionary FreeSat T1.cfg"), Path.Combine(RunParameters.ConfigDirectory, "Huffman Dictionary FreeSat T2.cfg"));

            GetStationData(dataProvider, worker);
            if (worker.CancellationPending)
                return (CollectorReply.Cancelled);

            bool bouquetNeeded = checkBouquetNeeded();
            if (RunParameters.Instance.ChannelBouquet != -1 || 
                RunParameters.Instance.Options.Contains("USECHANNELID") || 
                RunParameters.Instance.Options.Contains("USELCN") ||
                RunParameters.Instance.Options.Contains("CREATEBRCHANNELS") ||
                RunParameters.Instance.Options.Contains("CREATEARCHANNELS"))
            {
                GetBouquetSections(dataProvider, worker);

                string bouquetType;

                if (eitChannels > 0)
                    bouquetType = "Freeview";
                else
                    bouquetType = "OpenTV";

                Logger.Instance.Write("Used " + bouquetType + " channel descriptors");

                if (worker.CancellationPending)
                    return (CollectorReply.Cancelled);
            }

            getEITSections(dataProvider, worker);

            return (CollectorReply.OK);
        }

        private bool checkBouquetNeeded()
        {
            if (RunParameters.Instance.ChannelBouquet != -1)
                return (true);

            if (!RunParameters.Instance.Options.Contains("USECHANNELID") &&
                !RunParameters.Instance.Options.Contains("USELCN") &&
                !RunParameters.Instance.Options.Contains("CREATEBRCHANNELS") &&
                !RunParameters.Instance.Options.Contains("CREATEARCHANNELS"))
                return (false);

            foreach (TVStation station in TVStation.StationCollection)
            {
                if (station.ChannelID != null)
                    return (false);
            }

            return (true);
        }

        /// <summary>
        /// Process the bouquet data.
        /// </summary>
        /// <param name="sections">A collection of MPEG2 sections containing the bouquet data.</param>
        protected override void ProcessBouquetSections(Collection<Mpeg2Section> sections)
        {
            foreach (Mpeg2Section section in sections)
            {
                if (RunParameters.Instance.DebugIDs.Contains("BOUQUETSECTIONS"))
                    Logger.Instance.Dump("Bouquet Section", section.Data, section.Data.Length);

                BouquetAssociationSection bouquetSection = BouquetAssociationSection.ProcessBouquetAssociationTable(section.Data);
                if (bouquetSection != null)
                {
                    bool added = BouquetAssociationSection.AddSection(bouquetSection);
                    if (added)
                    {
                        if (bouquetSection.TransportStreams != null)
                        {
                            foreach (TransportStream transportStream in bouquetSection.TransportStreams)
                            {
                                if (transportStream.Descriptors != null)
                                {
                                    foreach (DescriptorBase descriptor in transportStream.Descriptors)
                                    {
                                        FreeviewChannelInfoDescriptor freeviewInfoDescriptor = descriptor as FreeviewChannelInfoDescriptor;
                                        if (freeviewInfoDescriptor != null)
                                            processFreeviewInfoDescriptor(freeviewInfoDescriptor, transportStream.OriginalNetworkID, transportStream.TransportStreamID, bouquetSection.BouquetID);
                                        else
                                        {
                                            OpenTVChannelInfoDescriptor openTVInfoDescriptor = descriptor as OpenTVChannelInfoDescriptor;
                                            if (openTVInfoDescriptor != null)
                                                processOpenTVInfoDescriptor(openTVInfoDescriptor, transportStream.OriginalNetworkID, transportStream.TransportStreamID, bouquetSection.BouquetID);                                            
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void processFreeviewInfoDescriptor(FreeviewChannelInfoDescriptor freeviewInfoDescriptor, int originalNetworkID, int transportStreamID, int bouquetID)
        {
            if (freeviewInfoDescriptor.ChannelInfoEntries == null)
                return;

            if (openTVChannels != 0)
                return;

            foreach (FreeviewChannelInfoEntry channelInfoEntry in freeviewInfoDescriptor.ChannelInfoEntries)
            {
                EITChannel channel = new EITChannel();
                channel.OriginalNetworkID = originalNetworkID;
                channel.TransportStreamID = transportStreamID;
                channel.ServiceID = channelInfoEntry.ServiceID;
                channel.UserChannel = channelInfoEntry.UserNumber;
                channel.Flags = channelInfoEntry.Flags;
                channel.BouquetID = bouquetID;
                EITChannel.AddChannel(channel);
                
                eitChannels++;

                Bouquet bouquet = Bouquet.FindBouquet(channel.BouquetID);
                if (bouquet == null)
                {
                    bouquet = new Bouquet(channel.BouquetID, BouquetAssociationSection.FindBouquetName(channel.BouquetID));
                    Bouquet.AddBouquet(bouquet);
                }
                
                Region region = bouquet.FindRegion(channel.Region);
                if (region == null)
                {
                    region = new Region(string.Empty, channel.Region);
                    bouquet.AddRegion(region);
                }

                region.AddChannel(channel);
            }
        }

        private void processOpenTVInfoDescriptor(OpenTVChannelInfoDescriptor openTVInfoDescriptor, int originalNetworkID, int transportStreamID, int bouquetID)
        {
            if (openTVInfoDescriptor.ChannelInfoEntries == null)
                return;

            if (eitChannels != 0)
            {
                OpenTVChannel.Channels.Clear();
                eitChannels = 0;
                return;
            }

            foreach (OpenTVChannelInfoEntry channelInfoEntry in openTVInfoDescriptor.ChannelInfoEntries)
            {
                OpenTVChannel channel = new OpenTVChannel();
                channel.OriginalNetworkID = originalNetworkID;
                channel.TransportStreamID = transportStreamID;
                channel.ServiceID = channelInfoEntry.ServiceID;
                channel.ChannelID = channelInfoEntry.ChannelID;
                channel.UserChannel = channelInfoEntry.UserNumber;
                channel.Type = channelInfoEntry.Type;
                channel.Flags = channelInfoEntry.Flags;
                channel.BouquetID = bouquetID;
                channel.Region = openTVInfoDescriptor.Region;
                OpenTVChannel.AddChannel(channel);

                openTVChannels++;

                Bouquet bouquet = Bouquet.FindBouquet(channel.BouquetID);
                if (bouquet == null)
                {
                    bouquet = new Bouquet(channel.BouquetID, BouquetAssociationSection.FindBouquetName(channel.BouquetID));
                    Bouquet.AddBouquet(bouquet);
                }

                Region region = bouquet.FindRegion(channel.Region);
                if (region == null)
                {
                    region = new Region(string.Empty, channel.Region);
                    bouquet.AddRegion(region);
                }

                region.AddChannel(channel); 
            }
        }

        private void getEITSections(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            Logger.Instance.Write("Collecting EIT data", false, true);

            int actualPid;
            if (RunParameters.Instance.EITPid == -1)
                actualPid = BDAGraph.EitPid;
            else
                actualPid = RunParameters.Instance.EITPid;

            dataProvider.ChangePidMapping(new int[] { actualPid });            

            eitReader = new TSStreamReader(2000, dataProvider.BufferAddress); 
            eitReader.Run();

            int lastCount = 0;
            int repeats = 0;

            while (!eitSectionsDone)
            {
                if (worker.CancellationPending)
                    return;

                Thread.Sleep(2000);
                Logger.Instance.Write(".", false, false);

                Collection<Mpeg2Section> sections = new Collection<Mpeg2Section>();

                eitReader.Lock("LoadMessages");
                if (eitReader.Sections.Count != 0)
                {                    
                    foreach (Mpeg2Section section in eitReader.Sections)
                        sections.Add(section);
                    eitReader.Sections.Clear();
                }
                eitReader.Release("LoadMessages");

                if (sections.Count != 0)
                    processSections(sections);

                if (TVStation.EPGCount == lastCount)
                {
                    repeats++;
                    eitSectionsDone = (repeats == RunParameters.Instance.Repeats);
                }
                else
                    repeats = 0;

                lastCount = TVStation.EPGCount;
            }

            Logger.Instance.Write("", true, false);
            Logger.Instance.Write("Stopping reader");
            eitReader.Stop();

            Logger.Instance.Write("EPG count: " + TVStation.EPGCount + " buffer space used: " + dataProvider.BufferSpaceUsed + " discontinuities: " + eitReader.Discontinuities);            
        }

        private void processSections(Collection<Mpeg2Section> sections)
        {
            foreach (Mpeg2Section section in sections)
            {
                if (RunParameters.Instance.DebugIDs.Contains("DUMPEITSECTIONS"))
                    Logger.Instance.Dump("EIT Section", section.Data, section.Data.Length);

                if (section.Table >= 0x4e && section.Table <= 0x6f)
                {
                    try
                    {
                        Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();
                        mpeg2Header.Process(section.Data);
                        if (mpeg2Header.Current)
                        {
                            EITSection eitSection = new EITSection();
                            eitSection.Process(section.Data, mpeg2Header);
                            eitSection.LogMessage();
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Logger.Instance.Write("<e> EIT error: " + e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Create the EPG entries.
        /// </summary>
        public override void FinishFrequency() 
        {
            if (RunParameters.Instance.ChannelBouquet != -1)
            {
                foreach (TVStation tvStation in TVStation.StationCollection)
                {
                    bool process = checkChannelMapping(tvStation);
                    if (!process)
                        tvStation.EPGCollection.Clear();
                }
            }
            else
            {
                foreach (Bouquet bouquet in Bouquet.Bouquets)
                {
                    foreach (Region region in bouquet.Regions)
                    {
                        foreach (Channel channel in region.Channels)
                        {
                            TVStation station = TVStation.FindStation(channel.OriginalNetworkID, channel.TransportStreamID, channel.ServiceID);
                            if (station != null && station.LogicalChannelNumber == -1)
                                station.LogicalChannelNumber = channel.UserChannel;
                        }
                    }
                }
            }

            EITProgramContent.LogContentUsage();
            LanguageCode.LogUsage();
            logChannelInfo();
        }

        private bool checkChannelMapping(TVStation tvStation)
        {
            Bouquet bouquet = Bouquet.FindBouquet(RunParameters.Instance.ChannelBouquet);
            if (bouquet == null)
                return (false);

            Region region = bouquet.FindRegion(RunParameters.Instance.ChannelRegion);
            if (region == null)
                return (false);

            Channel channel = region.FindChannel(tvStation.OriginalNetworkID, tvStation.TransportStreamID, tvStation.ServiceID);
            if (channel == null)
                return (false);

            if (tvStation.LogicalChannelNumber != -1)
                return (true);

            tvStation.LogicalChannelNumber = channel.UserChannel;

            return (true);
        }

        private void logChannelInfo()
        {
            if (!RunParameters.Instance.DebugIDs.Contains("LOGCHANNELS"))
                return;

            if (RunParameters.Instance.ChannelBouquet == -1 &&
                !RunParameters.Instance.Options.Contains("USECHANNELID") &&
                !RunParameters.Instance.Options.Contains("USELCN") &&
                !RunParameters.Instance.Options.Contains("CREATEBRCHANNELS") &&
                !RunParameters.Instance.Options.Contains("CREATEARCHANNELS"))
                return;

            Logger.Instance.WriteSeparator("Bouquet Usage");

            bool firstBouquet = true;

            foreach (Bouquet bouquet in Bouquet.GetBouquetsInNameOrder())
            {
                if (!firstBouquet)
                    Logger.Instance.Write("");

                firstBouquet = false;

                if (!firstBouquet)
                    Logger.Instance.Write("");

                firstBouquet = false;

                foreach (Region region in bouquet.Regions)
                {
                    Logger.Instance.Write("Bouquet: " + bouquet.BouquetID + " - " + bouquet.Name + " Region: " + region.Code + " (channels = " + region.Channels.Count + ")");

                    foreach (Channel channel in region.GetChannelsInNameOrder())
                        Logger.Instance.Write("    Channel: " + channel.ToString());
                }
            }
        }
    }
}
