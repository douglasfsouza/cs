using dgWFP1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dgWFP1
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSum_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DgMath math = new DgMath();
                double n1 = Convert.ToDouble(string.IsNullOrEmpty(txtN1.Text) ? "0" : txtN1.Text);
                double n2 = Convert.ToDouble(string.IsNullOrEmpty(txtN2.Text) ? "0" : txtN2.Text);
                double r = math.Sum(n1, n2);
                txtResult.Text = r.ToString();
                MessageBox.Show($"The rusult of {n1}+{n2} is {r}");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            
            
        }
    }
}
