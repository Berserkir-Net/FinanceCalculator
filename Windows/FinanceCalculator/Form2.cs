using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private string QueryValue(string FinanceCode)
        {
            string ReturnValue = "0.000000";
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\BAL\\EFLCalc\\Rates");
            try
            {
                ReturnValue = "0.000000";
                ReturnValue = (string)rk.GetValue("rate_" + FinanceCode);
                if (ReturnValue == null)
                {
                    ReturnValue = "0.000000";
                }
            }
            catch
            {
                ReturnValue = "0.000000";
            }
            return ReturnValue;
        }

        private void SaveValue(string FinanceCode, string FinanceRate)
        {
            RegistryKey rk = Registry.CurrentUser.CreateSubKey("SOFTWARE\\BAL\\EFLCalc\\Rates");
            rk.SetValue("rate_" + FinanceCode, FinanceRate);
            return;
        }


        private void Form2_Load(object sender, EventArgs e)
        {
            txtGSTRate.Text = "0.00";
            txtGSTRate.Text = QueryValue("GST");

            txtrate_r_12_1.Text = QueryValue("r_12_1");
            txtrate_r_12_2.Text = QueryValue("r_12_2");
            txtrate_r_12_3.Text = QueryValue("r_12_3");
            txtrate_r_12_4.Text = QueryValue("r_12_4");
            txtrate_r_12_5.Text = QueryValue("r_12_5");

            txtrate_l_12_1.Text = QueryValue("l_12_1");
            txtrate_l_12_2.Text = QueryValue("l_12_2");
            txtrate_l_12_3.Text = QueryValue("l_12_3");
            txtrate_l_12_4.Text = QueryValue("l_12_4");
            txtrate_l_12_5.Text = QueryValue("l_12_5");

            txtrate_r_24_1.Text = QueryValue("r_24_1");
            txtrate_r_24_2.Text = QueryValue("r_24_2");
            txtrate_r_24_3.Text = QueryValue("r_24_3");
            txtrate_r_24_4.Text = QueryValue("r_24_4");
            txtrate_r_24_5.Text = QueryValue("r_24_5");

            txtrate_l_24_1.Text = QueryValue("l_24_1");
            txtrate_l_24_2.Text = QueryValue("l_24_2");
            txtrate_l_24_3.Text = QueryValue("l_24_3");
            txtrate_l_24_4.Text = QueryValue("l_24_4");
            txtrate_l_24_5.Text = QueryValue("l_24_5");

            txtrate_r_36_1.Text = QueryValue("r_36_1");
            txtrate_r_36_2.Text = QueryValue("r_36_2");
            txtrate_r_36_3.Text = QueryValue("r_36_3");
            txtrate_r_36_4.Text = QueryValue("r_36_4");
            txtrate_r_36_5.Text = QueryValue("r_36_5");

            txtrate_l_36_1.Text = QueryValue("l_36_1");
            txtrate_l_36_2.Text = QueryValue("l_36_2");
            txtrate_l_36_3.Text = QueryValue("l_36_3");
            txtrate_l_36_4.Text = QueryValue("l_36_4");
            txtrate_l_36_5.Text = QueryValue("l_36_5");

            txtrate_r_48_1.Text = QueryValue("r_48_1");
            txtrate_r_48_2.Text = QueryValue("r_48_2");
            txtrate_r_48_3.Text = QueryValue("r_48_3");
            txtrate_r_48_4.Text = QueryValue("r_48_4");
            txtrate_r_48_5.Text = QueryValue("r_48_5");

            txtrate_l_48_1.Text = QueryValue("l_48_1");
            txtrate_l_48_2.Text = QueryValue("l_48_2");
            txtrate_l_48_3.Text = QueryValue("l_48_3");
            txtrate_l_48_4.Text = QueryValue("l_48_4");
            txtrate_l_48_5.Text = QueryValue("l_48_5");

            txtrate_r_60_1.Text = QueryValue("r_60_1");
            txtrate_r_60_2.Text = QueryValue("r_60_2");
            txtrate_r_60_3.Text = QueryValue("r_60_3");
            txtrate_r_60_4.Text = QueryValue("r_60_4");
            txtrate_r_60_5.Text = QueryValue("r_60_5");

            txtrate_l_60_1.Text = QueryValue("l_60_1");
            txtrate_l_60_2.Text = QueryValue("l_60_2");
            txtrate_l_60_3.Text = QueryValue("l_60_3");
            txtrate_l_60_4.Text = QueryValue("l_60_4");
            txtrate_l_60_5.Text = QueryValue("l_60_5");

            //Acceptance Fees for both

            txtrate_r_fee_1.Text = QueryValue("r_fee_1");
            txtrate_r_fee_2.Text = QueryValue("r_fee_2");
            txtrate_r_fee_3.Text = QueryValue("r_fee_3");
            txtrate_r_fee_4.Text = QueryValue("r_fee_4");
            txtrate_r_fee_5.Text = QueryValue("r_fee_5");

            txtrate_l_fee_1.Text = QueryValue("l_fee_1");
            txtrate_l_fee_2.Text = QueryValue("l_fee_2");
            txtrate_l_fee_3.Text = QueryValue("l_fee_3");
            txtrate_l_fee_4.Text = QueryValue("l_fee_4");
            txtrate_l_fee_5.Text = QueryValue("l_fee_5");
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            // Save Rates from this form
            // Rental Rates
            SaveValue("r_12_1", txtrate_r_12_1.Text);
            SaveValue("r_12_2", txtrate_r_12_2.Text);
            SaveValue("r_12_3", txtrate_r_12_3.Text);
            SaveValue("r_12_4", txtrate_r_12_4.Text);
            SaveValue("r_12_5", txtrate_r_12_5.Text);

            SaveValue("r_24_1", txtrate_r_24_1.Text);
            SaveValue("r_24_2", txtrate_r_24_2.Text);
            SaveValue("r_24_3", txtrate_r_24_3.Text);
            SaveValue("r_24_4", txtrate_r_24_4.Text);
            SaveValue("r_24_5", txtrate_r_24_5.Text);

            SaveValue("r_36_1", txtrate_r_36_1.Text);
            SaveValue("r_36_2", txtrate_r_36_2.Text);
            SaveValue("r_36_3", txtrate_r_36_3.Text);
            SaveValue("r_36_4", txtrate_r_36_4.Text);
            SaveValue("r_36_5", txtrate_r_36_5.Text);

            SaveValue("r_48_1", txtrate_r_48_1.Text);
            SaveValue("r_48_2", txtrate_r_48_2.Text);
            SaveValue("r_48_3", txtrate_r_48_3.Text);
            SaveValue("r_48_4", txtrate_r_48_4.Text);
            SaveValue("r_48_5", txtrate_r_48_5.Text);

            SaveValue("r_60_1", txtrate_r_60_1.Text);
            SaveValue("r_60_2", txtrate_r_60_2.Text);
            SaveValue("r_60_3", txtrate_r_60_3.Text);
            SaveValue("r_60_4", txtrate_r_60_4.Text);
            SaveValue("r_60_5", txtrate_r_60_5.Text);

            SaveValue("r_fee_1", txtrate_r_fee_1.Text);
            SaveValue("r_fee_2", txtrate_r_fee_2.Text);
            SaveValue("r_fee_3", txtrate_r_fee_3.Text);
            SaveValue("r_fee_4", txtrate_r_fee_4.Text);
            SaveValue("r_fee_5", txtrate_r_fee_5.Text);

            //Fixed Term Loan Rates
            SaveValue("l_12_1", txtrate_l_12_1.Text);
            SaveValue("l_12_2", txtrate_l_12_2.Text);
            SaveValue("l_12_3", txtrate_l_12_3.Text);
            SaveValue("l_12_4", txtrate_l_12_4.Text);
            SaveValue("l_12_5", txtrate_l_12_5.Text);

            SaveValue("l_24_1", txtrate_l_24_1.Text);
            SaveValue("l_24_2", txtrate_l_24_2.Text);
            SaveValue("l_24_3", txtrate_l_24_3.Text);
            SaveValue("l_24_4", txtrate_l_24_4.Text);
            SaveValue("l_24_5", txtrate_l_24_5.Text);

            SaveValue("l_36_1", txtrate_l_36_1.Text);
            SaveValue("l_36_2", txtrate_l_36_2.Text);
            SaveValue("l_36_3", txtrate_l_36_3.Text);
            SaveValue("l_36_4", txtrate_l_36_4.Text);
            SaveValue("l_36_5", txtrate_l_36_5.Text);

            SaveValue("l_48_1", txtrate_l_48_1.Text);
            SaveValue("l_48_2", txtrate_l_48_2.Text);
            SaveValue("l_48_3", txtrate_l_48_3.Text);
            SaveValue("l_48_4", txtrate_l_48_4.Text);
            SaveValue("l_48_5", txtrate_l_48_5.Text);

            SaveValue("l_60_1", txtrate_l_60_1.Text);
            SaveValue("l_60_2", txtrate_l_60_2.Text);
            SaveValue("l_60_3", txtrate_l_60_3.Text);
            SaveValue("l_60_4", txtrate_l_60_4.Text);
            SaveValue("l_60_5", txtrate_l_60_5.Text);

            SaveValue("l_fee_1", txtrate_l_fee_1.Text);
            SaveValue("l_fee_2", txtrate_l_fee_2.Text);
            SaveValue("l_fee_3", txtrate_l_fee_3.Text);
            SaveValue("l_fee_4", txtrate_l_fee_4.Text);
            SaveValue("l_fee_5", txtrate_l_fee_5.Text);

            //GST Rate
            SaveValue("GST", txtGSTRate.Text);

            MessageBox.Show("Saved");

            this.Close();
        }

        private void btnAbortChanges_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
