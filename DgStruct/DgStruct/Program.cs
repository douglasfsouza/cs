using System;

namespace DgStruct
{
    class Program
    {
        static void Main(string[] args)
        {
            Carro carro = new Carro("Peugeot","Prata",2008);
            carro.Dirigir();
            carro.Ano = DateTime.Today.Year;
            carro.Dirigir();
            Console.WriteLine($"Ano:{carro.Ano}(fora)");
            Console.WriteLine("Segundo carro:");

            Carro carro2 = new Carro()
            {
                Fabricante = "Fiat",
                Cor = "Verde",
                Ano = 1998
            };
            carro2.Dirigir();
            
        }
    }
    public struct Carro
    {
        private string _fabricante;
        private string _cor;
        private int _ano;
        public Carro(string fabricante, string cor, int ano)
        {
            _fabricante = fabricante;
            _cor = cor;
            _ano = ano;
        }
        public string Fabricante { get { return _fabricante; } set { _fabricante = value; } }
        public string Cor { get { return _cor; } set { _cor = value; } }
        public int Ano { get { return _ano; } set { _ano = value; } }
        public void Dirigir()
        {
            Console.WriteLine("Structs são como classes, porem mais leves e com limitações");
            Console.WriteLine($"Fabricante:{_fabricante}");
            Console.WriteLine($"Cor:{_cor}");
            Console.WriteLine($"Ano:{_ano}");
        }
    }
}
