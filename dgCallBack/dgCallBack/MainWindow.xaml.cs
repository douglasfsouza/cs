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

namespace dgCallBack
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

        private async void btnGetCoffee_Click(object sender, RoutedEventArgs e)
        { 
            await GetCoffees(RemoveDuplicates);
        }
        private async Task GetCoffees(Action<List<string>> callback)
        {
            var coffees = await Task.Run(() =>
            {
                List<string> coffeesList = new List<string>()
                {
                    "Cappucino",
                    "Cafe au Leit",
                    "Mocacino",
                    "Cappucino"
                };
                return coffeesList;
            });

            await Task.Run(() => callback(coffees));
           
        }
        private async void RemoveDuplicates(List<string> coffees)
        {
            List<string> unique = (from u in coffees
                                   select u).Distinct().ToList();

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                lblResult.Content = unique.Count();
                listBox.Items.Clear();
                foreach (var item in unique)
                {
                    listBox.Items.Add(item);
                }
                
            }));           
        }
    }
}
