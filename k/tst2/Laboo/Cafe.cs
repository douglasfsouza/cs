using System;
using System.Collections.Generic;
using System.Text;

namespace Laboo
{
    public abstract class Cafe
    {
        public int Intensidade { get; set; }
        public string Tipo { get; set; }
        public int Origem { get; set; }

        public string Temperatura(int graus)
        {
            return $"{graus}º";
        }

        //virtual se implementa
        public virtual string ModoPreparo()
        {
            {
                return "Filtro de Papel";
            }
        }

        public abstract string ModoDeServir();

        public abstract void Formula();
        

    }
}
