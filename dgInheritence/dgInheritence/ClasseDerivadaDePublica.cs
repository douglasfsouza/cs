using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace dgInheritence
{
    public class ClasseDerivadaDePublica: ClasseBasePublica
    {
        private string _prop;
        private int _qqc;
        public ClasseDerivadaDePublica(string prop, int qqc) : base(prop)
        {
            //Demonstrando como chama o construtor da base explicitamente
            _prop = prop;
            _qqc = qqc;
            Console.WriteLine("A classe herdada tambem recebeu: {0} e {1}", prop, qqc);
        }
        sealed public override void MetodoParaSobrescrever()
        {
            Console.WriteLine("Metodo Virtual sobrescrito, a palavra override é opcional e pode ser substuida por new\r\nSealed é para impedir que seja sobrescrito novamente em outra sub herança");
        }
        
        public void MetodoProtegido()
        {
            Console.WriteLine("Sobrescrevendo metodo protegido, estes precisam ser implementados");
        }

        public void DefinirProp(string prop)
        {
            base.Prop = prop;
        }
        public string PegarProp()
        {
            return base.Prop;
        }
        
    }
}
