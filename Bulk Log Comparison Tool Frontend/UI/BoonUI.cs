using Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool.Util;
using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool_Frontend.Compare;
using Bulk_Log_Comparison_Tool_Frontend.Utils;
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
        private readonly CheckBox boonDuration;
        private readonly CheckBox graph;
        private readonly NumericUpDown time;
        private readonly UILogParser _logParser;

        public BoonUI(DataGridView tableBoons, Label lblSelectedBoon, Label lblSelectedPhase, ComboBox cbPhase, ComboBox cbBoon, TabPage tabBoons, CheckBox boonDuration, CheckBox graph, NumericUpDown time, UILogParser logParser, List<string> activePlayers):base(activePlayers)
        {
            this.tableBoons = tableBoons;
            this.lblSelectedBoon = lblSelectedBoon;
            this.lblSelectedPhase = lblSelectedPhase;
            this.cbPhase = cbPhase;
            this.cbBoon = cbBoon;
            this.tabBoons = tabBoons;
            _logParser = logParser;
            this.boonDuration = boonDuration;
            this.graph = graph;
            this.time = time;
            cbBoon.SelectedIndexChanged += OnCbBoonSelectedIndexChanged;
            cbPhase.SelectedIndexChanged += OnCbPhaseSelectedIndexChanged;
            boonDuration.CheckedChanged += UpdateEvent;
            graph.CheckedChanged += UpdateEvent;
            time.ValueChanged += UpdateEvent;
        }

        private void UpdateEvent(object? sender, EventArgs e)
        {
            UpdatePanel();
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
            tableBoons.SuspendLayout();
            tableBoons.ClearTable();

            var Logs = _logParser.BulkLog.Logs;
            tableBoons.RowCount = ActivePlayers.Count + Groups.Count();
            tableBoons.ColumnCount = Logs.Count() + 1;

            var Phases = _logParser.BulkLog.GetPhases([]);
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
            var boonType = Logs.FirstOrDefault()?.GetBoonStackType(_selectedBoon) ?? BuffStackTyping.Stacking;
            var cellFormat = GetCellFormat(boonType);
            cbBoon.Items.Clear();
            cbBoon.Items.AddRange(boonNames);
            lblSelectedBoon.Text = _selectedBoon;

            tableBoons.TopLeftHeaderCell.Value = _selectedPhase;
            for (int x = 0; x < Logs.Count(); x++)
            {
                tableBoons.Columns[x].HeaderCell.Value = Logs[x].GetFileName();
                tableBoons.Columns[x].DefaultCellStyle.Format = cellFormat;
                tableBoons.Columns[x].MinimumWidth = 10;
            }
            tableBoons.Columns[Logs.Count()].HeaderCell.Value = "Average";
            ImageGenerator imageGenerator = new ImageGenerator();
            for (int y = 0; y < ActivePlayers.Count; y++)
            {
                tableBoons.Rows[y].HeaderCell.Value = ActivePlayers[y];
                //tableBoons.Rows.Insert(y, ActivePlayers[y]);
                List<double> boonNumbers = new();
                for (int x = 0; x < Logs.Count(); x++)
                {
                    if (!Logs[x].HasPlayer(ActivePlayers[y]))
                    {
                        tableBoons.Rows[y].Cells[x].Value = "";
                        continue;
                    }
                    if(graph.Checked)
                    {
                        List<(int,int)> boons = new();
                        var phases = Logs[x].GetPhases([]);
                        phases = phases.Skip(1).ToArray();
                        var phaseStart = Logs[x].GetPhaseStart(_selectedPhase);
                        while (phases.Count() > 0 && Logs[x].GetPhaseStart(phases.First()) < phaseStart)
                        {
                            phases = phases.Take(phases.Count() - 1).ToArray();
                        }

                        var phaseIndex = 0;
                        for(long i = phaseStart+(long)Math.Round(time.Value); i < Logs[x].GetPhaseEnd(_selectedPhase); i += 1000)
                        {
                            if(phases.Count() > phaseIndex && i > Logs[x].GetPhaseEnd(phases[phaseIndex]) && (_selectedPhase == "Full Fight" || _selectedPhase == ""))
                            {
                                phaseIndex++;
                            }
                            boons.Add(((int)Logs[x].GetBoon(ActivePlayers[y], _selectedBoon, _selectedPhase, i/1000, boonDuration.Checked), phaseIndex));
                        }
                        DataGridViewImageCell img = new DataGridViewImageCell();
                        var image = imageGenerator.GetGraph(boons.ToArray(), boonType == BuffStackTyping.Queue ? 30:25);
                        img.Value = image;
                        tableBoons.Rows[y].Cells[x] = img;
                    }
                    else
                    {
                        double boonUptime = Logs[x].GetBoon(ActivePlayers[y], _selectedBoon, _selectedPhase, (long)time.Value, boonDuration.Checked);
                        boonNumbers.Add(boonUptime);
                        tableBoons.Rows[y].Cells[x].Value = boonUptime;
                    }
                }
                if(boonNumbers.Count == 0)
                {
                    tableBoons.Rows[y].Cells[Logs.Count()].Value = "";
                    continue;
                }
                float RoundedAverage = 0;
                if(boonType == BuffStackTyping.Stacking)
                {
                    RoundedAverage = (float)Math.Round(boonNumbers.Select(x => x).Average(), 1);
                }
                else
                {
                    RoundedAverage = (float)Math.Round(boonNumbers.Select(x => x).Average(), 3);
                }
                tableBoons.Columns[Logs.Count()].DefaultCellStyle.Format = cellFormat;
                tableBoons.Rows[y].Cells[Logs.Count()].Value = RoundedAverage;


            }
            int row = ActivePlayers.Count;
            foreach (var group in Groups)
            {
                tableBoons.Rows[row].HeaderCell.Value = $"Group {group}";
                for (int x = 0; x < Logs.Count(); x++)
                {
                    var boonUptime = Logs[x].GetBoon(group, _selectedBoon, _selectedPhase, (long)time.Value, boonDuration.Checked);
                    tableBoons.Rows[row].Cells[x].Value = boonUptime;
                }
                row++;
            }
            tableBoons.UpdatePlayersWithClassicons(Logs, ActivePlayers.ToArray());
            tableBoons.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            tableBoons.ResumeLayout();
        }

        private string GetCellFormat(BuffStackTyping bt)
        {
            if (boonDuration.Checked)
            {
                return "F1";
            }
            return bt switch
            {
                BuffStackTyping.Queue => "P1",
                BuffStackTyping.Regeneration => "P1",
                _ => "F1",
            };
        }
    }
}
