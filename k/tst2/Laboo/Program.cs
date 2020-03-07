using System;

namespace Laboo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] != "maquina") {
                Cafe caf = new CafeExpresso();
                Console.WriteLine("expresso");
                Console.WriteLine(caf.ModoDeServir());
            }
            else
            {
                Cafe cafc = new CafeCaseiro();
                Console.WriteLine("caseiro");
                Console.WriteLine(cafc.ModoDeServir());
            }

            

            


            //Console.WriteLine($"tipo:{caf.Tipo}");
            Console.ReadKey();

        }
    }
}
