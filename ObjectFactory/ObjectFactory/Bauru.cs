using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectFactory
{
    public class Bauru : Lanche
    {
        private string _Nome;
        
        public Bauru()
        {
            _Nome = "Bauru";  
            Ingredients.Add("Pão francês");
            Ingredients.Add("Presunto");
            Ingredients.Add("Queijo");
            Ingredients.Add("Tomate");
        }    
        public override string Nome { get => _Nome; }
    }
}
