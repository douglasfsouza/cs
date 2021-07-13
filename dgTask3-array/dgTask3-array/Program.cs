using System;
using System.Threading.Tasks;

namespace dgTask3_array
{
    class Program
    {
        static void Main(string[] args)
        {
            Task[] tasks = new Task[3]
            {
                Task.Run(()=> { Console.WriteLine("Task 1"); }),
                Task.Run(()=> { Console.WriteLine("Task 2"); }),
                Task.Run(()=> { Console.WriteLine("Task 3"); })
            };

            Task.WaitAny(tasks);
            Console.WriteLine("After all");
        }
    }
}
