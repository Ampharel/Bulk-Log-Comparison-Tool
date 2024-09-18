using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool_Frontend.Compare
{
    public static class CustomSortComparer
    {
        public static void CustomSortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            string aString = e.CellValue1.ToString() ?? "";
            string bString = e.CellValue2.ToString() ?? "";

            aString = aString.Replace("k", "");
            bString = bString.Replace("k", "");
            var bParsed = double.TryParse(aString, out double b);
            var aParsed = double.TryParse(bString, out double a);

            if(e.CellValue1 == null)
            {
                e.SortResult = -1;
                e.Handled = true;
                return;
            }
            if(e.CellValue2 == null)
            {
                e.SortResult = 1;
                e.Handled = true;
                return;
            }

            if (!bParsed && !aParsed)
            {
                e.SortResult = e.CellValue1.ToString().CompareTo(e.CellValue2.ToString());
                e.Handled = true;
                return;
            }

            if (!bParsed)
            {
                e.SortResult = 1;
                e.Handled = true;
                return;
            }
            if (!aParsed)
            {
                e.SortResult = -1;
                e.Handled = true;
                return;
            }
            e.SortResult = a.CompareTo(b);
            e.Handled = true;
        }
    }
}
