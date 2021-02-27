using System;

namespace DgArrayMult
{
    class Program
    {
        static void Main(string[] args)
        {
            string[, ] animals = new string[3, 2];
            animals[0, 0] = "mojo";
            animals[0, 1] = "dog";

            animals[1, 0] = "koda";
            animals[1, 1] = "dog";

            animals[2, 0] = "hulk";
            animals[2, 1] = "cat";

            for (int i = 0; i < animals.Length / animals.Rank; i++)
            {
                Console.WriteLine($"Name:{animals[i, 0]}\r\nType:{animals[i, 1]}");
                Console.WriteLine();

            }
                
        }
    }
}
