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
using System.Drawing;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using DomainObjects;
using DirectShow;
using DVBServices;

namespace EPGCentre
{
    public partial class FindEPGControl : UserControl, IUpdateControl
    {
        /// <summary>
        /// Get the general window heading for the data.
        /// </summary>
        public string Heading { get { return ("EPG Centre - Find EPG"); } }
        /// <summary>
        /// Get the default directory.
        /// </summary>
        public string DefaultDirectory { get { return (null); } }
        /// <summary>
        /// Get the default output file name.
        /// </summary>
        public string DefaultFileName { get { return (null); } }
        /// <summary>
        /// Get the save file filter.
        /// </summary>
        public string SaveFileFilter { get { return (null); } }
        /// <summary>
        /// Get the save file title.
        /// </summary>
        public string SaveFileTitle { get { return (null); } }
        /// <summary>
        /// Get the save file suffix.
        /// </summary>
        public string SaveFileSuffix { get { return (null); } }

        /// <summary>
        /// Return the state of the data set.
        /// </summary>
        public DataState DataState { get { return (new DataState()); } }

        private delegate DialogResult ShowMessage(string message, MessageBoxButtons buttons, MessageBoxIcon icon);

        private Collection<PidSpec> pidList;
        private AnalysisParameters analysisParameters;

        private BackgroundWorker workerAnalyze;
        private AutoResetEvent resetEvent = new AutoResetEvent(false);
        
        private Collection<TVStation> stations;

        public FindEPGControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Process the transport stream.
        /// </summary>
        public void Process()
        {
            frequencySelectionControl.Process();

            nudSignalLockTimeout.Value = 10;
            nudDataCollectionTimeout.Value = 60;
        }

        private void btTimeoutDefaults_Click(object sender, EventArgs e)
        {
            nudSignalLockTimeout.Value = 10;
            nudDataCollectionTimeout.Value = 60;
        }

        private void tbFindDump_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Transport Stream Dump Files (*.ts)|*.ts";
            openFile.RestoreDirectory = true;
            openFile.Title = "Find Transport Stream Dump File";

            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            tbDumpFile.Text = openFile.FileName;
        }

        private void btSetUpAnalysis_Click(object sender, EventArgs e)
        {
            dgViewResults.Visible = false;
            btSetUpFind.Visible = false;

            cmdScan.Visible = true;
        }

        private void cmdScan_Click(object sender, EventArgs e)
        {
            if (cmdScan.Text == "Stop Find")
            {
                Logger.Instance.Write("Stop find requested");
                workerAnalyze.CancelAsync();
                resetEvent.WaitOne(new TimeSpan(0, 0, 45));
                cmdScan.Text = "Start Find";
                lblScanning.Visible = false;
                pbarProgress.Visible = false;
                pbarProgress.Enabled = false;
                MainWindow.ChangeMenuItemAvailability(true);
                return;
            }

            if (!checkData())
                return;

            analysisParameters = getAnalysisParameters();

            if (analysisParameters.DumpFileName != null && analysisParameters.DumpFileName != string.Empty)
                Logger.Instance.Write("Analysis started for file " + analysisParameters.DumpFileName);
            else
                Logger.Instance.Write("Analysis started for provider " + analysisParameters.ScanningFrequency.Provider.Name +
                    " frequency " + analysisParameters.ScanningFrequency.ToString());

            cmdScan.Text = "Stop Find";
            lblScanning.Text = "Scanning " + analysisParameters.ScanningFrequency.ToString();
            lblScanning.Visible = true;
            pbarProgress.Visible = true;
            pbarProgress.Enabled = true;
            MainWindow.ChangeMenuItemAvailability(false);

            workerAnalyze = new BackgroundWorker();
            workerAnalyze.WorkerSupportsCancellation = true;
            workerAnalyze.DoWork += new DoWorkEventHandler(doAnalysis);
            workerAnalyze.RunWorkerCompleted += new RunWorkerCompletedEventHandler(runWorkerCompleted);
            workerAnalyze.RunWorkerAsync(analysisParameters);
        }

        private bool checkData()
        {
            string reply = frequencySelectionControl.ValidateForm();
            if (reply != null)
            {
                MessageBox.Show(reply, "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }

            return (true);
        }

        private void doAnalysis(object sender, DoWorkEventArgs e)
        {
            AnalysisParameters analysisParameters = e.Argument as AnalysisParameters;

            pidList = null;

            TunerNodeType tunerNodeType;
            TuningSpec tuningSpec;

            SatelliteFrequency satelliteFrequency = analysisParameters.ScanningFrequency as SatelliteFrequency;
            if (satelliteFrequency != null)
            {
                tunerNodeType = TunerNodeType.Satellite;
                tuningSpec = new TuningSpec((Satellite)satelliteFrequency.Provider, satelliteFrequency);
            }
            else
            {
                TerrestrialFrequency terrestrialFrequency = analysisParameters.ScanningFrequency as TerrestrialFrequency;
                if (terrestrialFrequency != null)
                {
                    tunerNodeType = TunerNodeType.Terrestrial;
                    tuningSpec = new TuningSpec(terrestrialFrequency);
                }
                else
                {
                    CableFrequency cableFrequency = analysisParameters.ScanningFrequency as CableFrequency;
                    if (cableFrequency != null)
                    {
                        tunerNodeType = TunerNodeType.Cable;
                        tuningSpec = new TuningSpec(cableFrequency);
                    }
                    else
                    {
                        AtscFrequency atscFrequency = analysisParameters.ScanningFrequency as AtscFrequency;
                        if (atscFrequency != null)
                        {
                            if (atscFrequency.TunerType == TunerType.ATSC)
                                tunerNodeType = TunerNodeType.ATSC;
                            else
                                tunerNodeType = TunerNodeType.Cable;
                            tuningSpec = new TuningSpec(atscFrequency);
                        }
                        else
                        {
                            ClearQamFrequency clearQamFrequency = analysisParameters.ScanningFrequency as ClearQamFrequency;
                            if (clearQamFrequency != null)
                            {
                                tunerNodeType = TunerNodeType.Cable;
                                tuningSpec = new TuningSpec(clearQamFrequency);
                            }
                            else
                            {
                                ISDBSatelliteFrequency isdbSatelliteFrequency = analysisParameters.ScanningFrequency as ISDBSatelliteFrequency;
                                if (isdbSatelliteFrequency != null)
                                {
                                    tunerNodeType = TunerNodeType.ISDBS;
                                    tuningSpec = new TuningSpec((Satellite)satelliteFrequency.Provider, isdbSatelliteFrequency);
                                }
                                else
                                {
                                    ISDBTerrestrialFrequency isdbTerrestrialFrequency = analysisParameters.ScanningFrequency as ISDBTerrestrialFrequency;
                                    if (isdbTerrestrialFrequency != null)
                                    {
                                        tunerNodeType = TunerNodeType.ISDBT;
                                        tuningSpec = new TuningSpec(isdbTerrestrialFrequency);
                                    }
                                    else
                                        throw (new InvalidOperationException("Tuning frequency not recognized"));
                                }
                            }
                        }
                    }
                }
            }

            Tuner currentTuner = null;
            bool finished = false;

            while (!finished)
            {
                if ((sender as BackgroundWorker).CancellationPending)
                {
                    Logger.Instance.Write("Find abandoned by user");
                    e.Cancel = true;
                    resetEvent.Set();
                    return;
                }

                BDAGraph graph = BDAGraph.FindTuner(analysisParameters.Tuners, tunerNodeType, tuningSpec, currentTuner, analysisParameters.RepeatDiseqc, analysisParameters.SwitchAfterPlay);
                if (graph == null)
                {
                    Logger.Instance.Write("<e> No tuner able to tune frequency " + analysisParameters.ScanningFrequency.ToString());

                    frequencySelectionControl.Invoke(new ShowMessage(showMessage), "No tuner able to tune frequency " + analysisParameters.ScanningFrequency.ToString(),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    finished = true;
                }
                else
                {
                    string tuneReply = checkTuning(graph, analysisParameters, sender as BackgroundWorker);

                    if ((sender as BackgroundWorker).CancellationPending)
                    {
                        Logger.Instance.Write("Find abandoned by user");
                        graph.Dispose();
                        e.Cancel = true;
                        resetEvent.Set();
                        return;
                    }

                    if (tuneReply == null)
                    {
                        try
                        {
                            if (analysisParameters.DumpFileName == null || analysisParameters.DumpFileName == string.Empty)
                                getData(graph, analysisParameters, sender as BackgroundWorker);
                            else
                            {
                                SimulationDataProvider dataProvider = new SimulationDataProvider(analysisParameters.DumpFileName, graph.Frequency);
                                string providerReply = dataProvider.Run();
                                if (providerReply != null)
                                {
                                    Logger.Instance.Write("<e> Simulation Data Provider failed");
                                    Logger.Instance.Write("<e> " + providerReply);
                                    frequencySelectionControl.Invoke(new ShowMessage(showMessage), "Simulation Data Provider failed." +
                                        Environment.NewLine + Environment.NewLine + providerReply,
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);                                    
                                }
                                else
                                {
                                    getData(dataProvider, analysisParameters, sender as BackgroundWorker);
                                    dataProvider.Stop();
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            Logger.Instance.Write("<e> Failed to process dump file");
                            Logger.Instance.Write("<e> " + ex.Message);
                            frequencySelectionControl.Invoke(new ShowMessage(showMessage), "Failed to process dump file." +
                                Environment.NewLine + Environment.NewLine + ex.Message,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        graph.Dispose();
                        finished = true;
                    }
                    else
                    {
                        Logger.Instance.Write("Failed to tune frequency " + analysisParameters.ScanningFrequency.ToString());
                        graph.Dispose();
                        currentTuner = graph.Tuner;
                    }
                }
            }

            e.Cancel = true;
            resetEvent.Set();
        }

        private DialogResult showMessage(string message, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            lblScanning.Visible = false;
            pbarProgress.Enabled = false;
            pbarProgress.Visible = false;

            DialogResult result = MessageBox.Show(message, "EPG Centre", buttons, icon);
            if (result == DialogResult.Yes)
            {
                lblScanning.Visible = true;
                pbarProgress.Enabled = true;
                pbarProgress.Visible = true;
            }

            return (result);
        }

        private string checkTuning(BDAGraph graph, AnalysisParameters analysisParameters, BackgroundWorker worker)
        {
            TimeSpan timeout = new TimeSpan();
            bool done = false;
            bool locked = false;
            int frequencyRetries = 0;

            while (!done)
            {
                if (worker.CancellationPending)
                {
                    Logger.Instance.Write("Find abandoned by user");
                    return (null);
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
                        if (!analysisParameters.UseSignalPresent)
                        {
                            Logger.Instance.Write("Signal not locked and signal quality not > 0");
                            Thread.Sleep(1000);
                            timeout = timeout.Add(new TimeSpan(0, 0, 1));
                            done = (timeout.TotalSeconds == analysisParameters.SignalLockTimeout);
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
                                done = (timeout.TotalSeconds == analysisParameters.SignalLockTimeout);
                            }
                        }

                        if (done)
                        {
                            done = (frequencyRetries == 2);
                            if (done)
                                Logger.Instance.Write("<e> Failed to acquire signal");
                            else
                            {
                                Logger.Instance.Write("Retrying frequency");
                                timeout = new TimeSpan();
                                frequencyRetries++;
                            }
                        }
                    }
                }
                else
                {
                    Logger.Instance.Write("Signal acquired: lock is " + graph.SignalLocked + " quality is " + graph.SignalQuality + " strength is " + graph.SignalStrength);
                    done = true;
                }
            }

            if (locked)
                return (null);
            else
                return ("<e> The tuner failed to acquire a signal for frequency " + analysisParameters.ScanningFrequency.ToString());
        }

        private void getData(ISampleDataProvider dataProvider, AnalysisParameters analysisParameters, BackgroundWorker worker)
        {
            Logger.Instance.Write("Starting analysis");

            analysisParameters.ScanningFrequency.CollectionType = CollectionType.MHEG5;
            FrequencyScanner frequencyScanner = new FrequencyScanner(dataProvider, worker);
            stations = frequencyScanner.FindTVStations();

            pidList = new Collection<PidSpec>();

            dataProvider.ChangePidMapping(new int[] { -1 });            

            IntPtr memoryPointer = dataProvider.BufferAddress;
            int currentOffset = 0;

            byte[] buffer = new byte[188];
            DateTime startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds < analysisParameters.DataCollectionTimeout && !worker.CancellationPending)
            {
                if (currentOffset >= dataProvider.BufferSpaceUsed)
                {
                    Thread.Sleep(1000);
                    if (currentOffset >= dataProvider.BufferSpaceUsed && dataProvider as SimulationDataProvider == null)
                    {
                        dataProvider.ChangePidMapping(new int[] { -1 });
                        currentOffset = 0;
                    }
                }
                else
                {
                    IntPtr currentPointer = new IntPtr(memoryPointer.ToInt64() + currentOffset + 136);
                    Marshal.Copy(currentPointer, buffer, 0, 188);

                    TransportPacket transportPacket = new TransportPacket();

                    try
                    {
                        transportPacket.Process(buffer);

                        if (!transportPacket.ErrorIndicator && !transportPacket.IsNullPacket)
                        {
                            bool ignorePid = checkPid(transportPacket.PID, stations);
                            if (!ignorePid)
                            {
                                PidSpec pidSpec = findPidSpec(pidList, transportPacket.PID);
                                if (pidSpec == null)
                                {
                                    pidSpec = new PidSpec(transportPacket.PID);
                                    addPid(pidList, new PidSpec(transportPacket.PID));
                                }
                                pidSpec.ProcessPacket(buffer, transportPacket);
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException) { }

                    currentOffset += buffer.Length;
                }
            }

            Logger.Instance.Write("Analysis completed: " + pidList.Count + " PID's loaded");
        }

        private bool checkPid(int pid, Collection<TVStation> stations)
        {
            if (stations == null)
                return (false);

            foreach (TVStation station in stations)
            {
                if (pid == station.AudioPID || pid == station.VideoPID)
                    return (true);
            }

            return (false);
        }

        private PidSpec findPidSpec(Collection<PidSpec> pidList, int pid)
        {
            foreach (PidSpec pidSpec in pidList)
            {
                if (pidSpec.Pid == pid)
                    return (pidSpec);

                if (pidSpec.Pid > pid)
                    return (null);
            }

            return (null);
        }

        private void addPid(Collection<PidSpec> pidList, PidSpec newPID)
        {
            foreach (PidSpec oldPID in pidList)
            {
                if (oldPID.Pid == newPID.Pid)
                    return;

                if (oldPID.Pid > newPID.Pid)
                {
                    pidList.Insert(pidList.IndexOf(oldPID), newPID);
                    return;
                }
            }

            pidList.Add(newPID);
        }

        private void runWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cmdScan.Text = "Start Find";

            if (e.Error != null)
                throw new InvalidOperationException("Background worker failed - see inner exception", e.Error);

            lblScanning.Visible = false;
            pbarProgress.Enabled = false;
            pbarProgress.Visible = false;
            MainWindow.ChangeMenuItemAvailability(true);

            if (pidList == null)
                return;

            processResults();

            btSetUpFind.Visible = true;
            dgViewResults.Visible = true;
            cmdScan.Visible = false;            

            MessageBox.Show("The search for EPG data has been completed.",
                "EPG Centre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void processResults()
        {
            if (dgViewResults.Rows.Count != 0)
                dgViewResults.Rows.Add();

            checkForEIT(dgViewResults);
            checkForMHEG5(dgViewResults);            
            checkForOpenTV(dgViewResults);
            checkForFreeSat(dgViewResults);
            checkForMHW1(dgViewResults);
            checkForMHW2(dgViewResults);
            checkForNagraGuide(dgViewResults);
            checkForSiehFernInfo(dgViewResults);
            checkForATSCPSIP(dgViewResults);
            checkForDishNetwork(dgViewResults);
            checkForBellTV(dgViewResults);

            dgViewResults.FirstDisplayedScrollingRowIndex = dgViewResults.Rows.Count - 1;
        }

        private void checkForEIT(DataGridView dgViewResults)
        {
            Logger.Instance.Write("Checking EIT data");

            DataGridViewRow row = new DataGridViewRow();
            row.Height = 16;

            DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
            frequencyCell.Value = analysisParameters.ScanningFrequency.ToString();
            row.Cells.Add(frequencyCell);

            DataGridViewCell epgTypeCell = new DataGridViewTextBoxCell();
            epgTypeCell.Value = "EIT";
            row.Cells.Add(epgTypeCell);

            Collection<int> nowNextPids = new Collection<int>();
            Collection<int> schedulePids = new Collection<int>();

            bool firstNowNext = true;
            bool firstSchedule = true;

            foreach (PidSpec pidSpec in pidList)
            {
                foreach (int table in pidSpec.Tables)
                {
                    if (table == 0x4e || table == 0x4f)
                        addAnalysisPid(nowNextPids, pidSpec.Pid);
                    else
                    {
                        if (table >= 0x50 && table <= 0x6f)
                            addAnalysisPid(schedulePids, pidSpec.Pid);
                    }
                }
            }

            StringBuilder comment = new StringBuilder();

            if (nowNextPids.Count == 0)
            {
                if (schedulePids.Count == 0)
                    comment.Append("Not found");
                else
                {
                    foreach (int pid in schedulePids)
                    {
                        if (firstNowNext)
                        {
                            comment.Append("Now/next not found; Schedule on PID(s) ");
                            firstNowNext = false;
                        }
                        else
                            comment.Append(", ");
                        comment.Append("0x" + pid.ToString("X"));
                    }
                }
            }
            else
            {
                foreach (int pid in nowNextPids)
                {
                    if (firstNowNext)
                    {
                        comment.Append("Now/next on PID(s) ");
                        firstNowNext = false;
                    }
                    else
                        comment.Append(", ");
                    comment.Append("0x" + pid.ToString("X"));
                }

                if (schedulePids.Count == 0)
                    comment.Append("; Schedule not found");
                else
                {
                    foreach (int pid in schedulePids)
                    {
                        if (firstSchedule)
                        {
                            comment.Append("; Schedule on PID(s) ");
                            firstSchedule = false;
                        }
                        else
                            comment.Append(", ");
                        comment.Append("0x" + pid.ToString("X"));
                    }
                }
            }

            DataGridViewCell commentCell = new DataGridViewTextBoxCell();
            commentCell.Value = comment.ToString();
            row.Cells.Add(commentCell);

            dgViewResults.Rows.Add(row);
        }

        private void addAnalysisPid(Collection<int> pids, int newPid)
        {
            foreach (int oldPid in pids)
            {
                if (oldPid == newPid)
                    return;

                if (oldPid > newPid)
                {
                    pids.Insert(pids.IndexOf(oldPid), newPid);
                    return;
                }
            }

            pids.Add(newPid);
        }

        private void checkForMHEG5(DataGridView dgViewResults)
        {
            Logger.Instance.Write("Checking MHEG5 data");

            DataGridViewRow row = new DataGridViewRow();
            row.Height = 16;

            DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
            frequencyCell.Value = string.Empty;
            row.Cells.Add(frequencyCell);

            DataGridViewCell epgTypeCell = new DataGridViewTextBoxCell();
            epgTypeCell.Value = "MHEG5";
            row.Cells.Add(epgTypeCell);

            Collection<int> mheg5Pids = new Collection<int>();

            foreach (TVStation station in stations)
            {
                if (station.DSMCCPID != 0)
                    addAnalysisPid(mheg5Pids, station.DSMCCPID);
            }

            DataGridViewCell commentCell = new DataGridViewTextBoxCell();
            
            if (mheg5Pids.Count == 0)
                commentCell.Value = "Not found";
            else
            {
                StringBuilder pidString = new StringBuilder();

                foreach (int pid in mheg5Pids)
                {
                    if (pidString.Length == 0)
                        pidString.Append("Found on PID(s) 0x" + pid.ToString("x"));
                    else
                        pidString.Append(", 0x" + pid.ToString("X"));
                }

                commentCell.Value = pidString.ToString();
            }

            row.Cells.Add(commentCell);

            dgViewResults.Rows.Add(row);
        }

        private void checkForOpenTV(DataGridView dgViewResults)
        {
            Logger.Instance.Write("Checking OpenTV data");

            DataGridViewRow row = new DataGridViewRow();
            row.Height = 16;

            DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
            frequencyCell.Value = string.Empty;
            row.Cells.Add(frequencyCell);

            DataGridViewCell epgTypeCell = new DataGridViewTextBoxCell();
            epgTypeCell.Value = "OpenTV";
            row.Cells.Add(epgTypeCell);            

            bool pid30 = false;
            bool pid31 = false;
            bool pid32 = false;
            bool pid33 = false;
            bool pid34 = false;
            bool pid35 = false;
            bool pid36 = false;
            bool pid37 = false;
            bool pid40 = false;
            bool pid41 = false;
            bool pid42 = false;
            bool pid43 = false;
            bool pid44 = false;
            bool pid45 = false;
            bool pid46 = false;
            bool pid47 = false;

            bool reply = false;

            foreach (PidSpec pidSpec in pidList)
            {
                switch (pidSpec.Pid)
                {
                    case 0x30:
                        reply = checkTable(pidSpec, 0xa0, 0xa3);
                        if (reply)
                            pid30 = true;
                        break;
                    case 0x31:
                        reply = checkTable(pidSpec, 0xa0, 0xa3);
                        if (reply)
                            pid31 = true;
                        break;
                    case 0x32:
                        reply = checkTable(pidSpec, 0xa0, 0xa3);
                        if (reply)
                            pid32 = true;
                        break;
                    case 0x33:
                        reply = checkTable(pidSpec, 0xa0, 0xa3);
                        if (reply)
                            pid33 = true;
                        break;
                    case 0x34:
                        reply = checkTable(pidSpec, 0xa0, 0xa3);
                        if (reply)
                            pid34 = true;
                        break;
                    case 0x35:
                        reply = checkTable(pidSpec, 0xa0, 0xa3);
                        if (reply)
                            pid35 = true;
                        break;
                    case 0x36:
                        reply = checkTable(pidSpec, 0xa0, 0xa3);
                        if (reply)
                            pid36 = true;
                        break;
                    case 0x37:
                        reply = checkTable(pidSpec, 0xa0, 0xa3);
                        if (reply)
                            pid37 = true;
                        break;
                    case 0x40:
                        reply = checkTable(pidSpec, 0xa8, 0xab);
                        if (reply)
                            pid40 = true;
                        break;
                    case 0x41:
                        reply = checkTable(pidSpec, 0xa8, 0xab);
                        if (reply)
                            pid41 = true;
                        break;
                    case 0x42:
                        reply = checkTable(pidSpec, 0xa8, 0xab);
                        if (reply)
                            pid42 = true;
                        break;
                    case 0x43:
                        reply = checkTable(pidSpec, 0xa8, 0xab);
                        if (reply)
                            pid43 = true;
                        break;
                    case 0x44:
                        reply = checkTable(pidSpec, 0xa8, 0xab);
                        if (reply)
                            pid44 = true;
                        break;
                    case 0x45:
                        reply = checkTable(pidSpec, 0xa8, 0xab);
                        if (reply)
                            pid45 = true;
                        break;
                    case 0x46:
                        reply = checkTable(pidSpec, 0xa8, 0xab);
                        if (reply)
                            pid46 = true;
                        break;
                    case 0x47:
                        reply = checkTable(pidSpec, 0xa8, 0xab);
                        if (reply)
                            pid47 = true;
                        break;
                    default:
                        break;
                }
            }

            DataGridViewCell commentCell = new DataGridViewTextBoxCell();

            if (pid30 && pid31 && pid32 && pid33 && pid34 && pid35 && pid36 && pid37 &&
                pid40 && pid41 && pid42 && pid43 && pid44 && pid45 && pid46 && pid47)
                commentCell.Value = "Found";
            else
                commentCell.Value = "Not found";
            row.Cells.Add(commentCell);   

            dgViewResults.Rows.Add(row);
        }

        private bool checkTable(PidSpec pidSpec, int minTable, int maxTable)
        {
            foreach (int table in pidSpec.Tables)
            {
                if (table >= minTable && table <= maxTable)
                    return (true);
            }

            return (false);
        }

        private void checkForFreeSat(DataGridView dgViewResults)
        {
            Logger.Instance.Write("Checking FreeSat data");

            DataGridViewRow row = new DataGridViewRow();
            row.Height = 16;

            DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
            frequencyCell.Value = string.Empty;
            row.Cells.Add(frequencyCell);

            DataGridViewCell epgTypeCell = new DataGridViewTextBoxCell();
            epgTypeCell.Value = "FreeSat";
            row.Cells.Add(epgTypeCell);

            bool bba = false;
            bool bbb = false;
            bool f01 = false;
            bool f02 = false;

            foreach (PidSpec pidSpec in pidList)
            {
                switch (pidSpec.Pid)
                {
                    case 0xbba:
                        bba = true;
                        break;
                    case 0xbbb:
                        bbb = true;
                        break;
                    case 0xf01:
                        f01 = true;
                        break;
                    case 0xf02:
                        f02 = true;
                        break;
                    default:
                        break;
                }
            }

            DataGridViewCell commentCell = new DataGridViewTextBoxCell();

            if ((bba && bbb) || (f01 && f02))
                commentCell.Value = "Found";
            else
                commentCell.Value = "Not found";
            row.Cells.Add(commentCell);

            dgViewResults.Rows.Add(row);
        }

        private void checkForMHW1(DataGridView dgViewResults)
        {
            Logger.Instance.Write("Checking MediaHighway1 data");

            DataGridViewRow row = new DataGridViewRow();
            row.Height = 16;

            DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
            frequencyCell.Value = string.Empty;
            row.Cells.Add(frequencyCell);

            DataGridViewCell epgTypeCell = new DataGridViewTextBoxCell();
            epgTypeCell.Value = "MediaHighway1";
            row.Cells.Add(epgTypeCell);

            bool titles = false;
            bool table90 = false;
            bool table91 = false;
            bool table92 = false;

            foreach (PidSpec pidSpec in pidList)
            {
                switch (pidSpec.Pid)
                {
                    case 0xd2:
                        foreach (int table in pidSpec.Tables)
                        {
                            if (table == 0x90)
                                titles = true;
                        }
                        break;
                    case 0xd3:
                        foreach (int table in pidSpec.Tables)
                        {
                            switch (table)
                            {
                                case 0x90:
                                    table90 = true;
                                    break;
                                case 0x91:
                                    table91 = true;
                                    break;
                                case 0x92:
                                    table92 = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            DataGridViewCell commentCell = new DataGridViewTextBoxCell();

            if (titles && table90 && table91 && table92)
                commentCell.Value = "Found";
            else
                commentCell.Value = "Not found";
            row.Cells.Add(commentCell);

            dgViewResults.Rows.Add(row);
        }

        private void checkForMHW2(DataGridView dgViewResults)
        {
            Logger.Instance.Write("Checking MediaHighway2 data");

            DataGridViewRow row = new DataGridViewRow();
            row.Height = 16;

            DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
            frequencyCell.Value = string.Empty;
            row.Cells.Add(frequencyCell);

            DataGridViewCell epgTypeCell = new DataGridViewTextBoxCell();
            epgTypeCell.Value = "MediaHighway2";
            row.Cells.Add(epgTypeCell);

            bool channelsThemes = false;
            bool titles = false;
            bool summaries = false;

            foreach (PidSpec pidSpec in pidList)
            {
                switch (pidSpec.Pid)
                {
                    case 0x231:
                        foreach (int table in pidSpec.Tables)
                        {
                            if (table == 0xc8)
                                channelsThemes = true;
                        }
                        break;
                    case 0x234:
                        foreach (int table in pidSpec.Tables)
                        {
                            if (table == 0xe6)
                                titles = true;
                        }
                        break;
                    case 0x236:
                        foreach (int table in pidSpec.Tables)
                        {
                            if (table == 0x96)
                                summaries = true;
                        }
                        break;
                    default:
                        break;
                }
            }

            DataGridViewCell commentCell = new DataGridViewTextBoxCell();

            if (channelsThemes && titles && summaries)
                commentCell.Value = "Found";
            else
                commentCell.Value = "Not found";
            row.Cells.Add(commentCell);

            dgViewResults.Rows.Add(row);
        }

        private void checkForNagraGuide(DataGridView dgViewResults)
        {
            Logger.Instance.Write("Checking NagraGuide data");

            DataGridViewRow row = new DataGridViewRow();
            row.Height = 16;

            DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
            frequencyCell.Value = string.Empty;
            row.Cells.Add(frequencyCell);

            DataGridViewCell epgTypeCell = new DataGridViewTextBoxCell();
            epgTypeCell.Value = "NagraGuide";
            row.Cells.Add(epgTypeCell);

            DataGridViewCell commentCell = new DataGridViewTextBoxCell();
            commentCell.Value = "Not found";

            foreach (PidSpec pidSpec in pidList)
            {
                if (pidSpec.Pid == 0xc8)
                {
                    foreach (int table in pidSpec.Tables)
                    {
                        if (table == 0xb0)
                        {
                            commentCell.Value = "Found";
                            break;
                        }
                    }
                }
            }

            row.Cells.Add(commentCell);

            dgViewResults.Rows.Add(row);
        }

        private void checkForSiehFernInfo(DataGridView dgViewResults)
        {
            Logger.Instance.Write("Checking SiehFern Info data");

            DataGridViewRow row = new DataGridViewRow();
            row.Height = 16;

            DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
            frequencyCell.Value = string.Empty;
            row.Cells.Add(frequencyCell);

            DataGridViewCell epgTypeCell = new DataGridViewTextBoxCell();
            epgTypeCell.Value = "SiehFern Info";
            row.Cells.Add(epgTypeCell);

            DataGridViewCell commentCell = new DataGridViewTextBoxCell();
            commentCell.Value = "Not found";

            foreach (PidSpec pidSpec in pidList)
            {
                if (pidSpec.Pid == 0x711)
                {
                    if (pidSpec.Tables.Contains(0x3e))
                    {
                        commentCell.Value = "Found";
                        break;
                    }
                }
            }

            row.Cells.Add(commentCell);

            dgViewResults.Rows.Add(row);
        }

        private void checkForATSCPSIP(DataGridView dgViewResults)
        {
            Logger.Instance.Write("Checking ATSC PSIP data");

            DataGridViewRow row = new DataGridViewRow();
            row.Height = 16;

            DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
            frequencyCell.Value = string.Empty;
            row.Cells.Add(frequencyCell);

            DataGridViewCell epgTypeCell = new DataGridViewTextBoxCell();
            epgTypeCell.Value = "ATSC PSIP";
            row.Cells.Add(epgTypeCell);

            bool masterGuideTable = false;
            bool virtualChannelTable = false;
            bool eitTable = false;

            foreach (PidSpec pidSpec in pidList)
            {
                switch (pidSpec.Pid)
                {
                    case 0x1ffb:
                        if (pidSpec.Tables.Contains(0xc7))
                            masterGuideTable = true;
                        if (pidSpec.Tables.Contains(0xc8))
                            virtualChannelTable = true;
                        break;
                    default:
                        if (pidSpec.Tables.Contains(0xcb))
                            eitTable = true;
                        break;
                }
            }

            DataGridViewCell commentCell = new DataGridViewTextBoxCell();

            if (masterGuideTable && virtualChannelTable && eitTable)
                commentCell.Value = "Found";
            else
                commentCell.Value = "Not found";
            row.Cells.Add(commentCell);

            dgViewResults.Rows.Add(row);
        }

        private void checkForDishNetwork(DataGridView dgViewResults)
        {
            Logger.Instance.Write("Checking Dish Network data");

            DataGridViewRow row = new DataGridViewRow();
            row.Height = 16;

            DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
            frequencyCell.Value = string.Empty;
            row.Cells.Add(frequencyCell);

            DataGridViewCell epgTypeCell = new DataGridViewTextBoxCell();
            epgTypeCell.Value = "Dish Network";
            row.Cells.Add(epgTypeCell);

            int correctTables = 0;
            int incorrectTables = 0;

            foreach (PidSpec pidSpec in pidList)
            {
                if (pidSpec.Pid == 0x300)
                {
                    foreach (int table in pidSpec.Tables)
                    {
                        if (table > 0x80 && table < 0xa5)
                            correctTables++;
                    }
                }
            }

            DataGridViewCell commentCell = new DataGridViewTextBoxCell();

            if (correctTables != 0 && incorrectTables == 0)
                commentCell.Value = "Found";
            else
                commentCell.Value = "Not found";
            row.Cells.Add(commentCell);

            dgViewResults.Rows.Add(row);
        }

        private void checkForBellTV(DataGridView dgViewResults)
        {
            Logger.Instance.Write("Checking Bell TV data");

            DataGridViewRow row = new DataGridViewRow();
            row.Height = 16;

            DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
            frequencyCell.Value = string.Empty;
            row.Cells.Add(frequencyCell);

            DataGridViewCell epgTypeCell = new DataGridViewTextBoxCell();
            epgTypeCell.Value = "Bell TV";
            row.Cells.Add(epgTypeCell);

            int correctTables = 0;
            int incorrectTables = 0;

            foreach (PidSpec pidSpec in pidList)
            {
                if (pidSpec.Pid == 0x441)
                {
                    foreach (int table in pidSpec.Tables)
                    {
                        if (table > 0x80 && table < 0xa5)
                            correctTables++;                        
                    }
                }
            }

            DataGridViewCell commentCell = new DataGridViewTextBoxCell();

            if (correctTables != 0 && incorrectTables == 0)
                commentCell.Value = "Found";
            else
                commentCell.Value = "Not found";
            row.Cells.Add(commentCell);

            dgViewResults.Rows.Add(row);
        }

        private AnalysisParameters getAnalysisParameters()
        {
            AnalysisParameters analysisParameters = new AnalysisParameters();

            analysisParameters.Tuners = frequencySelectionControl.Tuners;
            analysisParameters.ScanningFrequency = frequencySelectionControl.SelectedFrequency;

            if (analysisParameters.ScanningFrequency as SatelliteFrequency != null)
            {
                SatelliteFrequency satelliteFrequency = (SatelliteFrequency)analysisParameters.ScanningFrequency;
                satelliteFrequency.SatelliteDish = new SatelliteDish();
                satelliteFrequency.SatelliteDish.LNBLowBandFrequency = frequencySelectionControl.LNBLowBandFrequency;
                satelliteFrequency.SatelliteDish.LNBHighBandFrequency = frequencySelectionControl.LNBHighBandFrequency;
                satelliteFrequency.SatelliteDish.LNBSwitchFrequency = frequencySelectionControl.LNBSwitchFrequency;
                satelliteFrequency.SatelliteDish.DiseqcSwitch = frequencySelectionControl.DiseqcSwitch;
            }

            analysisParameters.UseSignalPresent = frequencySelectionControl.UseSignalPresent;
            analysisParameters.SwitchAfterPlay = frequencySelectionControl.SwitchAfterPlay;
            analysisParameters.RepeatDiseqc = frequencySelectionControl.RepeatDiseqc;
            analysisParameters.SignalLockTimeout = (int)nudSignalLockTimeout.Value;
            analysisParameters.DataCollectionTimeout = (int)nudDataCollectionTimeout.Value;
            analysisParameters.DumpFileName = tbDumpFile.Text;

            return (analysisParameters);
        }

        /// <summary>
        /// Prepare to save update data.
        /// </summary>
        /// <returns>False. This function is not implemented.</returns>
        public bool PrepareToSave()
        {
            return (false);
        }

        /// <summary>
        /// Save updated data.
        /// </summary>
        /// <returns>False. This function is not implemented.</returns>
        public bool Save()
        {
            return (false);
        }

        /// <summary>
        /// Save updated data to a file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>False. This function is not implemented.</returns>
        public bool Save(string fileName)
        {
            return (false);
        }

        private class AnalysisParameters
        {
            internal Collection<int> Tuners
            {
                get { return (tuners); }
                set { tuners = value; }
            }

            internal TuningFrequency ScanningFrequency
            {
                get { return (scanningFrequency); }
                set { scanningFrequency = value; }
            }

            internal bool UseSignalPresent
            {
                get { return (useSignalPresent); }
                set { useSignalPresent = value; }
            }

            internal bool SwitchAfterPlay
            {
                get { return (switchAfterPlay); }
                set { switchAfterPlay = value; }
            }

            internal bool RepeatDiseqc
            {
                get { return (repeatDiseqc); }
                set { repeatDiseqc = value; }
            }

            internal int SignalLockTimeout
            {
                get { return (signalLockTimeout); }
                set { signalLockTimeout = value; }
            }

            internal int DataCollectionTimeout
            {
                get { return (dataCollectionTimeout); }
                set { dataCollectionTimeout = value; }
            }

            internal string DumpFileName
            {
                get { return (dumpFileName); }
                set { dumpFileName = value; }
            }

            private Collection<int> tuners;
            private TuningFrequency scanningFrequency;
            private bool useSignalPresent;
            private bool switchAfterPlay;
            private bool repeatDiseqc;
            private int signalLockTimeout;
            private int dataCollectionTimeout;
            private string dumpFileName;

            internal AnalysisParameters() { }
        }       
    }
}
