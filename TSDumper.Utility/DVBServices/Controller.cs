using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

using DomainObjects;
using DirectShow;

namespace EPGService
{
    public class Controller
    {
        private EPGDatabase epgDatabase;

        public Controller() { }

        public void Run()
        {
            SatelliteDatabase satelliteDatabase = new SatelliteDatabase();
            satelliteDatabase.Load();

            TVDatabase tvDatabase = new TVDatabase();
            tvDatabase.Load();

            epgDatabase = new EPGDatabase();
            epgDatabase.Load();
            
            Collection<TuningSpec> tuningCollection = new Collection<TuningSpec>();

            foreach (TVStation tvStation in tvDatabase.StationCollection)
            {
                TuningSpec tuningSpec = findTuningSpec(tuningCollection, tvStation);
                if (tuningSpec == null)
                {
                    TuningSpec newTuningSpec = new TuningSpec();
                    newTuningSpec.Satellite = tvStation.Satellite;
                    newTuningSpec.OriginalNetworkID = tvStation.OriginalNetworkID;
                    newTuningSpec.TransportStreamID = tvStation.TransportStreamID;
                    newTuningSpec.Frequency = tvStation.Frequency;
                    newTuningSpec.SymbolRate = tvStation.SymbolRate; 
                    newTuningSpec.Tag = new EPGGraph(satelliteDatabase[tvStation.Satellite], newTuningSpec);
                    tuningCollection.Add(newTuningSpec);
                }
            }

            bool end = false;
            DateTime lastUpdateTime = DateTime.Now;
            TimeSpan interval = new TimeSpan(0, 1, 0);

            while (!end)
            {
                foreach (TuningSpec tuningSpec in tuningCollection)
                {
                    EPGGraph currentGraph = tuningSpec.Tag as EPGGraph;
                    currentGraph.Play();

                    EITController eitController = new EITController();

                    while (!eitController.Finished && !end)
                    {
                        Collection<EITEntry> eitCollection = eitController.ProcessEITSections(currentGraph);
                        updateSchedule(eitCollection);

                        if (lastUpdateTime + interval < DateTime.Now)
                        {
                            epgDatabase.UpdateDatabase();
                            lastUpdateTime = DateTime.Now;                            
                        }
                    }

                    currentGraph.Stop();
                }

                Thread.Sleep(2000);
            }
        }

        private TuningSpec findTuningSpec(Collection<TuningSpec> tuningCollection, TVStation tvStation)
        {
            foreach (TuningSpec tuningSpec in tuningCollection)
            {
                if (tuningSpec.Satellite == tvStation.Satellite &&
                    tuningSpec.OriginalNetworkID == tvStation.OriginalNetworkID &&
                    tuningSpec.TransportStreamID == tvStation.TransportStreamID &&
                    tuningSpec.Frequency == tvStation.Frequency &&
                    tuningSpec.SymbolRate == tvStation.SymbolRate)
                    return (tuningSpec);
            }

            return (null);
        }

        private void updateSchedule(Collection<EITEntry> eitCollection)
        {
            foreach (EITEntry eitEntry in eitCollection)
            {
                EPGEntry epgEntry = findEPGEntry(eitEntry);
                if (epgEntry == null)
                    addEPGEntry(eitEntry);
                else
                    updateEPGEntry(epgEntry, eitEntry);
            }
        }

        private EPGEntry findEPGEntry(EITEntry eitEntry)
        {
            foreach (EPGEntry epgEntry in epgDatabase.EPGCollection)
            {
                if (epgEntry.OriginalNetworkID == eitEntry.OriginalNetworkID &&
                    epgEntry.TransportStreamID == eitEntry.TransportStreamID &&
                    epgEntry.ServiceID == eitEntry.ServiceID &&
                    epgEntry.StartTime == eitEntry.StartTime)
                    return (epgEntry);
            }

            return (null);
        }

        private void addEPGEntry(EITEntry eitEntry)
        {
            EPGEntry epgEntry = new EPGEntry();

            epgEntry.ComponentTypeAudio = eitEntry.ComponentTypeAudio;
            epgEntry.ComponentTypeVideo = eitEntry.ComponentTypeVideo;
            epgEntry.ContentSubType = eitEntry.ContentSubType;
            epgEntry.ContentType = eitEntry.ContentType;
            epgEntry.Duration = eitEntry.Duration;
            epgEntry.EventID = eitEntry.EventID;
            epgEntry.EventName = eitEntry.EventName;
            epgEntry.ExtendedDescription = eitEntry.ExtendedDescription;
            epgEntry.OriginalNetworkID = eitEntry.OriginalNetworkID;
            epgEntry.ParentalRating = eitEntry.ParentalRating;
            epgEntry.RunningStatus = eitEntry.RunningStatus;
            epgEntry.Scrambled = eitEntry.Scrambled;
            epgEntry.ServiceID = eitEntry.ServiceID;
            epgEntry.ShortDescription = eitEntry.ShortDescription;
            epgEntry.StartTime = eitEntry.StartTime;
            epgEntry.TransportStreamID = eitEntry.TransportStreamID;
            epgEntry.VersionNumber = eitEntry.VersionNumber;

            epgDatabase.AddEPGEntry(epgEntry);
        }

        private void updateEPGEntry(EPGEntry epgEntry, EITEntry eitEntry)
        {
            epgEntry.ComponentTypeAudio = eitEntry.ComponentTypeAudio;
            epgEntry.ComponentTypeVideo = eitEntry.ComponentTypeVideo;
            epgEntry.ContentSubType = eitEntry.ContentSubType;
            epgEntry.ContentType = eitEntry.ContentType;
            epgEntry.Duration = eitEntry.Duration;
            epgEntry.EventID = eitEntry.EventID;
            epgEntry.EventName = eitEntry.EventName;
            epgEntry.ExtendedDescription = eitEntry.ExtendedDescription;
            epgEntry.ParentalRating = eitEntry.ParentalRating;
            epgEntry.RunningStatus = eitEntry.RunningStatus;
            epgEntry.Scrambled = eitEntry.Scrambled;
            epgEntry.ShortDescription = eitEntry.ShortDescription;
            epgEntry.VersionNumber = eitEntry.VersionNumber;
        }
    }
}
