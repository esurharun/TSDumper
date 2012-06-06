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
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using DirectShow;
using DomainObjects;
using DVBServices;

namespace EPGCollector
{
    class Program
    {
        /// <summary>
        /// Get the full assembly version number.
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return (version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision);
            }
        }

        private static BDAGraph graph;

        private static TimerCallback timerDelegate;
        private static System.Threading.Timer timer;

        private static BackgroundWorker graphWorker;
        private static BackgroundWorker keyboardWorker;

        private static AutoResetEvent endProgramEvent = new AutoResetEvent(false);
        private static AutoResetEvent endFrequencyEvent = new AutoResetEvent(false);

        private static bool cancelGraph;
        private static bool ignoreProcessComplete;
        
        private static int collectionsWorked;
        private static int tuneFailed;
        private static int timeOuts;

        private static DateTime startTime;
        private static int totalOverlaps;
        private static int totalGaps;

        private static bool pluginAbandon = false;

        static void Main(string[] args)
        {
            RunParameters.BaseDirectory = Application.StartupPath;

            try
            {
                Logger.Instance.WriteSeparator("EPG Collector (Version " + RunParameters.SystemVersion + ")");
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot write log file");
                Console.WriteLine(e.Message);
                Environment.Exit(9);
            }

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(unhandledException);
            Thread.CurrentThread.Name = "Main Thread";

            startTime = DateTime.Now;

            Logger.Instance.Write("OS version: " + Environment.OSVersion.Version);
            Logger.Instance.Write("");
            Logger.Instance.Write("Executable build: " + AssemblyVersion);
            Logger.Instance.Write("DirectShow build: " + DirectShowGraph.AssemblyVersion);
            Logger.Instance.Write("DomainObjects build: " + RunParameters.AssemblyVersion);
            Logger.Instance.Write("DVBServices build: " + Utils.AssemblyVersion);
            Logger.Instance.Write("");
            Logger.Instance.Write("Privilege level: " + RunParameters.Role);
            Logger.Instance.Write("");
            Logger.Instance.Write("Base directory: " + RunParameters.BaseDirectory);
            Logger.Instance.Write("Data directory: " + RunParameters.DataDirectory);
            Logger.Instance.Write("");

            bool reply = CommandLine.Process(args);
            if (!reply)
            {
                Logger.Instance.Write("<e> Incorrect command line");
                Logger.Instance.Write("<e> Exiting with code = 4");
                Environment.Exit(4);
            }

            if (!CommandLine.PluginMode)
                runNormalCollection();
            else
                runPluginCollection();
        }

        private static void runNormalCollection()
        {
            if (CommandLine.TunerQueryOnly)
            {
                BDAGraph.LoadTuners();
                if (!Tuner.TunerPresent)
                {
                    Logger.Instance.Write("<e> No tuners detected");
                    Logger.Instance.Write("<e> Exiting with code = 1");
                    Environment.Exit(1);
                }
                else
                {
                    Logger.Instance.Write("Tuner query only");
                    Logger.Instance.Write("<C> Exiting with code = 0");
                    Environment.Exit(0);
                }
            }

            int errorCode = RunParameters.Instance.Process(CommandLine.IniFileName);
            if (errorCode != 0)
            {
                Logger.Instance.Write("<e> Configuration incorrect");
                Logger.Instance.Write("<e> Exiting with code = " + errorCode);
                Environment.Exit(errorCode);
            }

            if (RunParameters.Instance.TSFileName == null)
                processTunerCollection();
            else
                processSimulatedCollection();
        }

        private static void processTunerCollection()
        {
            BDAGraph.LoadTuners();
            if (!Tuner.TunerPresent)
            {
                Logger.Instance.Write("<e> No tuners detected");
                Logger.Instance.Write("<e> Exiting with code = 1");
                Environment.Exit(1);
            }

            bool reply = checkConfiguration();
            if (!reply)
            {
                Logger.Instance.Write("<e> Configuration does not match ini parameters");
                Logger.Instance.Write("<e> Exiting with code = 8");
                Environment.Exit(8);
            }

            EPGController.ProcessComplete += new EPGController.ProcessCompleteHandler(epgControllerProcessComplete);

            graphWorker = new BackgroundWorker();
            graphWorker.WorkerSupportsCancellation = true;
            graphWorker.DoWork += new DoWorkEventHandler(graphWorkerDoWork);
            graphWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(graphWorkerCompleted);
            graphWorker.RunWorkerAsync();

            if (!RunParameters.Instance.Options.Contains("RUNFROMSERVICE"))
            {
                keyboardWorker = new BackgroundWorker();
                keyboardWorker.WorkerSupportsCancellation = true;
                keyboardWorker.DoWork += new DoWorkEventHandler(keyboardWorkerDoWork);
                keyboardWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(keyboardWorkerCompleted);
                keyboardWorker.RunWorkerAsync();
            }
            else
                Logger.Instance.Write("Running from service - cancellation not available");

            reply = endProgramEvent.WaitOne();

            if (timer != null)
                timer.Dispose();

            if (cancelGraph)
            {
                Logger.Instance.Write("Cancelled by user - no file created");
                Logger.Instance.Write("<C> Exiting with code = 7");
                Environment.Exit(7);
            }

            if (collectionsWorked != 0)
            {
                logDataCollected();

                string outputReply = OutputFile.Process(RunParameters.Instance.OutputFileName);
                if (outputReply != null)
                {
                    Logger.Instance.Write("<e> The output file could not be created");
                    Logger.Instance.Write("<e> " + outputReply);
                    Logger.Instance.Write("<C> Exiting with code = 11");
                    Environment.Exit(11);
                }
            }

            Logger.Instance.Write("<C> Finished - created " + TVStation.EPGCount + " EPG entries");

            if (collectionsWorked != TuningFrequency.FrequencyCollection.Count)
            {
                Logger.Instance.Write("<e> Exiting with code = 6");
                Environment.Exit(6);
            }

            if (TVStation.EPGCount == 0)
            {
                Logger.Instance.Write("<e> No data collected");
                Logger.Instance.Write("<e> Exiting with code = 13");
                Environment.Exit(13);
            }

            if (RunParameters.Instance.Options.Contains("STORESTATIONINFO"))
            {
                string unloadReply = TVStation.Unload(Path.Combine(RunParameters.DataDirectory, "Station Cache.xml"), TVStation.StationCollection);
                if (unloadReply != null)
                {
                    Logger.Instance.Write("<C> Failed to output station cache file");
                    Logger.Instance.Write("<C> " + unloadReply);
                }
                else
                    Logger.Instance.Write("Station cache file output successfully");
            }

            Logger.Instance.Write("<C> Exiting with code = 0");
            Environment.Exit(0);
        }

        private static void graphWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                throw new InvalidOperationException("Graph background worker failed - see inner exception", e.Error);
        }

        private static void keyboardWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                throw new InvalidOperationException("Keyboard background worker failed - see inner exception", e.Error);
        }

        private static void unhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;

            if (exception != null)
            {
                while (exception.InnerException != null)
                    exception = exception.InnerException;

                Logger.Instance.Write("<E> ** The program has failed with an exception");
                Logger.Instance.Write("<E> ** Exception: " + exception.Message);
                Logger.Instance.Write("<E> ** Location: " + exception.StackTrace);
            }
            else
                Logger.Instance.Write("<E> An unhandled exception of type " + e.ExceptionObject + " has occurred");

            Logger.Instance.Write("<E> Exiting with code = 5");
            System.Environment.Exit(5);
        }

        private static void epgControllerProcessComplete(object sender, EventArgs e)
        {
            if (ignoreProcessComplete)
                return;

            endFrequencyEvent.Set();
        }

        private static void timerCallback(object stateObject)
        {
            timeOuts++;
            endFrequencyEvent.Set();
        }

        private static void graphWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int frequencyIndex = 0;

            while (frequencyIndex < TuningFrequency.FrequencyCollection.Count && !cancelGraph)
            {
                TuningFrequency frequency = TuningFrequency.FrequencyCollection[frequencyIndex];

                bool tuned = tuneFrequency(frequency);
                if (!tuned)
                    tuneFailed++;
                else
                {
                    if (!getData(frequency))
                        timeOuts++;

                    if (graph != null)
                        graph.Dispose();
                }

                frequencyIndex++;
            }

            endProgramEvent.Set();
        }

        private static bool getData(TuningFrequency frequency)
        {
            timerDelegate = new TimerCallback(timerCallback);
            timer = new System.Threading.Timer(timerDelegate, null, RunParameters.Instance.FrequencyTimeout, RunParameters.Instance.FrequencyTimeout);

            ignoreProcessComplete = false;

            if (RunParameters.Instance.TSFileName == null)
                EPGController.Instance.Run(graph, frequency);
            else
            {
                SimulationDataProvider dataProvider = new SimulationDataProvider(RunParameters.Instance.TSFileName, frequency);
                string providerReply = dataProvider.Run();
                if (providerReply != null)
                {
                    Logger.Instance.Write("<e> Simulation Data Provider failed");
                    Logger.Instance.Write("<e> " + providerReply);
                    timer.Dispose();
                    return (false);
                }
                EPGController.Instance.Run(dataProvider, frequency);
            }

            bool reply = endFrequencyEvent.WaitOne();

            ignoreProcessComplete = true;

            EPGController.Instance.Stop();
            timer.Dispose();

            collectionsWorked++;

            return (true);
        }

        private static bool tuneFrequency(TuningFrequency frequency)
        {
            Logger.Instance.Write("Tuning to frequency " + frequency.Frequency + " on " + frequency.TunerType);

            TuningSpec tuningSpec;
            TunerNodeType tunerNodeType;

            switch (frequency.TunerType)
            {
                case TunerType.Satellite:
                    tuningSpec = new TuningSpec((Satellite)frequency.Provider, (SatelliteFrequency)frequency);
                    tunerNodeType = TunerNodeType.Satellite;
                    break;
                case TunerType.Terrestrial:
                    tuningSpec = new TuningSpec((TerrestrialFrequency)frequency);
                    tunerNodeType = TunerNodeType.Terrestrial;
                    break;
                case TunerType.Cable:
                    tuningSpec = new TuningSpec((CableFrequency)frequency);
                    tunerNodeType = TunerNodeType.Cable;
                    break;
                case TunerType.ATSC:
                case TunerType.ATSCCable:
                    tuningSpec = new TuningSpec((AtscFrequency)frequency);
                    tunerNodeType = TunerNodeType.ATSC;
                    break;
                case TunerType.ClearQAM:
                    tuningSpec = new TuningSpec((ClearQamFrequency)frequency);
                    tunerNodeType = TunerNodeType.Cable;
                    break;
                case TunerType.ISDBS:
                    tuningSpec = new TuningSpec((Satellite)frequency.Provider, (ISDBSatelliteFrequency)frequency);
                    tunerNodeType = TunerNodeType.ISDBS;
                    break;
                case TunerType.ISDBT:
                    tuningSpec = new TuningSpec((ISDBTerrestrialFrequency)frequency);
                    tunerNodeType = TunerNodeType.ISDBT;
                    break;
                default:
                    throw (new InvalidOperationException("Frequency tuner type not recognized"));
            }

            bool finished = false;
            int frequencyRetries = 0;

            Tuner currentTuner = null;

            while (!finished)
            {
                graph = BDAGraph.FindTuner(RunParameters.Instance.SelectedTuners, tunerNodeType, tuningSpec, currentTuner);
                if (graph == null)
                {
                    Logger.Instance.Write("<e> No tuner able to tune frequency " + frequency.ToString());
                    return (false);
                }

                TimeSpan timeout = new TimeSpan();
                bool done = false;
                bool locked = false;

                while (!done)
                {
                    if (cancelGraph)
                    {
                        graph.Dispose();
                        return (false);
                    }

                    locked = graph.SignalLocked;
                    if (!locked)
                    {
                        int signalQuality = graph.SignalQuality;
                        if (signalQuality > 0)
                        {
                            Logger.Instance.Write("Signal not locked but signal quality > 0");
                            locked = true;
                            done = true;
                        }
                        else
                        {
                            if (!RunParameters.Instance.Options.Contains("USESIGNALPRESENT"))
                            {
                                Logger.Instance.Write("Signal not locked and signal quality not > 0");
                                Thread.Sleep(1000);
                                timeout = timeout.Add(new TimeSpan(0, 0, 1));
                                done = (timeout.TotalSeconds == RunParameters.Instance.LockTimeout.TotalSeconds);
                            }
                            else
                            {
                                bool signalPresent = graph.SignalPresent;
                                if (signalPresent)
                                {
                                    Logger.Instance.Write("Signal present");
                                    locked = true;
                                    done = true;
                                }
                                else
                                {
                                    Logger.Instance.Write("Signal not present");
                                    Thread.Sleep(1000);
                                    timeout = timeout.Add(new TimeSpan(0, 0, 1));
                                    done = (timeout.TotalSeconds == RunParameters.Instance.LockTimeout.TotalSeconds);
                                }
                            }
                        }
                    }
                    else
                        done = true;
                }

                if (!locked)
                {
                    Logger.Instance.Write("<e> Failed to acquire signal");
                    graph.Dispose();

                    if (frequencyRetries == 2)
                    {
                        currentTuner = graph.Tuner; 
                        frequencyRetries = 0;
                    }
                    else
                    {
                        frequencyRetries++;
                        Logger.Instance.Write("Retrying frequency");
                    }
                }
                else
                {
                    finished = true;
                    Logger.Instance.Write("Signal acquired: lock is " + graph.SignalLocked + " quality is " + graph.SignalQuality + " strength is " + graph.SignalStrength);
                }
            }

            return (true);
        }

        private static void keyboardWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            bool abandon = false;            

            do
            {
                if (!CommandLine.BackgroundMode)
                {
                    ConsoleKeyInfo abandonKey = Console.ReadKey();
                    abandon = (abandonKey.Key == ConsoleKey.Q);
                }
                else
                {
                    Mutex cancelMutex = new Mutex(false, "EPG Collector Cancel Mutex " + CommandLine.RunReference);
                    cancelMutex.WaitOne();                    
                    cancelMutex.Close();
                    abandon = true;
                }
            }
            while (!abandon);

            cancelGraph = true;
            Logger.Instance.Write("Abandon request from user");

            endFrequencyEvent.Set();
        }

        private static bool checkConfiguration()
        {
            foreach (int tuner in RunParameters.Instance.SelectedTuners)
            {
                if (tuner > Tuner.TunerCollection.Count)
                {
                    Logger.Instance.Write("<e> INI file format error: A Tuner number is out of range.");
                    System.Environment.Exit(3);
                }
            }

            if (TuningFrequency.HasDVBSatelliteFrequency)
            {
                if (RunParameters.Instance.SelectedTuners.Count == 0)
                {
                    if (Tuner.SatelliteTuner == null)
                    {
                        Logger.Instance.Write("<e> INI file format error: Satellite parameters but no satellite tuner present.");
                        Logger.Instance.Write("<e> Exiting with code = 3");
                        System.Environment.Exit(3);
                    }
                }
                else
                {
                    bool satelliteOK = false;

                    foreach (int tuner in RunParameters.Instance.SelectedTuners)
                    {
                        if (Tuner.TunerCollection[tuner - 1].DVBSatelliteNode != null)
                            satelliteOK = true;                        
                    }

                    if (!satelliteOK)
                    {
                        Logger.Instance.Write("<e> INI file format error: Satellite parameters but no satellite tuner selected.");
                        Logger.Instance.Write("<e> Exiting with code = 3");
                        System.Environment.Exit(3);
                    }
                }
            }

            if (TuningFrequency.HasDVBTerrestrialFrequency)
            {
                if (RunParameters.Instance.SelectedTuners.Count == 0)
                {
                    if (Tuner.TerrestrialTuner == null)
                    {
                        Logger.Instance.Write("<e> INI file format error: Terrestrial parameters but no terrestrial tuner present.");
                        Logger.Instance.Write("<e> Exiting with code = 3");
                        System.Environment.Exit(3);
                    }
                }
                else
                {
                    bool terrestrialOK = false;

                    foreach (int tuner in RunParameters.Instance.SelectedTuners)
                    {
                        if (Tuner.TunerCollection[tuner - 1].DVBTerrestrialNode != null)
                            terrestrialOK = true; ;                        
                    }

                    if (!terrestrialOK)
                    {
                        Logger.Instance.Write("<e> INI file format error: Terrestrial parameters but no terrestrial tuner selected.");
                        Logger.Instance.Write("<e> Exiting with code = 3");
                        System.Environment.Exit(3);
                    }
                }
            }

            if (TuningFrequency.HasDVBCableFrequency)
            {
                if (RunParameters.Instance.SelectedTuners.Count == 0)
                {
                    if (Tuner.CableTuner == null)
                    {
                        Logger.Instance.Write("<e> INI file format error: Cable parameters but no cable tuner present.");
                        Logger.Instance.Write("<e> Exiting with code = 3");
                        System.Environment.Exit(3);
                    }
                }
                else
                {
                    bool cableOK = false;

                    foreach (int tuner in RunParameters.Instance.SelectedTuners)
                    {
                        if (Tuner.TunerCollection[tuner - 1].DVBCableNode != null)
                            cableOK = true;
                    }

                    if (!cableOK)
                    {
                        Logger.Instance.Write("<e> INI file format error: Cable parameters but no cable tuner selected.");
                        Logger.Instance.Write("<e> Exiting with code = 3");
                        System.Environment.Exit(3);
                    }
                }
            }

            if (TuningFrequency.HasAtscFrequency)
            {
                if (RunParameters.Instance.SelectedTuners.Count == 0)
                {
                    if (Tuner.AtscTuner == null)
                    {
                        Logger.Instance.Write("<e> INI file format error: ATSC parameters but no ATSC tuner present.");
                        Logger.Instance.Write("<e> Exiting with code = 3");
                        System.Environment.Exit(3);
                    }
                }
                else
                {
                    bool atscOK = false;

                    foreach (int tuner in RunParameters.Instance.SelectedTuners)
                    {
                        if (Tuner.TunerCollection[tuner - 1].AtscNode != null)
                            atscOK = true;
                    }

                    if (!atscOK)
                    {
                        Logger.Instance.Write("<e> INI file format error: ATSC parameters but no ATSC tuner selected.");
                        Logger.Instance.Write("<e> Exiting with code = 3");
                        System.Environment.Exit(3);
                    }
                }
            }

            if (TuningFrequency.HasClearQamFrequency)
            {
                if (RunParameters.Instance.SelectedTuners.Count == 0)
                {
                    if (Tuner.ClearQamTuner == null)
                    {
                        Logger.Instance.Write("<e> INI file format error: Clear QAM parameters but no cable tuner present.");
                        Logger.Instance.Write("<e> Exiting with code = 3");
                        System.Environment.Exit(3);
                    }
                }
                else
                {
                    bool atscOK = false;

                    foreach (int tuner in RunParameters.Instance.SelectedTuners)
                    {
                        if (Tuner.TunerCollection[tuner - 1].AtscNode != null)
                            atscOK = true;
                    }

                    if (!atscOK)
                    {
                        Logger.Instance.Write("<e> INI file format error: ATSC parameters but no ATSC tuner selected.");
                        Logger.Instance.Write("<e> Exiting with code = 3");
                        System.Environment.Exit(3);
                    }
                }
            }

            if (TuningFrequency.HasOpenTVFrequency && RunParameters.Instance.CountryCode == null)
            {
                Logger.Instance.Write("<e> INI file format error: A Location parameter is mandatory for OpenTV collection.");
                Logger.Instance.Write("<e> Exiting with code = 3");
                System.Environment.Exit(3);
            }

            return (true);
        }

        private static void logDataCollected()
        {
            EPGController.Instance.FinishRun();

            Logger.Instance.WriteSeparator("Stations With No Data");

            foreach (TVStation tvStation in TVStation.StationCollection)
            {
                if (!tvStation.Excluded && tvStation.EPGCollection.Count == 0)
                    logStationCollected(tvStation);
            }

            Logger.Instance.WriteSeparator("Output Data");

            int stations = 0;
            foreach (TVStation tvStation in TVStation.StationCollection)
            {
                if (!tvStation.Excluded && tvStation.EPGCollection.Count != 0)
                {
                    logStationCollected(tvStation);
                    stations++;
                }
            }

            Logger.Instance.Write("<S> Summary: Total Stations = " + stations +
                " Total Gaps = " + totalGaps +
                " Total Overlaps = " + totalOverlaps +
                " Total Time = " + (DateTime.Now - startTime));
        }

        private static void logStationCollected(TVStation tvStation)
        {
            int records = 0;
            int gaps = 0;
            int overlaps = 0;
            DateTime startTime = DateTime.MinValue;
            DateTime endTime = DateTime.MinValue;

            foreach (EPGEntry epgEntry in tvStation.EPGCollection)
            {
                records++;

                if (startTime == DateTime.MinValue)
                {
                    startTime = epgEntry.StartTime;
                    endTime = epgEntry.StartTime + epgEntry.Duration;
                }
                else
                {
                    if (epgEntry.StartTime < endTime)
                    {
                        Logger.Instance.Write("Station: " + tvStation.FixedLengthName + " (" + tvStation.FullID + ") Overlap at " + epgEntry.StartTime + " " + epgEntry.EventName);
                        overlaps++;
                    }
                    else
                    {
                        if (RunParameters.Instance.Options.Contains("ACCEPTBREAKS"))
                        {
                            if (epgEntry.StartTime > endTime + new TimeSpan(0, 5, 0))
                            {
                                Logger.Instance.Write("Station: " + tvStation.FixedLengthName + " (" + tvStation.FullID + ") Gap " + endTime + " to " + epgEntry.StartTime + " " + epgEntry.EventName);
                                gaps++;
                            }
                        }
                        else
                        {
                            if (epgEntry.StartTime > endTime)
                            {
                                Logger.Instance.Write("Station: " + tvStation.FixedLengthName + " (" + tvStation.FullID + ") Gap " + endTime + " to " + epgEntry.StartTime + " " + epgEntry.EventName);
                                gaps++;
                            }
                        }
                    }

                    endTime = epgEntry.StartTime + epgEntry.Duration;
                }
            }

            if (startTime == DateTime.MinValue)
            {
                string epg;

                if (tvStation.NextFollowingAvailable)
                {
                    if (tvStation.ScheduleAvailable)
                        epg = "NN&S";
                    else
                        epg = "NN";
                }
                else
                {
                    if (tvStation.ScheduleAvailable)
                        epg = "S";
                    else
                        epg = "None";
                }

                Logger.Instance.Write("Station: " + tvStation.FixedLengthName + " (" + tvStation.FullID + " EPG: " + epg +") No data");
            }
            else
                Logger.Instance.Write("Station: " + tvStation.FixedLengthName + " (" + tvStation.FullID + ") Start: " +
                    startTime + " End: " + endTime +
                    " Records: " + records +
                    " Overlaps: " + overlaps + " Gaps: " + gaps);

            totalOverlaps += overlaps;
            totalGaps += gaps;
        }

        private static void processSimulatedCollection()
        {
            SimulationDataProvider dataProvider = new SimulationDataProvider(RunParameters.Instance.TSFileName, TuningFrequency.FrequencyCollection[0]);
            string providerReply = dataProvider.Run();
            if (providerReply != null)
            {
                Logger.Instance.Write("<e> Simulation Data Provider failed");
                Logger.Instance.Write("<e> " + providerReply);
                Logger.Instance.Write("<C> Exiting with code = 12");
                Environment.Exit(12);
            }

            EPGController.ProcessComplete += new EPGController.ProcessCompleteHandler(simulationProcessComplete);
            EPGController.Instance.Run(dataProvider, TuningFrequency.FrequencyCollection[0]);

            if (!RunParameters.Instance.Options.Contains("RUNFROMSERVICE"))
            {
                keyboardWorker = new BackgroundWorker();
                keyboardWorker.WorkerSupportsCancellation = true;
                keyboardWorker.DoWork += new DoWorkEventHandler(keyboardWorkerDoWork);
                keyboardWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(keyboardWorkerCompleted);
                keyboardWorker.RunWorkerAsync();
            }
            else
                Logger.Instance.Write("Running from service - cancellation not available");

            bool reply = endProgramEvent.WaitOne();

            dataProvider.Stop();

            if (cancelGraph)
            {
                Logger.Instance.Write("Cancelled by user - no data created");
                Logger.Instance.Write("<C> Exiting with code = 7");
                Environment.Exit(7);
            }

            logDataCollected();

            string outputReply = OutputFile.Process(RunParameters.Instance.OutputFileName);
            if (outputReply != null)
            {
                Logger.Instance.Write("<e> The output data could not be created");
                Logger.Instance.Write("<e> " + outputReply);
                Logger.Instance.Write("<C> Exiting with code = 11");
                Environment.Exit(11);
            }

            Logger.Instance.Write("<C> Finished - created " + TVStation.EPGCount + " EPG entries");

            Logger.Instance.Write("<C> Exiting with code = 0");
            Environment.Exit(0);
        }

        private static void simulationProcessComplete(object sender, EventArgs e)
        {
            endProgramEvent.Set();
        }

        private static void runPluginCollection()
        {
            RunParameters.Instance = new RunParameters(ParameterSet.Plugin);
            int errorCode = RunParameters.Instance.Process(CommandLine.IniFileName);
            if (errorCode != 0)
            {
                Logger.Instance.Write("<e> Plugin parameters incorrect");
                Logger.Instance.Write("<e> Exiting with code = " + errorCode);
                Environment.Exit(errorCode);
            }

            PluginDataProvider dataProvider = new PluginDataProvider(RunParameters.Instance.PluginFrequency, CommandLine.RunReference);
            dataProvider.Run(CommandLine.RunReference);

            EPGController.ProcessComplete += new EPGController.ProcessCompleteHandler(pluginProcessComplete);
            EPGController.Instance.Run(dataProvider, RunParameters.Instance.PluginFrequency);

            BackgroundWorker pluginAbandonWorker = new BackgroundWorker();
            pluginAbandonWorker.WorkerSupportsCancellation = true;
            pluginAbandonWorker.DoWork += new DoWorkEventHandler(pluginAbandonWorkerDoWork);
            pluginAbandonWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(pluginAbandonWorkerCompleted);
            pluginAbandonWorker.RunWorkerAsync();

            bool reply = endProgramEvent.WaitOne();

            dataProvider.Stop();

            if (pluginAbandon)
            {
                Logger.Instance.Write("Cancelled by user - no data created");
                Logger.Instance.Write("<C> Exiting with code = 7");
                Environment.Exit(7);
            }

            logDataCollected();

            string outputReply;
            if (RunParameters.Instance.OutputFileSet)
                outputReply = OutputFile.Process(RunParameters.Instance.OutputFileName);
            else
            {
                if (!RunParameters.Instance.Options.Contains("WMCIMPORT"))
                {
                    FileInfo iniFileInfo = new FileInfo(CommandLine.IniFileName);
                    string outputFileName = Path.Combine(iniFileInfo.DirectoryName, "EPG Collector Plugin.xml");
                    outputReply = OutputFilePlugin.Process(outputFileName);
                }
                else
                    outputReply = OutputFileMXF.Process();
            }
            if (outputReply != null)
            {
                Logger.Instance.Write("<e> The output data could not be created");
                Logger.Instance.Write("<e> " + outputReply);
                Logger.Instance.Write("<C> Exiting with code = 11");
                Environment.Exit(11);
            }

            Logger.Instance.Write("<C> Finished - created " + TVStation.EPGCount + " EPG entries");

            if (TVStation.EPGCount == 0)
            {
                Logger.Instance.Write("<e> No data collected");
                Logger.Instance.Write("<e> Exiting with code = 13");
                Environment.Exit(13);
            }

            if (RunParameters.Instance.Options.Contains("STORESTATIONINFO"))
            {
                string unloadReply = TVStation.Unload(Path.Combine(RunParameters.DataDirectory, "Station Cache.xml"), TVStation.StationCollection);
                if (unloadReply != null)
                {
                    Logger.Instance.Write("<C> Failed to output station cache file");
                    Logger.Instance.Write("<C> " + unloadReply);
                }
                else
                    Logger.Instance.Write("Station cache file output successfully");
            }

            Logger.Instance.Write("<C> Exiting with code = 0");
            Environment.Exit(0);
        }

        private static void pluginProcessComplete(object sender, EventArgs e)
        {
            endProgramEvent.Set();
        }

        private static void pluginAbandonWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            string cancellationName = "EPG Collector Cancellation Mutex " + CommandLine.RunReference;
            Logger.Instance.Write("Cancellation mutex name is " + cancellationName);
            
            Mutex cancelMutex = new Mutex(false, cancellationName);

            try
            {
                cancelMutex.WaitOne();
                cancelMutex.Close();
            }       
            catch (AbandonedMutexException)
            {
                Logger.Instance.Write("<E> TVSource has failed - the collection will be abandoned");                
            }
            
            pluginAbandon = true;

            endProgramEvent.Set();
        }

        private static void pluginAbandonWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                throw new InvalidOperationException("Plugin abandon background worker failed - see inner exception", e.Error);
        }
    }
}
