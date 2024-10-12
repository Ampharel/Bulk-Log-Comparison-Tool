using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.Enums;
using Bulk_Log_Comparison_Tool_Frontend.UI;
using Bulk_Log_Comparison_Tool_Frontend.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool_Frontend.Compare
{
    internal class ImageGenerator
    {

        public Image? GetImage(IParsedEvtcLog Log, string Player, Image? image, List<(long, int)> shockwaves)
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
                
                var hadStab = Log.HasStabDuringShockwave(Player, (ShockwaveType)shockwave.Item2, shockwave.Item1, out var intersectionTime);
                var wasHit = Log.GetMechanicLogs(mechanic, start: intersectionTime-1000, end: intersectionTime+1000).Where(x => x.Item1.Equals(Player)).Count() > 0;

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
            Image _checkMarkImage = new Bitmap(32, 32);
            Graphics graphics = Graphics.FromImage(_checkMarkImage);
            Font font = new Font(fontName, IPanel.columnFont.Size + 4);
            StringFormat format = StringFormat.GenericDefault;
            graphics.DrawString("✓", font, GetBrushColour(shockwaveType), 0, 0);
            return _checkMarkImage;
        }

        private Image GetWarningImage(int shockwaveType)
        {
            Image _warningImage = new Bitmap(32, 32);
            Graphics graphics = Graphics.FromImage(_warningImage);
            Font font = new Font(fontName, IPanel.columnFont.Size + 4);
            StringFormat format = StringFormat.GenericDefault;
            graphics.DrawString("⚠", font, GetBrushColour(shockwaveType), 0, 0);
            return _warningImage;
        }

        private Image GetSkullImage(int shockwaveType)
        {
            Image _skullImage = new Bitmap(32, 32);
            Graphics graphics = Graphics.FromImage(_skullImage);
            Font font = new Font(fontName, IPanel.columnFont.Size + 4);
            StringFormat format = StringFormat.GenericDefault;
            graphics.DrawString("☠", font, GetBrushColour(shockwaveType), 0, 0);
            return _skullImage;
        }

        private Image GetShieldImage(int shockwaveType)
        {
            Image _shieldImage = new Bitmap(32, 32);
            Graphics graphics = Graphics.FromImage(_shieldImage);
            Font font = new Font(fontName, IPanel.columnFont.Size + 4);
            StringFormat format = StringFormat.GenericDefault;
            graphics.DrawString("🛡", font, GetBrushColour(shockwaveType), 0, 0);
            return _shieldImage;
        }
        private Image GetDownedImage(int shockwaveType)
        {
            Image _downedImage = new Bitmap(32, 32);
            Graphics graphics = Graphics.FromImage(_downedImage);
            Font font = new Font(fontName, IPanel.columnFont.Size + 4);
            StringFormat format = StringFormat.GenericDefault;
            graphics.DrawString("🔻", font, GetBrushColour(shockwaveType), 0, 0);
            return _downedImage;
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
        
        public Image GetGraph((int,int)[] values, int maxValue)
        {
            maxValue += 1;
            int width = values.Count(); // adjust to your desired width
            int height = maxValue; // adjust to your desired height
            if(width == 0)
            {
                width = 1;
            }
            Image image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            Font font = new Font(fontName, IPanel.columnFont.Size + 4);
            for(int i = 0; i < values.Count(); i++)
            {
                graphics.DrawLine(new Pen(GetBrush(values[i].Item2)), i, maxValue, i, maxValue - values[i].Item1);
            }
            return image;
        }

        public Brush GetBrush(int i)
        {
            i = i % 5;
            return i switch
            {
                0 => Brushes.Green,
                1 => Brushes.Blue,
                2 => Brushes.Purple,
                3 => Brushes.Red,
                4 => Brushes.Yellow,
                _ => Brushes.Black
            };
        }

        public static Image BlankImage = new Bitmap(1, 1);
        private readonly Dictionary<string, Image> _specIcons = new();

        public Image? GetSpecIcon(string currentSpec)
        {
            string path = $"icons/{currentSpec.ToLower()}.png";
            if (File.Exists(path))
            {
                return Image.FromFile(path);
            }
            return BlankImage;
        }

    }
}
