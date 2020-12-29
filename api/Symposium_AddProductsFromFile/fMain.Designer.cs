namespace Symposium_AddProductsFromFile
{
    partial class fMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fMain));
            this.grpTop = new System.Windows.Forms.GroupBox();
            this.ctlUnit = new System.Windows.Forms.ComboBox();
            this.lblUnit = new System.Windows.Forms.Label();
            this.ctlRateCode = new System.Windows.Forms.ComboBox();
            this.lblRateCode = new System.Windows.Forms.Label();
            this.chkFirstRow = new System.Windows.Forms.CheckBox();
            this.btnReadWorkBook = new System.Windows.Forms.Button();
            this.ctlSelSheet = new System.Windows.Forms.ComboBox();
            this.lblSelSheet = new System.Windows.Forms.Label();
            this.btnSelFile = new System.Windows.Forms.Button();
            this.ctlSelFile = new System.Windows.Forms.TextBox();
            this.lblSelFile = new System.Windows.Forms.Label();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReadData = new System.Windows.Forms.ToolStripMenuItem();
            this.sep_1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuStart = new System.Windows.Forms.ToolStripMenuItem();
            this.grbProg = new System.Windows.Forms.GroupBox();
            this.lblTotRecs = new System.Windows.Forms.Label();
            this.lblProg = new System.Windows.Forms.Label();
            this.prgMain = new System.Windows.Forms.ProgressBar();
            this.pgMain = new System.Windows.Forms.TabControl();
            this.tbsGrid = new System.Windows.Forms.TabPage();
            this.grdMain = new System.Windows.Forms.DataGridView();
            this.tbsResult = new System.Windows.Forms.TabPage();
            this.ctlErrors = new System.Windows.Forms.TextBox();
            this.ctlCostRateCode = new System.Windows.Forms.ComboBox();
            this.lblCostRateCode = new System.Windows.Forms.Label();
            this.grpTop.SuspendLayout();
            this.mnuMain.SuspendLayout();
            this.grbProg.SuspendLayout();
            this.pgMain.SuspendLayout();
            this.tbsGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdMain)).BeginInit();
            this.tbsResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpTop
            // 
            this.grpTop.Controls.Add(this.lblCostRateCode);
            this.grpTop.Controls.Add(this.ctlCostRateCode);
            this.grpTop.Controls.Add(this.ctlUnit);
            this.grpTop.Controls.Add(this.lblUnit);
            this.grpTop.Controls.Add(this.ctlRateCode);
            this.grpTop.Controls.Add(this.lblRateCode);
            this.grpTop.Controls.Add(this.chkFirstRow);
            this.grpTop.Controls.Add(this.btnReadWorkBook);
            this.grpTop.Controls.Add(this.ctlSelSheet);
            this.grpTop.Controls.Add(this.lblSelSheet);
            this.grpTop.Controls.Add(this.btnSelFile);
            this.grpTop.Controls.Add(this.ctlSelFile);
            this.grpTop.Controls.Add(this.lblSelFile);
            this.grpTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTop.Location = new System.Drawing.Point(0, 24);
            this.grpTop.Name = "grpTop";
            this.grpTop.Size = new System.Drawing.Size(864, 67);
            this.grpTop.TabIndex = 0;
            this.grpTop.TabStop = false;
            // 
            // ctlUnit
            // 
            this.ctlUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ctlUnit.FormattingEnabled = true;
            this.ctlUnit.Location = new System.Drawing.Point(357, 37);
            this.ctlUnit.Name = "ctlUnit";
            this.ctlUnit.Size = new System.Drawing.Size(97, 21);
            this.ctlUnit.TabIndex = 4;
            // 
            // lblUnit
            // 
            this.lblUnit.AutoSize = true;
            this.lblUnit.Location = new System.Drawing.Point(332, 41);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(26, 13);
            this.lblUnit.TabIndex = 7;
            this.lblUnit.Text = "Unit";
            // 
            // ctlRateCode
            // 
            this.ctlRateCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ctlRateCode.FormattingEnabled = true;
            this.ctlRateCode.Location = new System.Drawing.Point(59, 37);
            this.ctlRateCode.Name = "ctlRateCode";
            this.ctlRateCode.Size = new System.Drawing.Size(117, 21);
            this.ctlRateCode.TabIndex = 2;
            // 
            // lblRateCode
            // 
            this.lblRateCode.AutoSize = true;
            this.lblRateCode.Location = new System.Drawing.Point(4, 41);
            this.lblRateCode.Name = "lblRateCode";
            this.lblRateCode.Size = new System.Drawing.Size(50, 13);
            this.lblRateCode.TabIndex = 5;
            this.lblRateCode.Text = "Price List";
            // 
            // chkFirstRow
            // 
            this.chkFirstRow.AutoSize = true;
            this.chkFirstRow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFirstRow.Checked = true;
            this.chkFirstRow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFirstRow.Location = new System.Drawing.Point(455, 41);
            this.chkFirstRow.Name = "chkFirstRow";
            this.chkFirstRow.Size = new System.Drawing.Size(146, 17);
            this.chkFirstRow.TabIndex = 5;
            this.chkFirstRow.Text = "First row is columns name";
            this.chkFirstRow.UseVisualStyleBackColor = true;
            // 
            // btnReadWorkBook
            // 
            this.btnReadWorkBook.Location = new System.Drawing.Point(797, 38);
            this.btnReadWorkBook.Name = "btnReadWorkBook";
            this.btnReadWorkBook.Size = new System.Drawing.Size(55, 23);
            this.btnReadWorkBook.TabIndex = 7;
            this.btnReadWorkBook.Text = "Read";
            this.btnReadWorkBook.UseVisualStyleBackColor = true;
            this.btnReadWorkBook.Click += new System.EventHandler(this.btnReadWorkBook_Click);
            // 
            // ctlSelSheet
            // 
            this.ctlSelSheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ctlSelSheet.FormattingEnabled = true;
            this.ctlSelSheet.Location = new System.Drawing.Point(672, 39);
            this.ctlSelSheet.Name = "ctlSelSheet";
            this.ctlSelSheet.Size = new System.Drawing.Size(121, 21);
            this.ctlSelSheet.TabIndex = 6;
            // 
            // lblSelSheet
            // 
            this.lblSelSheet.AutoSize = true;
            this.lblSelSheet.Location = new System.Drawing.Point(601, 43);
            this.lblSelSheet.Name = "lblSelSheet";
            this.lblSelSheet.Size = new System.Drawing.Size(68, 13);
            this.lblSelSheet.TabIndex = 4;
            this.lblSelSheet.Text = "Select Sheet";
            // 
            // btnSelFile
            // 
            this.btnSelFile.Location = new System.Drawing.Point(830, 12);
            this.btnSelFile.Name = "btnSelFile";
            this.btnSelFile.Size = new System.Drawing.Size(25, 23);
            this.btnSelFile.TabIndex = 1;
            this.btnSelFile.Text = "...";
            this.btnSelFile.UseVisualStyleBackColor = true;
            this.btnSelFile.Click += new System.EventHandler(this.btnSelFile_Click);
            // 
            // ctlSelFile
            // 
            this.ctlSelFile.Location = new System.Drawing.Point(59, 13);
            this.ctlSelFile.Name = "ctlSelFile";
            this.ctlSelFile.Size = new System.Drawing.Size(765, 20);
            this.ctlSelFile.TabIndex = 0;
            // 
            // lblSelFile
            // 
            this.lblSelFile.AutoSize = true;
            this.lblSelFile.Location = new System.Drawing.Point(4, 17);
            this.lblSelFile.Name = "lblSelFile";
            this.lblSelFile.Size = new System.Drawing.Size(56, 13);
            this.lblSelFile.TabIndex = 1;
            this.lblSelFile.Text = "Select File";
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuExit,
            this.mnuActions});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(864, 24);
            this.mnuMain.TabIndex = 1;
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(37, 20);
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuActions
            // 
            this.mnuActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLoad,
            this.mnuReadData,
            this.sep_1,
            this.mnuStart});
            this.mnuActions.Name = "mnuActions";
            this.mnuActions.Size = new System.Drawing.Size(59, 20);
            this.mnuActions.Text = "Actions";
            // 
            // mnuLoad
            // 
            this.mnuLoad.Name = "mnuLoad";
            this.mnuLoad.Size = new System.Drawing.Size(127, 22);
            this.mnuLoad.Text = "Load File";
            this.mnuLoad.Click += new System.EventHandler(this.mnuLoad_Click);
            // 
            // mnuReadData
            // 
            this.mnuReadData.Name = "mnuReadData";
            this.mnuReadData.Size = new System.Drawing.Size(127, 22);
            this.mnuReadData.Text = "Read Data";
            this.mnuReadData.Click += new System.EventHandler(this.mnuReadData_Click);
            // 
            // sep_1
            // 
            this.sep_1.Name = "sep_1";
            this.sep_1.Size = new System.Drawing.Size(124, 6);
            // 
            // mnuStart
            // 
            this.mnuStart.Name = "mnuStart";
            this.mnuStart.Size = new System.Drawing.Size(127, 22);
            this.mnuStart.Text = "Start";
            this.mnuStart.Click += new System.EventHandler(this.mnuStart_Click);
            // 
            // grbProg
            // 
            this.grbProg.Controls.Add(this.lblTotRecs);
            this.grbProg.Controls.Add(this.lblProg);
            this.grbProg.Controls.Add(this.prgMain);
            this.grbProg.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grbProg.Location = new System.Drawing.Point(0, 347);
            this.grbProg.Name = "grbProg";
            this.grbProg.Size = new System.Drawing.Size(864, 81);
            this.grbProg.TabIndex = 3;
            this.grbProg.TabStop = false;
            this.grbProg.Text = "Current Progress";
            // 
            // lblTotRecs
            // 
            this.lblTotRecs.AutoSize = true;
            this.lblTotRecs.Location = new System.Drawing.Point(9, 16);
            this.lblTotRecs.Name = "lblTotRecs";
            this.lblTotRecs.Size = new System.Drawing.Size(16, 13);
            this.lblTotRecs.TabIndex = 2;
            this.lblTotRecs.Text = "...";
            // 
            // lblProg
            // 
            this.lblProg.AutoSize = true;
            this.lblProg.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblProg.Location = new System.Drawing.Point(3, 42);
            this.lblProg.Name = "lblProg";
            this.lblProg.Size = new System.Drawing.Size(16, 13);
            this.lblProg.TabIndex = 1;
            this.lblProg.Text = "...";
            // 
            // prgMain
            // 
            this.prgMain.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.prgMain.Location = new System.Drawing.Point(3, 55);
            this.prgMain.Name = "prgMain";
            this.prgMain.Size = new System.Drawing.Size(858, 23);
            this.prgMain.TabIndex = 0;
            // 
            // pgMain
            // 
            this.pgMain.Controls.Add(this.tbsGrid);
            this.pgMain.Controls.Add(this.tbsResult);
            this.pgMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgMain.Location = new System.Drawing.Point(0, 91);
            this.pgMain.Name = "pgMain";
            this.pgMain.SelectedIndex = 0;
            this.pgMain.Size = new System.Drawing.Size(864, 256);
            this.pgMain.TabIndex = 4;
            // 
            // tbsGrid
            // 
            this.tbsGrid.Controls.Add(this.grdMain);
            this.tbsGrid.Location = new System.Drawing.Point(4, 22);
            this.tbsGrid.Name = "tbsGrid";
            this.tbsGrid.Padding = new System.Windows.Forms.Padding(3);
            this.tbsGrid.Size = new System.Drawing.Size(856, 230);
            this.tbsGrid.TabIndex = 0;
            this.tbsGrid.Text = "Grid View";
            this.tbsGrid.UseVisualStyleBackColor = true;
            // 
            // grdMain
            // 
            this.grdMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdMain.Location = new System.Drawing.Point(3, 3);
            this.grdMain.Name = "grdMain";
            this.grdMain.ReadOnly = true;
            this.grdMain.Size = new System.Drawing.Size(850, 224);
            this.grdMain.TabIndex = 3;
            // 
            // tbsResult
            // 
            this.tbsResult.Controls.Add(this.ctlErrors);
            this.tbsResult.Location = new System.Drawing.Point(4, 22);
            this.tbsResult.Name = "tbsResult";
            this.tbsResult.Padding = new System.Windows.Forms.Padding(3);
            this.tbsResult.Size = new System.Drawing.Size(856, 230);
            this.tbsResult.TabIndex = 1;
            this.tbsResult.Text = "Resutls";
            this.tbsResult.UseVisualStyleBackColor = true;
            // 
            // ctlErrors
            // 
            this.ctlErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlErrors.Location = new System.Drawing.Point(3, 3);
            this.ctlErrors.Multiline = true;
            this.ctlErrors.Name = "ctlErrors";
            this.ctlErrors.ReadOnly = true;
            this.ctlErrors.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ctlErrors.Size = new System.Drawing.Size(850, 224);
            this.ctlErrors.TabIndex = 0;
            // 
            // ctlCostRateCode
            // 
            this.ctlCostRateCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ctlCostRateCode.FormattingEnabled = true;
            this.ctlCostRateCode.Location = new System.Drawing.Point(238, 37);
            this.ctlCostRateCode.Name = "ctlCostRateCode";
            this.ctlCostRateCode.Size = new System.Drawing.Size(88, 21);
            this.ctlCostRateCode.TabIndex = 3;
            // 
            // lblCostRateCode
            // 
            this.lblCostRateCode.AutoSize = true;
            this.lblCostRateCode.Location = new System.Drawing.Point(182, 42);
            this.lblCostRateCode.Name = "lblCostRateCode";
            this.lblCostRateCode.Size = new System.Drawing.Size(50, 13);
            this.lblCostRateCode.TabIndex = 10;
            this.lblCostRateCode.Text = "Cost P.L.";
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 428);
            this.Controls.Add(this.pgMain);
            this.Controls.Add(this.grbProg);
            this.Controls.Add(this.grpTop);
            this.Controls.Add(this.mnuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuMain;
            this.Name = "fMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Insert products from file";
            this.Load += new System.EventHandler(this.fMain_Load);
            this.grpTop.ResumeLayout(false);
            this.grpTop.PerformLayout();
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.grbProg.ResumeLayout(false);
            this.grbProg.PerformLayout();
            this.pgMain.ResumeLayout(false);
            this.tbsGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdMain)).EndInit();
            this.tbsResult.ResumeLayout(false);
            this.tbsResult.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpTop;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.Button btnSelFile;
        private System.Windows.Forms.TextBox ctlSelFile;
        private System.Windows.Forms.Label lblSelFile;
        private System.Windows.Forms.ToolStripMenuItem mnuActions;
        private System.Windows.Forms.ToolStripMenuItem mnuLoad;
        private System.Windows.Forms.ComboBox ctlSelSheet;
        private System.Windows.Forms.Label lblSelSheet;
        private System.Windows.Forms.Button btnReadWorkBook;
        private System.Windows.Forms.CheckBox chkFirstRow;
        private System.Windows.Forms.GroupBox grbProg;
        private System.Windows.Forms.TabControl pgMain;
        private System.Windows.Forms.TabPage tbsGrid;
        private System.Windows.Forms.DataGridView grdMain;
        private System.Windows.Forms.TabPage tbsResult;
        private System.Windows.Forms.ToolStripMenuItem mnuReadData;
        private System.Windows.Forms.ToolStripSeparator sep_1;
        private System.Windows.Forms.ToolStripMenuItem mnuStart;
        private System.Windows.Forms.Label lblTotRecs;
        private System.Windows.Forms.Label lblProg;
        private System.Windows.Forms.ProgressBar prgMain;
        private System.Windows.Forms.TextBox ctlErrors;
        private System.Windows.Forms.ComboBox ctlRateCode;
        private System.Windows.Forms.Label lblRateCode;
        private System.Windows.Forms.ComboBox ctlUnit;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.Label lblCostRateCode;
        private System.Windows.Forms.ComboBox ctlCostRateCode;
    }
}

