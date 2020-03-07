using System;
using System.Collections.Generic;
using System.Text;

namespace Laboo
{
    public class CafeExpresso : Cafe
    {
        public override string ModoPreparo()
        {
            return "Maquina expressa";
        }

        public override string ModoDeServir()
        {
            return "Copo de vidro";
        }        

    }
    
}
