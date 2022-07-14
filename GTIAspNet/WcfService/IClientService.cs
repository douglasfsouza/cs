using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract]
    public interface IClientService
    {
        [OperationContract]
        string Insert(Cliente client);

        [OperationContract]
        string Update(Cliente client);

        [OperationContract]
        string Delete(int Id);

        [OperationContract]
        GetCliente GetInfo();

        [OperationContract]
        DataSet GetClienteRecords();

        [OperationContract]
        DataSet SearchClient(Cliente cliente);

        [OperationContract]
        Task<string> GetCep(string cep);
    }
    

    [DataContract]
    public class Cliente
    {
        int _Id = 0;
        string _CPF = string.Empty;
        string _Nome = string.Empty;
        string _RG = string.Empty;
        DateTime? _DataExpedicao = DateTime.Today;
        string _OrgaoExpedicao = string.Empty;
        string _UF = string.Empty;
        DateTime _DataNascimento = DateTime.Today;
        string _Sexo = string.Empty;
        string _EstadoCivil = string.Empty;
        string _EnderecoCEP = string.Empty;
        string _EnderecoLogradouro = string.Empty;
        string _EnderecoNumero = string.Empty;
        string _EnderecoComplemento = string.Empty;
        string _EnderecoBairro = string.Empty;
        string _EnderecoCidade = string.Empty;
        string _EnderecoUF = string.Empty;

        [DataMember]
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        [DataMember]
        public string CPF
        {
            get { return _CPF; }
            set { _CPF = value; }
        }

        [DataMember]
        public string Nome
        {
            get { return _Nome; }
            set { _Nome = value; }
        }

        [DataMember]
        public string RG
        {
            get { return _RG; }
            set { _RG = value; }
        }

        [DataMember]
        public DateTime? DataExpedicao
        {
            get { return _DataExpedicao; }
            set { _DataExpedicao = value; }
        }

        [DataMember]
        public string OrgaoExpedicao
        {
            get { return _OrgaoExpedicao; }
            set { _OrgaoExpedicao = value; }
        }

        [DataMember]
        public string UF
        {
            get { return _UF; }
            set { _UF = value; }
        }

        [DataMember]
        public DateTime DataNascimento
        {
            get { return _DataNascimento; }
            set { _DataNascimento = value; }
        }

        [DataMember]
        public string Sexo
        {
            get { return _Sexo; }
            set { _Sexo = value; }
        }

        [DataMember]
        public string EstadoCivil
        {
            get { return _EstadoCivil; }
            set { _EstadoCivil = value; }
        }

        [DataMember]
        public string EnderecoCEP
        {
            get { return _EnderecoCEP; }
            set { _EnderecoCEP = value; }
        }

        [DataMember]
        public string EnderecoLogradouro
        {
            get { return _EnderecoLogradouro; }
            set { _EnderecoLogradouro = value; }
        }

        [DataMember]
        public string EnderecoNumero
        {
            get { return _EnderecoNumero; }
            set { _EnderecoNumero = value; }
        }

        [DataMember]
        public string EnderecoComplemento
        {
            get { return _EnderecoComplemento; }
            set { _EnderecoComplemento = value; }
        }

        [DataMember]
        public string EnderecoBairro
        {
            get { return _EnderecoBairro; }
            set { _EnderecoBairro = value; }
        }

        [DataMember]
        public string EnderecoCidade
        {
            get { return _EnderecoCidade; }
            set { _EnderecoCidade = value; }
        }

        [DataMember]
        public string EnderecoUF
        {
            get { return _EnderecoUF; }
            set { _EnderecoUF = value; }
        }

    }

    [DataContract]
    public class GetCliente
    {
        [DataMember]
        public DataTable ClienteTable
        {
            get;
            set;
        }
    }

}
