using System;
using System.Threading;

namespace Thread1
{
    class Program
    {
        
        static void TMethod()
        {
            for (int i =0; i<10; i++)
            {
                Console.WriteLine("TMethod: {0}", i);
                Thread.Sleep(0);
            }
        }
        static void Main(string[] args)
        {
            Thread t = new Thread(new ThreadStart(TMethod));
            t.Start();
            for (int i =0; i<4; i++)
            {
                Console.WriteLine("Main:{0}",i);
                Thread.Sleep(0);
            }
            t.Join();
            
        }
    }
}
