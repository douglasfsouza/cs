﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace dgRegex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           // MessageBox.Show("Hello world");
            var t1 = "sfsffasfs{123}fadfafadsfa(234)";

            var r = new Regex(@".*\{(\d+)\}.*\((\d+)\)");

            var ee = r.Match(t1);
            if (r.IsMatch(t1))
                MessageBox.Show("Achou");
            else
                MessageBox.Show("não achou");
            MessageBox.Show(ee.ToString());

            var e2 = r.Matches(t1);

            foreach( Match x in e2) {
                MessageBox.Show(x.Groups[1].Value.ToString());
                MessageBox.Show(x.Groups[2].Value.ToString());
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var em = "douglas.fsouza2014@@gmail.com";

            if(ValidateEmail(em))
                MessageBox.Show("Email valido!");
            else
                MessageBox.Show($"Email Invalido!: {em}");

        }
        public bool ValidateEmail(string emailAddress)
        {
            string regexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            Match matches = Regex.Match(emailAddress, regexPattern);
            return matches.Success;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string hw = "Hell0 World";
            string d = "\\d";
            if (Regex.IsMatch(hw, d, RegexOptions.None))
            {
                MessageBox.Show($"Number found in {hw}");
            }
            else
            {
                MessageBox.Show($"Number Not found in {hw}");

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Match nota = Regex.Match(txtNota.Text, @"[A-E][+-]?$");
            if (nota.Success)
            {
                MessageBox.Show("Nota válida");
            }
            else
            {
                MessageBox.Show("Informe nota de E- à A+");
            }
        }
    }
}

