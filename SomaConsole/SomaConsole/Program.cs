using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomaConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Informe um nro:");
            int num1 = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Informe outro nro:");
            int num2 = Convert.ToInt32(Console.ReadLine());

            int res = Somar(n2: num2, n1: num1);

            Console.WriteLine($"{num1}+{num2}={res}");
            Console.ReadLine();

        }
        public static int Somar(int n1 = 0, int n2 = 0)
        {
            return n1 + n2;
        }
        public static int Somar(int n1 = 0, int n2 = 0, int n3=0)
        {
            return Somar(n1, n2) + n3;
        }

    }

}
