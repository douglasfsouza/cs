using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ValidarTitulos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmBolFor f = new frmBolFor();
            f.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            FrmDarj f = new FrmDarj();
            f.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmDarf f = new frmDarf();
            f.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FrmGareSpIcms f = new FrmGareSpIcms();
            f.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FrmGareSpItcmd f = new FrmGareSpItcmd();
            f.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FrmGareSPDR f = new FrmGareSPDR();
            f.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FrmIPVA f = new FrmIPVA();
            f.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            FrmDPVAT f = new FrmDPVAT();
            f.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        { 
            FrmCLRV f = new FrmCLRV();
            f.Show();

        }

        private void button12_Click(object sender, EventArgs e)
        {
            FrmGPS f = new FrmGPS();
            f.Show();

        }

        private void button13_Click(object sender, EventArgs e)
        {
            FrmFGTS f = new FrmFGTS();
            f.Show();

        }

        private void button14_Click(object sender, EventArgs e)
        {
            FrmDepJud f = new FrmDepJud();
            f.Show();

        }

        private void button15_Click(object sender, EventArgs e)
        {
           

        }
    }
}
