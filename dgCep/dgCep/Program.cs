using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dgCep
{
    class Program
    {
        static void Main(string[] args)
        {
            bool sair = false;
            while (!sair)
            {
                Console.WriteLine("Informe o cep:");
                string cepi = Console.ReadLine();
                if (cepi == string.Empty)
                {
                    sair = true;
                }
                else
                {
                    Task<List<Cep>> cep = Getcep(cepi);

                    //Console.WriteLine(cep.Result);
                    Console.Clear();
                    try
                    {
                        foreach (var item in cep.Result)
                        {
                            Console.WriteLine($"Cidade: {item.localidade}-{item.uf}");
                            Console.WriteLine($"Bairro: {item.bairro}");
                            Console.WriteLine($"Rua: {item.logradouro}");
                            System.Threading.Thread.Sleep(5000);
                            Console.Clear();

                        }

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Nao encontrado");      
                    }                                     
                    
                }               

            }            
        }
        private static async Task<List<Cep>> Getcep(object cep)
        {
            List<Cep> ceps = new List<Cep>();
            string u = $"http://viacep.com.br/ws/{cep}/json";
            HttpClient c = new HttpClient();
            string rr = await c.GetStringAsync(u);
            Cep cepd = JsonConvert.DeserializeObject<Cep>(rr);
            ceps.Add(cepd);
            return ceps;
        }

    }
    
}
