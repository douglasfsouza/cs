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

                if (!file.EndsWith(".exe"))
                {
                    DataGridViewColumn c = new DataGridViewColumn();

                    lstFiles.Add(new Files() { File = file });
                }

                

                
                
            }
            grdFiles.DataSource = lstFiles;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = AppContext.BaseDirectory;
            txtPasta.Text = folderBrowserDialog1.SelectedPath;
            loadFiles();
        }

        private bool Break()
        {
            StreamReader r;
            StreamWriter w;     

            string[] files = Directory.GetFiles(txtPasta.Text);
            int errors = 0;
            int countFiles = 1;
            long linesPerFile = Convert.ToInt64(txtLinhasPorArquivo.Text);
            if (linesPerFile == 0) linesPerFile = 500;

            long linesCabec = Convert.ToInt64(txtQtdCab.Text);
            if (linesCabec == 0) linesCabec = 1;

            foreach (var file in files)
            {
                if (!file.EndsWith(".ret"))
                {
                    string newFileName = $"{file.Substring(0,file.Length -4)}-{countFiles}.ret";
                    string cab1 = string.Empty;
                    string cab2 = string.Empty;
                    string cab3 = string.Empty;

                    r = File.OpenText(file);
                    w = File.CreateText(newFileName);                    

                    while (!r.EndOfStream)
                    {
                        //inicia arquivo novo
                        long countNew = 0;
                        while (countNew++ < linesPerFile)
                        {
                            if (r.EndOfStream)
                                break;

                            string line = r.ReadLine();
                         
                            w.WriteLine(line);

                            if (cab1 == string.Empty)
                            {
                                cab1 = line;
                            }
                            if (countNew == 2 && cab2 == string.Empty)
                            {
                                cab2 = line;
                            }
                            if (countNew == 3 && cab3 == string.Empty)
                            {
                                cab3 = line;
                            }
                        }
                        w.Close();
                        if (!r.EndOfStream)
                        {
                            newFileName = $"{file.Substring(0, file.Length - 4)}-{++countFiles}.ret";
                            w = File.CreateText(newFileName);
                            if (linesCabec >=1)
                                w.WriteLine(cab1);

                            if (linesCabec >=2)
                                w.WriteLine(cab2);

                            if (linesCabec >=3)
                                w.WriteLine(cab3);
                        }
                    }
                    r.Close();
                }
            }
            return true;
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (Break())
            {
                MessageBox.Show("Arquivos gerados com sucesso");
            }
        }

        private class Files
        {
            public string File { get; set; }
        }
    }
}
