using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DgsGofVisitor
{
    public class Carro : ICarro
    {
        private string _nome;
        private double _preco;

        public string Nome { get => _nome; set => _nome = value; }
        public double Preco { get => _preco; set => _preco = value; }
        public Carro(string nome, double preco)
        {
            _nome = nome;
            _preco = preco;            
        }
        public void Visit(IVisitor vis)
        {
            vis.Accept(this);
        }
    }
}
