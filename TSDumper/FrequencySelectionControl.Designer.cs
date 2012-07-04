namespace TSDumper
{
    partial class FrequencySelectionControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.clbTuners = new System.Windows.Forms.CheckedListBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbSwitchAfterPlay = new System.Windows.Forms.CheckBox();
            this.cbUseSignalPresent = new System.Windows.Forms.CheckBox();
            this.cbRepeatDiseqc = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbcDeliverySystem = new System.Windows.Forms.TabControl();
            this.tbpSatellite = new System.Windows.Forms.TabPage();
            this.btLNBDefaults = new System.Windows.Forms.Button();
            this.txtLNBSwitch = new System.Windows.Forms.TextBox();
            this.txtLNBHigh = new System.Windows.Forms.TextBox();
            this.cboDVBSScanningFrequency = new System.Windows.Forms.ComboBox();
            this.label29 = new System.Windows.Forms.Label();
            this.txtLNBLow = new System.Windows.Forms.TextBox();
            this.cboSatellite = new System.Windows.Forms.ComboBox();
            this.label36 = new System.Windows.Forms.Label();
            this.cboDiseqc = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.tbpAtsc = new System.Windows.Forms.TabPage();
            this.label50 = new System.Windows.Forms.Label();
            this.cboAtscProvider = new System.Windows.Forms.ComboBox();
            this.label53 = new System.Windows.Forms.Label();
            this.cboAtscScanningFrequency = new System.Windows.Forms.ComboBox();
            this.tbpClearQAM = new System.Windows.Forms.TabPage();
            this.label57 = new System.Windows.Forms.Label();
            this.cboClearQamProvider = new System.Windows.Forms.ComboBox();
            this.label58 = new System.Windows.Forms.Label();
            this.cboClearQamScanningFrequency = new System.Windows.Forms.ComboBox();
            this.tbpISDBSatellite = new System.Windows.Forms.TabPage();
            this.btISDBLNBDefaults = new System.Windows.Forms.Button();
            this.txtISDBLNBSwitch = new System.Windows.Forms.TextBox();
            this.txtISDBLNBHigh = new System.Windows.Forms.TextBox();
            this.cboISDBSScanningFrequency = new System.Windows.Forms.ComboBox();
            this.label61 = new System.Windows.Forms.Label();
            this.txtISDBLNBLow = new System.Windows.Forms.TextBox();
            this.cboISDBSatellite = new System.Windows.Forms.ComboBox();
            this.label62 = new System.Windows.Forms.Label();
            this.cboISDBDiseqc = new System.Windows.Forms.ComboBox();
            this.label63 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.label65 = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.tbpISDBTerrestrial = new System.Windows.Forms.TabPage();
            this.label67 = new System.Windows.Forms.Label();
            this.cboISDBTProvider = new System.Windows.Forms.ComboBox();
            this.label68 = new System.Windows.Forms.Label();
            this.cboISDBTScanningFrequency = new System.Windows.Forms.ComboBox();
            this.cboCableScanningFrequency = new System.Windows.Forms.ComboBox();
            this.label52 = new System.Windows.Forms.Label();
            this.cboCable = new System.Windows.Forms.ComboBox();
            this.label54 = new System.Windows.Forms.Label();
            this.tbpCable = new System.Windows.Forms.TabPage();
            this.label24 = new System.Windows.Forms.Label();
            this.cboDVBTScanningFrequency = new System.Windows.Forms.ComboBox();
            this.cboCountry = new System.Windows.Forms.ComboBox();
            this.lblCountry = new System.Windows.Forms.Label();
            this.cboArea = new System.Windows.Forms.ComboBox();
            this.lblArea = new System.Windows.Forms.Label();
            this.tbpTerrestrial = new System.Windows.Forms.TabPage();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tbcDeliverySystem.SuspendLayout();
            this.tbpSatellite.SuspendLayout();
            this.tbpAtsc.SuspendLayout();
            this.tbpClearQAM.SuspendLayout();
            this.tbpISDBSatellite.SuspendLayout();
            this.tbpISDBTerrestrial.SuspendLayout();
            this.tbpCable.SuspendLayout();
            this.tbpTerrestrial.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.clbTuners);
            this.groupBox3.Location = new System.Drawing.Point(6, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(440, 100);
            this.groupBox3.TabIndex = 100;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tuners";
            // 
            // clbTuners
            // 
            this.clbTuners.CheckOnClick = true;
            this.clbTuners.FormattingEnabled = true;
            this.clbTuners.Location = new System.Drawing.Point(13, 19);
            this.clbTuners.Name = "clbTuners";
            this.clbTuners.Size = new System.Drawing.Size(412, 64);
            this.clbTuners.TabIndex = 101;
            this.clbTuners.SelectedIndexChanged += new System.EventHandler(this.clbTuners_SelectedIndexChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbSwitchAfterPlay);
            this.groupBox5.Controls.Add(this.cbUseSignalPresent);
            this.groupBox5.Controls.Add(this.cbRepeatDiseqc);
            this.groupBox5.Location = new System.Drawing.Point(10, 213);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(379, 89);
            this.groupBox5.TabIndex = 300;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Tuning Options";
            // 
            // cbSwitchAfterPlay
            // 
            this.cbSwitchAfterPlay.AutoSize = true;
            this.cbSwitchAfterPlay.Location = new System.Drawing.Point(16, 42);
            this.cbSwitchAfterPlay.Name = "cbSwitchAfterPlay";
            this.cbSwitchAfterPlay.Size = new System.Drawing.Size(255, 17);
            this.cbSwitchAfterPlay.TabIndex = 302;
            this.cbSwitchAfterPlay.Text = "Change DiSEqC switch only when graph running";
            this.cbSwitchAfterPlay.UseVisualStyleBackColor = true;
            // 
            // cbUseSignalPresent
            // 
            this.cbUseSignalPresent.AutoSize = true;
            this.cbUseSignalPresent.Location = new System.Drawing.Point(16, 19);
            this.cbUseSignalPresent.Name = "cbUseSignalPresent";
            this.cbUseSignalPresent.Size = new System.Drawing.Size(335, 17);
            this.cbUseSignalPresent.TabIndex = 301;
            this.cbUseSignalPresent.Text = "Use signal present when tuning if signal lock and signal quality fail";
            this.cbUseSignalPresent.UseVisualStyleBackColor = true;
            // 
            // cbRepeatDiseqc
            // 
            this.cbRepeatDiseqc.AutoSize = true;
            this.cbRepeatDiseqc.Location = new System.Drawing.Point(16, 65);
            this.cbRepeatDiseqc.Name = "cbRepeatDiseqc";
            this.cbRepeatDiseqc.Size = new System.Drawing.Size(236, 17);
            this.cbRepeatDiseqc.TabIndex = 303;
            this.cbRepeatDiseqc.Text = "Repeat DiSEqC command if first attempt fails";
            this.cbRepeatDiseqc.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbcDeliverySystem);
            this.groupBox1.Location = new System.Drawing.Point(7, 111);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(439, 364);
            this.groupBox1.TabIndex = 200;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Delivery System";
            // 
            // tbcDeliverySystem
            // 
            this.tbcDeliverySystem.Controls.Add(this.tbpSatellite);
            this.tbcDeliverySystem.Controls.Add(this.tbpTerrestrial);
            this.tbcDeliverySystem.Controls.Add(this.tbpCable);
            this.tbcDeliverySystem.Controls.Add(this.tbpAtsc);
            this.tbcDeliverySystem.Controls.Add(this.tbpClearQAM);
            this.tbcDeliverySystem.Controls.Add(this.tbpISDBSatellite);
            this.tbcDeliverySystem.Controls.Add(this.tbpISDBTerrestrial);
            this.tbcDeliverySystem.Location = new System.Drawing.Point(12, 24);
            this.tbcDeliverySystem.Name = "tbcDeliverySystem";
            this.tbcDeliverySystem.SelectedIndex = 0;
            this.tbcDeliverySystem.Size = new System.Drawing.Size(412, 334);
            this.tbcDeliverySystem.TabIndex = 201;
            // 
            // tbpSatellite
            // 
            this.tbpSatellite.Controls.Add(this.btLNBDefaults);
            this.tbpSatellite.Controls.Add(this.groupBox5);
            this.tbpSatellite.Controls.Add(this.txtLNBSwitch);
            this.tbpSatellite.Controls.Add(this.txtLNBHigh);
            this.tbpSatellite.Controls.Add(this.cboDVBSScanningFrequency);
            this.tbpSatellite.Controls.Add(this.label29);
            this.tbpSatellite.Controls.Add(this.txtLNBLow);
            this.tbpSatellite.Controls.Add(this.cboSatellite);
            this.tbpSatellite.Controls.Add(this.label36);
            this.tbpSatellite.Controls.Add(this.cboDiseqc);
            this.tbpSatellite.Controls.Add(this.label4);
            this.tbpSatellite.Controls.Add(this.label31);
            this.tbpSatellite.Controls.Add(this.label33);
            this.tbpSatellite.Controls.Add(this.label32);
            this.tbpSatellite.Location = new System.Drawing.Point(4, 22);
            this.tbpSatellite.Name = "tbpSatellite";
            this.tbpSatellite.Padding = new System.Windows.Forms.Padding(3);
            this.tbpSatellite.Size = new System.Drawing.Size(404, 308);
            this.tbpSatellite.TabIndex = 0;
            this.tbpSatellite.Text = "DVB Satellite";
            this.tbpSatellite.UseVisualStyleBackColor = true;
            // 
            // btLNBDefaults
            // 
            this.btLNBDefaults.Location = new System.Drawing.Point(169, 184);
            this.btLNBDefaults.Name = "btLNBDefaults";
            this.btLNBDefaults.Size = new System.Drawing.Size(75, 23);
            this.btLNBDefaults.TabIndex = 208;
            this.btLNBDefaults.Text = "Defaults";
            this.btLNBDefaults.UseVisualStyleBackColor = true;
            this.btLNBDefaults.Click += new System.EventHandler(this.btLNBDefaults_Click);
            // 
            // txtLNBSwitch
            // 
            this.txtLNBSwitch.Location = new System.Drawing.Point(155, 126);
            this.txtLNBSwitch.Name = "txtLNBSwitch";
            this.txtLNBSwitch.Size = new System.Drawing.Size(92, 20);
            this.txtLNBSwitch.TabIndex = 206;
            // 
            // txtLNBHigh
            // 
            this.txtLNBHigh.Location = new System.Drawing.Point(155, 99);
            this.txtLNBHigh.Name = "txtLNBHigh";
            this.txtLNBHigh.Size = new System.Drawing.Size(92, 20);
            this.txtLNBHigh.TabIndex = 205;
            // 
            // cboDVBSScanningFrequency
            // 
            this.cboDVBSScanningFrequency.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboDVBSScanningFrequency.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboDVBSScanningFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDVBSScanningFrequency.FormattingEnabled = true;
            this.cboDVBSScanningFrequency.ItemHeight = 13;
            this.cboDVBSScanningFrequency.Location = new System.Drawing.Point(155, 44);
            this.cboDVBSScanningFrequency.MaxDropDownItems = 20;
            this.cboDVBSScanningFrequency.Name = "cboDVBSScanningFrequency";
            this.cboDVBSScanningFrequency.Size = new System.Drawing.Size(150, 21);
            this.cboDVBSScanningFrequency.TabIndex = 203;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(7, 47);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(57, 13);
            this.label29.TabIndex = 130;
            this.label29.Text = "Frequency";
            // 
            // txtLNBLow
            // 
            this.txtLNBLow.Location = new System.Drawing.Point(155, 72);
            this.txtLNBLow.Name = "txtLNBLow";
            this.txtLNBLow.Size = new System.Drawing.Size(92, 20);
            this.txtLNBLow.TabIndex = 204;
            // 
            // cboSatellite
            // 
            this.cboSatellite.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboSatellite.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSatellite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSatellite.FormattingEnabled = true;
            this.cboSatellite.Location = new System.Drawing.Point(155, 16);
            this.cboSatellite.MaxDropDownItems = 20;
            this.cboSatellite.Name = "cboSatellite";
            this.cboSatellite.Size = new System.Drawing.Size(234, 21);
            this.cboSatellite.TabIndex = 202;
            this.cboSatellite.SelectedIndexChanged += new System.EventHandler(this.cboSatellite_SelectedIndexChanged);
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(7, 159);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(44, 13);
            this.label36.TabIndex = 125;
            this.label36.Text = "DiSEqC";
            // 
            // cboDiseqc
            // 
            this.cboDiseqc.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboDiseqc.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboDiseqc.FormattingEnabled = true;
            this.cboDiseqc.Items.AddRange(new object[] {
            "None",
            "A",
            "B",
            "AA",
            "AB",
            "BA",
            "BB"});
            this.cboDiseqc.Location = new System.Drawing.Point(155, 153);
            this.cboDiseqc.Name = "cboDiseqc";
            this.cboDiseqc.Size = new System.Drawing.Size(191, 21);
            this.cboDiseqc.TabIndex = 207;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 128;
            this.label4.Text = "Satellite";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(7, 131);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(63, 13);
            this.label31.TabIndex = 123;
            this.label31.Text = "LNB Switch";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(7, 75);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(79, 13);
            this.label33.TabIndex = 119;
            this.label33.Text = "LNB Low Band";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(7, 103);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(81, 13);
            this.label32.TabIndex = 121;
            this.label32.Text = "LNB High Band";
            // 
            // tbpAtsc
            // 
            this.tbpAtsc.Controls.Add(this.label50);
            this.tbpAtsc.Controls.Add(this.cboAtscProvider);
            this.tbpAtsc.Controls.Add(this.label53);
            this.tbpAtsc.Controls.Add(this.cboAtscScanningFrequency);
            this.tbpAtsc.Location = new System.Drawing.Point(4, 22);
            this.tbpAtsc.Name = "tbpAtsc";
            this.tbpAtsc.Size = new System.Drawing.Size(404, 308);
            this.tbpAtsc.TabIndex = 3;
            this.tbpAtsc.Text = "ATSC";
            this.tbpAtsc.UseVisualStyleBackColor = true;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(100, 60);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(46, 13);
            this.label50.TabIndex = 137;
            this.label50.Text = "Provider";
            // 
            // cboAtscProvider
            // 
            this.cboAtscProvider.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboAtscProvider.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboAtscProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAtscProvider.FormattingEnabled = true;
            this.cboAtscProvider.Location = new System.Drawing.Point(220, 57);
            this.cboAtscProvider.MaxDropDownItems = 20;
            this.cboAtscProvider.Name = "cboAtscProvider";
            this.cboAtscProvider.Size = new System.Drawing.Size(221, 21);
            this.cboAtscProvider.TabIndex = 240;
            this.cboAtscProvider.SelectedIndexChanged += new System.EventHandler(this.cboAtscProvider_SelectedIndexChanged);
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(100, 100);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(46, 13);
            this.label53.TabIndex = 139;
            this.label53.Text = "Channel";
            // 
            // cboAtscScanningFrequency
            // 
            this.cboAtscScanningFrequency.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboAtscScanningFrequency.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboAtscScanningFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAtscScanningFrequency.FormattingEnabled = true;
            this.cboAtscScanningFrequency.Location = new System.Drawing.Point(220, 97);
            this.cboAtscScanningFrequency.MaxDropDownItems = 20;
            this.cboAtscScanningFrequency.Name = "cboAtscScanningFrequency";
            this.cboAtscScanningFrequency.Size = new System.Drawing.Size(150, 21);
            this.cboAtscScanningFrequency.TabIndex = 241;
            // 
            // tbpClearQAM
            // 
            this.tbpClearQAM.Controls.Add(this.label57);
            this.tbpClearQAM.Controls.Add(this.cboClearQamProvider);
            this.tbpClearQAM.Controls.Add(this.label58);
            this.tbpClearQAM.Controls.Add(this.cboClearQamScanningFrequency);
            this.tbpClearQAM.Location = new System.Drawing.Point(4, 22);
            this.tbpClearQAM.Name = "tbpClearQAM";
            this.tbpClearQAM.Size = new System.Drawing.Size(404, 308);
            this.tbpClearQAM.TabIndex = 4;
            this.tbpClearQAM.Text = "Clear QAM";
            this.tbpClearQAM.UseVisualStyleBackColor = true;
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Location = new System.Drawing.Point(100, 60);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(46, 13);
            this.label57.TabIndex = 143;
            this.label57.Text = "Provider";
            // 
            // cboClearQamProvider
            // 
            this.cboClearQamProvider.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboClearQamProvider.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboClearQamProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClearQamProvider.FormattingEnabled = true;
            this.cboClearQamProvider.Location = new System.Drawing.Point(220, 57);
            this.cboClearQamProvider.MaxDropDownItems = 20;
            this.cboClearQamProvider.Name = "cboClearQamProvider";
            this.cboClearQamProvider.Size = new System.Drawing.Size(221, 21);
            this.cboClearQamProvider.TabIndex = 250;
            this.cboClearQamProvider.SelectedIndexChanged += new System.EventHandler(this.cboClearQamProvider_SelectedIndexChanged);
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(100, 100);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(46, 13);
            this.label58.TabIndex = 145;
            this.label58.Text = "Channel";
            // 
            // cboClearQamScanningFrequency
            // 
            this.cboClearQamScanningFrequency.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboClearQamScanningFrequency.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboClearQamScanningFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClearQamScanningFrequency.FormattingEnabled = true;
            this.cboClearQamScanningFrequency.Location = new System.Drawing.Point(220, 97);
            this.cboClearQamScanningFrequency.MaxDropDownItems = 20;
            this.cboClearQamScanningFrequency.Name = "cboClearQamScanningFrequency";
            this.cboClearQamScanningFrequency.Size = new System.Drawing.Size(150, 21);
            this.cboClearQamScanningFrequency.TabIndex = 251;
            // 
            // tbpISDBSatellite
            // 
            this.tbpISDBSatellite.Controls.Add(this.btISDBLNBDefaults);
            this.tbpISDBSatellite.Controls.Add(this.txtISDBLNBSwitch);
            this.tbpISDBSatellite.Controls.Add(this.txtISDBLNBHigh);
            this.tbpISDBSatellite.Controls.Add(this.cboISDBSScanningFrequency);
            this.tbpISDBSatellite.Controls.Add(this.label61);
            this.tbpISDBSatellite.Controls.Add(this.txtISDBLNBLow);
            this.tbpISDBSatellite.Controls.Add(this.cboISDBSatellite);
            this.tbpISDBSatellite.Controls.Add(this.label62);
            this.tbpISDBSatellite.Controls.Add(this.cboISDBDiseqc);
            this.tbpISDBSatellite.Controls.Add(this.label63);
            this.tbpISDBSatellite.Controls.Add(this.label64);
            this.tbpISDBSatellite.Controls.Add(this.label65);
            this.tbpISDBSatellite.Controls.Add(this.label66);
            this.tbpISDBSatellite.Location = new System.Drawing.Point(4, 22);
            this.tbpISDBSatellite.Name = "tbpISDBSatellite";
            this.tbpISDBSatellite.Size = new System.Drawing.Size(404, 308);
            this.tbpISDBSatellite.TabIndex = 5;
            this.tbpISDBSatellite.Text = "ISDB Satellite";
            this.tbpISDBSatellite.UseVisualStyleBackColor = true;
            // 
            // btISDBLNBDefaults
            // 
            this.btISDBLNBDefaults.Location = new System.Drawing.Point(557, 138);
            this.btISDBLNBDefaults.Name = "btISDBLNBDefaults";
            this.btISDBLNBDefaults.Size = new System.Drawing.Size(75, 23);
            this.btISDBLNBDefaults.TabIndex = 241;
            this.btISDBLNBDefaults.Text = "Defaults";
            this.btISDBLNBDefaults.UseVisualStyleBackColor = true;
            // 
            // txtISDBLNBSwitch
            // 
            this.txtISDBLNBSwitch.Location = new System.Drawing.Point(557, 81);
            this.txtISDBLNBSwitch.Name = "txtISDBLNBSwitch";
            this.txtISDBLNBSwitch.Size = new System.Drawing.Size(92, 20);
            this.txtISDBLNBSwitch.TabIndex = 239;
            // 
            // txtISDBLNBHigh
            // 
            this.txtISDBLNBHigh.Location = new System.Drawing.Point(557, 51);
            this.txtISDBLNBHigh.Name = "txtISDBLNBHigh";
            this.txtISDBLNBHigh.Size = new System.Drawing.Size(92, 20);
            this.txtISDBLNBHigh.TabIndex = 238;
            // 
            // cboISDBSScanningFrequency
            // 
            this.cboISDBSScanningFrequency.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboISDBSScanningFrequency.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboISDBSScanningFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboISDBSScanningFrequency.FormattingEnabled = true;
            this.cboISDBSScanningFrequency.ItemHeight = 13;
            this.cboISDBSScanningFrequency.Location = new System.Drawing.Point(157, 51);
            this.cboISDBSScanningFrequency.MaxDropDownItems = 20;
            this.cboISDBSScanningFrequency.Name = "cboISDBSScanningFrequency";
            this.cboISDBSScanningFrequency.Size = new System.Drawing.Size(150, 21);
            this.cboISDBSScanningFrequency.TabIndex = 235;
            // 
            // label61
            // 
            this.label61.AutoSize = true;
            this.label61.Location = new System.Drawing.Point(32, 54);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(57, 13);
            this.label61.TabIndex = 232;
            this.label61.Text = "Frequency";
            // 
            // txtISDBLNBLow
            // 
            this.txtISDBLNBLow.Location = new System.Drawing.Point(557, 21);
            this.txtISDBLNBLow.Name = "txtISDBLNBLow";
            this.txtISDBLNBLow.Size = new System.Drawing.Size(92, 20);
            this.txtISDBLNBLow.TabIndex = 237;
            // 
            // cboISDBSatellite
            // 
            this.cboISDBSatellite.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboISDBSatellite.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboISDBSatellite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboISDBSatellite.FormattingEnabled = true;
            this.cboISDBSatellite.Location = new System.Drawing.Point(157, 21);
            this.cboISDBSatellite.MaxDropDownItems = 20;
            this.cboISDBSatellite.Name = "cboISDBSatellite";
            this.cboISDBSatellite.Size = new System.Drawing.Size(221, 21);
            this.cboISDBSatellite.TabIndex = 234;
            this.cboISDBSatellite.SelectedIndexChanged += new System.EventHandler(this.cboISDBSatellite_SelectedIndexChanged);
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(432, 114);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(44, 13);
            this.label62.TabIndex = 230;
            this.label62.Text = "DiSEqC";
            // 
            // cboISDBDiseqc
            // 
            this.cboISDBDiseqc.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboISDBDiseqc.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboISDBDiseqc.FormattingEnabled = true;
            this.cboISDBDiseqc.Items.AddRange(new object[] {
            "None",
            "A",
            "B",
            "AA",
            "AB",
            "BA",
            "BB"});
            this.cboISDBDiseqc.Location = new System.Drawing.Point(557, 111);
            this.cboISDBDiseqc.MaxDropDownItems = 20;
            this.cboISDBDiseqc.Name = "cboISDBDiseqc";
            this.cboISDBDiseqc.Size = new System.Drawing.Size(191, 21);
            this.cboISDBDiseqc.TabIndex = 240;
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(32, 24);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(44, 13);
            this.label63.TabIndex = 231;
            this.label63.Text = "Satellite";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(432, 84);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(63, 13);
            this.label64.TabIndex = 229;
            this.label64.Text = "LNB Switch";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(432, 24);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(79, 13);
            this.label65.TabIndex = 227;
            this.label65.Text = "LNB Low Band";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(432, 54);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(81, 13);
            this.label66.TabIndex = 228;
            this.label66.Text = "LNB High Band";
            // 
            // tbpISDBTerrestrial
            // 
            this.tbpISDBTerrestrial.Controls.Add(this.label67);
            this.tbpISDBTerrestrial.Controls.Add(this.cboISDBTProvider);
            this.tbpISDBTerrestrial.Controls.Add(this.label68);
            this.tbpISDBTerrestrial.Controls.Add(this.cboISDBTScanningFrequency);
            this.tbpISDBTerrestrial.Location = new System.Drawing.Point(4, 22);
            this.tbpISDBTerrestrial.Name = "tbpISDBTerrestrial";
            this.tbpISDBTerrestrial.Size = new System.Drawing.Size(404, 308);
            this.tbpISDBTerrestrial.TabIndex = 6;
            this.tbpISDBTerrestrial.Text = "ISDB Terrestrial";
            this.tbpISDBTerrestrial.UseVisualStyleBackColor = true;
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(100, 60);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(48, 13);
            this.label67.TabIndex = 250;
            this.label67.Text = "Location";
            // 
            // cboISDBTProvider
            // 
            this.cboISDBTProvider.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboISDBTProvider.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboISDBTProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboISDBTProvider.FormattingEnabled = true;
            this.cboISDBTProvider.Location = new System.Drawing.Point(210, 57);
            this.cboISDBTProvider.MaxDropDownItems = 20;
            this.cboISDBTProvider.Name = "cboISDBTProvider";
            this.cboISDBTProvider.Size = new System.Drawing.Size(221, 21);
            this.cboISDBTProvider.TabIndex = 252;
            this.cboISDBTProvider.SelectedIndexChanged += new System.EventHandler(this.cboISDBTProvider_SelectedIndexChanged);
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(100, 100);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(46, 13);
            this.label68.TabIndex = 251;
            this.label68.Text = "Channel";
            // 
            // cboISDBTScanningFrequency
            // 
            this.cboISDBTScanningFrequency.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboISDBTScanningFrequency.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboISDBTScanningFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboISDBTScanningFrequency.FormattingEnabled = true;
            this.cboISDBTScanningFrequency.Location = new System.Drawing.Point(210, 97);
            this.cboISDBTScanningFrequency.MaxDropDownItems = 20;
            this.cboISDBTScanningFrequency.Name = "cboISDBTScanningFrequency";
            this.cboISDBTScanningFrequency.Size = new System.Drawing.Size(150, 21);
            this.cboISDBTScanningFrequency.TabIndex = 253;
            // 
            // cboCableScanningFrequency
            // 
            this.cboCableScanningFrequency.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboCableScanningFrequency.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCableScanningFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCableScanningFrequency.FormattingEnabled = true;
            this.cboCableScanningFrequency.Location = new System.Drawing.Point(220, 97);
            this.cboCableScanningFrequency.MaxDropDownItems = 20;
            this.cboCableScanningFrequency.Name = "cboCableScanningFrequency";
            this.cboCableScanningFrequency.Size = new System.Drawing.Size(150, 21);
            this.cboCableScanningFrequency.TabIndex = 231;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(100, 100);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(0, 13);
            this.label52.TabIndex = 139;
            // 
            // cboCable
            // 
            this.cboCable.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboCable.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCable.FormattingEnabled = true;
            this.cboCable.Location = new System.Drawing.Point(220, 57);
            this.cboCable.MaxDropDownItems = 20;
            this.cboCable.Name = "cboCable";
            this.cboCable.Size = new System.Drawing.Size(221, 21);
            this.cboCable.TabIndex = 230;
            this.cboCable.SelectedIndexChanged += new System.EventHandler(this.cboCable_SelectedIndexChanged);
            // 
            // label54
            // 
            this.label54.AutoSize = true;
            this.label54.Location = new System.Drawing.Point(100, 60);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(0, 13);
            this.label54.TabIndex = 137;
            // 
            // tbpCable
            // 
            this.tbpCable.Controls.Add(this.label54);
            this.tbpCable.Controls.Add(this.cboCable);
            this.tbpCable.Controls.Add(this.label52);
            this.tbpCable.Controls.Add(this.cboCableScanningFrequency);
            this.tbpCable.Location = new System.Drawing.Point(4, 22);
            this.tbpCable.Name = "tbpCable";
            this.tbpCable.Size = new System.Drawing.Size(404, 308);
            this.tbpCable.TabIndex = 2;
            this.tbpCable.Text = "DVB Cable";
            this.tbpCable.UseVisualStyleBackColor = true;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(100, 120);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(0, 13);
            this.label24.TabIndex = 133;
            // 
            // cboDVBTScanningFrequency
            // 
            this.cboDVBTScanningFrequency.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboDVBTScanningFrequency.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboDVBTScanningFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDVBTScanningFrequency.FormattingEnabled = true;
            this.cboDVBTScanningFrequency.Location = new System.Drawing.Point(220, 117);
            this.cboDVBTScanningFrequency.MaxDropDownItems = 20;
            this.cboDVBTScanningFrequency.Name = "cboDVBTScanningFrequency";
            this.cboDVBTScanningFrequency.Size = new System.Drawing.Size(150, 21);
            this.cboDVBTScanningFrequency.TabIndex = 223;
            // 
            // cboCountry
            // 
            this.cboCountry.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboCountry.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCountry.FormattingEnabled = true;
            this.cboCountry.Location = new System.Drawing.Point(220, 37);
            this.cboCountry.MaxDropDownItems = 20;
            this.cboCountry.Name = "cboCountry";
            this.cboCountry.Size = new System.Drawing.Size(221, 21);
            this.cboCountry.TabIndex = 221;
            this.cboCountry.SelectedIndexChanged += new System.EventHandler(this.cboCountry_SelectedIndexChanged);
            // 
            // lblCountry
            // 
            this.lblCountry.AutoSize = true;
            this.lblCountry.Location = new System.Drawing.Point(100, 40);
            this.lblCountry.Name = "lblCountry";
            this.lblCountry.Size = new System.Drawing.Size(0, 13);
            this.lblCountry.TabIndex = 129;
            // 
            // cboArea
            // 
            this.cboArea.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboArea.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboArea.FormattingEnabled = true;
            this.cboArea.Location = new System.Drawing.Point(220, 77);
            this.cboArea.MaxDropDownItems = 20;
            this.cboArea.Name = "cboArea";
            this.cboArea.Size = new System.Drawing.Size(221, 21);
            this.cboArea.TabIndex = 222;
            this.cboArea.SelectedIndexChanged += new System.EventHandler(this.cboArea_SelectedIndexChanged);
            // 
            // lblArea
            // 
            this.lblArea.AutoSize = true;
            this.lblArea.Location = new System.Drawing.Point(100, 80);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(0, 13);
            this.lblArea.TabIndex = 131;
            // 
            // tbpTerrestrial
            // 
            this.tbpTerrestrial.Controls.Add(this.lblArea);
            this.tbpTerrestrial.Controls.Add(this.cboArea);
            this.tbpTerrestrial.Controls.Add(this.lblCountry);
            this.tbpTerrestrial.Controls.Add(this.cboCountry);
            this.tbpTerrestrial.Controls.Add(this.cboDVBTScanningFrequency);
            this.tbpTerrestrial.Controls.Add(this.label24);
            this.tbpTerrestrial.Location = new System.Drawing.Point(4, 22);
            this.tbpTerrestrial.Name = "tbpTerrestrial";
            this.tbpTerrestrial.Padding = new System.Windows.Forms.Padding(3);
            this.tbpTerrestrial.Size = new System.Drawing.Size(404, 308);
            this.tbpTerrestrial.TabIndex = 1;
            this.tbpTerrestrial.Text = "DVB Terrestrial";
            this.tbpTerrestrial.UseVisualStyleBackColor = true;
            // 
            // FrequencySelectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Name = "FrequencySelectionControl";
            this.Size = new System.Drawing.Size(455, 478);
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tbcDeliverySystem.ResumeLayout(false);
            this.tbpSatellite.ResumeLayout(false);
            this.tbpSatellite.PerformLayout();
            this.tbpAtsc.ResumeLayout(false);
            this.tbpAtsc.PerformLayout();
            this.tbpClearQAM.ResumeLayout(false);
            this.tbpClearQAM.PerformLayout();
            this.tbpISDBSatellite.ResumeLayout(false);
            this.tbpISDBSatellite.PerformLayout();
            this.tbpISDBTerrestrial.ResumeLayout(false);
            this.tbpISDBTerrestrial.PerformLayout();
            this.tbpCable.ResumeLayout(false);
            this.tbpCable.PerformLayout();
            this.tbpTerrestrial.ResumeLayout(false);
            this.tbpTerrestrial.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckedListBox clbTuners;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cbRepeatDiseqc;
        private System.Windows.Forms.CheckBox cbUseSignalPresent;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tbcDeliverySystem;
        private System.Windows.Forms.TabPage tbpSatellite;
        private System.Windows.Forms.Button btLNBDefaults;
        private System.Windows.Forms.TextBox txtLNBSwitch;
        private System.Windows.Forms.TextBox txtLNBHigh;
        private System.Windows.Forms.ComboBox cboDVBSScanningFrequency;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox txtLNBLow;
        private System.Windows.Forms.ComboBox cboSatellite;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.ComboBox cboDiseqc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TabPage tbpAtsc;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.ComboBox cboAtscProvider;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.ComboBox cboAtscScanningFrequency;
        private System.Windows.Forms.TabPage tbpClearQAM;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.ComboBox cboClearQamProvider;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.ComboBox cboClearQamScanningFrequency;
        private System.Windows.Forms.CheckBox cbSwitchAfterPlay;
        private System.Windows.Forms.TabPage tbpISDBSatellite;
        private System.Windows.Forms.TabPage tbpISDBTerrestrial;
        private System.Windows.Forms.Button btISDBLNBDefaults;
        private System.Windows.Forms.TextBox txtISDBLNBSwitch;
        private System.Windows.Forms.TextBox txtISDBLNBHigh;
        private System.Windows.Forms.ComboBox cboISDBSScanningFrequency;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.TextBox txtISDBLNBLow;
        private System.Windows.Forms.ComboBox cboISDBSatellite;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.ComboBox cboISDBDiseqc;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.ComboBox cboISDBTProvider;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.ComboBox cboISDBTScanningFrequency;
        private System.Windows.Forms.TabPage tbpTerrestrial;
        private System.Windows.Forms.Label lblArea;
        private System.Windows.Forms.ComboBox cboArea;
        private System.Windows.Forms.Label lblCountry;
        private System.Windows.Forms.ComboBox cboCountry;
        private System.Windows.Forms.ComboBox cboDVBTScanningFrequency;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TabPage tbpCable;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.ComboBox cboCable;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.ComboBox cboCableScanningFrequency;
    }
}
