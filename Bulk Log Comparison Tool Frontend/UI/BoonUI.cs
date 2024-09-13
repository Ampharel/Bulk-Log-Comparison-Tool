using Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool.Util;
using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bulk_Log_Comparison_Tool_Frontend.UI
{
    internal class BoonUI : PlayerUI
    {
        private string _selectedBoon = "";
        private string _selectedPhase = "";
        
        private readonly DataGridView tableBoons;
        private readonly Label lblSelectedBoon;
        private readonly Label lblSelectedPhase;
        private readonly ComboBox cbPhase;
        private readonly ComboBox cbBoon;
        private readonly TabPage tabBoons;
        private readonly UILogParser _logParser;

        public BoonUI(DataGridView tableBoons, Label lblSelectedBoon, Label lblSelectedPhase, ComboBox cbBoon, ComboBox cbPhase, TabPage tabBoons, UILogParser logParser, List<string> activePlayers) : base(activePlayers)
        {
            this.tableBoons = tableBoons;
            this.lblSelectedBoon = lblSelectedBoon;
            this.lblSelectedPhase = lblSelectedPhase;
            this.cbBoon = cbBoon;
            this.cbPhase = cbPhase;
            this.tabBoons = tabBoons;
            _logParser = logParser;
            cbBoon.SelectedIndexChanged += OnCbBoonSelectedIndexChanged;
            cbPhase.SelectedIndexChanged += OnCbPhaseSelectedIndexChanged;
        }

        private void OnCbBoonSelectedIndexChanged(object? sender, EventArgs e)
        {
            _selectedBoon = cbBoon.SelectedItem?.ToString() ?? "";
            UpdatePanel();
        }

        private void OnCbPhaseSelectedIndexChanged(object? sender, EventArgs e)
        {
            _selectedPhase = cbPhase.SelectedItem?.ToString() ?? "";
            UpdatePanel();
        }

        ~BoonUI()
        {
            cbBoon.SelectedIndexChanged -= OnCbBoonSelectedIndexChanged;
            cbPhase.SelectedIndexChanged -= OnCbPhaseSelectedIndexChanged;
        }

        public override void UpdatePanel()
        {
            var Groups = _logParser.BulkLog.GetGroups();
            if (ActivePlayers.Count + Groups.Count() == 0)
            {
                return;
            }
            tabBoons.Controls.Remove(tableBoons);
            tableBoons.DataSource = null;

            var Logs = _logParser.BulkLog.Logs;
            tableBoons.RowCount = ActivePlayers.Count + Groups.Count();
            tableBoons.ColumnCount = Logs.Count() + 1;

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
            cbPhase.Items.Clear();
            cbPhase.Items.AddRange(Phases);
            lblSelectedPhase.Text = _selectedPhase;

            var boonNames = _logParser.BulkLog.GetBoonNames();
            if (_selectedBoon == "" || !boonNames.Contains(_selectedBoon))
            {
                var Boon = boonNames.FirstOrDefault();
                if (Boon == null)
                {
                    return;
                }
                _selectedBoon = Boon;
            }

            cbBoon.Items.Clear();
            cbBoon.Items.AddRange(boonNames);
            lblSelectedBoon.Text = _selectedBoon;

            tableBoons.TopLeftHeaderCell.Value = _selectedPhase;
            for (int x = 0; x < Logs.Count(); x++)
            {
                tableBoons.Columns[x].HeaderCell.Value = Logs[x].GetFileName();
                tableBoons.Columns[x].MinimumWidth = 10;
            }
            tableBoons.Columns[Logs.Count()].HeaderCell.Value = "Trimmed Mean";
            for (int y = 0; y < ActivePlayers.Count; y++)
            {
                tableBoons.Rows[y].HeaderCell.Value = ActivePlayers[y];
                List<double> boonNumbers = new();
                BuffStackTyping boonType = BuffStackTyping.Stacking;
                for (int x = 0; x < Logs.Count(); x++)
                {
                    if (!Logs[x].HasPlayer(ActivePlayers[y]))
                    {
                        tableBoons.Rows[y].Cells[x].Value = "";
                        continue;
                    }
                    double boonUptime = Logs[x].GetBoon(ActivePlayers[y], _selectedBoon, _selectedPhase);
                    boonType = Logs[x].GetBoonStackType(_selectedBoon);
                    boonNumbers.Add(boonUptime);
                    var text = $"{boonUptime.ToString("F1")}";
                    if (boonType == BuffStackTyping.Queue || boonType == BuffStackTyping.Regeneration)
                    {
                        tableBoons.Columns[x].DefaultCellStyle.Format = "P1";
                        boonUptime /= 100f;
                    }
                    else
                    {
                        tableBoons.Columns[x].DefaultCellStyle.Format = "F1";
                    }
                    tableBoons.Rows[y].Cells[x].Value = boonUptime;
                }
                float RoundedAverage = (float)Math.Round(Util.TrimmedAverage(boonNumbers).Average(), 1);
                var averageText = $"{RoundedAverage.ToString("F1")}";
                if (boonType == BuffStackTyping.Queue || boonType == BuffStackTyping.Regeneration)
                {
                    tableBoons.Columns[Logs.Count()].DefaultCellStyle.Format = "P1";
                    RoundedAverage /= 100f;
                }
                else
                {
                    tableBoons.Columns[Logs.Count()].DefaultCellStyle.Format = "F1";
                }
                tableBoons.Rows[y].Cells[Logs.Count()].Value = RoundedAverage;


            }
            int row = ActivePlayers.Count;
            foreach (var group in Groups)
            {
                tableBoons.Rows[row].HeaderCell.Value = $"Group {group}";
                for (int x = 0; x < Logs.Count(); x++)
                {
                    var boonUptime = Logs[x].GetBoon(group, _selectedBoon, _selectedPhase);
                    var groupText = $"{boonUptime.ToString("F1")}";
                    var bt = Logs[x].GetBoonStackType(_selectedBoon);
                    if (bt == BuffStackTyping.Queue || bt == BuffStackTyping.Regeneration)
                    {
                        tableBoons.Columns[x].DefaultCellStyle.Format = "P1";
                        boonUptime /= 100f;
                    }
                    else
                    {
                        tableBoons.Columns[x].DefaultCellStyle.Format = "F1";
                    }
                    tableBoons.Rows[row].Cells[x].Value = boonUptime;
                }
                row++;
            }
            tableBoons.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            tabBoons.Controls.Add(tableBoons);
        }
    }
}
