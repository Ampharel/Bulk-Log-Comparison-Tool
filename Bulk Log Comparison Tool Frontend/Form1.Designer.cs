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
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            tabBoons = new TabPage();
            lblSelectedBoonBoons = new Label();
            lblSelectedPhaseBoons = new Label();
            label5 = new Label();
            cbBoonBoons = new ComboBox();
            label4 = new Label();
            cbBoonPhase = new ComboBox();
            tableBoons = new DataGridView();
            tabDps = new TabPage();
            lblSelectedPhaseDps = new Label();
            label3 = new Label();
            cbDpsPhase = new ComboBox();
            tableDps = new DataGridView();
            tabStealth = new TabPage();
            lblSelectedPhaseStealth = new Label();
            label1 = new Label();
            cbStealthPhase = new ComboBox();
            tableStealth = new DataGridView();
            tabsControl = new TabControl();
            tabShockwaves = new TabPage();
            tableShockwave = new DataGridView();
            panelStealth.SuspendLayout();
            tabBoons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tableBoons).BeginInit();
            tabDps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tableDps).BeginInit();
            tabStealth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tableStealth).BeginInit();
            tabsControl.SuspendLayout();
            tabShockwaves.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tableShockwave).BeginInit();
            SuspendLayout();
            // 
            // panelPlayers
            // 
            panelPlayers.AutoScroll = true;
            panelPlayers.BackColor = SystemColors.Control;
            panelPlayers.Dock = DockStyle.Top;
            panelPlayers.Location = new Point(0, 0);
            panelPlayers.Name = "panelPlayers";
            panelPlayers.Size = new Size(1089, 50);
            panelPlayers.TabIndex = 4;
            // 
            // panelStealth
            // 
            panelStealth.Controls.Add(btnDeleteSelected);
            panelStealth.Controls.Add(btnOpenLogs);
            panelStealth.Controls.Add(lbLoadedFiles);
            panelStealth.Dock = DockStyle.Right;
            panelStealth.Location = new Point(1089, 0);
            panelStealth.Name = "panelStealth";
            panelStealth.Size = new Size(172, 681);
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
            lbLoadedFiles.BackColor = SystemColors.Control;
            lbLoadedFiles.ForeColor = SystemColors.Menu;
            lbLoadedFiles.FormattingEnabled = true;
            lbLoadedFiles.ItemHeight = 15;
            lbLoadedFiles.Location = new Point(9, 68);
            lbLoadedFiles.Name = "lbLoadedFiles";
            lbLoadedFiles.SelectionMode = SelectionMode.MultiExtended;
            lbLoadedFiles.Size = new Size(160, 589);
            lbLoadedFiles.TabIndex = 0;
            // 
            // tabBoons
            // 
            tabBoons.Controls.Add(lblSelectedBoonBoons);
            tabBoons.Controls.Add(lblSelectedPhaseBoons);
            tabBoons.Controls.Add(label5);
            tabBoons.Controls.Add(cbBoonBoons);
            tabBoons.Controls.Add(label4);
            tabBoons.Controls.Add(cbBoonPhase);
            tabBoons.Controls.Add(tableBoons);
            tabBoons.Location = new Point(4, 24);
            tabBoons.Name = "tabBoons";
            tabBoons.Size = new Size(1081, 603);
            tabBoons.TabIndex = 2;
            tabBoons.Text = "Boons";
            tabBoons.UseVisualStyleBackColor = true;
            // 
            // lblSelectedBoonBoons
            // 
            lblSelectedBoonBoons.AutoSize = true;
            lblSelectedBoonBoons.Location = new Point(523, 6);
            lblSelectedBoonBoons.Name = "lblSelectedBoonBoons";
            lblSelectedBoonBoons.Size = new Size(38, 15);
            lblSelectedBoonBoons.TabIndex = 16;
            lblSelectedBoonBoons.Text = "label2";
            // 
            // lblSelectedPhaseBoons
            // 
            lblSelectedPhaseBoons.AutoSize = true;
            lblSelectedPhaseBoons.Location = new Point(177, 6);
            lblSelectedPhaseBoons.Name = "lblSelectedPhaseBoons";
            lblSelectedPhaseBoons.Size = new Size(38, 15);
            lblSelectedPhaseBoons.TabIndex = 15;
            lblSelectedPhaseBoons.Text = "label2";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(352, 6);
            label5.Name = "label5";
            label5.Size = new Size(35, 15);
            label5.TabIndex = 14;
            label5.Text = "Boon";
            // 
            // cbBoonBoons
            // 
            cbBoonBoons.FormattingEnabled = true;
            cbBoonBoons.Location = new Point(396, 3);
            cbBoonBoons.Name = "cbBoonBoons";
            cbBoonBoons.Size = new Size(121, 23);
            cbBoonBoons.TabIndex = 13;
            cbBoonBoons.SelectedIndexChanged += cbBoonBoons_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 6);
            label4.Name = "label4";
            label4.Size = new Size(38, 15);
            label4.TabIndex = 12;
            label4.Text = "Phase";
            // 
            // cbBoonPhase
            // 
            cbBoonPhase.FormattingEnabled = true;
            cbBoonPhase.Location = new Point(50, 3);
            cbBoonPhase.Name = "cbBoonPhase";
            cbBoonPhase.Size = new Size(121, 23);
            cbBoonPhase.TabIndex = 11;
            cbBoonPhase.SelectedIndexChanged += cbBoonPhase_SelectedIndexChanged;
            // 
            // tableBoons
            // 
            tableBoons.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableBoons.Location = new Point(6, 37);
            tableBoons.Name = "tableBoons";
            tableBoons.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableBoons.Size = new Size(1050, 570);
            tableBoons.TabIndex = 0;
            // 
            // tabDps
            // 
            tabDps.Controls.Add(lblSelectedPhaseDps);
            tabDps.Controls.Add(label3);
            tabDps.Controls.Add(cbDpsPhase);
            tabDps.Controls.Add(tableDps);
            tabDps.Location = new Point(4, 24);
            tabDps.Name = "tabDps";
            tabDps.Padding = new Padding(3);
            tabDps.Size = new Size(1081, 603);
            tabDps.TabIndex = 1;
            tabDps.Text = "Dps";
            tabDps.UseVisualStyleBackColor = true;
            // 
            // lblSelectedPhaseDps
            // 
            lblSelectedPhaseDps.AutoSize = true;
            lblSelectedPhaseDps.Location = new Point(177, 9);
            lblSelectedPhaseDps.Name = "lblSelectedPhaseDps";
            lblSelectedPhaseDps.Size = new Size(38, 15);
            lblSelectedPhaseDps.TabIndex = 11;
            lblSelectedPhaseDps.Text = "label2";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 9);
            label3.Name = "label3";
            label3.Size = new Size(38, 15);
            label3.TabIndex = 10;
            label3.Text = "Phase";
            // 
            // cbDpsPhase
            // 
            cbDpsPhase.FormattingEnabled = true;
            cbDpsPhase.Location = new Point(50, 6);
            cbDpsPhase.Name = "cbDpsPhase";
            cbDpsPhase.Size = new Size(121, 23);
            cbDpsPhase.TabIndex = 9;
            cbDpsPhase.SelectedIndexChanged += cbDpsPhase_SelectedIndexChanged;
            // 
            // tableDps
            // 
            tableDps.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableDps.Location = new Point(6, 37);
            tableDps.Name = "tableDps";
            tableDps.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableDps.Size = new Size(1050, 570);
            tableDps.TabIndex = 0;
            // 
            // tabStealth
            // 
            tabStealth.Controls.Add(lblSelectedPhaseStealth);
            tabStealth.Controls.Add(label1);
            tabStealth.Controls.Add(cbStealthPhase);
            tabStealth.Controls.Add(tableStealth);
            tabStealth.Location = new Point(4, 24);
            tabStealth.Name = "tabStealth";
            tabStealth.Padding = new Padding(3);
            tabStealth.Size = new Size(1081, 603);
            tabStealth.TabIndex = 0;
            tabStealth.Text = "Stealth Analytics";
            tabStealth.UseVisualStyleBackColor = true;
            // 
            // lblSelectedPhaseStealth
            // 
            lblSelectedPhaseStealth.AutoSize = true;
            lblSelectedPhaseStealth.Location = new Point(177, 9);
            lblSelectedPhaseStealth.Name = "lblSelectedPhaseStealth";
            lblSelectedPhaseStealth.Size = new Size(0, 15);
            lblSelectedPhaseStealth.TabIndex = 8;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 9);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 7;
            label1.Text = "Phase";
            // 
            // cbStealthPhase
            // 
            cbStealthPhase.FormattingEnabled = true;
            cbStealthPhase.Location = new Point(50, 6);
            cbStealthPhase.Name = "cbStealthPhase";
            cbStealthPhase.Size = new Size(121, 23);
            cbStealthPhase.TabIndex = 1;
            cbStealthPhase.SelectedIndexChanged += cbStealthPhase_SelectedIndexChanged;
            // 
            // tableStealth
            // 
            tableStealth.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableStealth.Location = new Point(6, 37);
            tableStealth.Name = "tableStealth";
            tableStealth.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableStealth.Size = new Size(1050, 560);
            tableStealth.TabIndex = 0;
            // 
            // tabsControl
            // 
            tabsControl.Controls.Add(tabStealth);
            tabsControl.Controls.Add(tabDps);
            tabsControl.Controls.Add(tabBoons);
            tabsControl.Controls.Add(tabShockwaves);
            tabsControl.Dock = DockStyle.Fill;
            tabsControl.Location = new Point(0, 50);
            tabsControl.Name = "tabsControl";
            tabsControl.SelectedIndex = 0;
            tabsControl.Size = new Size(1089, 631);
            tabsControl.TabIndex = 8;
            // 
            // tabShockwaves
            // 
            tabShockwaves.Controls.Add(tableShockwave);
            tabShockwaves.Location = new Point(4, 24);
            tabShockwaves.Name = "tabShockwaves";
            tabShockwaves.Padding = new Padding(3);
            tabShockwaves.Size = new Size(1081, 603);
            tabShockwaves.TabIndex = 3;
            tabShockwaves.Text = "Shockwaves";
            tabShockwaves.UseVisualStyleBackColor = true;
            // 
            // tableShockwave
            // 
            tableShockwave.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableShockwave.Location = new Point(8, 6);
            tableShockwave.Name = "tableShockwave";
            tableShockwave.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableShockwave.Size = new Size(1067, 591);
            tableShockwave.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1261, 681);
            Controls.Add(tabsControl);
            Controls.Add(panelPlayers);
            Controls.Add(panelStealth);
            MaximizeBox = false;
            MinimumSize = new Size(128, 72);
            Name = "Form1";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "BLCT";
            panelStealth.ResumeLayout(false);
            tabBoons.ResumeLayout(false);
            tabBoons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tableBoons).EndInit();
            tabDps.ResumeLayout(false);
            tabDps.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tableDps).EndInit();
            tabStealth.ResumeLayout(false);
            tabStealth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tableStealth).EndInit();
            tabsControl.ResumeLayout(false);
            tabShockwaves.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tableShockwave).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Button btnOpenLogs;
        private Panel panelStealth;
        private ListBox lbLoadedFiles;
        private Button btnDeleteSelected;
        private Panel panelPlayers;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private TabPage tabBoons;
        private Label lblSelectedBoonBoons;
        private Label lblSelectedPhaseBoons;
        private Label label5;
        private ComboBox cbBoonBoons;
        private Label label4;
        private ComboBox cbBoonPhase;
        private DataGridView tableBoons;
        private TabPage tabDps;
        private Label lblSelectedPhaseDps;
        private Label label3;
        private ComboBox cbDpsPhase;
        private DataGridView tableDps;
        private TabPage tabStealth;
        private Label lblSelectedPhaseStealth;
        private Label label1;
        private ComboBox cbStealthPhase;
        private DataGridView tableStealth;
        private TabControl tabsControl;
        private TabPage tabShockwaves;
        private Label label2;
        private Label label6;
        private ComboBox comboBox1;
        private DataGridView tableShockwave;
    }
}
