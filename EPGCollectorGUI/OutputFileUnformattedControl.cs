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

using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;

using DomainObjects;

namespace EPGCentre
{
    public partial class OutputFileUnformattedControl : UserControl, IView
    {
        /// <summary>
        /// Get the availability of Find and Filter.
        /// </summary>
        public bool FindFilterAvailability { get { return (records.Count != 0); } }

        private string fileName;
        private Collection<string> records;
        private string lastSearchText;
        private bool lastSearchIgnoreCase;
        private bool lastSearchDown;
        private string lastFilterText;
        private bool lastFilterIgnoreCase;

        public OutputFileUnformattedControl()
        {
            InitializeComponent();
        }

        public void Process(string fileName)
        {
            Cursor.Current = Cursors.WaitCursor;

            this.fileName = fileName;
            records = new Collection<string>();

            FileStream fileStream;
            
            try { fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read); }
            catch (IOException)
            {
                Logger.Instance.Write("Failed to open " + fileName);
                MessageBox.Show("Failed to open " + fileName, "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            StreamReader streamReader = new StreamReader(fileStream);            

            while (!streamReader.EndOfStream)
                records.Add(streamReader.ReadLine());

            streamReader.Close();
            fileStream.Close();

            dgViewFile.Rows.Clear();
            dgViewFile.VirtualMode = true;
            dgViewFile.CellValueNeeded += new DataGridViewCellValueEventHandler(cellValueNeeded);
            dgViewFile.RowCount = records.Count;

            Cursor.Current = Cursors.Arrow;
        }

        private void cellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= records.Count)
            {
                e.Value = string.Empty;
                return;
            }

            e.Value = records[e.RowIndex];
            dgViewFile.Rows[e.RowIndex].Height = 16;
        }

        /// <summary>
        /// Find a specific line.
        /// </summary>
        public void FindText()
        {
            FindText findText = new FindText(lastSearchText, lastSearchIgnoreCase, lastSearchDown);
            DialogResult result = findText.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            lastSearchText = findText.SearchText;
            lastSearchIgnoreCase = findText.IgnoreCase;
            lastSearchDown = findText.SearchDown;

            string[] searchStrings = lastSearchText.Split(new char[] { '+' });

            int index = 0;
            int increment = 1;
            int lastIndex = records.Count - 1;

            if (!findText.BeginningEnd)
            {
                if (findText.SearchDown)
                {
                    if (dgViewFile.FirstDisplayedCell.RowIndex + 1 < records.Count)
                        index = dgViewFile.FirstDisplayedCell.RowIndex + 1;
                }
                else
                {
                    if (dgViewFile.FirstDisplayedCell.RowIndex > 0)
                        index = dgViewFile.FirstDisplayedCell.RowIndex - 1;
                }
            }

            if (!findText.SearchDown)
            {
                if (findText.BeginningEnd)
                    index = records.Count - 1;
                increment = -1;
                lastIndex = records.Count - 1;
            }

            for (; checkIndex(index, lastIndex, findText.SearchDown); index += increment)
            {
                string record = records[index];

                foreach (string searchString in searchStrings)
                {
                    if (!findText.IgnoreCase)
                    {
                        if (record.Contains(searchString.Trim()))
                        {
                            dgViewFile.FirstDisplayedCell = dgViewFile.Rows[records.IndexOf(record)].Cells[0];
                            return;
                        }
                    }
                    else
                    {
                        if (record.ToUpper().Contains(searchString.Trim().ToUpper()))
                        {
                            dgViewFile.FirstDisplayedCell = dgViewFile.Rows[records.IndexOf(record)].Cells[0];
                            return;
                        }
                    }
                }
            }

            MessageBox.Show("The search string(s) were not located.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool checkIndex(int currentIndex, int lastIndex, bool searchDown)
        {
            if (searchDown)
                return (currentIndex <= lastIndex);
            else
                return (currentIndex >= lastIndex);
        }

        /// <summary>
        /// Filter the data.
        /// </summary>
        public void FilterText()
        {
            FilterText filterText = new FilterText(lastFilterText, lastFilterIgnoreCase);
            DialogResult result = filterText.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            lastFilterText = filterText.Filter;
            lastFilterIgnoreCase = filterText.IgnoreCase;

            string[] filterStrings = lastFilterText.Split(new char[] { '+' });

            Cursor.Current = Cursors.WaitCursor;
            Collection<string> newRecords = new Collection<string>();

            FileStream fileStream;

            try { fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read); }
            catch (IOException)
            {
                Logger.Instance.Write("Failed to open " + fileName);
                MessageBox.Show("Failed to open " + fileName, "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            StreamReader streamReader = new StreamReader(fileStream);

            while (!streamReader.EndOfStream)
            {
                bool include = checkLine(streamReader.ReadLine(), filterStrings, lastFilterIgnoreCase);
                if (include)
                    newRecords.Add(streamReader.ReadLine());
            }

            streamReader.Close();
            fileStream.Close();

            if (newRecords.Count == 0)
            {
                MessageBox.Show("No records match the filter criteria.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            records = newRecords;

            dgViewFile.Rows.Clear();
            dgViewFile.VirtualMode = true;
            dgViewFile.CellValueNeeded += new DataGridViewCellValueEventHandler(cellValueNeeded);
            dgViewFile.RowCount = records.Count;

            Cursor.Current = Cursors.Arrow;
        }

        private bool checkLine(string line, string[] filterStrings, bool ignoreCase)
        {
            foreach (string filterString in filterStrings)
            {
                if (!ignoreCase)
                {
                    if (line.Contains(filterString.Trim()))
                        return (true);
                }
                else
                {
                    if (line.ToUpper().Contains(filterString.Trim().ToUpper()))
                        return (true);
                }
            }

            return (false);
        }
    }
}
