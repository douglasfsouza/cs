using System;
using System.Collections.Generic;
using System.Text;

namespace dgInheritence
{
    public abstract class ClasseBasePublica
    {
        private string _prop;

        public ClasseBasePublica(string prop)
        {
            _prop = prop;
            Console.WriteLine("A classe base recebeu {0}", prop);
        }
        public void Metodo()
        {
            Console.WriteLine("Esta classe pode ser herdada em qualquer classe de qualquer assembly ");
        }
        public virtual void MetodoParaSobrescrever()
        {
            Console.WriteLine("Esta linha foi escrita na classe base");
        }

        protected void MetodoProtegido()
        {
            Console.WriteLine("Este metodo é protegido");
        }

        private void MetodoPrivado()
        {
            Console.WriteLine("Este metodo é privado, não pode ser herdado");
        }

        internal void MetodoInterno()
        {
            Console.WriteLine("Este metodo é interno, não pode ser herdado em outro assembly\r\nNão precisa ser sobrescrito");
        }

        public string Prop
        {
            get
            {
                return _prop;
            }
            set
            {
                _prop = value;
            }
        }

    }
}
