using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Th4
{
    class Program
    {
        public static void Th4(Object o)
        {
            for (int i =0; i <(int) o; i++)
            {
                Console.WriteLine("th:{0}", i);
                Thread.Sleep(0);
            }
        }
        static void Main(string[] args)
        {
            Thread t = new Thread(new ParameterizedThreadStart(Th4));
            t.Start( int.Parse(args[0]));
            t.Join();
        }
    }
}
