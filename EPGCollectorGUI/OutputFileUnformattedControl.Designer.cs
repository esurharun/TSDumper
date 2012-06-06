namespace EPGCentre
{
    partial class OutputFileUnformattedControl
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
            this.dgViewFile = new System.Windows.Forms.DataGridView();
            this.detailColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgViewFile)).BeginInit();
            this.SuspendLayout();
            // 
            // dgViewFile
            // 
            this.dgViewFile.AllowUserToAddRows = false;
            this.dgViewFile.AllowUserToDeleteRows = false;
            this.dgViewFile.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgViewFile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgViewFile.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.detailColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgViewFile.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgViewFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgViewFile.GridColor = System.Drawing.SystemColors.Control;
            this.dgViewFile.Location = new System.Drawing.Point(0, 0);
            this.dgViewFile.Name = "dgViewFile";
            this.dgViewFile.ReadOnly = true;
            this.dgViewFile.RowHeadersVisible = false;
            this.dgViewFile.RowTemplate.Height = 16;
            this.dgViewFile.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgViewFile.ShowCellErrors = false;
            this.dgViewFile.ShowCellToolTips = false;
            this.dgViewFile.ShowEditingIcon = false;
            this.dgViewFile.ShowRowErrors = false;
            this.dgViewFile.Size = new System.Drawing.Size(950, 672);
            this.dgViewFile.TabIndex = 1;
            // 
            // detailColumn
            // 
            this.detailColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.detailColumn.HeaderText = "Detail";
            this.detailColumn.Name = "detailColumn";
            this.detailColumn.ReadOnly = true;
            // 
            // OutputFileUnformattedControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgViewFile);
            this.Name = "OutputFileUnformattedControl";
            this.Size = new System.Drawing.Size(950, 672);
            ((System.ComponentModel.ISupportInitialize)(this.dgViewFile)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgViewFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn detailColumn;
    }
}
