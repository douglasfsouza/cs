using System;
using System.Security.Cryptography.X509Certificates;

namespace dgBoxing
{
    class Program
    {
        static void Main(string[] args)
        {
            int a = 100;
            //boxing
            object b = a;
            a = 200;

            //unboxing
            int c = (int)b;

            object d = b;
            b = 500;
            
            Console.WriteLine($"a={a}, b={b}, c={c}, d={d}");
            Console.WriteLine("Boxing é transformar um tipo de valor em um tipo de referencia");
            Console.WriteLine("Mas não funcionou, se b e d são referenciais então deveriam ter o mesmo valor");
            Console.WriteLine("A diferença é que o objeto é armazenado ho heap e não na stack");

            object e =  100;
            object f =  e;
            e =  200;
            Console.WriteLine($"e={e}, f={f}");
        }
    }
}
