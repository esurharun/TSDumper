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
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Globalization;

using DomainObjects;
using DirectShow;
using DVBServices;

namespace EPGCentre
{
    public partial class CollectorParametersControl : UserControl, IUpdateControl
    {
        /// <summary>
        /// Get the general window heading for the data.
        /// </summary>
        public string Heading { get { return ("EPG Centre - Collection Parameters - "); } }        
        /// <summary>
        /// Get the default directory.
        /// </summary>
        public string DefaultDirectory { get { return (RunParameters.DataDirectory); } }
        /// <summary>
        /// Get the default output file name.
        /// </summary>
        public string DefaultFileName { get { return ("EPG Collector"); } }
        /// <summary>
        /// Get the save file filter.
        /// </summary>
        public string SaveFileFilter { get { return ("INI Files (*.ini)|*.ini"); } }
        /// <summary>
        /// Get the save file title.
        /// </summary>
        public string SaveFileTitle { get { return ("Save EPG Collector Parameter File"); } }
        /// <summary>
        /// Get the save file suffix.
        /// </summary>
        public string SaveFileSuffix { get { return ("ini"); } }

        /// <summary>
        /// Return true if file is new; false otherwise.
        /// </summary>
        public bool NewFile { get { return (newFile); } }

        /// <summary>
        /// Return the state of the data set.
        /// </summary>
        public DataState DataState { get { return (hasDataChanged()); } }

        private delegate DialogResult ShowMessage(string message, MessageBoxButtons buttons, MessageBoxIcon icon);

        private string currentFileName;
        private RunParameters originalData;
        private bool newFile;

        private const int timeoutLock = 10;
        private const int timeoutCollection = 300;
        private const int timeoutRetries = 5;

        private BackgroundWorker workerScanStations;
        private AutoResetEvent resetEvent = new AutoResetEvent(false);
        private Collection<TuningFrequency> scanningFrequencies;        

        private BindingList<TVStation> bindingList;

        private string sortedColumnName;
        private string sortedKeyName;
        private bool sortedAscending;

        private bool useSignalPresent;
        private bool switchAfterPlay;
        private bool repeatDiseqc;

        private bool satelliteUsed = false;
        private bool terrestrialUsed = false;
        private bool cableUsed = false;
        private bool atscUsed = false;
        private bool clearQamUsed = false;
        private bool isdbsUsed = false;
        private bool isdbtUsed = false;
        
        public CollectorParametersControl()
        {
            InitializeComponent();

            Satellite.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "dvbs")) + Path.DirectorySeparatorChar);
            TerrestrialProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "dvbt")) + Path.DirectorySeparatorChar);
            CableProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "dvbc")) + Path.DirectorySeparatorChar);
            AtscProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "atsc")) + Path.DirectorySeparatorChar);
            ClearQamProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "clear QAM")) + Path.DirectorySeparatorChar);
            ISDBSatelliteProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "isdbs")) + Path.DirectorySeparatorChar);
            ISDBTerrestrialProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "isdbt")) + Path.DirectorySeparatorChar);
        }

        public void Process()
        {
            RunParameters.Instance = new RunParameters(ParameterSet.Collector);
            TVStation.StationCollection.Clear();
            TuningFrequency.FrequencyCollection.Clear();
            TimeOffsetChannel.Channels.Clear();
            ChannelFilterEntry.ChannelFilters = null;
            RepeatExclusion.Exclusions = null;
            RepeatExclusion.PhrasesToIgnore = null;

            currentFileName = null;
            newFile = true;
            start();
        }

        public void Process(string fileName, bool newFile)
        {
            currentFileName = fileName;
            this.newFile = newFile;

            RunParameters.Instance = new RunParameters(ParameterSet.Collector);
            TVStation.StationCollection.Clear();
            TuningFrequency.FrequencyCollection.Clear();
            TimeOffsetChannel.Channels.Clear();
            ChannelFilterEntry.ChannelFilters = null;

            Cursor.Current = Cursors.WaitCursor;
            int reply = RunParameters.Instance.Process(fileName);
            Cursor.Current = Cursors.Arrow;

            if (reply != 0)
            {
                MessageBox.Show("The parameter file could not be completely processed due to a format error." + Environment.NewLine + Environment.NewLine +
                    "Some parameters may have default values.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            originalData = RunParameters.Instance.Clone();
            start();
        }

        private void start()
        {
            tbcParameters.SelectedTab = tbcParameters.TabPages[0];
            
            clbTuners.Items.Clear();
            clbTuners.Items.Add("Any available Tuner");

            bool found = false;

            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK"))
                {
                    clbTuners.Items.Add(tuner);

                    if (RunParameters.Instance.SelectedTuners.Contains(Tuner.TunerCollection.IndexOf(tuner) + 1))
                    {
                        clbTuners.SetItemChecked(clbTuners.Items.Count - 1, true);
                        found = true;
                    }
                }
            }

            if (!found)
                clbTuners.SetItemChecked(0, true);

            initializeSatelliteTab();
            initializeTerrestrialTab();
            initializeCableTab();
            initializeAtscTab();
            initializeClearQamTab();
            initializeISDBSTab();
            initializeISDBTTab();

            lvSelectedFrequencies.Items.Clear();
            lbScanningFrequencies.Items.Clear();

            foreach (TuningFrequency tuningFrequency in TuningFrequency.FrequencyCollection)
            {
                ListViewItem item = new ListViewItem(tuningFrequency.ToString());
                item.Tag = tuningFrequency;

                item.SubItems.Add(tuningFrequency.Provider.Name);

                switch (tuningFrequency.TunerType)
                {
                    case TunerType.Satellite:                        
                        item.SubItems.Add("Satellite");            
                        break;
                    case TunerType.Terrestrial:
                        item.SubItems.Add("Terrestrial"); 
                        break;
                    case TunerType.Cable:
                        item.SubItems.Add("Cable"); 
                        break;
                    case TunerType.ATSC:
                        item.SubItems.Add("ATSC Terrestrial");
                        break;
                    case TunerType.ATSCCable:
                        item.SubItems.Add("ATSC Cable");
                        break;
                    case TunerType.ClearQAM:
                        item.SubItems.Add("Clear QAM");
                        break;
                    case TunerType.ISDBS:
                        item.SubItems.Add("ISDB Satellite");
                        break;
                    case TunerType.ISDBT:
                        item.SubItems.Add("ISDB Terrestrial");
                        break;
                    default:
                        item.SubItems.Add("Unknown"); 
                        break;
                }

                item.SubItems.Add(tuningFrequency.CollectionType.ToString());

                if (tuningFrequency.TunerType == TunerType.Satellite)
                {
                    SatelliteFrequency satelliteFrequency = tuningFrequency as SatelliteFrequency;
                    item.SubItems.Add(satelliteFrequency.SatelliteDish.LNBLowBandFrequency.ToString());
                    item.SubItems.Add(satelliteFrequency.SatelliteDish.LNBHighBandFrequency.ToString());
                    item.SubItems.Add(satelliteFrequency.SatelliteDish.LNBSwitchFrequency.ToString());
                    if (satelliteFrequency.SatelliteDish.DiseqcSwitch != null)
                        item.SubItems.Add(satelliteFrequency.SatelliteDish.DiseqcSwitch);
                }

                lvSelectedFrequencies.Items.Add(item);
                lbScanningFrequencies.Items.Add(tuningFrequency);
            }

            btDelete.Enabled = (lvSelectedFrequencies.SelectedItems.Count != 0);

            txtOutputFile.Text = RunParameters.Instance.OutputFileName;
            cbAllowBreaks.Checked = RunParameters.Instance.Options.Contains("ACCEPTBREAKS");
            cbRoundTime.Checked = RunParameters.Instance.Options.Contains("ROUNDTIME");
            cbUseBSEPG.Checked = RunParameters.Instance.Options.Contains("USEBSEPG");            
            cbUseChannelID.Checked = RunParameters.Instance.Options.Contains("USECHANNELID");
            cbUseLCN.Checked = RunParameters.Instance.Options.Contains("USELCN");
            cbUseRawCRID.Checked = RunParameters.Instance.Options.Contains("USERAWCRID");
            cbUseNumericCRID.Checked = RunParameters.Instance.Options.Contains("USENUMERICCRID");
            cbUseDescAsCategory.Checked = RunParameters.Instance.Options.Contains("USEDESCASCATEGORY");
            cbUseStationLogos.Checked = RunParameters.Instance.Options.Contains("USEIMAGE");
            cbCreateBladeRunnerFile.Checked = RunParameters.Instance.Options.Contains("CREATEBRCHANNELS");
            cbCreateAreaRegionFile.Checked = RunParameters.Instance.Options.Contains("CREATEARCHANNELS");
            cbCheckForRepeats.Checked = RunParameters.Instance.Options.Contains("CHECKFORREPEATS");
            cbRemoveExtractedData.Checked = RunParameters.Instance.Options.Contains("NOREMOVEDATA");
            cbCreateSameData.Checked = RunParameters.Instance.Options.Contains("DUPLICATESAMECHANNELS");
            
            if (Environment.OSVersion.Version.Major != 5)
            {
                cbWMCImport.Enabled = true;
                cbWMCImport.Checked = RunParameters.Instance.Options.Contains("WMCIMPORT");
                if (cbWMCImport.Enabled)
                {
                    txtImportName.Text = RunParameters.Instance.WMCImportName;
                    txtImportName.Enabled = true;
                    cbAutoMapEPG.Enabled = true;
                    cbAutoMapEPG.Checked = RunParameters.Instance.Options.Contains("AUTOMAPEPG");
                    cbAllSeries.Enabled = true;
                    cbAllSeries.Checked = RunParameters.Instance.Options.Contains("ALLSERIES");              
                }
                else
                {
                    txtImportName.Text = string.Empty;
                    txtImportName.Enabled = false;
                    cbAutoMapEPG.Enabled = false;
                    cbAutoMapEPG.Checked = false;
                    cbAllSeries.Enabled = false;
                    cbAllSeries.Checked = false;  
                }
            }
            else
            {
                cbWMCImport.Enabled = false;
                cbWMCImport.Checked = false;
                txtImportName.Text = string.Empty;
                txtImportName.Enabled = false;
                cbAutoMapEPG.Enabled = false;
                cbAutoMapEPG.Checked = false;
                cbAllSeries.Enabled = false;
                cbAllSeries.Checked = false;  
            }

            txtImportName.KeyPress -= new KeyPressEventHandler(txtImportName_KeyPressAlphaNumeric);
            txtImportName.KeyPress += new KeyPressEventHandler(txtImportName_KeyPressAlphaNumeric);
            
            cbUseDVBViewer.Checked = RunParameters.Instance.Options.Contains("USEDVBVIEWER");
            cbDVBViewerImport.Checked = RunParameters.Instance.Options.Contains("DVBVIEWERIMPORT");
            cbRecordingServiceImport.Checked = RunParameters.Instance.Options.Contains("DVBVIEWERRECSVCIMPORT");
            cbDVBViewerClear.Checked = RunParameters.Instance.Options.Contains("DVBVIEWERCLEAR");

            decimal port = 8089;

            foreach (string optionID in RunParameters.Instance.Options)
            {
                if (optionID.StartsWith("DVBVIEWERRECSVCPORT-"))
                {
                    string[] parts = optionID.Split(new char[] { '-' });
                    port = Decimal.Parse(parts[1].Trim());
                }
            }

            nudPort.Value = port;
            nudPort.Enabled = RunParameters.Instance.Options.Contains("DVBVIEWERRECSVCIMPORT");

            if (TVStation.StationCollection.Count != 0)
            {
                Collection<TVStation> sortedStations = new Collection<TVStation>();

                foreach (TVStation station in TVStation.StationCollection)
                    addInOrder(sortedStations, station, true, "Name");
                    
                bindingList = new BindingList<TVStation>();
                foreach (TVStation station in sortedStations)
                    bindingList.Add(station);
                
                tVStationBindingSource.DataSource = bindingList;
            }
            else
                tVStationBindingSource.DataSource = null;

            sortedColumnName = "nameColumn";
            sortedKeyName = "Name";
            sortedAscending = true;

            populatePlusChannels(lbPlusSourceChannel);
            populatePlusChannels(lbPlusDestinationChannel);
            lvPlusSelectedChannels.Items.Clear();
            
            foreach (TimeOffsetChannel timeOffsetChannel in TimeOffsetChannel.Channels)
            {
                ListViewItem newItem = new ListViewItem(timeOffsetChannel.SourceChannel.Name);
                newItem.Tag = timeOffsetChannel;
                newItem.SubItems.Add(timeOffsetChannel.DestinationChannel.Name);
                newItem.SubItems.Add(timeOffsetChannel.Offset.ToString());
                lvPlusSelectedChannels.Items.Add(newItem);
            }

            btPlusDelete.Enabled = (lvPlusSelectedChannels.SelectedItems.Count != 0);            

            lvExcludedIdentifiers.Items.Clear();

            if (ChannelFilterEntry.ChannelFilters != null)
            {
                foreach (ChannelFilterEntry filterEntry in ChannelFilterEntry.ChannelFilters)
                {
                    if (filterEntry.OriginalNetworkID != -1)
                    {
                        ListViewItem newItem = new ListViewItem(filterEntry.OriginalNetworkID.ToString());
                        newItem.Tag = filterEntry;
                        if (filterEntry.TransportStreamID != -1)
                            newItem.SubItems.Add(filterEntry.TransportStreamID.ToString());
                        else
                            newItem.SubItems.Add("");
                        if (filterEntry.StartServiceID != -1)
                            newItem.SubItems.Add(filterEntry.StartServiceID.ToString());
                        else
                            newItem.SubItems.Add("");
                        if (filterEntry.EndServiceID != -1)
                            newItem.SubItems.Add(filterEntry.EndServiceID.ToString());
                        else
                            newItem.SubItems.Add("");
                        lvExcludedIdentifiers.Items.Add(newItem);
                    }
                }                
            }

            if (RunParameters.Instance.MaxService != -1)
                tbExcludedMaxChannel.Text = RunParameters.Instance.MaxService.ToString();
            else
                tbExcludedMaxChannel.Text = string.Empty;

            lvRepeatPrograms.Items.Clear();

            if (RepeatExclusion.Exclusions != null)
            {
                foreach (RepeatExclusion repeatExclusion in RepeatExclusion.Exclusions)
                {
                    ListViewItem newItem = new ListViewItem(repeatExclusion.Title);
                    newItem.SubItems.Add(repeatExclusion.Description);
                    lvRepeatPrograms.Items.Add(newItem);
                }
            }

            StringBuilder phrasesToIgnore = new StringBuilder();            
            if (RepeatExclusion.PhrasesToIgnore != null)
            {
                foreach (string phrase in RepeatExclusion.PhrasesToIgnore)
                {
                    if (phrasesToIgnore.Length != 0)
                        phrasesToIgnore.Append(',');
                    phrasesToIgnore.Append(phrase);
                }
            }
            tbPhrasesToIgnore.Text = phrasesToIgnore.ToString();

            cbUseContentSubtype.Checked = RunParameters.Instance.Options.Contains("USECONTENTSUBTYPE");
            cbUseSignalPresent.Checked = RunParameters.Instance.Options.Contains("USESIGNALPRESENT");
            cbUseSafeDiseqc.Checked = RunParameters.Instance.Options.Contains("USESAFEDISEQC");
            cbSwitchAfterPlay.Checked = RunParameters.Instance.Options.Contains("SWITCHAFTERPLAY");
            cbRepeatDiseqc.Checked = RunParameters.Instance.Options.Contains("REPEATDISEQC");
            cbUseFreeSatTables.Checked = RunParameters.Instance.Options.Contains("USEFREESATTABLES");
            cbCustomCategoriesOverride.Checked = RunParameters.Instance.Options.Contains("CUSTOMCATEGORYOVERRIDE");
            cbStoreStationInfo.Checked = RunParameters.Instance.Options.Contains("STORESTATIONINFO");
            cbUseStoredStationInfo.Checked = RunParameters.Instance.Options.Contains("USESTOREDSTATIONINFO");
            cbProcessAllStations.Checked = RunParameters.Instance.Options.Contains("PROCESSALLSTATIONS");
            cbFromService.Checked = RunParameters.Instance.Options.Contains("RUNFROMSERVICE"); 
            
            nudSignalLockTimeout.Value = (decimal)RunParameters.Instance.LockTimeout.TotalSeconds;
            nudDataCollectionTimeout.Value = (decimal)RunParameters.Instance.FrequencyTimeout.TotalSeconds;
            nudScanRetries.Value = (decimal)RunParameters.Instance.Repeats;

            cboLocation.DataSource = Country.Load();
            if (RunParameters.Instance.CountryCode != null)
            {
                Country country = Country.FindCountryCode(RunParameters.Instance.CountryCode, (Collection<Country>)cboLocation.DataSource);
                if (country != null)
                {
                    cboLocation.SelectedItem = country;
                    cboLocation.Text = country.Name;

                    if (RunParameters.Instance.ChannelBouquet != -1)
                    {
                        Area area = null;

                        if (RunParameters.Instance.ChannelRegion == -1)
                            area = country.FindArea(RunParameters.Instance.ChannelBouquet);
                        else
                            area = country.FindArea(RunParameters.Instance.ChannelBouquet, RunParameters.Instance.ChannelRegion);

                        if (area != null)
                        {
                            cboLocationArea.SelectedItem = area;
                            cboLocationArea.Text = area.Name;

                            if (RunParameters.Instance.ChannelRegion != -1)
                            {
                                DomainObjects.Region region = area.FindRegion(RunParameters.Instance.ChannelRegion);
                                if (region != null)
                                {
                                    cboLocationRegion.SelectedItem = region;
                                    cboLocationRegion.Text = region.Name;
                                }
                            }
                        }
                    }
                }
                else
                    cboLocation.SelectedItem = cboLocation.Items[0];
            }
            else
                cboLocation.SelectedItem = cboLocation.Items[0];

            cboCharacterSet.DataSource = CharacterSet.CharacterSets;
            if (RunParameters.Instance.CharacterSet != null)
                cboCharacterSet.Text = CharacterSet.FindCharacterSet(RunParameters.Instance.CharacterSet).ToString();
            else
                cboCharacterSet.Text = cboCharacterSet.Items[0].ToString();

            cboInputLanguage.DataSource = LanguageCode.LanguageCodeList;
            if (RunParameters.Instance.InputLanguage != null)
                cboInputLanguage.Text = LanguageCode.FindLanguageCode(RunParameters.Instance.InputLanguage).ToString();
            else
                cboInputLanguage.Text = cboInputLanguage.Items[0].ToString();
            
            if (RunParameters.Instance.EITPid != -1)
                nudEITPid.Value = RunParameters.Instance.EITPid;
            else
                nudEITPid.Value = 0;

            if (RunParameters.Instance.MHW1Pids != null)
            {
                nudMHW1Pid1.Value = RunParameters.Instance.MHW1Pids[0];
                nudMHW1Pid2.Value = RunParameters.Instance.MHW1Pids[1];
            }
            else
            {
                nudMHW1Pid1.Value = 0;
                nudMHW1Pid2.Value = 0;
            }

            if (RunParameters.Instance.MHW2Pids != null)
            {
                nudMHW2Pid1.Value = RunParameters.Instance.MHW2Pids[0];
                nudMHW2Pid2.Value = RunParameters.Instance.MHW2Pids[1];
                nudMHW2Pid3.Value = RunParameters.Instance.MHW2Pids[2];
            }
            else
            {
                nudMHW2Pid1.Value = 0;
                nudMHW2Pid2.Value = 0;
                nudMHW2Pid3.Value = 0;
            }

            cbManualTime.Checked = RunParameters.Instance.TimeZoneSet;
            gpTimeAdjustments.Enabled = RunParameters.Instance.TimeZoneSet;

            if (cbManualTime.Checked)
            {
                nudCurrentOffsetHours.Value = RunParameters.Instance.TimeZone.Hours;
                nudCurrentOffsetMinutes.Value = RunParameters.Instance.TimeZone.Minutes;
                nudNextOffsetHours.Value = RunParameters.Instance.NextTimeZone.Hours;
                nudNextOffsetMinutes.Value = RunParameters.Instance.NextTimeZone.Minutes;
                tbChangeDate.Text = RunParameters.Instance.NextTimeZoneChange.Date.ToString("ddMMyy");
                nudChangeHours.Value = RunParameters.Instance.NextTimeZoneChange.Hour;
                nudChangeMinutes.Value = RunParameters.Instance.NextTimeZoneChange.Minute;
            }
            else
            {
                nudCurrentOffsetHours.Value = 0;
                nudCurrentOffsetMinutes.Value = 0;
                nudNextOffsetHours.Value = 0;
                nudNextOffsetMinutes.Value = 0;
                tbChangeDate.Text = string.Empty;
                nudChangeHours.Value = 0;
                nudChangeMinutes.Value = 0;
            }

            tbDebugIDs.Text = string.Empty;
            foreach (string debugID in RunParameters.Instance.DebugIDs)
            {
                if (tbDebugIDs.Text.Length != 0)
                    tbDebugIDs.Text = tbDebugIDs.Text + "," + debugID;
                else
                    tbDebugIDs.Text = tbDebugIDs.Text + debugID;
            }

            tbTraceIDs.Text = string.Empty;
            foreach (string traceID in RunParameters.Instance.TraceIDs)
            {
                if (tbTraceIDs.Text.Length != 0)
                    tbTraceIDs.Text = tbTraceIDs.Text + "," + traceID;
                else
                    tbTraceIDs.Text = tbTraceIDs.Text + traceID;
            }

            if (RunParameters.Instance.TSFileName != null)
                tbDumpFile.Text = RunParameters.Instance.TSFileName;
            else
                tbDumpFile.Text = string.Empty;
        }

        private void initializeSatelliteTab()
        {
            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK") && tuner.Supports(TunerNodeType.Satellite))
                    satelliteUsed = true;
            }

            if (!satelliteUsed)
            {
                tbcDeliverySystem.TabPages.RemoveByKey("tbpSatellite");
                return;
            }

            cboDVBSCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));
            cboSatellite.DataSource = Satellite.Providers;
            cboSatellite.SelectedItem = cboSatellite.Items[0];

            SatelliteDish satelliteDish = null;

            foreach (TuningFrequency tuningFrequency in TuningFrequency.FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.Satellite)
                {
                    cboSatellite.Text = tuningFrequency.Provider.Name;
                    cboDVBSScanningFrequency.Text = tuningFrequency.ToString();
                    if (satelliteDish == null)
                        satelliteDish = ((SatelliteFrequency)tuningFrequency).SatelliteDish;
                    cboDVBSCollectionType.Text = tuningFrequency.CollectionType.ToString();
                }
            }

            if (satelliteDish == null)
                satelliteDish = SatelliteDish.FirstDefault;

            txtLNBLow.Text = satelliteDish.LNBLowBandFrequency.ToString();
            txtLNBHigh.Text = satelliteDish.LNBHighBandFrequency.ToString();
            txtLNBSwitch.Text = satelliteDish.LNBSwitchFrequency.ToString();

            cboDiseqc.DataSource = Enum.GetValues(typeof(DiseqcSettings));
            if (satelliteDish.DiseqcSwitch != null)
                cboDiseqc.Text = satelliteDish.DiseqcSwitch;
        }

        private void initializeTerrestrialTab()
        {
            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK") && tuner.Supports(TunerNodeType.Terrestrial))
                    terrestrialUsed = true;
            }

            if (!terrestrialUsed)
            {
                tbcDeliverySystem.TabPages.RemoveByKey("tbpTerrestrial");
                return;
            }

            cboDVBTCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));
            cboCountry.DataSource = TerrestrialProvider.Countries;
            cboCountry.SelectedItem = cboCountry.Items[0];

            foreach (TuningFrequency tuningFrequency in TuningFrequency.FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.Terrestrial)
                {
                    cboCountry.Text = ((TerrestrialProvider)tuningFrequency.Provider).Country.Name;
                    cboArea.Text = ((TerrestrialProvider)tuningFrequency.Provider).Area.Name;
                    cboDVBTScanningFrequency.Text = tuningFrequency.ToString();
                    cboDVBTCollectionType.Text = tuningFrequency.CollectionType.ToString();
                }
            }
        }

        private void initializeCableTab()
        {
            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK") && tuner.Supports(TunerNodeType.Cable))
                    cableUsed = true;
            }

            if (!cableUsed)
            {
                tbcDeliverySystem.TabPages.RemoveByKey("tbpCable");
                return;
            }

            cboCableCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));
            cboCable.DataSource = CableProvider.Providers;
            cboCable.SelectedItem = cboCable.Items[0];

            foreach (TuningFrequency tuningFrequency in TuningFrequency.FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.Cable)
                {
                    cboCable.Text = tuningFrequency.Provider.Name;
                    cboCableScanningFrequency.Text = tuningFrequency.ToString();
                    cboCableCollectionType.Text = tuningFrequency.CollectionType.ToString();
                }
            }
        }

        private void initializeAtscTab()
        {
            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK") && tuner.Supports(TunerNodeType.ATSC))
                    atscUsed = true;
            }

            if (!atscUsed)
            {
                tbcDeliverySystem.TabPages.RemoveByKey("tbpAtsc");
                return;
            }

            cboAtscCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));
            cboAtscProvider.DataSource = AtscProvider.Providers;
            cboAtscProvider.SelectedItem = cboAtscProvider.Items[0];

            foreach (TuningFrequency tuningFrequency in TuningFrequency.FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.ATSC)
                {
                    cboAtscProvider.Text = tuningFrequency.Provider.Name;
                    cboAtscScanningFrequency.Text = tuningFrequency.ToString();
                    cboAtscCollectionType.Text = tuningFrequency.CollectionType.ToString();
                }
            }
        }

        private void initializeClearQamTab()
        {
            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK") && tuner.Supports(TunerNodeType.Cable))
                    clearQamUsed = true;
            }

            if (!clearQamUsed)
            {
                tbcDeliverySystem.TabPages.RemoveByKey("tbpClearQAM");
                return;
            }

            cboClearQamCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));
            cboClearQamProvider.DataSource = ClearQamProvider.Providers;
            cboClearQamProvider.SelectedItem = cboClearQamProvider.Items[0];

            foreach (TuningFrequency tuningFrequency in TuningFrequency.FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.ClearQAM)
                {
                    cboClearQamProvider.Text = tuningFrequency.Provider.Name;
                    cboClearQamScanningFrequency.Text = tuningFrequency.ToString();
                    cboClearQamCollectionType.Text = tuningFrequency.CollectionType.ToString();
                }
            }
        }

        private void initializeISDBSTab()
        {
            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK") && tuner.Supports(TunerNodeType.ISDBS))
                    isdbsUsed = true;
            }

            if (!isdbsUsed)
            {
                tbcDeliverySystem.TabPages.RemoveByKey("tbpISDBSatellite");
                return;
            }

            cboISDBSCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));
            cboISDBSatellite.DataSource = ISDBSatelliteProvider.Providers;
            cboISDBSatellite.SelectedItem = cboISDBSatellite.Items[0];

            SatelliteDish satelliteDish = null;

            foreach (TuningFrequency tuningFrequency in TuningFrequency.FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.ISDBS)
                {
                    cboISDBSatellite.Text = tuningFrequency.Provider.Name;
                    cboISDBSScanningFrequency.Text = tuningFrequency.ToString();
                    if (satelliteDish == null)
                        satelliteDish = ((ISDBSatelliteFrequency)tuningFrequency).SatelliteDish;
                    cboISDBSCollectionType.Text = tuningFrequency.CollectionType.ToString();
                }
            }

            if (satelliteDish == null)
                satelliteDish = SatelliteDish.FirstDefault;

            txtISDBLNBLow.Text = satelliteDish.LNBLowBandFrequency.ToString();
            txtISDBLNBHigh.Text = satelliteDish.LNBHighBandFrequency.ToString();
            txtISDBLNBSwitch.Text = satelliteDish.LNBSwitchFrequency.ToString();

            cboISDBDiseqc.DataSource = Enum.GetValues(typeof(DiseqcSettings));
            if (satelliteDish.DiseqcSwitch != null)
                cboISDBDiseqc.Text = satelliteDish.DiseqcSwitch;
        }

        private void initializeISDBTTab()
        {
            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK") && tuner.Supports(TunerNodeType.ISDBT))
                    isdbtUsed = true;
            }

            if (!isdbtUsed)
            {
                tbcDeliverySystem.TabPages.RemoveByKey("tbpISDBTerrestrial");
                return;
            }

            cboISDBTCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));
            cboISDBTProvider.DataSource = ISDBTerrestrialProvider.Providers;
            cboISDBTProvider.SelectedItem = cboISDBTProvider.Items[0];

            foreach (TuningFrequency tuningFrequency in TuningFrequency.FrequencyCollection)
            {
                if (tuningFrequency.TunerType == TunerType.ISDBT)
                {
                    cboISDBTProvider.Text = tuningFrequency.Provider.Name;
                    cboISDBTScanningFrequency.Text = tuningFrequency.ToString();
                    cboISDBTCollectionType.Text = tuningFrequency.CollectionType.ToString();
                }
            }
        }

        private void clbTuners_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbTuners.SelectedIndices.Count == 0)
                return;

            if (clbTuners.SelectedIndices[0] == 0)
            {
                for (int index = 1; index < clbTuners.Items.Count; index++)
                    clbTuners.SetItemChecked(index, false);
            }
            else
                clbTuners.SetItemChecked(0, false);
        }

        private void btLNBDefaults_Click(object sender, EventArgs e)
        {
            SatelliteDish defaultSatellite = SatelliteDish.Default;

            txtLNBLow.Text = defaultSatellite.LNBLowBandFrequency.ToString();
            txtLNBHigh.Text = defaultSatellite.LNBHighBandFrequency.ToString();
            txtLNBSwitch.Text = defaultSatellite.LNBSwitchFrequency.ToString();

            cboDiseqc.SelectedIndex = 0;
        }

        private void cboSatellite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSatellite.SelectedItem != null)
                cboDVBSScanningFrequency.DataSource = ((Satellite)cboSatellite.SelectedItem).Frequencies;            
        }

        private void cboDVBSScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboDVBSScanningFrequency.SelectedItem as TuningFrequency;
            cboDVBSCollectionType.Text = tuningFrequency.CollectionType.ToString();
        }

        private void btAddSatellite_Click(object sender, EventArgs e)
        {
            SatelliteFrequency satelliteFrequency = (SatelliteFrequency)(cboDVBSScanningFrequency.SelectedItem as SatelliteFrequency).Clone();
            satelliteFrequency.CollectionType = (CollectionType)cboDVBSCollectionType.SelectedItem;

            satelliteFrequency.SatelliteDish = new SatelliteDish();

            try
            {
                satelliteFrequency.SatelliteDish.LNBLowBandFrequency = Int32.Parse(txtLNBLow.Text.Trim());
                satelliteFrequency.SatelliteDish.LNBHighBandFrequency = Int32.Parse(txtLNBHigh.Text.Trim());
                satelliteFrequency.SatelliteDish.LNBSwitchFrequency = Int32.Parse(txtLNBSwitch.Text.Trim());
            }
            catch (FormatException)
            {
                MessageBox.Show("A dish parameter is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (ArithmeticException)
            {
                MessageBox.Show("A dish parameter is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cboDiseqc.SelectedIndex != 0)
                satelliteFrequency.SatelliteDish.DiseqcSwitch = cboDiseqc.Text;

            ListViewItem newItem = new ListViewItem(satelliteFrequency.ToString());
            newItem.Tag = satelliteFrequency;
            newItem.SubItems.Add(((Satellite)cboSatellite.SelectedItem).Name);
            newItem.SubItems.Add("Satellite");
            newItem.SubItems.Add(satelliteFrequency.CollectionType.ToString());
            newItem.SubItems.Add(satelliteFrequency.SatelliteDish.LNBLowBandFrequency.ToString());
            newItem.SubItems.Add(satelliteFrequency.SatelliteDish.LNBHighBandFrequency.ToString());
            newItem.SubItems.Add(satelliteFrequency.SatelliteDish.LNBSwitchFrequency.ToString());
            if (satelliteFrequency.SatelliteDish.DiseqcSwitch != null)
                newItem.SubItems.Add(satelliteFrequency.SatelliteDish.DiseqcSwitch);

            foreach (ListViewItem oldItem in lvSelectedFrequencies.Items)
            {
                int index = lvSelectedFrequencies.Items.IndexOf(oldItem);

                SatelliteFrequency oldFrequency = oldItem.Tag as SatelliteFrequency;
                if (oldFrequency != null && oldFrequency.ToString() == satelliteFrequency.ToString() &&
                    oldFrequency.Provider == satelliteFrequency.Provider)
                {
                    if (oldFrequency.CollectionType != satelliteFrequency.CollectionType ||
                        oldFrequency.SatelliteDish.LNBLowBandFrequency != satelliteFrequency.SatelliteDish.LNBLowBandFrequency ||
                        oldFrequency.SatelliteDish.LNBHighBandFrequency != satelliteFrequency.SatelliteDish.LNBHighBandFrequency ||
                        oldFrequency.SatelliteDish.LNBSwitchFrequency != satelliteFrequency.SatelliteDish.LNBSwitchFrequency ||
                        oldFrequency.SatelliteDish.DiseqcSwitch != satelliteFrequency.SatelliteDish.DiseqcSwitch)
                    {
                        DialogResult result = MessageBox.Show("The frequency has already been selected." + Environment.NewLine + Environment.NewLine +
                            "Do you want to overwrite it?", "EPG Centre", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        switch (result)
                        {
                            case DialogResult.Cancel:
                                return;
                            case DialogResult.Yes:
                                lvSelectedFrequencies.Items.Remove(oldItem);
                                lbScanningFrequencies.Items.Remove(oldItem.Tag);
                                lvSelectedFrequencies.Items.Insert(index, newItem);
                                lbScanningFrequencies.Items.Insert(index, satelliteFrequency);
                                return;
                            default:
                                lvSelectedFrequencies.Items.Add(newItem);
                                lbScanningFrequencies.Items.Add(satelliteFrequency);
                                return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("The frequency has already been selected with the same parameters.",
                            "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            lvSelectedFrequencies.Items.Add(newItem);
            lbScanningFrequencies.Items.Add(satelliteFrequency);
        }

        private void cboCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCountry.SelectedItem != null)
                cboArea.DataSource = ((Country)cboCountry.SelectedItem).Areas;
        }

        private void cboArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            TerrestrialProvider provider = TerrestrialProvider.FindProvider(cboCountry.Text, cboArea.Text);
            if (provider == null)
                return;

            cboDVBTScanningFrequency.DataSource = provider.Frequencies;
        }

        private void cboDVBTScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboDVBTScanningFrequency.SelectedItem as TuningFrequency;
            cboDVBTCollectionType.Text = tuningFrequency.CollectionType.ToString();            
        }

        private void btAddTerrestrial_Click(object sender, EventArgs e)
        {
            TerrestrialFrequency terrestrialFrequency = (TerrestrialFrequency)(cboDVBTScanningFrequency.SelectedItem as TerrestrialFrequency).Clone();
            terrestrialFrequency.CollectionType = (CollectionType)cboDVBTCollectionType.SelectedItem;

            ListViewItem newItem = new ListViewItem(terrestrialFrequency.ToString());
            newItem.Tag = terrestrialFrequency;
            newItem.SubItems.Add(cboCountry.Text + " " + cboArea.Text);
            newItem.SubItems.Add("Terrestrial");
            newItem.SubItems.Add(terrestrialFrequency.CollectionType.ToString());

            foreach (ListViewItem oldItem in lvSelectedFrequencies.Items)
            {
                int index = lvSelectedFrequencies.Items.IndexOf(oldItem);

                TerrestrialFrequency oldFrequency = oldItem.Tag as TerrestrialFrequency;
                if (oldFrequency != null && oldFrequency.ToString() == terrestrialFrequency.ToString() &&
                    oldFrequency.Provider == terrestrialFrequency.Provider)
                {
                    if (oldFrequency.CollectionType != terrestrialFrequency.CollectionType)
                    {
                        DialogResult result = MessageBox.Show("The frequency has already been selected." + Environment.NewLine + Environment.NewLine +
                            "Do you want to overwrite it?", "EPG Centre", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        switch (result)
                        {
                            case DialogResult.Cancel:
                                return;
                            case DialogResult.Yes:
                                lvSelectedFrequencies.Items.Remove(oldItem);
                                lbScanningFrequencies.Items.Remove(oldItem.Tag);
                                lvSelectedFrequencies.Items.Insert(index, newItem);
                                lbScanningFrequencies.Items.Insert(index, terrestrialFrequency);
                                return;
                            default:
                                lvSelectedFrequencies.Items.Add(newItem);
                                lbScanningFrequencies.Items.Add(terrestrialFrequency);
                                return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("The frequency has already been selected with the same parameters.",
                            "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            lvSelectedFrequencies.Items.Add(newItem);
            lbScanningFrequencies.Items.Add(terrestrialFrequency);
        }

        private void cboCable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCable.SelectedItem != null)
                cboCableScanningFrequency.DataSource = ((CableProvider)cboCable.SelectedItem).Frequencies;
        }

        private void cboCableScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboCableScanningFrequency.SelectedItem as TuningFrequency;
            cboCableCollectionType.Text = tuningFrequency.CollectionType.ToString();
        }

        private void btAddCable_Click(object sender, EventArgs e)
        {
            CableFrequency cableFrequency = (CableFrequency)(cboCableScanningFrequency.SelectedItem as CableFrequency).Clone();
            cableFrequency.CollectionType = (CollectionType)cboCableCollectionType.SelectedItem;

            ListViewItem newItem = new ListViewItem(cableFrequency.ToString());
            newItem.Tag = cableFrequency;
            newItem.SubItems.Add(((CableProvider)cboCable.SelectedItem).Name);
            newItem.SubItems.Add("Cable");
            newItem.SubItems.Add(cableFrequency.CollectionType.ToString());

            foreach (ListViewItem oldItem in lvSelectedFrequencies.Items)
            {
                int index = lvSelectedFrequencies.Items.IndexOf(oldItem);

                CableFrequency oldFrequency = oldItem.Tag as CableFrequency;
                if (oldFrequency != null && oldFrequency.ToString() == cableFrequency.ToString() &&
                    oldFrequency.Provider == cableFrequency.Provider)
                {
                    if (oldFrequency.CollectionType != cableFrequency.CollectionType)
                    {
                        DialogResult result = MessageBox.Show("The frequency has already been selected." + Environment.NewLine + Environment.NewLine +
                            "Do you want to overwrite it?", "EPG Centre", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        switch (result)
                        {
                            case DialogResult.Cancel:
                                return;
                            case DialogResult.Yes:
                                lvSelectedFrequencies.Items.Remove(oldItem);
                                lbScanningFrequencies.Items.Remove(oldItem.Tag);
                                lvSelectedFrequencies.Items.Insert(index, newItem);
                                lbScanningFrequencies.Items.Insert(index, cableFrequency);
                                return;
                            default:
                                lvSelectedFrequencies.Items.Add(newItem);
                                lbScanningFrequencies.Items.Add(cableFrequency);
                                return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("The frequency has already been selected with the same parameters.",
                            "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            lvSelectedFrequencies.Items.Add(newItem);
            lbScanningFrequencies.Items.Add(cableFrequency);
        }

        private void cboAtscProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboAtscProvider.SelectedItem != null)
                cboAtscScanningFrequency.DataSource = ((AtscProvider)cboAtscProvider.SelectedItem).Channels;
        }

        private void cboAtscScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboAtscScanningFrequency.SelectedItem as TuningFrequency;
            cboAtscCollectionType.Text = tuningFrequency.CollectionType.ToString();
        }

        private void btAddAtsc_Click(object sender, EventArgs e)
        {
            AtscFrequency atscFrequency = (AtscFrequency)(cboAtscScanningFrequency.SelectedItem as AtscFrequency).Clone();
            atscFrequency.CollectionType = (CollectionType)cboAtscCollectionType.SelectedItem;

            ListViewItem newItem = new ListViewItem(atscFrequency.ToString());
            newItem.Tag = atscFrequency;
            newItem.SubItems.Add(((AtscProvider)cboAtscProvider.SelectedItem).Name);
            newItem.SubItems.Add("ATSC");
            newItem.SubItems.Add(atscFrequency.CollectionType.ToString());

            foreach (ListViewItem oldItem in lvSelectedFrequencies.Items)
            {
                int index = lvSelectedFrequencies.Items.IndexOf(oldItem);

                AtscFrequency oldFrequency = oldItem.Tag as AtscFrequency;
                if (oldFrequency != null && oldFrequency.ToString() == atscFrequency.ToString() &&
                    oldFrequency.Provider == atscFrequency.Provider)
                {
                    if (oldFrequency.CollectionType != atscFrequency.CollectionType)
                    {
                        DialogResult result = MessageBox.Show("The frequency has already been selected." + Environment.NewLine + Environment.NewLine +
                            "Do you want to overwrite it?", "EPG Centre", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        switch (result)
                        {
                            case DialogResult.Cancel:
                                return;
                            case DialogResult.Yes:
                                lvSelectedFrequencies.Items.Remove(oldItem);
                                lbScanningFrequencies.Items.Remove(oldItem.Tag);
                                lvSelectedFrequencies.Items.Insert(index, newItem);
                                lbScanningFrequencies.Items.Insert(index, atscFrequency);
                                return;
                            default:
                                lvSelectedFrequencies.Items.Add(newItem);
                                lbScanningFrequencies.Items.Add(atscFrequency);
                                return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("The frequency has already been selected with the same parameters.",
                            "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            lvSelectedFrequencies.Items.Add(newItem);
            lbScanningFrequencies.Items.Add(atscFrequency);
        }

        private void cboClearQamProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboClearQamProvider.SelectedItem != null)
                cboClearQamScanningFrequency.DataSource = ((ClearQamProvider)cboClearQamProvider.SelectedItem).Channels;
        }

        private void cboClearQamScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboClearQamScanningFrequency.SelectedItem as TuningFrequency;
            cboClearQamCollectionType.Text = tuningFrequency.CollectionType.ToString();
        }

        private void btAddClearQam_Click(object sender, EventArgs e)
        {
            ClearQamFrequency clearQamFrequency = (ClearQamFrequency)(cboClearQamScanningFrequency.SelectedItem as ClearQamFrequency).Clone();
            clearQamFrequency.CollectionType = (CollectionType)cboClearQamCollectionType.SelectedItem;

            ListViewItem newItem = new ListViewItem(clearQamFrequency.ToString());
            newItem.Tag = clearQamFrequency;
            newItem.SubItems.Add(((ClearQamProvider)cboClearQamProvider.SelectedItem).Name);
            newItem.SubItems.Add("Clear QAM");
            newItem.SubItems.Add(clearQamFrequency.CollectionType.ToString());

            foreach (ListViewItem oldItem in lvSelectedFrequencies.Items)
            {
                int index = lvSelectedFrequencies.Items.IndexOf(oldItem);

                ClearQamFrequency oldFrequency = oldItem.Tag as ClearQamFrequency;
                if (oldFrequency != null && oldFrequency.ToString() == clearQamFrequency.ToString() &&
                    oldFrequency.Provider == clearQamFrequency.Provider)
                {
                    if (oldFrequency.CollectionType != clearQamFrequency.CollectionType)
                    {
                        DialogResult result = MessageBox.Show("The frequency has already been selected." + Environment.NewLine + Environment.NewLine +
                            "Do you want to overwrite it?", "EPG Centre", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        switch (result)
                        {
                            case DialogResult.Cancel:
                                return;
                            case DialogResult.Yes:
                                lvSelectedFrequencies.Items.Remove(oldItem);
                                lbScanningFrequencies.Items.Remove(oldItem.Tag);
                                lvSelectedFrequencies.Items.Insert(index, newItem);
                                lbScanningFrequencies.Items.Insert(index, clearQamFrequency);
                                return;
                            default:
                                lvSelectedFrequencies.Items.Add(newItem);
                                lbScanningFrequencies.Items.Add(clearQamFrequency);
                                return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("The frequency has already been selected with the same parameters.",
                            "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            lvSelectedFrequencies.Items.Add(newItem);
            lbScanningFrequencies.Items.Add(clearQamFrequency);
        }

        private void btISDBLNBDefaults_Click(object sender, EventArgs e)
        {
            SatelliteDish defaultSatellite = SatelliteDish.Default;

            txtISDBLNBLow.Text = defaultSatellite.LNBLowBandFrequency.ToString();
            txtISDBLNBHigh.Text = defaultSatellite.LNBHighBandFrequency.ToString();
            txtISDBLNBSwitch.Text = defaultSatellite.LNBSwitchFrequency.ToString();

            cboISDBDiseqc.SelectedIndex = 0;
        }

        private void cboISDBSatellite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboISDBSatellite.SelectedItem != null)
                cboISDBSScanningFrequency.DataSource = ((ISDBSatelliteProvider)cboISDBSatellite.SelectedItem).Frequencies;
        }

        private void cboISDBSScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboISDBSScanningFrequency.SelectedItem as TuningFrequency;
            cboISDBSCollectionType.Text = tuningFrequency.CollectionType.ToString();
        }

        private void btAddISDBSatellite_Click(object sender, EventArgs e)
        {
            ISDBSatelliteFrequency isdbsFrequency = (ISDBSatelliteFrequency)(cboISDBSScanningFrequency.SelectedItem as ISDBSatelliteFrequency).Clone();
            isdbsFrequency.CollectionType = (CollectionType)cboISDBSCollectionType.SelectedItem;

            isdbsFrequency.SatelliteDish = new SatelliteDish();

            try
            {
                isdbsFrequency.SatelliteDish.LNBLowBandFrequency = Int32.Parse(txtISDBLNBLow.Text.Trim());
                isdbsFrequency.SatelliteDish.LNBHighBandFrequency = Int32.Parse(txtISDBLNBHigh.Text.Trim());
                isdbsFrequency.SatelliteDish.LNBSwitchFrequency = Int32.Parse(txtISDBLNBSwitch.Text.Trim());
            }
            catch (FormatException)
            {
                MessageBox.Show("A dish parameter is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (ArithmeticException)
            {
                MessageBox.Show("A dish parameter is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cboISDBDiseqc.SelectedIndex != 0)
                isdbsFrequency.SatelliteDish.DiseqcSwitch = cboISDBDiseqc.Text;

            ListViewItem newItem = new ListViewItem(isdbsFrequency.ToString());
            newItem.Tag = isdbsFrequency;
            newItem.SubItems.Add(((ISDBSatelliteProvider)cboISDBSatellite.SelectedItem).Name);
            newItem.SubItems.Add("ISDB Satellite");
            newItem.SubItems.Add(isdbsFrequency.CollectionType.ToString());
            newItem.SubItems.Add(isdbsFrequency.SatelliteDish.LNBLowBandFrequency.ToString());
            newItem.SubItems.Add(isdbsFrequency.SatelliteDish.LNBHighBandFrequency.ToString());
            newItem.SubItems.Add(isdbsFrequency.SatelliteDish.LNBSwitchFrequency.ToString());
            if (isdbsFrequency.SatelliteDish.DiseqcSwitch != null)
                newItem.SubItems.Add(isdbsFrequency.SatelliteDish.DiseqcSwitch);

            foreach (ListViewItem oldItem in lvSelectedFrequencies.Items)
            {
                int index = lvSelectedFrequencies.Items.IndexOf(oldItem);

                SatelliteFrequency oldFrequency = oldItem.Tag as SatelliteFrequency;
                if (oldFrequency != null && oldFrequency.ToString() == isdbsFrequency.ToString() &&
                    oldFrequency.Provider == isdbsFrequency.Provider)
                {
                    if (oldFrequency.CollectionType != isdbsFrequency.CollectionType ||
                        oldFrequency.SatelliteDish.LNBLowBandFrequency != isdbsFrequency.SatelliteDish.LNBLowBandFrequency ||
                        oldFrequency.SatelliteDish.LNBHighBandFrequency != isdbsFrequency.SatelliteDish.LNBHighBandFrequency ||
                        oldFrequency.SatelliteDish.LNBSwitchFrequency != isdbsFrequency.SatelliteDish.LNBSwitchFrequency ||
                        oldFrequency.SatelliteDish.DiseqcSwitch != isdbsFrequency.SatelliteDish.DiseqcSwitch)
                    {
                        DialogResult result = MessageBox.Show("The frequency has already been selected." + Environment.NewLine + Environment.NewLine +
                            "Do you want to overwrite it?", "EPG Centre", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        switch (result)
                        {
                            case DialogResult.Cancel:
                                return;
                            case DialogResult.Yes:
                                lvSelectedFrequencies.Items.Remove(oldItem);
                                lbScanningFrequencies.Items.Remove(oldItem.Tag);
                                lvSelectedFrequencies.Items.Insert(index, newItem);
                                lbScanningFrequencies.Items.Insert(index, isdbsFrequency);
                                return;
                            default:
                                lvSelectedFrequencies.Items.Add(newItem);
                                lbScanningFrequencies.Items.Add(isdbsFrequency);
                                return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("The frequency has already been selected with the same parameters.",
                            "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            lvSelectedFrequencies.Items.Add(newItem);
            lbScanningFrequencies.Items.Add(isdbsFrequency);
        }

        private void cboISDBTProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboISDBTProvider.SelectedItem != null)
                cboISDBTScanningFrequency.DataSource = ((ISDBTerrestrialProvider)cboISDBTProvider.SelectedItem).Channels;
        }

        private void cboISDBTScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboISDBTScanningFrequency.SelectedItem as TuningFrequency;
            cboISDBTCollectionType.Text = tuningFrequency.CollectionType.ToString();
        }

        private void btAddISDBTerrestrial_Click(object sender, EventArgs e)
        {
            ISDBTerrestrialFrequency isdbtFrequency = (ISDBTerrestrialFrequency)(cboISDBTScanningFrequency.SelectedItem as ISDBTerrestrialFrequency).Clone();
            isdbtFrequency.CollectionType = (CollectionType)cboISDBTCollectionType.SelectedItem;

            ListViewItem newItem = new ListViewItem(isdbtFrequency.ToString());
            newItem.Tag = isdbtFrequency;
            newItem.SubItems.Add(((ISDBTerrestrialProvider)cboISDBTProvider.SelectedItem).Name);
            newItem.SubItems.Add("ISDB Terrestrial");
            newItem.SubItems.Add(isdbtFrequency.CollectionType.ToString());

            foreach (ListViewItem oldItem in lvSelectedFrequencies.Items)
            {
                int index = lvSelectedFrequencies.Items.IndexOf(oldItem);

                AtscFrequency oldFrequency = oldItem.Tag as AtscFrequency;
                if (oldFrequency != null && oldFrequency.ToString() == isdbtFrequency.ToString() &&
                    oldFrequency.Provider == isdbtFrequency.Provider)
                {
                    if (oldFrequency.CollectionType != isdbtFrequency.CollectionType)
                    {
                        DialogResult result = MessageBox.Show("The frequency has already been selected." + Environment.NewLine + Environment.NewLine +
                            "Do you want to overwrite it?", "EPG Centre", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        switch (result)
                        {
                            case DialogResult.Cancel:
                                return;
                            case DialogResult.Yes:
                                lvSelectedFrequencies.Items.Remove(oldItem);
                                lbScanningFrequencies.Items.Remove(oldItem.Tag);
                                lvSelectedFrequencies.Items.Insert(index, newItem);
                                lbScanningFrequencies.Items.Insert(index, isdbtFrequency);
                                return;
                            default:
                                lvSelectedFrequencies.Items.Add(newItem);
                                lbScanningFrequencies.Items.Add(isdbtFrequency);
                                return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("The frequency has already been selected with the same parameters.",
                            "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            lvSelectedFrequencies.Items.Add(newItem);
            lbScanningFrequencies.Items.Add(isdbtFrequency);
        }

        private void lvSelectedFrequencies_SelectedIndexChanged(object sender, EventArgs e)
        {
            btDelete.Enabled = (lvSelectedFrequencies.SelectedItems.Count != 0);
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvSelectedFrequencies.SelectedItems)
            {
                lvSelectedFrequencies.Items.Remove(item);
                lbScanningFrequencies.Items.Remove(item.Tag);
            }

            btDelete.Enabled = (lvSelectedFrequencies.SelectedItems.Count != 0);            
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browseFile = new FolderBrowserDialog();
            browseFile.Description = "EPG Centre - Find Output File Directory";            
            DialogResult result = browseFile.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            if (!browseFile.SelectedPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                txtOutputFile.Text = Path.Combine(browseFile.SelectedPath, "TVGuide.xml");
            else
                txtOutputFile.Text = browseFile.SelectedPath + "TVGuide.xml";
        }

        private void cbUseBSEPG_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUseBSEPG.Checked)
            {
                cbWMCImport.Checked = false;
                txtImportName.Text = string.Empty;
                txtImportName.Enabled = false;
                cbAutoMapEPG.Checked = false;
                cbAutoMapEPG.Enabled = false;
                cbAllSeries.Checked = false;
                cbAllSeries.Enabled = false;

                cbUseDVBViewer.Checked = false;
                cbDVBViewerImport.Checked = false;
                cbRecordingServiceImport.Checked = false;
                cbDVBViewerClear.Checked = false;
                cbDVBViewerClear.Enabled = false;
            }
        }

        private void txtImportName_KeyPressAlphaNumeric(object sender, KeyPressEventArgs e)
        {
            Regex alphaNumericPattern = new Regex(@"[a-zA-Z0-9\s\b]");
            e.Handled = !alphaNumericPattern.IsMatch(e.KeyChar.ToString());
        }

        private void cbWMCImport_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWMCImport.Checked)
            {
                txtImportName.Text = string.Empty;
                txtImportName.Enabled = true;
                cbAutoMapEPG.Enabled = true;
                cbAllSeries.Enabled = true; 
                cbUseBSEPG.Checked = false;

                cbUseDVBViewer.Checked = false;
                cbDVBViewerImport.Checked = false;
                cbRecordingServiceImport.Checked = false;
                cbDVBViewerClear.Checked = false;
                cbDVBViewerClear.Enabled = false;
            }
            else
            {
                txtImportName.Text = string.Empty;
                txtImportName.Enabled = false;
                cbAutoMapEPG.Checked = false;                
                cbAutoMapEPG.Enabled = false;
                cbAllSeries.Checked = false;
                cbAllSeries.Enabled = false;
            }
        }

        private void cbUseDVBViewer_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUseDVBViewer.Checked)
            {
                cbDVBViewerImport.Checked = false;
                cbRecordingServiceImport.Checked = false;

                cbDVBViewerClear.Checked = false;
                cbDVBViewerClear.Enabled = false;

                nudPort.Enabled = false;

                cbWMCImport.Checked = false;
                txtImportName.Text = string.Empty;
                txtImportName.Enabled = false;
                cbAutoMapEPG.Checked = false;
                cbAutoMapEPG.Enabled = false;
                cbAllSeries.Checked = false;
                cbAllSeries.Enabled = false;
                cbUseBSEPG.Checked = false;
            }            
        }

        private void cbDVBViewerImport_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDVBViewerImport.Checked)
            {
                cbUseDVBViewer.Checked = false;
                cbRecordingServiceImport.Checked = false;
                
                cbDVBViewerClear.Checked = false;
                cbDVBViewerClear.Enabled = false;                
                
                nudPort.Enabled = false;

                cbWMCImport.Checked = false;
                cbAutoMapEPG.Checked = false;
                cbAutoMapEPG.Enabled = false;
                cbAllSeries.Checked = false;
                cbAllSeries.Enabled = false;
                cbUseBSEPG.Checked = false;
            }
        }

        private void cbRecordingServiceImport_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRecordingServiceImport.Checked)
            {
                cbUseDVBViewer.Checked = false;
                cbDVBViewerImport.Checked = false;                

                cbDVBViewerClear.Enabled = true;
                nudPort.Enabled = true;

                cbWMCImport.Checked = false;
                cbAutoMapEPG.Checked = false;
                cbAutoMapEPG.Enabled = false;
                cbAllSeries.Checked = false;
                cbAllSeries.Enabled = false;
                cbUseBSEPG.Checked = false;
            }
            else
            {
                cbDVBViewerClear.Checked = false;
                cbDVBViewerClear.Enabled = false;

                nudPort.Enabled = false;
            }
        }

        private void cbStoreStationInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (cbStoreStationInfo.Checked)
                cbUseStoredStationInfo.Checked = false;

        }

        private void cbUseStoredStationInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUseStoredStationInfo.Checked)
                cbStoreStationInfo.Checked = false;
        }

        private void onCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (bindingList[e.RowIndex].Excluded)
                e.CellStyle.ForeColor = Color.Red;
        }

        private void dgServices_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgServices.CurrentCell.ColumnIndex == dgServices.Columns["newNameColumn"].Index)
            {
                TextBox textEdit = e.Control as TextBox;
                textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressNumeric);
                textEdit.KeyPress += new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
            }
            else
            {
                if (dgServices.CurrentCell.ColumnIndex == dgServices.Columns["logicalChannelNumberColumn"].Index)
                {
                    TextBox textEdit = e.Control as TextBox;
                    textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                    textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressNumeric);
                    textEdit.KeyPress += new KeyPressEventHandler(textEdit_KeyPressNumeric);
                }
            }
        }

        private void textEdit_KeyPressAlphaNumeric(object sender, KeyPressEventArgs e)
        {
            Regex alphaNumericPattern = new Regex(@"[a-zA-Z0-9!&*()-+?\s\b]");
            e.Handled = !alphaNumericPattern.IsMatch(e.KeyChar.ToString());
        }

        private void textEdit_KeyPressNumeric(object sender, KeyPressEventArgs e)
        {
            if ("0123456789\b".IndexOf(e.KeyChar) == -1)
                e.Handled = true;
        }

        private void dgServices_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgServices.IsCurrentCellDirty && dgServices.Columns[dgServices.CurrentCell.ColumnIndex].Name == "excludedColumn")
                dgServices.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgServices_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgServices.Columns[e.ColumnIndex].Name != "excludedColumn")
                return;

            if (bindingList[e.RowIndex].Excluded)
            {
                foreach (DataGridViewCell cell in dgServices.Rows[e.RowIndex].Cells)
                {
                    cell.Style.ForeColor = Color.Red;
                    cell.Style.SelectionForeColor = Color.Red;
                }
            }
            else
            {
                foreach (DataGridViewCell cell in dgServices.Rows[e.RowIndex].Cells)
                {
                    cell.Style.ForeColor = Color.Black;
                    cell.Style.SelectionForeColor = Color.White;
                }
            }

            dgServices.Invalidate();
        }

        private void dgServices_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgServices.EndEdit();

            if (sortedColumnName == null)
            {
                sortedAscending = true;
                sortedColumnName = dgServices.Columns[e.ColumnIndex].Name;
            }
            else
            {
                if (sortedColumnName == dgServices.Columns[e.ColumnIndex].Name)
                    sortedAscending = !sortedAscending;
                else
                    sortedColumnName = dgServices.Columns[e.ColumnIndex].Name; 
            }

            Collection<TVStation> sortedStations = new Collection<TVStation>();

            foreach (TVStation station in bindingList)
            {
                switch (dgServices.Columns[e.ColumnIndex].Name)
                {
                    case "nameColumn":
                        addInOrder(sortedStations, station, sortedAscending, "Name");
                        break;
                    case "originalNetworkIDColumn":
                        addInOrder(sortedStations, station, sortedAscending, "ONID");
                        break;
                    case "transportStreamIDColumn":
                        addInOrder(sortedStations, station, sortedAscending, "TSID");
                        break;
                    case "serviceIDColumn":
                        addInOrder(sortedStations, station, sortedAscending, "SID");
                        break;
                    case "excludedColumn":
                        addInOrder(sortedStations, station, sortedAscending, "Excluded");
                        break;
                    case "newNameColumn":
                        addInOrder(sortedStations, station, sortedAscending, "NewName");
                        break;
                    case "logicalChannelNumberColumn":
                        addInOrder(sortedStations, station, sortedAscending, "ChannelNumber");
                        break;
                    default:
                        return;
                }
            }

            bindingList = new BindingList<TVStation>();
            foreach (TVStation station in sortedStations)
                bindingList.Add(station);

            tVStationBindingSource.DataSource = bindingList;
        }

        private void addInOrder(Collection<TVStation> stations, TVStation newStation, bool sortedAscending, string keyName)
        {
            sortedKeyName = keyName;

            foreach (TVStation oldStation in stations)
            {
                if (sortedAscending)
                {
                    if (oldStation.CompareForSorting(newStation, keyName) > 0)
                    {
                        stations.Insert(stations.IndexOf(oldStation), newStation);
                        return;
                    }
                }
                else
                {
                    if (oldStation.CompareForSorting(newStation, keyName) < 0)
                    {
                        stations.Insert(stations.IndexOf(oldStation), newStation);
                        return;
                    }
                }
            }

            stations.Add(newStation);
        }

        private void cmdClearScan_Click(object sender, EventArgs e)
        {
            bindingList.Clear();
            TVStation.StationCollection.Clear();
        }

        private void tbcParametersDeselecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabServices && pbarChannels.Enabled)
                e.Cancel = true;
        }

        private void cmdScan_Click(object sender, EventArgs e)
        {
            if (cmdScan.Text == "Stop Scan")
            {
                Logger.Instance.Write("Stop scan requested");
                workerScanStations.CancelAsync();
                bool reply = resetEvent.WaitOne(new TimeSpan(0, 0, 45));
                cmdClearScan.Enabled = (bindingList != null && bindingList.Count == 0);
                cmdScan.Text = "Start Scan";
                cmdSelectAll.Enabled = true;
                cmdSelectNone.Enabled = true;
                lblScanning.Visible = false;
                pbarChannels.Enabled = false;
                pbarChannels.Visible = false;
                
                btPlusScan.Text = "Start Scan";
                lblPlusScanning.Visible = false;
                pbarPlusScan.Enabled = false;
                pbarPlusScan.Visible = false;

                MainWindow.ChangeMenuItemAvailability(true);
                
                return;
            }

            Logger.Instance.Write("Scan started");

            scanningFrequencies = new Collection<TuningFrequency>();
            foreach (ListViewItem item in lvSelectedFrequencies.Items)
            {
                TuningFrequency tuningFrequency = item.Tag as TuningFrequency;

                bool reply = checkTunerSelection(tuningFrequency);
                if (reply)
                    scanningFrequencies.Add(tuningFrequency);
                else
                    MessageBox.Show("Frequency " + tuningFrequency.ToString() + " will not be scanned as a suitable tuner has not been selected.",
                        "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (scanningFrequencies.Count == 0)
            {
                MessageBox.Show("No frequencies available to scan.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cmdClearScan.Enabled = false;
            cmdScan.Text = "Stop Scan";
            cmdSelectAll.Enabled = false;
            cmdSelectNone.Enabled = false;
            lblScanning.Visible = true;            
            pbarChannels.Visible = true;
            pbarChannels.Enabled = true;

            btPlusScan.Text = "Stop Scan";
            lblPlusScanning.Visible = true;
            pbarPlusScan.Enabled = true;
            pbarPlusScan.Visible = true;

            MainWindow.ChangeMenuItemAvailability(false);

            useSignalPresent = cbUseSignalPresent.Checked;
            switchAfterPlay = cbSwitchAfterPlay.Checked;
            repeatDiseqc = cbRepeatDiseqc.Checked;

            if (scanningFrequencies[0].TunerType != TunerType.ATSC)
            {
                dgServices.Columns["originalNetworkIDColumn"].HeaderText = "ONID";
                dgServices.Columns["transportStreamIDColumn"].HeaderText = "TSID";
                dgServices.Columns["serviceIDColumn"].HeaderText = "SID";
                dgServices.Columns["logicalChannelNumberColumn"].Visible = true;
            }
            else
            {
                dgServices.Columns["originalNetworkIDColumn"].HeaderText = "Frequency";
                dgServices.Columns["transportStreamIDColumn"].HeaderText = "Channel";
                dgServices.Columns["serviceIDColumn"].HeaderText = "Sub-Channel";
                dgServices.Columns["logicalChannelNumberColumn"].Visible = false;
            }

            workerScanStations = new BackgroundWorker();
            workerScanStations.WorkerReportsProgress = true;
            workerScanStations.WorkerSupportsCancellation = true;
            workerScanStations.DoWork += new DoWorkEventHandler(scanStationsDoScan);
            workerScanStations.RunWorkerCompleted += new RunWorkerCompletedEventHandler(scanStationsRunWorkerCompleted);
            workerScanStations.ProgressChanged += new ProgressChangedEventHandler(scanStationsProgressChanged);
            workerScanStations.RunWorkerAsync(scanningFrequencies);
        }

        private bool checkTunerSelection(TuningFrequency tuningFrequency)
        {
            if (clbTuners.GetItemChecked(0))
                return(true);

            for (int index = 1; index < clbTuners.Items.Count; index++)
            {
                if (clbTuners.GetItemChecked(index))
                {
                    Tuner tuner = (Tuner)clbTuners.Items[index];

                    switch (tuningFrequency.TunerType)
                    {                        
                        case TunerType.Satellite:
                            if (tuner.Supports(TunerNodeType.Satellite))
                                return (true);
                            break;
                        case TunerType.Terrestrial:
                            if (tuner.Supports(TunerNodeType.Terrestrial))
                                return (true);
                            break;
                        case TunerType.Cable:
                            if (tuner.Supports(TunerNodeType.Cable))
                                return (true);
                            break;
                        case TunerType.ATSC:
                        case TunerType.ATSCCable:
                            if (tuner.Supports(TunerNodeType.ATSC))
                                return (true);
                            break;
                        case TunerType.ClearQAM:
                            if (tuner.Supports(TunerNodeType.Cable))
                                return (true);
                            break;
                        case TunerType.ISDBS:
                            if (tuner.Supports(TunerNodeType.ISDBS))
                                return (true);
                            break;
                        case TunerType.ISDBT:
                            if (tuner.Supports(TunerNodeType.ISDBT))
                                return (true);
                            break;
                        default:
                            return (false);
                    }
                }  
            }

            return (false);
        }

        private void scanStationsProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblScanning.Text = "Scanning " + scanningFrequencies[e.ProgressPercentage];
            lblPlusScanning.Text = "Scanning " + scanningFrequencies[e.ProgressPercentage];
        }

        private void scanStationsDoScan(object sender, DoWorkEventArgs e)
        {
            Collection<TuningFrequency> frequencies = e.Argument as Collection<TuningFrequency>;
            
            Collection<int> tuners = new Collection<int>();
            for (int index = 1; index < clbTuners.Items.Count; index++ )
            {
                if (clbTuners.GetItemChecked(index))
                    tuners.Add(Tuner.TunerCollection.IndexOf((Tuner)clbTuners.Items[index]) + 1);
            }

            Logger.Instance.Write("Scanning " + frequencies.Count + " frequencies");

            foreach (TuningFrequency frequency in frequencies)
            {
                if ((sender as BackgroundWorker).CancellationPending)
                {
                    Logger.Instance.Write("Scan abandoned by user");
                    e.Cancel = true;
                    resetEvent.Set();
                    return;
                }

                Logger.Instance.Write("Scanning frequency " + frequency.ToString() + " on " + frequency.TunerType);
                (sender as BackgroundWorker).ReportProgress(frequencies.IndexOf(frequency));

                TunerNodeType tunerNodeType;
                TuningSpec tuningSpec;

                SatelliteFrequency satelliteFrequency = frequency as SatelliteFrequency;
                if (satelliteFrequency != null)
                {
                    tunerNodeType = TunerNodeType.Satellite;
                    tuningSpec = new TuningSpec((Satellite)satelliteFrequency.Provider, satelliteFrequency);
                }
                else
                {
                    TerrestrialFrequency terrestrialFrequency = frequency as TerrestrialFrequency;
                    if (terrestrialFrequency != null)
                    {
                        tunerNodeType = TunerNodeType.Terrestrial;
                        tuningSpec = new TuningSpec(terrestrialFrequency);
                    }
                    else
                    {
                        CableFrequency cableFrequency = frequency as CableFrequency;
                        if (cableFrequency != null)
                        {
                            tunerNodeType = TunerNodeType.Cable;
                            tuningSpec = new TuningSpec(cableFrequency);
                        }
                        else
                        {
                            AtscFrequency atscFrequency = frequency as AtscFrequency;
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
                                ClearQamFrequency clearQamFrequency = frequency as ClearQamFrequency;
                                if (clearQamFrequency != null)
                                {
                                    tunerNodeType = TunerNodeType.Cable;
                                    tuningSpec = new TuningSpec(clearQamFrequency);
                                }
                                else
                                {
                                    ISDBSatelliteFrequency isdbSatelliteFrequency = frequency as ISDBSatelliteFrequency;
                                    if (isdbSatelliteFrequency != null)
                                    {
                                        tunerNodeType = TunerNodeType.ISDBS;
                                        tuningSpec = new TuningSpec((Satellite)satelliteFrequency.Provider, isdbSatelliteFrequency);
                                    }
                                    else
                                    {
                                        ISDBTerrestrialFrequency isdbTerrestrialFrequency = frequency as ISDBTerrestrialFrequency;
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
                    
                    BDAGraph graph = BDAGraph.FindTuner(tuners, tunerNodeType, tuningSpec, currentTuner, repeatDiseqc, switchAfterPlay);
                    if (graph == null)
                    {
                        Logger.Instance.Write("<e> No tuner able to tune frequency " + frequency.ToString());

                        if (frequencies.IndexOf(frequency) != frequencies.Count - 1)
                        {
                            Logger.Instance.Write("Asking user whether to continue");

                            DialogResult result = (DialogResult)dgServices.Invoke(new ShowMessage(showMessage), "No tuner able to tune frequency " + frequency.ToString() +
                                Environment.NewLine + Environment.NewLine + "Do you want to continue scanning the other frequencies?",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.No)
                            {
                                Logger.Instance.Write("User cancelled scan");
                                e.Cancel = true;
                                resetEvent.Set();
                                return;
                            }
                            else
                            {
                                Logger.Instance.Write("Scan continuing");
                                finished = true;
                            }
                        }
                        else
                        {
                            dgServices.Invoke(new ShowMessage(showMessage), "No tuner able to tune frequency " + frequency.ToString(),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            finished = true;
                        }
                    }
                    else
                    {
                        string tuneReply = checkTuning(graph, frequency, sender as BackgroundWorker);

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
                            getStations(graph, frequency, sender as BackgroundWorker);
                            graph.Dispose();
                            finished = true;
                        }
                        else
                        {
                            Logger.Instance.Write("Failed to tune frequency " + frequency.ToString());
                            graph.Dispose();
                            currentTuner = graph.Tuner;
                        }
                    }
                }
            }

            e.Cancel = true;
            resetEvent.Set();
        }

        private DialogResult showMessage(string message, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            lblScanning.Visible = false;
            pbarChannels.Visible = false;
            pbarChannels.Enabled = false;

            lblPlusScanning.Visible = false;
            pbarPlusScan.Enabled = false;
            pbarPlusScan.Visible = false;

            DialogResult result = MessageBox.Show(message, "EPG Centre", buttons, icon);
            if (result == DialogResult.Yes)
            {
                lblScanning.Visible = true;
                pbarChannels.Visible = true;
                pbarChannels.Enabled = true;

                lblPlusScanning.Visible = true;
                pbarPlusScan.Enabled = true;
                pbarPlusScan.Visible = true;
            }

            return(result);
        }

        private string checkTuning(BDAGraph graph, TuningFrequency frequency, BackgroundWorker worker)
        {
            TimeSpan timeout = new TimeSpan();
            bool done = false;
            bool locked = false;
            int frequencyRetries = 0;

            while (!done)
            {
                if (worker.CancellationPending)
                {
                    Logger.Instance.Write("Scan abandoned by user");
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
                        if (!useSignalPresent)
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
                return ("<e> The tuner failed to acquire a signal for frequency " + frequency.ToString());
        }

        private bool getStations(BDAGraph graph, TuningFrequency frequency, BackgroundWorker worker)
        {
            FrequencyScanner frequencyScanner = new FrequencyScanner(graph, worker);
            Collection<TVStation> stations = frequencyScanner.FindTVStations();

            int addedCount = 0;

            if (stations != null)
            {
                foreach (TVStation tvStation in stations)
                {
                    TVStation existingStation = TVStation.FindStation(tvStation.OriginalNetworkID, tvStation.TransportStreamID, tvStation.ServiceID);
                    if (existingStation == null)
                    {
                        tvStation.CollectionType = frequency.CollectionType;
                        bool added = TVStation.AddStation(tvStation);
                        if (added)
                        {
                            Logger.Instance.Write("Included station: " + tvStation.FixedLengthName + " (" + tvStation.FullID + " Service type " + tvStation.ServiceType + ")");
                            addedCount++;
                        }
                    }
                    else
                        existingStation.Name = tvStation.Name;
                }

                Logger.Instance.Write("Added " + addedCount + " stations for frequency " + frequency);
            }

            return (true);
        }

        private void scanStationsRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cmdScan.Text = "Start Scan";
            btPlusScan.Text = "Start Scan";

            if (e.Error != null)
                throw new InvalidOperationException("Background worker failed - see inner exception", e.Error);

            lblScanning.Visible = false;
            pbarChannels.Visible = false;
            pbarChannels.Enabled = false;
            
            lblPlusScanning.Visible = false;
            pbarPlusScan.Enabled = false;
            pbarPlusScan.Visible = false;

            MainWindow.ChangeMenuItemAvailability(true);

            populateServicesGrid();
            populatePlusChannels(lbPlusSourceChannel);
            populatePlusChannels(lbPlusDestinationChannel);
        }

        private void populateServicesGrid()
        {
            if (TVStation.StationCollection.Count != 0)
            {
                Collection<TVStation> sortedStations = new Collection<TVStation>();

                foreach (TVStation station in TVStation.StationCollection)
                    addInOrder(sortedStations, station, sortedAscending, sortedKeyName);

                bindingList = new BindingList<TVStation>();
                foreach (TVStation station in sortedStations)
                    bindingList.Add(station);

                tVStationBindingSource.DataSource = bindingList;

                dgServices.DataSource = tVStationBindingSource;
                cmdSelectAll.Enabled = true;
                cmdSelectNone.Enabled = true;
                cmdClearScan.Enabled = true;

                MessageBox.Show("The scan for channels is complete." + Environment.NewLine + Environment.NewLine +
                    "There are now " + bindingList.Count + " channels in the list.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                cmdScan.Enabled = true;
                cmdSelectAll.Enabled = false;
                cmdSelectNone.Enabled = false;
                cmdClearScan.Enabled = false;
            }
        }

        private void tmrProgressTick(object sender, EventArgs e)
        {
            
        }

        private void cmdIncludeAll_Click(object sender, EventArgs e)
        {
            foreach (TVStation station in bindingList)
                station.Excluded = false;

            foreach (DataGridViewRow row in dgServices.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.ForeColor = Color.Black;
                    cell.Style.SelectionForeColor = Color.White;
                }
            }
        }

        private void cmdExcludeAll_Click(object sender, EventArgs e)
        {
            foreach (TVStation station in bindingList)
                station.Excluded = true;

            foreach (DataGridViewRow row in dgServices.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.ForeColor = Color.Red;
                    cell.Style.SelectionForeColor = Color.Red;
                }
            }
        }

        private void populatePlusChannels(ListBox listBox)
        {
            Collection<TVStation> stationCollection = new Collection<TVStation>();
            
            foreach (TVStation station in TVStation.StationCollection)
                stationCollection.Add(station);

            listBox.DataSource = stationCollection;
        }

        private void btPlusAdd_Click(object sender, EventArgs e)
        {
            if ((TVStation)lbPlusSourceChannel.SelectedItem == (TVStation)lbPlusDestinationChannel.SelectedItem)
            {
                showErrorMessage("The source and destination channels must be different");
                return;
            }
           
            TimeOffsetChannel newChannel = new TimeOffsetChannel((TVStation)lbPlusSourceChannel.SelectedItem,
                (TVStation)lbPlusDestinationChannel.SelectedItem,
                (int)nudPlusIncrement.Value);

            ListViewItem newItem = new ListViewItem(newChannel.SourceChannel.Name);
            newItem.Tag = newChannel;
            newItem.SubItems.Add(newChannel.DestinationChannel.Name);
            newItem.SubItems.Add(newChannel.Offset.ToString());

            foreach (ListViewItem oldItem in lvPlusSelectedChannels.Items)
            {
                int index = lvPlusSelectedChannels.Items.IndexOf(oldItem);
 
                TimeOffsetChannel oldChannel = oldItem.Tag as TimeOffsetChannel;

                if (oldChannel.SourceChannel.Name == newChannel.SourceChannel.Name &&
                    oldChannel.DestinationChannel.Name == newChannel.DestinationChannel.Name)
                {
                    if (oldChannel.Offset == newChannel.Offset)
                    {
                        MessageBox.Show("The channels have already been selected with the same offset.",
                            "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show("The channels have already been selected with a different offset." + Environment.NewLine + Environment.NewLine +
                            "Do you want to overwrite the existing entry?", "EPG Centre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        switch (result)
                        {
                            case DialogResult.Yes:
                                lvPlusSelectedChannels.Items.Remove(oldItem);
                                lvPlusSelectedChannels.Items.Insert(index, newItem);
                                return;
                            default:
                                return;
                        }
                    }
                }
            }

            lvPlusSelectedChannels.Items.Add(newItem);
        }

        private void lvPlusSelectedChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            btPlusDelete.Enabled = (lvPlusSelectedChannels.SelectedItems.Count != 0);
        }

        private void btPlusDelete_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvPlusSelectedChannels.SelectedItems)
                lvPlusSelectedChannels.Items.Remove(item);

            btPlusDelete.Enabled = (lvPlusSelectedChannels.SelectedItems.Count != 0);  
        }

        private void tbExcludeONID_TextChanged(object sender, EventArgs e)
        {
            btExcludeAdd.Enabled = tbExcludeONID.Text.Length != 0;
        }

        private void btExcludeAdd_Click(object sender, EventArgs e)
        {
            int originalNetworkID;
            int transportStreamID = -1;
            int startServiceID = -1;
            int endServiceID = -1;

            try
            {
                originalNetworkID = Int32.Parse(tbExcludeONID.Text.Trim());
            }
            catch (ArithmeticException)
            {
                MessageBox.Show("The original network ID is incorrect.",
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("The original network ID is incorrect.",
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (tbExcludeTSID.Text.Length != 0)
            {
                try
                {
                    transportStreamID = Int32.Parse(tbExcludeTSID.Text.Trim());
                }
                catch (ArithmeticException)
                {
                    MessageBox.Show("The transport stream ID is incorrect.",
                        "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("The transport stream ID is incorrect.",
                        "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (tbExcludeSIDStart.Text.Length != 0)
            {
                try
                {
                    startServiceID = Int32.Parse(tbExcludeSIDStart.Text.Trim());
                }
                catch (ArithmeticException)
                {
                    MessageBox.Show("The start service ID is incorrect.",
                        "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("The start service ID is incorrect.",
                        "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (tbExcludeSIDEnd.Text.Length != 0)
            {
                try
                {
                    endServiceID = Int32.Parse(tbExcludeSIDEnd.Text.Trim());
                }
                catch (ArithmeticException)
                {
                    MessageBox.Show("The end service ID is incorrect.",
                        "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("The end service ID is incorrect.",
                        "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (startServiceID == -1 && endServiceID != -1)
            {
                MessageBox.Show("The start service ID cannot be omitted if an end service ID is entered.",
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ChannelFilterEntry newFilter = new ChannelFilterEntry(originalNetworkID, transportStreamID, startServiceID, endServiceID);

            ListViewItem newItem = new ListViewItem(originalNetworkID.ToString());
            newItem.Tag = newFilter;
            if (transportStreamID != -1)
                newItem.SubItems.Add(transportStreamID.ToString());
            else
                newItem.SubItems.Add("");
            if (startServiceID != -1)
                newItem.SubItems.Add(startServiceID.ToString());
            else
                newItem.SubItems.Add("");
            if (endServiceID != -1)
                newItem.SubItems.Add(endServiceID.ToString());
            else
                newItem.SubItems.Add("");

            foreach (ListViewItem oldItem in lvExcludedIdentifiers.Items)
            {
                int index = lvExcludedIdentifiers.Items.IndexOf(oldItem);

                ChannelFilterEntry oldFilter= oldItem.Tag as ChannelFilterEntry;

                if (oldFilter.OriginalNetworkID == newFilter.OriginalNetworkID &&
                    oldFilter.TransportStreamID == newFilter.TransportStreamID &&
                    oldFilter.StartServiceID == newFilter.StartServiceID &&
                    oldFilter.EndServiceID == newFilter.EndServiceID)
                {
                    MessageBox.Show("A filter has already been created with the same parameters.",
                        "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;                    
                }
            }

            lvExcludedIdentifiers.Items.Add(newItem);
        }

        private void btExcludeDelete_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvExcludedIdentifiers.SelectedItems)
                lvExcludedIdentifiers.Items.Remove(item);

            btExcludeDelete.Enabled = (lvExcludedIdentifiers.SelectedItems.Count != 0);  
        }

        private void lvExcludedIdentifiers_SelectedIndexChanged(object sender, EventArgs e)
        {
            btExcludeDelete.Enabled = (lvExcludedIdentifiers.SelectedItems.Count != 0);
        }

        private void tbRepeatTitle_TextChanged(object sender, EventArgs e)
        {
            btRepeatAdd.Enabled = tbRepeatTitle.Text.Length != 0 || tbRepeatDescription.Text.Length != 0; 
        }

        private void tbRepeatDescription_TextChanged(object sender, EventArgs e)
        {
            btRepeatAdd.Enabled = tbRepeatTitle.Text.Length != 0 || tbRepeatDescription.Text.Length != 0;
        }

        private void btRepeatAdd_Click(object sender, EventArgs e)
        {
            ListViewItem newItem = new ListViewItem(tbRepeatTitle.Text);
            newItem.SubItems.Add(tbRepeatDescription.Text);
            lvRepeatPrograms.Items.Add(newItem);

            tbRepeatTitle.Text = string.Empty;
            tbRepeatDescription.Text = string.Empty;
        }

        private void lvRepeatPrograms_SelectedIndexChanged(object sender, EventArgs e)
        {
            btRepeatDelete.Enabled = lvRepeatPrograms.Items.Count != 0;
        }

        private void btRepeatDelete_Click(object sender, EventArgs e)
        {
            lvRepeatPrograms.Items.Remove(lvRepeatPrograms.SelectedItems[0]);
            btRepeatDelete.Enabled = lvRepeatPrograms.Items.Count != 0;
        }

        private void btTimeoutDefaults_Click(object sender, EventArgs e)
        {
            nudSignalLockTimeout.Value = timeoutLock;
            nudDataCollectionTimeout.Value = timeoutCollection;
            nudScanRetries.Value = timeoutRetries;
        }

        private void cboLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            Country country = cboLocation.SelectedItem as Country;
            if (country == null)
                cboLocationArea.DataSource = null;

            if (country.Areas == null || country.Areas.Count == 0)
                cboLocationArea.DataSource = null;
            else
                cboLocationArea.DataSource = country.Areas;            
        }

        private void cboLocationArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            Area area = cboLocationArea.SelectedItem as Area;
            if (area == null || area.Regions == null || area.Regions.Count == 0)
            {
                cboLocationRegion.DataSource = null;
                return;
            }

            cboLocationRegion.DataSource = area.Regions;
        }

        private void cbHexPids_CheckedChanged(object sender, EventArgs e)
        {
            nudEITPid.Hexadecimal = cbHexPids.Checked;
            nudMHW1Pid1.Hexadecimal = cbHexPids.Checked;
            nudMHW1Pid2.Hexadecimal = cbHexPids.Checked;
            nudMHW2Pid1.Hexadecimal = cbHexPids.Checked;
            nudMHW2Pid2.Hexadecimal = cbHexPids.Checked;
            nudMHW2Pid3.Hexadecimal = cbHexPids.Checked;
        }

        private void cbManualTime_CheckedChanged(object sender, EventArgs e)
        {
            gpTimeAdjustments.Enabled = cbManualTime.Checked;

            if (!gpTimeAdjustments.Enabled)
            {
                nudCurrentOffsetHours.Value = 0;
                nudCurrentOffsetMinutes.Value = 0;
                nudNextOffsetHours.Value = 0;
                nudNextOffsetMinutes.Value = 0;
                tbChangeDate.Text = string.Empty;
                nudChangeHours.Value = 0;
                nudChangeMinutes.Value = 0;
            }                
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

        public bool PrepareToSave()
        {
            dgServices.EndEdit();

            bool reply = validateData();
            if (reply)
                setRunParameters();

            return (reply);
        }

        private bool validateData()
        {
            int selectedTunerCount = 0;

            for (int index = 0; index < clbTuners.Items.Count; index++)
            {
                bool isChecked = clbTuners.GetItemChecked(index);
                if (isChecked)
                    selectedTunerCount++;
            }

            if (selectedTunerCount == 0)
            {
                showErrorMessage("No tuner selected.");
                return (false);
            }

            if (lvSelectedFrequencies.Items.Count == 0)
            {
                showErrorMessage("No frequencies selected.");
                return (false);
            }

            Collection<int> tuners = new Collection<int>();

            for (int index = 1; index < clbTuners.Items.Count; index++)
            {
                if (clbTuners.GetItemChecked(index))
                    tuners.Add(Tuner.TunerCollection.IndexOf((Tuner)clbTuners.Items[index]) + 1);
            }

            foreach (ListViewItem item in lvSelectedFrequencies.Items)
            {
                string reply = checkTunerSelectionValid(tuners, item.Tag as TuningFrequency);
                if (reply != null)
                {
                    showErrorMessage(reply);
                    return (false);
                }
            }

            if (txtOutputFile.Text.Trim() == string.Empty)
            {
                showErrorMessage("The output file name is incorrect.");
                return (false);
            }

            if (bindingList != null)
            {
                foreach (TVStation station in bindingList)
                {
                    if (station.Excluded && (station.NewName != null || (station.NewName != null && station.NewName.Trim() != string.Empty) || station.LogicalChannelNumber != -1))
                    {
                        showErrorMessage("Station " + station.Name + " has been both excluded and updated.");
                        return (false);
                    }
                }
            }

            if (tbExcludedMaxChannel.Text.Trim() != string.Empty)
            {
                try
                {
                    Int32.Parse(tbExcludedMaxChannel.Text);
                }
                catch (ArgumentException)
                {
                    showErrorMessage("The maximum service ID on the Filters tab is incorrect.");
                    return (false);
                }
                catch (ArithmeticException)
                {
                    showErrorMessage("The maximum channel number on the Filters tab is incorrect.");
                    return (false);
                }
            }

            foreach (ListViewItem item in lvSelectedFrequencies.Items)
            {
                if (((TuningFrequency)item.Tag).CollectionType == CollectionType.OpenTV)
                {
                    if (((Country)cboLocation.SelectedItem).Code == string.Empty)
                    {
                        showErrorMessage("A country must be selected for OpenTV collections.");
                        return (false);
                    }
                }
            }

            if (lvSelectedFrequencies.Items.Count > 1)
            {
                if (((Area)cboLocationArea.SelectedItem).Code != 0)
                {
                    showErrorMessage("An area cannot be selected if more than 1 frequency is chosen.");
                    return (false);
                }
            }

            if (cbManualTime.Checked)
            {
                if (tbChangeDate.Text.Trim().Length != 0)
                {
                    if (tbChangeDate.Text.Trim().Length != 6)
                    {
                        showErrorMessage("The date of change to the next time zone is incorrect (ddmmyy)");
                        return (false);
                    }

                    try
                    {
                        DateTime.ParseExact(tbChangeDate.Text.Trim().Substring(0, 2) + tbChangeDate.Text.Trim().Substring(2, 2) + tbChangeDate.Text.Trim().Substring(4, 2), "ddMMyy", CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        showErrorMessage("The date of change to the next time zone is incorrect (ddmmyy)");
                        return (false);
                    }
                }

                if (nudCurrentOffsetHours.Value != 0 ||
                    nudCurrentOffsetMinutes.Value != 0 ||
                    nudNextOffsetHours.Value != 0 ||
                    nudNextOffsetMinutes.Value != 0 ||
                    nudChangeHours.Value != 0 ||
                    nudChangeMinutes.Value != 0)
                {
                    if (tbChangeDate.Text.Trim().Length == 0)
                    {
                        showErrorMessage("The time zone change data is incorrect." + Environment.NewLine + Environment.NewLine +
                            "A date of change must be entered.");
                        return (false);
                    }
                }
            }

            return (true);
        }

        private string checkTunerSelectionValid(Collection<int> tuners, TuningFrequency tuningFrequency)
        {
            bool tunerValid;

            if (tuningFrequency as SatelliteFrequency != null)
                tunerValid = checkForTunerType(tuners, TunerNodeType.Satellite);
            else
            {
                if (tuningFrequency as TerrestrialFrequency != null)
                    tunerValid = checkForTunerType(tuners, TunerNodeType.Terrestrial);
                else
                {
                    if (tbcDeliverySystem.SelectedTab.Name == "tbpCable")
                        tunerValid = checkForTunerType(tuners, TunerNodeType.Cable);
                    else
                    {
                        if (tbcDeliverySystem.SelectedTab.Name == "tbpAtsc")
                            tunerValid = checkForTunerType(tuners, TunerNodeType.ATSC);
                        else
                        {
                            if (tbcDeliverySystem.SelectedTab.Name == "tbpClearQAM")
                                tunerValid = checkForTunerType(tuners, TunerNodeType.Cable);
                            else
                            {
                                if (tbcDeliverySystem.SelectedTab.Name == "tbpISDBSatellite")
                                    tunerValid = checkForTunerType(tuners, TunerNodeType.ISDBS);
                                else
                                    tunerValid = checkForTunerType(tuners, TunerNodeType.ISDBT);
                            }
                        }

                    }
                }
            }

            if (!tunerValid)
                return ("The tuner(s) selected do not support the delivery systems of the selected frequencies.");
            else
                return (null);
        }

        private bool checkForTunerType(Collection<int> tuners, TunerNodeType tunerNodeType)
        {
            if (tuners.Count == 0)
                return (true);

            foreach (int tuner in tuners)
            {
                if (Tuner.TunerCollection[tuner - 1].Supports(tunerNodeType))
                    return (true);
            }

            return (false);
        }

        private void showErrorMessage(string message)
        {
            MessageBox.Show(message, "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void setRunParameters()
        {
            RunParameters.Instance.SelectedTuners.Clear();

            for (int index = 1; index < clbTuners.Items.Count; index++ )
            {
                if (clbTuners.GetItemChecked(index))
                    RunParameters.Instance.SelectedTuners.Add(Tuner.TunerCollection.IndexOf((Tuner)clbTuners.Items[index]) + 1);
            }

            TuningFrequency.FrequencyCollection.Clear();
            foreach (ListViewItem item in lvSelectedFrequencies.Items)
                TuningFrequency.FrequencyCollection.Add(item.Tag as TuningFrequency);

            RunParameters.Instance.OutputFileName = txtOutputFile.Text;

            RunParameters.Instance.Options.Clear();

            if (cbAllowBreaks.Checked)
                RunParameters.Instance.Options.Add("ACCEPTBREAKS");
            if (cbRoundTime.Checked)
                RunParameters.Instance.Options.Add("ROUNDTIME");
            if (cbUseBSEPG.Checked)
                RunParameters.Instance.Options.Add("USEBSEPG");            
            if (cbUseChannelID.Checked)
                RunParameters.Instance.Options.Add("USECHANNELID");
            if (cbUseLCN.Checked)
                RunParameters.Instance.Options.Add("USELCN");
            if (cbUseRawCRID.Checked)
                RunParameters.Instance.Options.Add("USERAWCRID");
            if (cbUseNumericCRID.Checked)
                RunParameters.Instance.Options.Add("USENUMERICCRID");
            if (cbUseDescAsCategory.Checked)
                RunParameters.Instance.Options.Add("USEDESCASCATEGORY");
            if (cbUseStationLogos.Checked)
                RunParameters.Instance.Options.Add("USEIMAGE");
            if (cbCreateBladeRunnerFile.Checked)
                RunParameters.Instance.Options.Add("CREATEBRCHANNELS");
            if (cbCreateAreaRegionFile.Checked)
                RunParameters.Instance.Options.Add("CREATEARCHANNELS");
            if (cbCreateSameData.Checked)
                RunParameters.Instance.Options.Add("DUPLICATESAMECHANNELS");
            if (cbWMCImport.Checked)
                RunParameters.Instance.Options.Add("WMCIMPORT");
            if (txtImportName != null && txtImportName.Text.Trim() != string.Empty)
                RunParameters.Instance.WMCImportName = txtImportName.Text.Trim();
            if (cbAutoMapEPG.Checked)
                RunParameters.Instance.Options.Add("AUTOMAPEPG");
            if (cbAllSeries.Checked)
                RunParameters.Instance.Options.Add("ALLSERIES");
            if (cbCheckForRepeats.Checked)
                RunParameters.Instance.Options.Add("CHECKFORREPEATS");
            if (cbRemoveExtractedData.Checked)
                RunParameters.Instance.Options.Add("NOREMOVEDATA");
            if (cbUseDVBViewer.Checked)
                RunParameters.Instance.Options.Add("USEDVBVIEWER");
            if (cbDVBViewerImport.Checked)
                RunParameters.Instance.Options.Add("DVBVIEWERIMPORT");
            if (cbRecordingServiceImport.Checked)
                RunParameters.Instance.Options.Add("DVBVIEWERRECSVCIMPORT");
            if (cbDVBViewerClear.Checked)
                RunParameters.Instance.Options.Add("DVBVIEWERCLEAR");

            if (cbRecordingServiceImport.Checked)
                RunParameters.Instance.Options.Add("DVBVIEWERRECSVCPORT-" + nudPort.Value);

            if (cbUseContentSubtype.Checked)
                RunParameters.Instance.Options.Add("USECONTENTSUBTYPE");
            if (cbUseSignalPresent.Checked)
                RunParameters.Instance.Options.Add("USESIGNALPRESENT");
            if (cbUseSafeDiseqc.Checked)
                RunParameters.Instance.Options.Add("USESAFEDISEQC");
            if (cbSwitchAfterPlay.Checked)
                RunParameters.Instance.Options.Add("SWITCHAFTERPLAY");
            if (cbRepeatDiseqc.Checked)
                RunParameters.Instance.Options.Add("REPEATDISEQC");
            if (cbUseFreeSatTables.Checked)
                RunParameters.Instance.Options.Add("USEFREESATTABLES");
            if (cbCustomCategoriesOverride.Checked)
                RunParameters.Instance.Options.Add("CUSTOMCATEGORYOVERRIDE");
            if (cbStoreStationInfo.Checked)
                RunParameters.Instance.Options.Add("STORESTATIONINFO");
            if (cbUseStoredStationInfo.Checked)
                RunParameters.Instance.Options.Add("USESTOREDSTATIONINFO");
            if (cbProcessAllStations.Checked)
                RunParameters.Instance.Options.Add("PROCESSALLSTATIONS");
            if (cbFromService.Checked)
                RunParameters.Instance.Options.Add("RUNFROMSERVICE");
            
            if (bindingList != null)
            {
                foreach (TVStation station in bindingList)
                {
                    TVStation originalStation = TVStation.FindStation(station.OriginalNetworkID, station.TransportStreamID, station.ServiceID);
                    if (originalStation != null)
                    {
                        originalStation.Excluded = station.Excluded;
                        originalStation.NewName = station.NewName;
                        originalStation.LogicalChannelNumber = station.LogicalChannelNumber;
                    }
                }
            }

            TimeOffsetChannel.Channels.Clear();
            
            foreach (ListViewItem timeOffsetItem in lvPlusSelectedChannels.Items)
            {
                TimeOffsetChannel timeOffsetChannel = timeOffsetItem.Tag as TimeOffsetChannel;
                TimeOffsetChannel.Channels.Add(timeOffsetChannel);
            }

            if (lvExcludedIdentifiers.Items.Count == 0)
                ChannelFilterEntry.ChannelFilters = null;
            else
            {
                ChannelFilterEntry.ChannelFilters = new Collection<ChannelFilterEntry>();

                foreach (ListViewItem filterItem in lvExcludedIdentifiers.Items)
                {
                    ChannelFilterEntry filterEntry = filterItem.Tag as ChannelFilterEntry;
                    ChannelFilterEntry.ChannelFilters.Add(filterEntry);
                }
            }

            if (tbExcludedMaxChannel.Text.Trim().Length != 0)
                RunParameters.Instance.MaxService = Int32.Parse(tbExcludedMaxChannel.Text.Trim());
            else
                RunParameters.Instance.MaxService = -1;

            if (lvRepeatPrograms.Items.Count == 0)
                RepeatExclusion.Exclusions = null;
            else
            {
                RepeatExclusion.Exclusions = new Collection<RepeatExclusion>();

                foreach (ListViewItem exclusionEntry in lvRepeatPrograms.Items)
                {
                    RepeatExclusion exclusion = new RepeatExclusion(exclusionEntry.SubItems[0].Text, exclusionEntry.SubItems[1].Text);
                    RepeatExclusion.Exclusions.Add(exclusion);
                }
            }

            if (string.IsNullOrEmpty(tbPhrasesToIgnore.Text))
                RepeatExclusion.PhrasesToIgnore = null;
            else
            {
                string[] phrases = tbPhrasesToIgnore.Text.Split(new char[] { ',' });

                RepeatExclusion.PhrasesToIgnore = new Collection<string>();

                foreach (string phrase in phrases)
                    RepeatExclusion.PhrasesToIgnore.Add(phrase);
            }

            RunParameters.Instance.LockTimeout = new TimeSpan((long)(nudSignalLockTimeout.Value * 10000000));
            RunParameters.Instance.FrequencyTimeout = new TimeSpan((long)(nudDataCollectionTimeout.Value * 10000000));
            RunParameters.Instance.Repeats = (int)nudScanRetries.Value;

            if (((Country)cboLocation.SelectedItem).Code != string.Empty)
                RunParameters.Instance.CountryCode = ((Country)cboLocation.SelectedItem).Code;
            else
                RunParameters.Instance.CountryCode = null;

            if (cboLocationArea.SelectedItem != null)
            {
                int area = ((Area)cboLocationArea.SelectedItem).Code;
                if (area != 0)
                    RunParameters.Instance.ChannelBouquet = area;
                else
                    RunParameters.Instance.ChannelBouquet = -1;
            }
            else
                RunParameters.Instance.ChannelBouquet = -1;

            if (cboLocationRegion.SelectedItem != null)
            {
                int region = ((DomainObjects.Region)cboLocationRegion.SelectedItem).Code;
                if (region != 0)
                    RunParameters.Instance.ChannelRegion = region;
                else
                    RunParameters.Instance.ChannelRegion = -1;
            }
            else
                RunParameters.Instance.ChannelRegion = -1;

            string characterSet = ((CharacterSet)cboCharacterSet.SelectedItem).Name;
            if (characterSet != string.Empty)
                RunParameters.Instance.CharacterSet = characterSet;
            else
                RunParameters.Instance.CharacterSet = null;

            if (((LanguageCode)cboInputLanguage.SelectedItem).Code != string.Empty)
                RunParameters.Instance.InputLanguage = ((LanguageCode)cboInputLanguage.SelectedItem).Code;
            else
                RunParameters.Instance.InputLanguage = null;

            if (nudEITPid.Value != 0)
                RunParameters.Instance.EITPid = (int)nudEITPid.Value;
            else
                RunParameters.Instance.EITPid = -1;

            if (nudMHW1Pid1.Value != 0)
                RunParameters.Instance.MHW1Pids = new int[] { (int)nudMHW1Pid1.Value, (int)nudMHW1Pid2.Value };
            else
                RunParameters.Instance.MHW1Pids = null;

            if (nudMHW2Pid1.Value != 0)
                RunParameters.Instance.MHW2Pids = new int[] { (int)nudMHW2Pid1.Value, (int)nudMHW2Pid2.Value, (int)nudMHW2Pid3.Value };
            else
                RunParameters.Instance.MHW2Pids = null;

            if (cbManualTime.Checked)
            {
                if (tbChangeDate.Text.Trim() != string.Empty)
                {
                    RunParameters.Instance.TimeZone = new TimeSpan((int)nudCurrentOffsetHours.Value, (int)nudCurrentOffsetMinutes.Value, 0);
                    RunParameters.Instance.NextTimeZone = new TimeSpan((int)nudNextOffsetHours.Value, (int)nudNextOffsetMinutes.Value, 0);

                    try
                    {
                        RunParameters.Instance.NextTimeZoneChange = DateTime.ParseExact(tbChangeDate.Text.Trim().Substring(0, 2) + tbChangeDate.Text.Trim().Substring(2, 2) + tbChangeDate.Text.Trim().Substring(4, 2) + " 000000", "ddMMyy HHmmss", CultureInfo.InvariantCulture) +
                            new TimeSpan((int)nudChangeHours.Value, (int)nudChangeMinutes.Value, 0);
                    }
                    catch (FormatException) { RunParameters.Instance.NextTimeZoneChange = DateTime.MaxValue; }
                    catch (ArgumentOutOfRangeException) { RunParameters.Instance.NextTimeZoneChange = DateTime.MaxValue; }

                    RunParameters.Instance.TimeZoneSet = true;
                }
                else
                    RunParameters.Instance.TimeZoneSet = false;
            }
            else
            {
                RunParameters.Instance.TimeZone = new TimeSpan();
                RunParameters.Instance.NextTimeZone = new TimeSpan();
                RunParameters.Instance.NextTimeZoneChange = new DateTime();
                RunParameters.Instance.TimeZoneSet = false;
            }

            RunParameters.Instance.DebugIDs.Clear();

            if (tbDebugIDs.Text.Trim() != string.Empty)
            {
                string[] parts = tbDebugIDs.Text.Trim().Split(new char[] { ',' });

                foreach (string part in parts)
                    RunParameters.Instance.DebugIDs.Add(part);
            }

            RunParameters.Instance.TraceIDs.Clear();

            if (tbTraceIDs.Text.Trim() != string.Empty)
            {
                string[] parts = tbTraceIDs.Text.Trim().Split(new char[] { ',' });

                foreach (string part in parts)
                    RunParameters.Instance.TraceIDs.Add(part);
            }

            if (tbDumpFile.Text.Trim() != string.Empty)
                RunParameters.Instance.TSFileName = tbDumpFile.Text.Trim();
            else
                RunParameters.Instance.TSFileName = null;
        }

        private DataState hasDataChanged()
        {
            dgServices.EndEdit();

            setRunParameters();

            if (originalData == null || newFile)
                return (DomainObjects.DataState.Changed);

            return (RunParameters.Instance.HasDataChanged(originalData));
        }

        /// <summary>
        /// Save the data to the original file.
        /// </summary>
        /// <returns>True if the file has been saved; false otherwise.</returns>
        public bool Save()
        {
            return (Save(currentFileName));
        }

        /// <summary>
        /// Save the current data set to a specified file.
        /// </summary>
        /// <param name="fileName">The name of the file to be saved.</param>
        /// <returns>True if the data has been saved; false otherwise.</returns>
        public bool Save(string fileName)
        {
            Cursor.Current = Cursors.WaitCursor;
            string message = RunParameters.Instance.Save(fileName);
            Cursor.Current = Cursors.Arrow;

            if (message == null)
            {
                MessageBox.Show("The parameters have been saved to '" + fileName + "'", " EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Information);
                newFile = false;
                originalData = RunParameters.Instance.Clone();
                currentFileName = fileName;
            }
            else
                MessageBox.Show("An error has occurred while writing the parameters." + Environment.NewLine + Environment.NewLine + message, "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        
            return (message == null);
        }
    }    
}
