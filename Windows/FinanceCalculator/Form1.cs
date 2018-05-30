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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            radrent36m.Checked = true;
            txtRentalAmtToFinance.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            CalculateRental();
            //// Work out what Term we're on.
            //int SelectedTerm = 0;
            //if (radrent12m.Checked)
            //{
            //    SelectedTerm = 12;
            //}
            //else if (radrent24m.Checked)
            //{
            //    SelectedTerm = 24;
            //}
            //else if (radrent36m.Checked)
            //{ 
            //    SelectedTerm = 36; 
            //}
            //else if (radrent48m.Checked) 
            //{ 
            //    SelectedTerm = 48; 
            //} 
            //else if (radrent60m.Checked) 
            //{ 
            //    SelectedTerm = 60; 
            //}
                        
            //// Run Rental Calculation
            //RentalCalculation(SelectedTerm);

            // Return the focus to the correct box
            txtRentalAmtToFinance.Focus();
        }

        private float RegistryRateRetrieve(string rateregkey)
        {
            // Return the string contained within the registry rate data.
            // Convert the string to a float before returning it.

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\BAL\\EFLCalc\\Rates");
            string rktemprate = (string)rk.GetValue(rateregkey);
            float rkrate = Convert.ToSingle(rktemprate);
            return rkrate;
        }

        private string RegistryKeyRetrieve(string regkey)
        {
            // Return the string containing the registry data (config settings)

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\BAL\\EFLCalc");
            string rkkey = (string)rk.GetValue(regkey);
            return rkkey;
        }

        private float ReturnRate(int TermOfRental, float AmountOfRental, int CorporateRate, char TransactionType)
        {
            // Find Correct Rate based on Term and Amount
            // Return a float.

            float RentalReturnRate = 0;

            if (TermOfRental > 60) { return 0; }
            if (TermOfRental < 12) { return 0; }
            if (AmountOfRental < 500) { return 0; }

            string ratepickup = "";

            ratepickup = "rate_" + TransactionType + "_" + Convert.ToString(TermOfRental) + "_";

            if ((AmountOfRental > 499) && (AmountOfRental < 7500))
            {
                RentalReturnRate = RegistryRateRetrieve(ratepickup+"1");
            }
            if ((AmountOfRental > 7499) && (AmountOfRental < 20000))
            {
                RentalReturnRate = RegistryRateRetrieve(ratepickup+"2");
            }
            if ((AmountOfRental > 19999) && (AmountOfRental < 30000))
            {
                RentalReturnRate = RegistryRateRetrieve(ratepickup+"3");
            }
            if (AmountOfRental > 29999)
            {
                RentalReturnRate = RegistryRateRetrieve(ratepickup+"4");
            }
            if (CorporateRate == 1)
            {
                RentalReturnRate = RegistryRateRetrieve(ratepickup+"5");
            }

            return RentalReturnRate;
        }

        private float ReturnTaxRate()
        {
            // Find current rate of GST/Tax in system
            // Return this value as a float.

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\BAL\\EFLCalc\\Rates");
            string rktemptax = (string)rk.GetValue("rate_GST");
            float rktax = Convert.ToSingle(rktemptax);
            return rktax;
        }

        private float ReturnAcceptanceFee(int TermOfRental, float AmountOfRental, char TransactionType)
        {
            // Find current acceptance rate for this Rental transaction
            // Return this value a a float.

            float FeeReturnRate = 0;

            string feepickup = "";

            feepickup = "rate_" + TransactionType + "_fee_";

            if ((AmountOfRental > 499) && (AmountOfRental < 7500))
            {
                FeeReturnRate = RegistryRateRetrieve(feepickup + "1");
            }
            if ((AmountOfRental > 7499) && (AmountOfRental < 20000))
            {
                FeeReturnRate = RegistryRateRetrieve(feepickup + "2");
            }
            if ((AmountOfRental > 19999) && (AmountOfRental < 30000))
            {
                FeeReturnRate = RegistryRateRetrieve(feepickup + "3");
            }
            if (AmountOfRental > 29999)
            {
                FeeReturnRate = RegistryRateRetrieve(feepickup + "4");
            }

            return FeeReturnRate;
        }

        private void RentalCalculation(int RentalTerm)
        {
            // Calculate Rental
           
            float RentalAmountToFinanceExc;
            float RentalAmountToFinanceInc;
            int CorpRate;

            float RentalTaxRate;
            float RentalTaxAmount;
            float RentalFactor;
            float RentalRateExc;
            float RentalPerDayExc;
            float RentalPerWeekExc;
            float RentalPerMonthExc;
            float RentalFinanceRate;
            float RentalFinanceTotalExc;
            float RentalFinanceTotalInc;
            float RentalFinanceTotalDiffExc;
            float RentalFinanceTotalDiffInc;
            float RentalAcceptanceFeeExc;
            float RentalAcceptanceFeeInc;
            float RentalInitialPayment;
            
            float RentalPerDayInc;
            float RentalPerWeekInc;
            float RentalPerMonthInc;            

            // Grab the amount to finance from the screen

            try
            {
                RentalAmountToFinanceExc = Convert.ToSingle(txtRentalAmtToFinance.Text);
            }
            catch
            {
                RentalAmountToFinanceExc = 0;
            }

            // Check to make sure the calculation is even valid.

            if (RentalTerm < 12 || RentalTerm > 60)
            {
                MessageBox.Show("Rental Term is Invalid.");
                return;
            }
            
            if (RentalAmountToFinanceExc < 500f)
            {
                MessageBox.Show("Rental Amount must be above\r\n$1,000 exc GST.");
                return;
            }
            if (RentalAmountToFinanceExc < 1000f)
            {
                if (cmbRentalCompany.Text == "EFL")
                {
                    DialogResult changerentalco;
                    changerentalco = MessageBox.Show("EFL Rentals are only above $1,000 ex Tax. Change Rental Co to BAL?", "Excuse me", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (changerentalco == DialogResult.Yes)
                    {
                        cmbRentalCompany.Text = "BAL";
                    }
                    else
                    {
                        return;
                    }
                }
            }

            // Work out the rental factors and rates

            if (chkCorporateRate.Checked)
            {
                CorpRate = 1; // Yes, Corporate/Government Rate
            }
            else
            {
                CorpRate = 0; // No, Normal Consumer Rates Apply
            }

            RentalFactor = ReturnRate(RentalTerm, RentalAmountToFinanceExc, CorpRate, 'r');
            RentalTaxRate = ReturnTaxRate();
            RentalAcceptanceFeeExc = ReturnAcceptanceFee(RentalTerm, RentalAmountToFinanceExc, 'r');

            // Stop - if we're with BAL, do this differently
            if (cmbRentalCompany.Text == "BAL")
            {
                RentalAmountToFinanceExc = RentalAmountToFinanceExc + RentalAcceptanceFeeExc; // Acceptance Fee
                RentalAcceptanceFeeExc = 0; // Reset Acceptance Fee
                RentalAmountToFinanceExc = RentalAmountToFinanceExc + 5f; // PPSR fee
            }


            // Calculations done Exclusively.
          
            RentalPerMonthExc = (RentalFactor * RentalAmountToFinanceExc);
            RentalPerWeekExc = ((RentalPerMonthExc * 12) / 52);
            RentalPerDayExc = ((RentalPerMonthExc * 12) / 365);

            // Get the GST values of this transaction
            RentalAmountToFinanceInc = ((RentalAmountToFinanceExc * (Convert.ToSingle(RentalTaxRate) / 100)) + RentalAmountToFinanceExc);
            RentalAcceptanceFeeInc = ((RentalAcceptanceFeeExc * (RentalTaxRate / 100)) + RentalAcceptanceFeeExc);
            RentalTaxAmount = RentalAmountToFinanceInc - RentalAmountToFinanceExc;

            // Show the GST Inclusive Values by Multiplying GST
            // Note that this is 'split the GST by the term then add to each rate'            

            RentalPerMonthInc = RentalPerMonthExc + (RentalTaxAmount / RentalTerm);
            RentalPerWeekInc = ((RentalPerMonthInc * 12) / 52);
            RentalPerDayInc = ((RentalPerMonthInc * 12) / 365);

            // Totals and Rates

            RentalFinanceTotalExc = (RentalPerMonthExc * RentalTerm) + RentalAcceptanceFeeExc;
            RentalFinanceTotalInc = (RentalPerMonthInc * RentalTerm) + RentalAcceptanceFeeInc;

            RentalFinanceTotalDiffExc = RentalFinanceTotalExc - RentalAmountToFinanceExc;
            RentalFinanceTotalDiffInc = RentalFinanceTotalInc - RentalAmountToFinanceInc;

            if (cmbRentalCompany.Text == "BAL")
            {
                RentalInitialPayment = RentalAcceptanceFeeExc + RentalPerMonthExc;
            }
            else
            {
                RentalInitialPayment = RentalAcceptanceFeeExc + RentalPerMonthExc + 5.00f;
            }

            // Fill in the text boxes
            txtRentalFinancePerWeekExc.Text = String.Format("{0:C}", RentalPerWeekExc);
            txtRentalFinancePerDayExc.Text = String.Format("{0:C}", RentalPerDayExc);
            txtRentalFinancePerMonthExc.Text = String.Format("{0:C}", RentalPerMonthExc);

            txtRentalFinancePerWeekInc.Text = String.Format("{0:C}", RentalPerWeekInc);
            txtRentalFinancePerDayInc.Text = String.Format("{0:C}", RentalPerDayInc);
            txtRentalFinancePerMonthInc.Text = String.Format("{0:C}", RentalPerMonthInc);

            txtRentalFinanceTotalExc.Text = String.Format("{0:C}", RentalFinanceTotalExc);
            txtRentalFinanceTotalInc.Text = String.Format("{0:C}", RentalFinanceTotalInc);

            txtRentalAcceptanceFee.Text = String.Format("{0:C}", RentalAcceptanceFeeExc);

            txtRentalFinanceRate.Text = String.Format("{0}", RentalFactor);

            txtRentalFinanceTotalDiffExc.Text = String.Format("{0:C}", RentalFinanceTotalDiffExc);
            txtRentalFinanceTotalDiffInc.Text = String.Format("{0:C}", RentalFinanceTotalDiffInc);

            txtTransactionType.Text = "Rental";
            txtRentalTerm.Text = Convert.ToString(RentalTerm);

            txtRentalInitialPayment.Text = String.Format("{0:C}", RentalInitialPayment);

        }

        private void FTLCalculation(int FTLTerm)
        {
            // Calculate Fixed Term Loan (FTL)

            int FTLAmountToFinanceExc;
            int FTLAmountToFinanceInc;
            int CorpRate;

            float FTLTaxRate;
            float FTLFactor;
            float FTLRateExc;
            float FTLPerDayExc;
            float FTLPerWeekExc;
            float FTLPerMonthExc;
            float FTLFinanceRate;
            float FTLFinanceTotalExc;
            float FTLFinanceTotalInc;
            float FTLFinanceTotalDiffExc;
            float FTLFinanceTotalDiffInc;
            float FTLAcceptanceFeeExc;
            float FTLAcceptanceFeeInc;            
            
            float FTLPerDayInc;
            float FTLPerWeekInc;
            float FTLPerMonthInc;

            float FTLInitialPayment;

            // Grab the amount to finance from the screen

            try
            {
                FTLAmountToFinanceExc = Convert.ToInt32(txtRentalAmtToFinance.Text);
            }
            catch
            {
                FTLAmountToFinanceExc = 0;
            }

            // Check to make sure the calculation is even valid.

            if (FTLTerm < 12 || FTLTerm > 60)
            {
                MessageBox.Show("Fixed Term Loan Term is Invalid.");
                return;
            }

            if (FTLAmountToFinanceExc < 1000)
            {
                MessageBox.Show("Fixed Term Loan Amount must be above\r\n$1,000 exc GST.");
                return;
            }

            // Work out the rental factors and rates

            if (chkCorporateRate.Checked)
            {
                CorpRate = 1; // Yes, Corporate/Government Rate
            }
            else
            {
                CorpRate = 0; // No, Normal Consumer Rates Apply
            }

            FTLFactor = ReturnRate(FTLTerm, FTLAmountToFinanceExc, CorpRate, 'l');
            FTLTaxRate = ReturnTaxRate();
            FTLAcceptanceFeeExc = ReturnAcceptanceFee(FTLTerm, FTLAmountToFinanceExc, 'l');

            // Calculations done Exclusively.

            FTLPerMonthExc = (FTLFactor * FTLAmountToFinanceExc);
            FTLPerWeekExc = ((FTLPerMonthExc * 12) / 52);
            FTLPerDayExc = ((FTLPerMonthExc * 12) / 365);

            // Show the GST Inclusive Values by Multiplying GST

            FTLPerWeekInc = ((FTLPerWeekExc * (FTLTaxRate / 100)) + FTLPerWeekExc);
            FTLPerDayInc = ((FTLPerDayExc * (FTLTaxRate / 100)) + FTLPerDayExc);
            FTLPerMonthInc = ((FTLPerMonthExc * (FTLTaxRate / 100)) + FTLPerMonthExc);
            FTLAcceptanceFeeInc = ((FTLAcceptanceFeeExc * (FTLTaxRate / 100)) + FTLAcceptanceFeeExc);
            FTLAmountToFinanceInc = ((FTLAmountToFinanceExc * (Convert.ToInt32(FTLTaxRate) / 100)) + FTLAmountToFinanceExc);

            // Totals and Rates

            FTLFinanceTotalExc = (FTLPerMonthExc * FTLTerm) + FTLAcceptanceFeeExc;
            FTLFinanceTotalInc = (FTLPerMonthInc * FTLTerm) + FTLAcceptanceFeeInc;

            FTLFinanceTotalDiffExc = FTLFinanceTotalExc - FTLAmountToFinanceExc;
            FTLFinanceTotalDiffInc = FTLFinanceTotalInc - FTLAmountToFinanceInc;

            FTLInitialPayment = FTLAcceptanceFeeExc + FTLPerMonthExc + 5.00f;
            FTLInitialPayment = FTLInitialPayment + (FTLAmountToFinanceExc * (FTLTaxRate / 100));

            // Fill in the text boxes (on the rental screen at the moment 20110615

            txtRentalFinancePerWeekExc.Text = String.Format("{0:C}", FTLPerWeekExc);
            txtRentalFinancePerDayExc.Text = String.Format("{0:C}", FTLPerDayExc);
            txtRentalFinancePerMonthExc.Text = String.Format("{0:C}", FTLPerMonthExc);

            txtRentalFinancePerWeekInc.Text = String.Format("{0:C}", FTLPerWeekInc);
            txtRentalFinancePerDayInc.Text = String.Format("{0:C}", FTLPerDayInc);
            txtRentalFinancePerMonthInc.Text = String.Format("{0:C}", FTLPerMonthInc);

            txtRentalFinanceTotalExc.Text = String.Format("{0:C}", FTLFinanceTotalExc);
            txtRentalFinanceTotalInc.Text = String.Format("{0:C}", FTLFinanceTotalInc);

            txtRentalAcceptanceFee.Text = String.Format("{0:C}", FTLAcceptanceFeeExc);

            txtRentalFinanceRate.Text = String.Format("{0}", FTLFactor);

            txtRentalFinanceTotalDiffExc.Text = String.Format("{0:C}", FTLFinanceTotalDiffExc);
            txtRentalFinanceTotalDiffInc.Text = String.Format("{0:C}", FTLFinanceTotalDiffInc);

            txtTransactionType.Text = "FTL";
            txtRentalTerm.Text = Convert.ToString(FTLTerm);

            txtRentalInitialPayment.Text = String.Format("{0:C}", FTLInitialPayment);

        }


        private void txtRentalAmtToFinance_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // Run Rental Calculation
                btnCalculate_Click(sender, e);
            }
        }

        private void radrent12m_CheckedChanged(object sender, EventArgs e)
        {
            if (txtRentalAmtToFinance.Text != "")
            {
                btnCalculate_Click(sender, e);
            }            
        }

        private void radrent24m_CheckedChanged(object sender, EventArgs e)
        {
            if (txtRentalAmtToFinance.Text != "")
            {
                btnCalculate_Click(sender, e);
            }
        }

        private void radrent36m_CheckedChanged(object sender, EventArgs e)
        {
            if (txtRentalAmtToFinance.Text != "")
            {
                btnCalculate_Click(sender, e);
            }
        }

        private void radrent48m_CheckedChanged(object sender, EventArgs e)
        {
            if (txtRentalAmtToFinance.Text != "")
            {
                btnCalculate_Click(sender, e);
            }
        }

        private void radrent60m_CheckedChanged(object sender, EventArgs e)
        {
            if (txtRentalAmtToFinance.Text != "")
            {
                btnCalculate_Click(sender, e);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRentFinDetails.Checked == true)
            {
                groupBox3.Visible = true;
            }
            else
            {
                groupBox3.Visible = false;
            }
        }

        private void btnFTLCalculate_Click(object sender, EventArgs e)
        {
            // Work out what Term we're on.
            int SelectedTerm = 0;
            if (radrent12m.Checked)
            {
                SelectedTerm = 12;
            }
            else if (radrent24m.Checked)
            {
                SelectedTerm = 24;
            }
            else if (radrent36m.Checked)
            {
                SelectedTerm = 36;
            }
            else if (radrent48m.Checked)
            {
                SelectedTerm = 48;
            }
            else if (radrent60m.Checked)
            {
                SelectedTerm = 60;
            }

            // Run Rental Calculation
            FTLCalculation(SelectedTerm);

            // Return the focus to the correct box
            txtRentalAmtToFinance.Focus();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            // Check to see if rental calculated
            if (txtRentalInitialPayment.Text == "")
            {
                //Calculate Rental
                CalculateRental();
            }
            // Blank out the box so we can generate a new rental
            rtbPrintBuffer.Text = "";

            // Generate the information we need
            GenerateRentalToPrint();

            // Request the information be printed
            TryPrinting();
        }

        private void GenerateRentalToPrint()
        {
            string CurrentDateTime = String.Format("{0}", DateTime.Now);            

            rtbPrintBuffer.AppendText("+------------------------------------+\n");
            rtbPrintBuffer.AppendText("| Finance Calculator - Printed Quote |\n");
            rtbPrintBuffer.AppendText("| Generated "+CurrentDateTime+"     |\n");
            rtbPrintBuffer.AppendText("+------------------------------------+\n");
            rtbPrintBuffer.AppendText("\n");
            rtbPrintBuffer.AppendText("\nFinance Company    : " + cmbRentalCompany.Text);
            rtbPrintBuffer.AppendText("\nType of Transaction: " + txtTransactionType.Text);
            rtbPrintBuffer.AppendText("\nDuration           : " + txtRentalTerm.Text + " months");
            rtbPrintBuffer.AppendText("\nAmount to Finance  : " + String.Format("{0:C}", Convert.ToSingle(txtRentalAmtToFinance.Text)));            
            rtbPrintBuffer.AppendText("\n");
            rtbPrintBuffer.AppendText("\nAmount per Month   : " + txtRentalFinancePerMonthExc.Text);
            rtbPrintBuffer.AppendText("\nAmount per Week    : " + txtRentalFinancePerWeekExc.Text);
            rtbPrintBuffer.AppendText("\nAmount per Day     : " + txtRentalFinancePerDayExc.Text);
            rtbPrintBuffer.AppendText("\n");            
            rtbPrintBuffer.AppendText("\nFirst Month Payment: " + txtRentalInitialPayment.Text);
            if (cmbRentalCompany.Text == "EFL")
            {
                rtbPrintBuffer.AppendText("\nCalculated from:");
                rtbPrintBuffer.AppendText("\n * Acceptance Fee    - " + txtRentalAcceptanceFee.Text);
                rtbPrintBuffer.AppendText("\n * PPSR Fee          - $5.00");
                rtbPrintBuffer.AppendText("\n * First Payment     - " + txtRentalFinancePerMonthExc.Text);
            }
            if (txtTransactionType.Text == "FTL")
            {
                float TaxContent = ReturnTaxRate();
                float TaxContentofInitial = (Convert.ToSingle(txtRentalAmtToFinance.Text) * (TaxContent / 100));

                rtbPrintBuffer.AppendText("\n * Tax content [" + ReturnTaxRate() + "%] - " + String.Format("{0:C}",TaxContentofInitial));
            }
            rtbPrintBuffer.AppendText("\n");
            rtbPrintBuffer.AppendText("\nAll amounts shown exclude Tax of " + ReturnTaxRate() + "%");
            rtbPrintBuffer.AppendText("\n");            
            rtbPrintBuffer.AppendText("\nThis information is valid from 7\ndays from this date.");
            rtbPrintBuffer.AppendText("\n\n\n.");
        }

        private void TryPrinting()
        {
            string CurrentPrinter = ReadRegistryKey("PrinterName");
            if (CurrentPrinter != "")
            {
                DialogResult messagedialog;
                messagedialog = MessageBox.Show("Use current printer " + CurrentPrinter + "?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messagedialog == DialogResult.No)
                {
                    PrintDialog printDialog1 = new PrintDialog();
                    printDialog1.Document = printDocument1;
                    DialogResult result = printDialog1.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        WriteRegistryKey("PrinterName", printDialog1.PrinterSettings.PrinterName);
                        printDocument1.Print();
                    }
                    else
                    {
                        MessageBox.Show("Cancelled Printing Operation", "Just so you know", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                if (messagedialog == DialogResult.Yes)
                {
                    printDocument1.PrinterSettings.PrinterName = CurrentPrinter;
                    printDocument1.Print();
                }
            }
            else
            {
                PrintDialog printDialog1 = new PrintDialog();
                printDialog1.Document = printDocument1;
                DialogResult result = printDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    WriteRegistryKey("PrinterName", printDialog1.PrinterSettings.PrinterName);
                    printDocument1.Print();
                }
                else
                {
                    MessageBox.Show("Cancelled Printing Operation", "Just so you know", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void WriteRegistryKey(string regkey, string regvalue)
        {
            // Save Registry Key regkey
            RegistryKey rk = Registry.CurrentUser.CreateSubKey("SOFTWARE\\BAL\\EFLCalc");
            rk.SetValue(regkey, regvalue);
        }

        private string ReadRegistryKey(string regkey)
        {
            // Read Registry key and return it
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\BAL\\EFLCalc");
            return (string)rk.GetValue(regkey);
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            System.Drawing.Font printFont;
            printFont = new System.Drawing.Font("Courier New", 9, FontStyle.Bold);

            float yPos = 0f;
            int count = 0;
            float leftMargin = 1; // e.MarginBounds.Left;
            float topMargin = 0; // e.MarginBounds.Top;
            string line = null;
            float linesPerPage = e.MarginBounds.Height / printFont.GetHeight(e.Graphics);
                yPos = topMargin + count * printFont.GetHeight(e.Graphics);
                e.Graphics.DrawString(rtbPrintBuffer.Text, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
             
     
        
        }

        private void chkBuilderCalculate_Click(object sender, EventArgs e)
        {
            // Calculate based on items enabled
                        
            // Initialise Variables            
            txtBuilderTotal.Text = "";
            float BuilderTotal;
            float Builder1Price = 0;
            float Builder2Price = 0;
            float Builder3Price = 0;
            float Builder4Price = 0;
            float Builder5Price = 0;
            float Builder6Price = 0;
            float Builder7Price = 0;
            float Builder8Price = 0;
            float Builder9Price = 0;
            float Builder10Price = 0;
            float Builder11Price = 0;
            float BuilderMarkup = 0;

            // Check certain things exist correctly (to prevent errors)
            if (txtBuilderAmtToAdd.Text == "") { txtBuilderAmtToAdd.Text = "0.00"; }

            // Add the totals of the items if needed
            if (chkBuilder1Enabled.Checked == true)
            {
                try
                {
                    Builder1Price = Convert.ToSingle(txtBuilder1Price.Text);
                }
                catch
                {
                    Builder1Price = 0;
                }
            }
            if (chkBuilder2Enabled.Checked == true)
            {
                try
                {
                    Builder2Price = Convert.ToSingle(txtBuilder2Price.Text);
                }
                catch
                {
                    Builder2Price = 0;
                }
            }
            if (chkBuilder3Enabled.Checked == true)
            {
                try
                {
                    Builder3Price = Convert.ToSingle(txtBuilder3Price.Text);
                }
                catch
                {
                    Builder3Price = 0;
                }
            }
            if (chkBuilder4Enabled.Checked == true)
            {
                try
                {
                    Builder4Price = Convert.ToSingle(txtBuilder4Price.Text);
                }
                catch
                {
                    Builder4Price = 0;
                }
            }
            if (chkBuilder5Enabled.Checked == true)
            {
                try
                {
                    Builder5Price = Convert.ToSingle(txtBuilder5Price.Text);
                }
                catch
                {
                    Builder5Price = 0;
                }
            }
            if (chkBuilder6Enabled.Checked == true)
            {
                try
                {
                    Builder6Price = Convert.ToSingle(txtBuilder6Price.Text);
                }
                catch
                {
                    Builder6Price = 0;
                }
            }
            if (chkBuilder7Enabled.Checked == true)
            {
                try
                {
                    Builder7Price = Convert.ToSingle(txtBuilder7Price.Text);
                }
                catch
                {
                    Builder7Price = 0;
                }
            }
            if (chkBuilder8Enabled.Checked == true)
            {
                try
                {
                    Builder8Price = Convert.ToSingle(txtBuilder8Price.Text);
                }
                catch
                {
                    Builder9Price = 0;
                }      
            }
            if (chkBuilder9Enabled.Checked == true)
            {
                try
                {
                    Builder9Price = Convert.ToSingle(txtBuilder9Price.Text);
                }
                catch
                {
                    Builder9Price = 0;
                }
            }
            if (chkBuilder10Enabled.Checked == true)
            {
                try
                {
                    Builder10Price = Convert.ToSingle(txtBuilder10Price.Text);
                }
                catch
                {
                    Builder10Price = 0;
                }
            }
            if (chkBuilder11Enabled.Checked == true)
            {
                try
                {
                    Builder11Price = Convert.ToSingle(txtBuilder11Price.Text);
                }
                catch
                {
                    Builder11Price = 0;
                }
            }


            BuilderTotal = (Builder1Price + Builder2Price + Builder3Price + Builder4Price + Builder5Price + Builder6Price + Builder7Price + Builder8Price + Builder9Price + Builder10Price + Builder11Price);
            
            // Calculate the markup and add it to the total
            if (chkBuilderAmtPercentage.Checked == true)
            {
                // Amount is a percentage
                BuilderMarkup = (BuilderTotal * (Convert.ToSingle(txtBuilderAmtToAdd.Text) / 100));
            }
            else
            {
                // Amount is a dollar figure
                BuilderMarkup = Convert.ToSingle(txtBuilderAmtToAdd.Text);
            }

            BuilderTotal = BuilderTotal + BuilderMarkup;

            txtBuilderMarkup.Text = Convert.ToString(BuilderMarkup);
            txtBuilderTotal.Text = Convert.ToString(BuilderTotal);
        }

        private void chkBuilderTransferTotal_Click(object sender, EventArgs e)
        {
            // Transfer Amount to Finance Screen
            txtRentalAmtToFinance.Text = txtBuilderTotal.Text;
            // Display Finance Screen
            tabControl1.SelectedIndex = 0;

            // Calculate Rental Pricing
            CalculateRental();
            // Select Amount to Finance Box
            txtRentalAmtToFinance.Focus();
        }

        private void txtBuilder1Desc_TextChanged(object sender, EventArgs e)
        {
            if (txtBuilder1Desc.Text != "")
            {
                chkBuilder1Enabled.Checked = true;
            }
        }

        private void txtBuilder2Desc_TextChanged(object sender, EventArgs e)
        {
            if (txtBuilder2Desc.Text != "")
            {
                chkBuilder2Enabled.Checked = true;
            }
        }

        private void txtBuilder3Desc_TextChanged(object sender, EventArgs e)
        {
            if (txtBuilder3Desc.Text != "")
            {
                chkBuilder3Enabled.Checked = true;
            }
        }

        private void txtBuilder4Desc_TextChanged(object sender, EventArgs e)
        {
            if (txtBuilder4Desc.Text != "")
            {
                chkBuilder4Enabled.Checked = true;
            }
        }

        private void txtBuilder5Desc_TextChanged(object sender, EventArgs e)
        {
            if (txtBuilder5Desc.Text != "")
            {
                chkBuilder5Enabled.Checked = true;
            }
        }

        private void txtBuilder6Desc_TextChanged(object sender, EventArgs e)
        {
            if (txtBuilder6Desc.Text != "")
            {
                chkBuilder6Enabled.Checked = true;
            }
        }

        private void txtBuilder7Desc_TextChanged(object sender, EventArgs e)
        {
            if (txtBuilder7Desc.Text != "")
            {
                chkBuilder7Enabled.Checked = true;
            }
        }

        private void txtBuilder8Desc_TextChanged(object sender, EventArgs e)
        {
            if (txtBuilder8Desc.Text != "")
            {
                chkBuilder8Enabled.Checked = true;
            }
        }

        private void txtBuilder9Desc_TextChanged(object sender, EventArgs e)
        {
            if (txtBuilder9Desc.Text != "")
            {
                chkBuilder9Enabled.Checked = true;
            }
        }

        private void txtBuilder10Desc_TextChanged(object sender, EventArgs e)
        {
            if (txtBuilder10Desc.Text != "")
            {
                chkBuilder10Enabled.Checked = true;
            }
        }

        private void txtBuilder11Desc_TextChanged(object sender, EventArgs e)
        {
            if (txtBuilder11Desc.Text != "")
            {
                chkBuilder11Enabled.Checked = true;
            }
        }

        private void chkBuilderAdjustments_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBuilderAdjustments.Checked == true)
            {
                groupBox6.Visible = true;
            }
            else
            {
                groupBox6.Visible = false;
            }
        }

        private void btnPrintBuilder_Click(object sender, EventArgs e)
        {
            // Blank out the information
            rtbPrintBuffer.Text = "";

            // Check to see if rental calculated
            if (txtRentalInitialPayment.Text == "")
            {
                //Calculate Rental
                CalculateRental();
            }

            if (txtBuilder1Desc.Text == "")
            {
                //MessageBox.Show("Builder fields look empty. Press 'Print Rental' instead.");
                //return;
                DialogResult dialogResult = MessageBox.Show("Builder fields look empty. \nWould you like to print anyway?", "Are you sure?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    rtbPrintBuffer.Text = "";
                    FillBuilder();
                }
            }

            // Generate the information we need
            GenerateBuilderToPrint();

            // Print the information we need
            TryPrinting();
        }

        private void GenerateBuilderToPrint()
        {
            string CurrentDateTime = String.Format("{0}", DateTime.Now);

            rtbPrintBuffer.Text =     "+------------------------------------+\n";
            rtbPrintBuffer.AppendText("| Finance Calculator - Quote Builder |\n");
            rtbPrintBuffer.AppendText("| Generated " + CurrentDateTime + "     |\n");
            rtbPrintBuffer.AppendText("+------------------------------------+\n");
            rtbPrintBuffer.AppendText("\n\n");
            rtbPrintBuffer.AppendText("\nQuoted for " + txtBuilderCustomerName.Text);
            if (chkBuilder1Enabled.Checked == true)
            {
                rtbPrintBuffer.AppendText("\n" + txtBuilder1Desc.Text + " -\t " + String.Format("{0:C}",Convert.ToSingle(txtBuilder1Price.Text)));
            }
            if (chkBuilder2Enabled.Checked == true)
            {
                rtbPrintBuffer.AppendText("\n" + txtBuilder2Desc.Text + " -\t " + String.Format("{0:C}",Convert.ToSingle(txtBuilder2Price.Text)));
            }
            if (chkBuilder3Enabled.Checked == true)
            {
                rtbPrintBuffer.AppendText("\n" + txtBuilder3Desc.Text + " -\t " + String.Format("{0:C}",Convert.ToSingle(txtBuilder3Price.Text)));
            }
            if (chkBuilder4Enabled.Checked == true)
            {
                rtbPrintBuffer.AppendText("\n" + txtBuilder4Desc.Text + " -\t " + String.Format("{0:C}",Convert.ToSingle(txtBuilder4Price.Text)));
            }
            if (chkBuilder5Enabled.Checked == true)
            {
                rtbPrintBuffer.AppendText("\n" + txtBuilder5Desc.Text + " -\t " + String.Format("{0:C}",Convert.ToSingle(txtBuilder5Price.Text)));
            }
            if (chkBuilder6Enabled.Checked == true)
            {
                rtbPrintBuffer.AppendText("\n" + txtBuilder6Desc.Text + " -\t " + String.Format("{0:C}",Convert.ToSingle(txtBuilder6Price.Text)));
            }
            if (chkBuilder7Enabled.Checked == true)
            {
                rtbPrintBuffer.AppendText("\n" + txtBuilder7Desc.Text + " -\t " + String.Format("{0:C}",Convert.ToSingle(txtBuilder7Price.Text)));
            }
            if (chkBuilder8Enabled.Checked == true)
            {
                rtbPrintBuffer.AppendText("\n" + txtBuilder8Desc.Text + " -\t " + String.Format("{0:C}",Convert.ToSingle(txtBuilder8Price.Text)));
            }
            if (chkBuilder9Enabled.Checked == true)
            {
                rtbPrintBuffer.AppendText("\n" + txtBuilder9Desc.Text + " -\t " + String.Format("{0:C}",Convert.ToSingle(txtBuilder9Price.Text)));
            }
            if (chkBuilder10Enabled.Checked == true)
            {
                rtbPrintBuffer.AppendText("\n" + txtBuilder10Desc.Text + " -\t " + String.Format("{0:C}",Convert.ToSingle(txtBuilder10Price.Text)));
            }
            if (chkBuilder11Enabled.Checked == true)
            {
                rtbPrintBuffer.AppendText("\n" + txtBuilder11Desc.Text + " -\t " + String.Format("{0:C}",Convert.ToSingle(txtBuilder11Price.Text)));
            }
            rtbPrintBuffer.AppendText("\n------------------------------------");
            rtbPrintBuffer.AppendText("\nBuilder Total        : \t " + txtBuilderTotal.Text);
            rtbPrintBuffer.AppendText("\n------------------------------------");
            if (chkBuilderAmtPercentage.Checked == true)
            {
                rtbPrintBuffer.AppendText("\nMarkup Percentage: \t" + txtBuilderAmtToAdd.Text + "%");
                rtbPrintBuffer.AppendText("\nMarkup Total     : \t" + String.Format("{0:C}",Convert.ToSingle(txtBuilderMarkup.Text)));
            }
            else
            {
                rtbPrintBuffer.AppendText("\nMarkup Amount    : \t" + String.Format("{0:C}", Convert.ToSingle(txtBuilderAmtToAdd.Text)));
                rtbPrintBuffer.AppendText("\nMarkup Total     : \t" + String.Format("{0:C}",Convert.ToSingle(txtBuilderMarkup.Text)));
            }
            rtbPrintBuffer.AppendText("\n------------------------------------");
            rtbPrintBuffer.AppendText("\n\nTotal to Finance : \t" + String.Format("{0:C}",Convert.ToSingle(txtBuilderTotal.Text)));
            rtbPrintBuffer.AppendText("\n\n");

        }

        private void btnPrintAll_Click(object sender, EventArgs e)
        {
            // Check to see if rental calculated
            if (txtRentalInitialPayment.Text == "")
            {
                //Calculate Rental
                CalculateRental();
            }
            // Check to see if both forms are being used
            if (txtBuilder1Desc.Text == "")
            {
                //MessageBox.Show("Builder fields look empty. Press 'Print Rental' instead.");
                //return;
                DialogResult dialogResult = MessageBox.Show("Builder fields look empty. \nWould you like to print anyway?", "Are you sure?", MessageBoxButtons.YesNo);
                if(dialogResult == DialogResult.Yes)
                {
                    rtbPrintBuffer.Text = "";
                    FillBuilder();
                }
            }
            if (txtRentalFinancePerDayExc.Text == "")
            {
                MessageBox.Show("Rental/LTO fields look empty. Press 'Print Builder' instead.");
                return;
            }


            // Clear out the print buffer
            rtbPrintBuffer.Text = "";

            // Add details of both items
            GenerateBuilderToPrint();
            GenerateRentalToPrint();

            // Now attempt to print this
            TryPrinting();

        }

        private void txtBuilderAmtToAdd_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnMarkupSave_Click(object sender, EventArgs e)
        {
            WriteRegistryKey("MarkupRate", txtBuilderAmtToAdd.Text);
            WriteRegistryKey("MarkupType", Convert.ToString(chkBuilderAmtPercentage.Checked));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                txtBuilderAmtToAdd.Text = RegistryKeyRetrieve("MarkupRate");
            }
            catch
            {
                txtBuilderAmtToAdd.Text = "0";
            }
            try
            {
                chkBuilderAmtPercentage.Checked = Convert.ToBoolean(RegistryKeyRetrieve("MarkupType"));
            }
            catch
            {
                chkBuilderAmtPercentage.Checked = false;
            }
        }

        private void chkOnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOnTop.Checked == true)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }

        private void btnRentalToClipboard_Click(object sender, EventArgs e)
        {
            // Same as printing, except saving to clipboard instead
            rtbPrintBuffer.Text = "";
            GenerateRentalToPrint();
            Clipboard.SetText(rtbPrintBuffer.Text);
        }

        private void FillBuilder()
        {
            txtBuilderCustomerName.Text = "Quick Quote";
            chkBuilder1Enabled.Checked = true;
            txtBuilder1Desc.Text = "Quick Item";
            txtBuilder1Price.Text = txtRentalAmtToFinance.Text;
            txtBuilderAmtToAdd.Text = "0";
            txtBuilderMarkup.Text = "0";
            txtBuilderTotal.Text = txtRentalAmtToFinance.Text;          
        }
        private void CalculateRental()
        {
            // Work out what Term we're on.
            int SelectedTerm = 0;
            if (radrent12m.Checked)
            {
                SelectedTerm = 12;
            }
            else if (radrent24m.Checked)
            {
                SelectedTerm = 24;
            }
            else if (radrent36m.Checked)
            {
                SelectedTerm = 36;
            }
            else if (radrent48m.Checked)
            {
                SelectedTerm = 48;
            }
            else if (radrent60m.Checked)
            {
                SelectedTerm = 60;
            }

            // Run Rental Calculation
            RentalCalculation(SelectedTerm);
        }
    }
}
