using System;

namespace DgJagged
{
    class Program
    {
        static void Main(string[] args)
        {
            string[][] casa = new string[4][];
            casa[0] = new string[] { "Mojo", "Koda" };
            casa[1] = new string[] { "Hulk" };
            casa[2] = new string[] { "Andreia", "Suzanne" };
            casa[3] = new string[] { "Douglas", "Matheus", "Nicolas" };
            
            int i = 0;
            Console.WriteLine("Dogs:");
            for (int a = 0; a < casa[i].Length; a++)
                Console.WriteLine(casa[i][a]);
            Console.WriteLine();

            i = 1;
            Console.WriteLine("Cats:");
            for (int a = 0; a < casa[i].Length; a++)
                Console.WriteLine(casa[i][a]);
            Console.WriteLine();

            i = 2;
            Console.WriteLine("Womem:");
            for (int a = 0; a < casa[i].Length; a++)
                Console.WriteLine(casa[i][a]);
            Console.WriteLine();

            i = 3;
            Console.WriteLine("Mem:");
            for (int a = 0; a < casa[i].Length; a++)
                Console.WriteLine(casa[i][a]);
            Console.WriteLine();           
            
        }
    }
}
