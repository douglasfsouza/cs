using System;
using System.Threading.Tasks;

namespace dgTask3_anonimous
{
    class Program
    {
        static void Main(string[] args)
        {
            Task task = new Task(() => { Console.WriteLine("The time is {0}", DateTime.Now.ToString("hh:mm")); });
            task.RunSynchronously();
            
        }
    }
}
