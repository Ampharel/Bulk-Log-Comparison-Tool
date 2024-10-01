using Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool.Enums;
using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool_Frontend.Utils;
using System;

namespace Bulk_Log_Comparison_Tool_Frontend.UI
{
    internal class StealthAnalysisUI : PlayerUI
    {
        public static StealthAlgoritmns stealthAlgoritmn = StealthAlgoritmns.OutlierFiltering;
        private string _selectedPhase = "";
        
        private readonly DataGridView tableStealth;
        private readonly Label lblSelectedPhaseStealth;
        private readonly ComboBox cbStealthPhase;
        private readonly TabPage tabStealth;
        private readonly UILogParser _logParser;
        private readonly CheckBox _showLate;
        private readonly ComboBox _cbStealthAlgoritmn;
        private readonly Button _btnEnableStealthAlgoritmn;

        public StealthAnalysisUI(DataGridView tableStealth, Label lblSelectedPhaseStealth, ComboBox cbStealthPhase, TabPage tabStealth, UILogParser logParser, List<string> activePlayers, CheckBox showLate, ComboBox cbStealthAlgoritmn, Button btnEnableStealthAlgoritmn) : base(activePlayers)
        {
            this.tableStealth = tableStealth;
            this.lblSelectedPhaseStealth = lblSelectedPhaseStealth;
            this.cbStealthPhase = cbStealthPhase;
            this.tabStealth = tabStealth;
            _logParser = logParser;
            _showLate = showLate;
            _cbStealthAlgoritmn = cbStealthAlgoritmn;
            _btnEnableStealthAlgoritmn = btnEnableStealthAlgoritmn;
            cbStealthPhase.SelectedIndexChanged += OnCbStealthPhaseSelectedIndexChanged;
            _showLate.CheckedChanged += OnShowLateCheckedChanged;
            _cbStealthAlgoritmn.SelectedIndexChanged += OnCbStealthAlgoritmnSelectedIndexChanged;
            _btnEnableStealthAlgoritmn.Click += OnBtnEnableStealthAlgoritmnClick;
            _cbStealthAlgoritmn.Items.AddRange(Enum.GetNames(typeof(StealthAlgoritmns)));
        }

        private void OnBtnEnableStealthAlgoritmnClick(object? sender, EventArgs e)
        {
            _cbStealthAlgoritmn.Visible = !_cbStealthAlgoritmn.Visible;
        }

        private void OnCbStealthAlgoritmnSelectedIndexChanged(object? sender, EventArgs e)
        {
            stealthAlgoritmn = (StealthAlgoritmns)_cbStealthAlgoritmn.SelectedIndex;
            UpdatePanel();
        }

        private void OnShowLateCheckedChanged(object? sender, EventArgs e)
        {
            UpdatePanel();
        }

        public void OnCbStealthPhaseSelectedIndexChanged(object? sender, EventArgs e)
        {
            _selectedPhase = cbStealthPhase.SelectedItem?.ToString() ?? "";
            UpdatePanel();
        }

        ~StealthAnalysisUI()
        {
            cbStealthPhase.SelectedIndexChanged -= OnCbStealthPhaseSelectedIndexChanged;
        }

        public override void UpdatePanel()
        {
            if (ActivePlayers.Count == 0) return;
            var parent = tableStealth.RemoveFromParent();
            tableStealth.ClearTable();
            tableStealth.RowCount = ActivePlayers.Count;
            var Logs = _logParser.BulkLog.Logs;
            tableStealth.ColumnCount = Logs.Count() + 1;

            var StealthPhases = _logParser.BulkLog.GetStealthPhases();
            if (_selectedPhase == "" || !StealthPhases.Contains(_selectedPhase))
            {
                var Phase = StealthPhases.FirstOrDefault();
                if (Phase == null)
                {
                    return;
                }
                _selectedPhase = Phase;
            }
            lblSelectedPhaseStealth.Text = _selectedPhase;
            cbStealthPhase.Items.Clear();
            cbStealthPhase.Items.AddRange(StealthPhases);

            tableStealth.TopLeftHeaderCell.Value = _selectedPhase;
            for (int x = 0; x < Logs.Count(); x++)
            {
                tableStealth.Columns[x].HeaderCell.Value = Logs[x].GetFileName();
                tableStealth.Columns[x].MinimumWidth = 10;
            }
            for (int y = 0; y < ActivePlayers.Count; y++)
            {
                tableStealth.Rows[y].HeaderCell.Value = ActivePlayers[y];
                int stealthCount = 0;
                int successCount = 0;
                for (int x = 0; x < Logs.Count(); x++)
                {
                    if (!Logs[x].HasPlayer(ActivePlayers[y]))
                    {
                        tableStealth.Rows[y].Cells[x].Value = "";
                        continue;
                    }
                    var StealthForPlayer = Logs[x].GetStealthResult(ActivePlayers[y], stealthAlgoritmn, _showLate.Checked);
                    var StealthForPhase = StealthForPlayer.Where(x => x.Item1 == _selectedPhase).Select(x => x.Item2).FirstOrDefault();

                    var text = StealthForPhase;
                    if (text == null && _logParser.BulkLog.GetPlayers().Contains(ActivePlayers[y]))
                    {
                        text = " ";
                    }
                    else
                    {
                        stealthCount++;
                        if (text?.Equals("✓") ?? false)
                        {
                            successCount++;
                        }
                    }
                    tableStealth.Rows[y].Cells[x].Value = text;
                }

                tableStealth.Rows[y].Cells[Logs.Count()].Value = $"{successCount}/{stealthCount}";
            }
            tableStealth.UpdatePlayersWithClassicons(Logs, ActivePlayers.ToArray());
            tableStealth.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            tableStealth.AddToParent(parent);
        }
    }
}
