using System;

namespace Lab02
{
    class Program
    {
        static void Main(string[] args)
        {
            int n1 = 10;
            int n2 = 0;
            try
            {
                int r = n1 / n2;

            }
            catch (DivideByZeroException dex)
            {
                Console.WriteLine(dex.Message);
            }

            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            Console.ReadKey(); 
            
        }
    }
}
