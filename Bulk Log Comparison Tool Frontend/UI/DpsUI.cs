using Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool.Util;
using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool_Frontend.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bulk_Log_Comparison_Tool_Frontend.UI
{
    internal class DpsUI : PlayerUI
    {

        private string _selectedPhase = "";
        private readonly DataGridView tableDps;
        private readonly Label lblSelectedPhaseDps;
        private readonly ComboBox cbDpsPhase;
        private readonly TabPage tabDps;
        private readonly UILogParser _logParser;
        private readonly CheckBox _cumulative;
        private readonly CheckBox _defiance;
        private readonly CheckBox _allTargets;




        public DpsUI(DataGridView tableDps, Label lblSelectedPhaseDps, ComboBox cbDpsPhase, TabPage tabDps, UILogParser logParser, List<string> activePlayers, CheckBox cumulative, CheckBox defiance, CheckBox allTargets) : base(activePlayers)
        {
            this.tableDps = tableDps;
            this.lblSelectedPhaseDps = lblSelectedPhaseDps;
            this.cbDpsPhase = cbDpsPhase;
            this.tabDps = tabDps;
            _logParser = logParser;
            _cumulative = cumulative;
            _defiance = defiance;
            _allTargets = allTargets;
            cbDpsPhase.SelectedIndexChanged += OnCbDpsPhaseSelectedIndexChanged;
            _cumulative.CheckedChanged += OnCheckboxCumulativeCheckedChanged;
            _defiance.CheckedChanged += OnCheckboxDefianceCheckedChanged;
            _allTargets.CheckedChanged += OnCheckboxTargetsCheckedChanged;
        }

        private void OnCheckboxCumulativeCheckedChanged(object? sender, EventArgs e)
        {
            UpdatePanel();
        }

        public void OnCbDpsPhaseSelectedIndexChanged(object? sender, EventArgs e)
        {
            _selectedPhase = cbDpsPhase.SelectedItem?.ToString() ?? "";
            UpdatePanel();
        }

        public void OnCheckboxDefianceCheckedChanged(object? sender, EventArgs e)
        {
            if(_defiance.Checked)
            {
                _cumulative.Checked = true;
                _cumulative.Enabled = false;
            }
            else
            {
                _cumulative.Enabled = true;
            }
            UpdatePanel();
        }

        public void OnCheckboxTargetsCheckedChanged(object? sender, EventArgs e)
        {
            UpdatePanel();
        }

        ~DpsUI()
        {
            cbDpsPhase.SelectedIndexChanged -= OnCbDpsPhaseSelectedIndexChanged;
            _cumulative.CheckedChanged -= OnCheckboxCumulativeCheckedChanged;
            _defiance.CheckedChanged -= OnCheckboxDefianceCheckedChanged;
            _allTargets.CheckedChanged -= OnCheckboxTargetsCheckedChanged;
        }

        public override void UpdatePanel()
        {
            tableDps.SuspendLayout();
            tableDps.ClearTable();
            tableDps.RowCount = ActivePlayers.Count + 1;
            var Logs = _logParser.BulkLog.Logs;
            tableDps.ColumnCount = Logs.Count() + 1;

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
            cbDpsPhase.Items.Clear();
            cbDpsPhase.Items.AddRange(Phases);
            lblSelectedPhaseDps.Text = _selectedPhase;

            tableDps.TopLeftHeaderCell.Value = _selectedPhase;
            for (int x = 0; x < Logs.Count(); x++)
            {
                tableDps.Columns[x].HeaderCell.Value = Logs[x].GetFileName();
                tableDps.Columns[x].MinimumWidth = 10;
                tableDps.Columns[x].DefaultCellStyle.Format = "N0";
                tableDps.Columns[x].DefaultCellStyle.FormatProvider = new CultureInfo("ru-RU");
            }
            var count = Logs.Count();
            tableDps.Columns[count].HeaderCell.Value = "Average";
            tableDps.Columns[count].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            tableDps.Columns[count].DefaultCellStyle.Format = "N0";
            tableDps.Columns[count].DefaultCellStyle.FormatProvider = new CultureInfo("ru-RU");
            var TotalDps = new Dictionary<string, List<double>>();
            for (int y = 0; y < ActivePlayers.Count; y++)
            {
                tableDps.Rows[y].HeaderCell.Value = ActivePlayers[y];
                List<double> dpsnumbers = new();
                for (int x = 0; x < Logs.Count(); x++)
                {
                    if (!Logs[x].HasPlayer(ActivePlayers[y]))
                    {
                        tableDps.Rows[y].Cells[x].Value = "";
                        continue;
                    }
                    if (TotalDps.ContainsKey(Logs[x].GetFileName()) == false)
                    {
                        TotalDps.Add(Logs[x].GetFileName(), new List<double>());
                    }
                    double dps = Logs[x].GetPlayerDps(ActivePlayers[y], _selectedPhase, _allTargets.Checked, _cumulative.Checked, _defiance.Checked);
                    TotalDps[Logs[x].GetFileName()].Add(dps);
                    dpsnumbers.Add(dps);
                    tableDps.Rows[y].Cells[x].Value = DpsToText(dps);// text;
                }
                var dpsNumbersWithoutZero = dpsnumbers.Where(x => x != 0).ToList();
                var averageDps = dpsNumbersWithoutZero.Count == 0 ? 0 : dpsNumbersWithoutZero.Average();
                float Average = (float)Math.Round(averageDps / 1000f);
                tableDps.Rows[y].Cells[Logs.Count()].Value = DpsToText(averageDps);//$"{Average}k";
            }
            int row = ActivePlayers.Count + 1;

            tableDps.Rows[ActivePlayers.Count].HeaderCell.Value = GetTotalHeaderType();
            for (int x = 0; x < Logs.Count(); x++)
            {
                tableDps.Rows[ActivePlayers.Count].Cells[x].Value = DpsToText(TotalDps[Logs[x].GetFileName()].Sum());
            }
            tableDps.UpdatePlayersWithClassicons(Logs, ActivePlayers.ToArray());
            tableDps.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            tableDps.ResumeLayout();
        }

        private string GetTotalHeaderType()
        {
            if(_defiance.Checked)
            {
                return "Total CC";
            }
            return "Total DPS";
        }

        private string DpsToText(double dps)
        {
            if(dps == 0)
            {
                return "0";
            }
            if(_defiance.Checked)
            {
                return dps.ToString("N0");
            }
            return (dps / 1000).ToString("F1") + "k";
        }
    }
}
