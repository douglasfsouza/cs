using System;

namespace dgValidateSetProperty
{
    class Program
    {
        static void Main(string[] args)
        {
            Validar v = new Validar();
            try
            {
                v.data = DateTime.Today.AddDays(1);
            }
            catch (Exception e)
            {

                Console.WriteLine("Erro ao definir data. Erro: {0}", e.Message);
            }            

            Console.WriteLine("Data: {0}",v.data);
        }
    }
}
