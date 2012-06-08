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
using System.Text;

using DirectShow;
using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that controls the acquisition and processing of the DSMCC protocol.
    /// </summary>
    public class DSMCCController : ControllerBase
    {
        /// <summary>
        /// Return true if the DSMCC data is complete; false otherwise.
        /// </summary>
        public override bool AllDataProcessed { get { return (checkAllDataLoaded()); } }

        private TSStreamReader dsmccReader;

        private DSMCCDownloadServerInitiate dsiMessage;
        private Collection<DSMCCDownloadInfoIndication> diiMessages;
        private Collection<DSMCCModule> modules;

        private static Logger titleLogger;
        private static Logger descriptionLogger;

        private string prefix = "";

        private bool noMHEGPid;

        /// <summary>
        /// Initialize a new instance of the DSMCCController class.
        /// </summary>
        public DSMCCController()
        {
            if (diiMessages == null)
                diiMessages = new Collection<DSMCCDownloadInfoIndication>();
            if (modules == null)
                modules = new Collection<DSMCCModule>();
        }

        /// <summary>
        /// Stop acquiring and processing DSMCC data.
        /// </summary>
        public override void Stop()
        {
            Logger.Instance.Write("Stopping section readers");

            if (BouquetReader != null)
                BouquetReader.Stop();

            if (dsmccReader != null)
                dsmccReader.Stop();

            Logger.Instance.Write("Stopped section readers");
        }

        /// <summary>
        /// Acquire and process DSMCC data.
        /// </summary>
        /// <param name="dataProvider">A sample data provider.</param>
        /// <param name="worker">The background worker that is running this collection.</param>
        /// <returns>A CollectorReply code.</returns>
        public override CollectorReply Process(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            if (RunParameters.Instance.CountryCode == null)
                MHEGParserParameters.Process("MHEG5 Parser Format NZL.cfg");
            else
                MHEGParserParameters.Process("MHEG5 Parser Format " + RunParameters.Instance.CountryCode + ".cfg");

            CustomProgramCategory.Load();
            ParentalRating.Load();

            GetStationData(dataProvider, worker);
            if (worker.CancellationPending)
                return (CollectorReply.Cancelled);

            if (RunParameters.Instance.ChannelBouquet != -1 ||
                RunParameters.Instance.Options.Contains("USECHANNELID") ||
                RunParameters.Instance.Options.Contains("USELCN") ||
                RunParameters.Instance.Options.Contains("CREATEBRCHANNELS") ||
                RunParameters.Instance.Options.Contains("CREATEARCHANNELS"))
            {
                GetBouquetSections(dataProvider, worker);
                if (worker.CancellationPending)
                    return (CollectorReply.Cancelled);
            }

            getDSMCCSections(dataProvider, worker);

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
                                        FreeviewChannelInfoDescriptor infoDescriptor = descriptor as FreeviewChannelInfoDescriptor;
                                        if (infoDescriptor != null)
                                        {
                                            if (infoDescriptor.ChannelInfoEntries != null)
                                            {
                                                foreach (FreeviewChannelInfoEntry channelInfoEntry in infoDescriptor.ChannelInfoEntries)
                                                {
                                                    MHEG5Channel channel = new MHEG5Channel();
                                                    channel.OriginalNetworkID = transportStream.OriginalNetworkID;
                                                    channel.TransportStreamID = transportStream.TransportStreamID;
                                                    channel.ServiceID = channelInfoEntry.ServiceID;
                                                    channel.UserChannel = channelInfoEntry.UserNumber;
                                                    channel.Flags = channelInfoEntry.Flags;
                                                    channel.BouquetID = bouquetSection.BouquetID;
                                                    MHEG5Channel.AddChannel(channel);

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

        private void getDSMCCSections(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            int pid = dataProvider.Frequency.DSMCCPid;

            if (pid == 0)
            {
                noMHEGPid = true;
                Logger.Instance.Write("No MHEG5 PID's on frequency " + dataProvider.Frequency);
                return;
            }
            
            Logger.Instance.Write("Collecting MHEG5 data from PID 0x" + pid.ToString("X").ToLowerInvariant(), false, true);

            dataProvider.ChangePidMapping(new int[] { pid });
            
            dsmccReader = new TSStreamReader(500, dataProvider.BufferAddress);
            dsmccReader.Run();

            while (!checkAllDataLoaded())
            {
                if (worker.CancellationPending)
                    return;

                Thread.Sleep(2000);
                Logger.Instance.Write(".", false, false);

                Collection<Mpeg2Section> sections = new Collection<Mpeg2Section>();

                dsmccReader.Lock("LoadMessages");
                if (dsmccReader.Sections.Count != 0)
                {
                    foreach (Mpeg2Section section in dsmccReader.Sections)
                        sections.Add(section);
                    dsmccReader.Sections.Clear();
                }
                dsmccReader.Release("LoadMessages");

                foreach (Mpeg2Section section in sections)
                {
                    switch (section.Table)
                    {
                        case 0x3b:
                            processControlSection(section);
                            break;
                        case 0x3c:
                            processDataSection(section);
                            break;
                        default:
                            break;
                    }
                }
            }

            Logger.Instance.Write("", true, false);

            Logger.Instance.Write("Stopping reader for frequency " + dataProvider.Frequency + " PID 0x" + pid.ToString("X").ToLowerInvariant());
            dsmccReader.Stop();
            
            int totalBlocks = 0;
            foreach (DSMCCModule module in modules)
            {
                module.LogMessage();
                totalBlocks += module.Blocks.Count;
            }

            Logger.Instance.Write("Data blocks: " + totalBlocks + " buffer space used: " + dataProvider.BufferSpaceUsed + " discontinuities: " + dsmccReader.Discontinuities); 
        }

        private bool checkAllDataLoaded()
        {
            if (dsiMessage == null || diiMessages == null || diiMessages.Count == 0)
            {
                if (RunParameters.Instance.TraceIDs.Contains("DSMCCCOMPLETE"))
                {
                    int dsiCount = 0;
                    int diiCount = 0;

                    if (dsiMessage != null)
                        dsiCount++;
                    if (diiMessages != null)
                        diiCount = diiMessages.Count;

                    Logger.Instance.Write("DSMCC: DSI count: " + dsiCount + " DII count: " + diiCount);                    
                }
                return (false);
            }

            int foundCount = 0;
            int missingCount = 0;

            foreach (DSMCCDownloadInfoIndication diiMessage in diiMessages)
            {
                foreach (DSMCCDownloadInfoIndicationModule infoModule in diiMessage.ModuleList)
                {
                    bool found = false;

                    foreach (DSMCCModule module in modules)
                    {
                        if (module.ModuleID == infoModule.ModuleID)
                            found = true;
                    }

                    if (found)
                        foundCount++;
                    else
                        missingCount++;
                }
            }

            if (RunParameters.Instance.TraceIDs.Contains("DSMCCCOMPLETE"))
                Logger.Instance.Write("DSMCC: Found: " + foundCount + " Missing: " + missingCount);

            int incompleteModules = 0;
            foreach (DSMCCModule module in modules)
            {
                if (!module.Complete)
                    incompleteModules++;
            }
            if (RunParameters.Instance.TraceIDs.Contains("DSMCCCOMPLETE"))
                Logger.Instance.Write("DSMCC: Incomplete modules: " + incompleteModules);

            return (missingCount == 0 && incompleteModules == 0);
        }

        private void processControlSection(Mpeg2Section section)
        {
            Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();

            try
            {
                mpeg2Header.Process(section.Data);
                if (mpeg2Header.Current)
                {
                    DSMCCSection dsmccSection = new DSMCCSection();
                    dsmccSection.Process(section.Data, mpeg2Header);

                    if (dsmccSection.DSMCCMessage as DSMCCDownloadServerInitiate != null)
                        addDSIMessage(dsmccSection.DSMCCMessage as DSMCCDownloadServerInitiate);
                    else
                    {
                        if (dsmccSection.DSMCCMessage as DSMCCDownloadInfoIndication != null)
                            addDIIMessage(dsmccSection.DSMCCMessage as DSMCCDownloadInfoIndication);
                        else
                        {
                            if (dsmccSection.DSMCCMessage as DSMCCDownloadCancel != null)
                            {
                                Logger.Instance.Write("DSMCC Cancel message received: reloading all data");
                                dsiMessage = null;
                                diiMessages = null;
                                modules = null;
                            }
                        }
                    }
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Error processing DSMCC control message: " + e.Message);
            }
        }

        private void processDataSection(Mpeg2Section section)
        {
            Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();

            try
            {
                mpeg2Header.Process(section.Data);
                if (mpeg2Header.Current)
                {
                    DSMCCSection dsmccSection = new DSMCCSection();
                    dsmccSection.Process(section.Data, mpeg2Header);

                    if (dsmccSection.DSMCCMessage as DSMCCDownloadDataBlock != null)
                        addDDBMessage(dsmccSection.DSMCCMessage as DSMCCDownloadDataBlock);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Error processing DSMCC data message: " + e.Message);
            }
        }

        private bool addDSIMessage(DSMCCDownloadServerInitiate newMessage)
        {
            if (dsiMessage == null)
            {
                dsiMessage = newMessage;
                dsiMessage.LogMessage();
                return (true); ;
            }

            if (dsiMessage.DSMCCHeader.TransactionID.Identification == newMessage.DSMCCHeader.TransactionID.Identification)
            {
                if (dsiMessage.DSMCCHeader.TransactionID.Version == newMessage.DSMCCHeader.TransactionID.Version)
                    return (false);
                else
                {
                    dsiMessage = newMessage;
                    dsiMessage.LogMessage();
                }
            }
            else
            {
                dsiMessage = newMessage;
                dsiMessage.LogMessage();
            }

            return (true);
        }

        private bool addDIIMessage(DSMCCDownloadInfoIndication newMessage)
        {
            foreach (DSMCCDownloadInfoIndication oldMessage in diiMessages)
            {
                if (oldMessage.DownloadID == newMessage.DownloadID)
                {
                    if (oldMessage.DSMCCHeader.TransactionID.Identification == newMessage.DSMCCHeader.TransactionID.Identification)
                    {
                        if (oldMessage.DSMCCHeader.TransactionID.Version == newMessage.DSMCCHeader.TransactionID.Version)
                            return (false);
                        else
                        {
                            if (Logger.ProtocolLogger != null)
                                Logger.ProtocolLogger.Write("DII Message version change (" +
                                    oldMessage.DSMCCHeader.TransactionID.Version + " -> " +
                                    newMessage.DSMCCHeader.TransactionID.Version + ") - removing modules");
                            diiMessages.Remove(oldMessage);
                            removeModules(oldMessage);
                            diiMessages.Add(newMessage);
                            newMessage.LogMessage();
                            addModules(newMessage);
                            return (true);
                        }
                    }
                    else
                    {
                        diiMessages.Add(newMessage);
                        newMessage.LogMessage();
                        addModules(newMessage);
                        return (true);
                    }
                }
            }

            diiMessages.Add(newMessage);
            newMessage.LogMessage();
            addModules(newMessage);

            return (true);
        }

        private void removeModules(DSMCCDownloadInfoIndication diiMessage)
        {
            foreach (DSMCCDownloadInfoIndicationModule module in diiMessage.ModuleList)
                checkRemoveModule(module);
        }

        private void checkRemoveModule(DSMCCDownloadInfoIndicationModule module)
        {
            foreach (DSMCCModule existingModule in modules)
            {
                if (existingModule.ModuleID == module.ModuleID)
                {
                    modules.Remove(existingModule);
                    return;
                }
            }
        }

        private void addModules(DSMCCDownloadInfoIndication diiMessage)
        {
            foreach (DSMCCDownloadInfoIndicationModule module in diiMessage.ModuleList)
                checkAddModule(module);
        }

        private void checkAddModule(DSMCCDownloadInfoIndicationModule module)
        {
            foreach (DSMCCModule oldModule in modules)
            {
                if (oldModule.ModuleID == module.ModuleID)
                {
                    if (oldModule.Version == module.ModuleVersion)
                        return;
                    else
                    {
                        if (oldModule.Version != module.ModuleVersion)
                        {
                            int replaceIndex = modules.IndexOf(oldModule);
                            modules.Remove(oldModule);
                            DSMCCModule replaceModule = new DSMCCModule(module.ModuleID, module.ModuleVersion, module.ModuleSize, module.OriginalSize);
                            if (replaceIndex != modules.Count)
                                modules.Insert(replaceIndex, replaceModule);
                            else
                                modules.Add(replaceModule);
                            return;
                        }
                    }
                }
                else
                {
                    if (oldModule.ModuleID > module.ModuleID)
                    {
                        DSMCCModule insertModule = new DSMCCModule(module.ModuleID, module.ModuleVersion, module.ModuleSize, module.OriginalSize);
                        modules.Insert(modules.IndexOf(oldModule), insertModule);
                        return;
                    }
                }
            }

            DSMCCModule newModule = new DSMCCModule(module.ModuleID, module.ModuleVersion, module.ModuleSize, module.OriginalSize);
            modules.Add(newModule);
        }

        private bool addDDBMessage(DSMCCDownloadDataBlock downloadDataBlock)
        {
            downloadDataBlock.LogMessage();

            foreach (DSMCCModule module in modules)
            {
                if (module.ModuleID == downloadDataBlock.ModuleID && module.Version == downloadDataBlock.ModuleVersion)
                    return (module.AddBlock(downloadDataBlock));
            }

            return (false);
        }

        /// <summary>
        /// Create the EPG entries.
        /// </summary>
        public override void FinishFrequency()
        {
            if (noMHEGPid)
                return;

            if (RunParameters.Instance.TraceIDs.Contains("DSMCCMODULES"))
                logModules();

            if (RunParameters.Instance.DebugIDs.Contains("LOGTITLES") && titleLogger == null)
                titleLogger = new Logger("EPG Titles.log");
            if (RunParameters.Instance.DebugIDs.Contains("LOGDESCRIPTIONS") && descriptionLogger == null)
                descriptionLogger = new Logger("EPG Descriptions.log");

            if (RunParameters.Instance.CountryCode == null)
            {
                processEPGforNZL();
                logChannelInfo();
                return;
            }

            switch (RunParameters.Instance.CountryCode)
            {
                case "NZL":
                    processEPGforNZL();
                    break;
                case "AUS":
                    processEPGforAUS();
                    break;
                default:
                    break;
            }

            logChannelInfo();
        }

        private void processEPGforNZL()
        {
            if (dsiMessage == null)
                return;
            if (dsiMessage.ServiceGatewayInfo == null)
                return;
            if (dsiMessage.ServiceGatewayInfo.IOPIOR == null)
                return;
            if (dsiMessage.ServiceGatewayInfo.IOPIOR.TaggedProfiles == null)
                return;
            if (dsiMessage.ServiceGatewayInfo.IOPIOR.TaggedProfiles[0].ProfileBody == null)
                return;

            int serviceGatewayModuleID = dsiMessage.ServiceGatewayInfo.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ModuleID;
            byte[] serviceGatewayObjectKey = dsiMessage.ServiceGatewayInfo.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ObjectKeyData;

            BIOPServiceGatewayMessage serviceGateway = findObject(serviceGatewayModuleID, serviceGatewayObjectKey) as BIOPServiceGatewayMessage;
            if (serviceGateway == null)
                return;

            if (serviceGateway.Bindings == null)
                return;

            if (RunParameters.Instance.TraceIDs.Contains("DSMCCDIRLAYOUT"))
                logDirectoryStructure(serviceGateway.Bindings);

            BIOPDirectoryMessage epgDirectory = findObject(serviceGateway.Bindings, "epg", "dir") as BIOPDirectoryMessage;
            if (epgDirectory == null)
                return;

            BIOPDirectoryMessage dataDirectory = findObject(epgDirectory.Bindings, "data", "dir") as BIOPDirectoryMessage;
            if (dataDirectory == null)
                return;

            if (dataDirectory.Bindings == null)
                return;

            foreach (BIOPBinding dateBinding in dataDirectory.Bindings)
            {
                if (dateBinding.Names[0].Kind == "dir")
                {
                    if (dateBinding.IOPIOR != null)
                    {
                        int dateModuleID = dateBinding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ModuleID;
                        byte[] dateObjectKey = dateBinding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ObjectKeyData;
                        BIOPDirectoryMessage dateDirectory = findObject(dateModuleID, dateObjectKey) as BIOPDirectoryMessage;
                        if (dateDirectory != null)
                        {
                            if (dateDirectory.Bindings != null)
                            {
                                foreach (BIOPBinding fileBinding in dateDirectory.Bindings)
                                {
                                    if (fileBinding.Names[0].Kind == "fil")
                                    {
                                        if (fileBinding.IOPIOR != null)
                                        {
                                            int fileModuleID = fileBinding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ModuleID;
                                            byte[] fileObjectKey = fileBinding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ObjectKeyData;
                                            BIOPFileMessage epgFile = findObject(fileModuleID, fileObjectKey) as BIOPFileMessage;
                                            if (epgFile != null)
                                            {
                                                try
                                                {
                                                    processEPGFile(dateBinding.Names[0].Identity, fileBinding.Names[0].Identity, epgFile);
                                                }
                                                catch (ArgumentOutOfRangeException e)
                                                {
                                                    Logger.Instance.Write("Failed to process DSMCC file: " + e.Message);
                                                    modules.Clear();
                                                }
                                                catch (IndexOutOfRangeException e)
                                                {
                                                    Logger.Instance.Write("Failed to process DSMCC file: " + e.Message);
                                                    throw;
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

            combineMidnightPrograms();

            if (RunParameters.Instance.Options.Contains("USEIMAGE"))
            {
                BIOPDirectoryMessage pngsDirectory = findObject(epgDirectory.Bindings, "pngs", "dir") as BIOPDirectoryMessage;
                if (pngsDirectory == null)
                    return;

                if (pngsDirectory.Bindings == null)
                    return;

                foreach (BIOPBinding pngBinding in pngsDirectory.Bindings)
                {
                    if (pngBinding.Names[0].Kind == "fil")
                    {
                        if (pngBinding.IOPIOR != null)
                        {
                            int pngModuleID = pngBinding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ModuleID;
                            byte[] pngObjectKey = pngBinding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ObjectKeyData;
                            BIOPFileMessage pngFile = findObject(pngModuleID, pngObjectKey) as BIOPFileMessage;
                            if (pngFile != null)
                            {
                                if (pngBinding.Names[0].Identity[0] >= '0' && pngBinding.Names[0].Identity[0] <= '9')
                                {
                                    if (RunParameters.Instance.TraceIDs.Contains("PNGNAMES"))
                                        Logger.Instance.Write("BIOP PNG File Name = " + pngBinding.Names[0].Identity);

                                    try
                                    {
                                        if (!Directory.Exists(Path.Combine(RunParameters.DataDirectory, "Images") + Path.DirectorySeparatorChar))
                                            Directory.CreateDirectory(Path.Combine(RunParameters.DataDirectory, "Images") + Path.DirectorySeparatorChar);
                                        string outFile = RunParameters.DataDirectory + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar + pngBinding.Names[0].Identity;
                                        FileStream outFileStream = new FileStream(outFile, FileMode.Create);
                                        outFileStream.Write(pngFile.ContentData, 0, pngFile.ContentData.Length);
                                        outFileStream.Close();
                                    }
                                    catch (IOException e)
                                    {
                                        Logger.Instance.Write("Failed to process PNG DSMCC file: " + e.Message);
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void processEPGforAUS()
        {
            if (dsiMessage == null)
                return;
            if (dsiMessage.ServiceGatewayInfo == null)
                return;
            if (dsiMessage.ServiceGatewayInfo.IOPIOR == null)
                return;
            if (dsiMessage.ServiceGatewayInfo.IOPIOR.TaggedProfiles == null)
                return;
            if (dsiMessage.ServiceGatewayInfo.IOPIOR.TaggedProfiles[0].ProfileBody == null)
                return;

            int serviceGatewayModuleID = dsiMessage.ServiceGatewayInfo.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ModuleID;
            byte[] serviceGatewayObjectKey = dsiMessage.ServiceGatewayInfo.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ObjectKeyData;

            BIOPServiceGatewayMessage serviceGateway = findObject(serviceGatewayModuleID, serviceGatewayObjectKey) as BIOPServiceGatewayMessage;
            if (serviceGateway == null)
                return;

            if (serviceGateway.Bindings == null)
                return;

            if (RunParameters.Instance.TraceIDs.Contains("DSMCCDIRLAYOUT"))
                logDirectoryStructure(serviceGateway.Bindings);

            BIOPDirectoryMessage epgDirectory = findObject(serviceGateway.Bindings, "e1", "dir") as BIOPDirectoryMessage;
            if (epgDirectory == null)
            {
                Logger.Instance.Write("EPG base directory 'is missing - data cannot be processed");
                return;
            }

            BIOPFileMessage serviceInfo = findObject(epgDirectory.Bindings, "serviceinfo.txt", "fil") as BIOPFileMessage;
            if (serviceInfo == null)
            {
                Logger.Instance.Write("Service Info file is missing - data cannot be processed");
                return;
            }

            Collection<ServiceEntry> serviceEntries = createServiceEntries(serviceInfo.ContentData);
            if (serviceEntries == null || serviceEntries.Count == 0)
            {
                Logger.Instance.Write("Service Info file is empty or in the wrong format - data cannot be processed");
                return;
            }

            foreach (TVStation station in TVStation.StationCollection)
            {
                if (!station.Excluded)
                    processStation(station, serviceEntries, serviceGateway.Bindings);
            }

            combineMidnightPrograms();
        }

        private void processStation(TVStation station, Collection<ServiceEntry> serviceEntries, Collection<BIOPBinding> bindings)
        {
            ServiceEntry serviceEntry = findServiceEntry(station, serviceEntries);
            if (serviceEntry == null)
            {
                Logger.Instance.Write("Station " + station.Name + " (" + station.FullID + ") not in service info file - data cannot be processed");
                return;
            }

            BIOPDirectoryMessage epgDirectory = findObject(bindings, "e1", "dir") as BIOPDirectoryMessage;
            processDay(epgDirectory, 1, serviceEntry);

            epgDirectory = findObject(bindings, "e2-8", "dir") as BIOPDirectoryMessage;
            if (epgDirectory == null)
                return;

            for (int day = 2; day < 9; day++)
            {
                processDay(epgDirectory, day, serviceEntry);
                /*BIOPDirectoryMessage dayDirectory = findObject(epgDirectory.Bindings, day.ToString(), "dir") as BIOPDirectoryMessage;
                if (dayDirectory == null)
                    return;

                if (dayDirectory.Bindings == null)
                    return;

                foreach (BIOPBinding stationBinding in dayDirectory.Bindings)
                {
                    if (stationBinding.Names[0].Kind == "fil")
                    {
                        if (stationBinding.IOPIOR != null)
                        {
                            int fileModuleID = stationBinding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ModuleID;
                            byte[] fileObjectKey = stationBinding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ObjectKeyData;
                            BIOPFileMessage epgFile = findObject(fileModuleID, fileObjectKey) as BIOPFileMessage;
                            if (epgFile != null)
                            {
                                try
                                {
                                    string[] nameParts = stationBinding.Names[0].Identity.Split(new char[] { '_' });
                                    if (nameParts.Length == 3)
                                        processEPGFile(day, nameParts[2], epgFile);
                                    processEPGFile(day, nameParts[0], epgFile);
                                }
                                catch (ArgumentOutOfRangeException e)
                                {
                                    Logger.Instance.Write("Failed to process DSMCC file: " + e.Message);
                                    modules.Clear();
                                }
                                catch (IndexOutOfRangeException e)
                                {
                                    Logger.Instance.Write("Failed to process DSMCC file: " + e.Message);
                                    throw;
                                }
                            }
                        }
                    }
                }*/
            }
        }

        private Collection<ServiceEntry> createServiceEntries(byte[] contentData)
        {
            Collection<byte[]> contentFields = Utils.SplitBytes(contentData, 0x1d);

            if (contentFields[0].Length != 0 || contentFields[1].Length != 0)
            {
                Logger.Instance.Write("Service Info data is in the wrong format (1) - cannot be processed");
                return(null);
            }

            int count = 0;

            try
            {
                count = Int32.Parse(Utils.GetAsciiString(contentFields[2]));
            }
            catch (ArgumentException)
            {
                Logger.Instance.Write("Service Info data is in the wrong format (2) - cannot be processed");
                return (null);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("Service Info data is in the wrong format (2) - cannot be processed");
                return (null);
            }

            int entryIndex = count + 3;
            
            Collection<ServiceEntry> serviceEntries = new Collection<ServiceEntry>();

            while (entryIndex < contentFields.Count)
            {
                ServiceEntry serviceEntry = new ServiceEntry(Utils.GetAsciiString(contentFields[entryIndex]),
                    Utils.GetAsciiString(contentFields[entryIndex + 1]),
                    Utils.GetAsciiString(contentFields[entryIndex + 2]),
                    Utils.GetAsciiString(contentFields[entryIndex + 3]));

                serviceEntries.Add(serviceEntry);

                entryIndex += 4;
            }

            return (serviceEntries);
        }

        private ServiceEntry findServiceEntry(TVStation station, Collection<ServiceEntry> serviceEntries)
        {
            foreach (ServiceEntry serviceEntry in serviceEntries)
            {
                if (serviceEntry.OriginalNetworkID == station.OriginalNetworkID &&
                    serviceEntry.TransportStreamID == station.TransportStreamID &&
                    serviceEntry.ServiceID == station.ServiceID)
                    return (serviceEntry);
            }

            return (null);
        }

        private void processDay(BIOPDirectoryMessage epgDirectory, int day, ServiceEntry serviceEntry)
        {
            BIOPDirectoryMessage dayDirectory = findObject(epgDirectory.Bindings, day.ToString(), "dir") as BIOPDirectoryMessage;
            if (dayDirectory == null)
                return;

            BIOPFileMessage epgFile = findObject(dayDirectory.Bindings, serviceEntry.ReferenceNumber, "fil") as BIOPFileMessage;
            if (epgFile == null)
                return;

            processEPGFile(day, serviceEntry.ServiceID.ToString(), epgFile);                                
        }

        private void logModules()
        {
            foreach (DSMCCModule module in modules)
            {
                Logger.Instance.Write("Module: 0x" + module.ModuleID.ToString("X"));

                if (module.Objects != null)
                {
                    foreach (BIOPMessage objectEntry in module.Objects)
                    {
                        Logger.Instance.Write("  Object: " + objectEntry.Kind + " key " + Utils.ConvertToHex(objectEntry.ObjectKeyData));

                        BIOPDirectoryMessage directoryMessage = objectEntry.MessageDetail as BIOPDirectoryMessage;
                        if (directoryMessage != null)
                            logDirectoryStructure(directoryMessage.Bindings);                        
                    }
                }
            }
        }

        private void logDirectoryStructure(Collection<BIOPBinding> bindings)
        {
            prefix += "    ";

            if (bindings == null)
            {
                Logger.Instance.Write(prefix + "No bindings");
                return;
            }

            foreach (BIOPBinding binding in bindings)
            {
                Logger.Instance.Write(prefix + "Binding: " + binding.Names[0].Identity + " Kind: " + binding.Names[0].Kind);
                if (binding.Names[0].Kind == "dir")
                {
                    if (binding.IOPIOR != null)
                    {
                        int moduleID = binding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ModuleID;
                        byte[] objectKey = binding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ObjectKeyData;
                        Logger.Instance.Write("Searching for module 0x" + moduleID.ToString("X") + 
                            " object key " + Utils.ConvertToHex(objectKey));
                        BIOPDirectoryMessage directory = findObject(moduleID, objectKey) as BIOPDirectoryMessage;
                        if (directory != null)
                        {
                            if (directory.Bindings != null)
                                logDirectoryStructure(directory.Bindings);
                            else
                                Logger.Instance.Write(prefix + "Directory has no bindings");
                        }
                        else
                            Logger.Instance.Write(prefix + "Failed to find directory");
                    }
                }
            }

            prefix = prefix.Substring(4);
        }

        private BIOPMessageDetail findObject(Collection<BIOPBinding> bindings, string identity, string kind)
        {
            if (bindings == null)
                return (null);

            foreach (BIOPBinding binding in bindings)
            {
                if (binding.Names[0].Identity == identity && binding.Names[0].Kind == kind)
                {
                    if (binding.IOPIOR == null)
                        return (null);

                    int moduleID = binding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ModuleID;
                    byte[] objectKey = binding.IOPIOR.TaggedProfiles[0].ProfileBody.ObjectLocation.ObjectKeyData;

                    return (findObject(moduleID, objectKey));
                }
            }

            return (null);
        }

        private BIOPMessageDetail findObject(int moduleID, byte[] objectKey)
        {
            foreach (DSMCCModule module in modules)
            {
                if (module.ModuleID == module.ModuleID && module.Complete)
                {
                    if (module.Objects != null)
                    {
                        foreach (BIOPMessage objectEntry in module.Objects)
                        {
                            if (Utils.CompareBytes(objectEntry.ObjectKeyData, objectKey))
                                return (objectEntry.MessageDetail);
                        }
                    }
                }
            }

            return (null);
        }

        private void processEPGFile(string dateString, string serviceID, BIOPFileMessage epgFile)
        {
            try
            {
                DateTime date = DateTime.ParseExact(dateString, "yyyyMMdd", null);
                processDSMCCEPGFile(date, Int32.Parse(serviceID), epgFile.ContentData, Logger.ProtocolLogger);
            }
            catch (FormatException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC file could not be processed"));
            }
        }

        private void processEPGFile(int dayNumber, string serviceID, BIOPFileMessage epgFile)
        {
            try
            {
                DateTime date = DateTime.Today + new TimeSpan(dayNumber - 1, 0, 0, 0);
                processDSMCCEPGFile(date, Int32.Parse(serviceID), epgFile.ContentData, Logger.ProtocolLogger);
            }
            catch (FormatException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC file could not be processed"));
            }
            catch (ArgumentException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC file could not be processed"));
            }
            catch (ArithmeticException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC file could not be processed"));
            }
        }

        private void processDSMCCEPGFile(DateTime date, int serviceID, byte[] fileData, Logger logger)
        {
            if (RunParameters.Instance.CountryCode == null)
            {
                processEPGFileForNZL(date, serviceID, fileData, logger);
                return;
            }

            switch (RunParameters.Instance.CountryCode)
            {
                case "NZL":
                    processEPGFileForNZL(date, serviceID, fileData, logger);
                    break;
                case "AUS":
                    processEPGFileForAUS(date, serviceID, fileData, logger);
                    break;
                default:
                    break;
            }
        }

        private void processEPGFileForNZL(DateTime date, int serviceID, byte[] fileData, Logger logger)
        {
            if (logger != null && RunParameters.Instance.TraceIDs.Contains("DSMCCFILE"))
                logger.Dump("DSMCC Parser data - File Entry", fileData, fileData.Length);

            if (RunParameters.Instance.TraceIDs.Contains("DSMCCRECLAYOUT"))
            {
                Collection<byte[]> logRecords = Utils.SplitBytes(fileData, 0x1c);
                Logger.Instance.Write("Block contains " + logRecords.Count + " records");
                foreach (byte[] logRecord in logRecords)
                {
                    Collection<byte[]> logFields = Utils.SplitBytes(fileData, 0x1d);
                    Logger.Instance.Write("Record contains " + logFields.Count + " fields");
                    foreach (byte[] logField in logFields)
                        Logger.Instance.Write("    Field: " + Utils.GetAsciiString(logField));
                }
            }

            TVStation tvStation = TVStation.FindStation(serviceID);
            if (tvStation == null)
                return;

            bool process = checkChannelMapping(tvStation);
            if (!process)
                return;

            int rootCRIDFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.RootCRID);
            if (rootCRIDFieldNumber == -1)
                rootCRIDFieldNumber = 3;

            int programCountFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.ProgramCount);
            if (programCountFieldNumber == -1)
                programCountFieldNumber = 4;

            int eventIDFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.EventID);
            if (eventIDFieldNumber == -1)
                eventIDFieldNumber = 0;

            int startTimeFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.StartTime);
            if (startTimeFieldNumber == -1)
                startTimeFieldNumber = 1;

            int endTimeFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.EndTime);
            if (endTimeFieldNumber == -1)
                endTimeFieldNumber = 2;

            int eventCRIDFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.ProgramCRID);
            if (eventCRIDFieldNumber == -1)
                eventCRIDFieldNumber = 6;

            int eventNameFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.EventName);
            if (eventNameFieldNumber == -1)
                eventNameFieldNumber = 7;

            int shortDescriptionFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.ShortDescription);
            if (shortDescriptionFieldNumber == -1)
                shortDescriptionFieldNumber = 8;

            int imageCountFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.ImageCount);
            if (imageCountFieldNumber == -1)
                imageCountFieldNumber = 9;

            int seriesCRIDFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.SeriesCRID);
            if (seriesCRIDFieldNumber == -1)
                seriesCRIDFieldNumber = 2;

            try
            {
                Collection<byte[]> records = Utils.SplitBytes(fileData, 0x1c);

                for (int index = 0; index < records.Count; index++)
                {
                    Collection<byte[]> headerFields = Utils.SplitBytes(records[index], 0x1d);

                    int expectedHeaderFieldCount = MHEGParserParameters.HeaderFields;
                    if (expectedHeaderFieldCount != -1 && expectedHeaderFieldCount != headerFields.Count)
                        throw (new IndexOutOfRangeException(
                            "MHEG format error - count of header fields is incorrect - expected " +
                            expectedHeaderFieldCount + " got " + headerFields.Count));

                    // These fields are not used
                    // byte[] programNumber = headerFields[0];
                    // string friendlyDate = Utils.GetString(headerFields[1]);
                    // string stationName = Utils.GetString(headerFields[2]);

                    string rootCRID = Utils.GetAsciiString(headerFields[rootCRIDFieldNumber]);
                    int programCount = Int32.Parse(Utils.GetAsciiString(headerFields[programCountFieldNumber]));

                    while (programCount > 0)
                    {
                        index++;

                        if (logger != null && RunParameters.Instance.TraceIDs.Contains("DSMCCRECORD"))
                            logger.Dump("DSMCC Parser data - Program Entry", records[index], records[index].Length);

                        Collection<byte[]> dataFields = Utils.SplitBytes(records[index], 0x1d);

                        try
                        {
                            if (dataFields[1].Length != 0)
                            {
                                EPGEntry epgEntry = new EPGEntry();
                                epgEntry.OriginalNetworkID = tvStation.OriginalNetworkID;
                                epgEntry.TransportStreamID = tvStation.TransportStreamID;
                                epgEntry.ServiceID = serviceID;

                                epgEntry.EventID = Int32.Parse(Utils.GetAsciiString(headerFields[eventIDFieldNumber]));
                                epgEntry.StartTime = Utils.RoundTime(date.AddSeconds(((double)Int32.Parse(Utils.GetAsciiString(dataFields[startTimeFieldNumber])))));
                                epgEntry.Duration = Utils.RoundTime(date.AddSeconds(((double)Int32.Parse(Utils.GetAsciiString(dataFields[endTimeFieldNumber])))) - epgEntry.StartTime);

                                // These fields are not used
                                // byte[] titleLineCount = dataFields[3];
                                // byte[] friendlyTime = dataFields[4];
                                // byte[] entryType = dataFields[5];

                                string eventCRID = Utils.GetAsciiString(dataFields[eventCRIDFieldNumber]);

                                byte[] editedEventName = replaceByte(dataFields[eventNameFieldNumber], 0x0d, 0x20);
                                string eventName = Utils.GetString(editedEventName, "utf-8");
                                epgEntry.EventName = Utils.Compact(eventName);

                                byte[] editedDescription = replaceByte(dataFields[shortDescriptionFieldNumber], 0x0d, 0x20);
                                string eventDescription = Utils.GetString(editedDescription, "utf-8");
                                processNZLShortDescription(epgEntry, Utils.Compact(eventDescription));

                                int iconCount = Int32.Parse(Utils.GetAsciiString(dataFields[imageCountFieldNumber]));

                                if (iconCount < 0 || iconCount > 10)
                                {
                                    Logger.Instance.Dump("DSMCC Parser error - Icon Count - File Entry", fileData, fileData.Length);
                                    if (logger != null)
                                        logger.Dump("DSMCC Parser error - Icon Count - File Entry", fileData, fileData.Length);
                                }
                                else
                                {
                                    int imageIndex;

                                    for (imageIndex = imageCountFieldNumber + 1; imageIndex < iconCount + imageCountFieldNumber + 1; imageIndex++)
                                    {
                                        switch (Utils.GetAsciiString(dataFields[imageIndex]))
                                        {
                                            case "/pngs/ao.png":
                                                epgEntry.ParentalRating = "AO";
                                                epgEntry.MpaaParentalRating = ParentalRating.FindMpaaRating("NZL", "MHEG5", "AO");
                                                break;
                                            case "/pngs/pgr.png":
                                                epgEntry.ParentalRating = "PGR";
                                                epgEntry.MpaaParentalRating = ParentalRating.FindMpaaRating("NZL", "MHEG5", "PGR");
                                                break;
                                            case "/pngs/g.png":
                                                epgEntry.ParentalRating = "G";
                                                epgEntry.MpaaParentalRating = ParentalRating.FindMpaaRating("NZL", "MHEG5", "G");
                                                break;
                                            case "/pngs/ear.png":
                                                epgEntry.SubTitles = "teletext";
                                                break;
                                            case "/pngs/hd.png":
                                                epgEntry.VideoQuality = "HDTV";
                                                break;
                                            case "/pngs/dolby.png":
                                                epgEntry.AudioQuality = "dolby digital";
                                                break;
                                            default:
                                                break;
                                        }

                                    }

                                    int expectedDetailFieldCount = MHEGParserParameters.DetailFields;
                                    int addedDetailFieldCount = iconCount;

                                    string seriesCRID = string.Empty;

                                    if (Int32.Parse(Utils.GetAsciiString(dataFields[imageIndex])) != 0)
                                    {
                                        // This field is not used.
                                        // string entryType2 = Utils.GetString(dataFields[imageIndex + 1]);

                                        seriesCRID = Utils.GetAsciiString(dataFields[imageIndex + seriesCRIDFieldNumber]);

                                        // These fields are not used
                                        // string eventName2 = Utils.GetString(dataFields[imageIndex + 3]);                                        
                                        // string shortDescription2 = Utils.GetString(dataFields[imageIndex + 4]);
                                        // int otherIconCount = Int32.Parse(Utils.GetString(dataFields[imageIndex + 5]));

                                        addedDetailFieldCount += 6;
                                    }
                                    else
                                        addedDetailFieldCount += 2;

                                    if (expectedDetailFieldCount != -1 && expectedDetailFieldCount + addedDetailFieldCount != dataFields.Count)
                                        throw (new IndexOutOfRangeException(
                                            "MHEG format error - count of detail fields is incorrect - expected " +
                                            (expectedDetailFieldCount + addedDetailFieldCount) + " got " + dataFields.Count));

                                    processNZLCRID(epgEntry, seriesCRID, eventCRID);

                                    epgEntry.EventCategory = CustomProgramCategory.FindCategoryDescription(epgEntry.EventName);
                                    if (epgEntry.EventCategory == null)
                                        epgEntry.EventCategory = CustomProgramCategory.FindCategoryDescription(epgEntry.ShortDescription);

                                    tvStation.AddEPGEntry(epgEntry);

                                    if (titleLogger != null)
                                        logTitle(eventName, epgEntry, titleLogger);
                                    if (descriptionLogger != null)
                                        logDescription(eventDescription, epgEntry, descriptionLogger);
                                }
                            }

                            programCount--;
                        }
                        catch (ArithmeticException)
                        {
                            Logger.Instance.Dump("DSMCC Parser error - Arithmetic Exception - File Entry", fileData, fileData.Length);
                            if (logger != null)
                                logger.Dump("DSMCC Parser error - Arithmetic Exception - File Entry", fileData, fileData.Length);
                        }
                    }
                }
            }
            catch (ArithmeticException e)
            {
                throw (new ArgumentOutOfRangeException("DSMCC file entry parsing failed: " + e.Message));
            }
        }

        private byte[] replaceByte(byte[] inputBytes, byte oldValue, byte newValue)
        {
            byte[] outputBytes = new byte[inputBytes.Length];

            for (int index = 0; index < inputBytes.Length; index++)
            {
                if (inputBytes[index] == oldValue)
                    outputBytes[index] = newValue;
                else
                    outputBytes[index] = inputBytes[index];
            }

            return (outputBytes);
        }

        private void processNZLShortDescription(EPGEntry epgEntry, string description)
        {
            if (!description.StartsWith("'"))
            {
                epgEntry.ShortDescription = description;
                return;
            }

            int endIndex = description.IndexOf("'. ");
            if (endIndex == -1)
            {
                epgEntry.ShortDescription = description;
                return;
            }

            if (endIndex + 3 >= description.Length)
            {
                epgEntry.ShortDescription = description;
                return;
            }

            epgEntry.ShortDescription = description.Substring(endIndex + 3);
            epgEntry.EventSubTitle = description.Substring(1, endIndex - 1);
        }

        private void processNZLCRID(EPGEntry epgEntry, string seriesCRID, string episodeCRID)
        {
            if (RunParameters.Instance.Options.Contains("USERAWCRID"))
            {
                processNZLCRIDRaw(epgEntry, seriesCRID, episodeCRID);
                return;
            }

            if (RunParameters.Instance.Options.Contains("USEBSEPG"))
            {
                processNZLCRIDBSEPG(epgEntry, seriesCRID, episodeCRID);
                return;
            }

            processNZLCRIDNumber(epgEntry, seriesCRID, episodeCRID);            
        }

        private void processNZLCRIDRaw(EPGEntry epgEntry, string seriesCRID, string episodeCRID)
        {
            epgEntry.Series = seriesCRID;
            epgEntry.Episode = episodeCRID;
            
            epgEntry.EpisodeSystemType = "raw_crid";
            epgEntry.EpisodeSystemParts = 3;
        }

        private void processNZLCRIDNumber(EPGEntry epgEntry, string seriesCRID, string episodeCRID)
        {
            epgEntry.Series = getCRIDNumber(seriesCRID);                
            epgEntry.Episode = getCRIDNumber(episodeCRID);

            if (epgEntry.Series != null || epgEntry.Episode != null)
            {
                epgEntry.EpisodeSystemType = "xmltv_ns";
                epgEntry.EpisodeSystemParts = 3;
            }
        }

        private string getCRIDNumber(string cridText)
        {
            if (cridText.Trim().Length == 0)
                return (null);

            StringBuilder numericString = new StringBuilder();

            foreach (char cridChar in cridText)
            {
                if (cridChar >= '0' && cridChar <= '9')
                    numericString.Append(cridChar);
            }

            if (numericString.Length != 0)
                return (numericString.ToString());
            else
                return (null);
        }

        private void processNZLCRIDBSEPG(EPGEntry epgEntry, string seriesCRID, string episodeCRID)
        {
            if (seriesCRID.Length != 0)
                epgEntry.Series = "SE-" + seriesCRID.Replace("/", "").Trim();

            if (episodeCRID.Length != 0)
                epgEntry.Episode = "EP-" + episodeCRID.Replace("/", "").Trim();

            if (epgEntry.Series != null || epgEntry.Episode != null)
            {
                epgEntry.EpisodeSystemType = "bsepg-epid";
                epgEntry.EpisodeSystemParts = 2;
            }
        }

        private void processEPGFileForAUS(DateTime date, int serviceID, byte[] fileData, Logger logger)
        {
            /*if (logger != null && RunParameters.Instance.TraceIDs.Contains("DSMCCFILE"))
                logger.Dump("DSMCC Parser data - File Entry", fileData, fileData.Length);*/

            if (RunParameters.Instance.TraceIDs.Contains("DSMCCFILE"))
                Logger.Instance.Dump("DSMCC Parser data - File Entry", fileData, fileData.Length);            

            TVStation tvStation = TVStation.FindStation(serviceID);
            if (tvStation == null)
            {
                if (RunParameters.Instance.DebugIDs.Contains("CREATEDSMCCSTATIONS"))
                {
                    Logger.Instance.Write("No station for service ID: " + serviceID);
                    tvStation = new TVStation("Service ID " + serviceID);
                    tvStation.ServiceID = serviceID;
                    TVStation.StationCollection.Add(tvStation);
                }
                else
                    return;
            }

            bool process = checkChannelMapping(tvStation);
            if (!process)
                return;

            if (RunParameters.Instance.TraceIDs.Contains("DSMCCRECLAYOUT"))
            {
                Collection<byte[]> logRecords = Utils.SplitBytes(fileData, 0x1e);
                Logger.Instance.Write("Block contains " + logRecords.Count + " records");
                foreach (byte[] logRecord in logRecords)
                {
                    Collection<byte[]> logFields = Utils.SplitBytes(fileData, 0x1d);
                    Logger.Instance.Write("Record contains " + logFields.Count + " fields");
                    foreach (byte[] logField in logFields)
                        Logger.Instance.Write("    Field: " + Utils.GetAsciiString(logField));
                }
            }

            /*int rootCRIDFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.RootCRID);
            if (rootCRIDFieldNumber == -1)
                rootCRIDFieldNumber = 0;*/

            int endTimeFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.EndTime);
            if (endTimeFieldNumber == -1)
                endTimeFieldNumber = 0;

            int startTimeFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.StartTime);
            if (startTimeFieldNumber == -1)
                startTimeFieldNumber = 1;

            int eventCRIDFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.ProgramCRID);
            if (eventCRIDFieldNumber == -1)
                eventCRIDFieldNumber = 2;

            int seriesCRIDFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.SeriesCRID);
            if (seriesCRIDFieldNumber == -1)
                seriesCRIDFieldNumber = 3;

            int eventNameFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.EventName);
            if (eventNameFieldNumber == -1)
                eventNameFieldNumber = 4;

            int shortDescriptionFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.ShortDescription);
            if (shortDescriptionFieldNumber == -1)
                shortDescriptionFieldNumber = 5;

            int highDefinitionFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.HighDefinition);
            if (highDefinitionFieldNumber == -1)
                highDefinitionFieldNumber = 6;

            int closedCaptionsFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.ClosedCaptions);
            if (closedCaptionsFieldNumber == -1)
                closedCaptionsFieldNumber = 7;

            int parentalRatingFieldNumber = MHEGParserParameters.GetField(MHEGParserParameters.FieldName.ParentalRating);
            if (parentalRatingFieldNumber == -1)
                parentalRatingFieldNumber = 8;

            try
            {
                Collection<byte[]> records = Utils.SplitBytes(fileData, 0x1e);

                for (int index = 1; index < records.Count; index++)
                {
                    if (logger != null && RunParameters.Instance.TraceIDs.Contains("DSMCCRECORD"))
                        logger.Dump("DSMCC Parser data - Program Entry", records[index], records[index].Length);

                    Collection<byte[]> dataFields = Utils.SplitBytes(records[index], 0x1d);

                    if (dataFields[0].Length != 0)
                    {
                        try
                        {
                            EPGEntry epgEntry = new EPGEntry();
                            epgEntry.OriginalNetworkID = tvStation.OriginalNetworkID;
                            epgEntry.TransportStreamID = tvStation.TransportStreamID;
                            epgEntry.ServiceID = serviceID;

                            epgEntry.StartTime = Utils.RoundTime(date.AddSeconds(((double)Int32.Parse(Utils.GetAsciiString(dataFields[startTimeFieldNumber])))));
                            epgEntry.Duration = Utils.RoundTime(date.AddSeconds(((double)Int32.Parse(Utils.GetAsciiString(dataFields[endTimeFieldNumber])))) - epgEntry.StartTime);
                            string eventCRID = Utils.GetAsciiString(dataFields[eventCRIDFieldNumber]);
                            string seriesCRID = Utils.GetAsciiString(dataFields[seriesCRIDFieldNumber]);

                            byte[] editedEventName = replaceByte(dataFields[eventNameFieldNumber], 0x0d, 0x20);
                            string eventName = Utils.GetString(editedEventName, "utf-8");
                            epgEntry.EventName = Utils.Compact(eventName);

                            byte[] editedDescription = replaceByte(dataFields[shortDescriptionFieldNumber], 0x0d, 0x20);
                            string eventDescription = Utils.GetString(editedDescription, "utf-8");
                            processAUSShortDescription(epgEntry, Utils.Compact(eventDescription));
                                
                            if (Int32.Parse(Utils.GetAsciiString(dataFields[highDefinitionFieldNumber])) == 1)
                            {
                                epgEntry.AspectRatio = "16:9";
                                epgEntry.VideoQuality = "HDTV";
                                epgEntry.AudioQuality = "dolby digital";
                            }

                            if (Int32.Parse(Utils.GetAsciiString(dataFields[closedCaptionsFieldNumber])) == 1)
                                epgEntry.SubTitles = "teletext";

                            string parentalRating = Utils.GetAsciiString(dataFields[parentalRatingFieldNumber]).Replace("(", "").Replace(")", "");
                            if (parentalRating.Length != 0)
                            {
                                epgEntry.ParentalRating = parentalRating;
                                epgEntry.MpaaParentalRating = ParentalRating.FindMpaaRating("AUS", "MHEG5", parentalRating);
                                epgEntry.ParentalRatingSystem = ParentalRating.FindSystem("AUS", "MHEG5", parentalRating);
                            }

                            processAUSCRID(epgEntry, seriesCRID, eventCRID);

                            epgEntry.EventCategory = CustomProgramCategory.FindCategoryDescription(epgEntry.EventName);
                            if (epgEntry.EventCategory == null)
                                epgEntry.EventCategory = CustomProgramCategory.FindCategoryDescription(epgEntry.ShortDescription);

                            tvStation.AddEPGEntry(epgEntry);

                            if (titleLogger != null)
                                logTitle(eventName, epgEntry, titleLogger);
                            if (descriptionLogger != null)
                                logDescription(eventDescription, epgEntry, descriptionLogger);
                        }
                        catch (ArithmeticException)
                        {
                            Logger.Instance.Dump("DSMCC Parser error - Arithmetic Exception - File Entry", fileData, fileData.Length);
                            if (logger != null)
                                logger.Dump("DSMCC Parser error - Arithmetic Exception - File Entry", fileData, fileData.Length);
                        }
                    }
                }
            }
            catch (ArithmeticException e)
            {
                throw (new ArgumentOutOfRangeException("DSMCC file entry parsing failed: " + e.Message));
            }
        }

        private void processAUSShortDescription(EPGEntry epgEntry, string description)
        {
            string editedDescription = removeParentalRating(description);
            editedDescription = removeHDFlag(editedDescription);
            editedDescription = removeClosedCaptionsFlag(editedDescription);

            epgEntry.ShortDescription = editedDescription;            
        }

        private string removeParentalRating(string description)
        {
            if (description[0] != '(')
                return (description);

            int endIndex = description.IndexOf(')');
            if (endIndex == -1)
                return (description);

            if (endIndex + 1 >= description.Length)
                return (description);

            return (description.Substring(endIndex + 1).Trim());
        }

        private string removeHDFlag(string description)
        {
            int startIndex = description.IndexOf("[HD]");
            if (startIndex == -1)
                return (description);

            if (startIndex + 4 >= description.Length)
                return (description);

            return (description.Substring(startIndex + 4).Trim());
        }

        private string removeClosedCaptionsFlag(string description)
        {
            int startIndex = description.IndexOf("[CC]");
            if (startIndex == -1)
                return (description);

            if (startIndex + 4 >= description.Length)
                return (description);

            return (description.Substring(startIndex + 4).Trim());
        }

        private void processAUSCRID(EPGEntry epgEntry, string seriesCRID, string episodeCRID)
        {
            if (RunParameters.Instance.Options.Contains("USERAWCRID"))
            {
                processAUSCRIDRaw(epgEntry, seriesCRID, episodeCRID);
                return;
            }

            if (RunParameters.Instance.Options.Contains("USEBSEPG"))
            {
                processAUSCRIDBSEPG(epgEntry, seriesCRID, episodeCRID);
                return;
            }

            processAUSCRIDNumber(epgEntry, seriesCRID, episodeCRID);
        }

        private void processAUSCRIDRaw(EPGEntry epgEntry, string seriesCRID, string episodeCRID)
        {
            epgEntry.Series = seriesCRID;
            epgEntry.Episode = episodeCRID;
            
            epgEntry.EpisodeSystemType = "raw_crid";
            epgEntry.EpisodeSystemParts = 3;
        }

        private void processAUSCRIDNumber(EPGEntry epgEntry, string seriesCRID, string episodeCRID)
        {
            epgEntry.Series = getCRIDNumber(seriesCRID);
            epgEntry.Episode = getCRIDNumber(episodeCRID);

            if (epgEntry.Series != null || epgEntry.Episode != null)
            {
                epgEntry.EpisodeSystemType = "xmltv_ns";
                epgEntry.EpisodeSystemParts = 3;
            }
        }

        private void processAUSCRIDBSEPG(EPGEntry epgEntry, string seriesCRID, string episodeCRID)
        {
            bool dataSet = false;

            string series = "SE-";
            if (seriesCRID != null && seriesCRID.Length > 1)
            {
                series = series + seriesCRID.Substring(1);
                dataSet = true;
            }

            string episode = "EP-";
            if (episodeCRID != null && episodeCRID.Length > 1)
            {
                episode = episode + episodeCRID.Substring(1);
                dataSet = true;
            }

            if (dataSet)
            {
                epgEntry.Episode = episode;
                epgEntry.Series = series;
                epgEntry.EpisodeSystemType = "bsepg-epid";
                epgEntry.EpisodeSystemParts = 2;
            }
        
        }

        private void logTitle(string title, EPGEntry epgEntry, Logger logger)
        {
            logger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +
                epgEntry.StartTime.ToShortDateString() + " " +
                epgEntry.StartTime.ToString("HH:mm") + " - " +
                epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                title);
        }

        private void logDescription(string description, EPGEntry epgEntry, Logger logger)
        {
            logger.Write(epgEntry.OriginalNetworkID + ":" + epgEntry.TransportStreamID + ":" + epgEntry.ServiceID + " " +
                epgEntry.StartTime.ToShortDateString() + " " +
                epgEntry.StartTime.ToString("HH:mm") + " - " +
                epgEntry.StartTime.Add(epgEntry.Duration).ToString("HH:mm") + " " +
                description);
        }

        private bool checkChannelMapping(TVStation tvStation)
        {
            if (RunParameters.Instance.ChannelBouquet == -1 && 
                !RunParameters.Instance.Options.Contains("USECHANNELID") && 
                !RunParameters.Instance.Options.Contains("USELCN"))
                    return (true);

            if (RunParameters.Instance.ChannelBouquet != -1)
            {
                Bouquet bouquet = Bouquet.FindBouquet(RunParameters.Instance.ChannelBouquet);
                if (bouquet == null)
                    return (false);

                Region region;
                if (RunParameters.Instance.ChannelRegion != -1)
                    region = bouquet.FindRegion(RunParameters.Instance.ChannelRegion);
                else
                    region = bouquet.FindRegion(0);
                if (region == null)
                    return (false);

                Channel channel = region.FindChannel(tvStation.OriginalNetworkID, tvStation.TransportStreamID, tvStation.ServiceID);
                if (channel == null)
                    return (false);

                if (tvStation.LogicalChannelNumber != -1)
                    return (true);

                tvStation.LogicalChannelNumber = channel.UserChannel;
            }
            else
            {
                foreach (Bouquet bouquet in Bouquet.Bouquets)
                {
                    foreach (Region region in bouquet.Regions)
                    {
                        foreach (Channel channel in region.Channels)
                        {
                            if (channel.OriginalNetworkID == tvStation.OriginalNetworkID &&
                                channel.TransportStreamID == tvStation.TransportStreamID &&
                                channel.ServiceID == tvStation.ServiceID &&
                                tvStation.LogicalChannelNumber == -1)
                                    tvStation.LogicalChannelNumber = channel.UserChannel;
                        }
                    }
                }
            }

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

        private void combineMidnightPrograms()
        {
            foreach (TVStation tvStation in TVStation.StationCollection)
            {
                for (int index = 0; index < tvStation.EPGCollection.Count; index++)
                {
                    EPGEntry epgEntry = tvStation.EPGCollection[index];
                    checkMidnightBreak(tvStation, epgEntry, index);
                }
            }
        }

        private void checkMidnightBreak(TVStation tvStation, EPGEntry currentEntry, int index)
        {
            if (index == tvStation.EPGCollection.Count - 1)
                return;

            EPGEntry nextEntry = tvStation.EPGCollection[index + 1];

            if (currentEntry.EventName != nextEntry.EventName)
                return;

            bool combined = false;

            if (RunParameters.Instance.CountryCode == null || RunParameters.Instance.CountryCode == "NZL")
                combined = checkNZLTimes(currentEntry, nextEntry);
            else
            {
                if (RunParameters.Instance.CountryCode == "AUS")
                    combined = checkAUSTimes(currentEntry, nextEntry);
            }

            if (combined)
                tvStation.EPGCollection.RemoveAt(index + 1);
        }

        private bool checkNZLTimes(EPGEntry currentEntry, EPGEntry nextEntry)
        {
            if (!currentEntry.EndsAtMidnight)
                return (false);

            if (!nextEntry.StartsAtMidnight)
                return (false);

            if (currentEntry.StartTime + currentEntry.Duration != nextEntry.StartTime)
                return (false);

            if (nextEntry.Duration > new TimeSpan(3, 0, 0))
                return (false);

            Logger.Instance.Write("Combining " + currentEntry.ScheduleDescription + " with " + nextEntry.ScheduleDescription);
            currentEntry.Duration = currentEntry.Duration + nextEntry.Duration;

            return (true);
        }

        private bool checkAUSTimes(EPGEntry currentEntry, EPGEntry nextEntry)
        {
            if (!nextEntry.StartsAtMidnight)
                return (false);

            if (currentEntry.StartTime + currentEntry.Duration != nextEntry.StartTime + nextEntry.Duration)
                return (false);

            Logger.Instance.Write("Combining " + currentEntry.ScheduleDescription + " with " + nextEntry.ScheduleDescription);

            return (true);
        }

        private class ServiceEntry
        {
            internal int OriginalNetworkID { get { return (originalNetworkID); } }
            internal int TransportStreamID { get { return (transportStreamID); } }
            internal int ServiceID { get { return (serviceID); } }
            internal string ReferenceNumber { get { return (referenceNumber); } }
            internal string BaseCRID { get { return (baseCRID); } }
            internal string Name { get { return (name); } }

            private int originalNetworkID;
            private int transportStreamID;
            private int serviceID;
            private string referenceNumber;
            private string baseCRID;
            private string name;

            private ServiceEntry() { }

            internal ServiceEntry(string identities, string referenceNumber, string baseCRID, string name)
            {
                string[] identityParts = identities.Split(new char[] { '_' });
                if (identityParts.Length != 3)
                    return;

                try
                {
                    originalNetworkID = Int32.Parse(identityParts[0].Trim());
                    transportStreamID = Int32.Parse(identityParts[1].Trim());
                    serviceID = Int32.Parse(identityParts[2].Trim());
                }
                catch (ArgumentException) { return; }
                catch (ArithmeticException) { return; }
                catch (FormatException) { return; }

                this.referenceNumber = referenceNumber.Trim();                
                this.baseCRID = baseCRID.Trim();
                this.name = name.Trim();
            }
        }
    }
}