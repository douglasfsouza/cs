using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgTask
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Task<string> t1 = Task.Run<string>(() => DateTime.Now.DayOfWeek.ToString());
            Console.WriteLine($"Today is {t1.Result}");
            Console.ReadLine();
        }
        
    }
}
