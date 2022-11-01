using System;
using System.IO;
using System.Windows.Forms;
using Xl = Microsoft.Office.Interop.Excel;

namespace DSI.Core
{
    public class ExcelWriter
    {
        /// <summary>
        /// Provides access to the application logging interface.
        /// </summary>
        private readonly ApplicationLog log;


        /// <summary>
        /// The ExcelWriter constructor. This will open a SaveFIleDialog to prompt the user where they want to save
        /// the Excel workbook.
        /// </summary>
        /// <param name="templatePath">The path to the Excel template to write to.</param>
        /// <param name="defaultFileName">The file name to suggest to the user when the SaveFileDialog opens.</param>
        /// <param name="commandLog">The ApplicationLog passed into the writer to enable logging.</param>
        public ExcelWriter(string templatePath, string defaultFileName, ApplicationLog commandLog)
        {
            log = commandLog;
            TemplatePath = templatePath;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = defaultFileName,
                Filter = Properties.Resources.EXCEL_INFO_FILTER,
                FilterIndex = 1
            })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    WritePath = saveFileDialog.FileName;
                }
                else
                {
                    throw new OperationCanceledException();
                }
            }

            Excel = new Xl.Application
            {
                DisplayAlerts = false
            };

            CopyFileToLocationWithOverwrite(TemplatePath, WritePath);

            try
            {
                Workbook = Excel.Workbooks._Open(WritePath);
            }
            catch
            {
                Close();
            }
        }


        /// <summary>
        /// The open Excel instance.
        /// </summary>
        public Xl.Application Excel { get; } = null;


        /// <summary>
        /// The open Excel workbook instance.
        /// </summary>
        public Xl.Workbook Workbook { get; } = null;


        /// <summary>
        /// The path to the Excel template.
        /// </summary>
        public string TemplatePath { get; }


        /// <summary>
        /// The path to write the Excel workbook to.
        /// </summary>
        public string WritePath { get; }


        /// <summary>
        /// Saves the workbook and closes the running excel instance.
        /// </summary>
        public void Close()
        {
            try
            {
                if (Workbook != null)
                {
                    Workbook.SaveAs(WritePath);
                }
            }
            finally
            {
                if (Excel != null)
                {
                    Excel.Quit();
                }
            }
        }


        /// <summary>
        /// Bulk writes a range of data to an excel spreadsheet.
        /// </summary>
        /// <typeparam name="T">The type of data in the range.</typeparam>
        /// <param name="data">The range of data.</param>
        /// <param name="rows">The total number of rows to write.</param>
        /// <param name="columns">The total number of columns to write.</param>
        /// <param name="startRow">The initial row to start writing to.</param>
        /// <param name="startColumn">The intial column to start writing to.</param>
        /// <param name="worksheetIndex">The index of the worksheet to write to.</param>
        /// <remarks>Excel indexes are 1-based, not 0-based.</remarks>
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


        /// <summary>
        /// A more explicit redefinition of the File.Copy(string, string, bool) method.
        /// </summary>
        /// <param name="from">The source file path.</param>
        /// <param name="to">The path to the destination.</param>
        private void CopyFileToLocationWithOverwrite(string from, string to)
        {
            if (from == null)
            {
                throw new ArgumentNullException(paramName: nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(paramName: nameof(to));
            }

            try
            {
                File.Copy(from, to, true);
            }
            catch (UnauthorizedAccessException)
            {
                log.Logger.Error($"the user does not permission to write to {to}");
                Close();
            }
            catch (DirectoryNotFoundException e)
            {
                log.Logger.Error(e, "the specified directory was not found");
                Close();
            }
            catch (FileNotFoundException e)
            {
                log.Logger.Error(e, "the specified file was not found");
                Close();
            }
            catch (Exception e)
            {
                log.Logger.Error(e, "an exception occured");
                Close();
            }
        }
    }
}
