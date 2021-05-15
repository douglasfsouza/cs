using System;

namespace dgStatic
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Classes estáticas não são instanciadas, são chamadas diretamente!");

            double n = 15;
            double d = Conversoes.Dobro(n);

            Console.WriteLine("O dobro de {0} é {1}", n, d);
        }
    }
}
