using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Home
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            homeEntities h = new homeEntities();
            var n = h.Family.First(x => x.name == "Doug");
            if (n != null)
            {
                MessageBox.Show(n.name);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNome.Text))
            {
                MessageBox.Show("Nome não informado");
                return;
            }

            if (string.IsNullOrEmpty(cboSexo.Text))
            {
                MessageBox.Show("Sexo não informado");
                return;
            }
            homeEntities h = new homeEntities();
            
            Home.Family family = new Home.Family();
            var qtd = h.Family.Count();
            int maxId = 0;
            if (qtd !=0)
            {
                maxId = h.Family.Max(i => i.Id);
            }
            
            try
            {                
                family = h.Family.First(n => n.name == txtNome.Text);
            }
            catch (Exception)
            {
                
            }
           
            if (family.Id != 0 )
            {
                MessageBox.Show("Nome já incluído");
                return;
            }

            family.Id = ++maxId;
            family.name = txtNome.Text;
            family.Gender = cboSexo.Text.Substring(0, 1);
            family.DateOfBirth = dateTimePicker1.Value;

            h.Family.Add(family);

            h.SaveChanges();




            


            MessageBox.Show("Dados incluidos com sucesso");

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            homeEntities h = new homeEntities();
            foreach (var item in h.Family)
            {
                cboCodigo.Items.Add(item.Id.ToString());              

            }
        }

        private void cboCodigo_SelectedIndexChanged(object sender, EventArgs e)
        {
            homeEntities h = new homeEntities();
            var data = h.Family.First(m => m.Id.ToString() == cboCodigo.Text);
            if (data != null)
            {
                txtNome.Text = data.name;
                cboSexo.Text = data.Gender;
                dateTimePicker1.Value = Convert.ToDateTime( data.DateOfBirth);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboCodigo.Text))
            {
                MessageBox.Show("Código não informado");
                return;
            }
            homeEntities h = new homeEntities();
            var data = h.Family.First(m => m.Id.ToString() == cboCodigo.Text);
            if (data == null)
            {
                MessageBox.Show("Código não encontrado");
                return;
            }
            else
            {
                h.Family.AddOrUpdate(new Family()
                {
                    Id = Convert.ToInt32(cboCodigo.Text),
                    name = txtNome.Text,
                    Gender = cboSexo.Text.Substring(0,1),
                    DateOfBirth = dateTimePicker1.Value
                }) ;

            }
            h.SaveChanges();
            MessageBox.Show("Dados atualizados");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboCodigo.Text))
            {
                MessageBox.Show("Código não informado");
                return;
            }
            homeEntities h = new homeEntities();
            var data = h.Family.First(m => m.Id.ToString() == cboCodigo.Text);
            if (data == null)
            {
                MessageBox.Show("Código não encontrado");
                return;
            }
            else
            {
                h.Family.Remove(data);
                /*
                h.Family.Remove(new Family()
                {
                    Id = Convert.ToInt32(cboCodigo.Text)
                });*/

            }
            h.SaveChanges();
            MessageBox.Show("Dados excluidos");
        }
    }
}
