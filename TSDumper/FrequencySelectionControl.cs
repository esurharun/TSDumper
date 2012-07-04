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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms;
using System.IO;

using DomainObjects;

namespace TSDumper
{
    public partial class FrequencySelectionControl : UserControl
    {

        internal Collection<int> selected_tuner_indexes
        {
            get
            {
                Collection<int> tuners = new Collection<int>();

                for (int index = 0; index < clbTuners.Items.Count; index++)
                {
                    bool isChecked = clbTuners.GetItemChecked(index);

                    if (isChecked)
                        tuners.Add(index);
                    
                }

                return tuners;
            }

            set
            {
                for (int i = 0; i < clbTuners.Items.Count;i++ )
                {
                    clbTuners.SetItemChecked(i,false);
                }

                for (int i = 0; i < value.Count; i++)
                {

                    clbTuners.SetItemChecked(value[i] , true);
                }
            }
        }

        internal Collection<int> Tuners
        {
            get
            {
                int selectedTunerCount = 0;

                for (int index = 0; index < clbTuners.Items.Count; index++)
                {
                    bool isChecked = clbTuners.GetItemChecked(index);
                    if (isChecked)
                        selectedTunerCount++;
                }

                if (selectedTunerCount == 0)
                    return (null);

                Collection<int> tuners = new Collection<int>();

                for (int index = 1; index < clbTuners.Items.Count; index++)
                {
                    if (clbTuners.GetItemChecked(index))
                        tuners.Add(Tuner.TunerCollection.IndexOf((Tuner)clbTuners.Items[index]) + 1);
                }

                return (tuners);
            }

            set
            {
                
            }
        }

        internal int LNBLowBandFrequency
        {
            get
            {
                try
                {
                    return(Int32.Parse(txtLNBLow.Text.Trim()));                
                }
                catch (FormatException)
                {
                    return(-1);
                }
                catch (ArithmeticException)
                {
                    return(-1);
                }
            }

            set { txtLNBLow.Text = string.Format("{0}", value); }
        }

        internal int LNBHighBandFrequency
        {
            get
            {
                try
                {
                    return (Int32.Parse(txtLNBHigh.Text.Trim()));
                }
                catch (FormatException)
                {
                    return (-1);
                }
                catch (ArithmeticException)
                {
                    return (-1);
                }
            }

            set { txtLNBHigh.Text = string.Format("{0}", value); }
        }

        internal int LNBSwitchFrequency
        {
            get
            {
                try
                {
                    return (Int32.Parse(txtLNBSwitch.Text.Trim()));
                }
                catch (FormatException)
                {
                    return (-1);
                }
                catch (ArithmeticException)
                {
                    return (-1);
                }
            }

            set { txtLNBSwitch.Text = string.Format("{0}", value); }
        }

        internal string DiseqcSwitch
        {
            get
            {
                if (cboDiseqc.SelectedIndex != 0)
                    return (cboDiseqc.Text);
                else
                    return (null);
            }

            set { cboDiseqc.Text = value; }
        }

        internal int selected_satellite_index
        {
            get { return cboSatellite.SelectedIndex; }
            set { cboSatellite.SelectedIndex = value;
                cboSatellite_SelectedIndexChanged(cboSatellite, null);
            }
        }

        internal int selected_frequency_index
        {
            get { return cboDVBSScanningFrequency.SelectedIndex; }
            set { cboDVBSScanningFrequency.SelectedIndex = value;  }
        }

        internal TuningFrequency SelectedFrequency
        {
            get
            {
                if (tbcDeliverySystem.SelectedTab.Name == "tbpSatellite")
                    return ((TuningFrequency)cboDVBSScanningFrequency.SelectedItem);
                else
                {
                    if (tbcDeliverySystem.SelectedTab.Name == "tbpTerrestrial")
                        return ((TuningFrequency)cboDVBTScanningFrequency.SelectedItem);
                    else
                    {
                        if (tbcDeliverySystem.SelectedTab.Name == "tbpCable")
                            return ((TuningFrequency)cboCableScanningFrequency.SelectedItem);
                        else
                        {
                            if (tbcDeliverySystem.SelectedTab.Name == "tbpAtsc")
                                return ((TuningFrequency)cboAtscScanningFrequency.SelectedItem);
                            else
                                return ((TuningFrequency)cboClearQamScanningFrequency.SelectedItem);

                        }
                    }
                }
            }

           
        }

        internal bool UseSignalPresent { get { return (cbUseSignalPresent.Checked); } set { cbUseSignalPresent.Checked = value; } }

        internal bool SwitchAfterPlay { get { return (cbSwitchAfterPlay.Checked); } set { cbSwitchAfterPlay.Checked = value; } }
        internal bool RepeatDiseqc { get { return (cbRepeatDiseqc.Checked); } set { cbRepeatDiseqc.Checked = value; } }


        public FrequencySelectionControl()
        {
            InitializeComponent();
        }

        internal void Process()
        {
            Satellite.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "dvbs")) + Path.DirectorySeparatorChar);
            TerrestrialProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "dvbt")) + Path.DirectorySeparatorChar);
            CableProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "dvbc")) + Path.DirectorySeparatorChar);
            AtscProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "atsc")) + Path.DirectorySeparatorChar);
            ClearQamProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "clear QAM")) + Path.DirectorySeparatorChar);
            ISDBSatelliteProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "isdbs")) + Path.DirectorySeparatorChar);
            ISDBTerrestrialProvider.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "isdbt")) + Path.DirectorySeparatorChar);
        
            clbTuners.Items.Clear();
            //clbTuners.Items.Add("Any available Tuner");

            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK"))
                    clbTuners.Items.Add(tuner);
            }

            if (clbTuners.Items.Count > 0)
              clbTuners.SetItemChecked(0, true);

            initializeSatelliteTab();
            initializeTerrestrialTab();
            initializeCableTab();
            initializeAtscTab();
            initializeClearQamTab();
            initializeISDBSTab();
            initializeISDBTTab();
        }

        private void initializeSatelliteTab()
        {
            bool satelliteUsed = false;

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
            
            cboSatellite.DataSource = Satellite.Providers;
            cboSatellite.SelectedItem = cboSatellite.Items[0];

            SatelliteDish satelliteDish = SatelliteDish.FirstDefault;

            txtLNBLow.Text = satelliteDish.LNBLowBandFrequency.ToString();
            txtLNBHigh.Text = satelliteDish.LNBHighBandFrequency.ToString();
            txtLNBSwitch.Text = satelliteDish.LNBSwitchFrequency.ToString();

            cboDiseqc.DataSource = Enum.GetValues(typeof(DiseqcSettings));
            if (satelliteDish.DiseqcSwitch != null)
                cboDiseqc.Text = satelliteDish.DiseqcSwitch;
        }

        private void initializeTerrestrialTab()
        {
            bool terrestrialUsed = false;

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

            cboCountry.DataSource = TerrestrialProvider.Countries;
            cboCountry.SelectedItem = cboCountry.Items[0];

        }

        private void initializeCableTab()
        {
            bool cableUsed = false;

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

            cboCable.DataSource = CableProvider.Providers;
            cboCable.SelectedItem = cboCable.Items[0];
        }

        private void initializeAtscTab()
        {
            bool atscUsed = false;

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

            cboAtscProvider.DataSource = AtscProvider.Providers;
            cboAtscProvider.SelectedItem = cboAtscProvider.Items[0];
        }

        private void initializeClearQamTab()
        {
            bool clearQamUsed = false;

            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK") && tuner.Supports(TunerNodeType.ATSC))
                    clearQamUsed = true;
            }

            if (!clearQamUsed)
            {
                tbcDeliverySystem.TabPages.RemoveByKey("tbpClearQAM");
                return;
            }

            cboClearQamProvider.DataSource = ClearQamProvider.Providers;
            cboClearQamProvider.SelectedItem = cboClearQamProvider.Items[0];
        }

        private void initializeISDBSTab()
        {
            bool satelliteUsed = false;

            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK") && tuner.Supports(TunerNodeType.ISDBS))
                    satelliteUsed = true;
            }

            if (!satelliteUsed)
            {
                tbcDeliverySystem.TabPages.RemoveByKey("tbpISDBSatellite");
                return;
            }

            cboISDBSatellite.DataSource = ISDBSatelliteProvider.Providers;
            cboISDBSatellite.SelectedItem = cboISDBSatellite.Items[0];

            SatelliteDish satelliteDish = SatelliteDish.FirstDefault;

            txtLNBLow.Text = satelliteDish.LNBLowBandFrequency.ToString();
            txtLNBHigh.Text = satelliteDish.LNBHighBandFrequency.ToString();
            txtLNBSwitch.Text = satelliteDish.LNBSwitchFrequency.ToString();

            cboISDBDiseqc.DataSource = Enum.GetValues(typeof(DiseqcSettings));
            if (satelliteDish.DiseqcSwitch != null)
                cboISDBDiseqc.Text = satelliteDish.DiseqcSwitch;
        }

        private void initializeISDBTTab()
        {
            bool terrestrialUsed = false;

            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK") && tuner.Supports(TunerNodeType.ISDBT))
                    terrestrialUsed = true;
            }

            if (!terrestrialUsed)
            {
                tbcDeliverySystem.TabPages.RemoveByKey("tbpISDBTerrestrial");
                return;
            }

            cboISDBTProvider.DataSource = ISDBTerrestrialProvider.Providers;
            cboISDBTProvider.SelectedItem = cboISDBTProvider.Items[0];
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

        private void cboCable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCable.SelectedItem != null)
                cboCableScanningFrequency.DataSource = ((CableProvider)cboCable.SelectedItem).Frequencies;
        }

        private void cboAtscProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboAtscProvider.SelectedItem != null)
                cboAtscScanningFrequency.DataSource = ((AtscProvider)cboAtscProvider.SelectedItem).Frequencies;
        }

        private void cboClearQamProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboClearQamProvider.SelectedItem != null)
                cboClearQamScanningFrequency.DataSource = ((ClearQamProvider)cboClearQamProvider.SelectedItem).Frequencies;
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

        private void cboISDBTProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboISDBTProvider.SelectedItem != null)
                cboISDBTScanningFrequency.DataSource = ((ISDBTerrestrialProvider)cboISDBTProvider.SelectedItem).Channels;
        }

        internal string ValidateForm()
        {
            Collection<int> tuners = Tuners;
            if (tuners == null)
                return ("No tuner selected.");

            bool tunerValid;

            if (tbcDeliverySystem.SelectedTab.Name == "tbpSatellite")
                tunerValid = checkForTunerType(tuners, TunerNodeType.Satellite);
            else
            {
                if (tbcDeliverySystem.SelectedTab.Name == "tbpTerrestrial")
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
                return ("The tuner(s) selected do not support the delivery system.");

            if (tbcDeliverySystem.SelectedTab.Name == "tbpSatellite")
            {
                try
                {
                    int lnbLowBandFrequency = Int32.Parse(txtLNBLow.Text.Trim());
                    int lnbHighBandFrequency = Int32.Parse(txtLNBHigh.Text.Trim());
                    int lnbSwitchFrequency = Int32.Parse(txtLNBSwitch.Text.Trim());                    
                }
                catch (FormatException)
                {
                    return ("A dish parameter is incorrect.");
                }
                catch (ArithmeticException)
                {
                    return ("A dish parameter is incorrect.");
                }
            }

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
    }
}
