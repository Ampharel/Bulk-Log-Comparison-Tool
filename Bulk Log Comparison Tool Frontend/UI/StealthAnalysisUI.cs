using Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;

namespace Bulk_Log_Comparison_Tool_Frontend.UI
{
    internal class StealthAnalysisUI : PlayerUI
    {
        private string _selectedPhase = "";
        
        private readonly DataGridView tableStealth;
        private readonly Label lblSelectedPhaseStealth;
        private readonly ComboBox cbStealthPhase;
        private readonly TabPage tabStealth;
        private readonly UILogParser _logParser;

        public StealthAnalysisUI(DataGridView tableStealth, Label lblSelectedPhaseStealth, ComboBox cbStealthPhase, TabPage tabStealth, UILogParser logParser, List<string> activePlayers):base(activePlayers)
        {
            this.tableStealth = tableStealth;
            this.lblSelectedPhaseStealth = lblSelectedPhaseStealth;
            this.cbStealthPhase = cbStealthPhase;
            this.tabStealth = tabStealth;
            _logParser = logParser;
            cbStealthPhase.SelectedIndexChanged += OnCbStealthPhaseSelectedIndexChanged;
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
            tabStealth.Controls.Remove(tableStealth);
            tableStealth.DataSource = null;
            tableStealth.RowCount = ActivePlayers.Count;
            tableStealth.ColumnCount = _logParser.BulkLog.Logs.Count() + 1;

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
            for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
            {
                tableStealth.Columns[x].HeaderCell.Value = _logParser.BulkLog.Logs[x].GetFileName();
                tableStealth.Columns[x].MinimumWidth = 10;
            }
            for (int y = 0; y < ActivePlayers.Count; y++)
            {
                tableStealth.Rows[y].HeaderCell.Value = ActivePlayers[y];
                int stealthCount = 0;
                int successCount = 0;
                for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
                {
                    var StealthForPlayer = _logParser.BulkLog.Logs[x].GetStealthResult(ActivePlayers[y]);
                    var StealthForPhase = StealthForPlayer.Where(x => x.Item1 == _selectedPhase).Select(x => x.Item2).FirstOrDefault();

                    var text = _logParser.BulkLog.Logs[x].GetStealthResult(ActivePlayers[y]).Where(x => x.Item1 == _selectedPhase).Select(x => x.Item2).FirstOrDefault();
                    if (text == null && _logParser.BulkLog.GetPlayers().Contains(ActivePlayers[y]))
                    {
                        text = "No stealth";
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

                tableStealth.Rows[y].Cells[_logParser.BulkLog.Logs.Count()].Value = $"{successCount}/{stealthCount}";
            }
            tableStealth.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            tabStealth.Controls.Add(tableStealth);
        }
    }
}
