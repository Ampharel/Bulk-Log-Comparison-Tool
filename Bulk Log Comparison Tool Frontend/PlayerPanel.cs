using Bulk_Log_Comparison_Tool.DataClasses;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool_Frontend
{
    internal class PlayerPanel
    {
        private readonly int CBStartX = 3;
        private readonly int CBSpacing = 6;
        private readonly int CBWidth = 110;
        private readonly int CBStartY = 3;
        private readonly int CBHeight = 19;
        private readonly int ButtonWidth = 20;
        private readonly int ButtonHeight = 20;

        private Dictionary<string, CheckBox> _players = new();
        private BulkLog _logs;
        private Panel _panel;

        private List<string> _cachedPlayers = new();

        public delegate void PlayerSelectionChangedEventHandler(List<string> ActivePlayers);
        private event PlayerSelectionChangedEventHandler? _playerSelectionChangedEvent;

        public event PlayerSelectionChangedEventHandler PlayerSelectionChangedEvent
        {
            add
            {
                _playerSelectionChangedEvent += value;
            }
            remove
            {
                _playerSelectionChangedEvent -= value;
            }
        }

        public PlayerPanel(Panel panel, BulkLog logs)
        {
            _panel = panel;
            _logs = logs;
            Refresh();
        }

        private void ValidatePlayerList(string[] players)
        {
            foreach (var player in players)
            {
                if (!_cachedPlayers.Contains(player))
                {
                    _cachedPlayers.Add(player);
                }
            }
            foreach (var player in _cachedPlayers)
            {
                if (!players.Contains(player))
                {
                    _cachedPlayers.Remove(player);
                }
            }
        }

        public void Refresh()
        {
            _panel.Controls.Clear();
            _players.Clear();
            var players = _logs.GetPlayers();
            ValidatePlayerList(players);
            int index = 0;
            foreach (var player in _cachedPlayers)
            {
                var checkBox = new CheckBox();
                var btnUp = new Button();
                var btnDown = new Button();
                btnUp.Size = new Size(ButtonWidth, ButtonHeight);
                btnDown.Size = new Size(ButtonWidth, ButtonHeight);
                var x = CBStartX;
                var y = CBStartY + index * (CBHeight + CBSpacing);
                _players.Add(player, checkBox);
                checkBox.Text = player;
                checkBox.Checked = true;
                checkBox.AutoSize = false;
                checkBox.Location = new Point(x,y);
                btnUp.Location = new Point(x + CBWidth, y);
                btnDown.Location = new Point(x + CBWidth + ButtonWidth, y);
                btnUp.Text = "▲";
                btnDown.Text = "▼";
                checkBox.Name = $"cb{player}";
                checkBox.Size = new Size(CBWidth, CBHeight);
                checkBox.Text = $"{player}";
                checkBox.CheckedChanged += (sender, e) =>
                {
                    _playerSelectionChangedEvent?.Invoke(_cachedPlayers.Where(x => _players[x].Checked).ToList());
                };
                btnUp.Click += (sender, e) =>
                {
                    var index = _cachedPlayers.IndexOf(player);
                    if (index > 0)
                    {
                        _cachedPlayers.RemoveAt(index);
                        _cachedPlayers.Insert(index - 1, player);
                        Refresh();
                    }
                };
                btnDown.Click += (sender, e) =>
                {
                    var index = _cachedPlayers.IndexOf(player);
                    if (index < _cachedPlayers.Count - 1)
                    {
                        _cachedPlayers.RemoveAt(index);
                        _cachedPlayers.Insert(index + 1, player);
                        Refresh();
                    }
                };
                _panel.Controls.Add(checkBox);
                _panel.Controls.Add(btnUp);
                _panel.Controls.Add(btnDown);
                index++;
            }
            _playerSelectionChangedEvent?.Invoke(_cachedPlayers.Where(x => _players[x].Checked).ToList());
        }
    }
}
