namespace EPGCentre
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
            this.btTimeoutDefaults = new System.Windows.Forms.Button();
            this.nudSignalLockTimeout = new System.Windows.Forms.NumericUpDown();
            this.nudDataCollectionTimeout = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.frequencySelectionControl = new EPGCentre.FrequencySelectionControl();
            this.pbarProgress = new System.Windows.Forms.ProgressBar();
            this.gpCustomPids.SuspendLayout();
            this.gpOutputFile.SuspendLayout();
            this.gpTimeouts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignalLockTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDataCollectionTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // lblScanning
            // 
            this.lblScanning.Location = new System.Drawing.Point(120, 645);
            this.lblScanning.Name = "lblScanning";
            this.lblScanning.Size = new System.Drawing.Size(264, 18);
            this.lblScanning.TabIndex = 102;
            this.lblScanning.Text = "File size: 0Mb";
            this.lblScanning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.gpCustomPids.Location = new System.Drawing.Point(534, 484);
            this.gpCustomPids.Name = "gpCustomPids";
            this.gpCustomPids.Size = new System.Drawing.Size(399, 59);
            this.gpCustomPids.TabIndex = 76;
            this.gpCustomPids.TabStop = false;
            this.gpCustomPids.Text = "Custom PID\'s";
            // 
            // tbPidList
            // 
            this.tbPidList.Location = new System.Drawing.Point(82, 26);
            this.tbPidList.Name = "tbPidList";
            this.tbPidList.Size = new System.Drawing.Size(289, 20);
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
            this.lblEITPid.Location = new System.Drawing.Point(10, 31);
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
            this.gpOutputFile.Size = new System.Drawing.Size(916, 53);
            this.gpOutputFile.TabIndex = 90;
            this.gpOutputFile.TabStop = false;
            this.gpOutputFile.Text = "Output File";
            // 
            // txtOutputFile
            // 
            this.txtOutputFile.Location = new System.Drawing.Point(105, 19);
            this.txtOutputFile.Name = "txtOutputFile";
            this.txtOutputFile.Size = new System.Drawing.Size(692, 20);
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
            this.btnBrowse.Location = new System.Drawing.Point(813, 16);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(92, 24);
            this.btnBrowse.TabIndex = 93;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // gpTimeouts
            // 
            this.gpTimeouts.Controls.Add(this.btTimeoutDefaults);
            this.gpTimeouts.Controls.Add(this.nudSignalLockTimeout);
            this.gpTimeouts.Controls.Add(this.nudDataCollectionTimeout);
            this.gpTimeouts.Controls.Add(this.label2);
            this.gpTimeouts.Controls.Add(this.label1);
            this.gpTimeouts.Location = new System.Drawing.Point(17, 485);
            this.gpTimeouts.Name = "gpTimeouts";
            this.gpTimeouts.Size = new System.Drawing.Size(491, 60);
            this.gpTimeouts.TabIndex = 70;
            this.gpTimeouts.TabStop = false;
            this.gpTimeouts.Text = "Timeouts";
            // 
            // btTimeoutDefaults
            // 
            this.btTimeoutDefaults.Location = new System.Drawing.Point(391, 21);
            this.btTimeoutDefaults.Name = "btTimeoutDefaults";
            this.btTimeoutDefaults.Size = new System.Drawing.Size(75, 23);
            this.btTimeoutDefaults.TabIndex = 75;
            this.btTimeoutDefaults.Text = "Defaults";
            this.btTimeoutDefaults.UseVisualStyleBackColor = true;
            this.btTimeoutDefaults.Click += new System.EventHandler(this.btTimeoutDefaults_Click);
            // 
            // nudSignalLockTimeout
            // 
            this.nudSignalLockTimeout.Location = new System.Drawing.Point(105, 23);
            this.nudSignalLockTimeout.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudSignalLockTimeout.Name = "nudSignalLockTimeout";
            this.nudSignalLockTimeout.Size = new System.Drawing.Size(48, 20);
            this.nudSignalLockTimeout.TabIndex = 72;
            // 
            // nudDataCollectionTimeout
            // 
            this.nudDataCollectionTimeout.Location = new System.Drawing.Point(313, 23);
            this.nudDataCollectionTimeout.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudDataCollectionTimeout.Name = "nudDataCollectionTimeout";
            this.nudDataCollectionTimeout.Size = new System.Drawing.Size(48, 20);
            this.nudDataCollectionTimeout.TabIndex = 74;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(195, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 73;
            this.label2.Text = "Data Collection (sec)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 71;
            this.label1.Text = "Signal Lock (sec)";
            // 
            // frequencySelectionControl
            // 
            this.frequencySelectionControl.Location = new System.Drawing.Point(10, 0);
            this.frequencySelectionControl.Name = "frequencySelectionControl";
            this.frequencySelectionControl.Size = new System.Drawing.Size(930, 475);
            this.frequencySelectionControl.TabIndex = 103;
            // 
            // pbarProgress
            // 
            this.pbarProgress.Enabled = false;
            this.pbarProgress.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.pbarProgress.Location = new System.Drawing.Point(123, 628);
            this.pbarProgress.Maximum = 500;
            this.pbarProgress.Name = "pbarProgress";
            this.pbarProgress.Size = new System.Drawing.Size(270, 14);
            this.pbarProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbarProgress.TabIndex = 311;
            this.pbarProgress.Visible = false;
            // 
            // TransportStreamDumpControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbarProgress);
            this.Controls.Add(this.frequencySelectionControl);
            this.Controls.Add(this.lblScanning);
            this.Controls.Add(this.cmdScan);
            this.Controls.Add(this.gpCustomPids);
            this.Controls.Add(this.gpOutputFile);
            this.Controls.Add(this.gpTimeouts);
            this.Name = "TransportStreamDumpControl";
            this.Size = new System.Drawing.Size(950, 672);
            this.gpCustomPids.ResumeLayout(false);
            this.gpCustomPids.PerformLayout();
            this.gpOutputFile.ResumeLayout(false);
            this.gpOutputFile.PerformLayout();
            this.gpTimeouts.ResumeLayout(false);
            this.gpTimeouts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignalLockTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDataCollectionTimeout)).EndInit();
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
    }
}
