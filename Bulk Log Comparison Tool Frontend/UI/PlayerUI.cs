using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool_Frontend.UI
{
    internal abstract class PlayerUI
    {
        public Font columnFont = new Font("Verdana",20f);
        private readonly List<string> _activePlayers;

        public PlayerUI(List<string> activePlayers)
        {
            _activePlayers = activePlayers;
        }

        public void UpdateActivePlayers(List<string> activePlayers)
        {
            _activePlayers.Clear();
            _activePlayers.AddRange(activePlayers);
            UpdatePanel();
        }

        public List<string> ActivePlayers => _activePlayers;

        public abstract void UpdatePanel();
    }
}
