﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace dgTask12_Unobserved
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs e) =>
            {
                foreach (Exception ex in ((AggregateException)e.Exception).InnerExceptions)
                {
                    lblResult.Content = string.Format("An exception: {0}", ex.Message);
                }
                e.SetObserved();
            };
            Task.Run(() =>
            {
                using (WebClient client = new WebClient())
                {
                    Uri uri = new Uri("httpx://fourthcoffee/bogus");
                    client.DownloadString(uri);
                }
            });
            Thread.Sleep(5000);
            GC.WaitForPendingFinalizers();
            GC.Collect();
            MessageBox.Show("Execution completed");
        }
    }
}
