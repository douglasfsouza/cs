using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thread2
{
    public static class Program
    {
        public static void Th()
        {
            for(int i =0; i < 10; i++)
            {
                Console.WriteLine("th:{0}", i);
                Thread.Sleep(1000);
            }
        }

        public static void Main(string[] args)
        {
            Thread t = new Thread(new ThreadStart(Th));
            t.IsBackground = false;
            t.Start();
            
            //for (int i = 0; i< 4; i++)
           // {
            //    Console.WriteLine("main:{0}", i);
            //    Thread.Sleep(0);
           // }
           // t.Join();
        }
    }
}
