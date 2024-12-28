using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.Enums;
using Bulk_Log_Comparison_Tool.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BLCTWeb
{
    internal class WebImageGenerator
    {

        public byte[] GetImageBytes(IParsedEvtcLog Log, string Player, List<(long, int)> shockwaves)
        {
            var images = GetImage(Log, Player, null, shockwaves);
            if(images.Count == 0)
            {
                return [];
            }
            using (Image<Rgba32> outputImage = new Image<Rgba32>(images.Sum(x => x.Width), images.First().Height))
            {
                var offset = 0;
                foreach(var img in images)
                {
                    outputImage.Mutate(o => o.DrawImage(img, new Point(offset, 0), 1f));
                    offset += img.Width;
                }
                return outputImage.BytesFromImage();
            }
        }


        private List<Image> GetImage(IParsedEvtcLog Log, string Player, Image? image, List<(long, int)> shockwaves)
        {
            var images = new List<Image>();
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
                    images.Add(GetImage(shockwave.Item2,"death"));
                }
                else if (hadStab && wasHit)
                {
                    images.Add(GetImage(shockwave.Item2,"shield"));
                }
                else if (hadStab)
                {
                    images.Add(GetImage(shockwave.Item2,"jumped"));
                }
                else if (wasHit)
                {
                    images.Add(GetImage(shockwave.Item2,"down"));
                }
                else
                {
                    images.Add(GetImage(shockwave.Item2,"warning"));
                }
            }

            return images;
        }

        enum StabStatus
        {
            Dead,
            Stability,
            None
        };

        private const string fontName = "Segoe UI Symbol";


        public Image GetImage(int shockwaveType, string name)
        {
            Image img = GetIcon(name);
            img.Mutate(x => x.Filter(GetColorMatrix(shockwaveType)));
            return img;
        }

        private SettingsFile _colorFile = new SettingsFile("ColorSettings.txt", new (string, string)[] { ("Mordemoth", "#2E8B57"), ("Soo-Won", "#89CFF0"), ("Obliterator", "#A45EE9")});
        
        private ColorMatrix GetColorMatrix(int shockwaveType)
        {
            var hex = GetHexForType(shockwaveType);
            var r = byte.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            var g = byte.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            var b = byte.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            var cm = new ColorMatrix(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1,
                r, g, b, 0
                );
            return cm;
        }

        private string GetHexForType(int shockwaveType)
        {
            switch (shockwaveType)
            {
                case 0:
                    return _colorFile.GetSetting("Mordemoth");
                case 1:
                    return _colorFile.GetSetting("Soo-Won");
                case 2:
                    return _colorFile.GetSetting("Obliterator");
            }
            return "#000000";
        }
        
        private float GetHueAngle(int shockwaveType)
        {
            string hex = "";
            switch (shockwaveType)
            {
                case 0:
                    return 150f;
                case 1:
                    return 240f;
                case 2:
                    return 60f;
            }
            return 0f;
        }

        private readonly Dictionary<string, Image> _specIcons = new();

        public Image? GetIcon(string iconName)
        {
            string path = $"wwwroot/icons/{iconName.ToLower()}.png";
            if (File.Exists(path))
            {
                return Image.Load(path);
            }
            return null;
        }
    }

    static class ImageExtensions
    {
        public static byte[] BytesFromImage(this Image img)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, new PngEncoder());
                return ms.ToArray();
            }
        }
    }
}
