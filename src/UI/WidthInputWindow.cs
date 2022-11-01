using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Form = System.Windows.Forms.Form;

namespace DSI.UI
{
    [Transaction(TransactionMode.Manual)]
    public partial class WidthInputWindow : Form
    {
        public WidthInputWindow(ElementArray elements)
        {
            Elements = elements;
            IsInputNumeric = false;
            InitializeComponent();
        }

        public ElementArray Elements { get; set; }
        public bool IsInputNumeric { get; set; }
        public string UserInput { get; set; }
        public double UserInputAsDouble { get; set; }

        private void InputBoxLabel_Click(object sender, EventArgs e)
        {

        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (FeetRadioBtn.Checked)
            {
                UserInputAsDouble *= 12;
            }
        }

        private void InputBox_TextChanged(object sender, EventArgs e)
        {
            UserInput = InputBox.Text;
            if (double.TryParse(UserInput, out double num))
            {
                IsInputNumeric = true;
                OkBtn.Enabled = true;
                UserInputAsDouble = num;
            }
            else
            {
                IsInputNumeric = false;
                OkBtn.Enabled = false;
            }
        }
    }
}
