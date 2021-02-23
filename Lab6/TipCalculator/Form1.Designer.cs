
namespace TipCalculator
{
    partial class Form1
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
            this.EnterAmmountTextBox = new System.Windows.Forms.TextBox();
            this.TheCalculatedTipBox = new System.Windows.Forms.TextBox();
            this.TotalBillLabel = new System.Windows.Forms.Label();
            this.CustomTip = new System.Windows.Forms.Label();
            this.EnterTipTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // EnterAmmountTextBox
            // 
            this.EnterAmmountTextBox.Location = new System.Drawing.Point(402, 79);
            this.EnterAmmountTextBox.Name = "EnterAmmountTextBox";
            this.EnterAmmountTextBox.Size = new System.Drawing.Size(294, 22);
            this.EnterAmmountTextBox.TabIndex = 0;
            this.EnterAmmountTextBox.TextChanged += new System.EventHandler(this.EnterAmmountTextBox_TextChanged);
            // 
            // TheCalculatedTipBox
            // 
            this.TheCalculatedTipBox.Location = new System.Drawing.Point(402, 187);
            this.TheCalculatedTipBox.Name = "TheCalculatedTipBox";
            this.TheCalculatedTipBox.ReadOnly = true;
            this.TheCalculatedTipBox.Size = new System.Drawing.Size(294, 22);
            this.TheCalculatedTipBox.TabIndex = 1;
            this.TheCalculatedTipBox.TextChanged += new System.EventHandler(this.TheCalculatedTipBox_TextChanged);
            // 
            // TotalBillLabel
            // 
            this.TotalBillLabel.AutoSize = true;
            this.TotalBillLabel.Location = new System.Drawing.Point(257, 79);
            this.TotalBillLabel.Name = "TotalBillLabel";
            this.TotalBillLabel.Size = new System.Drawing.Size(100, 17);
            this.TotalBillLabel.TabIndex = 2;
            this.TotalBillLabel.Text = "Enter Total Bill";
            // 
            // CustomTip
            // 
            this.CustomTip.AutoSize = true;
            this.CustomTip.Location = new System.Drawing.Point(240, 132);
            this.CustomTip.Name = "CustomTip";
            this.CustomTip.Size = new System.Drawing.Size(143, 17);
            this.CustomTip.TabIndex = 4;
            this.CustomTip.Text = "Enter Tip Percentage";
            // 
            // EnterTipTextBox
            // 
            this.EnterTipTextBox.Location = new System.Drawing.Point(402, 132);
            this.EnterTipTextBox.Name = "EnterTipTextBox";
            this.EnterTipTextBox.Size = new System.Drawing.Size(294, 22);
            this.EnterTipTextBox.TabIndex = 5;
            this.EnterTipTextBox.TextChanged += new System.EventHandler(this.EnterTipTextBox_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.EnterTipTextBox);
            this.Controls.Add(this.CustomTip);
            this.Controls.Add(this.TotalBillLabel);
            this.Controls.Add(this.TheCalculatedTipBox);
            this.Controls.Add(this.EnterAmmountTextBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox EnterAmmountTextBox;
        private System.Windows.Forms.TextBox TheCalculatedTipBox;
        private System.Windows.Forms.Label TotalBillLabel;
        private System.Windows.Forms.Label CustomTip;
        private System.Windows.Forms.TextBox EnterTipTextBox;
    }
}

