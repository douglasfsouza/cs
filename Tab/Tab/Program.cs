using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tab
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            for(int i = 1; i < 11; i++)
            {
                Console.WriteLine("{0} X {1} = {2}", args[0], i, int.Parse(args[0]) * i);
                Console.Beep();
            }
            Console.Title = "Tabuada";
        }
    }
}
