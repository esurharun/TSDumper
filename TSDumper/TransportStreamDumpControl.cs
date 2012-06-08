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
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

using DomainObjects;
using DirectShow;


namespace TSDumper
{
    /// <summary>
    /// The class that dumps a transport stream.
    /// </summary>
    public partial class TransportStreamDumpControl : UserControl, IUpdateControl
    {
        /// <summary>
        /// Get the general window heading for the data.
        /// </summary>
        public string Heading { get { return ("TSDumper - Developed by Steve Bickell, Harun Esur"); } }
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

        private BackgroundWorker workerDump = null;
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
            start_hour_textbox.Text = "0000";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browseFile = new FolderBrowserDialog();
            browseFile.Description = "TSDumper - Find Output File Directory";
            DialogResult result = browseFile.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            txtOutputFile.Text = browseFile.SelectedPath;

            /*if (!browseFile.SelectedPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                txtOutputFile.Text = Path.Combine(browseFile.SelectedPath,
                    frequencySelectionControl.SelectedFrequency.ToString() + " " +
                    DateTime.Now.ToString("ddMMyy") + " " +
                    DateTime.Now.ToString("HHmmss")) +
                    ".ts";
            else
                txtOutputFile.Text = browseFile.SelectedPath + frequencySelectionControl.SelectedFrequency.ToString() + " " +
                    DateTime.Now.ToString("ddMMyy") + " " +
                    DateTime.Now.ToString("HHmmss") +
                    ".ts";*/
        }

        private ArrayList restart_times = new ArrayList();

        private static DumpParameters last_dump_parameters = null;

        private void start_dump()
        {
            ChangeControlStatus(false);

            lblScanning.Text = "File size: 0Mb";
            lblScanning.Visible = true;
            pbarProgress.Visible = true;
            pbarProgress.Enabled = true;
          
           
            
            workerDump = new BackgroundWorker();
            workerDump.WorkerReportsProgress = true;
            workerDump.WorkerSupportsCancellation = true;
            workerDump.DoWork += new DoWorkEventHandler(doDump);


            workerDump.RunWorkerCompleted += new RunWorkerCompletedEventHandler(runWorkerCompleted);
            workerDump.ProgressChanged += new ProgressChangedEventHandler(progressChanged);

            last_dump_parameters = getDumpParameters();
            workerDump.RunWorkerAsync(last_dump_parameters);
        }
        private void ChangeControlStatus(bool status)
        {
            ChangeControlStatus(status,this);
        }

         private void ChangeControlStatus(bool status, Control c)
        
        {


            foreach (Control ctrl in c.Controls)
            {

                if (ctrl is TextBox)

                    ((TextBox)ctrl).Enabled = status;

                else if (ctrl is Button)
                {
                    if (ctrl != cmdScan)
                        ((Button)ctrl).Enabled = status;
                }
                else if (ctrl is RadioButton)

                    ((RadioButton)ctrl).Enabled = status;
                else if (ctrl is ComboBox)

                    ((ComboBox)ctrl).Enabled = status;

                else if (ctrl is NumericUpDown)

                    ((NumericUpDown)ctrl).Enabled = status;
                else if (ctrl is MaskedTextBox)

                    ((MaskedTextBox)ctrl).Enabled = status;

                else if (ctrl is CheckBox)

                    ((CheckBox) ctrl).Enabled = status;

                ChangeControlStatus(status,ctrl);
            }



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
          

                if (last_dump_parameters != null)
                {
                    string val = ScriptRunner.run_after_finish(last_dump_parameters.FileName, get_active_tuner_idx(), after_recording_complete_script_path.Text);
                }
                   

                ChangeControlStatus(true);

                
                return;
            }

            if (!checkData())
                return;
            

           
            DateTime st_time = DateTime.ParseExact(start_hour_textbox.Text, "HH:mm", CultureInfo.InvariantCulture);
            int hour = st_time.Hour;
            int minute = st_time.Minute;
            int second = 0;

            int current_second = -1;
            int current_minute = -1;
            int current_hour = -1;

            DumpParameters dp = getDumpParameters();

            restart_times.Clear();

            while (!(current_hour == hour && current_minute == minute && current_second == second ))
            {
                st_time = st_time.AddSeconds(dp.DataCollectionTimeout);
                current_hour = st_time.Hour;
                current_minute = st_time.Minute;
                current_second = st_time.Second;

                restart_times.Add(st_time.ToString("HHmmss"));
               // Logger.Instance.Write(string.Format("restart time: {0}",calc_time.ToString()));
                
            }

            Logger.Instance.Write("Dump started");

            cmdScan.Text = "Stop Dump";
            
            start_dump();
        }

        private bool checkData()
        {
            string reply = frequencySelectionControl.ValidateForm();
            if (reply != null)
            {
                MessageBox.Show(reply, "TSDumper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }           

            if (txtOutputFile.Text == null || txtOutputFile.Text == string.Empty)
            {
                MessageBox.Show("No output path entered.", "TSDumper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return(false);
            }

            string pidReply = processPids();
            if (pidReply != null)
            {
                MessageBox.Show(pidReply, "TSDumper", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private int get_active_tuner_idx()
        {
            Collection<int> selected_tuner_indexes = frequencySelectionControl.selected_tuner_indexes;
        
            if (selected_tuner_indexes.Count > 0)
                return selected_tuner_indexes[0];

            return 0;
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
                    
                BDAGraph graph = BDAGraph.FindTuner(dumpParameters.Tuners, 
                    tunerNodeType, tuningSpec, currentTuner, dumpParameters.RepeatDiseqc, dumpParameters.SwitchAfterPlay, dumpParameters.FileName);
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

            DialogResult result = MessageBox.Show(message, "TSDumper", buttons, icon);
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
            Logger.Instance.Write(string.Format("Starting dump to {0}",dumpParameters.FileName));

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

            while (!worker.CancellationPending)
            {
                Thread.Sleep(100);
                
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
            
            Logger.Instance.Write("Background worker completed");

            if (e.Error != null)
                throw new InvalidOperationException("Background workfer failed - see inner exception", e.Error);

            
                    
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
            string newFilePath = string.Format("{0}\\tsdump-{1}.ts", Path.GetPathRoot(txtOutputFile.Text),
                                               DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond);
            try
            {
                if (before_recording_complete_script_path.Text != null)
                {


                    string val = ScriptRunner.run_before_start(newFilePath, get_active_tuner_idx(), before_recording_complete_script_path.Text);

                    //MessageBox.Show(val);
                    if (val != null)
                        newFilePath = val;
                }
            } catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            dumpParameters.FileName = newFilePath;

            return (dumpParameters);
        }
        
        // <summary>
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

        private void TransportStreamDumpControl_Load(object sender, EventArgs e)
        {

        }

        private void gpOutputFile_Enter(object sender, EventArgs e)
        {

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private string select_file(string filter)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Filter = filter;
            browseFile.CheckFileExists = false;
            browseFile.Title = "TSDumper - Find script file";
            DialogResult result = browseFile.ShowDialog();
            if (result == DialogResult.Cancel)
                return null;

             return browseFile.FileName;
        }

        private void select_file(TextBox resultBox,string filter)
        {
            string selected_file = select_file(filter);

            if (selected_file != null)
                resultBox.Text = selected_file;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           select_file(after_recording_complete_script_path, "CS Files (*.cs)|*.cs");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            select_file(before_recording_complete_script_path, "CS Files (*.cs)|*.cs");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (cmdScan.Text == "Stop Dump")
            {

                string current_time = DateTime.Now.ToString("HHmmss");



                if (restart_times.IndexOf(current_time) != -1)
                {
                    Logger.Instance.Write("Stop dump requested");
                    workerDump.CancelAsync();
                    resetEvent.WaitOne(new TimeSpan(0, 0, 45));
                    

                    Thread.Sleep(1000);

                    if (last_dump_parameters != null)
                    {
                        string val = ScriptRunner.run_after_finish(last_dump_parameters.FileName, get_active_tuner_idx(), after_recording_complete_script_path.Text);
                    }
                   
                    start_dump();
                }

                
                
            }
           
            

            last_log.Items.AddRange((string[])Logger.last_log.ToArray(typeof(string)));

       
            last_log.SelectedIndex = last_log.Items.Count - 1;

            Logger.last_log.Clear();

            
        
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }



        private string save_settings()
        {
            string file = select_file("INI Files (*.ini)|*.ini");

            if (file != null)
            {
                INIFile inifile = new INIFile(file);

                Collection<int> selected_tuner_indexes = frequencySelectionControl.selected_tuner_indexes;

                string str_selected_tuner_indexes = "";

                for (int i = 0; i < selected_tuner_indexes.Count;i++ )
                {
                    str_selected_tuner_indexes += string.Format("{0}", selected_tuner_indexes[i]);

                    if (i+1 != selected_tuner_indexes.Count)
                    {
                        str_selected_tuner_indexes += ",";
                    }    
                }

                inifile.Write("default", "selected_tuners", str_selected_tuner_indexes);
                inifile.Write("default", "selected_satellite", string.Format("{0}", frequencySelectionControl.selected_satellite_index));
                inifile.Write("default", "selected_frequency", string.Format("{0}", frequencySelectionControl.selected_frequency_index));
                inifile.Write("default", "LNBLowBandFrequency", string.Format("{0}", frequencySelectionControl.LNBLowBandFrequency));
                inifile.Write("default", "LNBHighBandFrequency", string.Format("{0}", frequencySelectionControl.LNBHighBandFrequency));
                inifile.Write("default", "LNBSwitchFrequency", string.Format("{0}", frequencySelectionControl.LNBSwitchFrequency));
                inifile.Write("default", "DiseqC", frequencySelectionControl.DiseqcSwitch);

                inifile.Write("default", "UseSignalPresent", frequencySelectionControl.UseSignalPresent ? bool.TrueString : bool.FalseString);
                inifile.Write("default", "SwitchAfterPlay", frequencySelectionControl.SwitchAfterPlay ? bool.TrueString : bool.FalseString);
                inifile.Write("default", "RepeatDiseqc", frequencySelectionControl.RepeatDiseqc ? bool.TrueString : bool.FalseString);
                inifile.Write("default", "SignalLockTimeout", string.Format("{0}", (int)nudSignalLockTimeout.Value));
                inifile.Write("default", "DataCollectionTimeout", string.Format("{0}", (int)nudDataCollectionTimeout.Value));
                inifile.Write("default", "PidList", tbPidList.Text);

                inifile.Write("default", "before_recording_complete_script_path", before_recording_complete_script_path.Text);
                inifile.Write("default", "after_recording_complete_script_path", after_recording_complete_script_path.Text);
                inifile.Write("default", "start_hour", start_hour_textbox.Text);
                inifile.Write("default", "output_path", txtOutputFile.Text);

                MessageBox.Show(string.Format("Settings successfully save to {0}", file));

            }

            return file;
        }

        private bool load_settings()
        {

            string file = select_file( "INI Files (*.ini)|*.ini");

            if (file != null)
            {
                try
                {
                        INIFile inifile = new INIFile(file);

                        string str_selected_tuner_indexes = inifile.Read("default", "selected_tuners");

                        string[] parts = str_selected_tuner_indexes.Split(',');
                        Collection<int> selected_tuner_indexes = new Collection<int>();
                        foreach (string part in parts)
                        {
                                selected_tuner_indexes.Add(Int32.Parse(part, NumberStyles.Integer));    
                        }

                        frequencySelectionControl.selected_tuner_indexes = selected_tuner_indexes;
                        frequencySelectionControl.selected_satellite_index = Int32.Parse(inifile.Read("default", "selected_satellite"), NumberStyles.Integer);
                        frequencySelectionControl.selected_frequency_index = Int32.Parse(inifile.Read("default", "selected_frequency"), NumberStyles.Integer);
                        frequencySelectionControl.LNBLowBandFrequency = Int32.Parse(inifile.Read("default", "LNBLowBandFrequency"), NumberStyles.Integer);
                        frequencySelectionControl.LNBHighBandFrequency = Int32.Parse(inifile.Read("default", "LNBHighBandFrequency"), NumberStyles.Integer);
                        frequencySelectionControl.LNBSwitchFrequency = Int32.Parse(inifile.Read("default", "LNBSwitchFrequency"), NumberStyles.Integer);
                        frequencySelectionControl.DiseqcSwitch = inifile.Read("default", "DiseqC");
                        frequencySelectionControl.UseSignalPresent = bool.Parse(inifile.Read("default", "UseSignalPresent"));
                        frequencySelectionControl.SwitchAfterPlay = bool.Parse(inifile.Read("default", "SwitchAfterPlay"));
                        frequencySelectionControl.RepeatDiseqc = bool.Parse(inifile.Read("default", "RepeatDiseqc"));
                        nudSignalLockTimeout.Value = Int32.Parse(inifile.Read("default", "SignalLockTimeout"));
                        nudDataCollectionTimeout.Value = Int32.Parse(inifile.Read("default", "DataCollectionTimeout"));
                        tbPidList.Text = inifile.Read("default", "PidList");

                        before_recording_complete_script_path.Text = inifile.Read("default", "before_recording_complete_script_path");
                        after_recording_complete_script_path.Text = inifile.Read("default", "after_recording_complete_script_path");
                        start_hour_textbox.Text = inifile.Read("default", "start_hour" );
                        txtOutputFile.Text = inifile.Read("default", "output_path");


                   // MessageBox.Show("Settings loaded successfully");
                } catch(Exception e)
                {
                    return false;
                }
                return true;
            }

            return false;

        }


        private void button3_Click(object sender, EventArgs e)
        {
            load_settings();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            save_settings();
        }


    }

   


}
