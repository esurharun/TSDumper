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
    public partial class ChangeProgramCategoryControl : UserControl, IUpdateControl
    {
        /// <summary>
        /// Get the general window heading for the data.
        /// </summary>
        public string Heading { get { return ("EPG Centre - Change OpenTV Program Categories - "); } }
        /// <summary>
        /// Get the default directory.
        /// </summary>
        public string DefaultDirectory { get { return (RunParameters.ConfigDirectory); } }
        /// <summary>
        /// Get the default output file name.
        /// </summary>
        public string DefaultFileName { get { return ("Program Categories XXX"); } }
        /// <summary>
        /// Get the save file filter.
        /// </summary>
        public string SaveFileFilter { get { return ("OpenTV Program Category Files (Program Categories*.cfg)|Program Categories*.cfg"); } }
        /// <summary>
        /// Get the save file title.
        /// </summary>
        public string SaveFileTitle { get { return ("Save EPG Collection OpenTV Program Category File"); } }
        /// <summary>
        /// Get the save file suffix.
        /// </summary>
        public string SaveFileSuffix { get { return ("cfg"); } }

        /// <summary>
        /// Return the state of the data set.
        /// </summary>
        public DataState DataState { get { return (hasDataChanged()); } }

        private BindingList<OpenTVProgramCategory> bindingList;

        private string sortedColumnName;
        private string sortedKeyName;
        private bool sortedAscending;

        private bool errors;
        private string currentFileName;

        public ChangeProgramCategoryControl()
        {
            InitializeComponent();
        }

        public bool Process(string fileName)
        {
            bool reply = OpenTVProgramCategory.Load(fileName);
            if (!reply)
                return (false);

            currentFileName = fileName;

            bindingList = new BindingList<OpenTVProgramCategory>();
            foreach (OpenTVProgramCategory category in OpenTVProgramCategory.Categories)
                bindingList.Add(new OpenTVProgramCategory(category.CategoryID, category.FullDescription));
            
            categoryBindingSource.DataSource = bindingList;
            dgCategories.FirstDisplayedCell = dgCategories.Rows[0].Cells[0];

            return (true);
        }

        private void dgCategories_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgCategories.CurrentCell.ColumnIndex == dgCategories.Columns["descriptionColumn"].Index)
            {
                TextBox textEdit = e.Control as TextBox;
                textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressNumeric);
                textEdit.KeyPress -= new KeyPressEventHandler(dvblogicTextEdit_KeyPressAlphaNumeric);
                textEdit.KeyPress -= new KeyPressEventHandler(dvbviewerTextEdit_KeyPressNumeric);
                textEdit.KeyPress += new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
            }
            else
            {
                if (dgCategories.CurrentCell.ColumnIndex == dgCategories.Columns["wmcDescriptionColumn"].Index)
                {
                    TextBox textEdit = e.Control as TextBox;
                    textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                    textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressNumeric);
                    textEdit.KeyPress -= new KeyPressEventHandler(dvblogicTextEdit_KeyPressAlphaNumeric);
                    textEdit.KeyPress -= new KeyPressEventHandler(dvbviewerTextEdit_KeyPressNumeric);
                    textEdit.KeyPress += new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                }
                else
                {
                    if (dgCategories.CurrentCell.ColumnIndex == dgCategories.Columns["dvblogicDescriptionColumn"].Index)
                    {
                        TextBox textEdit = e.Control as TextBox;
                        textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                        textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressNumeric);
                        textEdit.KeyPress -= new KeyPressEventHandler(dvblogicTextEdit_KeyPressAlphaNumeric);
                        textEdit.KeyPress -= new KeyPressEventHandler(dvbviewerTextEdit_KeyPressNumeric);
                        textEdit.KeyPress += new KeyPressEventHandler(dvblogicTextEdit_KeyPressAlphaNumeric);
                    }
                    else
                    {
                        if (dgCategories.CurrentCell.ColumnIndex == dgCategories.Columns["dvbviewerDescriptionColumn"].Index)
                        {
                            TextBox textEdit = e.Control as TextBox;
                            textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                            textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressNumeric);
                            textEdit.KeyPress -= new KeyPressEventHandler(dvblogicTextEdit_KeyPressAlphaNumeric);
                            textEdit.KeyPress -= new KeyPressEventHandler(dvbviewerTextEdit_KeyPressNumeric);
                            textEdit.KeyPress += new KeyPressEventHandler(dvbviewerTextEdit_KeyPressNumeric);
                        }
                        else
                        {
                            if (dgCategories.CurrentCell.ColumnIndex == dgCategories.Columns["categoryColumn"].Index)
                            {
                                TextBox textEdit = e.Control as TextBox;
                                textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressAlphaNumeric);
                                textEdit.KeyPress -= new KeyPressEventHandler(textEdit_KeyPressNumeric);
                                textEdit.KeyPress -= new KeyPressEventHandler(dvblogicTextEdit_KeyPressAlphaNumeric);
                                textEdit.KeyPress -= new KeyPressEventHandler(dvbviewerTextEdit_KeyPressNumeric);
                                textEdit.KeyPress += new KeyPressEventHandler(textEdit_KeyPressNumeric);
                            }
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

        private void dvbviewerTextEdit_KeyPressNumeric(object sender, KeyPressEventArgs e)
        {
            if ("0123456789,\b".IndexOf(e.KeyChar) == -1)
                e.Handled = true;
        }

        private void dgCategoriesRowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            string categoryIDString;
            string description;
            string wmcDescription;
            string dvblogicDescription;
            string dvbviewerDescription;

            if (dgCategories.Rows[e.RowIndex].Cells["categoryColumn"].Value == null)
                categoryIDString = string.Empty;
            else
                categoryIDString = dgCategories.Rows[e.RowIndex].Cells["categoryColumn"].Value.ToString().Trim();

            if (dgCategories.Rows[e.RowIndex].Cells["descriptionColumn"].Value == null)
                description = string.Empty;
            else
                description = dgCategories.Rows[e.RowIndex].Cells["descriptionColumn"].Value.ToString().Trim();

            if (dgCategories.Rows[e.RowIndex].Cells["wmcDescriptionColumn"].Value == null)
                wmcDescription = string.Empty;
            else
                wmcDescription = dgCategories.Rows[e.RowIndex].Cells["wmcDescriptionColumn"].Value.ToString().Trim();

            if (dgCategories.Rows[e.RowIndex].Cells["dvblogicDescriptionColumn"].Value == null)
                dvblogicDescription = string.Empty;
            else
                dvblogicDescription = dgCategories.Rows[e.RowIndex].Cells["dvblogicDescriptionColumn"].Value.ToString().Trim();

            if (dgCategories.Rows[e.RowIndex].Cells["dvbviewerDescriptionColumn"].Value == null)
                dvbviewerDescription = string.Empty;
            else
                dvbviewerDescription = dgCategories.Rows[e.RowIndex].Cells["dvbviewerDescriptionColumn"].Value.ToString().Trim();

            if (categoryIDString == string.Empty &&
                description == string.Empty &&
                wmcDescription == string.Empty &&
                dvblogicDescription == string.Empty &&
                dvbviewerDescription == string.Empty)
            {
                errors = false;
                e.Cancel = true;
                return;
            }

            errors = true;            

            try
            {
                int categoryID = Int32.Parse(categoryIDString);

                foreach (OpenTVProgramCategory category in bindingList)
                {
                    if (category.CategoryID == categoryID && bindingList.IndexOf(category) != e.RowIndex)
                    {
                        MessageBox.Show("Category " + categoryIDString + " already exists.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                        return;
                    }
                } 
            }
            catch (FormatException)
            {
                MessageBox.Show("Category '" + categoryIDString + "' is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            if (description == string.Empty)
            {
                MessageBox.Show("The OpenTV description for category " + categoryIDString + " is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            if (dvblogicDescription != string.Empty)
            {
                bool validValue = DVBLogicProgramCategory.CheckDescription(dvblogicDescription);
                if (!validValue)
                {
                    MessageBox.Show("The DVBLogic description for category " + categoryIDString + " is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }
            }

            if (dvbviewerDescription != string.Empty)
            {
                bool validValue = DVBViewerProgramCategory.CheckDescription(dvbviewerDescription);
                if (!validValue)
                {
                    MessageBox.Show("The DVBViewer description for category " + categoryIDString + " is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }
            }

            errors = false;
        }

        private void dgCategoriesDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["categoryColumn"].Value = string.Empty;
            e.Row.Cells["descriptionColumn"].Value = string.Empty;
            e.Row.Cells["wmcDescriptionColumn"].Value = string.Empty;
            e.Row.Cells["dvblogicDescriptionColumn"].Value = string.Empty;
            e.Row.Cells["dvbviewerDescriptionColumn"].Value = string.Empty;
        }

        private void dgCategoriesColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sortedColumnName == null)
            {
                sortedAscending = true;
                sortedColumnName = dgCategories.Columns[e.ColumnIndex].Name;
            }
            else
            {
                if (sortedColumnName == dgCategories.Columns[e.ColumnIndex].Name)
                    sortedAscending = !sortedAscending;
                else
                    sortedColumnName = dgCategories.Columns[e.ColumnIndex].Name;
            }

            Collection<OpenTVProgramCategory> sortedCategories = new Collection<OpenTVProgramCategory>();

            foreach (OpenTVProgramCategory category in bindingList)
            {
                switch (dgCategories.Columns[e.ColumnIndex].Name)
                {
                    case "categoryColumn":
                        addInOrder(sortedCategories, category, sortedAscending, "CategoryID");
                        break;
                    case "descriptionColumn":
                        addInOrder(sortedCategories, category, sortedAscending, "Description");
                        break;
                    case "wmcDescriptionColumn":
                        addInOrder(sortedCategories, category, sortedAscending, "WMCDescription");
                        break;
                    case "dvblogicDescriptionColumn":
                        addInOrder(sortedCategories, category, sortedAscending, "DVBLogicDescription");
                        break;
                    case "dvbviewerDescriptionColumn":
                        addInOrder(sortedCategories, category, sortedAscending, "DVBViewerDescription");
                        break;
                    default:
                        return;
                }
            }

            bindingList = new BindingList<OpenTVProgramCategory>();
            foreach (OpenTVProgramCategory category in sortedCategories)
                bindingList.Add(category);

            categoryBindingSource.DataSource = bindingList;
        }

        private void addInOrder(Collection<OpenTVProgramCategory> categories, OpenTVProgramCategory newCategory, bool sortedAscending, string keyName)
        {
            sortedKeyName = keyName;

            foreach (OpenTVProgramCategory oldCategory in categories)
            {
                if (sortedAscending)
                {
                    if (oldCategory.CompareForSorting(newCategory, keyName) > 0)
                    {
                        categories.Insert(categories.IndexOf(oldCategory), newCategory);
                        return;
                    }
                }
                else
                {
                    if (oldCategory.CompareForSorting(newCategory, keyName) < 0)
                    {
                        categories.Insert(categories.IndexOf(oldCategory), newCategory);
                        return;
                    }
                }
            }

            categories.Add(newCategory);
        }

        private DataState hasDataChanged()
        {
            dgCategories.EndEdit();

            if (errors)
                return (DataState.HasErrors);

            if (bindingList.Count != OpenTVProgramCategory.Categories.Count)
                return (DataState.Changed);

            foreach (OpenTVProgramCategory category in bindingList)
            {
                OpenTVProgramCategory existingCategory = OpenTVProgramCategory.FindCategory(category.CategoryID);
                if (existingCategory == null)
                    return (DataState.Changed);

                if (existingCategory.FullDescription != category.FullDescription)
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
            dgCategories.EndEdit();

            if (bindingList.Count == 0)
            {
                MessageBox.Show("No categories defined.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }
            
            Collection<OpenTVProgramCategory> addedList = new Collection<OpenTVProgramCategory>();

            foreach (OpenTVProgramCategory category in bindingList)
            {
                if (category.CategoryIDString != string.Empty)
                {
                    bool valid = validateEntry(addedList, category);
                    if (!valid)
                        return (false);
                    addedList.Add(new OpenTVProgramCategory(category.CategoryID, category.FullDescription));
                }
            }

            OpenTVProgramCategory.Categories.Clear();

            foreach (OpenTVProgramCategory category in bindingList)
            {
                if (category.CategoryIDString != string.Empty)
                    OpenTVProgramCategory.AddCategory(category.CategoryID, category.FullDescription);
            }

            return (true);
        }

        private bool validateEntry(Collection<OpenTVProgramCategory> addedList, OpenTVProgramCategory category)
        {
            foreach (OpenTVProgramCategory existingCategory in addedList)
            {
                if (existingCategory.CategoryID == category.CategoryID)
                {
                    MessageBox.Show("Category " + existingCategory.CategoryIDString + " already exists.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return (false);
                }
            }

            if (category.OpenTVDescription == null || category.OpenTVDescription == string.Empty)
            {
                MessageBox.Show("The OpenTV description for category " + category.CategoryIDString + " is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }

            bool validValue = DVBLogicProgramCategory.CheckDescription(category.DVBLogicDescription);
            if (!validValue)
            {
                MessageBox.Show("The DVBLogic description for category " + category.CategoryIDString + " is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }

            validValue = DVBViewerProgramCategory.CheckDescription(category.DVBViewerDescription);
            if (!validValue)
            {
                MessageBox.Show("The DVBViewer description for category " + category.CategoryIDString + " is incorrect.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }

            return (true);
        }

        /// <summary>
        /// Save the current data set to the original file.
        /// </summary>
        /// <param name="fileName">The name of the file to be saved.</param>
        /// <returns>True if the data has been saved; false otherwise.</returns>
        public bool Save()
        {
            FileInfo fileInfo = new FileInfo(currentFileName);
            return (Save(Path.Combine(RunParameters.DataDirectory, fileInfo.Name)));
        }

        /// <summary>
        /// Save the current data set to a specified file.
        /// </summary>
        /// <param name="fileName">The name of the file to be saved.</param>
        /// <returns>True if the data has been saved; false otherwise.</returns>
        public bool Save(string fileName)
        {
            string[] parts = fileName.Split(new char[] { Path.DirectorySeparatorChar });
            if (parts.Length < 2 || fileName != Path.Combine(RunParameters.DataDirectory, parts[parts.Length - 1]))
            {
                DialogResult result = MessageBox.Show("The data must be saved to the data directory.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false);
            }

            string message = OpenTVProgramCategory.Save(fileName);

            if (message == null)
            {
                MessageBox.Show("The OpenTV program categories have been saved to '" + fileName + "'", " EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Information);
                currentFileName = fileName;
            }
            else
                MessageBox.Show("An error has occurred while writing the OpenTV program categories." + Environment.NewLine + Environment.NewLine + message, "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return (message == null);
        }
    }
}
