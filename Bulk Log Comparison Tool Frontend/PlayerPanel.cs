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
        private readonly int CBWidth = 83;
        private readonly int CBStartY = 3;
        private readonly int CBHeight = 19;

        private Dictionary<string, CheckBox> _players = new();
        private BulkLog _logs;
        private Panel _panel;

        public delegate void PlayerSelectionChangedEventHandler(List<string> ActivePlayers);
        private event PlayerSelectionChangedEventHandler _playerSelectionChangedEvent;

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
        public void Refresh()
        {
            _panel.Controls.Clear();
            _players.Clear();
            var players = _logs.GetPlayers();
            int index = 0;
            foreach (var player in players)
            {
                var checkBox = new CheckBox();
                _players.Add(player, checkBox);
                checkBox.Text = player;
                checkBox.Checked = true;
                checkBox.AutoSize = false;
                checkBox.Location = new Point(CBStartX + index * (CBWidth + CBSpacing), CBStartY);
                checkBox.Name = $"cb{player}";
                checkBox.Size = new Size(CBWidth, CBHeight);
                checkBox.Text = $"{player}";
                checkBox.CheckedChanged += (sender, e) =>
                {
                    _playerSelectionChangedEvent?.Invoke(_players.Where(x => x.Value.Checked).Select(x => x.Key).ToList());
                };
                _panel.Controls.Add(checkBox);
                index++;
            }
            _playerSelectionChangedEvent?.Invoke(_players.Where(x => x.Value.Checked).Select(x => x.Key).ToList());
        }
    }
}
