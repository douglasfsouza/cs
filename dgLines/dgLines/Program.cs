using System;
using System.Linq;

namespace dgLines
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = { "123","34","123" };
            foreach(var l in lines)
            {
                Console.WriteLine(l);
            }
            Console.WriteLine("Distinct:");
            var dist = (from b in lines
                      select b).Distinct();
            foreach (var item in dist)
            {
                Console.WriteLine(item);
            }
            var menores = from m in dist
                          where m.Length < 3
                          select m;
            Console.WriteLine("Menores:");
            foreach (var m in menores)
            {
                Console.WriteLine(m);
            }

        }
    }
}
