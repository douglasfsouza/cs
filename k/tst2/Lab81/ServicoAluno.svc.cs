using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Lab07;

namespace Lab81
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServicoAluno" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ServicoAluno.svc or ServicoAluno.svc.cs at the Solution Explorer and start debugging.
    public class ServicoAluno : IServicoAluno
    {
        


        public void AddStudent(Aluno student)
        {
            var servicoAluno = new ServicoAluno();
            //servicoAluno.
        }
                

        public List<Aluno> GetAlunos(bool showPerformance)
        {
            var servicoAluno = new Lab07.ServicoAluno();
            return servicoAluno.ConsultarAluno();
           
        }
    }
}
