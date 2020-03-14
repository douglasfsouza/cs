
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsumirSvc
{
    public partial class Form1 : Form
    {
        ServiceReference1.ServicoAlunoClient c = new ServiceReference1.ServicoAlunoClient();
        

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PreencherLista();                                  
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PreencherLista();
        }

        private void PreencherLista()
        {
            
            dataGridView1.DataSource = c.GetAlunos(true);
        }

        private void button3_Click(object sender, EventArgs e)
        {                        
            
            c.AddStudent(new ServiceReference1.Aluno() {
                CPF = txtCPF.Text,
                Nome = txtNome.Text
            });

            PreencherLista();
            data



        }
    }
}
