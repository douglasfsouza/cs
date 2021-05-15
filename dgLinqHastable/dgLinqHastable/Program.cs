using System;
using System.Collections;
using System.Linq;

namespace dgLinqHastable
{
    class Program
    {
        static void Main(string[] args)
        {
            Hashtable prices = new Hashtable();
            prices.Add("Capuccino", 3.5M);
            prices.Add("Café au lait", 2.5M);
            prices.Add("Neat coffee", 1M);

            Console.WriteLine("In price order");
            var drinks =
                from string drink in prices.Keys
                orderby prices[drink] ascending
                select drink;
            Console.WriteLine($"The first is {drinks.FirstOrDefault()}");
            Console.WriteLine($"The last is {drinks.Last()}");
            
            foreach(var d in drinks)
            {
                Console.WriteLine($"The drink: {d} that cost {prices[d]}");
            }

            Console.WriteLine();
            Console.WriteLine("In price order:");

            var jprices =
                   from decimal price in prices.Values
                   orderby price
                   select price;
            foreach(var p in jprices)
            {
                Console.WriteLine($"The price is {p}");
            }

            Console.WriteLine($"The cheapest cost {jprices.Min()}");
            Console.WriteLine($"The highest cost {jprices.Max()}");
            Console.WriteLine();
            Console.WriteLine("Bargains:");
            var bargains =
                from string drink in prices.Keys
                where (decimal)prices[drink] < 2
                orderby prices[drink] ascending
                select drink;
            foreach(var b in bargains)
            {
                Console.WriteLine($"{b} the cost {prices[b]}");
            }

            Console.WriteLine();
            Console.WriteLine($"The media is {jprices.Average()}");

            Console.WriteLine();
            Console.WriteLine($"The count is {jprices.Count()}");

            var jmax =
                   (from decimal price in prices.Values                   
                  select price).Max();
            Console.WriteLine();
            Console.WriteLine($"Just Max in query is {jmax}");
        }
    }
}
