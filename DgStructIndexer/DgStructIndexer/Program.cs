using System;

namespace DgStructIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu("");
           menu[0] = "Capuccino";

            Console.WriteLine(menu[0]);
        }
    }
    public struct Menu
    {
        private string[] beverage;
        public Menu(string b) 
        {
            beverage = new string[] { "capuccino", "machiato", "expresso" };
        }
        public string this[int index]
        {
            get { return this.beverage[index]; }
            set { this.beverage[index] = value; }
        }
    }
}
