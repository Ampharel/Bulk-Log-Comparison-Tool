﻿using Bulk_Log_Comparison_Tool_Frontend.Compare;

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
            panelStealth = new Panel();
            tabControl1 = new TabControl();
            tabLogs = new TabPage();
            lbLoadedFiles = new ListBox();
            tabPlayers = new TabPage();
            panelPlayers = new Panel();
            lblFontSize = new Label();
            nudFontSize = new NumericUpDown();
            btnOpenFolder = new Button();
            btnDeleteSelected = new Button();
            btnOpenLogs = new Button();
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            tabBoons = new TabPage();
            cbGraph = new CheckBox();
            nudBoonTime = new NumericUpDown();
            cbBoonTime = new CheckBox();
            label9 = new Label();
            lblSelectedBoonBoons = new Label();
            lblSelectedPhaseBoons = new Label();
            comboBoonBoons = new ComboBox();
            comboBoonPhase = new ComboBox();
            tableBoons = new DataGridView();
            lblSelectedMechanic = new Label();
            label4 = new Label();
            tabMechanics = new TabPage();
            cbCount = new CheckBox();
            lblSelectedPhaseMechanics = new Label();
            comboMechanicMechanics = new ComboBox();
            comboMechanicPhase = new ComboBox();
            tableMechanics = new DataGridView();
            label7 = new Label();
            label8 = new Label();
            tabDps = new TabPage();
            cbAllTargets = new CheckBox();
            cbDefiance = new CheckBox();
            cbCumulative = new CheckBox();
            lblSelectedPhaseDps = new Label();
            label3 = new Label();
            comboDpsPhase = new ComboBox();
            tableDps = new DataGridView();
            tabStealth = new TabPage();
            btnShowAlgoritmns = new Button();
            cbAlgoritmn = new ComboBox();
            cbShowLate = new CheckBox();
            lblSelectedPhaseStealth = new Label();
            label1 = new Label();
            comboStealthPhase = new ComboBox();
            tableStealth = new DataGridView();
            tabsControl = new TabControl();
            tabSummary = new TabPage();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            tableStealthSummary = new DataGridView();
            tableMechanicsSummary = new DataGridView();
            splitContainer3 = new SplitContainer();
            tableShockwaveSummary = new DataGridView();
            tableDeaths = new DataGridView();
            comboSummaryLog = new ComboBox();
            tabShockwaves = new TabPage();
            label2 = new Label();
            tableShockwave = new DataGridView();
            panelStealth.SuspendLayout();
            tabControl1.SuspendLayout();
            tabLogs.SuspendLayout();
            tabPlayers.SuspendLayout();
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
            tabSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tableStealthSummary).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tableMechanicsSummary).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tableShockwaveSummary).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tableDeaths).BeginInit();
            tabShockwaves.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tableShockwave).BeginInit();
            SuspendLayout();
            // 
            // panelStealth
            // 
            panelStealth.Controls.Add(tabControl1);
            panelStealth.Controls.Add(lblFontSize);
            panelStealth.Controls.Add(nudFontSize);
            panelStealth.Controls.Add(btnOpenFolder);
            panelStealth.Controls.Add(btnDeleteSelected);
            panelStealth.Controls.Add(btnOpenLogs);
            panelStealth.Dock = DockStyle.Right;
            panelStealth.Location = new Point(1089, 0);
            panelStealth.Name = "panelStealth";
            panelStealth.Size = new Size(172, 681);
            panelStealth.TabIndex = 2;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabLogs);
            tabControl1.Controls.Add(tabPlayers);
            tabControl1.Location = new Point(2, 121);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(167, 556);
            tabControl1.TabIndex = 6;
            // 
            // tabLogs
            // 
            tabLogs.Controls.Add(lbLoadedFiles);
            tabLogs.Location = new Point(4, 24);
            tabLogs.Name = "tabLogs";
            tabLogs.Padding = new Padding(3);
            tabLogs.Size = new Size(159, 528);
            tabLogs.TabIndex = 0;
            tabLogs.Text = "Logs";
            tabLogs.UseVisualStyleBackColor = true;
            // 
            // lbLoadedFiles
            // 
            lbLoadedFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbLoadedFiles.BackColor = SystemColors.Control;
            lbLoadedFiles.ForeColor = SystemColors.Menu;
            lbLoadedFiles.FormattingEnabled = true;
            lbLoadedFiles.HorizontalScrollbar = true;
            lbLoadedFiles.ItemHeight = 15;
            lbLoadedFiles.Location = new Point(0, 0);
            lbLoadedFiles.Name = "lbLoadedFiles";
            lbLoadedFiles.SelectionMode = SelectionMode.MultiExtended;
            lbLoadedFiles.Size = new Size(160, 529);
            lbLoadedFiles.Sorted = true;
            lbLoadedFiles.TabIndex = 0;
            lbLoadedFiles.KeyDown += lbLoadedFiles_KeyDown;
            // 
            // tabPlayers
            // 
            tabPlayers.Controls.Add(panelPlayers);
            tabPlayers.Location = new Point(4, 24);
            tabPlayers.Name = "tabPlayers";
            tabPlayers.Padding = new Padding(3);
            tabPlayers.Size = new Size(159, 528);
            tabPlayers.TabIndex = 1;
            tabPlayers.Text = "Players";
            tabPlayers.UseVisualStyleBackColor = true;
            // 
            // panelPlayers
            // 
            panelPlayers.AutoScroll = true;
            panelPlayers.BackColor = SystemColors.Control;
            panelPlayers.Dock = DockStyle.Top;
            panelPlayers.Location = new Point(3, 3);
            panelPlayers.Name = "panelPlayers";
            panelPlayers.Size = new Size(153, 525);
            panelPlayers.TabIndex = 5;
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
            nudFontSize.Size = new Size(100, 23);
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
            // tabBoons
            // 
            tabBoons.Controls.Add(cbGraph);
            tabBoons.Controls.Add(nudBoonTime);
            tabBoons.Controls.Add(cbBoonTime);
            tabBoons.Controls.Add(label9);
            tabBoons.Controls.Add(lblSelectedBoonBoons);
            tabBoons.Controls.Add(lblSelectedPhaseBoons);
            tabBoons.Controls.Add(comboBoonBoons);
            tabBoons.Controls.Add(comboBoonPhase);
            tabBoons.Controls.Add(tableBoons);
            tabBoons.Location = new Point(4, 24);
            tabBoons.Name = "tabBoons";
            tabBoons.Size = new Size(1081, 653);
            tabBoons.TabIndex = 2;
            tabBoons.Text = "Boons";
            tabBoons.UseVisualStyleBackColor = true;
            // 
            // cbGraph
            // 
            cbGraph.AutoSize = true;
            cbGraph.Location = new Point(1017, 8);
            cbGraph.Name = "cbGraph";
            cbGraph.Size = new Size(58, 19);
            cbGraph.TabIndex = 20;
            cbGraph.Text = "Graph";
            cbGraph.UseVisualStyleBackColor = true;
            // 
            // nudBoonTime
            // 
            nudBoonTime.Location = new Point(777, 4);
            nudBoonTime.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudBoonTime.Name = "nudBoonTime";
            nudBoonTime.Size = new Size(120, 23);
            nudBoonTime.TabIndex = 19;
            // 
            // cbBoonTime
            // 
            cbBoonTime.AutoSize = true;
            cbBoonTime.Location = new Point(903, 8);
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
            lblSelectedPhaseBoons.Location = new Point(307, 6);
            lblSelectedPhaseBoons.Name = "lblSelectedPhaseBoons";
            lblSelectedPhaseBoons.Size = new Size(0, 15);
            lblSelectedPhaseBoons.TabIndex = 15;
            // 
            // comboBoonBoons
            // 
            comboBoonBoons.FormattingEnabled = true;
            comboBoonBoons.Location = new Point(566, 6);
            comboBoonBoons.Name = "comboBoonBoons";
            comboBoonBoons.Size = new Size(121, 23);
            comboBoonBoons.TabIndex = 13;
            // 
            // comboBoonPhase
            // 
            comboBoonPhase.FormattingEnabled = true;
            comboBoonPhase.Location = new Point(50, 6);
            comboBoonPhase.Name = "comboBoonPhase";
            comboBoonPhase.Size = new Size(250, 23);
            comboBoonPhase.TabIndex = 11;
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
            lblSelectedMechanic.Location = new Point(800, 6);
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
            tabMechanics.Controls.Add(cbCount);
            tabMechanics.Controls.Add(lblSelectedPhaseMechanics);
            tabMechanics.Controls.Add(lblSelectedMechanic);
            tabMechanics.Controls.Add(comboMechanicMechanics);
            tabMechanics.Controls.Add(label4);
            tabMechanics.Controls.Add(comboMechanicPhase);
            tabMechanics.Controls.Add(tableMechanics);
            tabMechanics.Location = new Point(4, 24);
            tabMechanics.Name = "tabMechanics";
            tabMechanics.Size = new Size(1081, 653);
            tabMechanics.TabIndex = 2;
            tabMechanics.Text = "Mechanics";
            tabMechanics.UseVisualStyleBackColor = true;
            // 
            // cbCount
            // 
            cbCount.AutoSize = true;
            cbCount.Location = new Point(1017, 8);
            cbCount.Name = "cbCount";
            cbCount.Size = new Size(59, 19);
            cbCount.TabIndex = 21;
            cbCount.Text = "Count";
            cbCount.UseVisualStyleBackColor = true;
            // 
            // lblSelectedPhaseMechanics
            // 
            lblSelectedPhaseMechanics.AutoSize = true;
            lblSelectedPhaseMechanics.Location = new Point(307, 6);
            lblSelectedPhaseMechanics.Name = "lblSelectedPhaseMechanics";
            lblSelectedPhaseMechanics.Size = new Size(0, 15);
            lblSelectedPhaseMechanics.TabIndex = 15;
            // 
            // comboMechanicMechanics
            // 
            comboMechanicMechanics.FormattingEnabled = true;
            comboMechanicMechanics.Location = new Point(533, 6);
            comboMechanicMechanics.Name = "comboMechanicMechanics";
            comboMechanicMechanics.Size = new Size(250, 23);
            comboMechanicMechanics.TabIndex = 13;
            // 
            // comboMechanicPhase
            // 
            comboMechanicPhase.FormattingEnabled = true;
            comboMechanicPhase.Location = new Point(50, 6);
            comboMechanicPhase.Name = "comboMechanicPhase";
            comboMechanicPhase.Size = new Size(250, 23);
            comboMechanicPhase.TabIndex = 11;
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
            tabDps.Controls.Add(comboDpsPhase);
            tabDps.Controls.Add(tableDps);
            tabDps.Location = new Point(4, 24);
            tabDps.Name = "tabDps";
            tabDps.Padding = new Padding(3);
            tabDps.Size = new Size(1081, 653);
            tabDps.TabIndex = 1;
            tabDps.Text = "Dps and CC";
            tabDps.UseVisualStyleBackColor = true;
            // 
            // cbAllTargets
            // 
            cbAllTargets.AutoSize = true;
            cbAllTargets.Location = new Point(551, 10);
            cbAllTargets.Name = "cbAllTargets";
            cbAllTargets.Size = new Size(80, 19);
            cbAllTargets.TabIndex = 14;
            cbAllTargets.Text = "All Targets";
            cbAllTargets.UseVisualStyleBackColor = true;
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
            lblSelectedPhaseDps.Location = new Point(307, 6);
            lblSelectedPhaseDps.Name = "lblSelectedPhaseDps";
            lblSelectedPhaseDps.Size = new Size(0, 15);
            lblSelectedPhaseDps.TabIndex = 11;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 6);
            label3.Name = "label3";
            label3.Size = new Size(38, 15);
            label3.TabIndex = 10;
            label3.Text = "Phase";
            // 
            // comboDpsPhase
            // 
            comboDpsPhase.FormattingEnabled = true;
            comboDpsPhase.Location = new Point(50, 6);
            comboDpsPhase.Name = "comboDpsPhase";
            comboDpsPhase.Size = new Size(250, 23);
            comboDpsPhase.TabIndex = 9;
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
            tabStealth.Controls.Add(btnShowAlgoritmns);
            tabStealth.Controls.Add(cbAlgoritmn);
            tabStealth.Controls.Add(cbShowLate);
            tabStealth.Controls.Add(lblSelectedPhaseStealth);
            tabStealth.Controls.Add(label1);
            tabStealth.Controls.Add(comboStealthPhase);
            tabStealth.Controls.Add(tableStealth);
            tabStealth.Location = new Point(4, 24);
            tabStealth.Name = "tabStealth";
            tabStealth.Padding = new Padding(3);
            tabStealth.Size = new Size(1081, 653);
            tabStealth.TabIndex = 0;
            tabStealth.Text = "Stealth Analytics";
            tabStealth.UseVisualStyleBackColor = true;
            // 
            // btnShowAlgoritmns
            // 
            btnShowAlgoritmns.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnShowAlgoritmns.Location = new Point(0, 593);
            btnShowAlgoritmns.Name = "btnShowAlgoritmns";
            btnShowAlgoritmns.Size = new Size(10, 10);
            btnShowAlgoritmns.TabIndex = 15;
            btnShowAlgoritmns.UseVisualStyleBackColor = true;
            // 
            // cbAlgoritmn
            // 
            cbAlgoritmn.FormattingEnabled = true;
            cbAlgoritmn.Location = new Point(942, 6);
            cbAlgoritmn.Name = "cbAlgoritmn";
            cbAlgoritmn.Size = new Size(133, 23);
            cbAlgoritmn.TabIndex = 14;
            cbAlgoritmn.Visible = false;
            // 
            // cbShowLate
            // 
            cbShowLate.AutoSize = true;
            cbShowLate.Location = new Point(441, 8);
            cbShowLate.Name = "cbShowLate";
            cbShowLate.Size = new Size(77, 19);
            cbShowLate.TabIndex = 13;
            cbShowLate.Text = "Show late";
            cbShowLate.UseVisualStyleBackColor = true;
            // 
            // lblSelectedPhaseStealth
            // 
            lblSelectedPhaseStealth.AutoSize = true;
            lblSelectedPhaseStealth.Location = new Point(307, 6);
            lblSelectedPhaseStealth.Name = "lblSelectedPhaseStealth";
            lblSelectedPhaseStealth.Size = new Size(0, 15);
            lblSelectedPhaseStealth.TabIndex = 8;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 6);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 7;
            label1.Text = "Phase";
            // 
            // comboStealthPhase
            // 
            comboStealthPhase.FormattingEnabled = true;
            comboStealthPhase.Location = new Point(50, 6);
            comboStealthPhase.Name = "comboStealthPhase";
            comboStealthPhase.Size = new Size(250, 23);
            comboStealthPhase.TabIndex = 1;
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
            tabsControl.Controls.Add(tabSummary);
            tabsControl.Controls.Add(tabStealth);
            tabsControl.Controls.Add(tabDps);
            tabsControl.Controls.Add(tabBoons);
            tabsControl.Controls.Add(tabMechanics);
            tabsControl.Controls.Add(tabShockwaves);
            tabsControl.Dock = DockStyle.Fill;
            tabsControl.Location = new Point(0, 0);
            tabsControl.Name = "tabsControl";
            tabsControl.SelectedIndex = 0;
            tabsControl.Size = new Size(1089, 681);
            tabsControl.TabIndex = 8;
            // 
            // tabSummary
            // 
            tabSummary.Controls.Add(splitContainer1);
            tabSummary.Controls.Add(comboSummaryLog);
            tabSummary.Location = new Point(4, 24);
            tabSummary.Name = "tabSummary";
            tabSummary.Size = new Size(1081, 653);
            tabSummary.TabIndex = 4;
            tabSummary.Text = "QuickSummary";
            tabSummary.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 23);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer3);
            splitContainer1.Size = new Size(1081, 630);
            splitContainer1.SplitterDistance = 313;
            splitContainer1.TabIndex = 8;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(tableStealthSummary);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(tableMechanicsSummary);
            splitContainer2.Size = new Size(1081, 313);
            splitContainer2.SplitterDistance = 611;
            splitContainer2.TabIndex = 0;
            // 
            // tableStealthSummary
            // 
            tableStealthSummary.AllowUserToAddRows = false;
            tableStealthSummary.AllowUserToDeleteRows = false;
            tableStealthSummary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableStealthSummary.Dock = DockStyle.Fill;
            tableStealthSummary.Location = new Point(0, 0);
            tableStealthSummary.Name = "tableStealthSummary";
            tableStealthSummary.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableStealthSummary.Size = new Size(611, 313);
            tableStealthSummary.TabIndex = 3;
            // 
            // tableMechanicsSummary
            // 
            tableMechanicsSummary.AllowUserToAddRows = false;
            tableMechanicsSummary.AllowUserToDeleteRows = false;
            tableMechanicsSummary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableMechanicsSummary.Dock = DockStyle.Fill;
            tableMechanicsSummary.Location = new Point(0, 0);
            tableMechanicsSummary.Name = "tableMechanicsSummary";
            tableMechanicsSummary.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableMechanicsSummary.Size = new Size(466, 313);
            tableMechanicsSummary.TabIndex = 5;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(tableShockwaveSummary);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(tableDeaths);
            splitContainer3.Size = new Size(1081, 313);
            splitContainer3.SplitterDistance = 612;
            splitContainer3.TabIndex = 0;
            // 
            // tableShockwaveSummary
            // 
            tableShockwaveSummary.AllowUserToAddRows = false;
            tableShockwaveSummary.AllowUserToDeleteRows = false;
            tableShockwaveSummary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableShockwaveSummary.Dock = DockStyle.Fill;
            tableShockwaveSummary.Location = new Point(0, 0);
            tableShockwaveSummary.Name = "tableShockwaveSummary";
            tableShockwaveSummary.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableShockwaveSummary.Size = new Size(612, 313);
            tableShockwaveSummary.TabIndex = 4;
            // 
            // tableDeaths
            // 
            tableDeaths.AllowUserToAddRows = false;
            tableDeaths.AllowUserToDeleteRows = false;
            tableDeaths.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableDeaths.Dock = DockStyle.Fill;
            tableDeaths.Location = new Point(0, 0);
            tableDeaths.Name = "tableDeaths";
            tableDeaths.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            tableDeaths.Size = new Size(465, 313);
            tableDeaths.TabIndex = 6;
            // 
            // comboSummaryLog
            // 
            comboSummaryLog.Dock = DockStyle.Top;
            comboSummaryLog.FormattingEnabled = true;
            comboSummaryLog.Location = new Point(0, 0);
            comboSummaryLog.Name = "comboSummaryLog";
            comboSummaryLog.Size = new Size(1081, 23);
            comboSummaryLog.TabIndex = 2;
            // 
            // tabShockwaves
            // 
            tabShockwaves.Controls.Add(label2);
            tabShockwaves.Controls.Add(tableShockwave);
            tabShockwaves.Location = new Point(4, 24);
            tabShockwaves.Name = "tabShockwaves";
            tabShockwaves.Padding = new Padding(3);
            tabShockwaves.Size = new Size(1081, 653);
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1261, 681);
            Controls.Add(tabsControl);
            Controls.Add(panelStealth);
            MinimumSize = new Size(128, 72);
            Name = "Form1";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "BLCT";
            panelStealth.ResumeLayout(false);
            panelStealth.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabLogs.ResumeLayout(false);
            tabPlayers.ResumeLayout(false);
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
            tabSummary.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tableStealthSummary).EndInit();
            ((System.ComponentModel.ISupportInitialize)tableMechanicsSummary).EndInit();
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tableShockwaveSummary).EndInit();
            ((System.ComponentModel.ISupportInitialize)tableDeaths).EndInit();
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
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private TabPage tabBoons;
        private Label lblSelectedBoonBoons;
        private Label lblSelectedPhaseBoons;
        private Label lblSelectedMechanic;
        private ComboBox comboBoonBoons;
        private Label label4;
        private ComboBox comboBoonPhase;
        private DataGridView tableBoons;
        private TabPage tabMechanics;
        private Label lblSelectedPhaseMechanics;
        private Label label7;
        private ComboBox comboMechanicMechanics;
        private Label label8;
        private ComboBox comboMechanicPhase;
        private DataGridView tableMechanics;
        private TabPage tabDps;
        private Label lblSelectedPhaseDps;
        private Label label3;
        private ComboBox comboDpsPhase;
        private DataGridView tableDps;
        private TabPage tabStealth;
        private Label lblSelectedPhaseStealth;
        private Label label1;
        private ComboBox comboStealthPhase;
        private DataGridView tableStealth;
        private TabControl tabsControl;
        private TabPage tabShockwaves;
        private Label label2;
        private ComboBox cbAlgoritmn;
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
        private TabPage tabSummary;
        private ComboBox comboSummaryLog;
        private DataGridView tableStealthSummary;
        private DataGridView tableShockwaveSummary;
        private DataGridView tableDeaths;
        private DataGridView tableMechanicsSummary;
        private CheckBox cbShowLate;
        private Button btnShowAlgoritmns;
        private TabControl tabControl1;
        private TabPage tabLogs;
        private TabPage tabPlayers;
        private Panel panelPlayers;
        private CheckBox cbGraph;
        private CheckBox cbCount;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private SplitContainer splitContainer3;
    }
}
