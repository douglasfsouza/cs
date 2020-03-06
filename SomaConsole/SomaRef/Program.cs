using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomaRef
{
    class Program
    {
        static void Main(string[] args)
        {
            int num1 = 5;
            Console.WriteLine(SomaRef(ref num1, 6));
            Console.ReadLine();
        }
        static int SomaRef(ref int n1, int n2)
        {
            return n1 + n2;
        }
    }
}
