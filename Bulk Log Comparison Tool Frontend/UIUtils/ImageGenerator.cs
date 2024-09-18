using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool_Frontend.UI;
using Bulk_Log_Comparison_Tool_Frontend.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool_Frontend.Compare
{
    internal class ImageGenerator
    {

        public Image GetImage(IParsedEvtcLog Log, string Player, Image image, List<(long, int)> shockwaves)
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
                if (!Log.HasPlayer(Player))
                {
                    continue;
                }
                var hadStab = Log.HasBoonDuringTime(Player, "Stability", shockwave.Item1, shockwave.Item1 + 2000);
                var wasHit = Log.GetMechanicLogs(mechanic, start: shockwave.Item1, end: shockwave.Item1 + 2000).Where(x => x.Item1.Equals(Player)).Count() > 0;

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
            Font font = new Font(fontName, IPanel.columnFont.Size + 4);
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
            Font font = new Font(fontName, IPanel.columnFont.Size+4);
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
            Font font = new Font(fontName, IPanel.columnFont.Size + 4);
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
            Font font = new Font(fontName, IPanel.columnFont.Size + 4);
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
            Font font = new Font(fontName, IPanel.columnFont.Size + 4);
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
