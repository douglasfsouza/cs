using Microsoft.VisualBasic.CompilerServices;
using System;

namespace DgParametersOut
{
    class Program
    {
        static void Main(string[] args)
        {
            string cp1 = "First Parameter is 'Named'";
            string p2 = null;
            myMethod(p1:cp1, out p2, out string p3);

            Console.WriteLine($"cp1={cp1}, p2={p2}, p3={p3}");
        }
        static void myMethod(string p1, out string p2, out string p3)
        {
            p2 = "MyMethod changed p2";
            p3 = "MyMethod changed p3 'Defined in Line'";
        }
    }
}
