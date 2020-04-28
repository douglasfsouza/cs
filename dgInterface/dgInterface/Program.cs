using System;

namespace dgInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            TV t = new TV();
            t.MudarCanal(3);
            Console.WriteLine($"A Tv está no canal {t.Canal}");
            TV2 t2 = new TV2();
            t2.AcessarProtegido();
            Console.ReadKey();
        }
    }
}
