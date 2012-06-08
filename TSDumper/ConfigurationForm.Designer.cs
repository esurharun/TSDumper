namespace EPGCollectorGUI
{
    partial class ConfigurationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.cboCollectionType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cboFrequencies = new System.Windows.Forms.ComboBox();
            this.cmdNewOutputFile = new System.Windows.Forms.Button();
            this.txtOptions = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmdOpenINIFile = new System.Windows.Forms.Button();
            this.gbSatellite = new System.Windows.Forms.GroupBox();
            this.txtAzimuth = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtLNBSwitch = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLNBHigh = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkDefaultLNB = new System.Windows.Forms.CheckBox();
            this.txtLNBLow = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdScanTuners = new System.Windows.Forms.Button();
            this.cboTuners = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmdSaveGeneral = new System.Windows.Forms.Button();
            this.gpTimeouts = new System.Windows.Forms.GroupBox();
            this.txtScanRetries = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.chkDefaultTimeouts = new System.Windows.Forms.CheckBox();
            this.txtDataCollectionTimeout = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSignalLockTimeout = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rtbFrequencies = new System.Windows.Forms.RichTextBox();
            this.gpLocation = new System.Windows.Forms.GroupBox();
            this.cmdLocationApply = new System.Windows.Forms.Button();
            this.lblArea = new System.Windows.Forms.Label();
            this.cboArea = new System.Windows.Forms.ComboBox();
            this.lblCountry = new System.Windows.Forms.Label();
            this.cboCountry = new System.Windows.Forms.ComboBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtOutputFile = new System.Windows.Forms.TextBox();
            this.lblOutputFile = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbDVBT = new System.Windows.Forms.RadioButton();
            this.rbSatellite = new System.Windows.Forms.RadioButton();
            this.tabChannels = new System.Windows.Forms.TabPage();
            this.cmdStopScan = new System.Windows.Forms.Button();
            this.cmdClearScan = new System.Windows.Forms.Button();
            this.cmdSelectNone = new System.Windows.Forms.Button();
            this.cmdSelectAll = new System.Windows.Forms.Button();
            this.pgbScanning = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdSaveChannels = new System.Windows.Forms.Button();
            this.lstTVStationsFound = new System.Windows.Forms.CheckedListBox();
            this.lblScanning = new System.Windows.Forms.Label();
            this.lblFound = new System.Windows.Forms.Label();
            this.cmdScan = new System.Windows.Forms.Button();
            this.tabOptions = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.lblUSEIMAGE = new System.Windows.Forms.Label();
            this.chkUSEIMAGE = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblUSECHANNELID = new System.Windows.Forms.Label();
            this.chkUSECHANNELID = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblROUNDTIME = new System.Windows.Forms.Label();
            this.chkROUNDTIME = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblACCEPTBREAKS = new System.Windows.Forms.Label();
            this.chkACCEPTBREAKS = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblNOSIGLOCK = new System.Windows.Forms.Label();
            this.chkNOSIGLOCK = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readMeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.backgroundWorkerScanStations = new System.ComponentModel.BackgroundWorker();
            this.tmrProgress = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.gbSatellite.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gpTimeouts.SuspendLayout();
            this.gpLocation.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabChannels.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabChannels);
            this.tabControl1.Controls.Add(this.tabOptions);
            this.tabControl1.Location = new System.Drawing.Point(1, 52);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(549, 628);
            this.tabControl1.TabIndex = 5;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.label11);
            this.tabGeneral.Controls.Add(this.cboCollectionType);
            this.tabGeneral.Controls.Add(this.label9);
            this.tabGeneral.Controls.Add(this.cboFrequencies);
            this.tabGeneral.Controls.Add(this.cmdNewOutputFile);
            this.tabGeneral.Controls.Add(this.txtOptions);
            this.tabGeneral.Controls.Add(this.label8);
            this.tabGeneral.Controls.Add(this.cmdOpenINIFile);
            this.tabGeneral.Controls.Add(this.gbSatellite);
            this.tabGeneral.Controls.Add(this.groupBox2);
            this.tabGeneral.Controls.Add(this.cmdSaveGeneral);
            this.tabGeneral.Controls.Add(this.gpTimeouts);
            this.tabGeneral.Controls.Add(this.rtbFrequencies);
            this.tabGeneral.Controls.Add(this.gpLocation);
            this.tabGeneral.Controls.Add(this.btnBrowse);
            this.tabGeneral.Controls.Add(this.txtOutputFile);
            this.tabGeneral.Controls.Add(this.lblOutputFile);
            this.tabGeneral.Controls.Add(this.groupBox1);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(541, 602);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(306, 374);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(83, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Collection Type:";
            // 
            // cboCollectionType
            // 
            this.cboCollectionType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboCollectionType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCollectionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCollectionType.FormattingEnabled = true;
            this.cboCollectionType.Location = new System.Drawing.Point(395, 371);
            this.cboCollectionType.Name = "cboCollectionType";
            this.cboCollectionType.Size = new System.Drawing.Size(139, 21);
            this.cboCollectionType.TabIndex = 21;
            this.cboCollectionType.SelectedIndexChanged += new System.EventHandler(this.cboCollectionType_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 374);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(108, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Scanning Frequency:";
            // 
            // cboFrequencies
            // 
            this.cboFrequencies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFrequencies.FormattingEnabled = true;
            this.cboFrequencies.Location = new System.Drawing.Point(154, 371);
            this.cboFrequencies.Name = "cboFrequencies";
            this.cboFrequencies.Size = new System.Drawing.Size(139, 21);
            this.cboFrequencies.TabIndex = 19;
            this.cboFrequencies.SelectedIndexChanged += new System.EventHandler(this.cboFrequencies_SelectedIndexChanged);
            // 
            // cmdNewOutputFile
            // 
            this.cmdNewOutputFile.Location = new System.Drawing.Point(467, 336);
            this.cmdNewOutputFile.Name = "cmdNewOutputFile";
            this.cmdNewOutputFile.Size = new System.Drawing.Size(61, 20);
            this.cmdNewOutputFile.TabIndex = 16;
            this.cmdNewOutputFile.Text = "Create...";
            this.cmdNewOutputFile.UseVisualStyleBackColor = true;
            this.cmdNewOutputFile.Click += new System.EventHandler(this.cmdNewOutputFile_Click);
            // 
            // txtOptions
            // 
            this.txtOptions.Enabled = false;
            this.txtOptions.Location = new System.Drawing.Point(154, 407);
            this.txtOptions.Name = "txtOptions";
            this.txtOptions.Size = new System.Drawing.Size(380, 20);
            this.txtOptions.TabIndex = 15;
            this.txtOptions.TextChanged += new System.EventHandler(this.txtOptions_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 410);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(146, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Advanced Options (see Tab):";
            // 
            // cmdOpenINIFile
            // 
            this.cmdOpenINIFile.Enabled = false;
            this.cmdOpenINIFile.Location = new System.Drawing.Point(16, 483);
            this.cmdOpenINIFile.Name = "cmdOpenINIFile";
            this.cmdOpenINIFile.Size = new System.Drawing.Size(83, 25);
            this.cmdOpenINIFile.TabIndex = 13;
            this.cmdOpenINIFile.Text = "Open File...";
            this.cmdOpenINIFile.UseVisualStyleBackColor = true;
            this.cmdOpenINIFile.Visible = false;
            this.cmdOpenINIFile.Click += new System.EventHandler(this.cmdOpenINIFile_Click);
            // 
            // gbSatellite
            // 
            this.gbSatellite.Controls.Add(this.txtAzimuth);
            this.gbSatellite.Controls.Add(this.label13);
            this.gbSatellite.Controls.Add(this.txtLNBSwitch);
            this.gbSatellite.Controls.Add(this.label7);
            this.gbSatellite.Controls.Add(this.txtLNBHigh);
            this.gbSatellite.Controls.Add(this.label4);
            this.gbSatellite.Controls.Add(this.chkDefaultLNB);
            this.gbSatellite.Controls.Add(this.txtLNBLow);
            this.gbSatellite.Controls.Add(this.label6);
            this.gbSatellite.Location = new System.Drawing.Point(6, 247);
            this.gbSatellite.Name = "gbSatellite";
            this.gbSatellite.Size = new System.Drawing.Size(528, 74);
            this.gbSatellite.TabIndex = 12;
            this.gbSatellite.TabStop = false;
            this.gbSatellite.Text = "Satellite LNB Parameters";
            // 
            // txtAzimuth
            // 
            this.txtAzimuth.Location = new System.Drawing.Point(344, 44);
            this.txtAzimuth.Name = "txtAzimuth";
            this.txtAzimuth.Size = new System.Drawing.Size(48, 20);
            this.txtAzimuth.TabIndex = 10;
            this.txtAzimuth.TextChanged += new System.EventHandler(this.txtAzimuth_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(221, 47);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(117, 13);
            this.label13.TabIndex = 9;
            this.label13.Text = "Satellite Azimuth (East):";
            // 
            // txtLNBSwitch
            // 
            this.txtLNBSwitch.Location = new System.Drawing.Point(296, 17);
            this.txtLNBSwitch.Name = "txtLNBSwitch";
            this.txtLNBSwitch.Size = new System.Drawing.Size(96, 20);
            this.txtLNBSwitch.TabIndex = 8;
            this.txtLNBSwitch.TextChanged += new System.EventHandler(this.txtLNBSwitch_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(221, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "LNB Switch:";
            // 
            // txtLNBHigh
            // 
            this.txtLNBHigh.AcceptsTab = true;
            this.txtLNBHigh.Location = new System.Drawing.Point(97, 43);
            this.txtLNBHigh.Name = "txtLNBHigh";
            this.txtLNBHigh.Size = new System.Drawing.Size(93, 20);
            this.txtLNBHigh.TabIndex = 6;
            this.txtLNBHigh.TextChanged += new System.EventHandler(this.txtLNBHigh_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "LNB High Band:";
            // 
            // chkDefaultLNB
            // 
            this.chkDefaultLNB.AutoSize = true;
            this.chkDefaultLNB.Location = new System.Drawing.Point(429, 17);
            this.chkDefaultLNB.Name = "chkDefaultLNB";
            this.chkDefaultLNB.Size = new System.Drawing.Size(93, 17);
            this.chkDefaultLNB.TabIndex = 4;
            this.chkDefaultLNB.Text = "Use Defaults?";
            this.chkDefaultLNB.UseVisualStyleBackColor = true;
            this.chkDefaultLNB.CheckedChanged += new System.EventHandler(this.chkDefaultLNB_CheckedChanged);
            // 
            // txtLNBLow
            // 
            this.txtLNBLow.Location = new System.Drawing.Point(97, 17);
            this.txtLNBLow.Name = "txtLNBLow";
            this.txtLNBLow.Size = new System.Drawing.Size(93, 20);
            this.txtLNBLow.TabIndex = 1;
            this.txtLNBLow.TextChanged += new System.EventHandler(this.txtLNBLow_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "LNB Low Band:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmdScanTuners);
            this.groupBox2.Controls.Add(this.cboTuners);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(6, 104);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(528, 55);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tuner Selection";
            // 
            // cmdScanTuners
            // 
            this.cmdScanTuners.Location = new System.Drawing.Point(429, 19);
            this.cmdScanTuners.Name = "cmdScanTuners";
            this.cmdScanTuners.Size = new System.Drawing.Size(92, 21);
            this.cmdScanTuners.TabIndex = 9;
            this.cmdScanTuners.Text = "Find Tuners...";
            this.cmdScanTuners.UseVisualStyleBackColor = true;
            this.cmdScanTuners.Click += new System.EventHandler(this.cmdScanTuners_Click);
            // 
            // cboTuners
            // 
            this.cboTuners.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTuners.FormattingEnabled = true;
            this.cboTuners.Location = new System.Drawing.Point(137, 19);
            this.cboTuners.Name = "cboTuners";
            this.cboTuners.Size = new System.Drawing.Size(286, 21);
            this.cboTuners.TabIndex = 1;
            this.cboTuners.SelectedIndexChanged += new System.EventHandler(this.cboTuners_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Tuner for EPG Collector:";
            // 
            // cmdSaveGeneral
            // 
            this.cmdSaveGeneral.Location = new System.Drawing.Point(445, 483);
            this.cmdSaveGeneral.Name = "cmdSaveGeneral";
            this.cmdSaveGeneral.Size = new System.Drawing.Size(83, 25);
            this.cmdSaveGeneral.TabIndex = 12;
            this.cmdSaveGeneral.Text = "Save";
            this.cmdSaveGeneral.UseVisualStyleBackColor = true;
            this.cmdSaveGeneral.Visible = false;
            this.cmdSaveGeneral.Click += new System.EventHandler(this.cmdSaveGeneral_Click);
            // 
            // gpTimeouts
            // 
            this.gpTimeouts.Controls.Add(this.txtScanRetries);
            this.gpTimeouts.Controls.Add(this.label12);
            this.gpTimeouts.Controls.Add(this.chkDefaultTimeouts);
            this.gpTimeouts.Controls.Add(this.txtDataCollectionTimeout);
            this.gpTimeouts.Controls.Add(this.label2);
            this.gpTimeouts.Controls.Add(this.txtSignalLockTimeout);
            this.gpTimeouts.Controls.Add(this.label1);
            this.gpTimeouts.Location = new System.Drawing.Point(7, 165);
            this.gpTimeouts.Name = "gpTimeouts";
            this.gpTimeouts.Size = new System.Drawing.Size(528, 76);
            this.gpTimeouts.TabIndex = 11;
            this.gpTimeouts.TabStop = false;
            this.gpTimeouts.Text = "Timeouts";
            // 
            // txtScanRetries
            // 
            this.txtScanRetries.Location = new System.Drawing.Point(158, 50);
            this.txtScanRetries.Name = "txtScanRetries";
            this.txtScanRetries.Size = new System.Drawing.Size(32, 20);
            this.txtScanRetries.TabIndex = 6;
            this.txtScanRetries.TextChanged += new System.EventHandler(this.txtScanRetries_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 54);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(130, 13);
            this.label12.TabIndex = 5;
            this.label12.Text = "Number of Times to Retry:";
            // 
            // chkDefaultTimeouts
            // 
            this.chkDefaultTimeouts.AutoSize = true;
            this.chkDefaultTimeouts.Location = new System.Drawing.Point(429, 27);
            this.chkDefaultTimeouts.Name = "chkDefaultTimeouts";
            this.chkDefaultTimeouts.Size = new System.Drawing.Size(93, 17);
            this.chkDefaultTimeouts.TabIndex = 4;
            this.chkDefaultTimeouts.Text = "Use Defaults?";
            this.chkDefaultTimeouts.UseVisualStyleBackColor = true;
            this.chkDefaultTimeouts.CheckedChanged += new System.EventHandler(this.chkDefaultTimeouts_CheckedChanged);
            // 
            // txtDataCollectionTimeout
            // 
            this.txtDataCollectionTimeout.Location = new System.Drawing.Point(361, 19);
            this.txtDataCollectionTimeout.Name = "txtDataCollectionTimeout";
            this.txtDataCollectionTimeout.Size = new System.Drawing.Size(31, 20);
            this.txtDataCollectionTimeout.TabIndex = 3;
            this.txtDataCollectionTimeout.TextChanged += new System.EventHandler(this.txtDataCollectionTimeout_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(196, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Max wait for Data Collection (sec):";
            // 
            // txtSignalLockTimeout
            // 
            this.txtSignalLockTimeout.Location = new System.Drawing.Point(158, 19);
            this.txtSignalLockTimeout.Name = "txtSignalLockTimeout";
            this.txtSignalLockTimeout.Size = new System.Drawing.Size(32, 20);
            this.txtSignalLockTimeout.TabIndex = 1;
            this.txtSignalLockTimeout.TextChanged += new System.EventHandler(this.txtSignalLockTimeout_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Max wait for Signal Lock (sec):";
            // 
            // rtbFrequencies
            // 
            this.rtbFrequencies.BackColor = System.Drawing.SystemColors.MenuBar;
            this.rtbFrequencies.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbFrequencies.Location = new System.Drawing.Point(7, 433);
            this.rtbFrequencies.Name = "rtbFrequencies";
            this.rtbFrequencies.Size = new System.Drawing.Size(527, 163);
            this.rtbFrequencies.TabIndex = 10;
            this.rtbFrequencies.Text = "";
            // 
            // gpLocation
            // 
            this.gpLocation.Controls.Add(this.cmdLocationApply);
            this.gpLocation.Controls.Add(this.lblArea);
            this.gpLocation.Controls.Add(this.cboArea);
            this.gpLocation.Controls.Add(this.lblCountry);
            this.gpLocation.Controls.Add(this.cboCountry);
            this.gpLocation.Location = new System.Drawing.Point(154, 6);
            this.gpLocation.Name = "gpLocation";
            this.gpLocation.Size = new System.Drawing.Size(380, 92);
            this.gpLocation.TabIndex = 9;
            this.gpLocation.TabStop = false;
            this.gpLocation.Text = "Location";
            // 
            // cmdLocationApply
            // 
            this.cmdLocationApply.Location = new System.Drawing.Point(306, 67);
            this.cmdLocationApply.Name = "cmdLocationApply";
            this.cmdLocationApply.Size = new System.Drawing.Size(67, 25);
            this.cmdLocationApply.TabIndex = 9;
            this.cmdLocationApply.Text = "Apply...";
            this.cmdLocationApply.UseVisualStyleBackColor = true;
            this.cmdLocationApply.Click += new System.EventHandler(this.cmdLocationApply_Click);
            // 
            // lblArea
            // 
            this.lblArea.AutoSize = true;
            this.lblArea.Location = new System.Drawing.Point(185, 19);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(32, 13);
            this.lblArea.TabIndex = 3;
            this.lblArea.Text = "Area:";
            // 
            // cboArea
            // 
            this.cboArea.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboArea.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboArea.FormattingEnabled = true;
            this.cboArea.Location = new System.Drawing.Point(185, 40);
            this.cboArea.Name = "cboArea";
            this.cboArea.Size = new System.Drawing.Size(188, 21);
            this.cboArea.TabIndex = 2;
            this.cboArea.SelectedIndexChanged += new System.EventHandler(this.cboArea_SelectedIndexChanged);
            // 
            // lblCountry
            // 
            this.lblCountry.AutoSize = true;
            this.lblCountry.Location = new System.Drawing.Point(6, 19);
            this.lblCountry.Name = "lblCountry";
            this.lblCountry.Size = new System.Drawing.Size(46, 13);
            this.lblCountry.TabIndex = 1;
            this.lblCountry.Text = "Country:";
            // 
            // cboCountry
            // 
            this.cboCountry.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboCountry.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCountry.FormattingEnabled = true;
            this.cboCountry.Location = new System.Drawing.Point(6, 40);
            this.cboCountry.Name = "cboCountry";
            this.cboCountry.Size = new System.Drawing.Size(173, 21);
            this.cboCountry.TabIndex = 0;
            this.cboCountry.SelectedIndexChanged += new System.EventHandler(this.cboCountry_SelectedIndexChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(404, 336);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(61, 20);
            this.btnBrowse.TabIndex = 8;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtOutputFile
            // 
            this.txtOutputFile.Location = new System.Drawing.Point(78, 336);
            this.txtOutputFile.Name = "txtOutputFile";
            this.txtOutputFile.Size = new System.Drawing.Size(320, 20);
            this.txtOutputFile.TabIndex = 7;
            this.txtOutputFile.TextChanged += new System.EventHandler(this.txtOutputFile_TextChanged);
            // 
            // lblOutputFile
            // 
            this.lblOutputFile.AutoSize = true;
            this.lblOutputFile.Location = new System.Drawing.Point(8, 339);
            this.lblOutputFile.Name = "lblOutputFile";
            this.lblOutputFile.Size = new System.Drawing.Size(61, 13);
            this.lblOutputFile.TabIndex = 6;
            this.lblOutputFile.Text = "Output File:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDVBT);
            this.groupBox1.Controls.Add(this.rbSatellite);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(142, 92);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transmission Type";
            // 
            // rbDVBT
            // 
            this.rbDVBT.AutoSize = true;
            this.rbDVBT.Location = new System.Drawing.Point(6, 29);
            this.rbDVBT.Name = "rbDVBT";
            this.rbDVBT.Size = new System.Drawing.Size(71, 17);
            this.rbDVBT.TabIndex = 1;
            this.rbDVBT.TabStop = true;
            this.rbDVBT.Text = "Terrestrial";
            this.rbDVBT.UseVisualStyleBackColor = true;
            this.rbDVBT.CheckedChanged += new System.EventHandler(this.rbDVBT_CheckedChanged);
            // 
            // rbSatellite
            // 
            this.rbSatellite.AutoSize = true;
            this.rbSatellite.Location = new System.Drawing.Point(6, 52);
            this.rbSatellite.Name = "rbSatellite";
            this.rbSatellite.Size = new System.Drawing.Size(62, 17);
            this.rbSatellite.TabIndex = 0;
            this.rbSatellite.TabStop = true;
            this.rbSatellite.Text = "Satellite";
            this.rbSatellite.UseVisualStyleBackColor = true;
            this.rbSatellite.CheckedChanged += new System.EventHandler(this.rbSatellite_CheckedChanged);
            // 
            // tabChannels
            // 
            this.tabChannels.Controls.Add(this.cmdStopScan);
            this.tabChannels.Controls.Add(this.cmdClearScan);
            this.tabChannels.Controls.Add(this.cmdSelectNone);
            this.tabChannels.Controls.Add(this.cmdSelectAll);
            this.tabChannels.Controls.Add(this.pgbScanning);
            this.tabChannels.Controls.Add(this.label3);
            this.tabChannels.Controls.Add(this.cmdSaveChannels);
            this.tabChannels.Controls.Add(this.lstTVStationsFound);
            this.tabChannels.Controls.Add(this.lblScanning);
            this.tabChannels.Controls.Add(this.lblFound);
            this.tabChannels.Controls.Add(this.cmdScan);
            this.tabChannels.Location = new System.Drawing.Point(4, 22);
            this.tabChannels.Name = "tabChannels";
            this.tabChannels.Padding = new System.Windows.Forms.Padding(3);
            this.tabChannels.Size = new System.Drawing.Size(541, 602);
            this.tabChannels.TabIndex = 1;
            this.tabChannels.Text = "Channel Selection";
            this.tabChannels.UseVisualStyleBackColor = true;
            // 
            // cmdStopScan
            // 
            this.cmdStopScan.Enabled = false;
            this.cmdStopScan.Location = new System.Drawing.Point(350, 43);
            this.cmdStopScan.Name = "cmdStopScan";
            this.cmdStopScan.Size = new System.Drawing.Size(88, 25);
            this.cmdStopScan.TabIndex = 17;
            this.cmdStopScan.Text = "Stop Scan...";
            this.cmdStopScan.UseVisualStyleBackColor = true;
            this.cmdStopScan.Visible = false;
            this.cmdStopScan.Click += new System.EventHandler(this.cmdStopScan_Click);
            // 
            // cmdClearScan
            // 
            this.cmdClearScan.Location = new System.Drawing.Point(447, 13);
            this.cmdClearScan.Name = "cmdClearScan";
            this.cmdClearScan.Size = new System.Drawing.Size(87, 24);
            this.cmdClearScan.TabIndex = 16;
            this.cmdClearScan.Text = "Clear Channels";
            this.cmdClearScan.UseVisualStyleBackColor = true;
            this.cmdClearScan.Click += new System.EventHandler(this.cmdClearScan_Click);
            // 
            // cmdSelectNone
            // 
            this.cmdSelectNone.Location = new System.Drawing.Point(259, 200);
            this.cmdSelectNone.Name = "cmdSelectNone";
            this.cmdSelectNone.Size = new System.Drawing.Size(97, 25);
            this.cmdSelectNone.TabIndex = 15;
            this.cmdSelectNone.Text = "Select None...";
            this.cmdSelectNone.UseVisualStyleBackColor = true;
            this.cmdSelectNone.Click += new System.EventHandler(this.cmdSelectNone_Click);
            // 
            // cmdSelectAll
            // 
            this.cmdSelectAll.Location = new System.Drawing.Point(259, 167);
            this.cmdSelectAll.Name = "cmdSelectAll";
            this.cmdSelectAll.Size = new System.Drawing.Size(97, 25);
            this.cmdSelectAll.TabIndex = 14;
            this.cmdSelectAll.Text = "Select All...";
            this.cmdSelectAll.UseVisualStyleBackColor = true;
            this.cmdSelectAll.Click += new System.EventHandler(this.cmdSelectAll_Click);
            // 
            // pgbScanning
            // 
            this.pgbScanning.Location = new System.Drawing.Point(3, 576);
            this.pgbScanning.Name = "pgbScanning";
            this.pgbScanning.Size = new System.Drawing.Size(532, 23);
            this.pgbScanning.TabIndex = 13;
            this.pgbScanning.Visible = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(362, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 77);
            this.label3.TabIndex = 12;
            this.label3.Text = "Select the Channels you wish to collect EPG data for...";
            // 
            // cmdSaveChannels
            // 
            this.cmdSaveChannels.Enabled = false;
            this.cmdSaveChannels.Location = new System.Drawing.Point(447, 43);
            this.cmdSaveChannels.Name = "cmdSaveChannels";
            this.cmdSaveChannels.Size = new System.Drawing.Size(87, 24);
            this.cmdSaveChannels.TabIndex = 11;
            this.cmdSaveChannels.Text = "Save";
            this.cmdSaveChannels.UseVisualStyleBackColor = true;
            this.cmdSaveChannels.Visible = false;
            this.cmdSaveChannels.Click += new System.EventHandler(this.cmdSaveChannels_Click);
            // 
            // lstTVStationsFound
            // 
            this.lstTVStationsFound.CheckOnClick = true;
            this.lstTVStationsFound.FormattingEnabled = true;
            this.lstTVStationsFound.Location = new System.Drawing.Point(25, 43);
            this.lstTVStationsFound.Name = "lstTVStationsFound";
            this.lstTVStationsFound.Size = new System.Drawing.Size(228, 484);
            this.lstTVStationsFound.TabIndex = 10;
            this.lstTVStationsFound.SelectedIndexChanged += new System.EventHandler(this.lstTVStationsFound_SelectedIndexChanged);
            // 
            // lblScanning
            // 
            this.lblScanning.AutoSize = true;
            this.lblScanning.Location = new System.Drawing.Point(7, 550);
            this.lblScanning.Name = "lblScanning";
            this.lblScanning.Size = new System.Drawing.Size(0, 13);
            this.lblScanning.TabIndex = 9;
            // 
            // lblFound
            // 
            this.lblFound.AutoSize = true;
            this.lblFound.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFound.Location = new System.Drawing.Point(23, 18);
            this.lblFound.Name = "lblFound";
            this.lblFound.Size = new System.Drawing.Size(104, 13);
            this.lblFound.TabIndex = 7;
            this.lblFound.Text = "Stations Found...";
            // 
            // cmdScan
            // 
            this.cmdScan.Location = new System.Drawing.Point(350, 12);
            this.cmdScan.Name = "cmdScan";
            this.cmdScan.Size = new System.Drawing.Size(88, 25);
            this.cmdScan.TabIndex = 0;
            this.cmdScan.Text = "Start Scan...";
            this.cmdScan.UseVisualStyleBackColor = true;
            this.cmdScan.Click += new System.EventHandler(this.cmdScan_Click);
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.groupBox7);
            this.tabOptions.Controls.Add(this.groupBox6);
            this.tabOptions.Controls.Add(this.groupBox5);
            this.tabOptions.Controls.Add(this.groupBox4);
            this.tabOptions.Controls.Add(this.label10);
            this.tabOptions.Controls.Add(this.groupBox3);
            this.tabOptions.Location = new System.Drawing.Point(4, 22);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Size = new System.Drawing.Size(541, 602);
            this.tabOptions.TabIndex = 2;
            this.tabOptions.Text = "Advanced Options";
            this.tabOptions.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.lblUSEIMAGE);
            this.groupBox7.Controls.Add(this.chkUSEIMAGE);
            this.groupBox7.Location = new System.Drawing.Point(12, 417);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(516, 88);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "USEIMAGE";
            // 
            // lblUSEIMAGE
            // 
            this.lblUSEIMAGE.Location = new System.Drawing.Point(11, 18);
            this.lblUSEIMAGE.Name = "lblUSEIMAGE";
            this.lblUSEIMAGE.Size = new System.Drawing.Size(494, 44);
            this.lblUSEIMAGE.TabIndex = 2;
            this.lblUSEIMAGE.Text = "label13";
            // 
            // chkUSEIMAGE
            // 
            this.chkUSEIMAGE.AutoSize = true;
            this.chkUSEIMAGE.Location = new System.Drawing.Point(6, 71);
            this.chkUSEIMAGE.Name = "chkUSEIMAGE";
            this.chkUSEIMAGE.Size = new System.Drawing.Size(104, 17);
            this.chkUSEIMAGE.TabIndex = 1;
            this.chkUSEIMAGE.Text = "Use USEIMAGE";
            this.chkUSEIMAGE.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblUSECHANNELID);
            this.groupBox6.Controls.Add(this.chkUSECHANNELID);
            this.groupBox6.Location = new System.Drawing.Point(12, 323);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(516, 88);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "USECHANNELID";
            // 
            // lblUSECHANNELID
            // 
            this.lblUSECHANNELID.Location = new System.Drawing.Point(11, 18);
            this.lblUSECHANNELID.Name = "lblUSECHANNELID";
            this.lblUSECHANNELID.Size = new System.Drawing.Size(494, 44);
            this.lblUSECHANNELID.TabIndex = 2;
            this.lblUSECHANNELID.Text = "label13";
            // 
            // chkUSECHANNELID
            // 
            this.chkUSECHANNELID.AutoSize = true;
            this.chkUSECHANNELID.Location = new System.Drawing.Point(6, 71);
            this.chkUSECHANNELID.Name = "chkUSECHANNELID";
            this.chkUSECHANNELID.Size = new System.Drawing.Size(132, 17);
            this.chkUSECHANNELID.TabIndex = 1;
            this.chkUSECHANNELID.Text = "Use USECHANNELID";
            this.chkUSECHANNELID.UseVisualStyleBackColor = true;
            this.chkUSECHANNELID.CheckedChanged += new System.EventHandler(this.chkUSECHANNELID_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lblROUNDTIME);
            this.groupBox5.Controls.Add(this.chkROUNDTIME);
            this.groupBox5.Location = new System.Drawing.Point(12, 229);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(516, 88);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "ROUNDTIME";
            // 
            // lblROUNDTIME
            // 
            this.lblROUNDTIME.Location = new System.Drawing.Point(11, 18);
            this.lblROUNDTIME.Name = "lblROUNDTIME";
            this.lblROUNDTIME.Size = new System.Drawing.Size(494, 44);
            this.lblROUNDTIME.TabIndex = 2;
            this.lblROUNDTIME.Text = "label12";
            // 
            // chkROUNDTIME
            // 
            this.chkROUNDTIME.AutoSize = true;
            this.chkROUNDTIME.Location = new System.Drawing.Point(6, 71);
            this.chkROUNDTIME.Name = "chkROUNDTIME";
            this.chkROUNDTIME.Size = new System.Drawing.Size(114, 17);
            this.chkROUNDTIME.TabIndex = 1;
            this.chkROUNDTIME.Text = "Use ROUNDTIME";
            this.chkROUNDTIME.UseVisualStyleBackColor = true;
            this.chkROUNDTIME.CheckedChanged += new System.EventHandler(this.chkROUNDTIME_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblACCEPTBREAKS);
            this.groupBox4.Controls.Add(this.chkACCEPTBREAKS);
            this.groupBox4.Location = new System.Drawing.Point(12, 135);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(516, 88);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ACCEPTBREAKS";
            // 
            // lblACCEPTBREAKS
            // 
            this.lblACCEPTBREAKS.Location = new System.Drawing.Point(11, 18);
            this.lblACCEPTBREAKS.Name = "lblACCEPTBREAKS";
            this.lblACCEPTBREAKS.Size = new System.Drawing.Size(494, 44);
            this.lblACCEPTBREAKS.TabIndex = 2;
            this.lblACCEPTBREAKS.Text = "label11";
            // 
            // chkACCEPTBREAKS
            // 
            this.chkACCEPTBREAKS.AutoSize = true;
            this.chkACCEPTBREAKS.Location = new System.Drawing.Point(6, 71);
            this.chkACCEPTBREAKS.Name = "chkACCEPTBREAKS";
            this.chkACCEPTBREAKS.Size = new System.Drawing.Size(133, 17);
            this.chkACCEPTBREAKS.TabIndex = 1;
            this.chkACCEPTBREAKS.Text = "Use ACCEPTBREAKS";
            this.chkACCEPTBREAKS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkACCEPTBREAKS.UseVisualStyleBackColor = true;
            this.chkACCEPTBREAKS.CheckedChanged += new System.EventHandler(this.chkACCEPTBREAKS_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(9, 11);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(325, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "These Options are NOT required for a normal collection.";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblNOSIGLOCK);
            this.groupBox3.Controls.Add(this.chkNOSIGLOCK);
            this.groupBox3.Location = new System.Drawing.Point(12, 41);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(516, 88);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "NOSIGLOCK";
            // 
            // lblNOSIGLOCK
            // 
            this.lblNOSIGLOCK.Location = new System.Drawing.Point(11, 18);
            this.lblNOSIGLOCK.Name = "lblNOSIGLOCK";
            this.lblNOSIGLOCK.Size = new System.Drawing.Size(494, 44);
            this.lblNOSIGLOCK.TabIndex = 2;
            this.lblNOSIGLOCK.Text = "label11";
            // 
            // chkNOSIGLOCK
            // 
            this.chkNOSIGLOCK.AutoSize = true;
            this.chkNOSIGLOCK.Location = new System.Drawing.Point(6, 71);
            this.chkNOSIGLOCK.Name = "chkNOSIGLOCK";
            this.chkNOSIGLOCK.Size = new System.Drawing.Size(110, 17);
            this.chkNOSIGLOCK.TabIndex = 1;
            this.chkNOSIGLOCK.Text = "Use NOSIGLOCK";
            this.chkNOSIGLOCK.UseVisualStyleBackColor = true;
            this.chkNOSIGLOCK.CheckedChanged += new System.EventHandler(this.chkNOSIGLOCK_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(551, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Checked = true;
            this.fileToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // createNewToolStripMenuItem
            // 
            this.createNewToolStripMenuItem.Name = "createNewToolStripMenuItem";
            this.createNewToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.createNewToolStripMenuItem.Text = "Create New ...";
            this.createNewToolStripMenuItem.Click += new System.EventHandler(this.createNewToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.saveToolStripMenuItem.Text = "Save...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readMeToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // readMeToolStripMenuItem
            // 
            this.readMeToolStripMenuItem.Name = "readMeToolStripMenuItem";
            this.readMeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.readMeToolStripMenuItem.Text = "ReadMe";
            this.readMeToolStripMenuItem.Click += new System.EventHandler(this.readMeToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(551, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.ToolTipText = "Create a new INI file...";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.ToolTipText = "open an existing INI file...";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "toolStripButton3";
            this.toolStripButton3.ToolTipText = "Save INI file...";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // tmrProgress
            // 
            this.tmrProgress.Interval = 1000;
            this.tmrProgress.Tick += new System.EventHandler(this.tmrProgress_Tick);
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 683);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ConfigurationForm";
            this.Text = "EPG Collector Configuration";
            this.Load += new System.EventHandler(this.ConfigurationForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.gbSatellite.ResumeLayout(false);
            this.gbSatellite.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gpTimeouts.ResumeLayout(false);
            this.gpTimeouts.PerformLayout();
            this.gpLocation.ResumeLayout(false);
            this.gpLocation.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabChannels.ResumeLayout(false);
            this.tabChannels.PerformLayout();
            this.tabOptions.ResumeLayout(false);
            this.tabOptions.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.GroupBox gpLocation;
        private System.Windows.Forms.Label lblArea;
        private System.Windows.Forms.ComboBox cboArea;
        private System.Windows.Forms.Label lblCountry;
        private System.Windows.Forms.ComboBox cboCountry;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtOutputFile;
        private System.Windows.Forms.Label lblOutputFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbDVBT;
        private System.Windows.Forms.RadioButton rbSatellite;
        private System.Windows.Forms.RichTextBox rtbFrequencies;
        private System.Windows.Forms.GroupBox gpTimeouts;
        private System.Windows.Forms.TextBox txtDataCollectionTimeout;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSignalLockTimeout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkDefaultTimeouts;
        private System.Windows.Forms.Button cmdSaveGeneral;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdScanTuners;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button cmdLocationApply;
        private System.Windows.Forms.GroupBox gbSatellite;
        private System.Windows.Forms.CheckBox chkDefaultLNB;
        private System.Windows.Forms.TextBox txtLNBLow;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtLNBHigh;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLNBSwitch;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button cmdOpenINIFile;
        private System.Windows.Forms.TextBox txtOptions;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ComboBox cboTuners;
        private System.Windows.Forms.ToolStripMenuItem createNewToolStripMenuItem;
        private System.Windows.Forms.Button cmdNewOutputFile;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cboFrequencies;
        private System.Windows.Forms.TabPage tabChannels;
        private System.Windows.Forms.Button cmdClearScan;
        private System.Windows.Forms.Button cmdSelectNone;
        private System.Windows.Forms.Button cmdSelectAll;
        private System.Windows.Forms.ProgressBar pgbScanning;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cmdSaveChannels;
        private System.Windows.Forms.CheckedListBox lstTVStationsFound;
        private System.Windows.Forms.Label lblScanning;
        private System.Windows.Forms.Label lblFound;
        private System.Windows.Forms.Button cmdScan;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readMeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkNOSIGLOCK;
        private System.Windows.Forms.Label lblNOSIGLOCK;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lblUSECHANNELID;
        private System.Windows.Forms.CheckBox chkUSECHANNELID;
        private System.Windows.Forms.Label lblROUNDTIME;
        private System.Windows.Forms.CheckBox chkROUNDTIME;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblACCEPTBREAKS;
        private System.Windows.Forms.CheckBox chkACCEPTBREAKS;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cboCollectionType;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label lblUSEIMAGE;
        private System.Windows.Forms.CheckBox chkUSEIMAGE;
        private System.Windows.Forms.Button cmdStopScan;
        private System.Windows.Forms.Timer tmrProgress;
        private System.Windows.Forms.TextBox txtScanRetries;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtAzimuth;
        private System.Windows.Forms.Label label13;


    }
}

