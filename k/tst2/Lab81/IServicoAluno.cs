using Lab07;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Lab81
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServicoAluno" in both code and config file together.
    [ServiceContract]
    public interface IServicoAluno
    {
        [OperationContract]
        void AddStudent(Aluno student );
        [OperationContract]
        List<Aluno> GetAlunos(bool showPerformance);
    }
}
