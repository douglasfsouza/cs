using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
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

namespace dgAsyncAwait
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

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            lblResult.Content = "Commencing long running operation.Move the screen with your mouse";
            /*
            Task<string> task1 = Task.Run<string>(() =>
            {
                lblResult.Content = "Commencing long running operation";
                Thread.Sleep(10000);
                return "Completed";
            });
            */
            lblResult.Content = LerS().Result;
        }

        private async void btnAsync_Click(object sender, RoutedEventArgs e)
        {
            lblResult.Content = "Commencing long running operation. Move the screen with your mouse";
            /*
            Task<string> task1 = Task.Run<string>(() =>
            {
                lblResult.Content = "Commencing long running operation";
                Thread.Sleep(10000);
                return "Completed";
            });*/
            lblResult.Content = await LerA();
        }
        private static async Task<string> LerA()
        {         
            await Task.Delay(TimeSpan.FromSeconds(10));
            return "Completed";
        }
        private static Task<string> LerS()
        {
            //Task.Delay(100000);
            Thread.Sleep(10000);
            return Task.FromResult("Completed");
        }
    }
}
