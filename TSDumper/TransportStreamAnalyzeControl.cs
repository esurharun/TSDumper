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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

using DomainObjects;
using DirectShow;
using DVBServices;

namespace TSDumper
{
    /// <summary>
    /// The class that analyzes a transport stream.
    /// </summary>
    public partial class TransportStreamAnalyzeControl : UserControl, IUpdateControl
    {
        /// <summary>
        /// Get the general window heading for the data.
        /// </summary>
        public string Heading { get { return ("TSDumper - Analyze Transport Stream"); } }
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
        
        public TransportStreamAnalyzeControl()
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
            btSetUpAnalysis.Visible = false;

            cmdScan.Visible = true;
        }

        private void cmdScan_Click(object sender, EventArgs e)
        {
            if (cmdScan.Text == "Stop Analysis")
            {
                Logger.Instance.Write("Stop analysis requested");
                workerAnalyze.CancelAsync();
                resetEvent.WaitOne(new TimeSpan(0, 0, 45));
                cmdScan.Text = "Start Analysis";
                lblScanning.Visible = false;
                pbarProgress.Visible = false;
                pbarProgress.Enabled = false;
                
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

            cmdScan.Text = "Stop Analysis";
            lblScanning.Text = "Scanning " + analysisParameters.ScanningFrequency.ToString();
            lblScanning.Visible = true;
            pbarProgress.Visible = true;
            pbarProgress.Enabled = true;
            

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
                MessageBox.Show(reply, "TSDumper", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Logger.Instance.Write("Scan abandoned by user");
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
                        Logger.Instance.Write("Scan abandoned by user");
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

            DialogResult result = MessageBox.Show(message, "TSDumper", buttons, icon);
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
                    Logger.Instance.Write("Analysis abandoned by user");
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
            Collection<TVStation> stations = frequencyScanner.FindTVStations();

            pidList = new Collection<PidSpec>();

            dataProvider.ChangePidMapping(new int[] { -1 });            

            IntPtr memoryPointer = dataProvider.BufferAddress;
            int currentOffset = 0;            

            byte[] buffer = new byte[188];
            DateTime startTime = DateTime.Now;
            int packetCount = 0;
            int errorPackets = 0;
            int nullPackets = 0;            

            while ((DateTime.Now - startTime).TotalSeconds < analysisParameters.DataCollectionTimeout && !worker.CancellationPending)
            {
                if (currentOffset >= dataProvider.BufferSpaceUsed)
                {
                    Thread.Sleep(2000);
                    if (currentOffset >= dataProvider.BufferSpaceUsed)
                    {
                        Logger.Instance.Write("Analysis resetting pid after " + packetCount + " packets (errors = " + errorPackets + " null = " + nullPackets + ")");
                        dataProvider.ChangePidMapping(new int[] { -1 });
                        currentOffset = 0;
                    }
                }
                else
                {
                    IntPtr currentPointer = new IntPtr(memoryPointer.ToInt64() + currentOffset + 136);
                    Marshal.Copy(currentPointer, buffer, 0, 188);
                    packetCount++;

                    /*if (dumpCount < 10000)
                    {
                        Logger.Instance.Dump("atsc " + dumpCount, buffer, buffer.Length);
                        dumpCount++;
                    }*/

                    TransportPacket transportPacket = new TransportPacket();

                    try
                    {
                        transportPacket.Process(buffer);

                        if (transportPacket.ErrorIndicator)
                            errorPackets++;
                        if (transportPacket.IsNullPacket)
                            nullPackets++;

                        if (!transportPacket.ErrorIndicator)
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
                        else
                            Logger.Instance.Write("Transport packet error in packet " + packetCount);

                    }
                    catch (ArgumentOutOfRangeException) 
                    {
                        Logger.Instance.Write("Failed to parse packet " + packetCount);
                    }

                    currentOffset += buffer.Length;
                }
            }

            Logger.Instance.Write("Analysis completed: " + pidList.Count + " PID's loaded from " + packetCount + " packets");
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
            cmdScan.Text = "Start Analysis";

            if (e.Error != null)
                throw new InvalidOperationException("Background worker failed - see inner exception", e.Error);

            lblScanning.Visible = false;
            pbarProgress.Enabled = false;
            pbarProgress.Visible = false;
          

            if (pidList == null)
                return;

            processResults();

            btSetUpAnalysis.Visible = true;
            dgViewResults.Visible = true;
            cmdScan.Visible = false;

            MessageBox.Show("The transport stream analysis has been completed.",
                "TSDumper",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void processResults()
        {
            if (dgViewResults.Rows.Count != 0)
                dgViewResults.Rows.Add();

            bool firstPid = true;

            foreach (PidSpec pidSpec in pidList)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Height = 16;

                DataGridViewCell frequencyCell = new DataGridViewTextBoxCell();
                if (firstPid)
                {
                    frequencyCell.Value = analysisParameters.ScanningFrequency.ToString();
                    firstPid = false;
                }
                else
                    frequencyCell.Value = string.Empty;
                row.Cells.Add(frequencyCell);

                DataGridViewCell pidCell = new DataGridViewTextBoxCell();
                pidCell.Value = decodePid(pidSpec.Pid);
                row.Cells.Add(pidCell);

                if (pidSpec.Tables.Count != 0)
                {
                    StringBuilder tableString = new StringBuilder();

                    bool multipleAdded = false;

                    foreach (int table in pidSpec.Tables)
                    {
                        if (pidSpec.Pid != 0x300 && pidSpec.Pid != 0x441)
                        {
                            if (tableString.Length != 0)
                                tableString.Append("; ");
                            tableString.Append(decodeTable(pidSpec.Pid, table));
                        }
                        else
                        {
                            if (table > 0x80 && table < 0xa5)
                            {
                                if (!multipleAdded)
                                {
                                    if (tableString.Length != 0)
                                        tableString.Append("; ");
                                    tableString.Append(decodeTable(pidSpec.Pid, table));
                                    multipleAdded = true;
                                }
                            }
                            else
                            {
                                if (tableString.Length != 0)
                                    tableString.Append("; ");
                                tableString.Append(decodeTable(pidSpec.Pid, table));
                            }
                        }
                    }

                    DataGridViewCell tableCell = new DataGridViewTextBoxCell();
                    tableCell.Value = tableString.ToString();
                    row.Cells.Add(tableCell);

                    Logger.Instance.Write("PID: " + decodePid(pidSpec.Pid) + " Tables: " + tableString.ToString());
                }
                else
                    Logger.Instance.Write("PID: " + decodePid(pidSpec.Pid) + " Tables: N/A");

                dgViewResults.Rows.Add(row);
            }

            if (dgViewResults.Rows.Count != 0)
                dgViewResults.FirstDisplayedScrollingRowIndex = dgViewResults.Rows.Count - 1;
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

        private string decodePid(int pid)
        {
            switch (pid)
            {
                case 0x00:
                    return ("0x00 Program association");
                case 0x01:
                    return ("0x01 Conditional access");
                case 0x10:
                    return ("0x10 Network information");
                case 0x11:
                    return ("0x11 Service description");
                case 0x12:
                    return ("0x12 Event information");
                case 0x13:
                    return ("0x13 Running status");
                case 0x14:
                    return ("0x14 Time and date");
                case 0x30:
                case 0x31:
                case 0x32:
                case 0x33:
                case 0x34:
                case 0x35:
                case 0x36:
                case 0x37:
                case 0x40:
                case 0x41:
                case 0x42:
                case 0x43:
                case 0x44:
                case 0x45:
                case 0x46:
                case 0x47:
                    return ("0x" + pid.ToString("X").ToLowerInvariant() + " OpenTV");
                case 0xc8:
                    return ("0xc8 NagraGuide");
                case 0xd2:                    
                case 0xd3:
                    return ("0x" + pid.ToString("X").ToLowerInvariant() + " MediaHighway1");
                case 0x231:
                case 0x234:
                case 0x236:
                    return ("0x" + pid.ToString("X").ToLowerInvariant() + " MediaHighway2");
                case 0x300:
                    return ("0x" + pid.ToString("X").ToLowerInvariant() + " Dish Network");
                case 0x441:
                    return ("0x" + pid.ToString("X").ToLowerInvariant() + " Bell TV");
                case 0x711:
                    return ("0x711 SiehFern Info");
                case 0xbba:
                case 0xbbb:
                case 0xf01:
                case 0xf02:
                    return ("0x" + pid.ToString("X").ToLowerInvariant() + " FreeSat");                
                case 0x1ffb:
                    return ("0x1ffb ATSC PSIP");
                case 0x1ffc:
                    return ("0x1ffc SCTE");
                case 0x1fff:
                    return ("0x1fff Null packet");
                default:
                    return ("0x" + pid.ToString("X").ToLowerInvariant() + " Unknown");
            }
        }

        private string decodeTable(int pid, int table)
        {
            if (pid == 0x300)
            {
                if (table > 0x80 && table < 0xa5)
                    return ("0x81 - 0xa4 Dish Network");
            }
            else
            {
                if (pid == 0x441)
                {
                    if (table > 0x80 && table < 0xa5)
                        return ("0x81 - 0xa4 Bell TV");
                }
            }

            switch (table)
            {
                case 0x00:
                    return ("0x00 Program association");
                case 0x01:
                    return ("0x01 Conditional access");
                case 0x02:
                    return ("0x02 Program map");
                case 0x03:
                    return ("0x03 Tranport stream description");
                case 0x3b:
                    return ("0x3b MHEG5 control");
                case 0x3c:
                    return ("0x3c MHEG5 data");
                case 0x3e:
                    return ("0x3e SiehFern Info");
                case 0x40:
                    return ("0x40 Network information (current transport)");
                case 0x41:
                    return ("0x41 Network information (other transport)");
                case 0x42:
                    return ("0x42 Service description (current transport)");
                case 0x46:
                    return ("0x46 Service description (other transport)");
                case 0x4a:
                    return ("0x4a Bouquet association");
                case 0x4e:
                    return ("0x4e Event information now/next (current transport)");
                case 0x4f:
                    return ("0x4f Event information now/next (other transport)");
                case 0x50:
                case 0x51:
                case 0x52:
                case 0x53:
                case 0x54:
                case 0x55:
                case 0x56:
                case 0x57:
                case 0x58:
                case 0x59:
                case 0x5a:
                case 0x5b:
                case 0x5c:
                case 0x5d:
                case 0x5e:
                case 0x5f:
                    return ("0x" + table.ToString("X").ToLowerInvariant() + " Event information schedule (current transport)");
                case 0x60:
                case 0x61:
                case 0x62:
                case 0x63:
                case 0x64:
                case 0x65:
                case 0x66:
                case 0x67:
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6b:
                case 0x6c:
                case 0x6d:
                case 0x6e:
                case 0x6f:
                    return ("0x" + table.ToString("X").ToLowerInvariant() + " Event information schedule (other transport)");
                case 0x70:
                    return ("0x70 Time and date");
                case 0x71:
                    return ("0x71 Running status");
                case 0x72:
                    return ("0x72 Stuffing");
                case 0x73:
                    return ("0x73 Time offset");
                case 0x90:
                case 0x91:
                case 0x92:
                    return ("0x" + table.ToString("X").ToLowerInvariant() + " MediaHighway1");                
                case 0xe6:
                case 0x96:
                    return ("0x" + table.ToString("X").ToLowerInvariant() + " MediaHighway2");
                case 0xa0:
                case 0xa1:
                case 0xa2:
                case 0xa3:
                case 0xa8:
                case 0xa9:
                case 0xaa:
                case 0xab:                
                    return ("0x" + table.ToString("X").ToLowerInvariant() + " OpenTV");
                case 0xb0:
                    return ("0xb0 NagraGuide");
                case 0xc2:
                    return ("0xc2 SCTE NIT");
                case 0xc3:
                    return ("0xc3 SCTE NTT");
                case 0xc4:
                    return ("0xc4 SCTE S-VCT");
                case 0xc5:
                    return ("0xc2 SCTE STT");
                case 0xc7:
                    if (pid == 0x1ffb)
                        return ("0xc7 ATSC PSIP MGT");
                    else
                        return ("0xc7 SCTE MGT");
                case 0xc8:
                    return ("0xc8 MediaHighway2/ATSC PSIP VCT");
                case 0xc9:
                    return ("0xc9 SCTE L-VCT");
                case 0xca:
                    if (pid == 0x1ffb)
                        return ("0xca ATSC PSIP RRT");
                    else
                        return ("0xca SCTE RRT");
                case 0xcb:
                    return ("0xcb ATSC PSIP EIT");
                case 0xcc:
                    return ("0xcc ATSC PSIP ETT");
                case 0xcd:
                    return ("0xcd ATSC PSIP STT"); 
                case 0xd3:
                    return ("0xd3 ATSC PSIP DCCT");
                case 0xd4:
                    return ("0xd4 ATSC PSIP DCCSCT");
                case 0xd6:
                    return ("0xd6 SCTE AEIT");
                case 0xd7:
                    return ("0xd7 SCTE AETT");
                case 0xd8:
                    return ("0xd8 SCTE CEAM"); 
                default:
                    return ("0x" + table.ToString("X").ToLowerInvariant() + " Unknown");
            }
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
