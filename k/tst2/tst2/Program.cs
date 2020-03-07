using System;


namespace tst2
{
    class Program
    {
        static void Main(string[] args)
        {
            int num1 = 3;
            int num2 = 2;

            Console.WriteLine(Somar(n2: num2, n1: ref num1));
            Console.ReadKey();

            

        }
        static int Somar(ref int n1, int n2 = 0)
        {
            int  s = n1 + n2;
            return s;
        }
    }
}
