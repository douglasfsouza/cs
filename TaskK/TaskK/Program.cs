using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskK
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch tm = new System.Diagnostics.Stopwatch();
            Console.WriteLine("begin");
            Task t1 = new Task(new Action<object>(Processar), "t1");
            Task t2 = new Task(delegate
            {
                Processar("t2");
            });
            Task t3 = new Task((obj) => Processar(obj), "t3");
            tm.Start();
            t1.Start();
            tm.Stop();
            Console.WriteLine($"t1:{ tm.ElapsedMilliseconds.ToString()}");
            tm.Restart();
            t2.Start();
            tm.Stop();
            Console.WriteLine($"t2:{ tm.ElapsedMilliseconds.ToString()}");
            tm.Restart();
            t3.Start();
            tm.Stop();
            Console.WriteLine($"t3:{ tm.ElapsedMilliseconds.ToString()}");
            Console.WriteLine("end");
            Console.Read();

            TaskScheduler task 
            
        }
        public static void Processar(object taskName)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"Task - {taskName} - {i} ");

            }
            System.Threading.Thread.Sleep(1000);

        }

    }
}
