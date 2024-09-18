using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool_Frontend.Compare;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool_Frontend.UI
{
    internal class LogSummaryUI : IPanel
    {
        private readonly TabPage _tabSummary;
        private readonly DataGridView tableStealth;
        private readonly DataGridView tableShockwave;
        private readonly DataGridView tableMechanics;
        private readonly UILogParser logParser;
        private readonly ComboBox logComboBox;


        private ImageGenerator imageGenerator = new ImageGenerator();

        public LogSummaryUI(TabPage tabSummary, DataGridView tableStealth, DataGridView tableShockwaves, DataGridView tableMechanics, UILogParser logParser, ComboBox logComboBox)
        {
            _tabSummary = tabSummary;
            this.tableStealth = tableStealth;
            tableShockwave = tableShockwaves;
            this.tableMechanics = tableMechanics;
            this.logParser = logParser;
            this.logComboBox = logComboBox;
            this.logComboBox.SelectedIndexChanged += OnLogComboBoxSelectedIndexChanged;
        }

        ~LogSummaryUI()
        {
            logComboBox.SelectedIndexChanged -= OnLogComboBoxSelectedIndexChanged;
        }

        private void OnLogComboBoxSelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdatePanel();
        }

        private IParsedEvtcLog? _selectedLog;

        public void UpdatePanel()
        {
            _selectedLog = logParser.BulkLog.GetLog(logComboBox.SelectedItem?.ToString() ?? "");
            if (_selectedLog == null)
            {
                return;
            }
            UpdateStealthTable();
            UpdateShockwaveTable();
        }

        private void UpdateShockwaveTable()
        {
            if (_selectedLog == null)
            {
                return;
            }
            var players = _selectedLog.GetPlayers();
            _tabSummary.Controls.Remove(tableShockwave);
            tableShockwave.DataSource = null;
            tableShockwave.ColumnCount = 1;
            tableShockwave.RowCount = players.Length;

            for (int y = 0; y < players.Length; y++)
            {
                var Player = players[y];
                tableShockwave.Rows[y].HeaderCell.Value = players[y];

                Image image = null;
                List<(long, int)> shockwaves = new();
                foreach (var shockwave in _selectedLog.GetShockwaves(0))
                {
                    shockwaves.Add((shockwave,0));
                }
                foreach (var shockwave in _selectedLog.GetShockwaves(1))
                {
                    shockwaves.Add((shockwave,1));
                }
                foreach (var shockwave in _selectedLog.GetShockwaves(2))
                {
                    shockwaves.Add((shockwave,2));
                }

                image = imageGenerator.GetImage(_selectedLog, Player, image, shockwaves);



                if (image != null)
                {
                    DataGridViewImageCell img = new DataGridViewImageCell();
                    img.Value = image;
                    tableShockwave.Rows[y].Cells[0] = img;
                }
            }
            tableShockwave.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            _tabSummary.Controls.Add(tableShockwave);
        }

        private void UpdateStealthTable()
        {
            if(_selectedLog == null)
            {
                return;
            }
            var players = _selectedLog.GetPlayers();
            _tabSummary.Controls.Remove(tableStealth);
            tableStealth.DataSource = null;
            tableStealth.RowCount = players.Length;

            var bulkLog = new BulkLog(new List<IParsedEvtcLog> { _selectedLog });
            string[] stealthPhases = bulkLog.GetStealthPhases();
            tableStealth.ColumnCount = stealthPhases.Count();

            for (int x = 0; x < stealthPhases.Length; x++)
            {
                tableStealth.Columns[x].HeaderCell.Value = stealthPhases[x];
                tableStealth.Columns[x].DefaultCellStyle.Font = IPanel.columnFont;
            }

            for (int y = 0; y < players.Length; y++)
            {
                tableStealth.Rows[y].HeaderCell.Value = players[y];
                int stealthCount = 0;
                int successCount = 0;
                var StealthForPlayer = _selectedLog.GetStealthResult(players[y]);
                for (int x = 0; x < stealthPhases.Count(); x++)
                {
                    var currentPhase = stealthPhases[x];
                    var StealthForPhase = StealthForPlayer.Where(x => x.Item1 == currentPhase).Select(x => x.Item2).FirstOrDefault();

                    if (StealthForPhase == null && logParser.BulkLog.GetPlayers().Contains(players[y]))
                    {
                        StealthForPhase = " ";
                    }
                    else
                    {
                        stealthCount++;
                        if (StealthForPhase?.Equals("✓") ?? false)
                        {
                            successCount++;
                        }
                    }
                    tableStealth.Rows[y].Cells[x].Value = StealthForPhase;
                }
            }
            tableStealth.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            _tabSummary.Controls.Add(tableStealth);
        }
    }
}
