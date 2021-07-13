using System;
using System.Threading;
using System.Threading.Tasks;

namespace dgTask5_Cancel
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            Task task = Task.Run(() => emLoop(ct));
            task.Wait();
            
            

            Console.WriteLine("End");
        }
        static void emLoop(CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();

                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("Cancelled");
                    return;
                }
                Console.WriteLine("in loop");
            }
            return;
        }
    }
}
