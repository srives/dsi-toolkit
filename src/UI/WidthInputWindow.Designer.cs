namespace DSI.UI
{
    partial class WidthInputWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FeetRadioBtn = new System.Windows.Forms.RadioButton();
            this.InchesRadioBtn = new System.Windows.Forms.RadioButton();
            this.InputBox = new System.Windows.Forms.TextBox();
            this.InputBoxLabel = new System.Windows.Forms.Label();
            this.OkBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FeetRadioBtn
            // 
            this.FeetRadioBtn.AutoSize = true;
            this.FeetRadioBtn.Location = new System.Drawing.Point(78, 73);
            this.FeetRadioBtn.Name = "FeetRadioBtn";
            this.FeetRadioBtn.Size = new System.Drawing.Size(46, 17);
            this.FeetRadioBtn.TabIndex = 0;
            this.FeetRadioBtn.Text = "Feet";
            this.FeetRadioBtn.UseVisualStyleBackColor = true;
            // 
            // InchesRadioBtn
            // 
            this.InchesRadioBtn.AutoSize = true;
            this.InchesRadioBtn.Checked = true;
            this.InchesRadioBtn.Location = new System.Drawing.Point(15, 73);
            this.InchesRadioBtn.Name = "InchesRadioBtn";
            this.InchesRadioBtn.Size = new System.Drawing.Size(57, 17);
            this.InchesRadioBtn.TabIndex = 1;
            this.InchesRadioBtn.TabStop = true;
            this.InchesRadioBtn.Text = "Inches";
            this.InchesRadioBtn.UseVisualStyleBackColor = true;
            // 
            // InputBox
            // 
            this.InputBox.Location = new System.Drawing.Point(15, 47);
            this.InputBox.Name = "InputBox";
            this.InputBox.Size = new System.Drawing.Size(199, 20);
            this.InputBox.TabIndex = 2;
            this.InputBox.TextChanged += new System.EventHandler(this.InputBox_TextChanged);
            // 
            // InputBoxLabel
            // 
            this.InputBoxLabel.AutoSize = true;
            this.InputBoxLabel.Location = new System.Drawing.Point(12, 24);
            this.InputBoxLabel.Name = "InputBoxLabel";
            this.InputBoxLabel.Size = new System.Drawing.Size(130, 13);
            this.InputBoxLabel.TabIndex = 3;
            this.InputBoxLabel.Text = "Enter New Unistrut Width:";
            this.InputBoxLabel.Click += new System.EventHandler(this.InputBoxLabel_Click);
            // 
            // OkBtn
            // 
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Enabled = false;
            this.OkBtn.Location = new System.Drawing.Point(246, 38);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(75, 23);
            this.OkBtn.TabIndex = 1;
            this.OkBtn.Text = "OK";
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(246, 67);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 5;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // WidthInputWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 105);
            this.ControlBox = false;
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OkBtn);
            this.Controls.Add(this.InputBoxLabel);
            this.Controls.Add(this.InputBox);
            this.Controls.Add(this.InchesRadioBtn);
            this.Controls.Add(this.FeetRadioBtn);
            this.Name = "WidthInputWindow";
            this.Text = "Change Unistrut Width";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton FeetRadioBtn;
        private System.Windows.Forms.RadioButton InchesRadioBtn;
        private System.Windows.Forms.TextBox InputBox;
        private System.Windows.Forms.Label InputBoxLabel;
        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button CancelBtn;
    }
}