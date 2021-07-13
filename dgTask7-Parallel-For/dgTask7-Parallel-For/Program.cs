using System;
using System.Threading.Tasks;

namespace dgTask7_Parallel_For
{
    class Program
    {
        static void Main(string[] args)
        {
            long ini = 0;
            long fim = 10;
            double[] array = new double[(fim - ini) + 1];
            Parallel.For(ini, fim, index =>
              {
                  array[index] = Math.Sqrt(index);
              });

            Task.WaitAll();

            foreach (long item in array)
            {
                Console.WriteLine("array[{0}]={1}",item,array[item]);
            }

            
        }
    }
}
