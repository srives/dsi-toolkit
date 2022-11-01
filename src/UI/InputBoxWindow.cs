using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSI.UI
{
    public partial class InputBoxWindow : Form
    {
        public InputBoxWindow(string windowName, string inputLabelText)
        {
            WindowName = windowName;
            InputLabelText = inputLabelText;
            InitializeComponent();
        }


        public string WindowName { get; set; }
        public string InputLabelText { get; set; }
        public string UserInput { get; set; }


        private void InputBoxWindow_Load(object sender, EventArgs e)
        {
            Text = WindowName;
            InputLabel.Text = InputLabelText;
        }

        private void InputBox_TextChanged(object sender, EventArgs e)
        {
            UserInput = InputBox.Text;
        }
    }
}
