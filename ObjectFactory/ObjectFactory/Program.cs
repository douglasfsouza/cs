using System;

namespace ObjectFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Implementando o padrão FactoryMethod do Gof - Gang of Four");
            Console.WriteLine("Escolha o lanche:");
            Console.WriteLine("hot-Hotdog, bau-Bauru ou xsa-X-Salada");
            var resposta = Console.ReadLine();

            LancheFactory factory = new LancheFactory();
            Lanche lanche = factory.CriarLanche(resposta);

            Console.WriteLine($"Você escolheu o lanche {lanche.Nome}");
            Console.WriteLine("Com os ingredientes:");
            foreach (var item in lanche.Ingredients)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }
    }
}
