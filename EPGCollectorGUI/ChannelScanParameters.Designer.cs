namespace EPGCentre
{
    partial class ChannelScanParameters
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChannelScanParameters));
            this.btLNBDefaults = new System.Windows.Forms.Button();
            this.txtLNBSwitch = new System.Windows.Forms.TextBox();
            this.txtLNBHigh = new System.Windows.Forms.TextBox();
            this.txtLNBLow = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.cboDiseqc = new System.Windows.Forms.ComboBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.clbTuners = new System.Windows.Forms.CheckedListBox();
            this.gpDish = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gpOptions = new System.Windows.Forms.GroupBox();
            this.cbRepeatDiseqc = new System.Windows.Forms.CheckBox();
            this.cbUseSignalPresent = new System.Windows.Forms.CheckBox();
            this.cbSwitchAfterPlay = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.gpDish.SuspendLayout();
            this.gpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btLNBDefaults
            // 
            this.btLNBDefaults.Location = new System.Drawing.Point(439, 79);
            this.btLNBDefaults.Name = "btLNBDefaults";
            this.btLNBDefaults.Size = new System.Drawing.Size(75, 23);
            this.btLNBDefaults.TabIndex = 120;
            this.btLNBDefaults.Text = "Defaults";
            this.btLNBDefaults.UseVisualStyleBackColor = true;
            this.btLNBDefaults.Click += new System.EventHandler(this.btLNBDefaults_Click);
            // 
            // txtLNBSwitch
            // 
            this.txtLNBSwitch.Location = new System.Drawing.Point(104, 80);
            this.txtLNBSwitch.Name = "txtLNBSwitch";
            this.txtLNBSwitch.Size = new System.Drawing.Size(92, 20);
            this.txtLNBSwitch.TabIndex = 117;
            // 
            // txtLNBHigh
            // 
            this.txtLNBHigh.Location = new System.Drawing.Point(309, 36);
            this.txtLNBHigh.Name = "txtLNBHigh";
            this.txtLNBHigh.Size = new System.Drawing.Size(92, 20);
            this.txtLNBHigh.TabIndex = 115;
            // 
            // txtLNBLow
            // 
            this.txtLNBLow.Location = new System.Drawing.Point(104, 37);
            this.txtLNBLow.Name = "txtLNBLow";
            this.txtLNBLow.Size = new System.Drawing.Size(92, 20);
            this.txtLNBLow.TabIndex = 113;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(224, 83);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(79, 13);
            this.label36.TabIndex = 118;
            this.label36.Text = "DiSEqC Switch";
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
            this.cboDiseqc.Location = new System.Drawing.Point(309, 79);
            this.cboDiseqc.Name = "cboDiseqc";
            this.cboDiseqc.Size = new System.Drawing.Size(104, 21);
            this.cboDiseqc.TabIndex = 119;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(10, 83);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(63, 13);
            this.label31.TabIndex = 116;
            this.label31.Text = "LNB Switch";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(10, 40);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(79, 13);
            this.label33.TabIndex = 112;
            this.label33.Text = "LNB Low Band";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(222, 40);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(81, 13);
            this.label32.TabIndex = 114;
            this.label32.Text = "LNB High Band";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.clbTuners);
            this.groupBox3.Location = new System.Drawing.Point(12, 21);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(530, 113);
            this.groupBox3.TabIndex = 121;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tuner";
            // 
            // clbTuners
            // 
            this.clbTuners.CheckOnClick = true;
            this.clbTuners.FormattingEnabled = true;
            this.clbTuners.Location = new System.Drawing.Point(13, 19);
            this.clbTuners.Name = "clbTuners";
            this.clbTuners.Size = new System.Drawing.Size(501, 79);
            this.clbTuners.TabIndex = 101;
            this.clbTuners.SelectedIndexChanged += new System.EventHandler(this.clbTuners_SelectedIndexChanged);
            // 
            // gpDish
            // 
            this.gpDish.Controls.Add(this.label33);
            this.gpDish.Controls.Add(this.txtLNBLow);
            this.gpDish.Controls.Add(this.btLNBDefaults);
            this.gpDish.Controls.Add(this.label32);
            this.gpDish.Controls.Add(this.cboDiseqc);
            this.gpDish.Controls.Add(this.label36);
            this.gpDish.Controls.Add(this.txtLNBSwitch);
            this.gpDish.Controls.Add(this.txtLNBHigh);
            this.gpDish.Controls.Add(this.label31);
            this.gpDish.Location = new System.Drawing.Point(12, 140);
            this.gpDish.Name = "gpDish";
            this.gpDish.Size = new System.Drawing.Size(530, 129);
            this.gpDish.TabIndex = 122;
            this.gpDish.TabStop = false;
            this.gpDish.Text = "Dish Parameters";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(157, 418);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 130;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(328, 418);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 131;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // gpOptions
            // 
            this.gpOptions.Controls.Add(this.cbSwitchAfterPlay);
            this.gpOptions.Controls.Add(this.cbRepeatDiseqc);
            this.gpOptions.Controls.Add(this.cbUseSignalPresent);
            this.gpOptions.Location = new System.Drawing.Point(12, 286);
            this.gpOptions.Name = "gpOptions";
            this.gpOptions.Size = new System.Drawing.Size(530, 121);
            this.gpOptions.TabIndex = 125;
            this.gpOptions.TabStop = false;
            this.gpOptions.Text = "Tuning Options";
            // 
            // cbRepeatDiseqc
            // 
            this.cbRepeatDiseqc.AutoSize = true;
            this.cbRepeatDiseqc.Location = new System.Drawing.Point(28, 92);
            this.cbRepeatDiseqc.Name = "cbRepeatDiseqc";
            this.cbRepeatDiseqc.Size = new System.Drawing.Size(236, 17);
            this.cbRepeatDiseqc.TabIndex = 128;
            this.cbRepeatDiseqc.Text = "Repeat DiSEqC command if first attempt fails";
            this.cbRepeatDiseqc.UseVisualStyleBackColor = true;
            // 
            // cbUseSignalPresent
            // 
            this.cbUseSignalPresent.AutoSize = true;
            this.cbUseSignalPresent.Location = new System.Drawing.Point(28, 24);
            this.cbUseSignalPresent.Name = "cbUseSignalPresent";
            this.cbUseSignalPresent.Size = new System.Drawing.Size(335, 17);
            this.cbUseSignalPresent.TabIndex = 126;
            this.cbUseSignalPresent.Text = "Use signal present when tuning if signal lock and signal quality fail";
            this.cbUseSignalPresent.UseVisualStyleBackColor = true;
            // 
            // cbSwitchAfterPlay
            // 
            this.cbSwitchAfterPlay.AutoSize = true;
            this.cbSwitchAfterPlay.Location = new System.Drawing.Point(28, 58);
            this.cbSwitchAfterPlay.Name = "cbSwitchAfterPlay";
            this.cbSwitchAfterPlay.Size = new System.Drawing.Size(250, 17);
            this.cbSwitchAfterPlay.TabIndex = 127;
            this.cbSwitchAfterPlay.Text = "Change DiSEqC switch only after graph running";
            this.cbSwitchAfterPlay.UseVisualStyleBackColor = true;
            // 
            // ChannelScanParameters
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(560, 452);
            this.Controls.Add(this.gpOptions);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gpDish);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChannelScanParameters";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EPG Centre - Select Parameters For Channel Scan";
            this.groupBox3.ResumeLayout(false);
            this.gpDish.ResumeLayout(false);
            this.gpDish.PerformLayout();
            this.gpOptions.ResumeLayout(false);
            this.gpOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btLNBDefaults;
        private System.Windows.Forms.TextBox txtLNBSwitch;
        private System.Windows.Forms.TextBox txtLNBHigh;
        private System.Windows.Forms.TextBox txtLNBLow;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.ComboBox cboDiseqc;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckedListBox clbTuners;
        private System.Windows.Forms.GroupBox gpDish;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gpOptions;
        private System.Windows.Forms.CheckBox cbUseSignalPresent;
        private System.Windows.Forms.CheckBox cbRepeatDiseqc;
        private System.Windows.Forms.CheckBox cbSwitchAfterPlay;
    }
}