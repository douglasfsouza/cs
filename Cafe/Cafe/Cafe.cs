using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe
{
    public abstract class Cafe
    {
        public string Fabricante { get; set; }
     
        public abstract string ModoDeServir();

        public virtual string Formula()
        {
            return "2 colheres de acucar p/ cada 100g de cafe";
        }
        public virtual string VericarFabricante()
        {
            return Fabricante;

        }
        
    }
}
