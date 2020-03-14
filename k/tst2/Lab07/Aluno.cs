using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lab07
{
    [DataContract]
    public class Aluno
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public string CPF { get; set; }
        [DataMember]
        public List<Desempenho> Desempenho { get; set; }
    }

    
}
