namespace DSI.UI
{
    partial class SleveNumberingWindow
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
            this.RectangularSleeveStartLabel = new System.Windows.Forms.Label();
            this.RoundSleeveStartLabel = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.OverrideExistingNumbersCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // OkBtn
            // 
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Location = new System.Drawing.Point(334, 89);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(75, 23);
            this.OkBtn.TabIndex = 0;
            this.OkBtn.Text = "OK";
            this.OkBtn.UseVisualStyleBackColor = true;
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(334, 118);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 1;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // RectangularSleeveStartLabel
            // 
            this.RectangularSleeveStartLabel.AutoSize = true;
            this.RectangularSleeveStartLabel.Location = new System.Drawing.Point(21, 12);
            this.RectangularSleeveStartLabel.Name = "RectangularSleeveStartLabel";
            this.RectangularSleeveStartLabel.Size = new System.Drawing.Size(183, 13);
            this.RectangularSleeveStartLabel.TabIndex = 3;
            this.RectangularSleeveStartLabel.Text = "Rectangular Sleeve Starting Number:";
            // 
            // RoundSleeveStartLabel
            // 
            this.RoundSleeveStartLabel.AutoSize = true;
            this.RoundSleeveStartLabel.Location = new System.Drawing.Point(21, 72);
            this.RoundSleeveStartLabel.Name = "RoundSleeveStartLabel";
            this.RoundSleeveStartLabel.Size = new System.Drawing.Size(157, 13);
            this.RoundSleeveStartLabel.TabIndex = 4;
            this.RoundSleeveStartLabel.Text = "Round Sleeve Starting Number:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(21, 29);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(292, 20);
            this.textBox2.TabIndex = 7;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(21, 89);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(292, 20);
            this.textBox3.TabIndex = 8;
            // 
            // OverrideExistingNumbersCheck
            // 
            this.OverrideExistingNumbersCheck.AutoSize = true;
            this.OverrideExistingNumbersCheck.Location = new System.Drawing.Point(154, 122);
            this.OverrideExistingNumbersCheck.Name = "OverrideExistingNumbersCheck";
            this.OverrideExistingNumbersCheck.Size = new System.Drawing.Size(159, 17);
            this.OverrideExistingNumbersCheck.TabIndex = 9;
            this.OverrideExistingNumbersCheck.Text = "Override Existing Numbering";
            this.OverrideExistingNumbersCheck.UseVisualStyleBackColor = true;
            // 
            // SleveNumberingWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 171);
            this.ControlBox = false;
            this.Controls.Add(this.OverrideExistingNumbersCheck);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.RoundSleeveStartLabel);
            this.Controls.Add(this.RectangularSleeveStartLabel);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OkBtn);
            this.Name = "SleveNumberingWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sleeve Numbering";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Label RectangularSleeveStartLabel;
        private System.Windows.Forms.Label RoundSleeveStartLabel;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.CheckBox OverrideExistingNumbersCheck;
    }
}