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

namespace EPGCentre
{
    /// <summary>
    /// The class that dumps a transport stream.
    /// </summary>
    public partial class TransportStreamDumpControl : UserControl, IUpdateControl
    {
        /// <summary>
        /// Get the general window heading for the data.
        /// </summary>
        public string Heading { get { return ("EPG Centre - Dump Transport Stream"); } }
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

        private Collection<int> pidList;

        private BackgroundWorker workerDump;
        private AutoResetEvent resetEvent = new AutoResetEvent(false); 

        private double finalSize;

        /// <summary>
        /// Initialize a new instance of the TransportStreamDumpControl class.
        /// </summary>
        public TransportStreamDumpControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Process the dump.
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

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browseFile = new FolderBrowserDialog();
            browseFile.Description = "EPG Centre - Find Output File Directory";
            DialogResult result = browseFile.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            if (!browseFile.SelectedPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                txtOutputFile.Text = Path.Combine(browseFile.SelectedPath,
                    frequencySelectionControl.SelectedFrequency.ToString() + " " +
                    DateTime.Now.ToString("ddMMyy") + " " +
                    DateTime.Now.ToString("HHmmss")) +
                    ".ts";
            else
                txtOutputFile.Text = browseFile.SelectedPath + frequencySelectionControl.SelectedFrequency.ToString() + " " +
                    DateTime.Now.ToString("ddMMyy") + " " +
                    DateTime.Now.ToString("HHmmss") +
                    ".ts";
        }

        private void cmdScan_Click(object sender, EventArgs e)
        {
            if (cmdScan.Text == "Stop Dump")
            {
                Logger.Instance.Write("Stop dump requested");
                workerDump.CancelAsync();
                resetEvent.WaitOne(new TimeSpan(0, 0, 45));
                cmdScan.Text = "Start Dump";
                lblScanning.Visible = false;
                pbarProgress.Visible = false;
                pbarProgress.Enabled = false;
                MainWindow.ChangeMenuItemAvailability(true);
                return;
            }

            if (!checkData())
                return;            

            Logger.Instance.Write("Dump started");

            cmdScan.Text = "Stop Dump";
            lblScanning.Text = "File size: 0Mb";
            lblScanning.Visible = true;
            pbarProgress.Visible = true;
            pbarProgress.Enabled = true;
            MainWindow.ChangeMenuItemAvailability(false);

            workerDump = new BackgroundWorker();
            workerDump.WorkerReportsProgress = true;
            workerDump.WorkerSupportsCancellation = true;
            workerDump.DoWork += new DoWorkEventHandler(doDump);
            workerDump.RunWorkerCompleted += new RunWorkerCompletedEventHandler(runWorkerCompleted);
            workerDump.ProgressChanged += new ProgressChangedEventHandler(progressChanged);
            workerDump.RunWorkerAsync(getDumpParameters());
        }

        private bool checkData()
        {
            string reply = frequencySelectionControl.ValidateForm();
            if (reply != null)
            {
                MessageBox.Show(reply, "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }           

            if (txtOutputFile.Text == null || txtOutputFile.Text == string.Empty)
            {
                MessageBox.Show("No output path entered.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return(false);
            }

            string pidReply = processPids();
            if (pidReply != null)
            {
                MessageBox.Show(pidReply, "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }

            if (pidList == null)
            {
                DialogResult result = MessageBox.Show("No PID's have been entered so the complete transport stream will be dumped." +
                    Environment.NewLine + Environment.NewLine + "Is that correct?",
                    "EPG Centre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return (false);
            }

            if (File.Exists(txtOutputFile.Text))
            {
                DialogResult result = MessageBox.Show("The file already exists." +
                    Environment.NewLine + Environment.NewLine + "Do you want to overwrite it?",
                    "EPG Centre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return (false);
            }

            return (true);
        }

        private string processPids()
        {
            if (tbPidList.Text == null || tbPidList.Text == string.Empty)
            {
                pidList = null;
                return (null);
            }

            string[] parts = tbPidList.Text.Split(new char[] { ',' });

            pidList = new Collection<int>();

            foreach (string part in parts)
            {
                try
                {
                    int pid = 0;
                    if (!cbHexPids.Checked)
                        pid = Int32.Parse(part, NumberStyles.Integer);
                    else
                        pid = Int32.Parse(part.Replace("0x", ""), NumberStyles.HexNumber);
                    if (pid < 0 || pid > 0x1ffe)
                        return ("A PID is out of range");
                    else
                        pidList.Add(pid);
                }
                catch (FormatException)
                {
                    return ("A PID is in the wrong format");
                }
                catch (ArithmeticException)
                {
                    return ("A PID is in the wrong format");
                }
            }

            return (null);
        }

        private void progressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblScanning.Text = "File size: " + e.ProgressPercentage + "Mb";
        }

        private void doDump(object sender, DoWorkEventArgs e)
        {
            finalSize = -1;

            DumpParameters dumpParameters = e.Argument as DumpParameters;
            
            TunerNodeType tunerNodeType;
            TuningSpec tuningSpec;

            SatelliteFrequency satelliteFrequency = dumpParameters.ScanningFrequency as SatelliteFrequency;
            if (satelliteFrequency != null)
            {
                tunerNodeType = TunerNodeType.Satellite;
                tuningSpec = new TuningSpec((Satellite)satelliteFrequency.Provider, satelliteFrequency);
            }
            else
            {
                TerrestrialFrequency terrestrialFrequency = dumpParameters.ScanningFrequency as TerrestrialFrequency;
                if (terrestrialFrequency != null)
                {
                    tunerNodeType = TunerNodeType.Terrestrial;
                    tuningSpec = new TuningSpec(terrestrialFrequency);
                }
                else
                {
                    CableFrequency cableFrequency = dumpParameters.ScanningFrequency as CableFrequency;
                    if (cableFrequency != null)
                    {
                        tunerNodeType = TunerNodeType.Cable;
                        tuningSpec = new TuningSpec(cableFrequency);
                    }
                    else
                    {
                        AtscFrequency atscFrequency = dumpParameters.ScanningFrequency as AtscFrequency;
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
                            ClearQamFrequency clearQamFrequency = dumpParameters.ScanningFrequency as ClearQamFrequency;
                            if (clearQamFrequency != null)
                            {
                                tunerNodeType = TunerNodeType.Cable;
                                tuningSpec = new TuningSpec(clearQamFrequency);
                            }
                            else
                            {
                                ISDBSatelliteFrequency isdbSatelliteFrequency = dumpParameters.ScanningFrequency as ISDBSatelliteFrequency;
                                if (isdbSatelliteFrequency != null)
                                {
                                    tunerNodeType = TunerNodeType.ISDBS;
                                    tuningSpec = new TuningSpec((Satellite)satelliteFrequency.Provider, isdbSatelliteFrequency);
                                }
                                else
                                {
                                    ISDBTerrestrialFrequency isdbTerrestrialFrequency = dumpParameters.ScanningFrequency as ISDBTerrestrialFrequency;
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
                    
                BDAGraph graph = BDAGraph.FindTuner(dumpParameters.Tuners, tunerNodeType, tuningSpec, currentTuner, dumpParameters.RepeatDiseqc, dumpParameters.SwitchAfterPlay, dumpParameters.FileName);
                if (graph == null)
                {
                    Logger.Instance.Write("<e> No tuner able to tune frequency " + dumpParameters.ScanningFrequency.ToString());

                    frequencySelectionControl.Invoke(new ShowMessage(showMessage), "No tuner able to tune frequency " + dumpParameters.ScanningFrequency.ToString(),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    finished = true;
                }
                else
                {
                    string tuneReply = checkTuning(graph, dumpParameters, sender as BackgroundWorker);

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
                            getData(graph, dumpParameters, sender as BackgroundWorker);
                        }
                        catch (IOException ex)
                        {
                            Logger.Instance.Write("<e> Failed to create dump file");
                            Logger.Instance.Write("<e> " + ex.Message);
                            frequencySelectionControl.Invoke(new ShowMessage(showMessage), "Failed to create dump file." + 
                                Environment.NewLine + Environment.NewLine + ex.Message,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        graph.Dispose();
                        finished = true;
                    }
                    else
                    {
                        Logger.Instance.Write("Failed to tune frequency " + dumpParameters.ScanningFrequency.ToString());
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

        private string checkTuning(BDAGraph graph, DumpParameters dumpParameters, BackgroundWorker worker)
        {
            TimeSpan timeout = new TimeSpan();
            bool done = false;
            bool locked = false;
            int frequencyRetries = 0;

            while (!done)
            {
                if (worker.CancellationPending)
                {
                    Logger.Instance.Write("Dump abandoned by user");
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
                        if (!dumpParameters.UseSignalPresent)
                        {
                            Logger.Instance.Write("Signal not locked and signal quality not > 0");
                            Thread.Sleep(1000);
                            timeout = timeout.Add(new TimeSpan(0, 0, 1));
                            done = (timeout.TotalSeconds == dumpParameters.SignalLockTimeout);
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
                                done = (timeout.TotalSeconds == dumpParameters.SignalLockTimeout);
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
                return ("<e> The tuner failed to acquire a signal for frequency " + dumpParameters.ScanningFrequency.ToString());
        }

        private void getData(BDAGraph graph, DumpParameters dumpParameters, BackgroundWorker worker)
        {
            Logger.Instance.Write("Starting dump");

            int[] newPids;

            if (dumpParameters.PidList != null && dumpParameters.PidList.Count != 0)
            {
                newPids = new int[dumpParameters.PidList.Count];
                for (int index = 0; index < dumpParameters.PidList.Count; index++)
                    newPids[index] = dumpParameters.PidList[index];                
            }
            else
            {
                newPids = new int[1];
                newPids[0] = -1;
            }

            graph.ChangePidMapping(newPids);

            DateTime startTime = DateTime.Now;

            int lastSize = 0;

            while ((DateTime.Now - startTime).TotalSeconds < dumpParameters.DataCollectionTimeout && !worker.CancellationPending)
            {
                Thread.Sleep(1000);
                
                int increment = 5;
                if (dumpParameters.PidList != null && dumpParameters.PidList.Count != 0)
                    increment = 1;

                int size = graph.DumpFileSize / (1024 * 1024);
                if (size >= lastSize + increment)
                {
                    Logger.Instance.Write("Dump now " + size + "Mb");
                    worker.ReportProgress((int)size);
                    lastSize = size;                    
                }                
            }

            finalSize = graph.DumpFileSize;
            Logger.Instance.Write("Dump completed - file size now " + finalSize + " bytes");
        }

        private void runWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cmdScan.Text = "Start Dump";

            if (e.Error != null)
                throw new InvalidOperationException("Background worker failed - see inner exception", e.Error);

            lblScanning.Visible = false;
            pbarProgress.Enabled = false;
            pbarProgress.Visible = false;
            MainWindow.ChangeMenuItemAvailability(true);

            if (finalSize == -1)
                return;

            double fileSize = Math.Round(finalSize / (1024 * 1024), 2);

            MessageBox.Show("The transport stream dump has been completed." +
                Environment.NewLine + Environment.NewLine +
                "The file size is " + fileSize + "Mb.",
                "EPG Centre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private DumpParameters getDumpParameters()
        {
            DumpParameters dumpParameters = new DumpParameters();

            dumpParameters.Tuners = frequencySelectionControl.Tuners;
            dumpParameters.ScanningFrequency = frequencySelectionControl.SelectedFrequency;

            if (dumpParameters.ScanningFrequency as SatelliteFrequency != null)
            {
                SatelliteFrequency satelliteFrequency = (SatelliteFrequency)dumpParameters.ScanningFrequency;
                satelliteFrequency.SatelliteDish = new SatelliteDish();
                satelliteFrequency.SatelliteDish.LNBLowBandFrequency = frequencySelectionControl.LNBLowBandFrequency;
                satelliteFrequency.SatelliteDish.LNBHighBandFrequency = frequencySelectionControl.LNBHighBandFrequency;
                satelliteFrequency.SatelliteDish.LNBSwitchFrequency = frequencySelectionControl.LNBSwitchFrequency;               
                satelliteFrequency.SatelliteDish.DiseqcSwitch = frequencySelectionControl.DiseqcSwitch;                
            }

            dumpParameters.UseSignalPresent = frequencySelectionControl.UseSignalPresent;
            dumpParameters.SwitchAfterPlay = frequencySelectionControl.SwitchAfterPlay;
            dumpParameters.RepeatDiseqc = frequencySelectionControl.RepeatDiseqc;
            dumpParameters.SignalLockTimeout = (int)nudSignalLockTimeout.Value;
            dumpParameters.DataCollectionTimeout = (int)nudDataCollectionTimeout.Value;
            dumpParameters.PidList = pidList;
            dumpParameters.FileName = txtOutputFile.Text;

            return (dumpParameters);
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

        private class DumpParameters
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

            internal Collection<int> PidList
            {
                get { return (pidList); }
                set { pidList = value; }
            }

            internal string FileName
            {
                get { return (fileName); }
                set { fileName = value; }
            }

            private Collection<int> tuners;
            private TuningFrequency scanningFrequency;
            private bool useSignalPresent;
            private bool switchAfterPlay;
            private bool repeatDiseqc;
            private int signalLockTimeout;
            private int dataCollectionTimeout;
            private Collection<int> pidList;
            private string fileName;

            internal DumpParameters() { }
        }
    }
}
