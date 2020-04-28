using System;
using System.Collections.Generic;
using System.Text;

namespace tst
{
    public class Tv2 : Tv
    {
        public int Canal { get; set; }
        public void Ligar()
        {
            Console.WriteLine("Tv2 Ligada");
        }
        public void MudarCanal(int pCanal)
        {
            Canal = pCanal;

        }
        
        public override bool ConectarInternet()
        {
            Console.WriteLine("Conectado por tv2");
            return true;
        }
    }
}
