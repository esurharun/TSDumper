namespace TSDumper
{
    partial class TransportStreamDumpControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransportStreamDumpControl));
            this.lblScanning = new System.Windows.Forms.Label();
            this.cmdScan = new System.Windows.Forms.Button();
            this.gpCustomPids = new System.Windows.Forms.GroupBox();
            this.tbPidList = new System.Windows.Forms.TextBox();
            this.cbHexPids = new System.Windows.Forms.CheckBox();
            this.lblEITPid = new System.Windows.Forms.Label();
            this.gpOutputFile = new System.Windows.Forms.GroupBox();
            this.txtOutputFile = new System.Windows.Forms.TextBox();
            this.lblOutputFile = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.gpTimeouts = new System.Windows.Forms.GroupBox();
            this.start_hour_textbox = new System.Windows.Forms.MaskedTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btTimeoutDefaults = new System.Windows.Forms.Button();
            this.nudSignalLockTimeout = new System.Windows.Forms.NumericUpDown();
            this.nudDataCollectionTimeout = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pbarProgress = new System.Windows.Forms.ProgressBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.before_recording_complete_script_path = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.after_recording_complete_script_path = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.last_log = new System.Windows.Forms.ListBox();
            this.frequencySelectionControl = new TSDumper.FrequencySelectionControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.gpCustomPids.SuspendLayout();
            this.gpOutputFile.SuspendLayout();
            this.gpTimeouts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignalLockTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDataCollectionTimeout)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblScanning
            // 
            this.lblScanning.Location = new System.Drawing.Point(669, 645);
            this.lblScanning.Name = "lblScanning";
            this.lblScanning.Size = new System.Drawing.Size(264, 18);
            this.lblScanning.TabIndex = 102;
            this.lblScanning.Text = "File size: 0Mb";
            this.lblScanning.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblScanning.Visible = false;
            // 
            // cmdScan
            // 
            this.cmdScan.Location = new System.Drawing.Point(17, 629);
            this.cmdScan.Name = "cmdScan";
            this.cmdScan.Size = new System.Drawing.Size(88, 25);
            this.cmdScan.TabIndex = 100;
            this.cmdScan.Text = "Start Dump";
            this.cmdScan.UseVisualStyleBackColor = true;
            this.cmdScan.Click += new System.EventHandler(this.cmdScan_Click);
            // 
            // gpCustomPids
            // 
            this.gpCustomPids.Controls.Add(this.tbPidList);
            this.gpCustomPids.Controls.Add(this.cbHexPids);
            this.gpCustomPids.Controls.Add(this.lblEITPid);
            this.gpCustomPids.Location = new System.Drawing.Point(692, 485);
            this.gpCustomPids.Name = "gpCustomPids";
            this.gpCustomPids.Size = new System.Drawing.Size(241, 60);
            this.gpCustomPids.TabIndex = 76;
            this.gpCustomPids.TabStop = false;
            this.gpCustomPids.Text = "Custom PID\'s";
            // 
            // tbPidList
            // 
            this.tbPidList.Location = new System.Drawing.Point(82, 22);
            this.tbPidList.Name = "tbPidList";
            this.tbPidList.Size = new System.Drawing.Size(141, 20);
            this.tbPidList.TabIndex = 79;
            // 
            // cbHexPids
            // 
            this.cbHexPids.AutoSize = true;
            this.cbHexPids.Location = new System.Drawing.Point(101, 1);
            this.cbHexPids.Name = "cbHexPids";
            this.cbHexPids.Size = new System.Drawing.Size(122, 17);
            this.cbHexPids.TabIndex = 77;
            this.cbHexPids.Text = "Hexadecimal Values";
            this.cbHexPids.UseVisualStyleBackColor = true;
            // 
            // lblEITPid
            // 
            this.lblEITPid.AutoSize = true;
            this.lblEITPid.Location = new System.Drawing.Point(10, 26);
            this.lblEITPid.Name = "lblEITPid";
            this.lblEITPid.Size = new System.Drawing.Size(41, 13);
            this.lblEITPid.TabIndex = 78;
            this.lblEITPid.Text = "Pid List";
            // 
            // gpOutputFile
            // 
            this.gpOutputFile.Controls.Add(this.txtOutputFile);
            this.gpOutputFile.Controls.Add(this.lblOutputFile);
            this.gpOutputFile.Controls.Add(this.btnBrowse);
            this.gpOutputFile.Location = new System.Drawing.Point(17, 557);
            this.gpOutputFile.Name = "gpOutputFile";
            this.gpOutputFile.Size = new System.Drawing.Size(444, 53);
            this.gpOutputFile.TabIndex = 90;
            this.gpOutputFile.TabStop = false;
            this.gpOutputFile.Text = "Output Directory";
            this.gpOutputFile.Enter += new System.EventHandler(this.gpOutputFile_Enter);
            // 
            // txtOutputFile
            // 
            this.txtOutputFile.Location = new System.Drawing.Point(105, 19);
            this.txtOutputFile.Name = "txtOutputFile";
            this.txtOutputFile.Size = new System.Drawing.Size(228, 20);
            this.txtOutputFile.TabIndex = 92;
            // 
            // lblOutputFile
            // 
            this.lblOutputFile.AutoSize = true;
            this.lblOutputFile.Location = new System.Drawing.Point(10, 22);
            this.lblOutputFile.Name = "lblOutputFile";
            this.lblOutputFile.Size = new System.Drawing.Size(29, 13);
            this.lblOutputFile.TabIndex = 91;
            this.lblOutputFile.Text = "Path";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(340, 17);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(92, 24);
            this.btnBrowse.TabIndex = 93;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // gpTimeouts
            // 
            this.gpTimeouts.Controls.Add(this.start_hour_textbox);
            this.gpTimeouts.Controls.Add(this.label3);
            this.gpTimeouts.Controls.Add(this.btTimeoutDefaults);
            this.gpTimeouts.Controls.Add(this.nudSignalLockTimeout);
            this.gpTimeouts.Controls.Add(this.nudDataCollectionTimeout);
            this.gpTimeouts.Controls.Add(this.label2);
            this.gpTimeouts.Controls.Add(this.label1);
            this.gpTimeouts.Location = new System.Drawing.Point(17, 485);
            this.gpTimeouts.Name = "gpTimeouts";
            this.gpTimeouts.Size = new System.Drawing.Size(669, 60);
            this.gpTimeouts.TabIndex = 70;
            this.gpTimeouts.TabStop = false;
            this.gpTimeouts.Text = "Timeouts";
            // 
            // start_hour_textbox
            // 
            this.start_hour_textbox.Location = new System.Drawing.Point(275, 23);
            this.start_hour_textbox.Mask = "00:00";
            this.start_hour_textbox.Name = "start_hour_textbox";
            this.start_hour_textbox.Size = new System.Drawing.Size(46, 20);
            this.start_hour_textbox.TabIndex = 77;
            this.start_hour_textbox.Text = "0000";
            this.start_hour_textbox.ValidatingType = typeof(System.DateTime);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(195, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 76;
            this.label3.Text = "Start hour:";
            // 
            // btTimeoutDefaults
            // 
            this.btTimeoutDefaults.Location = new System.Drawing.Point(582, 22);
            this.btTimeoutDefaults.Name = "btTimeoutDefaults";
            this.btTimeoutDefaults.Size = new System.Drawing.Size(75, 23);
            this.btTimeoutDefaults.TabIndex = 75;
            this.btTimeoutDefaults.Text = "Defaults";
            this.btTimeoutDefaults.UseVisualStyleBackColor = true;
            this.btTimeoutDefaults.Click += new System.EventHandler(this.btTimeoutDefaults_Click);
            // 
            // nudSignalLockTimeout
            // 
            this.nudSignalLockTimeout.Location = new System.Drawing.Point(123, 22);
            this.nudSignalLockTimeout.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudSignalLockTimeout.Name = "nudSignalLockTimeout";
            this.nudSignalLockTimeout.Size = new System.Drawing.Size(48, 20);
            this.nudSignalLockTimeout.TabIndex = 72;
            // 
            // nudDataCollectionTimeout
            // 
            this.nudDataCollectionTimeout.Location = new System.Drawing.Point(488, 23);
            this.nudDataCollectionTimeout.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudDataCollectionTimeout.Name = "nudDataCollectionTimeout";
            this.nudDataCollectionTimeout.Size = new System.Drawing.Size(70, 20);
            this.nudDataCollectionTimeout.TabIndex = 74;
            this.nudDataCollectionTimeout.Value = new decimal(new int[] {
            7200,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(345, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 73;
            this.label2.Text = "Recording interval (sec)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 71;
            this.label1.Text = "Signal Lock (sec)";
            // 
            // pbarProgress
            // 
            this.pbarProgress.Enabled = false;
            this.pbarProgress.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.pbarProgress.Location = new System.Drawing.Point(123, 629);
            this.pbarProgress.Maximum = 500;
            this.pbarProgress.Name = "pbarProgress";
            this.pbarProgress.Size = new System.Drawing.Size(810, 13);
            this.pbarProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbarProgress.TabIndex = 311;
            this.pbarProgress.Visible = false;
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Pattern special characters";
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.before_recording_complete_script_path);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.after_recording_complete_script_path);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(475, 267);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(455, 208);
            this.groupBox1.TabIndex = 313;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scripts";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label6.Location = new System.Drawing.Point(13, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(428, 104);
            this.label6.TabIndex = 100;
            this.label6.Text = resources.GetString("label6.Text");
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // before_recording_complete_script_path
            // 
            this.before_recording_complete_script_path.Location = new System.Drawing.Point(146, 44);
            this.before_recording_complete_script_path.Name = "before_recording_complete_script_path";
            this.before_recording_complete_script_path.Size = new System.Drawing.Size(209, 20);
            this.before_recording_complete_script_path.TabIndex = 98;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 13);
            this.label5.TabIndex = 97;
            this.label5.Text = "Before recording starts";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(372, 42);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(62, 24);
            this.button2.TabIndex = 99;
            this.button2.Text = "Browse...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // after_recording_complete_script_path
            // 
            this.after_recording_complete_script_path.Location = new System.Drawing.Point(146, 18);
            this.after_recording_complete_script_path.Name = "after_recording_complete_script_path";
            this.after_recording_complete_script_path.Size = new System.Drawing.Size(209, 20);
            this.after_recording_complete_script_path.TabIndex = 95;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 13);
            this.label4.TabIndex = 94;
            this.label4.Text = "After recording complete";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(372, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(62, 24);
            this.button1.TabIndex = 96;
            this.button1.Text = "Browse...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 250;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // last_log
            // 
            this.last_log.FormattingEnabled = true;
            this.last_log.Location = new System.Drawing.Point(475, 11);
            this.last_log.Name = "last_log";
            this.last_log.Size = new System.Drawing.Size(455, 238);
            this.last_log.TabIndex = 314;
            // 
            // frequencySelectionControl
            // 
            this.frequencySelectionControl.Location = new System.Drawing.Point(10, 0);
            this.frequencySelectionControl.Name = "frequencySelectionControl";
            this.frequencySelectionControl.Size = new System.Drawing.Size(451, 475);
            this.frequencySelectionControl.TabIndex = 103;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Location = new System.Drawing.Point(475, 557);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(455, 53);
            this.groupBox2.TabIndex = 317;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(233, 18);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(103, 23);
            this.button4.TabIndex = 318;
            this.button4.Text = "Save settings";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(342, 18);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(103, 23);
            this.button3.TabIndex = 317;
            this.button3.Text = "Load settings";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // TransportStreamDumpControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.last_log);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pbarProgress);
            this.Controls.Add(this.frequencySelectionControl);
            this.Controls.Add(this.lblScanning);
            this.Controls.Add(this.cmdScan);
            this.Controls.Add(this.gpCustomPids);
            this.Controls.Add(this.gpOutputFile);
            this.Controls.Add(this.gpTimeouts);
            this.Name = "TransportStreamDumpControl";
            this.Size = new System.Drawing.Size(950, 672);
            this.Load += new System.EventHandler(this.TransportStreamDumpControl_Load);
            this.gpCustomPids.ResumeLayout(false);
            this.gpCustomPids.PerformLayout();
            this.gpOutputFile.ResumeLayout(false);
            this.gpOutputFile.PerformLayout();
            this.gpTimeouts.ResumeLayout(false);
            this.gpTimeouts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignalLockTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDataCollectionTimeout)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblScanning;
        private System.Windows.Forms.Button cmdScan;
        private System.Windows.Forms.GroupBox gpCustomPids;
        private System.Windows.Forms.TextBox tbPidList;
        private System.Windows.Forms.CheckBox cbHexPids;
        private System.Windows.Forms.Label lblEITPid;
        private System.Windows.Forms.GroupBox gpOutputFile;
        private System.Windows.Forms.TextBox txtOutputFile;
        private System.Windows.Forms.Label lblOutputFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox gpTimeouts;
        private System.Windows.Forms.Button btTimeoutDefaults;
        private System.Windows.Forms.NumericUpDown nudSignalLockTimeout;
        private System.Windows.Forms.NumericUpDown nudDataCollectionTimeout;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private FrequencySelectionControl frequencySelectionControl;
        
        private System.Windows.Forms.ProgressBar pbarProgress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox start_hour_textbox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox before_recording_complete_script_path;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox after_recording_complete_script_path;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListBox last_log;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
    }
}
