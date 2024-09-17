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
            lblFontSize = new Label();
            nudFontSize = new NumericUpDown();
            btnOpenFolder = new Button();
            btnDeleteSelected = new Button();
            btnOpenLogs = new Button();
            lbLoadedFiles = new ListBox();
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            tabBoons = new TabPage();
            nudBoonTime = new NumericUpDown();
            cbBoonTime = new CheckBox();
            label9 = new Label();
            lblSelectedBoonBoons = new Label();
            lblSelectedPhaseBoons = new Label();
            cbBoonBoons = new ComboBox();
            cbBoonPhase = new ComboBox();
            tableBoons = new DataGridView();
            lblSelectedMechanic = new Label();
            label4 = new Label();
            tabMechanics = new TabPage();
            lblSelectedPhaseMechanics = new Label();
            cbMechanicMechanics = new ComboBox();
            cbMechanicPhase = new ComboBox();
            tableMechanics = new DataGridView();
            label7 = new Label();
            label8 = new Label();
            tabDps = new TabPage();
            cbDefiance = new CheckBox();
            cbCumulative = new CheckBox();
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
            label2 = new Label();
            tableShockwave = new DataGridView();
            cbAllTargets = new CheckBox();
            panelStealth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudFontSize).BeginInit();
            tabBoons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudBoonTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tableBoons).BeginInit();
            tabMechanics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tableMechanics).BeginInit();
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
            panelStealth.Controls.Add(lblFontSize);
            panelStealth.Controls.Add(nudFontSize);
            panelStealth.Controls.Add(btnOpenFolder);
            panelStealth.Controls.Add(btnDeleteSelected);
            panelStealth.Controls.Add(btnOpenLogs);
            panelStealth.Controls.Add(lbLoadedFiles);
            panelStealth.Dock = DockStyle.Right;
            panelStealth.Location = new Point(1089, 0);
            panelStealth.Name = "panelStealth";
            panelStealth.Size = new Size(172, 681);
            panelStealth.TabIndex = 2;
            // 
            // lblFontSize
            // 
            lblFontSize.AutoSize = true;
            lblFontSize.Location = new Point(9, 94);
            lblFontSize.Name = "lblFontSize";
            lblFontSize.Size = new Size(54, 15);
            lblFontSize.TabIndex = 5;
            lblFontSize.Text = "Font Size";
            // 
            // nudFontSize
            // 
            nudFontSize.Location = new Point(69, 92);
            nudFontSize.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudFontSize.Name = "nudFontSize";
            nudFontSize.Size = new Size(91, 23);
            nudFontSize.TabIndex = 4;
            nudFontSize.Value = new decimal(new int[] { 12, 0, 0, 0 });
            nudFontSize.ValueChanged += nudFontSize_ValueChanged;
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Location = new Point(9, 34);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(160, 23);
            btnOpenFolder.TabIndex = 3;
            btnOpenFolder.Text = "Open new logs in Folder";
            btnOpenFolder.UseVisualStyleBackColor = true;
            btnOpenFolder.Click += btnOpenFolder_Click;
            // 
            // btnDeleteSelected
            // 
            btnDeleteSelected.Location = new Point(9, 63);
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
            lbLoadedFiles.Location = new Point(9, 128);
            lbLoadedFiles.Name = "lbLoadedFiles";
            lbLoadedFiles.SelectionMode = SelectionMode.MultiExtended;
            lbLoadedFiles.Size = new Size(160, 529);
            lbLoadedFiles.Sorted = true;
            lbLoadedFiles.TabIndex = 0;
            // 
            // tabBoons
            // 
            tabBoons.Controls.Add(nudBoonTime);
            tabBoons.Controls.Add(cbBoonTime);
            tabBoons.Controls.Add(label9);
            tabBoons.Controls.Add(lblSelectedBoonBoons);
            tabBoons.Controls.Add(lblSelectedPhaseBoons);
            tabBoons.Controls.Add(cbBoonBoons);
            tabBoons.Controls.Add(cbBoonPhase);
            tabBoons.Controls.Add(tableBoons);
            tabBoons.Location = new Point(4, 24);
            tabBoons.Name = "tabBoons";
            tabBoons.Size = new Size(1081, 603);
            tabBoons.TabIndex = 2;
            tabBoons.Text = "Boons";
            tabBoons.UseVisualStyleBackColor = true;
            // 
            // nudBoonTime
            // 
            nudBoonTime.Location = new Point(809, 3);
            nudBoonTime.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudBoonTime.Name = "nudBoonTime";
            nudBoonTime.Size = new Size(120, 23);
            nudBoonTime.TabIndex = 19;
            // 
            // cbBoonTime
            // 
            cbBoonTime.AutoSize = true;
            cbBoonTime.Location = new Point(935, 5);
            cbBoonTime.Name = "cbBoonTime";
            cbBoonTime.Size = new Size(99, 19);
            cbBoonTime.TabIndex = 18;
            cbBoonTime.Text = "Boons at time";
            cbBoonTime.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(6, 6);
            label9.Name = "label9";
            label9.Size = new Size(38, 15);
            label9.TabIndex = 17;
            label9.Text = "Phase";
            // 
            // lblSelectedBoonBoons
            // 
            lblSelectedBoonBoons.AutoSize = true;
            lblSelectedBoonBoons.Location = new Point(693, 6);
            lblSelectedBoonBoons.Name = "lblSelectedBoonBoons";
            lblSelectedBoonBoons.Size = new Size(0, 15);
            lblSelectedBoonBoons.TabIndex = 16;
            // 
            // lblSelectedPhaseBoons
            // 
            lblSelectedPhaseBoons.AutoSize = true;
            lblSelectedPhaseBoons.Location = new Point(177, 6);
            lblSelectedPhaseBoons.Name = "lblSelectedPhaseBoons";
            lblSelectedPhaseBoons.Size = new Size(0, 15);
            lblSelectedPhaseBoons.TabIndex = 15;
            // 
            // cbBoonBoons
            // 
            cbBoonBoons.FormattingEnabled = true;
            cbBoonBoons.Location = new Point(566, 3);
            cbBoonBoons.Name = "cbBoonBoons";
            cbBoonBoons.Size = new Size(121, 23);
            cbBoonBoons.TabIndex = 13;
            // 
            // cbBoonPhase
            // 
            cbBoonPhase.FormattingEnabled = true;
            cbBoonPhase.Location = new Point(50, 3);
            cbBoonPhase.Name = "cbBoonPhase";
            cbBoonPhase.Size = new Size(121, 23);
            cbBoonPhase.TabIndex = 11;
            // 
            // tableBoons
            // 
            tableBoons.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableBoons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableBoons.Location = new Point(8, 37);
            tableBoons.Name = "tableBoons";
            tableBoons.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableBoons.Size = new Size(1067, 560);
            tableBoons.TabIndex = 0;
            // 
            // lblSelectedMechanic
            // 
            lblSelectedMechanic.AutoSize = true;
            lblSelectedMechanic.Location = new Point(660, 6);
            lblSelectedMechanic.Name = "lblSelectedMechanic";
            lblSelectedMechanic.Size = new Size(0, 15);
            lblSelectedMechanic.TabIndex = 14;
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
            // tabMechanics
            // 
            tabMechanics.Controls.Add(lblSelectedPhaseMechanics);
            tabMechanics.Controls.Add(lblSelectedMechanic);
            tabMechanics.Controls.Add(cbMechanicMechanics);
            tabMechanics.Controls.Add(label4);
            tabMechanics.Controls.Add(cbMechanicPhase);
            tabMechanics.Controls.Add(tableMechanics);
            tabMechanics.Location = new Point(4, 24);
            tabMechanics.Name = "tabMechanics";
            tabMechanics.Size = new Size(1081, 603);
            tabMechanics.TabIndex = 2;
            tabMechanics.Text = "Mechanics";
            tabMechanics.UseVisualStyleBackColor = true;
            // 
            // lblSelectedPhaseMechanics
            // 
            lblSelectedPhaseMechanics.AutoSize = true;
            lblSelectedPhaseMechanics.Location = new Point(177, 6);
            lblSelectedPhaseMechanics.Name = "lblSelectedPhaseMechanics";
            lblSelectedPhaseMechanics.Size = new Size(0, 15);
            lblSelectedPhaseMechanics.TabIndex = 15;
            // 
            // cbMechanicMechanics
            // 
            cbMechanicMechanics.FormattingEnabled = true;
            cbMechanicMechanics.Location = new Point(533, 3);
            cbMechanicMechanics.Name = "cbMechanicMechanics";
            cbMechanicMechanics.Size = new Size(121, 23);
            cbMechanicMechanics.TabIndex = 13;
            // 
            // cbMechanicPhase
            // 
            cbMechanicPhase.FormattingEnabled = true;
            cbMechanicPhase.Location = new Point(50, 3);
            cbMechanicPhase.Name = "cbMechanicPhase";
            cbMechanicPhase.Size = new Size(121, 23);
            cbMechanicPhase.TabIndex = 11;
            // 
            // tableMechanics
            // 
            tableMechanics.AllowUserToOrderColumns = true;
            tableMechanics.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableMechanics.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            tableMechanics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableMechanics.Location = new Point(8, 37);
            tableMechanics.Name = "tableMechanics";
            tableMechanics.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableMechanics.Size = new Size(1067, 560);
            tableMechanics.TabIndex = 0;
            // 
            // label7
            // 
            label7.Location = new Point(0, 0);
            label7.Name = "label7";
            label7.Size = new Size(100, 23);
            label7.TabIndex = 0;
            // 
            // label8
            // 
            label8.Location = new Point(0, 0);
            label8.Name = "label8";
            label8.Size = new Size(100, 23);
            label8.TabIndex = 0;
            // 
            // tabDps
            // 
            tabDps.Controls.Add(cbAllTargets);
            tabDps.Controls.Add(cbDefiance);
            tabDps.Controls.Add(cbCumulative);
            tabDps.Controls.Add(lblSelectedPhaseDps);
            tabDps.Controls.Add(label3);
            tabDps.Controls.Add(cbDpsPhase);
            tabDps.Controls.Add(tableDps);
            tabDps.Location = new Point(4, 24);
            tabDps.Name = "tabDps";
            tabDps.Padding = new Padding(3);
            tabDps.Size = new Size(1081, 603);
            tabDps.TabIndex = 1;
            tabDps.Text = "Dps and CC";
            tabDps.UseVisualStyleBackColor = true;
            // 
            // cbDefiance
            // 
            cbDefiance.AutoSize = true;
            cbDefiance.Location = new Point(473, 10);
            cbDefiance.Name = "cbDefiance";
            cbDefiance.Size = new Size(72, 19);
            cbDefiance.TabIndex = 13;
            cbDefiance.Text = "Defiance";
            cbDefiance.UseVisualStyleBackColor = true;
            // 
            // cbCumulative
            // 
            cbCumulative.AutoSize = true;
            cbCumulative.Location = new Point(380, 10);
            cbCumulative.Name = "cbCumulative";
            cbCumulative.Size = new Size(87, 19);
            cbCumulative.TabIndex = 12;
            cbCumulative.Text = "Cumulative";
            cbCumulative.UseVisualStyleBackColor = true;
            // 
            // lblSelectedPhaseDps
            // 
            lblSelectedPhaseDps.AutoSize = true;
            lblSelectedPhaseDps.Location = new Point(177, 9);
            lblSelectedPhaseDps.Name = "lblSelectedPhaseDps";
            lblSelectedPhaseDps.Size = new Size(0, 15);
            lblSelectedPhaseDps.TabIndex = 11;
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
            // 
            // tableDps
            // 
            tableDps.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableDps.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableDps.Location = new Point(8, 37);
            tableDps.Name = "tableDps";
            tableDps.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableDps.Size = new Size(1067, 560);
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
            // 
            // tableStealth
            // 
            tableStealth.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableStealth.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableStealth.Location = new Point(8, 37);
            tableStealth.Name = "tableStealth";
            tableStealth.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableStealth.Size = new Size(1067, 560);
            tableStealth.TabIndex = 0;
            // 
            // tabsControl
            // 
            tabsControl.Controls.Add(tabStealth);
            tabsControl.Controls.Add(tabDps);
            tabsControl.Controls.Add(tabBoons);
            tabsControl.Controls.Add(tabMechanics);
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
            tabShockwaves.Controls.Add(label2);
            tabShockwaves.Controls.Add(tableShockwave);
            tabShockwaves.Location = new Point(4, 24);
            tabShockwaves.Name = "tabShockwaves";
            tabShockwaves.Padding = new Padding(3);
            tabShockwaves.Size = new Size(1081, 603);
            tabShockwaves.TabIndex = 3;
            tabShockwaves.Text = "Shockwaves";
            tabShockwaves.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Symbol", 9F);
            label2.Location = new Point(8, 8);
            label2.Name = "label2";
            label2.Size = new Size(807, 15);
            label2.TabIndex = 1;
            label2.Text = "✓ = stability and jumped | ⚠ = no stability and jumped | 🛡 = stability saved  | 🔻 = downed | Green = Mordemoth | Blue = SooWon | Purple = Obliterator";
            // 
            // tableShockwave
            // 
            tableShockwave.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableShockwave.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableShockwave.Location = new Point(8, 26);
            tableShockwave.Name = "tableShockwave";
            tableShockwave.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableShockwave.Size = new Size(1067, 571);
            tableShockwave.TabIndex = 0;
            // 
            // cbAllTargets
            // 
            cbAllTargets.AutoSize = true;
            cbAllTargets.Location = new Point(551, 8);
            cbAllTargets.Name = "cbAllTargets";
            cbAllTargets.Size = new Size(80, 19);
            cbAllTargets.TabIndex = 14;
            cbAllTargets.Text = "All Targets";
            cbAllTargets.UseVisualStyleBackColor = true;
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
            MinimumSize = new Size(128, 72);
            Name = "Form1";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "BLCT";
            panelStealth.ResumeLayout(false);
            panelStealth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudFontSize).EndInit();
            tabBoons.ResumeLayout(false);
            tabBoons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudBoonTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)tableBoons).EndInit();
            tabMechanics.ResumeLayout(false);
            tabMechanics.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tableMechanics).EndInit();
            tabDps.ResumeLayout(false);
            tabDps.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tableDps).EndInit();
            tabStealth.ResumeLayout(false);
            tabStealth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tableStealth).EndInit();
            tabsControl.ResumeLayout(false);
            tabShockwaves.ResumeLayout(false);
            tabShockwaves.PerformLayout();
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
        private Label lblSelectedMechanic;
        private ComboBox cbBoonBoons;
        private Label label4;
        private ComboBox cbBoonPhase;
        private DataGridView tableBoons;
        private TabPage tabMechanics;
        private Label lblSelectedPhaseMechanics;
        private Label label7;
        private ComboBox cbMechanicMechanics;
        private Label label8;
        private ComboBox cbMechanicPhase;
        private DataGridView tableMechanics;
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
        private Label label9;
        private Button btnOpenFolder;
        private CheckBox cbCumulative;
        private CheckBox cbDefiance;
        private CheckBox cbBoonTime;
        private NumericUpDown nudBoonTime;
        private Label lblFontSize;
        private NumericUpDown nudFontSize;
        private CheckBox cbAllTargets;
    }
}
