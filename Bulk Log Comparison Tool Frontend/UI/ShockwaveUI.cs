using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;

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
            var Logs = _logParser.BulkLog.Logs;
            tableShockwave.ColumnCount = Logs.Count();

            for (int x = 0; x < Logs.Count(); x++)
            {
                tableShockwave.Columns[x].HeaderCell.Value = Logs[x].GetFileName();
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

                    image = GetImage(Log, Player, image, shockwaves);



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

        private Image GetImage(IParsedEvtcLog Log, string Player, Image image, List<(long,int)> shockwaves)
        {
            var mechanic = "";
            var sortedShockwaves = shockwaves.OrderBy(x => x.Item1);
            foreach (var shockwave in sortedShockwaves)
            {
                switch (shockwave.Item2)
                {
                    case 0:
                        mechanic = "Mordremoth Shockwave";
                        break;
                    case 1:
                        mechanic = "Soo-Won Tsunami";
                        break;
                    case 2:
                        mechanic = "Obliterator Shockwave"; //Name needs checking
                        break;
                }
                if(!Log.HasPlayer(Player))
                {
                    continue;
                }
                var hadStab = Log.HasBoonDuringTime(Player, "Stability", shockwave.Item1, shockwave.Item1 + 1000);
                var wasHit = Log.GetMechanicLogs("Mordremoth Shockwave", start: shockwave.Item1, end: shockwave.Item1 + 1000).Where(x => x.Item1.Equals(Player)).Count() > 0;

                var wasAlive = Log.IsAlive(Player, shockwave.Item1);
                if (!wasAlive)
                {
                    image = image.StitchImages(GetSkullImage(shockwave.Item2));
                }
                else if (hadStab && wasHit)
                {
                    image = image.StitchImages(GetShieldImage(shockwave.Item2));
                }
                else if (hadStab)
                {
                    image = image.StitchImages(GetCheckmarkImage(shockwave.Item2));
                }
                else if (wasHit)
                {
                    image = image.StitchImages(GetDownedImage(shockwave.Item2));
                }
                else
                {
                    image = image.StitchImages(GetWarningImage(shockwave.Item2));
                }
            }

            return image;
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
        enum StabStatus
        {
            Dead,
            Stability,
            None
        };

        private const string fontName = "Segoe UI Symbol";

        private Image GetCheckmarkImage(int shockwaveType)
        {
            int width = 32; // adjust to your desired width
            int height = 32; // adjust to your desired height
            Image image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            Font font = new Font(fontName, 16);
            StringFormat format = StringFormat.GenericDefault;
            graphics.DrawString("✓", font, GetBrushColour(shockwaveType), 0, 0);
            return image;
        }
        private Image GetWarningImage(int shockwaveType)
        {
            int width = 32; // adjust to your desired width
            int height = 32; // adjust to your desired height
            Image image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            Font font = new Font(fontName, 16);
            StringFormat format = StringFormat.GenericDefault;
            graphics.DrawString("⚠", font, GetBrushColour(shockwaveType), 0, 0);
            return image;
        }
        private Image GetSkullImage(int shockwaveType)
        {
            int width = 32; // adjust to your desired width
            int height = 32; // adjust to your desired height
            Image image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            Font font = new Font(fontName, 16);
            StringFormat format = StringFormat.GenericDefault;
            graphics.DrawString("☠", font, GetBrushColour(shockwaveType), 0, 0);
            return image;
        }
        private Image GetShieldImage(int shockwaveType)
        {
            int width = 32; // adjust to your desired width
            int height = 32; // adjust to your desired height
            Image image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            Font font = new Font(fontName, 16);
            StringFormat format = StringFormat.GenericDefault;
            graphics.DrawString("🛡", font, GetBrushColour(shockwaveType), 0, 0);
            return image;
        }
        private Image GetDownedImage(int shockwaveType)
        {
            int width = 32; // adjust to your desired width
            int height = 32; // adjust to your desired height
            Image image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            Font font = new Font(fontName, 16);
            StringFormat format = StringFormat.GenericDefault;
            graphics.DrawString("🔻", font, GetBrushColour(shockwaveType), 0, 0);
            return image;
        }

        private Brush GetBrushColour(int shockwaveType)
        {
            switch (shockwaveType)
            {
                case 0:
                    return Brushes.Green;
                case 1:
                    return Brushes.Blue;
                case 2:
                    return Brushes.Purple;
                default:
                    return Brushes.Black;
            }
        }

    }

}
