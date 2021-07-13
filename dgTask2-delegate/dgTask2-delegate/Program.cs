using System;
using System.Threading.Tasks;

namespace dgTask2_delegate
{
    class Program
    {
        static void Main(string[] args)
        {
            Task task = new Task(delegate { Console.WriteLine("The time is {0}", DateTime.Now); });
            task.RunSynchronously();
        }
    }
}
