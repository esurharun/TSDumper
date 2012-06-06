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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using DomainObjects;

namespace EPGCentre
{
    /// <summary>
    /// The class that gets the satellite and frequency for saving an MHW program category file.
    /// </summary>
    public partial class SaveMHWCategoryFile : Form
    {
        /// <summary>
        /// Get the selected satellite.
        /// </summary>
        public Satellite SelectedSatellite { get { return ((Satellite)cboSatellite.SelectedItem); } }

        /// <summary>
        /// Get the selected frequency.
        /// </summary>
        public TuningFrequency SelectedFrequency { get { return ((TuningFrequency)cboDVBSScanningFrequency.SelectedItem); } }

        /// <summary>
        /// Get the selected type.
        /// </summary>
        public int SelectedType 
        { 
            get 
            {
                int lastIndex = cbType.SelectedItem.ToString().Length - 1;
                return (Int32.Parse(cbType.SelectedItem.ToString().Substring(lastIndex, 1))); 
            } 
        }

        /// <summary>
        /// Initialize a new instance of the SaveMHWCategoryFile class.
        /// </summary>
        public SaveMHWCategoryFile()
        {
            InitializeComponent();

            Satellite.Load(Path.Combine(RunParameters.ConfigDirectory, Path.Combine("TuningParameters", "dvbs")) + Path.DirectorySeparatorChar);

            cboSatellite.DataSource = Satellite.Providers;
            cboSatellite.SelectedItem = cboSatellite.Items[0];
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
            TuningFrequency tuningFrequency = (TuningFrequency)cboDVBSScanningFrequency.SelectedItem;
            if (tuningFrequency.CollectionType == CollectionType.MediaHighway1)
                cbType.SelectedIndex = 0;
            else
                cbType.SelectedIndex = 1;

        }
    }
}
