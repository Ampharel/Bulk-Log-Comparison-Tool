﻿using Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool.Util;
using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bulk_Log_Comparison_Tool_Frontend.UI
{
    internal class MechanicsUI : PlayerUI
    {

        private string _selectedPhase = "";
        private string _selectedMechanic = "";
        private readonly DataGridView tableMechanics;
        private readonly Label lblSelectedPhaseMechanics;
        private readonly Label lblSelectedMechanic;
        private readonly ComboBox cbMechanicPhase;
        private readonly ComboBox cbMechanicMechanics;
        private readonly TabPage tabMechanics;
        private readonly UILogParser _logParser;

        public MechanicsUI(DataGridView tableMechanics, Label lblSelectedPhaseMechanics, Label lblSelectedMechanic, ComboBox cbMechanicPhase, ComboBox cbMechanicMechanics, TabPage tabMechanics, UILogParser logParser, List<string> activePlayers) : base(activePlayers)
        {
            this.tableMechanics = tableMechanics;
            this.lblSelectedPhaseMechanics = lblSelectedPhaseMechanics;
            this.lblSelectedMechanic = lblSelectedMechanic;
            this.cbMechanicPhase = cbMechanicPhase;
            this.cbMechanicMechanics = cbMechanicMechanics;
            this.tabMechanics = tabMechanics;
            _logParser = logParser;
            cbMechanicPhase.SelectedIndexChanged += OnCbMechanicPhaseSelectedIndexChanged;
            cbMechanicMechanics.SelectedIndexChanged += OnCbMechanicMechanicsSelectedIndexChanged;
            tableMechanics.ColumnHeaderMouseDoubleClick += tableMechanics_ColumnHeaderMouseDoubleClick;
        }

        public void OnCbMechanicPhaseSelectedIndexChanged(object? sender, EventArgs e)
        {
            _selectedPhase = cbMechanicPhase.SelectedItem?.ToString() ?? "";
            UpdatePanel();
        }

        public void OnCbMechanicMechanicsSelectedIndexChanged(object? sender, EventArgs e)
        {
            _selectedMechanic = cbMechanicMechanics.SelectedItem?.ToString() ?? "";
            UpdatePanel();
        }

        ~MechanicsUI()
        {
            cbMechanicPhase.SelectedIndexChanged -= OnCbMechanicPhaseSelectedIndexChanged;
            cbMechanicMechanics.SelectedIndexChanged -= OnCbMechanicMechanicsSelectedIndexChanged;
        }

        public override void UpdatePanel()
        {
            var Groups = _logParser.BulkLog.GetGroups();
            if (Groups.Count() == 0)
            {
                return;
            }
            tabMechanics.Controls.Remove(tableMechanics);
            tableMechanics.DataSource = null;

            tableMechanics.RowCount = ActivePlayers.Count + Groups.Count();
            tableMechanics.ColumnCount = _logParser.BulkLog.Logs.Count() + 1;

            var Phases = _logParser.BulkLog.GetPhases();
            if (_selectedPhase == "" || !Phases.Contains(_selectedPhase))
            {
                var Phase = Phases.FirstOrDefault();
                if (Phase == null)
                {
                    return;
                }
                _selectedPhase = Phase;
            }
            cbMechanicPhase.Items.Clear();
            cbMechanicPhase.Items.AddRange(Phases);
            lblSelectedPhaseMechanics.Text = _selectedPhase;

            var MechanicNames = _logParser.BulkLog.GetMechanicNames(_selectedPhase);
            if (_selectedMechanic == "" || !MechanicNames.Contains(_selectedMechanic))
            {
                var Mechanic = MechanicNames.FirstOrDefault();
                if (Mechanic == null)
                {
                    return;
                }
                _selectedMechanic = Mechanic;
            }

            cbMechanicMechanics.Items.Clear();
            cbMechanicMechanics.Items.AddRange(MechanicNames);
            lblSelectedMechanic.Text = _selectedMechanic;

            tableMechanics.TopLeftHeaderCell.Value = _selectedPhase;
            for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
            {
                tableMechanics.Columns[x].HeaderCell.Value = _logParser.BulkLog.Logs[x].GetFileName();
                tableMechanics.Columns[x].MinimumWidth = 10;
            }
            for (int y = 0; y < ActivePlayers.Count; y++)
            {
                string activePlayer = ActivePlayers[y];
                tableMechanics.Rows[y].HeaderCell.Value = activePlayer;
                List<double> MechanicNumbers = new();
                for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
                {
                    var mechanicLogs = _logParser.BulkLog.Logs[x].GetMechanicLogs(_selectedMechanic, _selectedPhase).Where(x => x.Item1.Equals(activePlayer));
                    StringBuilder sb = new();
                    foreach (var log in mechanicLogs)
                    {
                        sb.Append($"{log.Item2 / 1000}s ");
                    }
                    tableMechanics.Rows[y].Cells[x].Value = sb.ToString();
                }
            }
            tableMechanics.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            tabMechanics.Controls.Add(tableMechanics);
        }


        private void tableMechanics_ColumnHeaderMouseDoubleClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            var dataGrid = sender as DataGridView;
            if (dataGrid == null)
            {
                return;
            }
            var content = dataGrid.Columns[e.ColumnIndex].HeaderCell.Value;
            if (content == null || !(content is string))
            {
                return;
            }
            System.Windows.Forms.Clipboard.SetText(content as string ?? "");
        }
    }
}