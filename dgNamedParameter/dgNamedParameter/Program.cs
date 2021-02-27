using System;

namespace dgNamedParameter
{
    class Program
    {
        static void Main(string[] args)
        {
            MostrarParametros(p1: "Passing p1");
        }
        static void MostrarParametros(string p1, string p2 = "Default-2")
        {
            Console.WriteLine($"p1={p1}, p2={p2}");

        }
    }
}
