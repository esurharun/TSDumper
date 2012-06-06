namespace EPGCentre
{
    partial class ChangeDishNetworkCategoriesControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgContents = new System.Windows.Forms.DataGridView();
            this.contentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.contentIDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subContentIDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.wmcDescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dvblogicDescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dvbViewerDescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgContents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contentBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dgContents
            // 
            this.dgContents.AutoGenerateColumns = false;
            this.dgContents.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgContents.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgContents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgContents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.contentIDColumn,
            this.subContentIDColumn,
            this.descriptionColumn,
            this.wmcDescriptionColumn,
            this.dvblogicDescriptionColumn,
            this.dvbViewerDescriptionColumn});
            this.dgContents.DataSource = this.contentBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgContents.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgContents.GridColor = System.Drawing.SystemColors.Control;
            this.dgContents.Location = new System.Drawing.Point(0, 0);
            this.dgContents.Name = "dgContents";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgContents.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgContents.RowHeadersVisible = false;
            this.dgContents.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgContents.RowTemplate.Height = 18;
            this.dgContents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgContents.Size = new System.Drawing.Size(950, 672);
            this.dgContents.TabIndex = 4;
            // 
            // contentBindingSource
            // 
            this.contentBindingSource.AllowNew = true;
            this.contentBindingSource.DataSource = typeof(DVBServices.DishNetworkProgramCategory);
            this.contentBindingSource.Sort = "";
            // 
            // contentIDColumn
            // 
            this.contentIDColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.contentIDColumn.DataPropertyName = "CategoryIDString";
            this.contentIDColumn.HeaderText = "Category ID";
            this.contentIDColumn.MaxInputLength = 3;
            this.contentIDColumn.Name = "contentIDColumn";
            this.contentIDColumn.Width = 88;
            // 
            // subContentIDColumn
            // 
            this.subContentIDColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.subContentIDColumn.DataPropertyName = "SubCategoryIDString";
            this.subContentIDColumn.HeaderText = "Sub-Category ID";
            this.subContentIDColumn.MaxInputLength = 3;
            this.subContentIDColumn.Name = "subContentIDColumn";
            this.subContentIDColumn.Width = 101;
            // 
            // descriptionColumn
            // 
            this.descriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.descriptionColumn.DataPropertyName = "DishNetworkDescription";
            this.descriptionColumn.HeaderText = "Dish Network Description";
            this.descriptionColumn.MaxInputLength = 256;
            this.descriptionColumn.Name = "descriptionColumn";
            this.descriptionColumn.Width = 139;
            // 
            // wmcDescriptionColumn
            // 
            this.wmcDescriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.wmcDescriptionColumn.DataPropertyName = "WMCDescription";
            this.wmcDescriptionColumn.HeaderText = "Media Centre Description";
            this.wmcDescriptionColumn.Name = "wmcDescriptionColumn";
            this.wmcDescriptionColumn.Width = 138;
            // 
            // dvblogicDescriptionColumn
            // 
            this.dvblogicDescriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dvblogicDescriptionColumn.DataPropertyName = "DVBLogicDescription";
            this.dvblogicDescriptionColumn.HeaderText = "DVBLogic Description";
            this.dvblogicDescriptionColumn.Name = "dvblogicDescriptionColumn";
            this.dvblogicDescriptionColumn.Width = 124;
            // 
            // dvbViewerDescriptionColumn
            // 
            this.dvbViewerDescriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dvbViewerDescriptionColumn.DataPropertyName = "DVBViewerDescription";
            this.dvbViewerDescriptionColumn.HeaderText = "DVBViewer Description";
            this.dvbViewerDescriptionColumn.Name = "dvbViewerDescriptionColumn";
            // 
            // ChangeDishNetworkCategoriesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgContents);
            this.Name = "ChangeDishNetworkCategoriesControl";
            this.Size = new System.Drawing.Size(950, 672);
            ((System.ComponentModel.ISupportInitialize)(this.dgContents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contentBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgContents;
        private System.Windows.Forms.BindingSource contentBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn contentIDColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn subContentIDColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn wmcDescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dvblogicDescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dvbViewerDescriptionColumn;
    }
}
