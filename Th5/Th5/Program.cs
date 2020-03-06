using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Th5
{
    class Program
    {
        
       
        static void Main(string[] args)
        {
            bool blnStoped = false;
            Thread t = new Thread( new ThreadStart(() =>
            {
                while (!blnStoped)
                {
                    Console.WriteLine("Running...");
                    Thread.Sleep(1000);
                }    
            }));
            t.Start();
            Console.WriteLine("Press any key to scape");
            Console.ReadKey();
            blnStoped = true;
            t.Join();
        }
    }
}
