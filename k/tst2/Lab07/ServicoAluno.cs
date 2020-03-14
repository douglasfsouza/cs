using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab07
{
    public class ServicoAluno
    {
        private EscolaContexto contexto = new EscolaContexto("EscolaContexto");
        public List<Aluno> ConsultarAluno()
        {            
           
                contexto.Configuration.LazyLoadingEnabled = true;

                return contexto.Alunos.Include("Desempenho").ToList();
                
        }

        public  void CriarAluno(string cpf, string nome)
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
