using System;
using System.Runtime.CompilerServices;

namespace dgExtension
{
    static class Program
    {
        static void Main(string[] args)
        {
            string texto = "Hello World!";
            Console.WriteLine($"Tamanho de {texto}: {texto.tam()}");
            Console.WriteLine();

            ProdutoVM pv = new ProdutoVM()
            {
                Id = 1,
                Descricao = "Coca-cola",
                CategoriaId = 1,
                DescricaoCategoria = "Bebidas"
            };

            Produto p = pv.ToProduto();

            Console.WriteLine(p.Descricao);
        }

        static long tam(this string texto)
        {
            return texto.Length;
        }

        static Produto ToProduto(this ProdutoVM produto)
        {
            return new Produto()
            {
                Id = produto.Id,
                Descricao = produto.Descricao,
                CategoriaId = produto.CategoriaId
            };
        }


    }
    class Produto
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int CategoriaId { get; set; }
    }

    class Categoria
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
    }

    class ProdutoVM
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int CategoriaId { get; set; }
        public string DescricaoCategoria { get; set; }

        
    }

     

   
}
