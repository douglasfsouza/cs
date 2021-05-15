using System;

namespace dgGenerics
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("type safaty, no cast, no boxing / unboxing");
            Conversoes<string> s = new Conversoes<string>();
            s.UmMetodo("yes");

            Conversoes<long> l = new Conversoes<long>();
            l.UmMetodo(100);

            Conversoes<bool> b = new Conversoes<bool>();
            b.UmMetodo(true);

            SoConversoesString<Conversoes<String>> ss = new SoConversoesString<Conversoes<string>>();
            Conversoes<string> s1 = new Conversoes<string>();
            //assim não aceita:
            //Conversoes<long> s1 = new Conversoes<long>();

            ss.UmMetodo(s1);

            ss.OutroMetodo("qqc");
             
        }
    }
    class Conversoes<T>
    {
        /*T _value;
        public T this[int index]
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }*/
        public void UmMetodo(T item)
        {
            Console.WriteLine("Recebi: {0}", item);
        }          
    }

    //tambem poderia ser uma Interface, Classe, Tipo definido pelo Usuario
    // new, string ou classe base
    class SoConversoesString<T> where T : Conversoes<string>
    {
        public void UmMetodo(T item)
        {
            Console.WriteLine("SoConversoesString Recebeu: {0}", item);

        }
        public void OutroMetodo(string s)
        {
            Console.WriteLine("Outro metodo recebeu: {0}", s);

        }
        
    }
}
