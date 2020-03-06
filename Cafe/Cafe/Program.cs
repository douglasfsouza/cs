using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe
{
    class Program
    {
        static void Main(string[] args)
        {
            Cafe ce = new CafeExpresso();
            Console.WriteLine($"Expresso: {ce.ModoDeServir()} ");

            Cafe cc = new CafeCaseiro();
            Console.WriteLine($"Caseiro: {cc.ModoDeServir()}");

            Console.WriteLine($"Formula:{cc.Formula()}");

            ce.Fabricante = "Pele";
            Console.WriteLine($"Fabricante: {ce.Fabricante} ");
            Console.ReadKey();
        }
    }
}
