using System;
using System.Threading.Tasks;

namespace dgTaskt6_Parallel
{
    class Program
    {
        static void Main(string[] args)
        {
            Parallel.Invoke(() => met1(),
                            () => met2(),
                            () => met3());
            Console.WriteLine("End!");
        }
        static void met1()
        {
            while(true)
                Console.WriteLine("Met 1");
        }
        static void met2()
        {
            while (true)
                Console.WriteLine("Met 2");
        }
        static void met3()
        {
            while (true)
                Console.WriteLine("Met 3");
        }


    }
}
