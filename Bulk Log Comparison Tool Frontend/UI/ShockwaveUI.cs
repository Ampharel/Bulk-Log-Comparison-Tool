using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool_Frontend.Compare;

namespace Bulk_Log_Comparison_Tool_Frontend.UI
{
    internal class ShockwaveUI : PlayerUI
    {
        private readonly DataGridView tableShockwave;
        private readonly TabPage tabShockwaves;
        private readonly UILogParser _logParser;

        private ImageGenerator imageGenerator = new ImageGenerator();

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
            var Logs = _logParser.BulkLog.Logs;
            tableShockwave.ColumnCount = Logs.Count();

            for (int x = 0; x < Logs.Count(); x++)
            {
                tableShockwave.Columns[x].HeaderCell.Value = Logs[x].GetFileName();
                tableShockwave.Columns[x].DefaultCellStyle.Font = IPanel.columnFont;
                tableShockwave.Columns[x].MinimumWidth = 10;
            }
            for (int y = 0; y < ActivePlayers.Count; y++)
            {
                var Player = ActivePlayers[y];
                tableShockwave.Rows[y].HeaderCell.Value = ActivePlayers[y];
                for (int x = 0; x < Logs.Count(); x++)
                {
                    Image image = null;
                    var Log = Logs[x];
                    List<(long, int)> shockwaves = new();
                    shockwaves = GetShockwaves(Logs, x, shockwaves,0);
                    shockwaves = GetShockwaves(Logs, x, shockwaves,1);
                    shockwaves = GetShockwaves(Logs, x, shockwaves,2);

                    image = imageGenerator.GetImage(Log, Player, image, shockwaves);



                    if (image != null)
                    {
                        DataGridViewImageCell img = new DataGridViewImageCell();
                        img.Value = image;
                        tableShockwave.Rows[y].Cells[x] = img;
                    }

                }
            }
            tableShockwave.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            tabShockwaves.Controls.Add(tableShockwave);
        }

        private List<(long, int)> GetShockwaves(List<IParsedEvtcLog> Logs, int x, List<(long, int)> shockwaves, int shockwaveType)
        {
            foreach (var shockwave in Logs[x].GetShockwaves(shockwaveType))
            {
                shockwaves.Add((shockwave, shockwaveType));
            }
            return shockwaves;
        }
    }

}
