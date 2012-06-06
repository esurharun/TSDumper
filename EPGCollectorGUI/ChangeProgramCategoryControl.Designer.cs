namespace EPGCentre
{
    partial class ChangeProgramCategoryControl
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
            this.dgCategories = new System.Windows.Forms.DataGridView();
            this.categoryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.categoryColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.wmcDescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dvblogicDescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dvbviewerDescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgCategories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.categoryBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dgCategories
            // 
            this.dgCategories.AutoGenerateColumns = false;
            this.dgCategories.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgCategories.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgCategories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgCategories.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.categoryColumn,
            this.descriptionColumn,
            this.wmcDescriptionColumn,
            this.dvblogicDescriptionColumn,
            this.dvbviewerDescriptionColumn});
            this.dgCategories.DataSource = this.categoryBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgCategories.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgCategories.GridColor = System.Drawing.SystemColors.Control;
            this.dgCategories.Location = new System.Drawing.Point(0, 0);
            this.dgCategories.Name = "dgCategories";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgCategories.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgCategories.RowHeadersVisible = false;
            this.dgCategories.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgCategories.RowTemplate.Height = 18;
            this.dgCategories.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgCategories.Size = new System.Drawing.Size(950, 672);
            this.dgCategories.TabIndex = 2;
            this.dgCategories.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgCategoriesColumnHeaderMouseClick);
            this.dgCategories.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgCategoriesDefaultValuesNeeded);
            this.dgCategories.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgCategories_EditingControlShowing);
            this.dgCategories.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgCategoriesRowValidating);
            // 
            // categoryBindingSource
            // 
            this.categoryBindingSource.AllowNew = true;
            this.categoryBindingSource.DataSource = typeof(DVBServices.OpenTVProgramCategory);
            this.categoryBindingSource.Sort = "";
            // 
            // categoryColumn
            // 
            this.categoryColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.categoryColumn.DataPropertyName = "CategoryIDString";
            this.categoryColumn.HeaderText = "Category ID";
            this.categoryColumn.MaxInputLength = 3;
            this.categoryColumn.Name = "categoryColumn";
            this.categoryColumn.Width = 88;
            // 
            // descriptionColumn
            // 
            this.descriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.descriptionColumn.DataPropertyName = "OpenTVDescription";
            this.descriptionColumn.HeaderText = "OpenTV Description";
            this.descriptionColumn.Name = "descriptionColumn";
            this.descriptionColumn.Width = 117;
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
            // dvbviewerDescriptionColumn
            // 
            this.dvbviewerDescriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dvbviewerDescriptionColumn.DataPropertyName = "DVBViewerDescription";
            this.dvbviewerDescriptionColumn.HeaderText = "DVBViewer Description";
            this.dvbviewerDescriptionColumn.Name = "dvbviewerDescriptionColumn";
            this.dvbviewerDescriptionColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ChangeProgramCategoryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgCategories);
            this.Name = "ChangeProgramCategoryControl";
            this.Size = new System.Drawing.Size(950, 672);
            ((System.ComponentModel.ISupportInitialize)(this.dgCategories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.categoryBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgCategories;
        private System.Windows.Forms.BindingSource categoryBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn categoryColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn wmcDescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dvblogicDescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dvbviewerDescriptionColumn;
    }
}
