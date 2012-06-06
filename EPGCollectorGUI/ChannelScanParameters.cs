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
using System.Windows.Forms;

using DomainObjects;

namespace EPGCentre
{
    public partial class ChannelScanParameters : Form
    {
        public Collection<Tuner> Tuners
        {
            get
            {
                Collection<Tuner> tuners = new Collection<Tuner>();

                for (int index = 1; index < clbTuners.Items.Count; index++)
                {
                    if (clbTuners.GetItemChecked(index))
                        tuners.Add((Tuner)clbTuners.Items[index]);
                }

                return (tuners);
            }
        }

        public SatelliteDish SatelliteDish
        {
            get
            {
                SatelliteDish satelliteDish = new SatelliteDish();
                satelliteDish.LNBLowBandFrequency = Int32.Parse(txtLNBLow.Text.Trim());
                satelliteDish.LNBHighBandFrequency = Int32.Parse(txtLNBHigh.Text.Trim());
                satelliteDish.LNBSwitchFrequency = Int32.Parse(txtLNBSwitch.Text.Trim());

                if (cboDiseqc.SelectedIndex != 0)
                    satelliteDish.DiseqcSwitch = cboDiseqc.Text;

                return (satelliteDish);
            }
        }

        public bool UseSignalPresent { get { return (cbUseSignalPresent.Checked); } }
        public bool SwitchAfterPlay { get { return (cbSwitchAfterPlay.Checked); } }
        public bool RepeatDiseqc { get { return (cbRepeatDiseqc.Checked); } }

        private ChannelScanParameters() { }

        public ChannelScanParameters(TuningFrequency tuningFrequency) : base()
        {
            InitializeComponent();

            clbTuners.Items.Add("Any available Tuner");

            foreach (Tuner tuner in Tuner.TunerCollection)
            {
                if (!tuner.Name.ToUpper().Contains("DVBLINK"))
                    clbTuners.Items.Add(tuner);
            }

            clbTuners.SetItemChecked(0, true);

            SatelliteDish satelliteDish = SatelliteDish.FirstDefault;
            txtLNBLow.Text = satelliteDish.LNBLowBandFrequency.ToString();
            txtLNBHigh.Text = satelliteDish.LNBHighBandFrequency.ToString();
            txtLNBSwitch.Text = satelliteDish.LNBSwitchFrequency.ToString();

            cboDiseqc.DataSource = Enum.GetValues(typeof(DiseqcSettings));

            gpDish.Enabled = (tuningFrequency.TunerType == TunerType.Satellite);
        }

        private void clbTuners_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbTuners.SelectedIndices[0] == 0)
            {
                for (int index = 1; index < clbTuners.Items.Count; index++)
                    clbTuners.SetItemChecked(index, false);
            }
            else
                clbTuners.SetItemChecked(0, false);
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            try
            {
                Int32.Parse(txtLNBLow.Text.Trim());
                Int32.Parse(txtLNBHigh.Text.Trim());
                Int32.Parse(txtLNBSwitch.Text.Trim());
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

            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.Cancel;
        }

        private void btLNBDefaults_Click(object sender, EventArgs e)
        {
            SatelliteDish defaultSatellite = SatelliteDish.Default;

            txtLNBLow.Text = defaultSatellite.LNBLowBandFrequency.ToString();
            txtLNBHigh.Text = defaultSatellite.LNBHighBandFrequency.ToString();
            txtLNBSwitch.Text = defaultSatellite.LNBSwitchFrequency.ToString();

            cboDiseqc.SelectedIndex = 0;
        }
    }
}
