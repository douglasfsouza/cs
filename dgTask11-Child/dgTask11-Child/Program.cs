using System;
using System.Threading;
using System.Threading.Tasks;

namespace dgTask11_Child
{
    class Program
    {
        static void Main(string[] args)
        {           
            var parent = Task.Factory.StartNew(() => {
                Console.WriteLine("Parent task executing.");
                var child = Task.Factory.StartNew(() => {
                    Console.WriteLine("Attached child starting.");
                    Thread.SpinWait(5000000);
                    Console.WriteLine("Attached child completing.");
                }, TaskCreationOptions.AttachedToParent);
            });
            parent.Wait();
            Console.WriteLine("Parent has completed.");
            
            /*
            var parent = Task.Run( () =>
            {
                Console.WriteLine("parent task starting");
                var child = Task.Run( () =>
                {
                    Console.WriteLine("child task starting");
                    Thread.SpinWait(50000);
                    Console.WriteLine("child task completing");
                }, TaskCreationOptions.AttachedToParent);
            });
            parent.Wait();
            Console.WriteLine("parent task completed");
            */
        }
    }
}
