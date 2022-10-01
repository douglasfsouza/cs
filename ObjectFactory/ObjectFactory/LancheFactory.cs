using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectFactory
{
    public class LancheFactory : LancheFactoryMethod
    {
        public override Lanche CriarLanche(string Tipo)
        {
            switch (Tipo)
            {
                case "hot":
                    return new Hotdog();
                    break;
                case "xsa":
                    return new XSalada();
                    break;
                case "bau":
                    return new Bauru();
                    break;
                default:
                    throw new ArgumentException("Lanche não previsto");
            }
        }
    }
}
