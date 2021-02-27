using System;
using System.Runtime.Serialization;

namespace DgTry
{
    class Program
    {
        static void Main(string[] args)
        {
            int i;
            int b = 0;
           

            var exNull = new NullReferenceException("O valor está nulo");
            var exDZ = new NullReferenceException("Conseguiu dividir por zero ?");
            Console.ReadKey();
            try
            {
                if (b == 0)
                    throw new DivideByZeroException();
                    //throw new tstEx();
                    //throw exDZ;
                
                i = 1 / b;

            }

            // Primeiro as exceptions mais específicas
            catch (tstEx e)
            {
                Console.WriteLine("Tentou dividir por zero e caiu em tstEx");

            }
            catch (NullReferenceException)
            {
                //throw exNull;
            }

            catch (DivideByZeroException e)
            {
                Console.WriteLine("Tentou dividir por zero e caiu em DivideByZeroException");
                //throw; //retorna o erro novamente ao chamador                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro:{e.Message}");

                //throw e;
            }
            finally
            {
                Console.WriteLine("Finally, esse bloco será executado sempre, dando erro ou não");

            }

            Console.ReadKey();

            Carro carro = new Carro();
            carro.Dirigir();
            carro.Estacionar();
        }
    }

    [Serializable]
    class tstEx : Exception
    {
        public tstEx()
        {
            Console.WriteLine("Dentro da classe tstEx");
        }

        public tstEx(string message) : base(message)
        {
            Console.WriteLine("Dentro da classe tstEx");
        }

        public tstEx(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected tstEx(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    public class Carro
    {
        public void Dirigir()
        {
            Console.WriteLine("Dirigindo");
        }
        public void Estacionar()
        {
            //Se alguem chamar, dá erro porque ainda não está implementado
            //Sem isso, o chamador não veria o erro e daria como certo sem fazer nada

             throw new NotImplementedException();
        }


    }
}
