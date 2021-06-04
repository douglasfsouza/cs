using System;
using System.Collections.Generic;
using System.Linq;

namespace dgDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> fruits = new Dictionary<string, string>();
            fruits.Add("apple", "red");
            fruits.Add("lemon", "green");
            fruits.Add("banana", "yellow");
            var color = fruits.FirstOrDefault(c => c.Key == "lemonx");
            if (color.Key == null)
            {
                Console.WriteLine("Registro não encontrado");
            }
            else
            {
                Console.WriteLine("{0} is {1}", color.Key, color.Value);
            }
            Console.WriteLine();
            Console.WriteLine("Lista completa:");
            foreach (var item in fruits)
            {
                Console.WriteLine("This {0} is {1}", item.Key, item.Value);
            }
        }
    }
}
