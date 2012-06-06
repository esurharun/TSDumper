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
using System.IO;

using DirectShow;
using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that controls the acquisition and processing of FreeSat data.
    /// </summary>   
    public class FreeSatController : ControllerBase
    {
        /// <summary>
        /// Return true if all data has been processed; false otherwise.
        /// </summary>
        public override bool AllDataProcessed { get { return (true); } }

        private TSStreamReader freeSatReader;
        private bool freeSatSectionsDone = false;        

        /// <summary>
        /// Initialize a new instance of the FreeSatController class.
        /// </summary>
        public FreeSatController() { }

        /// <summary>
        /// Stop acquiring and processing data.
        /// </summary>
        public override void Stop()
        {
            Logger.Instance.Write("Stopping section readers");

            if (BouquetReader != null)
                BouquetReader.Stop();

            if (freeSatReader != null)
                freeSatReader.Stop();

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

            MultiTreeDictionaryEntry.Load(Path.Combine(RunParameters.ConfigDirectory, "Huffman Dictionary FreeSat T1.cfg"), Path.Combine(RunParameters.ConfigDirectory, "Huffman Dictionary FreeSat T2.cfg"));

            /*GetStationData(dataProvider, worker, new int[] { 0xbba, 0xc1e, 0xf01 });*/
            GetStationData(dataProvider, worker, new int[] { 0xbba });
            if (worker.CancellationPending)
                return (CollectorReply.Cancelled);

            if (RunParameters.Instance.ChannelBouquet != -1 ||
                RunParameters.Instance.Options.Contains("USECHANNELID") ||
                RunParameters.Instance.Options.Contains("USELCN") ||
                RunParameters.Instance.Options.Contains("CREATEBRCHANNELS") ||
                RunParameters.Instance.Options.Contains("CREATEARCHANNELS"))
            {
                /*GetBouquetSections(dataProvider, worker, new int[] { 0xbba, oxc1e, oxf01 } );*/
                GetBouquetSections(dataProvider, worker, new int[] { 0xbba });
                if (worker.CancellationPending)
                    return (CollectorReply.Cancelled);
            }

            getFreeSatSections(dataProvider, worker);

            return (CollectorReply.OK);
        }

        /// <summary>
        /// Process the bouquet data.
        /// </summary>
        /// <param name="sections">A collection of MPEG2 sections containing the bouquet data.</param>
        protected override void ProcessBouquetSections(Collection<Mpeg2Section> sections)
        {
            foreach (Mpeg2Section section in sections)
            {
                if (RunParameters.Instance.TraceIDs.Contains("BOUQUETSECTIONS"))
                    Logger.Instance.Dump("Bouquet Section", section.Data, section.Length);

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
                                        FreeSatChannelInfoDescriptor freeSatInfoDescriptor = descriptor as FreeSatChannelInfoDescriptor;
                                        if (freeSatInfoDescriptor != null)
                                        {
                                            if (freeSatInfoDescriptor.ChannelInfoEntries != null)
                                            {
                                                foreach (FreeSatChannelInfoEntry channelInfoEntry in freeSatInfoDescriptor.ChannelInfoEntries)
                                                {
                                                    FreeSatChannel channel = new FreeSatChannel();
                                                    channel.OriginalNetworkID = transportStream.OriginalNetworkID;
                                                    channel.TransportStreamID = transportStream.TransportStreamID;
                                                    channel.ServiceID = channelInfoEntry.ServiceID;
                                                    channel.UserChannel = channelInfoEntry.UserNumber;
                                                    channel.Unknown1 = channelInfoEntry.Unknown1;
                                                    channel.Unknown2 = channelInfoEntry.Unknown2;
                                                    channel.BouquetID = bouquetSection.BouquetID;
                                                    FreeSatChannel.AddChannel(channel);

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
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void getFreeSatSections(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            Logger.Instance.Write("Collecting FreeSat data", false, true);

            dataProvider.ChangePidMapping(new int[] { 0xbbb, 0xc1f, 0xf02 });

            freeSatReader = new TSStreamReader(2000, dataProvider.BufferAddress);
            freeSatReader.Run();

            int lastCount = 0;
            int repeats = 0;

            while (!freeSatSectionsDone)
            {
                if (worker.CancellationPending)
                    return;

                Thread.Sleep(2000);
                Logger.Instance.Write(".", false, false);

                Collection<Mpeg2Section> sections = new Collection<Mpeg2Section>();

                freeSatReader.Lock("LoadMessages");
                if (freeSatReader.Sections.Count != 0)
                {
                    foreach (Mpeg2Section section in freeSatReader.Sections)
                        sections.Add(section);
                    freeSatReader.Sections.Clear();
                }
                freeSatReader.Release("LoadMessages");

                if (sections.Count != 0)
                    processSections(sections);

                if (TVStation.EPGCount == lastCount)
                {
                    repeats++;
                    freeSatSectionsDone = (repeats == RunParameters.Instance.Repeats);
                }
                else
                    repeats = 0;

                lastCount = TVStation.EPGCount;
            }

            Logger.Instance.Write("", true, false);
            Logger.Instance.Write("Stopping reader");
            freeSatReader.Stop();

            Logger.Instance.Write("EPG count: " + TVStation.EPGCount + " buffer space used: " + dataProvider.BufferSpaceUsed);
        }

        private void processSections(Collection<Mpeg2Section> sections)
        {
            foreach (Mpeg2Section section in sections)
            {
                if (RunParameters.Instance.TraceIDs.Contains("FREESATSECTIONS"))
                    Logger.Instance.Dump("FreeSat Section", section.Data, section.Length);

                try
                {
                    Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();
                    mpeg2Header.Process(section.Data);
                    if (mpeg2Header.Current)
                    {
                        FreeSatSection freeSatSection = new FreeSatSection();
                        freeSatSection.Process(section.Data, mpeg2Header);
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Logger.Instance.Write("<e> FreeSat error: " + e.Message);
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
