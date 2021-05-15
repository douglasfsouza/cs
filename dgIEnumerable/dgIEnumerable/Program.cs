using System;

namespace dgIEnumerable
{
    class Program
    {
        static void Main(string[] args)
        {
            Contador cont = new Contador();
            foreach(var c in cont.Contar() )
            {
                Console.WriteLine(c);
            }            
        }
    }
}
