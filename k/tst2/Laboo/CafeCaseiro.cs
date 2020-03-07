using System;
using System.Collections.Generic;
using System.Text;

namespace Laboo
{
    class CafeCaseiro : Cafe
    {
        public override string ModoDeServir()
        {
            return "na xicara";
            //throw new NotImplementedException();
        }
        public sealed override void Formula()
        {
            string formula = "";
        }
    }
}
