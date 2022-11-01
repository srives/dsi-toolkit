using System;
using System.IO;
using System.Windows.Forms;

namespace DSI.Commands.Helpers
{
    public class ExportInfo
    {
        public string ExactPath { get; set; }


        public ExportInfo(string fileName)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = fileName,
                Filter = Properties.Resources.EXCEL_INFO_FILTER,
                FilterIndex = 1
            })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExactPath = saveFileDialog.FileName;
                }
                else
                {
                    throw new OperationCanceledException();
                }
            }
        }
    }
}
