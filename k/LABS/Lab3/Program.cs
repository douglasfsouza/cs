using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            //Pessoas();
            //Dicionario();
            //Filas(); //TODO: Criar Metodo Filas
            //xLinq();
            EventosDelegate();
            Console.ReadKey();//DEBIT: Corrigir urgentemente

        }

        private static void EventosDelegate()
        {
            Radar rsp = new Radar();
            rsp.isMovel = false;
            rsp.LimiteVelocidadePermitida = 50;
            rsp.via = "Bandeirantes, 65";
            rsp.EventoGerarMulta += Rsp_EventoGerarMulta;

            while (true)
            {
                Random placa = new Random();
                Random velocidade = new Random();
                string placaCarro = $"$AAA{placa.Next(0, 9999).ToString().PadLeft(4, '0')}";
                int velcarro = velocidade.Next(30, 60);
                if ( rsp.ValidarVelocidade(velcarro, placaCarro, rsp.via))
                {
                    Console.WriteLine($"{placaCarro} - {velcarro}");

                }


                
                System.Threading.Thread.Sleep(2000);
            }


            
        }

        private static bool Rsp_EventoGerarMulta(string placa, int velocidade, string via)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Multa SP {velocidade} ");
            Console.ForegroundColor = ConsoleColor.White;
            return true;
        }

        private static void xLinq()
        {
            List<Pessoa> pessoas = new List<Pessoa>();
            Pessoa pes = new Pessoa();

            pes.Nome = "Dg";
            pes.Idade = 43;
            pessoas.Add(pes);

            pes = new Pessoa();
            pes.Nome = "An";
            pes.Idade = 41;
            pessoas.Add(pes);

            List<Pessoa> comA = (from A in pessoas
                                where A.Nome.ToUpper().Contains("A")
                                select A).ToList();
            //lambda
            List<Pessoa> comLambda = pessoas.Where(x => x.Nome.ToUpper().Contains("A")).ToList();

            Console.WriteLine("Com linq");
            foreach(Pessoa p in comA)
            {
                Console.WriteLine(p.Nome);
            }

            Console.WriteLine("Com lambda");

            foreach (Pessoa p in comLambda)
            {
                Console.WriteLine(p.Nome);
            }
            Console.ReadKey();            

        }

        private static void Filas()
        {
            Queue<string> f = new Queue<string>();
            f.Enqueue("primeiro");
            f.Enqueue("segundo");
            f.Enqueue("terceiro");
            f.Dequeue();            

            //peek -> passa para o proximo
            
            foreach (var i in f)
            {
                Console.WriteLine(i);
                
            }
            
        }

        private static void Dicionario()
        {
            Dictionary<int, string> pessoas = new Dictionary<int, string>();

            pessoas.Add(43, "Dg");
            pessoas.Add(41, "And");

            Console.WriteLine(pessoas[41]);
            foreach(var i in pessoas)
            {
                Console.WriteLine(i.Key);
                Console.WriteLine(i.Value);
            }

        }

        private static void Pessoas()
        {
            List<Pessoa> pessoas = new List<Pessoa>();

            Pessoa nova = new Pessoa();
            nova.Nome = "Fabio Jr";
            nova.Idade = 59;
            pessoas.Add(nova);

            nova = new Pessoa();
            nova.Nome = "Julia";
            nova.Idade = 19;
            pessoas.Add(nova);


            foreach (Pessoa i in pessoas)
            {
                Console.WriteLine(i.Nome);
            }

            Console.WriteLine(pessoas.Sum(x => x.Idade));

            Pessoa pessoaMaisNova = (from A in pessoas
                                     orderby A.Idade descending
                                     select A).First();
            Console.WriteLine(pessoaMaisNova.Idade);
        }
    }
}
