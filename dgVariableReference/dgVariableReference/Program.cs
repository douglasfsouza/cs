using System;

namespace dgVariableReference
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = 1;
            var y = x;
            x = 42;
            Console.WriteLine($"{x}, y={y}");
            Console.WriteLine("Copy value and Not Reference like Objects!!");
        }
    }
}
