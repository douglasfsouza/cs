using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectFactory
{
    public class Hotdog : Lanche
    {
        private string _Nome;
        public Hotdog()
        {
            _Nome = "Hotdog";
            Ingredients.Add("Pão de hotdog");
            Ingredients.Add("Salsicha");
            Ingredients.Add("Purê");
            Ingredients.Add("Maioneze");
            Ingredients.Add("Catchup");
            Ingredients.Add("Milho verde");
            Ingredients.Add("Batata palha");                
        }
        public override string Nome { get => _Nome; }
    }
}
