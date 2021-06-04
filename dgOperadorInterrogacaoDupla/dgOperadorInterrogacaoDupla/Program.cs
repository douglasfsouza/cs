using System;

namespace dgOperadorInterrogacaoDupla
{
    class Program
    {
        static void Main(string[] args)
        {
            int? a = null; // com ? aceita nulo
            //int? a = 2;
            int b = a ?? 1;
             

            Console.WriteLine("{0} = {1}","b",b);
            Console.WriteLine("a = b ?? c");
            Console.WriteLine("Se o valor da esquerda for nulo então pega o da direita");
            Console.WriteLine("caso contrário pega o da esquerda");
            Console.WriteLine("Se b for null entao a = c");
            Console.WriteLine("caso contrario a = b");
        }
    }
}
