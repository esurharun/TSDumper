namespace EPGCentre
{
    partial class FindEPGControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btTimeoutDefaults = new System.Windows.Forms.Button();
            this.nudSignalLockTimeout = new System.Windows.Forms.NumericUpDown();
            this.dgViewResults = new System.Windows.Forms.DataGridView();
            this.frequencyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.epgTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.commentColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nudDataCollectionTimeout = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.gpTimeouts = new System.Windows.Forms.GroupBox();
            this.frequencySelectionControl = new EPGCentre.FrequencySelectionControl();
            this.btSetUpFind = new System.Windows.Forms.Button();
            this.tbDumpFile = new System.Windows.Forms.TextBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.tbFindDump = new System.Windows.Forms.Button();
            this.lblScanning = new System.Windows.Forms.Label();
            this.cmdScan = new System.Windows.Forms.Button();
            this.pbarProgress = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.nudSignalLockTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgViewResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDataCollectionTimeout)).BeginInit();
            this.gpTimeouts.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.SuspendLayout();
            // 
            // btTimeoutDefaults
            // 
            this.btTimeoutDefaults.Location = new System.Drawing.Point(391, 21);
            this.btTimeoutDefaults.Name = "btTimeoutDefaults";
            this.btTimeoutDefaults.Size = new System.Drawing.Size(75, 23);
            this.btTimeoutDefaults.TabIndex = 75;
            this.btTimeoutDefaults.Text = "Defaults";
            this.btTimeoutDefaults.UseVisualStyleBackColor = true;
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
            // dgViewResults
            // 
            this.dgViewResults.AllowUserToAddRows = false;
            this.dgViewResults.AllowUserToDeleteRows = false;
            this.dgViewResults.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgViewResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgViewResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.frequencyColumn,
            this.epgTypeColumn,
            this.commentColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgViewResults.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgViewResults.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgViewResults.GridColor = System.Drawing.SystemColors.Control;
            this.dgViewResults.Location = new System.Drawing.Point(0, 0);
            this.dgViewResults.MultiSelect = false;
            this.dgViewResults.Name = "dgViewResults";
            this.dgViewResults.ReadOnly = true;
            this.dgViewResults.RowHeadersVisible = false;
            this.dgViewResults.RowTemplate.Height = 14;
            this.dgViewResults.RowTemplate.ReadOnly = true;
            this.dgViewResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgViewResults.ShowCellErrors = false;
            this.dgViewResults.ShowCellToolTips = false;
            this.dgViewResults.ShowEditingIcon = false;
            this.dgViewResults.ShowRowErrors = false;
            this.dgViewResults.Size = new System.Drawing.Size(950, 621);
            this.dgViewResults.TabIndex = 912;
            this.dgViewResults.Visible = false;
            // 
            // frequencyColumn
            // 
            this.frequencyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.frequencyColumn.HeaderText = "Frequency";
            this.frequencyColumn.Name = "frequencyColumn";
            this.frequencyColumn.ReadOnly = true;
            this.frequencyColumn.Width = 82;
            // 
            // epgTypeColumn
            // 
            this.epgTypeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.epgTypeColumn.HeaderText = "EPG Type";
            this.epgTypeColumn.Name = "epgTypeColumn";
            this.epgTypeColumn.ReadOnly = true;
            this.epgTypeColumn.Width = 81;
            // 
            // commentColumn
            // 
            this.commentColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.commentColumn.HeaderText = "Comment";
            this.commentColumn.Name = "commentColumn";
            this.commentColumn.ReadOnly = true;
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
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(10, 27);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(71, 13);
            this.label30.TabIndex = 701;
            this.label30.Text = "TS Dump File";
            // 
            // gpTimeouts
            // 
            this.gpTimeouts.Controls.Add(this.btTimeoutDefaults);
            this.gpTimeouts.Controls.Add(this.nudSignalLockTimeout);
            this.gpTimeouts.Controls.Add(this.nudDataCollectionTimeout);
            this.gpTimeouts.Controls.Add(this.label2);
            this.gpTimeouts.Controls.Add(this.label1);
            this.gpTimeouts.Location = new System.Drawing.Point(18, 487);
            this.gpTimeouts.Name = "gpTimeouts";
            this.gpTimeouts.Size = new System.Drawing.Size(918, 60);
            this.gpTimeouts.TabIndex = 911;
            this.gpTimeouts.TabStop = false;
            this.gpTimeouts.Text = "Timeouts";
            // 
            // frequencySelectionControl
            // 
            this.frequencySelectionControl.Location = new System.Drawing.Point(10, 5);
            this.frequencySelectionControl.Name = "frequencySelectionControl";
            this.frequencySelectionControl.Size = new System.Drawing.Size(930, 475);
            this.frequencySelectionControl.TabIndex = 910;
            // 
            // btSetUpFind
            // 
            this.btSetUpFind.Location = new System.Drawing.Point(17, 635);
            this.btSetUpFind.Name = "btSetUpFind";
            this.btSetUpFind.Size = new System.Drawing.Size(95, 23);
            this.btSetUpFind.TabIndex = 909;
            this.btSetUpFind.Text = "Set Up Find";
            this.btSetUpFind.UseVisualStyleBackColor = true;
            this.btSetUpFind.Visible = false;
            this.btSetUpFind.Click += new System.EventHandler(this.btSetUpAnalysis_Click);
            // 
            // tbDumpFile
            // 
            this.tbDumpFile.Location = new System.Drawing.Point(100, 24);
            this.tbDumpFile.Name = "tbDumpFile";
            this.tbDumpFile.Size = new System.Drawing.Size(684, 20);
            this.tbDumpFile.TabIndex = 702;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.tbFindDump);
            this.groupBox11.Controls.Add(this.tbDumpFile);
            this.groupBox11.Controls.Add(this.label30);
            this.groupBox11.Location = new System.Drawing.Point(19, 558);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(915, 63);
            this.groupBox11.TabIndex = 905;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Simulation";
            // 
            // tbFindDump
            // 
            this.tbFindDump.Location = new System.Drawing.Point(801, 21);
            this.tbFindDump.Name = "tbFindDump";
            this.tbFindDump.Size = new System.Drawing.Size(75, 23);
            this.tbFindDump.TabIndex = 703;
            this.tbFindDump.Text = "Browse";
            this.tbFindDump.UseVisualStyleBackColor = true;
            this.tbFindDump.Click += new System.EventHandler(this.tbFindDump_Click);
            // 
            // lblScanning
            // 
            this.lblScanning.Location = new System.Drawing.Point(120, 650);
            this.lblScanning.Name = "lblScanning";
            this.lblScanning.Size = new System.Drawing.Size(264, 18);
            this.lblScanning.TabIndex = 908;
            this.lblScanning.Text = "Scanning";
            this.lblScanning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblScanning.Visible = false;
            // 
            // cmdScan
            // 
            this.cmdScan.Location = new System.Drawing.Point(17, 634);
            this.cmdScan.Name = "cmdScan";
            this.cmdScan.Size = new System.Drawing.Size(88, 25);
            this.cmdScan.TabIndex = 906;
            this.cmdScan.Text = "Start Find";
            this.cmdScan.UseVisualStyleBackColor = true;
            this.cmdScan.Click += new System.EventHandler(this.cmdScan_Click);
            // 
            // pbarProgress
            // 
            this.pbarProgress.Enabled = false;
            this.pbarProgress.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.pbarProgress.Location = new System.Drawing.Point(133, 635);
            this.pbarProgress.Maximum = 500;
            this.pbarProgress.Name = "pbarProgress";
            this.pbarProgress.Size = new System.Drawing.Size(270, 14);
            this.pbarProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbarProgress.TabIndex = 913;
            this.pbarProgress.Visible = false;
            // 
            // FindEPGControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbarProgress);
            this.Controls.Add(this.dgViewResults);
            this.Controls.Add(this.gpTimeouts);
            this.Controls.Add(this.frequencySelectionControl);
            this.Controls.Add(this.btSetUpFind);
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.lblScanning);
            this.Controls.Add(this.cmdScan);
            this.Name = "FindEPGControl";
            this.Size = new System.Drawing.Size(950, 672);
            ((System.ComponentModel.ISupportInitialize)(this.nudSignalLockTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgViewResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDataCollectionTimeout)).EndInit();
            this.gpTimeouts.ResumeLayout(false);
            this.gpTimeouts.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btTimeoutDefaults;
        private System.Windows.Forms.NumericUpDown nudSignalLockTimeout;
        private System.Windows.Forms.DataGridView dgViewResults;
        private System.Windows.Forms.DataGridViewTextBoxColumn frequencyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn epgTypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn commentColumn;
        private System.Windows.Forms.NumericUpDown nudDataCollectionTimeout;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.GroupBox gpTimeouts;
        private FrequencySelectionControl frequencySelectionControl;
        private System.Windows.Forms.Button btSetUpFind;
        private System.Windows.Forms.TextBox tbDumpFile;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Button tbFindDump;
        private System.Windows.Forms.Label lblScanning;
        private System.Windows.Forms.Button cmdScan;
        private System.Windows.Forms.ProgressBar pbarProgress;

    }
}
