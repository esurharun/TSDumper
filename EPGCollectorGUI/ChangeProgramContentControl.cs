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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

using DomainObjects;
using DVBServices;

namespace EPGCentre
{
    public partial class ChangeProgramContentControl : UserControl, IUpdateControl
    {
        /// <summary>
        /// Get the general window heading for the data.
        /// </summary>
        public string Heading { get { return ("EPG Centre - Change EIT Program Categories - "); } }
        /// <summary>
        /// Get the default directory.
        /// </summary>
        public string DefaultDirectory { get { return (RunParameters.ConfigDirectory); } }
        /// <summary>
        /// Get the default output file name.
        /// </summary>
        public string DefaultFileName { get { return ("EIT Categories"); } }
        /// <summary>
        /// Get the save file filter.
        /// </summary>
        public string SaveFileFilter { get { return ("EIT Program Category Files (EIT Categories*.cfg)|EIT Categories*.cfg"); } }
        /// <summary>
        /// Get the save file title.
        /// </summary>
        public string SaveFileTitle { get { return ("Save EPG Collection EIT Program Category File"); } }
        /// <summary>
        /// Get the save file suffix.
        /// </summary>
        public string SaveFileSuffix { get { return ("cfg"); } }

        /// <summary>
        /// Return the state of the data set.
        /// </summary>
        public DataState DataState { get { return (hasDataChanged()); } }

        private BindingList<EITProgramContent> bindingList;

        private string sortedColumnName;
        private string sortedKeyName;
        private bool sortedAscending;

        private bool errors;
        private string currentFileName;

        public ChangeProgramContentControl()
        {
            InitializeComponent();
        }

        public bool Process(string fileName)
        {
            bool reply = EITProgramContent.Load(fileName);
            if (!reply)
                return (false);

            currentFileName = fileName;

            bindingList = new BindingList<EITProgramContent>();
            foreach (EITProgramContent content in EITProgramContent.Contents)
                bindingList.Add(new EITProgramContent(content.ContentID, content.SubContentID, content.FullDescription));

            contentBindingSource.DataSource = bindingList;
            dgContents.FirstDisplayedCell = dgContents.Rows[0].Cells[0];

            return (true);
        }

        private void dgContents_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgContents.CurrentCell.ColumnIndex == dgContents.Columns["descriptionColumn"].Index)
            {
                TextBox textEdit = e.Control as TextBox;
                textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressNumeric);
                textEdit.KeyPress -= new KeyPressEventHandler(dvblogicTextEdit_KeyPressAlphaNumeric); 
                textEdit.KeyPress += new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
            }
            else
            {
                if (dgContents.CurrentCell.ColumnIndex == dgContents.Columns["wmcDescriptionColumn"].Index)
                {
                    TextBox textEdit = e.Control as TextBox;
                    textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                    textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressNumeric);
                    textEdit.KeyPress -= new KeyPressEventHandler(dvblogicTextEdit_KeyPressAlphaNumeric);
                    textEdit.KeyPress += new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                }
                else
                {
                    if (dgContents.CurrentCell.ColumnIndex == dgContents.Columns["dvblogicDescriptionColumn"].Index)
                    {
                        TextBox textEdit = e.Control as TextBox;
                        textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                        textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressNumeric);
                        textEdit.KeyPress -= new KeyPressEventHandler(dvblogicTextEdit_KeyPressAlphaNumeric);
                        textEdit.KeyPress += new KeyPressEventHandler(dvblogicTextEdit_KeyPressAlphaNumeric);
                    }
                    else
                    {
                        if (dgContents.CurrentCell.ColumnIndex == dgContents.Columns["contentIDColumn"].Index ||
                            dgContents.CurrentCell.ColumnIndex == dgContents.Columns["subContentIDColumn"].Index)
                        {
                            TextBox textEdit = e.Control as TextBox;
                            textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                            textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressNumeric);
                            textEdit.KeyPress -= new KeyPressEventHandler(dvblogicTextEdit_KeyPressAlphaNumeric);
                            textEdit.KeyPress += new KeyPressEventHandler(textEdit_KeyPressNumeric);
                        }
                    }
                }
            }
        }

        private void textEdit_KeyPressAlphaNumeric(object sender, KeyPressEventArgs e)
        {
            Regex alphaNumericPattern = new Regex(@"[a-zA-Z0-9,!&*()--+'?\s\b]");
            e.Handled = !alphaNumericPattern.IsMatch(e.KeyChar.ToString());
        }

        private void dvblogicTextEdit_KeyPressAlphaNumeric(object sender, KeyPressEventArgs e)
        {
            Regex alphaNumericPattern = new Regex(@"[a-zA-Z,\s\b]");
            e.Handled = !alphaNumericPattern.IsMatch(e.KeyChar.ToString());
        }

        private void textEdit_KeyPressNumeric(object sender, KeyPressEventArgs e)
        {
            if ("0123456789\b".IndexOf(e.KeyChar) == -1)
                e.Handled = true;
        }

        private void dgContentsRowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            string contentIDString;
            string subContentIDString;
            string description;
            string wmcDescription;
            string dvblogicDescription;            

            if (dgContents.Rows[e.RowIndex].Cells["contentIDColumn"].Value == null)
                contentIDString = string.Empty;
            else
                contentIDString = dgContents.Rows[e.RowIndex].Cells["contentIDColumn"].Value.ToString().Trim();

            if (dgContents.Rows[e.RowIndex].Cells["subContentIDColumn"].Value == null)
                subContentIDString = string.Empty;
            else
                subContentIDString = dgContents.Rows[e.RowIndex].Cells["subContentIDColumn"].Value.ToString().Trim();

            if (dgContents.Rows[e.RowIndex].Cells["descriptionColumn"].Value == null)
                description = string.Empty;
            else
                description = dgContents.Rows[e.RowIndex].Cells["descriptionColumn"].Value.ToString().Trim();

            if (dgContents.Rows[e.RowIndex].Cells["wmcDescriptionColumn"].Value == null)
                wmcDescription = string.Empty;
            else
                wmcDescription = dgContents.Rows[e.RowIndex].Cells["wmcDescriptionColumn"].Value.ToString().Trim();

            if (dgContents.Rows[e.RowIndex].Cells["dvblogicDescriptionColumn"].Value == null)
                dvblogicDescription = string.Empty;
            else
                dvblogicDescription = dgContents.Rows[e.RowIndex].Cells["dvblogicDescriptionColumn"].Value.ToString().Trim();

            if (contentIDString == string.Empty &&
                subContentIDString == string.Empty &&
                description == string.Empty &&
                wmcDescription == string.Empty &&
                dvblogicDescription == string.Empty)
            {
                errors = false;
                e.Cancel = true;
                return;
            }

            errors = true;
            int contentID;
            int subContentID;

            try
            {
                contentID = Int32.Parse(contentIDString);                
            }
            catch (FormatException)
            {
                MessageBox.Show("Category ID '" + contentIDString + "' is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }
            
            try
            {
                subContentID = Int32.Parse(subContentIDString);
            }
            catch (FormatException)
            {
                MessageBox.Show("Sub-category ID '" + subContentIDString + "' is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            foreach (EITProgramContent content in bindingList)
            {
                if (content.ContentID == contentID && content.SubContentID == subContentID && bindingList.IndexOf(content) != e.RowIndex)
                {
                    MessageBox.Show("Category " + contentIDString + " sub-category " + subContentID + " already exists.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }
            }

            if (description == string.Empty)
            {
                MessageBox.Show("The EIT description for category " + contentIDString + " sub-category " + subContentIDString + " is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            if (dvblogicDescription != null)
            {
                bool validValue = DVBLogicProgramCategory.CheckDescription(dvblogicDescription.ToString());
                if (!validValue)
                {
                    MessageBox.Show("The DVBLogic description for category " + contentIDString + " sub-category " + subContentIDString + " is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }
            }  

            errors = false;
        }

        private void dgContentsDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["contentIDColumn"].Value = string.Empty;
            e.Row.Cells["subContentIDColumn"].Value = string.Empty;
            e.Row.Cells["descriptionColumn"].Value = string.Empty;
            e.Row.Cells["wmcDescriptionColumn"].Value = string.Empty;
            e.Row.Cells["dvblogicDescriptionColumn"].Value = string.Empty;
        }

        private void dgContentsColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sortedColumnName == null)
            {
                sortedAscending = true;
                sortedColumnName = dgContents.Columns[e.ColumnIndex].Name;
            }
            else
            {
                if (sortedColumnName == dgContents.Columns[e.ColumnIndex].Name)
                    sortedAscending = !sortedAscending;
                else
                    sortedColumnName = dgContents.Columns[e.ColumnIndex].Name;
            }

            Collection<EITProgramContent> sortedContents = new Collection<EITProgramContent>();

            foreach (EITProgramContent content in bindingList)
            {
                switch (dgContents.Columns[e.ColumnIndex].Name)
                {
                    case "contentIDColumn":
                        addInOrder(sortedContents, content, sortedAscending, "ContentID");
                        break;
                    case "subContentIDColumn":
                        addInOrder(sortedContents, content, sortedAscending, "SubContentID");
                        break;
                    case "descriptionColumn":
                        addInOrder(sortedContents, content, sortedAscending, "Description");
                        break;
                    case "wmcDescriptionColumn":
                        addInOrder(sortedContents, content, sortedAscending, "WMCDescription");
                        break;
                    case "dvblogicDescriptionColumn":
                        addInOrder(sortedContents, content, sortedAscending, "DVBLogicDescription");
                        break;
                    default:
                        return;
                }
            }

            bindingList = new BindingList<EITProgramContent>();
            foreach (EITProgramContent content in sortedContents)
                bindingList.Add(content);

            contentBindingSource.DataSource = bindingList;
        }

        private void addInOrder(Collection<EITProgramContent> contents, EITProgramContent newContent, bool sortedAscending, string keyName)
        {
            sortedKeyName = keyName;

            foreach (EITProgramContent oldContent in contents)
            {
                if (sortedAscending)
                {
                    if (oldContent.CompareForSorting(newContent, keyName) > 0)
                    {
                        contents.Insert(contents.IndexOf(oldContent), newContent);
                        return;
                    }
                }
                else
                {
                    if (oldContent.CompareForSorting(newContent, keyName) < 0)
                    {
                        contents.Insert(contents.IndexOf(oldContent), newContent);
                        return;
                    }
                }
            }

            contents.Add(newContent);
        }

        private DataState hasDataChanged()
        {
            dgContents.EndEdit();

            if (errors)
                return (DataState.HasErrors);

            if (bindingList.Count != EITProgramContent.Contents.Count)
                return (DataState.Changed);

            foreach (EITProgramContent content in bindingList)
            {
                EITProgramContent existingContent = EITProgramContent.FindContent(content.ContentID, content.SubContentID);
                if (existingContent == null)
                    return (DataState.Changed);

                if (existingContent.FullDescription != content.FullDescription)
                    return (DataState.Changed);
            }

            return (DataState.NotChanged);
        }

        /// <summary>
        /// Validate the data and set up to save it.
        /// </summary>
        /// <returns>True if the data can be saved; false otherwise.</returns>
        public bool PrepareToSave()
        {
            dgContents.EndEdit();

            if (bindingList.Count == 0)
            {
                MessageBox.Show("No categories defined.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }

            Collection<EITProgramContent> addedList = new Collection<EITProgramContent>();

            foreach (EITProgramContent content in bindingList)
            {
                bool valid = validateEntry(addedList, content);
                if (!valid)
                    return (false);

                addedList.Add(new EITProgramContent(content.ContentID, content.SubContentID, content.FullDescription));
            }

            EITProgramContent.Contents.Clear();

            foreach (EITProgramContent content in bindingList)
                EITProgramContent.AddContent(content.ContentID, content.SubContentID, content.FullDescription);

            return (true);
        }

        private bool validateEntry(Collection<EITProgramContent> addedList, EITProgramContent content)
        {
            foreach (EITProgramContent existingContent in addedList)
            {
                if (existingContent.ContentID == content.ContentID &&
                    existingContent.SubContentID == content.SubContentID)
                {
                    MessageBox.Show("Category " + existingContent.ContentIDString + " sub-category " + existingContent.SubContentIDString + " already exists.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return (false);
                }
            }

            if (content.FullDescription == null || content.FullDescription == string.Empty)
            {
                MessageBox.Show("The EIT description for category " + content.ContentIDString + " sub-category " + content.SubContentIDString + " is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }

            bool validValue = DVBLogicProgramCategory.CheckDescription(content.DVBLogicDescription);
            if (!validValue)
            {
                MessageBox.Show("The DVBLogic description for category " + content.ContentIDString + " sub-category " + content.SubContentIDString + " is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }

            return (true);
        }

        /// <summary>
        /// Save the current data set to the original file.
        /// </summary>
        /// <returns>True if the data has been saved; false otherwise.</returns>
        public bool Save()
        {
            FileInfo fileInfo = new FileInfo(currentFileName);            
            return (Save(Path.Combine(RunParameters.DataDirectory, fileInfo.Name)));
        }

        /// <summary>
        /// Save the current data set to a specified file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>True if the data has been saved; false otherwise.</returns>
        public bool Save(string fileName)
        {
            string[] parts = fileName.Split(new char[] { Path.DirectorySeparatorChar });
            if (parts.Length < 2 || fileName != Path.Combine(RunParameters.DataDirectory, parts[parts.Length - 1]))
            {
                DialogResult result = MessageBox.Show("The data must be saved to the data directory.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }

            string message = EITProgramContent.Save(fileName);

            if (message == null)
            {
                MessageBox.Show("The EIT program categories have been saved to '" + fileName + "'", " EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Information);
                currentFileName = fileName;
            }
            else
                MessageBox.Show("An error has occurred while writing the EIT program categories." + Environment.NewLine + Environment.NewLine + message, "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return (message == null);
        }
    }
}
