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

namespace dgDispatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            showTime();
        }
        

        private void setTime(string time)
        {
            lblData.Content = time;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void time_click(object sender, RoutedEventArgs e)
        {
            showTime();
        }
        private void showTime()
        {
            Task.Run(() =>
            {
                string t = DateTime.Now.ToLongTimeString();
                lblData.Dispatcher.BeginInvoke(new Action(() => setTime(t)));
            });
        }
    }
}
