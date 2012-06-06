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
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;

using DomainObjects;
using DirectShow;
using DVBServices;

namespace EPGCentre
{
    public partial class DVBLogicParametersControl : UserControl, IUpdateControl
    {
        /// <summary>
        /// Get the general window heading for the data.
        /// </summary>
        public string Heading { get { return ("EPG Centre - DVBLogic Plugin Parameters"); } }
        /// <summary>
        /// Get the default directory.
        /// </summary>
        public string DefaultDirectory { get { return (RunParameters.DataDirectory); } }
        /// <summary>
        /// Get the default output file name.
        /// </summary>
        public string DefaultFileName { get { return (getSelectedFrequency()); } }
        /// <summary>
        /// Get the save file filter.
        /// </summary>
        public string SaveFileFilter { get { return ("INI Files (*.ini)|*.ini"); } }
        /// <summary>
        /// Get the save file title.
        /// </summary>
        public string SaveFileTitle { get { return ("Save EPG DVBLogic Plugin Parameter File"); } }
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

        private RunParameters runParameters;
        
        private string currentFileName;
        private RunParameters originalData;
        private bool newFile;

        private const int timeoutLock = 10;
        private const int timeoutCollection = 300;
        private const int timeoutRetries = 5;

        private BackgroundWorker workerScanStations;
        private AutoResetEvent resetEvent = new AutoResetEvent(false);
        private TuningFrequency scanningFrequency;
        private ChannelScanParameters scanParameters;

        private BindingList<TVStation> bindingList;

        private string sortedColumnName;
        private string sortedKeyName;
        private bool sortedAscending;

        private bool useSignalPresent;
        private bool switchAfterPlay;
        private bool repeatDiseqc;

        public DVBLogicParametersControl()
        {
            InitializeComponent();

            Satellite.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "dvbs")) + Path.DirectorySeparatorChar);
            TerrestrialProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "dvbt")) + Path.DirectorySeparatorChar);
            CableProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "dvbc")) + Path.DirectorySeparatorChar);
            AtscProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "atsc")) + Path.DirectorySeparatorChar);
            ClearQamProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "clear QAM")) + Path.DirectorySeparatorChar);
        }

        public void Process()
        {
            runParameters = new RunParameters(ParameterSet.Plugin);
            runParameters.OutputFileName = string.Empty;

            TVStation.StationCollection.Clear();
            
            currentFileName = null;
            newFile = true;
            start();
        }

        public void Process(string fileName, bool newFile)
        {
            currentFileName = fileName;
            this.newFile = newFile;

            runParameters = new RunParameters(ParameterSet.Plugin);

            TVStation.StationCollection.Clear();
            
            Cursor.Current = Cursors.WaitCursor;
            int reply = runParameters.Process(fileName);
            Cursor.Current = Cursors.Arrow;

            if (!runParameters.OutputFileSet)
                runParameters.OutputFileName = string.Empty;

            if (reply != 0)
            {
                MessageBox.Show("The parameter file could not be completely processed due to a format error." + Environment.NewLine + Environment.NewLine +
                    "Some parameters may have default values.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            originalData = runParameters.Clone();
            start();
        }

        private void start()
        {
            tbcParameters.SelectedTab = tbcParameters.TabPages[0];

            initializeSatelliteGroup();
            initializeTerrestrialGroup();
            initializeCableGroup();
            initializeAtscGroup();
            initializeClearQamGroup();

            if (!runParameters.OutputFileSet)
            {
                if (!runParameters.Options.Contains("WMCIMPORT"))
                {
                    rbPassToTVSource.Checked = true;
                    rbImportToWMC.Checked = false;
                    rbCreateXMLTVFile.Checked = false;
                }
                else
                {
                    if (Environment.OSVersion.Version.Major != 5)
                    {
                        rbImportToWMC.Enabled = true;
                        rbImportToWMC.Checked = runParameters.Options.Contains("WMCIMPORT");
                        cbAutoMapEPG.Enabled = true;
                        cbAutoMapEPG.Checked = runParameters.Options.Contains("AUTOMAPEPG");
                        cbAllSeries.Enabled = true;
                        cbAllSeries.Checked = runParameters.Options.Contains("ALLSERIES");
                    }
                }
            }
            else
            {
                rbPassToTVSource.Checked = false;

                rbImportToWMC.Checked = false;
                cbAutoMapEPG.Enabled = false;
                cbAutoMapEPG.Checked = false;
                cbAllSeries.Enabled = false;
                cbAllSeries.Checked = false;

                rbCreateXMLTVFile.Checked = true;
                txtOutputFile.Text = runParameters.OutputFileName;
            }

            if (Environment.OSVersion.Version.Major == 5)
            {
                rbImportToWMC.Enabled = false;
                rbImportToWMC.Checked = false;
                cbAutoMapEPG.Enabled = false;
                cbAutoMapEPG.Checked = false;
                cbAllSeries.Enabled = false;
                cbAllSeries.Checked = false;
            }

            cbAllowBreaks.Checked = runParameters.Options.Contains("ACCEPTBREAKS");
            cbRoundTime.Checked = runParameters.Options.Contains("ROUNDTIME");
            cbUseBSEPG.Checked = runParameters.Options.Contains("USEBSEPG");
            cbUseChannelID.Checked = runParameters.Options.Contains("USECHANNELID");
            cbUseLCN.Checked = runParameters.Options.Contains("USELCN");
            cbUseRawCRID.Checked = runParameters.Options.Contains("USERAWCRID");
            cbUseNumericCRID.Checked = runParameters.Options.Contains("USENUMERICCRID");
            cbUseDescAsCategory.Checked = runParameters.Options.Contains("USEDESCASCATEGORY");
            cbUseStationLogos.Checked = runParameters.Options.Contains("USEIMAGE");
            cbCreateBladeRunnerFile.Checked = runParameters.Options.Contains("CREATEBRCHANNELS");
            cbCheckForRepeats.Checked = runParameters.Options.Contains("CHECKFORREPEATS");
            cbRemoveExtractedData.Checked = runParameters.Options.Contains("NOREMOVEDATA");
            cbCreateSameData.Checked = runParameters.Options.Contains("DUPLICATESAMECHANNELS");

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

            if (runParameters.MaxService != -1)
                tbExcludedMaxChannel.Text = runParameters.MaxService.ToString();
            else
                tbExcludedMaxChannel.Text = string.Empty;

            cbUseContentSubtype.Checked = runParameters.Options.Contains("USECONTENTSUBTYPE");
            cbUseFreeSatTables.Checked = runParameters.Options.Contains("USEFREESATTABLES");
            cbCustomCategoriesOverride.Checked = runParameters.Options.Contains("CUSTOMCATEGORYOVERRIDE");
            cbStoreStationInfo.Checked = runParameters.Options.Contains("STORESTATIONINFO");
            cbUseStoredStationInfo.Checked = runParameters.Options.Contains("USESTOREDSTATIONINFO");
            cbProcessAllStations.Checked = runParameters.Options.Contains("PROCESSALLSTATIONS");
            
            nudDataCollectionTimeout.Value = (decimal)runParameters.FrequencyTimeout.TotalSeconds;
            nudScanRetries.Value = (decimal)runParameters.Repeats;

            cboLocation.DataSource = Country.Load();
            if (runParameters.CountryCode != null)
            {
                Country country = Country.FindCountryCode(runParameters.CountryCode, (Collection<Country>)cboLocation.DataSource);
                if (country != null)
                {
                    cboLocation.SelectedItem = country;
                    cboLocation.Text = country.Name;

                    if (runParameters.ChannelBouquet != -1)
                    {
                        Area area = null;

                        if (runParameters.ChannelRegion == -1)
                            area = country.FindArea(runParameters.ChannelBouquet);
                        else
                            area = country.FindArea(runParameters.ChannelBouquet, runParameters.ChannelRegion);

                        if (area != null)
                        {
                            cboLocationArea.SelectedItem = area;
                            cboLocationArea.Text = area.Name;

                            if (runParameters.ChannelRegion != -1)
                            {
                                DomainObjects.Region region = area.FindRegion(runParameters.ChannelRegion);
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
            if (runParameters.CharacterSet != null)
                cboCharacterSet.SelectedItem = CharacterSet.FindCharacterSet(runParameters.CharacterSet);
            else
                cboCharacterSet.SelectedItem = cboCharacterSet.Items[0];

            cboInputLanguage.DataSource = LanguageCode.LanguageCodes;
            if (runParameters.InputLanguage != null)
                cboInputLanguage.SelectedItem = LanguageCode.FindLanguageCode(runParameters.InputLanguage);
            else
                cboInputLanguage.SelectedItem = cboInputLanguage.Items[0];

            if (runParameters.EITPid != -1)
                nudEITPid.Value = runParameters.EITPid;
            else
                nudEITPid.Value = 0;

            if (runParameters.MHW1Pids != null)
            {
                nudMHW1Pid1.Value = runParameters.MHW1Pids[0];
                nudMHW1Pid2.Value = runParameters.MHW1Pids[1];
            }
            else
            {
                nudMHW1Pid1.Value = 0;
                nudMHW1Pid2.Value = 0;
            }

            if (runParameters.MHW2Pids != null)
            {
                nudMHW2Pid1.Value = runParameters.MHW2Pids[0];
                nudMHW2Pid2.Value = runParameters.MHW2Pids[1];
                nudMHW2Pid3.Value = runParameters.MHW2Pids[2];
            }
            else
            {
                nudMHW2Pid1.Value = 0;
                nudMHW2Pid2.Value = 0;
                nudMHW2Pid3.Value = 0;
            }

            cbManualTime.Checked = runParameters.TimeZoneSet;
            gpTimeAdjustments.Enabled = runParameters.TimeZoneSet;

            if (cbManualTime.Checked)
            {
                nudCurrentOffsetHours.Value = runParameters.TimeZone.Hours;
                nudCurrentOffsetMinutes.Value = runParameters.TimeZone.Minutes;
                nudNextOffsetHours.Value = runParameters.NextTimeZone.Hours;
                nudNextOffsetMinutes.Value = runParameters.NextTimeZone.Minutes;
                tbChangeDate.Text = runParameters.NextTimeZoneChange.Date.ToString("ddMMyy");
                nudChangeHours.Value = runParameters.NextTimeZoneChange.Hour;
                nudChangeMinutes.Value = runParameters.NextTimeZoneChange.Minute;
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
            foreach (string debugID in runParameters.DebugIDs)
            {
                if (tbDebugIDs.Text.Length != 0)
                    tbDebugIDs.Text = tbDebugIDs.Text + "," + debugID;
                else
                    tbDebugIDs.Text = tbDebugIDs.Text + debugID;
            }

            tbTraceIDs.Text = string.Empty;
            foreach (string traceID in runParameters.TraceIDs)
            {
                if (tbTraceIDs.Text.Length != 0)
                    tbTraceIDs.Text = tbTraceIDs.Text + "," + traceID;
                else
                    tbTraceIDs.Text = tbTraceIDs.Text + traceID;
            }
        }

        private void initializeSatelliteGroup()
        {
            if (cboDVBSCollectionType.Items.Count == 0)
                cboDVBSCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));

            cboSatellite.DataSource = Satellite.Providers;

            if (runParameters.PluginFrequency != null)
            {
                SatelliteFrequency satelliteFrequency = runParameters.PluginFrequency as SatelliteFrequency;
                if (satelliteFrequency != null)
                {
                    cbEnableSatellite.Checked = true;
                    cboSatellite.Text = satelliteFrequency.Provider.Name;
                    cboDVBSScanningFrequency.Text = runParameters.PluginFrequency.ToString();
                    cboDVBSCollectionType.Text = runParameters.PluginFrequency.CollectionType.ToString();                    
                }
            }
            else
            {
                cboSatellite.SelectedItem = null;
                cboSatellite.SelectedItem = cboSatellite.Items[0];
            }

            cbEnableSatellite.Enabled = (currentFileName == null);
            cboSatellite.Enabled = (currentFileName == null);
            cboDVBSScanningFrequency.Enabled = (currentFileName == null);
        }

        private void initializeTerrestrialGroup()
        {
            if (cboDVBTCollectionType.Items.Count == 0)
                cboDVBTCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));

            cboCountry.DataSource = TerrestrialProvider.Countries;

            if (runParameters.PluginFrequency != null)
            {
                TerrestrialFrequency terrestrialFrequency = runParameters.PluginFrequency as TerrestrialFrequency;
                if (terrestrialFrequency != null)
                {
                    cbEnableTerrestrial.Checked = true;
                    TerrestrialProvider provider = TerrestrialProvider.FindProvider(terrestrialFrequency.Provider.Name);
                    if (provider != null)
                    {
                        cboCountry.Text = provider.Country.Name;
                        cboArea.Text = provider.Area.Name;
                        TuningFrequency providerFrequency = provider.FindFrequency(terrestrialFrequency.Frequency);
                        if (providerFrequency != null)
                        {
                            cboDVBTScanningFrequency.SelectedItem = providerFrequency;
                            cboDVBTCollectionType.Text = terrestrialFrequency.CollectionType.ToString();
                        }
                        else
                            cboDVBTScanningFrequency.SelectedItem = provider.Frequencies[0];
                    }
                    else
                        cboCountry.SelectedItem = cboCountry.Items[0];
                }
            }
            else
                cboCountry.SelectedItem = cboCountry.Items[0];

            cbEnableTerrestrial.Enabled = (currentFileName == null);
            cboCountry.Enabled = (currentFileName == null);
            cboArea.Enabled = (currentFileName == null);
            cboDVBTScanningFrequency.Enabled = (currentFileName == null);
        }

        private void initializeCableGroup()
        {
            if (cboCableCollectionType.Items.Count == 0)
                cboCableCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));

            cboCable.DataSource = CableProvider.Providers;

            if (runParameters.PluginFrequency != null)
            {
                CableFrequency cableFrequency = runParameters.PluginFrequency as CableFrequency;
                if (cableFrequency != null)
                {
                    cbEnableCable.Checked = true;
                    CableProvider provider = CableProvider.FindProvider(cableFrequency.Provider.Name);
                    if (provider != null)
                    {
                        cboCable.SelectedItem = provider;
                        TuningFrequency providerFrequency = provider.FindFrequency(cableFrequency.Frequency);
                        if (providerFrequency != null)
                        {
                            cboCableScanningFrequency.SelectedItem = providerFrequency;
                            cboCableCollectionType.Text = cableFrequency.CollectionType.ToString();
                        }
                        else
                            cboCableScanningFrequency.SelectedItem = provider.Frequencies[0];
                    }
                    else
                        cboCable.SelectedItem = cboCable.Items[0];
                }
            }
            else
                cboCable.SelectedItem = cboCable.Items[0];

            cbEnableCable.Enabled = (currentFileName == null);
            cboCable.Enabled = (currentFileName == null);
            cboCableScanningFrequency.Enabled = (currentFileName == null);
        }

        private void initializeAtscGroup()
        {
            if (cboAtscCollectionType.Items.Count == 0)
                cboAtscCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));

            cboAtsc.DataSource = AtscProvider.Providers;

            if (runParameters.PluginFrequency != null)
            {
                AtscFrequency atscFrequency = runParameters.PluginFrequency as AtscFrequency;
                if (atscFrequency != null)
                {
                    cbEnableAtsc.Checked = true;
                    AtscProvider provider = AtscProvider.FindProvider(atscFrequency.Provider.Name);
                    if (provider != null)
                    {
                        cboAtsc.SelectedItem = provider;
                        TuningFrequency providerFrequency = provider.FindFrequency(atscFrequency.Frequency);
                        if (providerFrequency != null)
                        {
                            cboAtscScanningFrequency.SelectedItem = providerFrequency;
                            cboAtscCollectionType.Text = atscFrequency.CollectionType.ToString();
                        }
                        else
                            cboAtscScanningFrequency.SelectedItem = provider.Frequencies[0];
                    }
                    else
                        cboAtsc.SelectedItem = cboAtsc.Items[0];
                }
            }
            else
                cboAtsc.SelectedItem = cboAtsc.Items[0];

            cbEnableAtsc.Enabled = (currentFileName == null);
            cboAtsc.Enabled = (currentFileName == null);
            cboAtscScanningFrequency.Enabled = (currentFileName == null);
        }

        private void initializeClearQamGroup()
        {
            if (cboClearQamCollectionType.Items.Count == 0)
                cboClearQamCollectionType.DataSource = System.Enum.GetValues(typeof(CollectionType));

            cboClearQam.DataSource = ClearQamProvider.Providers;

            if (runParameters.PluginFrequency != null)
            {
                ClearQamFrequency clearQamFrequency = runParameters.PluginFrequency as ClearQamFrequency;
                if (clearQamFrequency != null)
                {
                    cbEnableClearQam.Checked = true;
                    ClearQamProvider provider = ClearQamProvider.FindProvider(clearQamFrequency.Provider.Name);
                    if (provider != null)
                    {
                        cboClearQam.SelectedItem = provider;
                        TuningFrequency providerFrequency = provider.FindFrequency(clearQamFrequency.Frequency);
                        if (providerFrequency != null)
                        {
                            cboClearQamScanningFrequency.SelectedItem = providerFrequency;
                            cboClearQamCollectionType.Text = clearQamFrequency.CollectionType.ToString();
                        }
                        else
                            cboClearQamScanningFrequency.SelectedItem = provider.Frequencies[0];
                    }
                    else
                        cboClearQam.SelectedItem = cboClearQam.Items[0];
                }
            }
            else
                cboClearQam.SelectedItem = cboClearQam.Items[0];

            cbEnableClearQam.Enabled = (currentFileName == null);
            cboClearQam.Enabled = (currentFileName == null);
            cboClearQamScanningFrequency.Enabled = (currentFileName == null);
        }

        private void cbEnableSatellite_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableSatellite.Checked)
            {
                gpSatellite.Enabled = true;
                cbEnableTerrestrial.Checked = false;
                gpTerrestrial.Enabled = false;
                cbEnableCable.Checked = false;
                gpCable.Enabled = false;
                cbEnableAtsc.Checked = false;
                gpAtsc.Enabled = false;
                cbEnableClearQam.Checked = false;
                gpClearQam.Enabled = false;
            }
            else
                gpSatellite.Enabled = false;
        }

        private void cboSatellite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSatellite.SelectedItem != null)
            {
                cboDVBSScanningFrequency.DataSource = ((Satellite)cboSatellite.SelectedItem).Frequencies;
                cboDVBSScanningFrequency.SelectedItem = cboDVBSScanningFrequency.Items[0];
            }
        }

        private void cboDVBSScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboDVBSScanningFrequency.SelectedItem as TuningFrequency;
            cboDVBSCollectionType.Text = tuningFrequency.CollectionType.ToString();
        }

        private void cbEnableTerrestrial_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableTerrestrial.Checked)
            {
                cbEnableSatellite.Checked = false;
                gpSatellite.Enabled = false;
                gpTerrestrial.Enabled = true;
                cbEnableCable.Checked = false;
                gpCable.Enabled = false;
                cbEnableAtsc.Checked = false;
                gpAtsc.Enabled = false;
                cbEnableClearQam.Checked = false;
                gpClearQam.Enabled = false;
            }
            else
                gpTerrestrial.Enabled = false;
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
            cboDVBTScanningFrequency.SelectedItem = cboDVBTScanningFrequency.Items[0];
        }

        private void cboDVBTScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboDVBTScanningFrequency.SelectedItem as TuningFrequency;
            cboDVBTCollectionType.Text = tuningFrequency.CollectionType.ToString();
        }

        private void cbEnableCable_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableCable.Checked)
            {
                cbEnableSatellite.Checked = false;
                gpSatellite.Enabled = false;
                cbEnableTerrestrial.Checked = false;
                gpTerrestrial.Enabled = false;                
                gpCable.Enabled = true;
                cbEnableAtsc.Checked = false;
                gpAtsc.Enabled = false;
                cbEnableClearQam.Checked = false;
                gpClearQam.Enabled = false;
            }
            else
                gpCable.Enabled = false;
        }

        private void cboCable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCable.SelectedItem != null)
            {
                cboCableScanningFrequency.DataSource = ((CableProvider)cboCable.SelectedItem).Frequencies;
                cboCableScanningFrequency.SelectedItem = cboCableScanningFrequency.Items[0];
            }
        }

        private void cboCableScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboCableScanningFrequency.SelectedItem as TuningFrequency;
            cboCableCollectionType.Text = tuningFrequency.CollectionType.ToString();
        }

        private void cbEnableAtsc_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableAtsc.Checked)
            {
                cbEnableSatellite.Checked = false;
                gpSatellite.Enabled = false;
                cbEnableTerrestrial.Checked = false;
                gpTerrestrial.Enabled = false;
                cbEnableCable.Checked = false;
                gpCable.Enabled = false;
                gpAtsc.Enabled = true;
                cbEnableClearQam.Checked = false;
                gpClearQam.Enabled = false;                
            }
            else
                gpAtsc.Enabled = false;
        }

        private void cboAtsc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboAtsc.SelectedItem != null)
            {
                cboAtscScanningFrequency.DataSource = ((AtscProvider)cboAtsc.SelectedItem).Frequencies;
                cboAtscScanningFrequency.SelectedItem = cboAtscScanningFrequency.Items[0];
            }
        }

        private void cboAtscScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboAtscScanningFrequency.SelectedItem as TuningFrequency;
            cboAtscCollectionType.Text = tuningFrequency.CollectionType.ToString();
        }

        private void cbEnableClearQam_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableClearQam.Checked)
            {
                cbEnableSatellite.Checked = false;
                gpSatellite.Enabled = false;
                cbEnableTerrestrial.Checked = false;
                gpTerrestrial.Enabled = false;
                cbEnableCable.Checked = false;
                gpCable.Enabled = false;
                cbEnableAtsc.Checked = false;
                gpAtsc.Enabled = false;
                gpClearQam.Enabled = true;
            }
            else
                gpClearQam.Enabled = false;
        }

        private void cboClearQam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboClearQam.SelectedItem != null)
            {
                cboClearQamScanningFrequency.DataSource = ((ClearQamProvider)cboClearQam.SelectedItem).Frequencies;
                cboClearQamScanningFrequency.SelectedItem = cboClearQamScanningFrequency.Items[0];
            }
        }

        private void cboClearQamScanningFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            TuningFrequency tuningFrequency = cboClearQamScanningFrequency.SelectedItem as TuningFrequency;
            cboClearQamCollectionType.Text = tuningFrequency.CollectionType.ToString();
        }

        private void rbPassToTVSource_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPassToTVSource.Checked)
            {
                txtOutputFile.Text = string.Empty;
                txtOutputFile.Enabled = false;
                btnBrowse.Enabled = false;
                cbCreateBladeRunnerFile.Checked = false;
                cbCreateBladeRunnerFile.Enabled = false;
                cbUseBSEPG.Checked = false;
                cbUseBSEPG.Enabled = false;
                cbUseChannelID.Checked = false;
                cbUseChannelID.Enabled = false;
                cbUseLCN.Checked = false;
                cbUseLCN.Enabled = false;
                cbUseNumericCRID.Checked = false;
                cbUseNumericCRID.Enabled = false;
                cbUseRawCRID.Checked = false;
                cbUseRawCRID.Enabled = false;
                cbUseStationLogos.Checked = false;
                cbUseStationLogos.Enabled = false;                
            }
        }

        private void rbImportToWMC_CheckedChanged(object sender, EventArgs e)
        {
            if (rbImportToWMC.Checked)
            {
                txtOutputFile.Text = string.Empty;
                txtOutputFile.Enabled = false;
                btnBrowse.Enabled = false;
                cbCreateBladeRunnerFile.Checked = false;
                cbCreateBladeRunnerFile.Enabled = false;
                cbUseBSEPG.Checked = false;
                cbUseBSEPG.Enabled = false;
                cbUseChannelID.Checked = false;
                cbUseChannelID.Enabled = false;
                cbUseLCN.Checked = false;
                cbUseLCN.Enabled = false;
                cbUseNumericCRID.Checked = false;
                cbUseNumericCRID.Enabled = false;
                cbUseRawCRID.Checked = false;
                cbUseRawCRID.Enabled = false;
                cbUseStationLogos.Checked = false;
                cbUseStationLogos.Enabled = false;
                cbAutoMapEPG.Enabled = true;
                cbAllSeries.Enabled = true;   
            }
            else
            {
                cbAutoMapEPG.Checked = false;
                cbAutoMapEPG.Enabled = false;
                cbAllSeries.Checked = false;
                cbAllSeries.Enabled = false;  
            }
        }

        private void rbCreateXMLTVFile_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCreateXMLTVFile.Checked)
            {
                txtOutputFile.Enabled = true;
                btnBrowse.Enabled = true;
                cbCreateBladeRunnerFile.Enabled = true;
                cbUseBSEPG.Enabled = true;
                cbUseChannelID.Enabled = true;
                cbUseLCN.Enabled = true;
                cbUseNumericCRID.Enabled = true;
                cbUseRawCRID.Enabled = true;
                cbUseStationLogos.Enabled = true;               
            }
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
                textEdit.KeyPress += new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
            }            
        }

        private void textEdit_KeyPressAlphaNumeric(object sender, KeyPressEventArgs e)
        {
            Regex alphaNumericPattern = new Regex(@"[a-zA-Z0-9!&*()-+?\s\b]");
            e.Handled = !alphaNumericPattern.IsMatch(e.KeyChar.ToString());
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
            if (e.TabPage == tabChannels && pbarChannels.Enabled)
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
                pbarChannels.Visible = false;
                pbarChannels.Enabled = false;

                btPlusScan.Text = "Start Scan";
                lblPlusScanning.Visible = false;
                pbarPlusScan.Enabled = false;
                pbarPlusScan.Visible = false;

                MainWindow.ChangeMenuItemAvailability(true);

                return;
            }

            this.Hide();

            Logger.Instance.Write("Scan requested");
            scanningFrequency = getPluginFrequency();

            scanParameters = new ChannelScanParameters(scanningFrequency);
            DialogResult result = scanParameters.ShowDialog();

            this.Show();

            if (result == DialogResult.Cancel)
                return;

            bool tunerReply = checkTunerSelection(scanningFrequency, scanParameters.Tuners);
            if (!tunerReply)
            {
                MessageBox.Show("Frequency " + scanningFrequency.ToString() + " cannnot be scanned as a suitable tuner has not been selected.",
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (scanningFrequency.TunerType == TunerType.Satellite)
                ((SatelliteFrequency)scanningFrequency).SatelliteDish = scanParameters.SatelliteDish;
            
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

            useSignalPresent = scanParameters.UseSignalPresent;
            switchAfterPlay = scanParameters.SwitchAfterPlay;
            repeatDiseqc = scanParameters.RepeatDiseqc;

            workerScanStations = new BackgroundWorker();
            workerScanStations.WorkerReportsProgress = true;
            workerScanStations.WorkerSupportsCancellation = true;
            workerScanStations.DoWork += new DoWorkEventHandler(scanStationsDoScan);
            workerScanStations.RunWorkerCompleted += new RunWorkerCompletedEventHandler(scanStationsRunWorkerCompleted);
            workerScanStations.ProgressChanged += new ProgressChangedEventHandler(scanStationsProgressChanged);
            workerScanStations.RunWorkerAsync(scanningFrequency);
        }

        private bool checkTunerSelection(TuningFrequency tuningFrequency, Collection<Tuner> tuners)
        {
            if (tuners.Count == 0)
                return (true);

            foreach(Tuner tuner in tuners)
            {
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
                    default:
                        return (false);
                }
            }

            return (false);
        }

        private void scanStationsProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblScanning.Text = "Scanning " + scanningFrequency;
            lblPlusScanning.Text = "Scanning " + scanningFrequency;
        }

        private void scanStationsDoScan(object sender, DoWorkEventArgs e)
        {
            TuningFrequency frequency = e.Argument as TuningFrequency;

            Collection<int> tuners = new Collection<int>();
            foreach (Tuner tuner in scanParameters.Tuners)
                tuners.Add(Tuner.TunerCollection.IndexOf(tuner) + 1);

            Logger.Instance.Write("Scanning frequency " + frequency.ToString() + " on " + frequency.TunerType);
            (sender as BackgroundWorker).ReportProgress(0);

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
                        tuningSpec = new TuningSpec((CableFrequency)frequency);
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
                            tuningSpec = new TuningSpec((AtscFrequency)frequency);
                        }
                        else
                        {
                            ClearQamFrequency clearQamFrequency = frequency as ClearQamFrequency;
                            if (clearQamFrequency != null)
                            {
                                tunerNodeType = TunerNodeType.Cable;
                                tuningSpec = new TuningSpec((ClearQamFrequency)frequency);
                            }
                            else
                                throw (new InvalidOperationException("Tuning frequency not recognized"));
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

                    dgServices.Invoke(new ShowMessage(showMessage), "No tuner able to tune frequency " + frequency.ToString(),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    finished = true;
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

            e.Cancel = true;
            resetEvent.Set();
        }

        private DialogResult showMessage(string message, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            lblScanning.Visible = false;
            pbarChannels.Enabled = false;
            pbarChannels.Visible = false;

            lblPlusScanning.Visible = false;
            pbarPlusScan.Enabled = false;
            pbarPlusScan.Visible = false;

            DialogResult result = MessageBox.Show(message, "EPG Centre", buttons, icon);
            if (result == DialogResult.Yes)
            {
                lblScanning.Visible = true;
                pbarChannels.Enabled = true;
                pbarChannels.Visible = true;

                lblPlusScanning.Visible = true;
                pbarPlusScan.Enabled = true;
                pbarPlusScan.Visible = true;
            }

            return (result);
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
            pbarChannels.Enabled = false;
            pbarChannels.Visible = false;

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

                ChannelFilterEntry oldFilter = oldItem.Tag as ChannelFilterEntry;

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
            if (!cbEnableSatellite.Checked && 
                !cbEnableTerrestrial.Checked && 
                !cbEnableCable.Checked &&
                !cbEnableAtsc.Checked &&
                !cbEnableClearQam.Checked)
            {
                showErrorMessage("No frequency selected.");
                return (false);
            }

            if (rbCreateXMLTVFile.Checked)
            {
                if (txtOutputFile.Text.Trim() != string.Empty)
                {
                    DialogResult result = MessageBox.Show("If an XMLTV file is created it must be manually imported into TV Server." +
                        Environment.NewLine + Environment.NewLine + "Is that correct?",
                        "EPG Centre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                        return (false);
                }
                else
                {
                    MessageBox.Show("No output path specified.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return (false);
                }
            }

            if (getPluginFrequency().CollectionType == CollectionType.OpenTV)
            {
                {
                    if (((Country)cboLocation.SelectedItem).Code == string.Empty)
                    {
                        showErrorMessage("A country must be selected for OpenTV collections.");
                        return (false);
                    }
                }
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
                        DateTime.ParseExact(tbChangeDate.Text.Trim().Substring(0, 2) + tbChangeDate.Text.Trim().Substring(2, 2) + tbChangeDate.Text.Trim().Substring(4, 2), "hhMMyy", CultureInfo.InvariantCulture);
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

        private void showErrorMessage(string message)
        {
            MessageBox.Show(message, "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void setRunParameters()
        {
            runParameters.PluginFrequency = getPluginFrequency();            

            runParameters.Options.Clear();

            if (rbImportToWMC.Checked)
                runParameters.Options.Add("WMCIMPORT");
            if (cbAutoMapEPG.Checked)
                runParameters.Options.Add("AUTOMAPEPG");            

            runParameters.OutputFileName = txtOutputFile.Text;

            if (cbAllowBreaks.Checked)
                runParameters.Options.Add("ACCEPTBREAKS");
            if (cbRoundTime.Checked)
                runParameters.Options.Add("ROUNDTIME");
            if (cbUseBSEPG.Checked)
                runParameters.Options.Add("USEBSEPG");
            if (cbUseChannelID.Checked)
                runParameters.Options.Add("USECHANNELID");
            if (cbUseLCN.Checked)
                runParameters.Options.Add("USELCN");
            if (cbUseRawCRID.Checked)
                runParameters.Options.Add("USERAWCRID");
            if (cbUseNumericCRID.Checked)
                runParameters.Options.Add("USENUMERICCRID");
            if (cbUseDescAsCategory.Checked)
                runParameters.Options.Add("USEDESCASCATEGORY");
            if (cbUseStationLogos.Checked)
                runParameters.Options.Add("USEIMAGE");
            if (cbCreateBladeRunnerFile.Checked)
                runParameters.Options.Add("CREATEBRCHANNELS");
            if (cbCheckForRepeats.Checked)
                runParameters.Options.Add("CHECKFORREPEATS");
            if (cbRemoveExtractedData.Checked)
                runParameters.Options.Add("NOREMOVEDATA");
            if (cbCreateSameData.Checked)
                runParameters.Options.Add("DUPLICATESAMECHANNELS");

            if (cbUseContentSubtype.Checked)
                runParameters.Options.Add("USECONTENTSUBTYPE");
            if (cbUseFreeSatTables.Checked)
                runParameters.Options.Add("USEFREESATTABLES");
            if (cbCustomCategoriesOverride.Checked)
                runParameters.Options.Add("CUSTOMCATEGORYOVERRIDE");
            if (cbStoreStationInfo.Checked)
                runParameters.Options.Add("STORESTATIONINFO");
            if (cbUseStoredStationInfo.Checked)
                runParameters.Options.Add("USESTOREDSTATIONINFO");
            if (cbProcessAllStations.Checked)
                runParameters.Options.Add("PROCESSALLSTATIONS");

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
                runParameters.MaxService = Int32.Parse(tbExcludedMaxChannel.Text.Trim());
            else
                runParameters.MaxService = -1;

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

            runParameters.FrequencyTimeout = new TimeSpan((long)(nudDataCollectionTimeout.Value * 10000000));
            runParameters.Repeats = (int)nudScanRetries.Value;

            if (((Country)cboLocation.SelectedItem).Code != string.Empty)
                runParameters.CountryCode = ((Country)cboLocation.SelectedItem).Code;
            else
                runParameters.CountryCode = null;

            if (cboLocationArea.SelectedItem != null)
            {
                int area = ((Area)cboLocationArea.SelectedItem).Code;
                if (area != 0)
                    runParameters.ChannelBouquet = area;
                else
                    runParameters.ChannelBouquet = -1;
            }
            else
                runParameters.ChannelBouquet = -1;

            if (cboLocationRegion.SelectedItem != null)
            {
                int region = ((DomainObjects.Region)cboLocationRegion.SelectedItem).Code;
                if (region != 0)
                    runParameters.ChannelRegion = region;
                else
                    runParameters.ChannelRegion = -1;
            }
            else
                runParameters.ChannelRegion = -1;

            string characterSet = ((CharacterSet)cboCharacterSet.SelectedItem).Name;
            if (characterSet != string.Empty)
                runParameters.CharacterSet = characterSet;
            else
                runParameters.CharacterSet = null;

            if (((LanguageCode)cboInputLanguage.SelectedItem).Code != string.Empty)
                runParameters.InputLanguage = ((LanguageCode)cboInputLanguage.SelectedItem).Code;
            else
                runParameters.InputLanguage = null;

            if (nudEITPid.Value != 0)
                runParameters.EITPid = (int)nudEITPid.Value;
            else
                runParameters.EITPid = -1;

            if (nudMHW1Pid1.Value != 0)
                runParameters.MHW1Pids = new int[] { (int)nudMHW1Pid1.Value, (int)nudMHW1Pid2.Value };
            else
                runParameters.MHW1Pids = null;

            if (nudMHW2Pid1.Value != 0)
                runParameters.MHW2Pids = new int[] { (int)nudMHW2Pid1.Value, (int)nudMHW2Pid2.Value, (int)nudMHW2Pid3.Value };
            else
                runParameters.MHW2Pids = null;

            if (cbManualTime.Checked)
            {
                if (tbChangeDate.Text.Trim() != string.Empty)
                {
                    runParameters.TimeZone = new TimeSpan((int)nudCurrentOffsetHours.Value, (int)nudCurrentOffsetMinutes.Value, 0);
                    runParameters.NextTimeZone = new TimeSpan((int)nudNextOffsetHours.Value, (int)nudNextOffsetMinutes.Value, 0);

                    try
                    {
                        runParameters.NextTimeZoneChange = DateTime.ParseExact(tbChangeDate.Text.Trim().Substring(0, 2) + tbChangeDate.Text.Trim().Substring(2, 2) + tbChangeDate.Text.Trim().Substring(4, 2) + " 000000", "ddMMyy HHmmss", CultureInfo.InvariantCulture) +
                            new TimeSpan((int)nudChangeHours.Value, (int)nudChangeMinutes.Value, 0);
                    }
                    catch (FormatException) { runParameters.NextTimeZoneChange = DateTime.MaxValue; }
                    catch (ArgumentOutOfRangeException) { runParameters.NextTimeZoneChange = DateTime.MaxValue; }

                    runParameters.TimeZoneSet = true;
                }
                else
                    runParameters.TimeZoneSet = false;
            }
            else
            {
                runParameters.TimeZone = new TimeSpan();
                runParameters.NextTimeZone = new TimeSpan();
                runParameters.NextTimeZoneChange = new DateTime();
                runParameters.TimeZoneSet = false;
            }

            runParameters.DebugIDs.Clear();

            if (tbDebugIDs.Text.Trim() != string.Empty)
            {
                string[] parts = tbDebugIDs.Text.Trim().Split(new char[] { ',' });

                foreach (string part in parts)
                    runParameters.DebugIDs.Add(part);
            }

            runParameters.TraceIDs.Clear();

            if (tbTraceIDs.Text.Trim() != string.Empty)
            {
                string[] parts = tbTraceIDs.Text.Trim().Split(new char[] { ',' });

                foreach (string part in parts)
                    runParameters.TraceIDs.Add(part);
            }
        }

        private TuningFrequency getPluginFrequency()
        {
            TuningFrequency selectedFrequency = null;

            if (cbEnableSatellite.Checked)
            {
                selectedFrequency = cboDVBSScanningFrequency.SelectedItem as SatelliteFrequency;
                selectedFrequency.CollectionType = (CollectionType)cboDVBSCollectionType.SelectedItem;
            }
            else
            {
                if (cbEnableTerrestrial.Checked)
                {
                    selectedFrequency = cboDVBTScanningFrequency.SelectedItem as TerrestrialFrequency;
                    selectedFrequency.CollectionType = (CollectionType)cboDVBTCollectionType.SelectedItem;
                }
                else
                {
                    if (cbEnableCable.Checked)
                    {
                        selectedFrequency = cboCableScanningFrequency.SelectedItem as CableFrequency;
                        selectedFrequency.CollectionType = (CollectionType)cboCableCollectionType.SelectedItem;
                    }
                    else
                    {
                        if (cbEnableAtsc.Checked)
                        {
                            selectedFrequency = cboAtscScanningFrequency.SelectedItem as AtscFrequency;
                            selectedFrequency.CollectionType = (CollectionType)cboAtscCollectionType.SelectedItem;
                        }
                        else
                        {
                            selectedFrequency = cboClearQamScanningFrequency.SelectedItem as ClearQamFrequency;
                            selectedFrequency.CollectionType = (CollectionType)cboClearQamCollectionType.SelectedItem;
                        }
                    }
                }
            }

            return(selectedFrequency);
        }

        private DataState hasDataChanged()
        {
            setRunParameters();

            if (originalData == null || newFile)
                return (DomainObjects.DataState.Changed);

            return (runParameters.HasDataChanged(originalData));
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
            FileInfo fileInfo = new FileInfo(fileName);
            if (!fileInfo.DirectoryName.ToUpper().EndsWith(Path.DirectorySeparatorChar + "EPG"))
            {
                DialogResult result = MessageBox.Show("The path does not reference an 'EPG' directory." +
                    Environment.NewLine + Environment.NewLine + "Is the path correct?", "EPG Centre",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return (false);
            }

            Cursor.Current = Cursors.WaitCursor;
            string message = runParameters.Save(fileName);
            Cursor.Current = Cursors.Arrow;

            if (message == null)
            {
                MessageBox.Show("The parameters have been saved to '" + fileName + "'", " EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Information);
                newFile = false;
                originalData = runParameters.Clone();
                currentFileName = fileName;
                copyPluginModule(fileName);
            }
            else
                MessageBox.Show("An error has occurred while writing the parameters." + Environment.NewLine + Environment.NewLine + message, "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return (message == null);
        }

        private string getSelectedFrequency()
        {
            if (cbEnableSatellite.Checked)
                return(((SatelliteFrequency)cboDVBSScanningFrequency.SelectedItem).Frequency.ToString());

            if (cbEnableTerrestrial.Checked)
                return (((TerrestrialFrequency)cboDVBTScanningFrequency.SelectedItem).Frequency.ToString());

            if (cbEnableCable.Checked)
                return (((CableFrequency)cboCableScanningFrequency.SelectedItem).Frequency.ToString());

            if (cbEnableAtsc.Checked)
                return (((AtscFrequency)cboAtscScanningFrequency.SelectedItem).Frequency.ToString());

            return (((ClearQamFrequency)cboClearQamScanningFrequency.SelectedItem).Frequency.ToString());
        }

        private CollectionType getCollectionType()
        {
            if (cbEnableSatellite.Checked)
                return ((CollectionType)cboDVBSCollectionType.SelectedItem);

            if (cbEnableTerrestrial.Checked)
                return ((CollectionType)cboDVBTCollectionType.SelectedItem);

            if (cbEnableCable.Checked)
                return ((CollectionType)cboCableCollectionType.SelectedItem);

            if (cbEnableAtsc.Checked)
                return ((CollectionType)cboAtscCollectionType.SelectedItem);

            return ((CollectionType)cboClearQamCollectionType.SelectedItem);
        }

        private void copyPluginModule(string parameterFileName)
        {
            FileInfo fileInfo = new FileInfo(parameterFileName);

            if (!fileInfo.DirectoryName.ToUpper().EndsWith(Path.DirectorySeparatorChar + "EPG"))
                return;

            string oldPluginPath = Path.Combine(fileInfo.DirectoryName, "DVBLogicCPPPlugin.dll");
            string newPluginPath = Path.Combine(RunParameters.BaseDirectory, "DVBLogicCPPPlugin.dll");

            if (File.Exists(oldPluginPath))
            {
                Logger.Instance.Write("Existing plugin located");

                DateTime newWriteTime = File.GetLastWriteTime(newPluginPath);
                DateTime existingWriteTime = File.GetLastWriteTime(oldPluginPath);

                if (newWriteTime <= existingWriteTime)
                {
                    Logger.Instance.Write("Existing plugin is up to date");
                    return;
                }
                else
                    Logger.Instance.Write("Newer version available to install");
            }

            try
            {
                File.Copy(newPluginPath, oldPluginPath, true);
                Logger.Instance.Write("Plugin installed - version now " + File.GetLastWriteTime(oldPluginPath));
                MessageBox.Show("The plugin module has been updated.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException e1)
            {
                Logger.Instance.Write("<e> Plugin install failed - " + e1.Message);
                MessageBox.Show("The plugin could not be updated." + Environment.NewLine + Environment.NewLine + e1.Message,
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (UnauthorizedAccessException e2)
            {
                Logger.Instance.Write("<e> Plugin install failed - " + e2.Message);
                MessageBox.Show("The plugin could not be updated." + Environment.NewLine + Environment.NewLine + e2.Message,
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string locationPath = Path.Combine(fileInfo.DirectoryName, "EPG Collector Gateway.cfg");

                if (File.Exists(locationPath))
                {
                    File.SetAttributes(locationPath, FileAttributes.Normal);
                    File.Delete(locationPath);
                }

                FileStream fileStream = new FileStream(locationPath, FileMode.Create, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fileStream);

                streamWriter.WriteLine("Location=" + RunParameters.BaseDirectory + Path.DirectorySeparatorChar + "DVBLogicPlugin.dll");

                streamWriter.Close();
                fileStream.Close();

                File.SetAttributes(locationPath, FileAttributes.ReadOnly);
                Logger.Instance.Write("Software location file written successfully");
                Logger.Instance.Write("Software location set to " + RunParameters.BaseDirectory);
            }
            catch (IOException e1)
            {
                Logger.Instance.Write("<e> The software location file could not be written - " + e1.Message);
                MessageBox.Show("The software location file could not be written." + Environment.NewLine + Environment.NewLine + e1.Message,
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (UnauthorizedAccessException e2)
            {
                Logger.Instance.Write("<e> The software location file could not be written - " + e2.Message);
                MessageBox.Show("The software location file could not be written." + Environment.NewLine + Environment.NewLine + e2.Message,
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
