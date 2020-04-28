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
                    throw new tstEx();
                i = 1 / b;

            }


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
                 throw exDZ;
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro:{e.Message}");

                //throw e;
            }
            finally
            {
                Console.WriteLine("Apos o erro em finally");

            }


            Console.ReadKey();
        }
    }

    [Serializable]
    class tstEx : Exception
    {
        public tstEx()
        {
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
}
