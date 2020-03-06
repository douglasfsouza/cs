using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Th6
{
    class Program
    {
        [ThreadStatic]
        static int _field;
        static void Main(string[] args)
        {
            new Thread(() =>
            {
                for(int i =0; i < 10; i++)
                {
                    _field++;
                    Console.WriteLine("Thread A:{0}", _field);
                }

            }).Start();

            new Thread(() =>
            {
                for (int x =0; x< 10; x++)
                {
                    _field++;
                    Console.WriteLine("Thread b:{0}", _field);
                }

            }).Start();

            Console.ReadKey();
            
        }
    }
}
