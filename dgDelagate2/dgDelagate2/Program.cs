using System;

delegate int Op(int n1, int n2);
delegate int Dob(int n);

namespace dgDelagate2
{
    class Program
    {
        static void Main(string[] args)
        {
            Op d = new Op(Mat.Soma);
            int s = d(10,5);

            d = new Op(Mat.Mult);

            int m = d(10, 5);

            Dob dob = new Dob(Mat.Dob);
            int rd = dob(15);
            
            Console.WriteLine("Soma: 10+5={0}, Multiplicação: 10x5={1}, Dobro de 15={2}",s,m,rd);
        }
        public class Mat
        {
            public static int Soma(int n1, int n2)
            {
                return n1 + n2;
            }
            public static int Mult (int n1, int n2)
            {
                return n1 * n2;
            }
            public static int Dob (int n)
            {
                return n * 2;
            }
        }
    }
}
