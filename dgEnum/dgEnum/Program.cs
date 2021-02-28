using System;

namespace dgEnum
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Hello World!. Today is {s.sabado} ");
            Console.ReadKey();

            Month m = Month.february;
            Console.WriteLine($"We are in {m}");
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

        };
        enum Month {january,
                    february,
                    march
       }
    }
}
