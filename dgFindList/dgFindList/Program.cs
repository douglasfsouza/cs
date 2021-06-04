using System;
using System.Collections.Generic;

namespace dgFindList
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> fruits = new List<string>() { "Banana", "Apple", "Peneaple" };
            var banana = fruits.Find(b => b == "Banana");
            if (banana == null)
            {
                Console.WriteLine("Banana não encontrada!");

            }
            else
            {
                Console.WriteLine("Achou Banana");
            }
            
        }
    }
}
