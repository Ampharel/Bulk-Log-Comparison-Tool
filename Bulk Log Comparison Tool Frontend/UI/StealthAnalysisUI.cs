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
                tableStealth.Columns[x].DefaultCellStyle.Font = IPanel.columnFont;
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
                    var StealthForPlayer = Logs[x].GetStealthResult(ActivePlayers[y]);
                    var StealthForPhase = StealthForPlayer.Where(x => x.Item1 == _selectedPhase).Select(x => x.Item2).FirstOrDefault();

                    var text = Logs[x].GetStealthResult(ActivePlayers[y]).Where(x => x.Item1 == _selectedPhase).Select(x => x.Item2).FirstOrDefault();
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
                tableStealth.Columns[Logs.Count()].DefaultCellStyle.Font = IPanel.columnFont;
            }
            tableStealth.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            tabStealth.Controls.Add(tableStealth);
        }
    }
}
