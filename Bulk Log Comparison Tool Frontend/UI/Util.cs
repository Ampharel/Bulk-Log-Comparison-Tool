using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool_Frontend.UI
{
    internal static class UIUtil
    {
        public static Image StitchImages(this Image image1, Image image2)
        {
            if(image1 == null)
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


    }

}
