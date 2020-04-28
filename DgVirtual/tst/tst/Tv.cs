using System;
using System.Collections.Generic;
using System.Text;

namespace tst
{
    public class Tv : ITv
    {
        public int Canal { get; set; }
        public void Ligar()
        {
            Console.WriteLine("Tv1 Ligada");
        }
        public void MudarCanal(int pCanal)
        {
            Canal = pCanal;
        }
        public virtual bool ConectarInternet()
        {
            Console.WriteLine("Conectado por t1");
            return true;
        }
    }


    
}
