using System;
using System.Threading;
using System.Threading.Tasks;

namespace dgTask10_Inner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Outer task starting");
            Task outer = Task.Run(() =>
            {
                Task inner = Task.Run(() =>
                {
                    Console.WriteLine("Inner task starting");
                    Thread.SpinWait(50000);
                    Console.WriteLine("Inner task completing");
                });
                

            });
            outer.Wait();
            Console.WriteLine("Outer task finished");
        }
    }
}
