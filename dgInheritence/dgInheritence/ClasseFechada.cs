using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace dgInheritence
{
    public sealed class ClasseFechada
    {
        public void Metodo()
        {
            Console.WriteLine("Classe Sealed não pode ser herdada");
        }
    }
}
