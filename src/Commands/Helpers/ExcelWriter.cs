using System;
using Xl = Microsoft.Office.Interop.Excel;

namespace DSI.Commands.Helpers
{
    public class ExcelWriter
    {
        public string WritePath { get; set; }
        public Xl.Application Excel { get; set; }
        public Xl.Workbook Workbook { get; set; }


        public ExcelWriter(string writePath)
        {
            WritePath = writePath;
            Excel = new Xl.Application
            {
                DisplayAlerts = false
            };

            Workbook = Excel.Workbooks._Open(WritePath);
        }


        public void WriteRange<T>(
            T[,] data,
            int rows,
            int columns,
            int startRow,
            int startColumn,
            int worksheetIndex)
        {
            try
            {
                Xl.Worksheet worksheet = Workbook.Worksheets[worksheetIndex];
                Xl.Range startCell = worksheet.Cells[startRow, startColumn];
                Xl.Range endCell = worksheet.Cells[rows + (startRow - 1), columns + (startColumn - 1)];
                Xl.Range writeRange = worksheet.Range[startCell, endCell];

                writeRange.Value2 = data;
            }
            catch (Exception)
            {
                Close();
            }
        }


        public void Close()
        {
            try
            {
                Workbook.SaveAs(WritePath);
            }
            finally
            {
                if (Excel != null)
                {
                    Excel.Quit();
                }
            }
        }
    }
}
