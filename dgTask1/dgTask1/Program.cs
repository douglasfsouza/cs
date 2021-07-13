using System;
using System.Threading.Tasks;

namespace dgTask1
{
    class Program
    {
        static void Main(string[] args)
        {
            Task tMet = new Task(new Action(myMet));

            tMet.RunSynchronously();          

        }
        public static void myMet()
        {
            Console.WriteLine("Inside the method");
        }
    }
}
