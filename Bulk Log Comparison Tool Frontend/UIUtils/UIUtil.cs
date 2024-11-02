using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool_Frontend.Compare;
using Bulk_Log_Comparison_Tool_Frontend.UI;
using System.IO;

namespace Bulk_Log_Comparison_Tool_Frontend.Utils
{
    internal static class UIUtil
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


        public static void UpdatePlayersWithClassicons(this DataGridView table, List<IParsedEvtcLog> parsedEvtcLogs, string[] players)
        {
            List<int> classColumns = new();
            var imgGen = new ImageGenerator();
            var offset = 0;
            Dictionary<string, string> playerSpecs = new();
            for (int i = 0; i < parsedEvtcLogs.Count; i++)
            {
                var log = parsedEvtcLogs[i];
                bool columnAdded = false;
                for (int j = 0; j < players.Length; j++)
                {
                    var player = players[j];
                    if (log.HasPlayer(player))
                    {
                        var oldSpec = playerSpecs.GetValueOrDefault(player, "");
                        var newSpec = log.GetSpec(player);
                        playerSpecs[player] = newSpec;
                        if (!oldSpec.Equals(newSpec))
                        {
                            if (!columnAdded)
                            {
                                var column = new DataGridViewImageColumn();
                                column.Width = 22;
                                table.Columns.Insert(i + offset, column);
                                foreach (DataGridViewRow row in table.Rows)
                                {
                                    row.Cells[i + offset].Value = Image.FromFile(Path.Combine("icons", "blank.png"));
                                }
                                columnAdded = true;
                            }
                        }
                    }
                }
                if (columnAdded) 
                { 
                    for (int j = 0; j < players.Length; j++)
                    {
                        var player = players[j];
                        if (log.HasPlayer(player))
                        {
                            var newSpec = log.GetSpec(player);
                            var image = imgGen.GetIcon(newSpec);

                            if (image != null)
                            {
                                var imgCell = new DataGridViewImageCell();
                                imgCell.Value = image;
                                table.Rows[j].Cells[i + offset] = imgCell;
                            }
                        }
                    }
                }
                if(columnAdded)
                {
                    offset++;
                }
            }
        }
    }

}
