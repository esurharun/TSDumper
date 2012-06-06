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
    /// The class that controls the acquisition and processing of Bell TV data.
    /// </summary>   
    public class BellTVController : ControllerBase
    {
        /// <summary>
        /// Return true if all data has been processed; false otherwise.
        /// </summary>
        public override bool AllDataProcessed { get { return (true); } }

        private TSStreamReader bellTVReader;
        private bool bellTVSectionsDone = false;

        /// <summary>
        /// Initialize a new instance of the BellTVController class.
        /// </summary>
        public BellTVController() { }

        /// <summary>
        /// Stop acquiring and processing data.
        /// </summary>
        public override void Stop()
        {
            Logger.Instance.Write("Stopping section readers");

            if (bellTVReader != null)
                bellTVReader.Stop();

            Logger.Instance.Write("Stopped section readers");
        }

        /// <summary>
        /// Acquire and process Bell TV data.
        /// </summary>
        /// <param name="dataProvider">A sample data provider.</param>
        /// <param name="worker">The background worker that is running this collection.</param>
        /// <returns>A CollectorReply code.</returns>
        public override CollectorReply Process(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            BellTVProgramCategory.Load();
            CustomProgramCategory.Load();
            ParentalRating.Load();

            GetStationData(dataProvider, worker);
            if (worker.CancellationPending)
                return (CollectorReply.Cancelled);

            getBellTVData(dataProvider, worker);

            return (CollectorReply.OK);
        }

        private void getBellTVData(ISampleDataProvider dataProvider, BackgroundWorker worker)
        {
            Logger.Instance.Write("Collecting Bell TV data", false, true);

            dataProvider.ChangePidMapping(0x441);

            bellTVReader = new TSStreamReader(2000, dataProvider.BufferAddress);
            bellTVReader.Run();

            int lastCount = 0;
            int repeats = 0;

            while (!bellTVSectionsDone)
            {
                if (worker.CancellationPending)
                    return;

                Thread.Sleep(2000);
                Logger.Instance.Write(".", false, false);

                Collection<Mpeg2Section> sections = new Collection<Mpeg2Section>();

                bellTVReader.Lock("LoadMessages");
                if (bellTVReader.Sections.Count != 0)
                {
                    foreach (Mpeg2Section section in bellTVReader.Sections)
                        sections.Add(section);
                    bellTVReader.Sections.Clear();
                }
                bellTVReader.Release("LoadMessages");

                if (sections.Count != 0)
                    processSections(sections);

                if (TVStation.EPGCount == lastCount)
                {
                    repeats++;
                    bellTVSectionsDone = (repeats == RunParameters.Instance.Repeats);
                }
                else
                    repeats = 0;

                lastCount = TVStation.EPGCount;
            }

            Logger.Instance.Write("", true, false);
            Logger.Instance.Write("Stopping reader");
            bellTVReader.Stop();

            Logger.Instance.Write("EPG count: " + TVStation.EPGCount + " buffer space used: " + dataProvider.BufferSpaceUsed);
        }

        private void processSections(Collection<Mpeg2Section> sections)
        {
            foreach (Mpeg2Section section in sections)
            {
                if (RunParameters.Instance.TraceIDs.Contains("BELLTVSECTIONS"))
                    Logger.Instance.Dump("Bell TV Section", section.Data, section.Length);

                try
                {
                    Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();
                    mpeg2Header.Process(section.Data);
                    if (mpeg2Header.Current)
                    {
                        if (mpeg2Header.TableID > 0x80 && mpeg2Header.TableID < 0xa5)
                        {
                            BellTVSection bellTVSection = new BellTVSection();
                            bellTVSection.Process(section.Data, mpeg2Header);
                        }
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Logger.Instance.Write("<e> Bell TV error: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Create the EPG entries.
        /// </summary>
        public override void FinishFrequency()
        {
            foreach (TVStation station in TVStation.StationCollection)
            {
                if (station.Name != null)
                    station.ChannelID = station.OriginalNetworkID + ":" +
                        station.TransportStreamID + ":" +
                        station.ServiceID + ":" +
                        station.Name;
                else
                    station.ChannelID = station.OriginalNetworkID + ":" +
                        station.TransportStreamID + ":" +
                        station.ServiceID;
            }

            BellTVProgramCategory.LogCategoryUsage();
            LanguageCode.LogUsage();
        }
    }
}
