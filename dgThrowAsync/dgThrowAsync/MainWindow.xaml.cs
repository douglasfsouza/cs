using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace dgThrowAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnThrow_Click(object sender, RoutedEventArgs e)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    lblResult.Content = "Loading...";
                    Uri uri = new Uri("http://fourthcoffee/bogus");
                    string data = await client.DownloadStringTaskAsync(uri);
                    lblResult.Content = data;
                }
                catch (Exception ex)
                {
                    lblResult.Content = ex.Message;
                }
            }
        }
    }
}
