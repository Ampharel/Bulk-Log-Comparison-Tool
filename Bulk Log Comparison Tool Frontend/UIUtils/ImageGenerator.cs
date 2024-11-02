using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.Enums;
using Bulk_Log_Comparison_Tool.Util;
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
                    image = image.StitchImages(GetImage(shockwave.Item2,"death"));
                }
                else if (hadStab && wasHit)
                {
                    image = image.StitchImages(GetImage(shockwave.Item2,"shield"));
                }
                else if (hadStab)
                {
                    image = image.StitchImages(GetImage(shockwave.Item2, "jumped"));
                }
                else if (wasHit)
                {
                    image = image.StitchImages(GetImage(shockwave.Item2,"down"));
                }
                else
                {
                    image = image.StitchImages(GetImage(shockwave.Item2,"warning"));
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


        private Image GetImage(int shockwaveType, string name)
        {
            Bitmap bmp = (Bitmap)GetIcon(name);
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), 
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] argbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, bytes);

            var Colour = GetBrushColour(shockwaveType);
            for (int counter = 0; counter < argbValues.Length; counter += 4)
            {
                argbValues[counter] = Colour.R;
                argbValues[counter + 1] = Colour.G;
                argbValues[counter + 2] = Colour.B;
            }


            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        private SettingsFile _colorFile = new SettingsFile("ColorSettings", new (string, string)[] { ("Mordemoth", "#274e13"), ("Soo-Won", "#0b5394"), ("Obliterator", "#674ea7")});
        private Color GetBrushColour(int shockwaveType)
        {
            string hex = "";
            switch (shockwaveType)
            {
                case 0:
                    hex = _colorFile.GetSetting("Mordemoth");
                    break;
                case 1:
                    hex = _colorFile.GetSetting("Soo-Won");
                    break;
                case 2:
                    hex = _colorFile.GetSetting("Obliterator");
                    break;
                default:
                    hex = "#000000";
                    break;
            }
            ColorConverter cc = new ColorConverter();
            return (Color)(cc.ConvertFromString(hex) ?? Color.Black);
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

        public Image? GetIcon(string iconName)
        {
            string path = $"icons/{iconName.ToLower()}.png";
            if (File.Exists(path))
            {
                return Image.FromFile(path);
            }
            return BlankImage;
        }
    }
}
