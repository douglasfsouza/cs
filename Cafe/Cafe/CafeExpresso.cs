using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe
{
    public class CafeExpresso : Cafe
    {
        public override string ModoDeServir()
        {
            return "maquina";
         //   throw new NotImplementedException();
        }
    }
}
