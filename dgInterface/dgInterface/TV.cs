using System;
using System.Collections.Generic;
using System.Text;

namespace dgInterface
{
    public class TV : ITV
    {
        public int Canal { get; set; }
        public void MudarCanal(int canal)
        {
            Canal = canal;

        }
        protected bool Protegido()
        {
            Console.WriteLine("Acessou o metodo protegido");
            return true;
        }
    }
}
