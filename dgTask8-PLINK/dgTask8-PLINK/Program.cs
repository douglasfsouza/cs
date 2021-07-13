using System;
using System.Collections.Generic;
using System.Linq;

namespace dgTask8_PLINK
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> fruits = new List<string>()
            {
                "Orange",
                "Apple",
                "Lemmon"
            };

            var pfruits = from f in fruits.AsParallel()
                          where string.Compare(f,"Apple") == 0
                          select f;

            foreach (var item in pfruits)
            {
                Console.WriteLine(item);
            }
            
        }
    }
}
