using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab07
{
    class Program
    {
        static EscolaContexto contexto = new EscolaContexto("EscolaContexto");
        static void Main(string[] args)
        {
            //CriarAluno("274.754.958.96","Natalia Padoca");


            //CriarDesempenho();
            AlterNota(9.8m, "1", "274.754.958.96", "Matematica");
            ConsultarAluno();
            //ConsultarDesempenho();

            Console.ReadKey();


        }

        private static void AlterNota(decimal nota, string bim, string cpf, string mater)
        {
            contexto.Database.Log = Console.WriteLine;
            var desempenho = (from des in contexto.Desempenho
                              where des.Aluno.CPF == cpf
                              && des.Mareria == mater
                              && des.Bimestre == bim
                              select des).FirstOrDefault();
            desempenho.Nota = nota;
            contexto.SaveChanges();
        }        


        private static void CriarDesempenho()
        {
            contexto.Database.Log = Console.WriteLine;
            List<Desempenho> d = new List<Desempenho>();

            Aluno Nat = (from na in contexto.Alunos
                         where na.CPF == "274.754.958.96"
                         select na).FirstOrDefault();

            d.Add(new Desempenho()
            {
                Ano = 2020,
                Bimestre = "1",
                Aluno = Nat,
                Mareria = "Matematica",
                Nota = 10
            });

            d.Add(new Desempenho()
            {
                Ano = 2020,
                Bimestre = "2",
                Aluno = Nat,
                Mareria = "Matematica",
                Nota = 9
            });

            d.Add(new Desempenho()
            {
                Ano = 2020,
                Bimestre = "3",
                Aluno = Nat,
                Mareria = "Matematica",
                Nota = 10
            });

            d.Add(new Desempenho()
            {
                Ano = 2020,
                Bimestre = "4",
                Aluno = Nat,
                Mareria = "Matematica",
                Nota = 9.5m
            });

            d.Add(new Desempenho()
            {
                Ano = 2020,
                Bimestre = "1",
                Aluno = Nat,
                Mareria = "Historia",
                Nota = 8
            });

            d.Add(new Desempenho()
            {
                Ano = 2020,
                Bimestre = "2",
                Aluno = Nat,
                Mareria = "Historia",
                Nota = 8.5m
            });

            d.Add(new Desempenho()
            {
                Ano = 2020,
                Bimestre = "3",
                Aluno = Nat,
                Mareria = "Historia",
                Nota = 9
            });

            d.Add(new Desempenho()
            {
                Ano = 2020,
                Bimestre = "4",
                Aluno = Nat,
                Mareria = "Historia",
                Nota = 9.5m
            });

            contexto.Desempenho.AddRange(d);
            contexto.SaveChanges();
        }

        private static void ConsultarAluno()
        {
            //contexto.Database.Log = Console.WriteLine;
            contexto.Configuration.LazyLoadingEnabled = true;

            foreach (var aluno in contexto.Alunos.Include("Desempenho"))
            {
                Console.WriteLine($"{aluno.Nome} - {aluno.CPF}");
                Console.WriteLine("ANO \t - BIM.\t MATERIA \t - NOTA");
                foreach (var des in aluno.Desempenho)
                {
                    Console.WriteLine($"{des.Ano}\t - {des.Bimestre}\t - {des.Mareria}\t - {des.Nota}\t ");
                }                                                               
            }
            
        }

        /*private static void ConsultarAlunoColunas()
        {
            //contexto.Database.Log = Console.WriteLine;
            contexto.Configuration.LazyLoadingEnabled = true;

            foreach (var ano in contexto.Alunos.de)

            

            foreach (var aluno in contexto.Alunos.Include("Desempenho"))
            {
                Console.WriteLine($"{aluno.Nome} - {aluno.CPF}");
                Console.WriteLine("ANO \t - BIM.\t MATERIA \t - NOTA");
                foreach ( var ano in aluno.an)


                var notas1 = aluno.Desempenho
                    .Where(anoc => anoc.Ano == ano.ano)
                    .Groupby(g => g.Materia )
                    .Select(x => new
                    {
                        Materia = x.Key,
                        PriB = x.Where(b)
                    }
                    )

                foreach (var des in aluno.Desempenho)
                {
                    Console.WriteLine($"{des.Ano}\t - {des.Bimestre}\t - {des.Mareria}\t - {des.Nota}\t ");
                }
            }

        }*/

        private static void CriarAluno(string cpf, string nome)
        {
            Aluno Anita = new Aluno()
            {
                CPF = cpf,
                Nome = nome
            };
            

            contexto.Alunos.Add(Anita);
            contexto.SaveChanges();
        }
    }
}
