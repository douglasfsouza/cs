using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReplaceCV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtPasta.Text = folderBrowserDialog1.SelectedPath;

            if (!Directory.Exists(txtPasta.Text))
            {
                MessageBox.Show("Pasta inexistente");
                return;
            }

            loadFiles();      


        }
        void loadFiles()
        {
            string[] files = Directory.GetFiles(txtPasta.Text);

            List<Files> lstFiles = new List<Files>();


//            listView1.Clear();
            foreach (var file in files)
            {

                //listView1.Items.Add(file);

                DataGridViewColumn c = new DataGridViewColumn();

                lstFiles.Add(new Files() { File = file });

                
                
            }
            grdFiles.DataSource = lstFiles;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadFiles();
        }

        private bool Replace()
        {
            StreamReader r;
            StreamWriter w;          


            string[] files = Directory.GetFiles(txtPasta.Text);
            foreach (var file in files)
            {
                string newFileName = $"{file}.new";

                r = File.OpenText(file);
                w = File.CreateText(newFileName);

                while (!r.EndOfStream)
                {
                    string line = r.ReadLine();

                    line=line.Replace("development.views.", "%placeholder%.");
                    line=line.Replace("sap.sbocarone.", "%placeholder%.");
                    line=line.Replace("SBO_CARONE", "%PLACEHOLDER%");

                    w.WriteLine(line);

                }
                r.Close();
                w.Close();
                File.Delete(file);
                File.Move(newFileName, file);

            }
            return true;
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (Replace())
            {
                MessageBox.Show("Arquivos substituidos com sucesso");
            }
        }

        private class Files
        {
            public string File { get; set; }
        }
    }
}
