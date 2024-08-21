namespace Bulk_Log_Comparison_Tool_Frontend
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            panelPlayers = new Panel();
            panelStealth = new Panel();
            btnDeleteSelected = new Button();
            btnOpenLogs = new Button();
            lbLoadedFiles = new ListBox();
            btnStealth = new Button();
            panel2 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            cbStealthPhase = new ComboBox();
            label1 = new Label();
            panelStealth.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panelPlayers
            // 
            panelPlayers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelPlayers.AutoScroll = true;
            panelPlayers.Location = new Point(5, 36);
            panelPlayers.Name = "panelPlayers";
            panelPlayers.Size = new Size(1066, 25);
            panelPlayers.TabIndex = 4;
            // 
            // panelStealth
            // 
            panelStealth.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            panelStealth.Controls.Add(btnDeleteSelected);
            panelStealth.Controls.Add(btnOpenLogs);
            panelStealth.Controls.Add(lbLoadedFiles);
            panelStealth.Location = new Point(1077, 4);
            panelStealth.Name = "panelStealth";
            panelStealth.Size = new Size(172, 670);
            panelStealth.TabIndex = 2;
            // 
            // btnDeleteSelected
            // 
            btnDeleteSelected.Location = new Point(9, 32);
            btnDeleteSelected.Name = "btnDeleteSelected";
            btnDeleteSelected.Size = new Size(160, 23);
            btnDeleteSelected.TabIndex = 2;
            btnDeleteSelected.Text = "Delete Selected";
            btnDeleteSelected.UseVisualStyleBackColor = true;
            btnDeleteSelected.Click += btnDeleteSelected_Click;
            // 
            // btnOpenLogs
            // 
            btnOpenLogs.Location = new Point(9, 5);
            btnOpenLogs.Name = "btnOpenLogs";
            btnOpenLogs.Size = new Size(160, 23);
            btnOpenLogs.TabIndex = 0;
            btnOpenLogs.Text = "Open Logs";
            btnOpenLogs.UseVisualStyleBackColor = true;
            btnOpenLogs.Click += btnOpenLogs_Click;
            // 
            // lbLoadedFiles
            // 
            lbLoadedFiles.FormattingEnabled = true;
            lbLoadedFiles.ItemHeight = 15;
            lbLoadedFiles.Location = new Point(9, 68);
            lbLoadedFiles.Name = "lbLoadedFiles";
            lbLoadedFiles.SelectionMode = SelectionMode.MultiExtended;
            lbLoadedFiles.Size = new Size(160, 589);
            lbLoadedFiles.TabIndex = 0;
            // 
            // btnStealth
            // 
            btnStealth.Location = new Point(3, 3);
            btnStealth.Name = "btnStealth";
            btnStealth.Size = new Size(99, 23);
            btnStealth.TabIndex = 2;
            btnStealth.Text = "Stealth Analysis";
            btnStealth.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.Controls.Add(btnStealth);
            panel2.Location = new Point(5, 4);
            panel2.Name = "panel2";
            panel2.Size = new Size(1066, 28);
            panel2.TabIndex = 6;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel1.Location = new Point(5, 96);
            tableLayoutPanel1.MaximumSize = new Size(1050, 800);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 250F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 250F));
            tableLayoutPanel1.Size = new Size(1050, 578);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // cbStealthPhase
            // 
            cbStealthPhase.FormattingEnabled = true;
            cbStealthPhase.Location = new Point(49, 67);
            cbStealthPhase.Name = "cbStealthPhase";
            cbStealthPhase.Size = new Size(121, 23);
            cbStealthPhase.TabIndex = 1;
            cbStealthPhase.SelectedIndexChanged += cbStealthPhase_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(5, 70);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 7;
            label1.Text = "Phase";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1261, 681);
            Controls.Add(label1);
            Controls.Add(cbStealthPhase);
            Controls.Add(panelPlayers);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(panel2);
            Controls.Add(panelStealth);
            MaximizeBox = false;
            MaximumSize = new Size(1277, 720);
            MinimumSize = new Size(1277, 720);
            Name = "Form1";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "BLCT";
            panelStealth.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Button btnOpenLogs;
        private Button btnStealth;
        private Panel panelStealth;
        private ListBox lbLoadedFiles;
        private Button btnDeleteSelected;
        private Panel panelPlayers;
        private Panel panel2;
        private TableLayoutPanel tableLayoutPanel1;
        private ComboBox cbStealthPhase;
        private Label label1;
    }
}
