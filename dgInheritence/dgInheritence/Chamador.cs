using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace dgInheritence
{
     
    public static class Chamador
    {
        
        public static void Main()
        {
            Console.WriteLine("Usar metodo da classe base:");
            ClasseDerivadaDePublica cdp = new ClasseDerivadaDePublica("myProp",1);
            cdp.Metodo();

            Console.WriteLine();
            Console.WriteLine("Sobrescrever metodo virtual:");
            cdp.MetodoParaSobrescrever();

            Console.WriteLine();
            Console.WriteLine("Usar método protegido:");
            cdp.MetodoProtegido();

            Console.WriteLine();
            Console.WriteLine("Usar método interno:");
            cdp.MetodoInterno();

            Console.WriteLine();
            Console.WriteLine("Usar a palavra 'base': ");
            cdp.DefinirProp("Hello");
            Console.WriteLine("palavra passada: {0}", cdp.PegarProp());


            Console.WriteLine();
            Console.WriteLine("Usar Classe Sealed:");
            ClasseFechada fechada = new ClasseFechada();
            fechada.Metodo();







        }
    }
}
