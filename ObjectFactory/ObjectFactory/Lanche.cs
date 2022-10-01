using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ObjectFactory
{
    public abstract class Lanche
    {
        public abstract string Nome { get; }
        public ArrayList Ingredients = new ArrayList();
    }
}
