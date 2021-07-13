using System;
using System.Threading.Tasks;

namespace dgTask2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Before task");
     
            Task task = new Task(() => Console.WriteLine("Task completed"));
            task.RunSynchronously();

            task.Wait();

            Task.WaitAll(); // para esperar todas as tasks
            Task.WaitAny(); //idem

            Console.WriteLine("After task");
        }
    }
}
