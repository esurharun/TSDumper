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
using System.IO;
using System.Windows.Forms;

using DomainObjects;

namespace EPGCentre
{
    public partial class SelectTask : Form
    {
        public string Task 
        { 
            get
            {
                if ((string)lbTasks.SelectedItem != customString)
                    return ((string)lbTasks.SelectedItem);
                else
                    return (null);
            } 
        }

        private string customString = " - Custom Parameters -";

        private SelectTask() { }

        public SelectTask(bool pluginList) : base()
        {
            InitializeComponent();

            DirectoryInfo directoryInfo;
            if (!pluginList)
                directoryInfo = new DirectoryInfo(Path.Combine(RunParameters.BaseDirectory, Path.Combine("Samples", "Collector")));
            else
                directoryInfo = new DirectoryInfo(Path.Combine(RunParameters.BaseDirectory, Path.Combine("Samples", "DVBLogic Plugin")));

            foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.ini"))
                lbTasks.Items.Add(fileInfo.Name.Substring(0, fileInfo.Name.Length - 4));

            lbTasks.Sorted = false;

            lbTasks.Items.Insert(0, customString);
            lbTasks.SelectedIndex = 0;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.Cancel;
        }

        private void lbTasksDoubleClick(object sender, MouseEventArgs e)
        {
            int index = lbTasks.IndexFromPoint(e.Location);

            if (index != ListBox.NoMatches)
            {
                this.Close();
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
