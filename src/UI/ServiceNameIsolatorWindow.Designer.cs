namespace DSI.UI
{
    partial class ServiceNameIsolatorWindow
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
            this.OkBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.ServiceNames = new System.Windows.Forms.CheckedListBox();
            this.IsolateElemBtn = new System.Windows.Forms.RadioButton();
            this.SelectElemBtn = new System.Windows.Forms.RadioButton();
            this.IsolateAndSelectBtn = new System.Windows.Forms.RadioButton();
            this.ElementsLabel = new System.Windows.Forms.Label();
            this.TotalNumOfElementsLabel = new System.Windows.Forms.Label();
            this.SelectedLabel = new System.Windows.Forms.Label();
            this.TotalNumOfSelectedElementsLabel = new System.Windows.Forms.Label();
            this.CheckAllBtn = new System.Windows.Forms.Button();
            this.UncheckAllBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OkBtn
            // 
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Location = new System.Drawing.Point(586, 285);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(90, 23);
            this.OkBtn.TabIndex = 0;
            this.OkBtn.Text = "OK";
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(586, 314);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(90, 23);
            this.CancelBtn.TabIndex = 1;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // ServiceNames
            // 
            this.ServiceNames.CheckOnClick = true;
            this.ServiceNames.FormattingEnabled = true;
            this.ServiceNames.Location = new System.Drawing.Point(26, 33);
            this.ServiceNames.Margin = new System.Windows.Forms.Padding(5);
            this.ServiceNames.Name = "ServiceNames";
            this.ServiceNames.Size = new System.Drawing.Size(538, 304);
            this.ServiceNames.TabIndex = 2;
            this.ServiceNames.SelectedIndexChanged += new System.EventHandler(this.ServiceNames_SelectedIndexChanged);
            // 
            // IsolateElemBtn
            // 
            this.IsolateElemBtn.AutoSize = true;
            this.IsolateElemBtn.Checked = true;
            this.IsolateElemBtn.Location = new System.Drawing.Point(26, 352);
            this.IsolateElemBtn.Name = "IsolateElemBtn";
            this.IsolateElemBtn.Size = new System.Drawing.Size(116, 19);
            this.IsolateElemBtn.TabIndex = 3;
            this.IsolateElemBtn.TabStop = true;
            this.IsolateElemBtn.Text = "Isolate Elements";
            this.IsolateElemBtn.UseVisualStyleBackColor = true;
            // 
            // SelectElemBtn
            // 
            this.SelectElemBtn.AutoSize = true;
            this.SelectElemBtn.Location = new System.Drawing.Point(147, 352);
            this.SelectElemBtn.Name = "SelectElemBtn";
            this.SelectElemBtn.Size = new System.Drawing.Size(114, 19);
            this.SelectElemBtn.TabIndex = 4;
            this.SelectElemBtn.Text = "Select Elements";
            this.SelectElemBtn.UseVisualStyleBackColor = true;
            // 
            // IsolateAndSelectBtn
            // 
            this.IsolateAndSelectBtn.AutoSize = true;
            this.IsolateAndSelectBtn.Location = new System.Drawing.Point(267, 352);
            this.IsolateAndSelectBtn.Name = "IsolateAndSelectBtn";
            this.IsolateAndSelectBtn.Size = new System.Drawing.Size(177, 19);
            this.IsolateAndSelectBtn.TabIndex = 5;
            this.IsolateAndSelectBtn.TabStop = true;
            this.IsolateAndSelectBtn.Text = "Isolate and Select Elements";
            this.IsolateAndSelectBtn.UseVisualStyleBackColor = true;
            // 
            // ElementsLabel
            // 
            this.ElementsLabel.AutoSize = true;
            this.ElementsLabel.Location = new System.Drawing.Point(23, 15);
            this.ElementsLabel.Name = "ElementsLabel";
            this.ElementsLabel.Size = new System.Drawing.Size(65, 15);
            this.ElementsLabel.TabIndex = 6;
            this.ElementsLabel.Text = "Elements: ";
            // 
            // TotalNumOfElementsLabel
            // 
            this.TotalNumOfElementsLabel.AutoSize = true;
            this.TotalNumOfElementsLabel.Location = new System.Drawing.Point(93, 15);
            this.TotalNumOfElementsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TotalNumOfElementsLabel.Name = "TotalNumOfElementsLabel";
            this.TotalNumOfElementsLabel.Size = new System.Drawing.Size(14, 15);
            this.TotalNumOfElementsLabel.TabIndex = 7;
            this.TotalNumOfElementsLabel.Text = "0";
            // 
            // SelectedLabel
            // 
            this.SelectedLabel.AutoSize = true;
            this.SelectedLabel.Location = new System.Drawing.Point(189, 15);
            this.SelectedLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SelectedLabel.Name = "SelectedLabel";
            this.SelectedLabel.Size = new System.Drawing.Size(61, 15);
            this.SelectedLabel.TabIndex = 8;
            this.SelectedLabel.Text = "Selected: ";
            // 
            // TotalNumOfSelectedElementsLabel
            // 
            this.TotalNumOfSelectedElementsLabel.AutoSize = true;
            this.TotalNumOfSelectedElementsLabel.Location = new System.Drawing.Point(254, 15);
            this.TotalNumOfSelectedElementsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TotalNumOfSelectedElementsLabel.Name = "TotalNumOfSelectedElementsLabel";
            this.TotalNumOfSelectedElementsLabel.Size = new System.Drawing.Size(14, 15);
            this.TotalNumOfSelectedElementsLabel.TabIndex = 9;
            this.TotalNumOfSelectedElementsLabel.Text = "0";
            // 
            // CheckAllBtn
            // 
            this.CheckAllBtn.Location = new System.Drawing.Point(583, 33);
            this.CheckAllBtn.Name = "CheckAllBtn";
            this.CheckAllBtn.Size = new System.Drawing.Size(90, 23);
            this.CheckAllBtn.TabIndex = 12;
            this.CheckAllBtn.Text = "Check All";
            this.CheckAllBtn.UseVisualStyleBackColor = true;
            this.CheckAllBtn.Click += new System.EventHandler(this.CheckAllBtn_Click);
            // 
            // UncheckAllBtn
            // 
            this.UncheckAllBtn.Location = new System.Drawing.Point(583, 62);
            this.UncheckAllBtn.Name = "UncheckAllBtn";
            this.UncheckAllBtn.Size = new System.Drawing.Size(90, 23);
            this.UncheckAllBtn.TabIndex = 13;
            this.UncheckAllBtn.Text = "Uncheck All";
            this.UncheckAllBtn.UseVisualStyleBackColor = true;
            this.UncheckAllBtn.Click += new System.EventHandler(this.UncheckAllBtn_Click);
            // 
            // ServiceNameIsolatorWindow
            // 
            this.AcceptButton = this.OkBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(699, 395);
            this.ControlBox = false;
            this.Controls.Add(this.UncheckAllBtn);
            this.Controls.Add(this.CheckAllBtn);
            this.Controls.Add(this.TotalNumOfSelectedElementsLabel);
            this.Controls.Add(this.SelectedLabel);
            this.Controls.Add(this.TotalNumOfElementsLabel);
            this.Controls.Add(this.ElementsLabel);
            this.Controls.Add(this.IsolateAndSelectBtn);
            this.Controls.Add(this.SelectElemBtn);
            this.Controls.Add(this.IsolateElemBtn);
            this.Controls.Add(this.ServiceNames);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OkBtn);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "ServiceNameIsolatorWindow";
            this.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.Text = "Service Name Filters";
            this.Load += new System.EventHandler(this.ServiceNameIsolatorWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.CheckedListBox ServiceNames;
        private System.Windows.Forms.RadioButton IsolateElemBtn;
        private System.Windows.Forms.RadioButton SelectElemBtn;
        private System.Windows.Forms.RadioButton IsolateAndSelectBtn;
        private System.Windows.Forms.Label ElementsLabel;
        private System.Windows.Forms.Label TotalNumOfElementsLabel;
        private System.Windows.Forms.Label SelectedLabel;
        private System.Windows.Forms.Label TotalNumOfSelectedElementsLabel;
        private System.Windows.Forms.Button CheckAllBtn;
        private System.Windows.Forms.Button UncheckAllBtn;
    }
}