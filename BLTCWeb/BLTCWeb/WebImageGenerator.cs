using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.Enums;
using Bulk_Log_Comparison_Tool.Util;
using System.Drawing;

namespace BLTCWeb
{
    internal class WebImageGenerator
    {

        public byte[] GetImageBytes(IParsedEvtcLog Log, string Player, List<(long, int)> shockwaves)
        {
            return GetImage(Log, Player, null, shockwaves)?.ToBytes() ?? [];
        }

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
                argbValues[counter] = Colour.B;
                argbValues[counter + 1] = Colour.G;
                argbValues[counter + 2] = Colour.R;
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
            return Color.FromArgb(int.Parse(hex.Replace("#",null), System.Globalization.NumberStyles.HexNumber));
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
    public static class ImageExtensions
    {
        public static Image StitchImages(this Image? image1, Image image2)
        {
            if (image1 == null)
            {
                return image2;
            }
            var newImage = new Bitmap(image1.Width + image2.Width, image1.Height);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image1, 0, 0);
                g.DrawImage(image2, image1.Width, 0);
            }
            return newImage;
        }
        public static byte[] ToBytes(this Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
