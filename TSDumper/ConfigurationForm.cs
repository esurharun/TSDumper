#region References
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using DirectShow;
using DomainObjects;
using DVBServices;
#endregion

namespace EPGCollectorGUI
{
    public partial class ConfigurationForm : Form
    {
        #region Parameters & Variables
        INIFile ConfigSettings;
        private static BDAGraph graph;
        private static bool cancelGraph;
        private static bool timedOut;
        private bool createNew;
        private bool formChanged;
        private bool loadingForm = true;
        
        private static Collection<TVStation> allStations;
        private static Collection<TVStation> includedStations;

        // This BackgroundWorker is used to perform asynchronous operations.
        private BackgroundWorker backgroundWorkerScanStations;


        #endregion

        #region Constructor
        public ConfigurationForm()
        {
            InitializeComponent();
            backgroundWorkerScanStations.WorkerReportsProgress = true;
            backgroundWorkerScanStations.WorkerSupportsCancellation = true;
            this.backgroundWorkerScanStations.DoWork += new DoWorkEventHandler(backgroundWorkerScanStations_DoScan);
            this.backgroundWorkerScanStations.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerScanStations_RunWorkerCompleted);

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigurationForm_Closing);
            this.tmrProgress.Tick +=new EventHandler(tmrProgress_Tick);

            Logger.Instance.WriteSeparator("EPG Collector Configuration V2.0.15");
            this.gbSatellite.Enabled = false;
            this.cmdSelectAll.Enabled = false;
            this.cmdSelectNone.Enabled = false;
            this.rbDVBT.Checked = true; // Default value
        }
        #endregion

        #region Menu Items ...
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile(false);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Exiting with code = 0");
            System.Environment.Exit(0);
        }

        private void createNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createNew = true;
            OpenFile(true);
        }

        #endregion

        #region Methods ...
        
        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            this.gbSatellite.Enabled = false;
            this.rbDVBT.Checked = true; // Default value

            this.cmdStopScan.Enabled = false;
            this.tabControl1.Enabled = false;
            this.cboCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType)); 

            #region Tool Tips ...
            System.Windows.Forms.ToolTip ToolTipOptions = new System.Windows.Forms.ToolTip();
            string optionsHelp = "Select 1 frequency to collect the EPG data from.\nIt can be any in the drop down box.";
            ToolTipOptions.SetToolTip(this.cboFrequencies, optionsHelp);
            #endregion

            #region Options Text
            this.lblNOSIGLOCK.Text = "Occasionally it is not possible to detect that a tuner has acquired a signal. This option ";
            this.lblNOSIGLOCK.Text = this.lblNOSIGLOCK.Text + "causes the tuning mechanism to omit the check for signal acquired and to continue normally ";
            this.lblNOSIGLOCK.Text = this.lblNOSIGLOCK.Text + "after waiting 5 seconds for the signal to be acquired.";

            this.lblACCEPTBREAKS.Text = "This option is only applicable to EPG data that is collected where the broadcaster has timed ";
            this.lblACCEPTBREAKS.Text = this.lblACCEPTBREAKS.Text + "programs to allow for ad breaks. The times are then not contiguous and the program will flag ";
            this.lblACCEPTBREAKS.Text = this.lblACCEPTBREAKS.Text + "each gap as an error in the log. If this option is present only gaps greater than 5 minutes will be logged as an error.";

            this.lblROUNDTIME.Text = "This option is only applicable to EPG data that is collected where the broadcaster has timed programs to allow for ad breaks.";
            this.lblROUNDTIME.Text = this.lblROUNDTIME.Text + "If this option is present program times will be adjusted so that they start on 5 minute ";
            this.lblROUNDTIME.Text = this.lblROUNDTIME.Text + "boundaries. Any time less than 3 minutes after a five minute boundary will be rounded down, anything else rounded up.";

            this.lblUSECHANNELID.Text = "This option is only applicable to OpenTV collections. With some locations it is possible for ";
            this.lblUSECHANNELID.Text = this.lblUSECHANNELID.Text + "the program to use the channel mappings provided by the broadcaster to set the channel ID in ";
            this.lblUSECHANNELID.Text = this.lblUSECHANNELID.Text + "the XMLTV file while with other locations the broadcast channel data can cause the program to set the wrong channel ID.";
            this.lblUSECHANNELID.Text = this.lblUSECHANNELID.Text + "This option should only be present in OpenTV collections made in New Zealand with the present version of the program.";

            this.lblUSEIMAGE.Text = "This option is applicable for both DVBT & DVBS. It will extract the channel logo's and store them in a folder ";
            this.lblUSEIMAGE.Text = this.lblUSEIMAGE.Text + "called [IMAGES] in the installation directory. The XMLTV file will then reference these files.";
            this.lblUSEIMAGE.Text = this.lblUSEIMAGE.Text + "";
            this.lblUSEIMAGE.Text = this.lblUSEIMAGE.Text + "";
            #endregion

            cmdScanTuners_Click(null, null);

        }

        private void ConfigurationForm_Closing(object sender, FormClosingEventArgs e)
        {
            if (formChanged)
            {
                DialogResult result;

                result = MessageBox.Show("Do you want to save the changes?", "Save File?",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        SaveFile();
                        formChanged = false;
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void UpdateForm()
        {
            // Output File
            this.txtOutputFile.Text = ConfigSettings.OutputFileName;

            // Transmission Type
            if (ConfigSettings.TransmissionType == "DVBT")
                this.rbDVBT.Checked = true;
            else
                this.rbSatellite.Checked = true;

            // Country
            this.cboCountry.Enabled = true;
            if (ConfigSettings.TransmissionType == "DVBT")
                this.cboCountry.SelectedIndex = this.cboCountry.FindString(ConfigSettings.Country);
            else
                this.cboCountry.SelectedIndex = this.cboCountry.FindString(ConfigSettings.Satellite);

            // Area
            this.cboArea.Enabled = this.rbDVBT.Checked;
            this.cboArea.SelectedIndex = this.cboArea.FindString(ConfigSettings.Area);

            // Frequencies
            this.rtbFrequencies.Text = "Frequencies Configured:\n";
            this.cboFrequencies.Items.Clear();
            for (int cnt = 0; cnt < TuningFrequency.FrequencyCollection.Count; cnt++)
            {
                this.rtbFrequencies.Text = this.rtbFrequencies.Text + TuningFrequency.FrequencyCollection[cnt].Frequency.ToString() + " KHz\n";
                this.cboFrequencies.Items.Add(TuningFrequency.FrequencyCollection[cnt].Frequency.ToString());
            }
            if (ConfigSettings.ScanningFrequency != null)
            {
                this.cboFrequencies.SelectedIndex = this.cboFrequencies.FindString(ConfigSettings.ScanningFrequency.Frequency.ToString());
                // Collection Type
                this.cboCollectionType.SelectedItem = ConfigSettings.ScanningFrequency.CollectionType.ToString();
            }

            // Timeouts
            this.txtDataCollectionTimeout.Text = ConfigSettings.DataCollectionTimeout.ToString();
            this.txtSignalLockTimeout.Text = ConfigSettings.SignalLockTimeout.ToString();
            this.txtScanRetries.Text = ConfigSettings.ScanRetries.ToString();
            
            // LNB Settings
            if (ConfigSettings.TransmissionType == "Satellite")
            {
                this.txtLNBLow.Text = ConfigSettings.LnbLow;
                this.txtLNBHigh.Text = ConfigSettings.LnbHigh;
                this.txtLNBSwitch.Text = ConfigSettings.LnbSwitch;
                this.txtAzimuth.Text = ConfigSettings.Azimuth.ToString();
            }

            // Options
            if (ConfigSettings.OptionsString != null)
            {
                this.txtOptions.Text = ConfigSettings.OptionsString;
                string[] optionsSplit = ConfigSettings.OptionsString.Split(',');
                for (int x = 0; x < optionsSplit.Length; x++)
                {
                    string tmp = optionsSplit[x];

                    switch (tmp)
                    {
                        case "NOSIGLOCK":
                            this.chkNOSIGLOCK.Checked = true;
                            break;
                        case "ACCEPTBREAKS":
                            this.chkACCEPTBREAKS.Checked = true;
                            break;
                        case "ROUNDTIME":
                            this.chkROUNDTIME.Checked = true;
                            break;
                        case "USECHANNELID":
                            this.chkUSECHANNELID.Checked = true;
                            break;
                        case "USEIMAGE":
                            this.chkUSEIMAGE.Checked = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (!loadingForm)
                formChanged = true;

            // Caption
            if (!createNew)
                this.Text = "EPG Collector Configuration - " + ConfigSettings.IniFileName.Substring(ConfigSettings.IniFileName.LastIndexOf("\\") + 1);

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Filter = "XML Files (*.xml)|*.xml";
            browseFile.Title = "Browse XML file";

            if (browseFile.ShowDialog() == DialogResult.Cancel)
                return;

            try
            {
                txtOutputFile.Text = browseFile.FileName;
            }
            catch (Exception)
            {
                MessageBox.Show("Error opening file", "File Error",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void rbDVBT_CheckedChanged(object sender, EventArgs e)
        {
            // Country
            //this.cboArea.Enabled = this.rbDVBT.Checked;
            //if (this.rbDVBT.Checked)
            //    this.lblCountry.Text = "Country :";
            //else
            //    this.lblCountry.Text = "Satellite :";

            //if (rbDVBT.Checked & !loadingForm)
            //{
            //    if (ConfigSettings.TransmissionType != "DVBT")
            //    {
            //        ConfigSettings.TransmissionType = "DVBT";
            //        ConfigSettings.GetCountries();
            //        cboCountry.DataSource = ConfigSettings.Countries;
            //        ConfigSettings.Country = ConfigSettings.Countries[0].ToString();
            //        cboCountry.SelectedIndex = this.cboCountry.FindString(ConfigSettings.Country);
            //    }
            //    formChanged = true;
            //}
        }

        private void cmdScan_Click(object sender, EventArgs e)
        {
            lstTVStationsFound.Items.Clear();
            TVStation.StationCollection.Clear();
            
            lblScanning.Text = "Scanning : ";
            //allStations = new Collection<TVStation>();


            this.cmdStopScan.Enabled = true;
            this.cmdScan.Enabled = false;
            this.cmdSelectAll.Enabled = false;
            this.cmdSelectNone.Enabled = false;

            try
            {
                this.backgroundWorkerScanStations.RunWorkerAsync();
            }
            catch (Exception x)
            {
                Console.Write("");
            }
            pgbScanning.Visible = true;
            pgbScanning.Minimum = 0;
            pgbScanning.Maximum = 10000;
            pgbScanning.Step = 1;

            tmrProgress.Enabled = true;
        }

        private void backgroundWorkerScanStations_DoScan(object sender, DoWorkEventArgs e)
        {
            //// The sender is the BackgroundWorker object we need it to
            //// report progress and check for cancellation.
            //BackgroundWorker bwAsync = sender as BackgroundWorker;

            try
            {
                if (ConfigSettings.ScanningFrequency.Frequency < 1)
                {
                    Logger.Instance.Write("Incorrect or Missing ScanningFrequency");
                    MessageBox.Show("Incorrect or Missing ScanningFrequency", "Warning...");
                }
                else
                {
                    BDAGraph.LoadTuners();
                    if (!Tuner.TunerPresent)
                    {
                        Logger.Instance.Write("No tuners detected");
                        MessageBox.Show("No Tuners detected!", "Warning...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (!getStations(ConfigSettings.ScanningFrequency))
                        {
                            Logger.Instance.Write("No Stations detected");
                            MessageBox.Show("No Stations detected - check signal & frequencies", "Warning...");
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Console.Write("");
            }
        }

        private void chkDefaultTimeouts_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDefaultTimeouts.Checked)
            {
                this.txtSignalLockTimeout.Text = "10";
                this.txtDataCollectionTimeout.Text = "600";
                // Disable text boxes
                this.txtSignalLockTimeout.Enabled = false;
                this.txtDataCollectionTimeout.Enabled = false;
                this.txtScanRetries.Enabled = false;
            }
            else
            {
                // Enable text boxes
                this.txtSignalLockTimeout.Enabled = true;
                this.txtDataCollectionTimeout.Enabled = true;
                this.txtScanRetries.Enabled = true;
            }
        }

        private void lstTVStationsFound_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                string item = lstTVStationsFound.Items[e.Index].ToString();

                //if (form2.listBox1.Items.IndexOf(item) == -1)
                //    form2.listBox1.Items.Add(item);
            }
        }

        private void cmdSaveChannels_Click(object sender, EventArgs e)
        {
            includedStations = new Collection<TVStation>();

            foreach (object objchecked in lstTVStationsFound.CheckedItems)
            {
                //MessageBox.Show(objchecked.ToString());
                TVStation tmp = TVStation.StationCollection[lstTVStationsFound.Items.IndexOf(objchecked.ToString())];
                includedStations.Add(tmp);
            }

            //this.tabControl1.SelectTab(0);
            ConfigSettings.SaveFile(includedStations, TVStation.StationCollection);
            formChanged = false;
        }

        private void cmdSaveGeneral_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void cmdScanTuners_Click(object sender, EventArgs e)
        {
            cboTuners.Items.Clear();

            BDAGraph.LoadTuners();
            if (!Tuner.TunerPresent)
            {
                Logger.Instance.Write("No tuners detected");
                MessageBox.Show("No Tuners detected!", "Warning...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                ObservableCollection<Tuner> tuners = Tuner.TunerCollection;

                foreach (Tuner item in tuners)
                {
                    this.cboTuners.Items.Add(item.Name);
                }

                if (cboTuners.Items.Count > 0)
                    cboTuners.SelectedIndex = 0;
            }
        }

        private void cboArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                ConfigSettings.Area = cboArea.Text;
        }

        private void cboCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
            {
                ConfigSettings.Country = cboCountry.Text;
                ConfigSettings.GetAreas(ConfigSettings.Country);
                this.cboArea.DataSource = ConfigSettings.Areas;
                this.cboArea.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.cboArea.AutoCompleteSource = AutoCompleteSource.ListItems;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.cboTuners.Text == "")
                tabControl1.SelectedTab = tabControl1.TabPages[0];
        }

        private void cmdLocationApply_Click(object sender, EventArgs e)
        {
            if (this.rbDVBT.Checked)
                ConfigSettings.TuningFile = cboCountry.Text + "." + cboArea.Text + ".xml";
            else
                ConfigSettings.TuningFile = cboCountry.Text + ".xml";

            ConfigSettings.SaveFile(includedStations, allStations);
            TuningFrequency.FrequencyCollection.Clear();
            ConfigSettings.ReadFile();
            UpdateForm();
        }

        private void rbSatellite_CheckedChanged(object sender, EventArgs e)
        {
            this.gbSatellite.Enabled = this.rbSatellite.Checked;
            this.cboArea.Enabled = this.rbDVBT.Checked;

            if (this.rbDVBT.Checked)
                this.lblCountry.Text = "Country :";
            else
                this.lblCountry.Text = "Satellite :";

            if (rbDVBT.Checked & !loadingForm)
            {
                if (ConfigSettings.TransmissionType != "DVBT")
                {
                    ConfigSettings.TransmissionType = "DVBT";
                    ConfigSettings.GetCountries();
                    cboCountry.DataSource = ConfigSettings.Countries;
                    ConfigSettings.Country = ConfigSettings.Countries[0].ToString();
                    cboCountry.SelectedIndex = this.cboCountry.FindString(ConfigSettings.Country);
                }
                formChanged = true;
            }


            if (rbSatellite.Checked & !loadingForm)
            {
                if (ConfigSettings.TransmissionType != "Satellite")
                {
                    ConfigSettings.TransmissionType = "Satellite";
                    ConfigSettings.GetCountries();
                    cboCountry.DataSource = ConfigSettings.Countries;
                    ConfigSettings.Country = ConfigSettings.Countries[0].ToString();
                    cboCountry.SelectedIndex = this.cboCountry.FindString(ConfigSettings.Country);
                }
            }

            if (createNew)
            {
                TuningFrequency.FrequencyCollection.Clear();
                if (this.rbDVBT.Checked)
                {
                    ConfigSettings.TuningFile = cboCountry.Text + "." + cboArea.Text + ".xml";
                    ConfigSettings.CollectionType = CollectionType.MHEG5;
                    this.cboCollectionType.SelectedIndex = Convert.ToInt16(CollectionType.MHEG5);
                    ConfigSettings.TransmissionType = "DVBT";
                }
                else
                {
                    this.chkDefaultLNB.Checked = true;
                    ConfigSettings.ResetDish(txtLNBLow.Text + "," + txtLNBHigh.Text + "," + txtLNBSwitch.Text);
                    ConfigSettings.TuningFile = cboCountry.Text + ".xml";
                    ConfigSettings.CollectionType = CollectionType.EIT;
                    this.cboCollectionType.SelectedIndex = Convert.ToInt16(CollectionType.EIT);
                    ConfigSettings.TransmissionType = "Satellite";
                    ConfigSettings.Azimuth = 1600;
                }
                ConfigSettings.ResetFrequencies();
                ConfigSettings.ScanningFrequency = TuningFrequency.FrequencyCollection[0];
                //ConfigSettings.CollectionType = CollectionType)cboCollectionType.SelectedItem;
                UpdateForm();

                //cmdLocationApply_Click(null, null);
            }
        }

        private void chkDefaultLNB_CheckedChanged(object sender, EventArgs e)
        {
            // Set default values
            if (chkDefaultLNB.Checked)
            {
                this.txtLNBLow.Text = "9750000";
                this.txtLNBHigh.Text = "10750000";
                this.txtLNBSwitch.Text = "11700000";
                this.txtAzimuth.Text = "1600";
                ConfigSettings.LnbLow = this.txtLNBLow.Text;
                ConfigSettings.LnbHigh = this.txtLNBHigh.Text;
                ConfigSettings.LnbSwitch = this.txtLNBSwitch.Text;
                ConfigSettings.Azimuth = Convert.ToInt32(this.txtAzimuth.Text);
            }
        }

        private void cmdOpenINIFile_Click(object sender, EventArgs e)
        {
            OpenFile(false);
        }

        private void cmdSelectAll_Click(object sender, EventArgs e)
        {
            for (int cnt = 0; cnt < lstTVStationsFound.Items.Count; cnt++)
                lstTVStationsFound.SetItemChecked(cnt, true);
        }

        private void cmdSelectNone_Click(object sender, EventArgs e)
        {
            for (int cnt = 0; cnt < lstTVStationsFound.Items.Count; cnt++)
                lstTVStationsFound.SetItemChecked(cnt, false);
        }

        private void cboTuners_SelectedIndexChanged(object sender, EventArgs e)
        {
            formChanged = true;
        }

        private void txtOutputFile_TextChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;
        }

        private void txtSignalLockTimeout_TextChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;
        }

        private void txtScanRetries_TextChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;
        }

        private void txtDataCollectionTimeout_TextChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;
        }

        private void txtLNBLow_TextChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;

            this.chkDefaultLNB.Checked = false;
        }

        private void txtLNBSwitch_TextChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;

            this.chkDefaultLNB.Checked = false;
        }

        private void txtLNBHigh_TextChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;

            this.chkDefaultLNB.Checked = false;
        }

        private void txtOptions_TextChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;
        }

        private void lstTVStationsFound_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;
        }

        #endregion

        #region Functions ...
        private static bool getStations(TuningFrequency frequency)
        {
            bool tuned = tuneFrequency(frequency);
            if (!tuned)
                return (false);

            FrequencyScanner frequencyScanner = new FrequencyScanner(graph);
            Collection<TVStation> stations = frequencyScanner.FindTVStations(frequency);

            if (stations != null)
            {
                Logger.Instance.Write("Found " + stations.Count + " stations on frequency " + frequency);
                int addedCount = 0;

                foreach (TVStation tvStation in stations)
                {
                    TVStation excludedStation = TVStation.FindExcludedStation(tvStation.OriginalNetworkID, tvStation.TransportStreamID, tvStation.ServiceID);
                    if (excludedStation == null)
                    {
                        if (tvStation.NextFollowingAvailable && tvStation.ScheduleAvailable)
                        {
                            bool added = TVStation.AddStation(tvStation);
                            if (added)
                            {
                                //allStations.Add(tvStation);
                                addedCount++;
                                Logger.Instance.Write("Included station: " + tvStation.FixedLengthName + " (" + tvStation.FullID + " Service type " + tvStation.ServiceType + ")");
                            }
                        }
                        else
                            Logger.Instance.Write("Excluded station: " + tvStation.FixedLengthName + " (" + tvStation.FullID + " Service type " + tvStation.ServiceType + ") No EPG data");
                    }
                    else
                        Logger.Instance.Write("Excluded station: " + tvStation.FixedLengthName + " (" + tvStation.FullID + " Service type " + tvStation.ServiceType + ")");
                }

                Logger.Instance.Write("Added " + addedCount + " stations for frequency " + frequency);
            }
            frequencyScanner = null;
            return (true);
        }

        private static bool tuneFrequency(TuningFrequency frequency)
        {
            Logger.Instance.Write("Tuning to frequency " + frequency.Frequency + " on " + frequency.TunerType);

            TuningSpec tuningSpec;
            TunerNodeType tunerNodeType;
            int frequencyRetries = 0;

            if (frequency.TunerType == TunerType.Satellite)
            {
                tuningSpec = new TuningSpec(Satellite.CurrentSatellite, (SatelliteFrequency)frequency);
                tunerNodeType = TunerNodeType.Satellite;
            }
            else if (frequency.TunerType == TunerType.Terrestrial)
            {
                tuningSpec = new TuningSpec((TerrestrialFrequency)frequency);
                tunerNodeType = TunerNodeType.Terrestrial;
            }
            else
            {
                tuningSpec = new TuningSpec((CableFrequency)frequency);
                tunerNodeType = TunerNodeType.Cable;
            }
            

            bool finished = false;

            while (!finished)
            {
                graph = BDAGraph.FindTuner(RunParameters.SelectedTuner, tunerNodeType, tuningSpec);
                if (graph == null)
                {
                    Logger.Instance.Write("Tuner not available");
                    return (false);
                }

                TimeSpan timeout = new TimeSpan();
                bool done = false;
                bool locked = false;
                

                if (RunParameters.Options.Contains("NOSIGLOCK"))
                {
                    Logger.Instance.Write("Option NOSIGLOCK: No lock check, wait 5 seconds instead");
                    locked = true;
                    Thread.Sleep(5000);
                }
                else
                {
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
                                Logger.Instance.Write("Signal not locked and signal quality not > 0");
                                Thread.Sleep(1000);
                                timeout = timeout.Add(new TimeSpan(0, 0, 1));
                                done = (timeout.TotalSeconds == RunParameters.LockTimeout.TotalSeconds);
                            }
                        }
                        else
                            done = true;
                    }
                }

                if (!locked)
                {
                    Logger.Instance.Write("Failed to acquire signal");
                    graph.Dispose();

                    if (frequencyRetries == 1)
                        return (false);
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

        private static bool getFrequencies()
        {
            return (true);
        }

        #endregion
        
        #region File Operations ...
        private void OpenFile(bool newFile)
        {
            loadingForm = true;
            //OpenFileDialog iniFile;
            if (!newFile)
            {
                try
                {
                    OpenFileDialog iniFile = new OpenFileDialog();
                    iniFile.Filter = "INI Files (*.ini)|*.ini";
                    iniFile.Title = "Open INI file";

                    if (iniFile.ShowDialog() == DialogResult.Cancel)
                        return;

                    ConfigSettings = new INIFile(iniFile.FileName);
                    this.tabControl1.Enabled = true;
                }
                catch (Exception x)
                {
                    MessageBox.Show("Error opening file - " + x.Message, "File Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                try
                {
                    ConfigSettings = new INIFile(Application.StartupPath + "\\default.ini");
                    this.tabControl1.Enabled = true;
                }
                catch (Exception x)
                {
                    MessageBox.Show("Error opening default INI file - " + x.Message, "File Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            if (ConfigSettings.TransmissionType == "DVBT")
                this.cboArea.DataSource = ConfigSettings.Areas;

            this.cboCountry.DataSource = ConfigSettings.Countries;
            if (!newFile)
                this.Text = "EPG Collector Configuration - " + ConfigSettings.IniFileName;
            else
                this.Text = "EPG Collector Configuration - New...";

            UpdateForm();
            loadingForm = false;
        }
        
        private void SaveFile()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "INI Files (*.ini)|*.ini";
            saveFile.Title = "Save INI file";

            if (saveFile.ShowDialog() == DialogResult.Cancel)
                return;

            if (createNew)
                CopyFile(saveFile.FileName);

            try
            {
                ConfigSettings.IniFileName = saveFile.FileName;
            }
            catch (Exception)
            {
                MessageBox.Show("Error saving file", "File Error",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // Update settings then write them all out...
            ConfigSettings.OutputFileName = this.txtOutputFile.Text;

            if (this.rbDVBT.Checked)
                ConfigSettings.TuningFile = cboCountry.Text + "." + cboArea.Text + ".xml";
            else
                ConfigSettings.TuningFile = cboCountry.Text + ".xml";

            if (cboTuners.SelectedIndex > 0)
                ConfigSettings.TunerIndex = cboTuners.SelectedIndex + 1;
            else
                ConfigSettings.TunerIndex = 1; //default value

            ConfigSettings.SignalLockTimeout = Convert.ToInt16(txtSignalLockTimeout.Text);
            ConfigSettings.DataCollectionTimeout = Convert.ToInt16(txtDataCollectionTimeout.Text);
            if (txtScanRetries.Text != null)
            {
                ConfigSettings.ScanRetries = Convert.ToInt16(txtScanRetries.Text);
            }

            if (this.txtOptions.Text.Length > 0)
                ConfigSettings.OptionsString = this.txtOptions.Text;

            ConfigSettings.OptionsString = "";
            if (this.chkACCEPTBREAKS.Checked)
                ConfigSettings.OptionsString = "ACCEPTBREAKS,";
            if (this.chkNOSIGLOCK.Checked)
                ConfigSettings.OptionsString = ConfigSettings.OptionsString + "NOSIGLOCK,";
            if (this.chkROUNDTIME.Checked)
                ConfigSettings.OptionsString = ConfigSettings.OptionsString + "ROUNDTIME,";
            if (this.chkUSECHANNELID.Checked)
                ConfigSettings.OptionsString = ConfigSettings.OptionsString + "USECHANNELID,";
            if (this.chkUSEIMAGE.Checked)
                ConfigSettings.OptionsString = ConfigSettings.OptionsString + "USEIMAGE,";
            if (ConfigSettings.OptionsString.Length > 0)
                ConfigSettings.OptionsString = ConfigSettings.OptionsString.Substring(0, ConfigSettings.OptionsString.Length - 1);


            if (this.rbSatellite.Checked)
            {
                ConfigSettings.LnbLow = txtLNBLow.Text;
                ConfigSettings.LnbHigh = txtLNBHigh.Text;
                ConfigSettings.LnbSwitch = txtLNBSwitch.Text;
                ConfigSettings.Azimuth = Convert.ToInt32(txtAzimuth.Text);
            }

            
            //ConfigSettings.ScanningFrequency = "";
            includedStations = new Collection<TVStation>();
            foreach (object objchecked in lstTVStationsFound.CheckedItems)
            {
                //MessageBox.Show(objchecked.ToString());
                TVStation tmp = TVStation.StationCollection[lstTVStationsFound.Items.IndexOf(objchecked.ToString())];
                includedStations.Add(tmp);
            }

            try
            {
                if (ConfigSettings.SaveFile(includedStations, TVStation.StationCollection))
                    MessageBox.Show("Configuration File Saved as \n" + ConfigSettings.IniFileName, "Saved");
                else
                    MessageBox.Show("Unable to save configuration file...\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                TuningFrequency.FrequencyCollection.Clear();
                ConfigSettings.ReadFile();
                UpdateForm();
            }
            catch (Exception x)
            {
                MessageBox.Show("Unable to save configuration file...\n" + x.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            formChanged = false;
        }

        private void CopyFile(string newFilePath)
        {
            string path = Application.StartupPath + "\\default.ini";

            try 
            {
                // Ensure that the target does not exist.
                File.Delete(newFilePath);

                // Copy the file.
                File.Copy(path, newFilePath);
                Console.WriteLine("{0} copied to {1}", path, newFilePath);
            } 
            catch 
            {
                Console.WriteLine("Double copy is not allowed, which was not expected.");
            }
            createNew = false;
        }

        #endregion

        private void cmdNewOutputFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.txtOutputFile.Text.Length > 0)
                {
                    File.Create(this.txtOutputFile.Text).Dispose();
                    MessageBox.Show("File created.", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please enter a Filepath & Filename\n eg. c:\\tvguide.xml", "Enter valid name...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception x)
            {
                MessageBox.Show("Unable to create file -> " + x.Message,"Error",MessageBoxButtons.RetryCancel,MessageBoxIcon.Error);
            }
        }

        private void cmdClearScan_Click(object sender, EventArgs e)
        {
            lstTVStationsFound.Items.Clear();
            TVStation.StationCollection.Clear();
        }

        private void cboFrequencies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
            {
                formChanged = true;
                ConfigSettings.UpdateScanningFrequency(cboFrequencies.SelectedItem.ToString());
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            createNew = true;
            OpenFile(true);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            OpenFile(false);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About newAbout = new About();
            newAbout.Show();
        }

        private void readMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = "notepad.exe";
            p.StartInfo.Arguments = Application.StartupPath + "\\Readme.txt";
            p.Start();
        }

        private void chkNOSIGLOCK_CheckedChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;
        }

        private void chkACCEPTBREAKS_CheckedChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;
        }

        private void chkROUNDTIME_CheckedChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;
        }

        private void chkUSECHANNELID_CheckedChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;
        }

        private void cboCollectionType_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!loadingForm)
            {
                ConfigSettings.CollectionType = (CollectionType)cboCollectionType.SelectedItem;
                formChanged = true;
            }
        }

        private void backgroundWorkerScanStations_RunWorkerCompleted(object sender,RunWorkerCompletedEventArgs e)
        {            
            if (e.Error != null)
                throw new InvalidOperationException("Graph background worker failed - see inner exception", e.Error);

            try
            {
                backgroundWorkerScanStations.Dispose();
                graph.Dispose();
            }
            catch (Exception x)
            {
                Logger.Instance.Write("Warning: " + x.Message);
            }

            try
            {
                this.cmdStopScan.Enabled = false;
                this.cmdScan.Enabled = true;
                this.cmdSelectAll.Enabled = true;
                this.cmdSelectNone.Enabled = true;

                this.tmrProgress.Enabled = false;
                this.pgbScanning.Value = this.pgbScanning.Maximum;

                if ((TVStation.StationCollection != null) & (TVStation.StationCollection.Count > 0))
                {
                    foreach (TVStation station in TVStation.StationCollection)
                    {
                        this.lstTVStationsFound.Items.Add(station.Name + " (" + station.ServiceID + ")");
                    }
                }
                else
                {
                    Logger.Instance.Write("No Stations detected");
                    MessageBox.Show("No Stations detected - check signal & frequencies", "Warning...");
                }
                //this.dataGridView1.DataSource = TVStation.StationCollection;
                
            }
            catch (Exception x)
            {
                Logger.Instance.Write("Error scanning services - " + x.Message);
                MessageBox.Show("Error scanning services - " + x.Message, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tmrProgress_Tick(object sender, EventArgs e)
        {
            this.tmrProgress.Enabled = false;
            for (int i = pgbScanning.Minimum; i <= pgbScanning.Maximum; i++)
            {
                pgbScanning.PerformStep();
            }
            pgbScanning.Value = 0;
            this.tmrProgress.Enabled = true;
        }

        private void cmdStopScan_Click(object sender, EventArgs e)
        {
            this.tmrProgress.Enabled = false;
            this.pgbScanning.Value = 0;

            this.backgroundWorkerScanStations.CancelAsync();
            //if (bwAsync.CancellationPending)
            //{
            //    // Pause for a bit to demonstrate that there is time between

            //    // "Cancelling..." and "Cancel ed".

            //    Thread.Sleep(1200);

            //    // Set the e.Cancel flag so that the WorkerCompleted event

            //    // knows that the process was cancelled.

                //e.Cancel = true;
                //return;
            //}

        }

        private void txtAzimuth_TextChanged(object sender, EventArgs e)
        {
            if (!loadingForm)
                formChanged = true;
        }


    }
}
