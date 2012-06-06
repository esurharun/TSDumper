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
using System.Windows.Forms;

namespace EPGCentre
{
    public partial class FilterText : Form
    {
        public string Filter { get { return (tbFilterText.Text.Trim()); } }
        public bool IgnoreCase { get { return (cbIgnoreCase.Checked); } }

        public FilterText(string filterText, bool ignoreCase)
        {
            InitializeComponent();

            if (filterText != null)
            {
                tbFilterText.Text = filterText;
                cbIgnoreCase.Checked = ignoreCase;
            }
        }

        private void tbFilterText_TextChanged(object sender, EventArgs e)
        {
            /*btOK.Enabled = (tbFilterText.Text.Length != 0);*/
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            Close();
            DialogResult = DialogResult.OK;
        }
    }
}
