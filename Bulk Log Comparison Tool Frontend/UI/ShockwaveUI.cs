using Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bulk_Log_Comparison_Tool_Frontend.UI
{
    internal class ShockwaveUI : PlayerUI
    {

        private readonly long _startPhaseOffset = 3000;
        private readonly long _shockwaveCooldown = 18315;
        private readonly long _shockwave2Internal = 2408;
        private readonly long _shockwave3Internal = 1934;

        private readonly DataGridView tableShockwave;
        private readonly TabPage tabShockwaves;
        private readonly UILogParser _logParser;

        public ShockwaveUI(DataGridView tableShockwave, TabPage tabShockwaves, UILogParser logParser, List<string> activePlayers) : base(activePlayers)
        {
            this.tableShockwave = tableShockwave;
            this.tabShockwaves = tabShockwaves;
            _logParser = logParser;
        }

        public override void UpdatePanel()
        {
            tabShockwaves.Controls.Remove(tableShockwave);
            tableShockwave.DataSource = null;
            tableShockwave.RowCount = ActivePlayers.Count;
            tableShockwave.ColumnCount = _logParser.BulkLog.Logs.Count();

            for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
            {
                tableShockwave.Columns[x].HeaderCell.Value = _logParser.BulkLog.Logs[x].GetFileName();
                tableShockwave.Columns[x].MinimumWidth = 10;
            }
            for (int y = 0; y < ActivePlayers.Count; y++)
            {
                tableShockwave.Rows[y].HeaderCell.Value = ActivePlayers[y];
                for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
                {
                    var phaseStart = _logParser.BulkLog.Logs[x].GetPhaseStart("Mordremoth");
                    var phaseEnd = _logParser.BulkLog.Logs[x].GetPhaseEnd("Mordremoth");
                    if (phaseStart == 0 || phaseEnd == 0)
                    {
                        tableShockwave.Rows[y].Cells[x].Value = "";
                        continue;
                    }
                    var waveStart = phaseStart + _startPhaseOffset + _shockwaveCooldown;
                    var wave = 0;

                    var resultsForPlayer = "";

                    while (true)
                    {
                        waveStart += GetWaveOffset(wave);
                        var hadStab = _logParser.BulkLog.Logs[x].HasBoonDuringTime(ActivePlayers[y], "Stability", waveStart, waveStart + 1000);
                        var wasAlive = _logParser.BulkLog.Logs[x].IsAlive(ActivePlayers[y], waveStart);
                        if (!wasAlive)
                        {
                            resultsForPlayer += "☠";
                        }
                        else if (hadStab)
                        {
                            resultsForPlayer += "✓";
                        }
                        else
                        {
                            resultsForPlayer += "   ";
                        }

                        wave++;
                        if (wave == 3)
                        {
                            wave = 0;
                            waveStart += _shockwaveCooldown;
                        }
                        if (waveStart + GetWaveOffset(wave) > phaseEnd)
                        {
                            break;
                        }
                        resultsForPlayer += " | ";
                    }
                    tableShockwave.Rows[y].Cells[x].Value = resultsForPlayer;

                }
            }
            tableShockwave.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            tabShockwaves.Controls.Add(tableShockwave);
        }

        private long GetWaveOffset(int wave)
        {

            switch (wave)
            {
                case 0:
                    return 0;
                case 1:
                    return _shockwave2Internal;
                case 2:
                    return _shockwave3Internal;
                default:
                    return -1;
            }
        }
    }
}
