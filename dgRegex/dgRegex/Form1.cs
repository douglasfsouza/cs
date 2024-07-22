using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


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

        private void button5_Click(object sender, EventArgs e)
        {
           // string data = "2021-02-28";
            string data = "28-02-2021";
            //Match mdata = Regex.Match(data, @"dddd");
            if (Regex.IsMatch(data,@"^[0-9]{4}",RegexOptions.None))
            {
                MessageBox.Show("A data inicia com ano");
            }
            else
            {
                MessageBox.Show("A data não inicia com ano");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

            string data = "28/02/2021";
            //string data = "2021/02/28";
            //data = "02/28/2021";

            string data2 = DateTime.Parse(data).ToString("yyyy-MM-dd");
            Console.WriteLine(data2);

            /*
            string data = "02/28/2021";
            data = Regex.Replace(data,
             @"\b(?<month>\d{1,2})/(?<day>\d{1,2})/(?<year>\d{2,4})\b",
            "${day}-${month}-${year}", RegexOptions.None,
            TimeSpan.FromMilliseconds(150));
            */

             data = "28-02-2021 17:00";
            //string data = "2021-02-28";
            data = Regex.Replace(data,
             @"\b(?<day>\d{1,2})[/-](?<month>\d{1,2})[/-](?<year>\d{2,4})\b",
            "${year}-${month}-${day}", RegexOptions.None,
            TimeSpan.FromMilliseconds(150));

            MessageBox.Show(data);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string scriptSubject = "new AjaxJspTag.Callout(\\n\"/projudi/ajaxUtils.do?_tj=f8f4dd97473c8d00351e50dc9650f7c7050f66a9f4c8140474c186872421c652\", {\\noverlib: \"STICKY,CLOSECLICK,DELAY,0,VAUTO,HAUTO,CLOSETEXT,'X',CLOSETITLE,'";
 
            if (scriptSubject != null)
            {

                // Define o padrão de regex para extrair o texto desejado
                string pattern = @"title: ""(.*?)""";

                // Cria um objeto Regex
                Regex regex = new Regex(pattern);

                // Procura por correspondências na entrada
                Match match = regex.Match(scriptSubject);

                // Verifica se houve correspondência e obtém o valor do grupo capturado
                if (match.Success)
                {
                    string subject = match.Groups[1].Value;
                    MessageBox.Show(subject);
                }

            }
        }
    }
}

