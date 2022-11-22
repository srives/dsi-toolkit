using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using Xl = Microsoft.Office.Interop.Excel;

namespace DSI.Core
{
    public class ExcelWriter
    {
        private readonly ApplicationLog log;
        public bool WriteCSVFile { get; set; } = false;
        public Xl.Application Excel { get; } = null;
        public Xl.Workbook Workbook { get; } = null;
        /// <summary>
        /// The path to the Excel template.
        /// </summary>
        public string TemplatePath { get; }
        public System.Text.StringBuilder CsvFileContents { get; }
        /// <summary>
        /// The path to write the Excel workbook to.
        /// </summary>
        public string WritePath { get; }

        /// <summary>
        /// The ExcelWriter constructor. NON-CSV version
        /// This will open a SaveFIleDialog to prompt the user where they want to save
        /// the Excel workbook.
        /// </summary>
        /// <param name="templatePath">The path to the Excel template to write to.</param>
        /// <param name="defaultFileName">The file name to suggest to the user when the SaveFileDialog opens.</param>
        /// <param name="commandLog">The ApplicationLog passed into the writer to enable logging.</param>
        public ExcelWriter(string templatePath, string defaultFileName, ApplicationLog commandLog)
        {
            WriteCSVFile = false;
            log = commandLog;
            TemplatePath = templatePath;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog {
                FileName = defaultFileName,
                Filter = Properties.Resources.EXCEL_INFO_FILTER,
                FilterIndex = 1 })
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

            var (success, message) = CopyFileToLocationWithOverwrite(TemplatePath, WritePath, WriteCSVFile);

            if (success == false)
            {
                MessageBox.Show("ERROR: " + message, $"Create {WritePath}"); 
            }

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
        /// The ExcelWriter constructor. CSV Version
        /// This will open a SaveFIleDialog to prompt the user where they want to save
        /// the Excel workbook.
        /// </summary>
        // /// <param name="templatePath">The path to the Excel template to write to.</param>
        /// <param name="columnHeaders">The header names to the files.</param>
        /// <param name="defaultFileName">The file name to suggest to the user when the SaveFileDialog opens.</param>
        /// <param name="commandLog">The ApplicationLog passed into the writer to enable logging.</param>
        public ExcelWriter(string[,] columnHeaders, string defaultFileName, ApplicationLog commandLog)
        {
            WriteCSVFile = true;
            log = commandLog;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog {
                FileName = defaultFileName,
                Filter = Properties.Resources.EXCEL_INFO_FILTER,
                FilterIndex = 1 })
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
            CsvFileContents = new System.Text.StringBuilder(); // we will flush this on close()
            WriteRangeCSV(columnHeaders);
        }

        /// <summary>
        /// Saves the CSV file, or the excel workbook (closing the running excel instance)
        /// </summary>
        public void Close()
        {
            if (WriteCSVFile)
            {
                File.WriteAllText(WritePath, CsvFileContents.ToString());
                return;
            }
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
        /// Bulk writes a range of data to an excel spreadsheet.
        /// </summary>
        /// <typeparam name="T">The type of data in the range.</typeparam>
        /// <param name="data">The range of data.</param>
        /// <remarks>Excel indexes are 1-based, not 0-based.</remarks>
        public void WriteRangeCSV<T>(
            T[,] data, bool escapeQuotes = true)
        {
            int columns = 0;
            int rows = 0;
            if (data == null || data.Length == 0)
            {
                MessageBox.Show("No data to write.", "Empty data.");
            }
            else
            {
                rows = data.GetLength(0);
                columns = data.GetLength(1);
                if (escapeQuotes)
                {
                    for (int row = 0; row < rows; ++row)
                    {
                        string line = string.Empty;
                        for (int column = 0; column < columns; ++column)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                line += ",";
                            }
                            string escQuotes = string.Empty;
                            if (data[row, column] != null)
                            {
                                string value = data[row, column].ToString();
                                escQuotes = value.Replace("\"", "\"\""); // replace " with ""
                                if (value.Contains(","))
                                {
                                    escQuotes = $"\"{escQuotes}\""; // If has comma, put quotes around it
                                }
                            }
                            line += $"{escQuotes}";
                        }
                        // string line = string.Join(",", Enumerable.Range(0, columns).Select(col => data[row, col]));
                        CsvFileContents.AppendLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// A more explicit redefinition of the File.Copy(string, string, bool) method.
        /// </summary>
        /// <param name="from">The source file path.</param>
        /// <param name="to">The path to the destination.</param>
        private (bool, string) CopyFileToLocationWithOverwrite(string from, string to, bool csv)
        {
            bool success = true;
            string errorMessage = string.Empty;

            if (from == null)
            {
                return (false, "File name source is empty.");
            }

            if (to == null)
            {
                return (false, "File save destination is empty.");
            }

            try
            {
                File.Copy(from, to, true);
            }
            catch (UnauthorizedAccessException)
            {
                success = false;
                errorMessage = $"The user does not permission to write to {to}";
                log.Logger.Error(errorMessage);
                Close();
            }
            catch (DirectoryNotFoundException e)
            {
                success = false;
                errorMessage = "The specified directory was not found";
                log.Logger.Error(e, errorMessage);
                Close();
            }
            catch (FileNotFoundException e)
            {
                success = false;
                errorMessage = "The specified file was not found";
                log.Logger.Error(e, errorMessage);
                Close();
            }
            catch (Exception e)
            {
                success = false;
                errorMessage = e.Message;
                log.Logger.Error(e, "an exception occured");
                Close();
            }

            return (success, errorMessage);
        }
    }
}
