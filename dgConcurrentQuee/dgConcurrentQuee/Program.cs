using System;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace dgConcurrentQuee
{
    class Program
    {
        static ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        static void Main(string[] args)
        {
            var taskPlace = Task.Run(() => PlaceOrders());
            Task.Run(() => Process());
            Task.Run(() => Process());
            Task.Run(() => Process());
            taskPlace.Wait();

            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
        }
        static void PlaceOrders()
        {
            for(int i = 0; i < 100; i++)
            {
                Thread.Sleep(250);
                string order = string.Format("Order {0}", i);
                queue.Enqueue(order);
                Console.WriteLine("Added {0}", order);
            }
        }
        static void Process()
        {
            while (true)
            {
                string order;
                if (queue.TryDequeue(out order))
                {
                    Console.WriteLine("Processed {0}",order);
                }
            }
        }
    }
}
