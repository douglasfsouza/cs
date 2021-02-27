using System;

namespace DgForEach
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] names = new string[] { "Douglas", "Andreia", "Matheus", "Nicolas", "Suzanne" };
            foreach(string n in names)
            {
                Console.WriteLine($"The name is {n}");
            }
           
        }
    }
}
