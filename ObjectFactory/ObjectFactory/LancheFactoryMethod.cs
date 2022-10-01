using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectFactory
{
    public abstract class LancheFactoryMethod
    {
        public abstract Lanche CriarLanche(string Tipo);
    }
}
