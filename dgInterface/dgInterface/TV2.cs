using System;
using System.Collections.Generic;
using System.Text;

namespace dgInterface
{
    class TV2 : TV
    {
        public void AcessarProtegido()
        {
            bool b = Protegido();
        }
    }
}
