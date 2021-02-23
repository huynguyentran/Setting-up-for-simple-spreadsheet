using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TipCalculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        //private void ComputeTipButton_Click(object sender, EventArgs e)
        //{
        //    if (Double.TryParse(EnterAmmountTextBox.Text, out double val) && double.TryParse(EnterTipTextBox.Text, out double val2))
        //    {

        //        TheCalculatedTipBox.Text = "" + (val * val2) / 100;
        //    }
        //    else
        //    {
        //        TheCalculatedTipBox.Text = "error!";
        //    }
        //}

        private void TheCalculatedTipBox_TextChanged(object sender, EventArgs e)
        {
            if (Double.TryParse(EnterAmmountTextBox.Text, out double val) && double.TryParse(EnterTipTextBox.Text, out double val2))
            {

                TheCalculatedTipBox.Text = "" + (val * val2) / 100;
            }
            else
            {
                TheCalculatedTipBox.Text = "error!";
            }
        }

        private void EnterAmmountTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Double.TryParse(EnterAmmountTextBox.Text, out double val) && double.TryParse(EnterTipTextBox.Text, out double val2))
            {

                TheCalculatedTipBox.Text = "" + (val * val2) / 100;
            }
            else
            {
                TheCalculatedTipBox.Text = "error!";
            }
        }

        private void EnterTipTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Double.TryParse(EnterAmmountTextBox.Text, out double val) && double.TryParse(EnterTipTextBox.Text, out double val2))
            {

                TheCalculatedTipBox.Text = "" + (val * val2) / 100;
            }
            else
            {
                TheCalculatedTipBox.Text = "error!";
            }
        }
    }
}
