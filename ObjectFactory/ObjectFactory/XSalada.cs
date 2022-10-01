using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectFactory
{
    public class XSalada : Lanche
    {
        private string _Nome;
        public XSalada()
        {
            _Nome = "X-Salada";
            Ingredients.Add("Pão de hamburguer");
            Ingredients.Add("Hamburguer");
            Ingredients.Add("Maioneze");
            Ingredients.Add("Alface");
            Ingredients.Add("Presunto");
            Ingredients.Add("Queijo");
        }
        public override string Nome { get => _Nome; }
    }
}
