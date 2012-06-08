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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

using DomainObjects;

namespace TSDumper
{
    public partial class ViewLogControl : UserControl, IView
    {
        /// <summary>
        /// Get the availability of Find and Filter.
        /// </summary>
        public bool FindFilterAvailability { get { return (records.Count != 0); } }

        private Collection<LineEntry> records;
        private string lastSearchText;
        private bool lastSearchIgnoreCase;
        private bool lastSearchDown;
        private string lastFilterText;
        private bool lastFilterIgnoreCase;
        private string logFileName;

        private Logger logger;

        private string informationText = "Information";
        private string exceptionText = "Exception";
        private string errorText = "Error";
        private string completedText = "Completed";
        private string summaryText = "Summary";

        public ViewLogControl()
        {
            InitializeComponent();
        }

        public void Process(string logFileName, Logger logger, bool useInformation)
        {
            //Cursor.Current = Cursors.WaitCursor;

            this.logFileName = logFileName;
            this.logger = logger;

            records = new Collection<LineEntry>();
            
            string line;

            try
            {
                do
                {
                    line = logger.Read();
                    if (line != null)
                        records.Add(processLine(line));
                } while (line != null);
            } catch (Exception e)
            {
                
            }


            dgViewLog.Rows.Clear();
            dgViewLog.Columns[1].Visible = useInformation;
            dgViewLog.VirtualMode = true;
            dgViewLog.CellValueNeeded += new DataGridViewCellValueEventHandler(cellValueNeeded);
            dgViewLog.RowCount = records.Count;

           // Cursor.Current = Cursors.Arrow;

            if (dgViewLog.Rows.Count > 0)
            dgViewLog.FirstDisplayedScrollingRowIndex = dgViewLog.Rows.Count - 1;
        }

        private void cellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= records.Count)
            {
                e.Value = string.Empty;
                return;
            }

            switch (dgViewLog.Columns[e.ColumnIndex].Name)
            {
                case "timeColumn":
                    e.Value = records[e.RowIndex].Time;
                    break;
                case "typeColumn":
                    e.Value = records[e.RowIndex].Type;

                    if ((string)e.Value == exceptionText)
                    {
                        dgViewLog.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                        dgViewLog.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                    }
                    else
                    {
                        if ((string)e.Value == errorText)
                        {
                            dgViewLog.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DarkRed;
                            dgViewLog.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                        }
                        else
                        {
                            if ((string)e.Value == completedText)
                            {
                                dgViewLog.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                                dgViewLog.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                            }
                            else
                            {
                                if ((string)e.Value == summaryText)
                                {
                                    dgViewLog.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DarkSlateBlue;
                                    dgViewLog.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                                }
                            }
                        }
                    }

                    break;
                case "detailColumn":
                    e.Value = records[e.RowIndex].Detail;
                    break;
                default:
                    break;
            }

            dgViewLog.Rows[e.RowIndex].Height = 16;
        }

        private LineEntry processLine(string line)
        {
            if (line.Length == 0)
            {
                LineEntry emptyEntry = new LineEntry();
                emptyEntry.Time = "";
                emptyEntry.Type = "";
                emptyEntry.Detail = "";
                return (emptyEntry);
            }

            string editedLine = line.Replace("\u0009", "    ");            
 
            LineEntry lineEntry = new LineEntry();

            bool timePresent = false;

            if (editedLine[0] >= '0' && editedLine[0] <= '9')
            {
                lineEntry.Time = editedLine.Substring(0, 12);
                timePresent = true;
            }
            else
                lineEntry.Time = "";

            int detailOffset = 13;

            if (timePresent && editedLine.Length > 17 && editedLine[13] == '<' && editedLine[15] == '>')
            {
                detailOffset = 17;

                switch (editedLine[14])
                {
                    case 'e':
                        lineEntry.Type = errorText;
                        break;
                    case 'E':
                        lineEntry.Type = exceptionText;
                        break;
                    case 'I':
                        lineEntry.Type = informationText;
                        break;
                    case 'C':
                        lineEntry.Type = completedText;
                        break;
                    case 'S':
                        lineEntry.Type = summaryText;
                        break;
                    default:
                        lineEntry.Type = informationText;
                        break;
                }
            }
            else
                lineEntry.Type = "Information";

            if (timePresent)
            {
                if (editedLine.Length > detailOffset)
                    lineEntry.Detail = editedLine.Substring(detailOffset);
                else
                    lineEntry.Detail = "";
            }
            else
                lineEntry.Detail = editedLine;
            
            return(lineEntry);
        }

        /// <summary>
        /// Find a specific line.
        /// </summary>
        public void FindText()
        {
           
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
            
        }

        private bool checkLine(LineEntry lineEntry, string[] filterStrings, bool ignoreCase)
        {
            foreach (string filterString in filterStrings)
            {
                if (!ignoreCase)
                {
                    if (lineEntry.Time.Contains(filterString.Trim()) || lineEntry.Type.Contains(filterString.Trim()) || lineEntry.Detail.Contains(filterString.Trim()))
                        return (true);
                }
                else
                {
                    string upperFilterString = filterString.Trim().ToUpper();
                    if (lineEntry.Time.ToUpper().Contains(upperFilterString) || lineEntry.Type.ToUpper().Contains(upperFilterString) || lineEntry.Detail.ToUpper().Contains(upperFilterString))
                        return (true);
                }
            }

            return(false);
        }

        /// <summary>
        /// Clear the view.
        /// </summary>
        public void Clear()
        {
            dgViewLog.Rows.Clear();
        }

        private class LineEntry
        {
            internal string Time
            {
                get { return (time); }
                set { time = value; }
            }

            internal string Type
            {
                get { return (type); }
                set { type = value; }
            }

            internal string Detail
            {
                get { return (detail); }
                set { detail = value; }
            }

            private string time;
            private string type;
            private string detail;

            internal LineEntry() { }
        }

        private void dgViewLog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
