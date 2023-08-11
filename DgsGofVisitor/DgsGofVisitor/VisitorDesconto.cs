using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DgsGofVisitor
{
    public class VisitorDesconto : IVisitor
    {
        private const double val = 12;
        public void Accept(Carro car)
        {
            double newPreco = car.Preco - (car.Preco * val / 100);
            Console.WriteLine("");
            Console.WriteLine("Aplicar o desconto:");
            Console.WriteLine($"Carro: {car.Nome}");
            Console.WriteLine($"Preço: {newPreco}");
        }       
    }
}
