using Bulk_Log_Comparison_Tool_Frontend.UI;

namespace Bulk_Log_Comparison_Tool_Frontend.Utils
{
    internal static class UIUtil
    {
        public static Image StitchImages(this Image image1, Image image2)
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

        public static void UpdateTableFont(this DataGridView table)
        {
            table.DefaultCellStyle.Font = IPanel.columnFont;
            table.ColumnHeadersDefaultCellStyle.Font = IPanel.columnFont;
            table.RowHeadersDefaultCellStyle.Font = IPanel.columnFont;
        }
        public static void ClearTable(this DataGridView table)
        {
            for(int i = table.Rows.Count - 1; i >= 0; i--)
            {
                for(int j = table.Columns.Count - 1; j >= 0; j--)
                {
                    if (table.Rows[i].Cells[j].Value is Image image)
                    {
                        //table.Rows[i].Cells[j].Value = null;
                        image.Dispose();
                    }
                    else if (table.Rows[i].Cells[j].Value is string)
                    {
                        table.Rows[i].Cells[j].Value = "";
                    }
                }
            }
            table.Columns.Clear();
        }
    }

}
