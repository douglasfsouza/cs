using System;

namespace DgDelatate
{
    class Program
    {
        static void Main(string[] args)
        {
            Del d = delegado;
            d("Hello World Delegate!");
        }
        public static void delegado(string msg)
        {
            Console.WriteLine(msg);
        }
    }
    public delegate void Del(string msg);
}
