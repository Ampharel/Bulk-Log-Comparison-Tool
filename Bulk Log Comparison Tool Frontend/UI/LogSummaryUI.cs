﻿using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.Enums;
using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool_Frontend.Compare;
using Bulk_Log_Comparison_Tool_Frontend.Utils;
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
        private readonly DataGridView tableDeaths;
        private readonly UILogParser logParser;
        private readonly ComboBox logComboBox;

        private readonly string[] mechanicNames = ["Debilitated", "Spread Bait", "Red Bait", "Orb Push", "Last Laugh", "Infirmity"];
        private readonly string[] mechanics = ["Debilitated Applied", "Spread Bait", "Red Bait", "Orb Push", "Void Explosion", "Infirmity"];

        private ImageGenerator imageGenerator = new ImageGenerator();

        public LogSummaryUI(TabPage tabSummary, DataGridView tableStealth, DataGridView tableShockwave, DataGridView tableMechanics, DataGridView tableDeaths, UILogParser logParser, ComboBox logComboBox)
        {
            _tabSummary = tabSummary;
            this.tableStealth = tableStealth;
            this.tableShockwave = tableShockwave;
            this.tableMechanics = tableMechanics;
            this.tableDeaths = tableDeaths;
            this.logParser = logParser;
            this.logComboBox = logComboBox;
            logComboBox.SelectedIndexChanged += OnLogComboBoxSelectedIndexChanged;
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
            UpdateMechanicsTable();
            UpdateDeathTable();
        }

        private void UpdateDeathTable()
        {
            if (_selectedLog == null)
            {
                return;
            }
            var players = _selectedLog.GetPlayers();
            var maxDowns = 1;
            foreach (var player in players)
            {
                var downed = _selectedLog.GetDownReasons(player);
                if (downed.Count() > maxDowns)
                {
                    maxDowns = downed.Count();
                }
            }
            _tabSummary.Controls.Remove(tableDeaths);
            tableDeaths.ClearTable();
            tableDeaths.TopLeftHeaderCell.Value = "Downs";
            tableDeaths.ColumnCount = maxDowns;
            tableDeaths.RowCount = players.Length;
            for (int y = 0; y < players.Length; y++)
            {
                var Player = players[y];
                var downed = _selectedLog.GetDownReasons(Player);
                tableDeaths.Rows[y].HeaderCell.Value = Player;

                for (int x = 0; x < maxDowns; x++)
                {
                    if(x < downed.Count())
                    {
                        tableDeaths.Rows[y].Cells[x].Value = downed[x] ?? "";
                    }
                    else
                    {
                        tableDeaths.Rows[y].Cells[x].Value = "";
                    }
                }
            }

            tableDeaths.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            _tabSummary.Controls.Add(tableDeaths);
        }

        private void UpdateMechanicsTable()
        {
            if (_selectedLog == null)
            {
                return;
            }
            var players = _selectedLog.GetPlayers();
            _tabSummary.Controls.Remove(tableMechanics);
            tableMechanics.ClearTable();
            tableMechanics.ColumnCount = 6;
            tableMechanics.TopLeftHeaderCell.Value = "Mechanics";
            for (int i = 0; i < mechanicNames.Length; i++)
            {
                tableMechanics.Columns[i].HeaderCell.Value = mechanics[i];
            }
            tableMechanics.RowCount = players.Length;

            for (int x = 0; x < mechanics.Length; x++)
            {
                var mechs = _selectedLog.GetMechanicLogs(mechanics[x].Split(':').Last().TrimStart(' '), "Full Fight");
                for (int y = 0; y < players.Length; y++)
                {
                    var Player = players[y];
                    tableMechanics.Rows[y].HeaderCell.Value = Player;
                    tableMechanics.Rows[y].Cells[x].Value = mechs.Where(x => x.Item1 == Player).Count();
                }
            }
            tableMechanics.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            _tabSummary.Controls.Add(tableMechanics);
        }

        private void UpdateShockwaveTable()
        {
            if (_selectedLog == null)
            {
                return;
            }
            var players = _selectedLog.GetPlayers();
            _tabSummary.Controls.Remove(tableShockwave);
            tableShockwave.ClearTable();
            tableShockwave.ColumnCount = 1;
            tableShockwave.TopLeftHeaderCell.Value = "Shockwaves";
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

            tableStealth.ClearTable();
            tableStealth.RowCount = players.Length;
            tableStealth.TopLeftHeaderCell.Value = "Stealth";
            tableStealth.ColumnCount = 1;

            var bulkLog = new BulkLog(new List<IParsedEvtcLog> { _selectedLog });
            string[] stealthPhases = bulkLog.GetStealthPhases();
            if (stealthPhases.Count() == 0)
            {
                tableStealth.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
                _tabSummary.Controls.Add(tableStealth);
                return;
            }
            tableStealth.ColumnCount = stealthPhases.Count();

            for (int x = 0; x < stealthPhases.Length; x++)
            {
                tableStealth.Columns[x].HeaderCell.Value = stealthPhases[x];
            }

            for (int y = 0; y < players.Length; y++)
            {
                tableStealth.Rows[y].HeaderCell.Value = players[y];
                int stealthCount = 0;
                int successCount = 0;
                var StealthForPlayer = _selectedLog.GetStealthResult(players[y], StealthAnalysisUI.stealthAlgoritmn);
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