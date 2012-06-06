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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

using DomainObjects;
using DirectShow;
using DVBServices;

namespace EPGCentre
{
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Get the full assembly version number.
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return (version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision);
            }
        }

        private CollectorParametersControl collectorParametersControl;
        private DVBLogicParametersControl pluginParametersControl;        
        private ChangeProgramContentControl changeProgramContentControl;
        private ChangeProgramCategoryControl changeProgramCategoryControl;
        private ChangeMHWCategoriesControl changeMHWCategoriesControl;
        private ChangePSIPCategoriesControl changePSIPCategoriesControl;
        private ChangeDishNetworkCategoriesControl changeDishNetworkCategoriesControl;
        private ChangeBellTVCategoriesControl changeBellTVCategoriesControl;
        private ChangeCustomCategoriesControl changeCustomCategoriesControl;

        private ViewLogControl viewLogControl;
        private OutputFileUnformattedControl outputFileUnformattedControl;
        private RunCollectionControl runCollectionControl;
        private FindEPGControl findEPGControl;
        private TransportStreamDumpControl tsDumpControl;
        private TransportStreamAnalyzeControl tsAnalyzeControl;

        private IUpdateControl currentControl;
        private string collectionParameters;

        private IView viewControl;

        private static MainWindow mainWindow;

        private bool updateChecksDone;

        public MainWindow()
        {
            InitializeComponent();

            Logger.Instance.WriteSeparator("EPG Centre (Version " + RunParameters.SystemVersion + ")");

            Logger.Instance.Write("Executable build: " + AssemblyVersion);
            Logger.Instance.Write("DirectShow build: " + DirectShowGraph.AssemblyVersion);
            Logger.Instance.Write("DomainObjects build: " + RunParameters.AssemblyVersion);
            Logger.Instance.Write("DVBServices build: " + Utils.AssemblyVersion);
            Logger.Instance.Write("");
            Logger.Instance.Write("Privilege level: " + RunParameters.Role);
            Logger.Instance.Write("");
            Logger.Instance.Write("Base directory: " + RunParameters.BaseDirectory);
            Logger.Instance.Write("Data directory: " + RunParameters.DataDirectory);
            Logger.Instance.Write("");

            mainWindow = this;

            BDAGraph.LoadTuners();
            if (Tuner.TunerCollection.Count == 0)
            {
                MessageBox.Show("There are no tuners installed on this machine.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(10);
            }

            collectorLogToolStripMenuItem_Click(null, null);

            Logger.Instance.Write("EPG Centre is loaded");
        }

        /// <summary>
        /// Change the availability of the main menu items.
        /// </summary>
        /// <param name="availability">True to make the items available; false otherwise.</param>
        public static void ChangeMenuItemAvailability(bool availability)
        {
            Logger.Instance.Write("Changing menu availability to " + availability);

            mainWindow.fileToolStripMenuItem.Enabled = availability;
            mainWindow.viewToolStripMenuItem.Enabled = availability;
            mainWindow.windowToolStripMenuItem.Enabled = availability;
            mainWindow.runToolStripMenuItem.Enabled = availability;

            foreach (ToolStripItem item in mainWindow.toolStrip.Items)
                item.Enabled = availability;

            if (availability)
            {
                mainWindow.changeSaveAvailability(mainWindow.saveToolStripMenuItem.Enabled, mainWindow.saveAsToolStripMenuItem.Enabled);
                mainWindow.changeFindFilterAvailability(mainWindow.filterTextToolStripMenuItem.Enabled);
            }
        }

        /// <summary>
        /// Change the availability of find and filter .
        /// </summary>
        /// <param name="availability">True to make the items available; false otherwise.</param>
        public static void ChangeFindFilterAvailability(bool availability)
        {
            Logger.Instance.Write("Changing Find/Filter availability to " + availability);
            mainWindow.changeFindFilterAvailability(availability);
        }

        private void createNewCollectorMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Create collection parameters menu option selected");

            bool proceed = checkSaveFile(collectorParametersControl, "Collection Parameters");
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            SelectTask selectTask = new SelectTask(false);
            DialogResult result = selectTask.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                Logger.Instance.Write("Select task aborted by user");
                return;
            }

            if (collectorParametersControl == null)
            {
                collectorParametersControl = new CollectorParametersControl();
                positionControl(collectorParametersControl);
            }

            if (selectTask.Task != null)
            {
                this.Text = collectorParametersControl.Heading + selectTask.Task;
                Logger.Instance.Write("Creating parameters using task '" + selectTask.Task + "'");
            }
            else
            {
                this.Text = collectorParametersControl.Heading + "Custom Parameters";
                Logger.Instance.Write("Creating custom parameters");
            }

            collectorParametersControl.Tag = new ControlStatus(this.Text);            

            saveAsToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = false;

            if (selectTask.Task != null)
            {
                string fileName = Path.Combine(RunParameters.BaseDirectory, Path.Combine("Samples", Path.Combine("Collector", selectTask.Task))) + ".ini";                
                collectionParameters = fileName;
                collectorParametersControl.Process(fileName, true);
            }
            else
                collectorParametersControl.Process();

            changeSaveAvailability(false, true);
            changeFindFilterAvailability(false);
            
            hideAllControls(collectorParametersControl);
            currentControl = collectorParametersControl;

            bar1ToolStripMenuItem.Visible = true;
            collectorParametersToolStripMenuItem.Visible = true;
        }

        private void openCollectorParametersMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Update collection parameters menu option selected");

            bool proceed = checkSaveFile(collectorParametersControl, "Collection Parameters");
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "INI Files (*.ini)|*.ini";
            openFile.InitialDirectory = RunParameters.DataDirectory;
            openFile.RestoreDirectory = true;
            openFile.Title = "Open EPG Collection Parameter File";
            
            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                Logger.Instance.Write("Select parameters aborted by user");
                return;
            }

            Logger.Instance.Write("Updating collection parameter file - " + openFile.FileName);

            if (collectorParametersControl == null)
            {
                collectorParametersControl = new CollectorParametersControl();
                positionControl(collectorParametersControl);
            }

            this.Text = collectorParametersControl.Heading + openFile.FileName;
            collectorParametersControl.Tag = new ControlStatus(this.Text);

            collectionParameters = openFile.FileName;
            
            collectorParametersControl.Process(openFile.FileName, false);

            changeSaveAvailability(true, true);
            changeFindFilterAvailability(false); 

            hideAllControls(collectorParametersControl);
            currentControl = collectorParametersControl;

            bar1ToolStripMenuItem.Visible = true;
            collectorParametersToolStripMenuItem.Visible = true;
        }

        private void pluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Create plugin parameters menu option selected");

            bool proceed = checkSaveFile(pluginParametersControl, "Plugin Parameters");
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            SelectTask selectTask = new SelectTask(true);
            DialogResult result = selectTask.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                Logger.Instance.Write("Select task aborted by user");
                return;
            }

            if (pluginParametersControl == null)
            {
                pluginParametersControl = new DVBLogicParametersControl();
                positionControl(pluginParametersControl);
            }

            if (selectTask.Task != null)
            {
                this.Text = pluginParametersControl.Heading + selectTask.Task;
                Logger.Instance.Write("Creating parameters using task '" + selectTask.Task + "'");
            }
            else
            {
                this.Text = pluginParametersControl.Heading + "Custom Parameters";
                Logger.Instance.Write("Creating custom parameters");
            }

            pluginParametersControl.Tag = new ControlStatus(this.Text);

            saveAsToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = false;

            if (selectTask.Task != null)
            {
                string fileName = Path.Combine(RunParameters.BaseDirectory, Path.Combine("Samples", Path.Combine("DVBLogic Plugin", selectTask.Task))) + ".ini";
                collectionParameters = fileName;
                pluginParametersControl.Process(fileName, true);
            }
            else
                pluginParametersControl.Process();
            
            changeSaveAvailability(false, true);
            changeFindFilterAvailability(false);

            hideAllControls(pluginParametersControl);
            currentControl = pluginParametersControl;

            bar1ToolStripMenuItem.Visible = true;
            pluginParametersToolStripMenuItem.Visible = true;
        }

        private void openPluginParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Update plugin parameters menu option selected");

            bool proceed = checkSaveFile(pluginParametersControl, "Plugin Parameters");
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "INI Files (*.ini)|*.ini";
            openFile.RestoreDirectory = true;
            openFile.Title = "Open EPG Plugin Parameter File";

            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                Logger.Instance.Write("Select parameters aborted by user");
                return;
            }

            Logger.Instance.Write("Updating plugin parameter file - " + openFile.FileName);

            if (pluginParametersControl == null)
            {
                pluginParametersControl = new DVBLogicParametersControl();
                positionControl(pluginParametersControl);
            }

            this.Text = pluginParametersControl.Heading + " - " + openFile.FileName;
            pluginParametersControl.Tag = new ControlStatus(this.Text);

            collectionParameters = openFile.FileName;
            
            pluginParametersControl.Process(openFile.FileName, false);

            changeSaveAvailability(true, true);
            changeFindFilterAvailability(false);

            hideAllControls(pluginParametersControl);
            currentControl = pluginParametersControl;

            bar1ToolStripMenuItem.Visible = true;
            pluginParametersToolStripMenuItem.Visible = true;
        }

        private void updateDVBLogicPluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Update DVBLogic Plugin menu option selected");

            FolderBrowserDialog browsePath = new FolderBrowserDialog();
            browsePath.Description = "EPG Centre - Find DVBLogic EPG Directory";
            DialogResult result = browsePath.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                Logger.Instance.Write("Cancelled by user");
                return;
            }

            if (!browsePath.SelectedPath.ToUpper().EndsWith(Path.DirectorySeparatorChar + "EPG"))
            {
                Logger.Instance.Write("Querying path selected.");
                result = MessageBox.Show("The path selected does not reference an 'EPG' directory." + Environment.NewLine + Environment.NewLine +
                    "Is the path correct?", "EPG Centre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    Logger.Instance.Write("Cancelled by user - path incorrect");
                    return;
                }
            }

            string pluginPath = Path.Combine(browsePath.SelectedPath, "DVBLogicCPPPlugin.dll");

            if (File.Exists(Path.Combine(browsePath.SelectedPath, "DVBLogicCPPPlugin.dll")))
            {
                Logger.Instance.Write("Existing plugin located");

                DateTime newWriteTime = File.GetLastWriteTime(Path.Combine(RunParameters.BaseDirectory, "DVBLogicCPPPlugin.dll"));
                DateTime existingWriteTime = File.GetLastWriteTime(pluginPath);

                if (newWriteTime <= existingWriteTime)
                {
                    Logger.Instance.Write("Latest version is installed - " + newWriteTime + ":" + existingWriteTime);
                    result = MessageBox.Show("The plugin software is up to date." + Environment.NewLine + Environment.NewLine +
                    "Do you still want to update it?", "EPG Centre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                    {
                        Logger.Instance.Write("Cancelled by user - existing plugin up to date");
                        return;
                    }
                }
                else
                    Logger.Instance.Write("Newer version available to install");
            }

            try
            {
                File.Copy(Path.Combine(RunParameters.BaseDirectory, "DVBLogicCPPPlugin.dll"), pluginPath, true);
                Logger.Instance.Write("Plugin installed - version now " + File.GetLastWriteTime(pluginPath));
                MessageBox.Show("The plugin module has been updated.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException e1)
            {
                Logger.Instance.Write("<e> Plugin install failed - " + e1.Message);
                MessageBox.Show("The plugin could not be updated." + Environment.NewLine + Environment.NewLine + e1.Message,
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            catch (UnauthorizedAccessException e2)
            {
                Logger.Instance.Write("<e> Plugin install failed - " + e2.Message);
                MessageBox.Show("The plugin could not be updated." + Environment.NewLine + Environment.NewLine + e2.Message,
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string locationPath = Path.Combine(browsePath.SelectedPath, "EPG Collector Gateway.cfg");

                if (File.Exists(locationPath))
                {
                    File.SetAttributes(locationPath, FileAttributes.Normal);
                    File.Delete(locationPath);                    
                }

                FileStream fileStream = new FileStream(locationPath, FileMode.Create, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fileStream);

                streamWriter.WriteLine("Location=" + Path.Combine(RunParameters.BaseDirectory, "DVBLogicPlugin.dll"));

                streamWriter.Close();
                fileStream.Close();

                File.SetAttributes(locationPath, FileAttributes.ReadOnly);
                Logger.Instance.Write("Software location file written successfully");
                Logger.Instance.Write("Software location set to " + RunParameters.BaseDirectory);
            }
            catch (IOException e1)
            {
                Logger.Instance.Write("<e> The software location file could not be written - " + e1.Message);
                MessageBox.Show("The software location file could not be written." + Environment.NewLine + Environment.NewLine + e1.Message,
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (UnauthorizedAccessException e2)
            {
                Logger.Instance.Write("<e> The software location file could not be written - " + e2.Message);
                MessageBox.Show("The software location file could not be written." + Environment.NewLine + Environment.NewLine + e2.Message,
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void editProgramContentEITToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Update EIT Program Contents menu option selected");

            bool proceed = checkSaveFile(changeProgramContentControl, "EIT Program Categories");
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            if (changeProgramContentControl == null)
            {
                changeProgramContentControl = new ChangeProgramContentControl();
                positionControl(changeProgramContentControl);
            }

            this.Text = changeProgramContentControl.Heading + EITProgramContent.FileName + ".cfg";
            changeProgramContentControl.Tag = new ControlStatus(this.Text);

            string fullPath = Path.Combine(RunParameters.DataDirectory, EITProgramContent.FileName + ".cfg");
            if (!File.Exists(fullPath))
                fullPath = Path.Combine(RunParameters.ConfigDirectory, Path.Combine("Program Categories", EITProgramContent.FileName + ".cfg"));

            Cursor.Current = Cursors.WaitCursor;
            changeProgramContentControl.Process(fullPath);
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false); 

            hideAllControls(changeProgramContentControl);
            currentControl = changeProgramContentControl;

            bar1ToolStripMenuItem.Visible = true;
            programContentsToolStripMenuItem.Visible = true;
        }

        private void editProgramCategoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Update OpenTV Program Categories menu option selected");

            bool proceed = checkSaveFile(changeProgramCategoryControl, "OpenTV Program Categories");
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            SelectCategoryFile selectCategoryFile = new SelectCategoryFile();
            DialogResult result = selectCategoryFile.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                Logger.Instance.Write("Select categories aborted by user");
                return;
            }

            Logger.Instance.Write("Updating OpenTV Program Categories file - " + selectCategoryFile.File);

            if (changeProgramCategoryControl == null)
            {
                changeProgramCategoryControl = new ChangeProgramCategoryControl();
                positionControl(changeProgramCategoryControl);
            }

            this.Text = changeProgramCategoryControl.Heading + selectCategoryFile.File;
            changeProgramCategoryControl.Tag = new ControlStatus(this.Text);

            string fullPath = Path.Combine(RunParameters.DataDirectory, selectCategoryFile.File);
            if (!File.Exists(fullPath))
                fullPath = Path.Combine(RunParameters.ConfigDirectory, Path.Combine("Program Categories", selectCategoryFile.File));

            Cursor.Current = Cursors.WaitCursor;
            changeProgramCategoryControl.Process(fullPath);
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false);

            hideAllControls(changeProgramCategoryControl);
            currentControl = changeProgramCategoryControl;

            bar1ToolStripMenuItem.Visible = true;
            programCategoriesToolStripMenuItem.Visible = true;
        }

        private void changeMediaHighwayProgramCategoryDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Update MHW Program Categories menu option selected");

            bool proceed = checkSaveFile(changeMHWCategoriesControl, "MediaHighway Program Categories");
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            SelectMHWCategoryFile selectCategoryFile = new SelectMHWCategoryFile();
            DialogResult result = selectCategoryFile.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                Logger.Instance.Write("Select categories aborted by user");
                return;
            }

            string selectedFile;

            if (selectCategoryFile.File[0] == ' ')
                selectedFile = "New File";
            else
                selectedFile = selectCategoryFile.File;

            Logger.Instance.Write("Updating MHW Program Categories file - " + selectedFile);

            if (changeMHWCategoriesControl == null)
            {
                changeMHWCategoriesControl = new ChangeMHWCategoriesControl();
                positionControl(changeMHWCategoriesControl);
            }

            this.Text = changeMHWCategoriesControl.Heading + selectedFile;
            changeMHWCategoriesControl.Tag = new ControlStatus(this.Text);

            string fullPath;
            
            if (selectedFile != "New File")
            {
                fullPath = Path.Combine(RunParameters.DataDirectory, selectCategoryFile.File);
                if (!File.Exists(fullPath))
                    fullPath = Path.Combine(RunParameters.ConfigDirectory, Path.Combine("Program Categories", selectCategoryFile.File));
            }
            else
                fullPath = null;

            Cursor.Current = Cursors.WaitCursor;
            changeMHWCategoriesControl.Process(fullPath);
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(selectedFile != "New File", selectedFile == "New File");
            changeFindFilterAvailability(false);

            hideAllControls(changeMHWCategoriesControl);
            currentControl = changeMHWCategoriesControl;

            bar1ToolStripMenuItem.Visible = true;
            mhwCategoriesToolStripMenuItem.Visible = true;
        }

        private void changePSIPProgramCategoryDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Update PSIP Program Contents menu option selected");

            bool proceed = checkSaveFile(changePSIPCategoriesControl, "PSIP Program Categories");
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            if (changePSIPCategoriesControl == null)
            {
                changePSIPCategoriesControl = new ChangePSIPCategoriesControl();
                positionControl(changePSIPCategoriesControl);
            }

            this.Text = changePSIPCategoriesControl.Heading + AtscPsipProgramCategory.FileName + ".cfg";
            changePSIPCategoriesControl.Tag = new ControlStatus(this.Text);

            string fullPath = Path.Combine(RunParameters.DataDirectory, AtscPsipProgramCategory.FileName + ".cfg");
            if (!File.Exists(fullPath))
                fullPath = Path.Combine(RunParameters.ConfigDirectory, Path.Combine("Program Categories", AtscPsipProgramCategory.FileName + ".cfg"));

            Cursor.Current = Cursors.WaitCursor;
            changePSIPCategoriesControl.Process(fullPath);
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false);

            hideAllControls(changePSIPCategoriesControl);
            currentControl = changePSIPCategoriesControl;

            bar1ToolStripMenuItem.Visible = true;
            psipCategoriesToolStripMenuItem.Visible = true;
        }

        private void changeDishNetworkProgramCategoryDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Update Dish Network Program Categories menu option selected");

            bool proceed = checkSaveFile(changeDishNetworkCategoriesControl, "Dish Network Program Categories");
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            if (changeDishNetworkCategoriesControl == null)
            {
                changeDishNetworkCategoriesControl = new ChangeDishNetworkCategoriesControl();
                positionControl(changeDishNetworkCategoriesControl);
            }

            this.Text = changeDishNetworkCategoriesControl.Heading + DishNetworkProgramCategory.FileName + ".cfg";
            changeDishNetworkCategoriesControl.Tag = new ControlStatus(this.Text);

            string fullPath = Path.Combine(RunParameters.DataDirectory, DishNetworkProgramCategory.FileName + ".cfg");
            if (!File.Exists(fullPath))
                fullPath = Path.Combine(RunParameters.ConfigDirectory, Path.Combine("Program Categories", DishNetworkProgramCategory.FileName + ".cfg"));

            Cursor.Current = Cursors.WaitCursor;
            changeDishNetworkCategoriesControl.Process(fullPath);
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false);

            hideAllControls(changeDishNetworkCategoriesControl);
            currentControl = changeDishNetworkCategoriesControl;

            bar1ToolStripMenuItem.Visible = true;
            dishNetworkCategoriesToolStripMenuItem.Visible = true;
        }

        private void changeBellTVProgramCategoryDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Update Bell TV Program Categories menu option selected");

            bool proceed = checkSaveFile(changeBellTVCategoriesControl, "Bell TV Program Categories");
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            if (changeBellTVCategoriesControl == null)
            {
                changeBellTVCategoriesControl = new ChangeBellTVCategoriesControl();
                positionControl(changeBellTVCategoriesControl);
            }

            this.Text = changeBellTVCategoriesControl.Heading + BellTVProgramCategory.FileName + ".cfg";
            changeBellTVCategoriesControl.Tag = new ControlStatus(this.Text);

            string fullPath = Path.Combine(RunParameters.DataDirectory, BellTVProgramCategory.FileName + ".cfg");
            if (!File.Exists(fullPath))
                fullPath = Path.Combine(RunParameters.ConfigDirectory, Path.Combine("Program Categories", BellTVProgramCategory.FileName + ".cfg"));

            Cursor.Current = Cursors.WaitCursor;
            changeBellTVCategoriesControl.Process(fullPath);
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false);

            hideAllControls(changeBellTVCategoriesControl);
            currentControl = changeBellTVCategoriesControl;

            bar1ToolStripMenuItem.Visible = true;
            bellTVCategoriesToolStripMenuItem.Visible = true;
        }

        private void changeCustomProgramCategoryDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Update Custom Program Contents menu option selected");

            bool proceed = checkSaveFile(changeCustomCategoriesControl, "Custom Program Categories");
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            if (changeCustomCategoriesControl == null)
            {
                changeCustomCategoriesControl = new ChangeCustomCategoriesControl();
                positionControl(changeCustomCategoriesControl);
            }

            this.Text = changeCustomCategoriesControl.Heading + CustomProgramCategory.FileName + ".cfg";
            changeCustomCategoriesControl.Tag = new ControlStatus(this.Text);

            string fullPath = Path.Combine(RunParameters.DataDirectory, CustomProgramCategory.FileName + ".cfg");
            if (!File.Exists(fullPath))
            {
                fullPath = Path.Combine(RunParameters.ConfigDirectory, Path.Combine("Program Categories", CustomProgramCategory.FileName + ".cfg"));
                if (!File.Exists(fullPath))
                    fullPath = null;
            }

            Cursor.Current = Cursors.WaitCursor;
            changeCustomCategoriesControl.Process(fullPath);
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false);

            hideAllControls(changeCustomCategoriesControl);
            currentControl = changeCustomCategoriesControl;

            bar1ToolStripMenuItem.Visible = true;
            customCategoriesToolStripMenuItem.Visible = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Save menu option selected");

            bool result = currentControl.PrepareToSave();
            if (!result)
            {
                Logger.Instance.Write("Aborted due to incorrect data");
                return;
            }

            currentControl.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Save As menu option selected");

            bool prepareResult = currentControl.PrepareToSave();
            if (!prepareResult)
            {
                Logger.Instance.Write("Aborted due to incorrect data");
                return;
            }

            string actualName;

            if (currentControl as ChangeMHWCategoriesControl != null)
            {
                SaveMHWCategoryFile saveMHWCategoryFile = new SaveMHWCategoryFile();
                DialogResult result = saveMHWCategoryFile.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    Logger.Instance.Write("Specify saved file aborted by user");
                    return;
                }
                else
                    actualName = Path.Combine(RunParameters.DataDirectory, "MHW" +  saveMHWCategoryFile.SelectedType + " Categories " + saveMHWCategoryFile.SelectedFrequency + ".cfg");
            }
            else
            {
                if (currentControl as DVBLogicParametersControl == null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = currentControl.SaveFileFilter;
                    saveFileDialog.Title = currentControl.SaveFileTitle;
                    saveFileDialog.InitialDirectory = currentControl.DefaultDirectory;
                    saveFileDialog.FileName = currentControl.DefaultFileName;
                    saveFileDialog.OverwritePrompt = false;
                    if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                    {
                        Logger.Instance.Write("Specify saved file aborted by user");
                        return;
                    }

                    if (saveFileDialog.FileName.Trim().EndsWith(currentControl.SaveFileSuffix))
                        actualName = saveFileDialog.FileName.Trim();
                    else
                        actualName = saveFileDialog.FileName.Trim() + currentControl.SaveFileSuffix;
                }
                else
                {
                    FolderBrowserDialog browsePath = new FolderBrowserDialog();
                    browsePath.Description = "EPG Centre - Find DVBLogic EPG Directory";
                    DialogResult result = browsePath.ShowDialog();
                    if (result == DialogResult.Cancel)
                    {
                        Logger.Instance.Write("Cancelled by user");
                        return;
                    }

                    actualName = Path.Combine(browsePath.SelectedPath, currentControl.DefaultFileName) + "." + currentControl.SaveFileSuffix;
                }
            }

            Logger.Instance.Write("Save file selected as " + actualName);

            if (File.Exists(actualName))
            {
                Logger.Instance.Write("File exists - asking for authority to overwrite");
                DialogResult questionResult = MessageBox.Show("The file '" + actualName + "' already exists." + Environment.NewLine + Environment.NewLine +
                    "Do you want to overwrite it?",
                    "EPG Centre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (questionResult == DialogResult.No)
                {
                    Logger.Instance.Write("User refused permission to overwrite - save aborted");
                    return;
                }
                else
                    Logger.Instance.Write("User authorised overwrite");
            }

            bool reply = currentControl.Save(actualName);
            if (!reply)
                return;

            if (currentControl as CollectorParametersControl != null)
            {
                collectionParameters = actualName;
                this.Text = currentControl.Heading + actualName;
            }
            else
            {
                if (currentControl as DVBLogicParametersControl != null)
                {
                    collectionParameters = actualName;
                    this.Text = currentControl.Heading + " - " + actualName;
                }
                else
                {
                    string[] nameParts = actualName.Split(new char[] { '\\' });
                    this.Text = currentControl.Heading + nameParts[nameParts.Length - 1];
                }
            }

            changeSaveAvailability(true, true);
        }

        private void clearCollectorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Clear General Log menu option selected");

            DialogResult result = MessageBox.Show("Please confirm that the General log is to be cleared.", 
                "EPG Centre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            string reply = Logger.Clear();

            if (reply != null)
            {
                Logger.Instance.Write("Log could not be cleared");
                Logger.Instance.Write(reply);
                MessageBox.Show("The log could not be cleared because an error occured." + Environment.NewLine + Environment.NewLine + reply,
                    "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Instance.Write("The general log has been cleared.");
            MessageBox.Show("The General log has been cleared.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            if (viewLogControl != null)
                viewLogControl.Clear();    
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Exit menu option selected");

            bool proceed = checkSaveAllFiles();
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            updateChecksDone = true;

            Logger.Instance.Write("EPG Centre closing down");
            this.Close();
        }

        private void collectorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("View general log menu option selected");

            Logger logger = new Logger(true);
            bool opened = logger.Open();
            if (!opened)
            {
                Logger.Instance.Write("Failed to open general log file - view general log aborted");
                MessageBox.Show("The log file is not available.", "EPG Centre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (viewLogControl == null)
            {
                viewLogControl = new ViewLogControl();
                positionControl(viewLogControl);
            }

            this.Text = "EPG Centre - View General Log";
            viewLogControl.Tag = new ControlStatus(this.Text);

            Cursor.Current = Cursors.WaitCursor;
            viewLogControl.Process(null, logger, true);
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(viewLogControl.FindFilterAvailability);  

            hideAllControls(viewLogControl);
            viewControl = viewLogControl;
        }

        private void otherLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("View other log menu option selected");

            SelectLog selectLog = new SelectLog();
            DialogResult result = selectLog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            string actualName = Path.Combine(RunParameters.DataDirectory, selectLog.File) + ".log";

            Logger.Instance.Write("Log file selected as " + actualName);

            Logger logger = new Logger(true);

            bool opened = logger.Open(actualName);
            if (!opened)
            {
                Logger.Instance.Write("Failed to open log file - view other log aborted");
                MessageBox.Show("The log file '" + actualName + "' is not available.", "EPG Collector", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (viewLogControl == null)
            {
                viewLogControl = new ViewLogControl();
                positionControl(viewLogControl);
            }

            this.Text = "EPG Centre - View Other Logs - " + selectLog.File;
            viewLogControl.Tag = new ControlStatus(this.Text);

            Cursor.Current = Cursors.WaitCursor;
            viewLogControl.Process(actualName, logger, false);
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(viewLogControl.FindFilterAvailability); 

            hideAllControls(viewLogControl);
            viewControl = viewLogControl;
        }

        private void outputFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("View output file unformatted menu option selected");

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "XMLTV Files (*.xml)|*.xml";
            openFile.RestoreDirectory = true;
            openFile.Title = "Open EPG Collection Output File";

            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            Logger.Instance.Write("File selected as " + openFile.FileName);

            if (outputFileUnformattedControl == null)
            {
                outputFileUnformattedControl = new OutputFileUnformattedControl();
                positionControl(outputFileUnformattedControl);
            }

            this.Text = "EPG Centre - View Output File Unformatted - " + openFile.FileName;
            outputFileUnformattedControl.Tag = new ControlStatus(this.Text);

            Cursor.Current = Cursors.WaitCursor;
            outputFileUnformattedControl.Process(openFile.FileName);
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(outputFileUnformattedControl.FindFilterAvailability); 

            hideAllControls(outputFileUnformattedControl);
            viewControl = outputFileUnformattedControl;

            bar2ToolStripMenuItem.Visible = true;
            outputFileUnformattedToolStripMenuItem.Visible = true;
        }

        private void outputFileFormattedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("View output file formatted menu option selected");

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Output Files (*.xml)|*.xml";
            openFile.RestoreDirectory = true;
            openFile.Title = "Open EPG Collection Output File";

            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            Logger.Instance.Write("File selected as " + openFile.FileName);

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(true); 



            bar2ToolStripMenuItem.Visible = true;
            outputFileFormattedToolStripMenuItem.Visible = true;
        }

        private void findTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Find text menu option selected");

            if (viewControl == null)
            {
                Logger.Instance.Write("No view control available - aborted");
                return;
            }

            viewControl.FindText();
        }

        private void filterTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Filter text menu option selected");

            if (viewControl == null)
            {
                Logger.Instance.Write("No view control available - aborted");
                return;
            }

            viewControl.FilterText();
        }

        private void collectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Run collection menu option selected");

            bool proceed = checkSaveAllFiles();
            if (!proceed)
            {
                Logger.Instance.Write("Aborted by user");
                return;
            }

            string fileName;

            if (collectionParameters != null)
            {
                Logger.Instance.Write("Asking if current parameters are to be used");
                DialogResult result = MessageBox.Show("Do you want to use the parameters currently loaded to run the collection?", "EPG Centre", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                {
                    Logger.Instance.Write("Aborted by user");
                    return;
                }

                if (result == DialogResult.No)
                {
                    Logger.Instance.Write("User has requested to use different parameters");
                    fileName = getParameterFile("Collecting EPG");
                    if (fileName == null)
                    {
                        Logger.Instance.Write("Aborted by user");
                        return;
                    }
                }
                else
                {
                    Logger.Instance.Write("Using current parameters");
                    fileName = collectionParameters;
                }
            }
            else
            {
                Logger.Instance.Write("No current parameters - getting parameter file");
                fileName = getParameterFile("Collecting EPG");
                if (fileName == null)
                {
                    Logger.Instance.Write("Aborted by user");
                    return;
                }
            }

            if (runCollectionControl == null)
            {
                runCollectionControl = new RunCollectionControl();
                positionControl(runCollectionControl);
            }

            Logger.Instance.Write("Running collection with " + fileName);

            this.Text = "EPG Centre - Collect EPG using " + fileName;
            runCollectionControl.Tag = new ControlStatus(this.Text);

            runCollectionControl.Process(fileName);

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(false); 

            hideAllControls(runCollectionControl);

            bar3ToolStripMenuItem.Visible = true;
            runCollectorToolStripMenuItem.Visible = true;
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Find EPG menu option selected");

            if (findEPGControl == null)
            {
                findEPGControl = new FindEPGControl();
                positionControl(findEPGControl);
            }

            this.Text = findEPGControl.Heading;
            findEPGControl.Tag = new ControlStatus(this.Text);

            Cursor.Current = Cursors.WaitCursor;
            findEPGControl.Process();
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(false);

            hideAllControls(findEPGControl);
            currentControl = findEPGControl;

            bar3ToolStripMenuItem.Visible = true;
            findEPGToolStripMenuItem.Visible = true;
        }

        private void dumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Dump Transport Stream menu option selected");

            if (tsDumpControl == null)
            {
                tsDumpControl = new TransportStreamDumpControl();
                positionControl(tsDumpControl);
            }

            this.Text = tsDumpControl.Heading;
            tsDumpControl.Tag = new ControlStatus(this.Text);

            Cursor.Current = Cursors.WaitCursor;
            tsDumpControl.Process();
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(false);

            hideAllControls(tsDumpControl);
            currentControl = tsDumpControl;

            bar4ToolStripMenuItem.Visible = true;
            dumpTransportStreamToolStripMenuItem.Visible = true;
        }

        private void analyzeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Analyze Transport Stream menu option selected");

            if (tsAnalyzeControl == null)
            {
                tsAnalyzeControl = new TransportStreamAnalyzeControl();
                positionControl(tsAnalyzeControl);
            }

            this.Text = tsAnalyzeControl.Heading;
            tsAnalyzeControl.Tag = new ControlStatus(this.Text);

            Cursor.Current = Cursors.WaitCursor;
            tsAnalyzeControl.Process();
            Cursor.Current = Cursors.Arrow;

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(false);

            hideAllControls(tsAnalyzeControl);
            currentControl = tsAnalyzeControl;

            bar4ToolStripMenuItem.Visible = true;
            analyzeTransportStreamToolStripMenuItem.Visible = true;
        }

        private void generalHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("General help menu option selected");

            Process process = new Process();

            string fileName = Path.Combine(RunParameters.BaseDirectory, "EPG Collector.chm");

            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = fileName;
            try
            {
                process.Start();
                Logger.Instance.Write("Help process started with file " + fileName);
            }
            catch (Exception ex)
            {
                Logger.Instance.Write("Help process failed to start for file " + fileName);
                Logger.Instance.Write(ex.Message);
                MessageBox.Show("Unable to open " + fileName + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        private void aboutEPGCollectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("About menu option selected");

            About about = new About();
            about.ShowDialog();
        }

        private void logViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Log view");

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(viewLogControl.FindFilterAvailability);            

            hideAllControls(viewLogControl);
            viewControl = viewLogControl;

            this.Text = (viewLogControl.Tag as ControlStatus).Heading;
        }

        private void collectorParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Collection Parameters view");

            changeSaveAvailability(!collectorParametersControl.NewFile, true);
            changeFindFilterAvailability(false); 

            hideAllControls(collectorParametersControl);
            currentControl = collectorParametersControl;

            this.Text = (collectorParametersControl.Tag as ControlStatus).Heading;
        }

        private void pluginParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Plugin Parameters view");

            changeSaveAvailability(!pluginParametersControl.NewFile, true);
            changeFindFilterAvailability(false);

            hideAllControls(pluginParametersControl);
            currentControl = pluginParametersControl;

            this.Text = (pluginParametersControl.Tag as ControlStatus).Heading;
        }

        private void programContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to EIT Program Categories view");

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false); 

            hideAllControls(changeProgramContentControl);
            currentControl = changeProgramContentControl;

            this.Text = (changeProgramContentControl.Tag as ControlStatus).Heading;
        }

        private void programCategoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to OpenTV Program Categories view");

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false);

            hideAllControls(changeProgramCategoryControl);
            currentControl = changeProgramCategoryControl;

            this.Text = (changeProgramCategoryControl.Tag as ControlStatus).Heading;
        }

        private void mediaHighwayCategoryDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to MediaHighway Program Categories view");

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false);

            hideAllControls(changeMHWCategoriesControl);
            currentControl = changeMHWCategoriesControl;

            this.Text = (changeMHWCategoriesControl.Tag as ControlStatus).Heading;
        }

        private void psipCategoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to PSIP Program Categories view");

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false);

            hideAllControls(changePSIPCategoriesControl);
            currentControl = changePSIPCategoriesControl;

            this.Text = (changePSIPCategoriesControl.Tag as ControlStatus).Heading;
        }

        private void dishNetworkCategoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Dish Network Program Categories view");

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false);

            hideAllControls(changeDishNetworkCategoriesControl);
            currentControl = changeDishNetworkCategoriesControl;

            this.Text = (changeDishNetworkCategoriesControl.Tag as ControlStatus).Heading;
        }

        private void bellTVCategoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Bell TV Program Categories view");

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false);

            hideAllControls(changeBellTVCategoriesControl);
            currentControl = changeBellTVCategoriesControl;

            this.Text = (changeBellTVCategoriesControl.Tag as ControlStatus).Heading;
        }   

        private void customCategoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Custom Program Categories view");

            changeSaveAvailability(true, false);
            changeFindFilterAvailability(false);

            hideAllControls(changeCustomCategoriesControl);
            currentControl = changeCustomCategoriesControl;

            this.Text = (changeCustomCategoriesControl.Tag as ControlStatus).Heading;
        }

        private void outputFileUnformattedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Output File Unformatted view");

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(outputFileUnformattedControl.FindFilterAvailability); 

            hideAllControls(outputFileUnformattedControl);
            viewControl = outputFileUnformattedControl;

            this.Text = (outputFileUnformattedControl.Tag as ControlStatus).Heading;
        }

        private void outputFileFormattedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Output File Formatted view");

        }

        private void runCollectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Run Collection view");

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(false); 

            hideAllControls(runCollectionControl);

            this.Text = (runCollectionControl.Tag as ControlStatus).Heading;
        }

        private void findEPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Find EPG view");

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(false); 

            hideAllControls(findEPGControl);

            this.Text = (findEPGControl.Tag as ControlStatus).Heading;
        }

        private void tsDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Transport Stream Dump view");

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(false);

            hideAllControls(tsDumpControl);

            this.Text = (tsDumpControl.Tag as ControlStatus).Heading;
        }

        private void tsAnalyzeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("Changing to Transport Stream Analysis view");

            changeSaveAvailability(false, false);
            changeFindFilterAvailability(false);

            hideAllControls(tsAnalyzeControl);

            this.Text = (tsAnalyzeControl.Tag as ControlStatus).Heading;
        } 

        private void sizeChanged(object sender, EventArgs e)
        {
            if (collectorParametersControl != null)
                positionControl(collectorParametersControl);

            if (pluginParametersControl != null)
                positionControl(pluginParametersControl);            

            if (changeProgramContentControl != null)
                positionControl(changeProgramContentControl);

            if (changeProgramCategoryControl != null)
                positionControl(changeProgramCategoryControl);

            if (changeMHWCategoriesControl != null)
                positionControl(changeMHWCategoriesControl);

            if (changePSIPCategoriesControl != null)
                positionControl(changePSIPCategoriesControl);

            if (changeDishNetworkCategoriesControl != null)
                positionControl(changeDishNetworkCategoriesControl);

            if (changeBellTVCategoriesControl != null)
                positionControl(changeBellTVCategoriesControl);

            if (changeCustomCategoriesControl != null)
                positionControl(changeCustomCategoriesControl);

            if (viewLogControl != null)
                positionControl(viewLogControl);

            if (outputFileUnformattedControl != null)
                positionControl(outputFileUnformattedControl);

            if (runCollectionControl != null)
                positionControl(runCollectionControl);

            if (findEPGControl != null)
                positionControl(findEPGControl);

            if (tsDumpControl != null)
                positionControl(tsDumpControl);

            if (tsAnalyzeControl != null)
                positionControl(tsAnalyzeControl);
        }

        private void positionControl(Control control)
        {
            control.Parent = this;
            control.Location = new Point(0, menuStrip1.Height + toolStrip.Height);
            control.Height = this.ClientSize.Height - (menuStrip1.Height + toolStrip.Height);
            control.Width = this.ClientSize.Width;            
        }

        private void hideAllControls(Control control)
        {
            if (collectorParametersControl != null && collectorParametersControl != control)
                collectorParametersControl.Visible = false;

            if (pluginParametersControl != null && pluginParametersControl != control)
                pluginParametersControl.Visible = false;

            if (changeProgramContentControl != null && changeProgramContentControl != control)
                changeProgramContentControl.Visible = false;

            if (changeProgramCategoryControl != null && changeProgramCategoryControl != control)
                changeProgramCategoryControl.Visible = false;

            if (changeMHWCategoriesControl != null && changeMHWCategoriesControl != control)
                changeMHWCategoriesControl.Visible = false;

            if (changePSIPCategoriesControl != null && changePSIPCategoriesControl != control)
                changePSIPCategoriesControl.Visible = false;

            if (changeDishNetworkCategoriesControl != null && changeDishNetworkCategoriesControl != control)
                changeDishNetworkCategoriesControl.Visible = false;

            if (changeBellTVCategoriesControl != null && changeBellTVCategoriesControl != control)
                changeBellTVCategoriesControl.Visible = false;

            if (changeCustomCategoriesControl != null && changeCustomCategoriesControl != control)
                changeCustomCategoriesControl.Visible = false;

            if (viewLogControl != null && viewLogControl != control)
                viewLogControl.Visible = false;

            if (outputFileUnformattedControl != null && outputFileUnformattedControl != control)
                outputFileUnformattedControl.Visible = false;
            
            if (runCollectionControl != null && runCollectionControl != control)
                runCollectionControl.Visible = false;

            if (findEPGControl != null && findEPGControl != control)
                findEPGControl.Visible = false;

            if (tsDumpControl != null && tsDumpControl != control)
                tsDumpControl.Visible = false;

            if (tsAnalyzeControl != null && tsAnalyzeControl != control)
                tsAnalyzeControl.Visible = false;

            control.Visible = true;
        }

        private void changeSaveAvailability(bool saveAvailability, bool saveAsAvailability)
        {
            Logger.Instance.Write("Save availability changed to " + saveAvailability + ":" + saveAsAvailability);

            saveToolStripMenuItem.Enabled = saveAvailability;
            saveAsToolStripMenuItem.Enabled = saveAsAvailability;

            saveToolStripButton.Enabled = saveAvailability;
            saveAsToolStripButton.Enabled = saveAsAvailability;
        }

        private void changeFindFilterAvailability(bool availability)
        {
            findTextToolStripMenuItem.Enabled = availability;
            filterTextToolStripMenuItem.Enabled = availability;

            findTextToolStripButton.Enabled = availability;
            filterTextToolStripButton.Enabled = availability;
        }

        private bool checkSaveAllFiles()
        {
            bool proceed = checkSaveFile(collectorParametersControl, "Collection Parameters");
            if (!proceed)
                return(false);

            proceed = checkSaveFile(pluginParametersControl, "Plugin Parameters");
            if (!proceed)
                return (false);            

            proceed = checkSaveFile(changeProgramContentControl, "EIT Program Categories");
            if (!proceed)
                return(false);

            proceed = checkSaveFile(changeProgramCategoryControl, "OpenTV Program Categories");
            if (!proceed)
                return (false);

            proceed = checkSaveFile(changeMHWCategoriesControl, "MediaHighway Program Categories");
            if (!proceed)
                return (false);

            proceed = checkSaveFile(changePSIPCategoriesControl, "PSIP Program Categories");
            if (!proceed)
                return (false);

            proceed = checkSaveFile(changeDishNetworkCategoriesControl, "Dish Network Program Categories");
            if (!proceed)
                return (false);

            proceed = checkSaveFile(changeBellTVCategoriesControl, "Bell TV Program Categories");
            if (!proceed)
                return (false);

            proceed = checkSaveFile(changeCustomCategoriesControl, "Custom Program Categories");
            if (!proceed)
                return (false);

            return (true);
        }

        private bool checkSaveFile(IUpdateControl updateControl, string description)
        {
            Logger.Instance.Write("Checking if save needed for " + description);

            if (updateControl == null)
            {
                Logger.Instance.Write("No update control - ok to proceed");
                return (true);
            }

            switch (updateControl.DataState)
            {
                case DataState.HasErrors:
                    Logger.Instance.Write("Data errors - do not proceed");
                    return (false);
                case DataState.Changed:
                    Logger.Instance.Write("Data has changed - asking user for action");
                    DialogResult result = MessageBox.Show("Any changes to the " + description + " will be lost if they are not saved." + Environment.NewLine + Environment.NewLine +
                        "Do you want to cancel the current action and save them?", "EPG Centre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    Logger.Instance.Write(result == DialogResult.Yes ? "Do not proceed - save requested" : "OK to proceed - no save requested" );
                    return (result != DialogResult.Yes);
                case DataState.NotChanged:
                    Logger.Instance.Write("Data has not changed - ok to proceed");
                    return (true);
                default:
                    Logger.Instance.Write("Data state unknown - ok to proceed");
                    return (true);
            }
        }

        private string getParameterFile(string description)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "INI Files (*.ini)|*.ini";
            openFile.RestoreDirectory = true;
            openFile.Title = "Locate EPG Collection Parameter File For " + description;

            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.Cancel)
                return (null);
            else
                return (openFile.FileName);
        }

        private void formClosing(object sender, FormClosingEventArgs e)
        {
            if (updateChecksDone)
                return;

            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = !mainWindow.fileToolStripMenuItem.Enabled;

            if (!e.Cancel)
                e.Cancel = !checkSaveAllFiles();

            if (!e.Cancel)
                Logger.Instance.Write("EPG Centre closing down");
        }             
    }
}
