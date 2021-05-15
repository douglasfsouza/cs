using System;

namespace dgInheritList
{
    class Program
    {
        static void Main(string[] args)
        {
            DgList<string> letters = new DgList<string>()
            {
                "a","b","c","a","d"
            };

            Console.WriteLine("Before remove duplicates:");
            foreach (string l in letters)
            {
                Console.WriteLine(l);
            }
            Console.WriteLine("Qtde: {0}", letters.Count);
            Console.WriteLine();
            Console.WriteLine("After remove duplicates");
            letters.RemoveDuplicates();
            foreach (string l in letters)
            {
                Console.WriteLine(l);
            }
            Console.WriteLine("Qtde: {0}", letters.Count);

        }
    }
}
