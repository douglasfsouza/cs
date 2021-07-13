using System;
using System.Reflection;
using System.Threading.Tasks;

namespace dgTask4_Result
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string> task1 = new Task<string>(() => DateTime.Now.DayOfWeek.ToString());
            task1.RunSynchronously();
            //task1.Wait();
            //dont need wait because .Result has wait
            Console.WriteLine("Today is {0}", task1.Result);

            //or
            Task<string> task2 = Task.Run<string>(() => DateTime.Today.ToString("dd-MM-yyyy"));
            //task2.Wait();
            Console.WriteLine("Today is {0}", task2.Result);



        }
    }
}
