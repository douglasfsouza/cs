using System;

namespace dgEnum
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Hello World!. Today is {s.sabado} ");
            Console.ReadKey();
        }
        enum s
        {
            domingo = 0,
            segunda = 1,
            terça = 2,
            quarta = 3,
            quinta = 4,
            sexta = 5,
            sabado = 6

        }
    }
}
