namespace EPGCentre
{
    partial class SaveMHWCategoryFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveMHWCategoryFile));
            this.cboDVBSScanningFrequency = new System.Windows.Forms.ComboBox();
            this.label29 = new System.Windows.Forms.Label();
            this.cboSatellite = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cboDVBSScanningFrequency
            // 
            this.cboDVBSScanningFrequency.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboDVBSScanningFrequency.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboDVBSScanningFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDVBSScanningFrequency.FormattingEnabled = true;
            this.cboDVBSScanningFrequency.ItemHeight = 13;
            this.cboDVBSScanningFrequency.Location = new System.Drawing.Point(87, 61);
            this.cboDVBSScanningFrequency.MaxDropDownItems = 20;
            this.cboDVBSScanningFrequency.Name = "cboDVBSScanningFrequency";
            this.cboDVBSScanningFrequency.Size = new System.Drawing.Size(145, 21);
            this.cboDVBSScanningFrequency.TabIndex = 4;
            this.cboDVBSScanningFrequency.SelectedIndexChanged += new System.EventHandler(this.cboDVBSScanningFrequency_SelectedIndexChanged);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(21, 64);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(57, 13);
            this.label29.TabIndex = 3;
            this.label29.Text = "Frequency";
            // 
            // cboSatellite
            // 
            this.cboSatellite.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboSatellite.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSatellite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSatellite.FormattingEnabled = true;
            this.cboSatellite.Location = new System.Drawing.Point(87, 22);
            this.cboSatellite.MaxDropDownItems = 20;
            this.cboSatellite.Name = "cboSatellite";
            this.cboSatellite.Size = new System.Drawing.Size(221, 21);
            this.cboSatellite.TabIndex = 2;
            this.cboSatellite.SelectedIndexChanged += new System.EventHandler(this.cboSatellite_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Satellite";
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(196, 145);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 11;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(74, 145);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 10;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Type";
            // 
            // cbType
            // 
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            "MediaHighway1",
            "MediaHighway2"});
            this.cbType.Location = new System.Drawing.Point(87, 103);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(145, 21);
            this.cbType.TabIndex = 6;
            // 
            // SaveMHWCategoryFile
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(345, 183);
            this.ControlBox = false;
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.cboDVBSScanningFrequency);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.cboSatellite);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SaveMHWCategoryFile";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EPG Centre - Save MHW Category File";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboDVBSScanningFrequency;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.ComboBox cboSatellite;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbType;
    }
}