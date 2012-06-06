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

using System.IO;
using System.Windows.Forms;

using DomainObjects;

namespace EPGCentre
{
    public partial class SelectContentFile : Form
    {
        public string File { get { return (((string)lbFiles.SelectedItem) + ".cfg"); } }

        public SelectContentFile()
        {
            InitializeComponent();

            DirectoryInfo dataDirectoryInfo = new DirectoryInfo(RunParameters.DataDirectory);

            foreach (FileInfo fileInfo in dataDirectoryInfo.GetFiles())
            {
                if (fileInfo.Name.Contains("EIT Categories") && fileInfo.Name.EndsWith(".cfg"))
                    lbFiles.Items.Add(fileInfo.Name.Substring(0, fileInfo.Name.Length - 4));
            }

            DirectoryInfo installDirectoryInfo = new DirectoryInfo(Path.Combine(RunParameters.ConfigDirectory, "Program Categories"));

            foreach (FileInfo fileInfo in installDirectoryInfo.GetFiles())
            {
                if (fileInfo.Name.Contains("EIT Categories") && fileInfo.Name.EndsWith(".cfg"))
                {
                    string newName = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
                    bool alreadyAdded = false;

                    foreach (string fileName in lbFiles.Items)
                    {
                        if (fileName == newName)
                            alreadyAdded = true;                        
                    }

                    if (!alreadyAdded)
                        lbFiles.Items.Add(newName);
                }
            }

            if (lbFiles.Items.Count > 0)
                lbFiles.SelectedIndex = 0;
        }

        private void lbFilesDoubleClick(object sender, MouseEventArgs e)
        {
            int index = lbFiles.IndexFromPoint(e.Location);

            if (index != ListBox.NoMatches)
            {
                this.Close();
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
