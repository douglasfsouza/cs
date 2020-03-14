using System.Runtime.Serialization;

namespace Lab07
{
    [DataContract(IsReference =true)]
    public class Desempenho
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Ano { get; set; }
        [DataMember]
        public string Bimestre { get; set; }
        [DataMember]
        public string Mareria { get; set; }
        [DataMember]
        public decimal Nota { get; set; }
        [DataMember]
        public Aluno Aluno { get; set; }
    }
}