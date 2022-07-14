using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "Service1" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione Service1.svc ou Service1.svc.cs no Gerenciador de Soluções e inicie a depuração.
    public class Service1 : IClientService
    {
        public string Insert(Cliente client)
        {
            string result = string.Empty;
            if (client == null)
            {
                throw new ArgumentException("client");
            }
            string stringConnection = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GTIDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection connection = new SqlConnection(stringConnection);
            connection.Open();
            string fields = "CPF,Nome,RG,DataExpedicao,OrgaoExpedicao,UF,DataNascimento,Sexo,EstadoCivil,EnderecoCEP,EnderecoLogradouro,EnderecoNumero,EnderecoComplemento,EnderecoBairro,EnderecoCidade,EnderecoUF";
            string values = "@CPF,@Nome,@RG,@DataExpedicao,@OrgaoExpedicao,@UF,@DataNascimento,@Sexo,@EstadoCivil,@EnderecoCEP,@EnderecoLogradouro,@EnderecoNumero,@EnderecoComplemento,@EnderecoBairro,@EnderecoCidade,@EnderecoUF";
            SqlCommand cmd = new SqlCommand($"insert into dbo.clientes ({fields}) values({values})",connection);

            cmd.Parameters.AddWithValue("@CPF", client.CPF);
            cmd.Parameters.AddWithValue("@Nome", client.Nome);
            cmd.Parameters.AddWithValue("@RG", client.RG);
            if (client.DataExpedicao == null)
            {
                cmd.Parameters.AddWithValue("@DataExpedicao", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@DataExpedicao", client.DataExpedicao);
            }
            cmd.Parameters.AddWithValue("@OrgaoExpedicao", client.OrgaoExpedicao);
            cmd.Parameters.AddWithValue("@UF", client.UF);
            cmd.Parameters.AddWithValue("@DataNascimento", client.DataNascimento);
            cmd.Parameters.AddWithValue("@Sexo", client.Sexo);
            cmd.Parameters.AddWithValue("@EstadoCivil", client.EstadoCivil);
            cmd.Parameters.AddWithValue("@EnderecoCEP", client.EnderecoCEP);
            cmd.Parameters.AddWithValue("@EnderecoLogradouro", client.EnderecoLogradouro);
            cmd.Parameters.AddWithValue("@EnderecoNumero", client.EnderecoNumero);
            cmd.Parameters.AddWithValue("@EnderecoComplemento", client.EnderecoComplemento);
            cmd.Parameters.AddWithValue("@EnderecoBairro", client.EnderecoBairro);
            cmd.Parameters.AddWithValue("@EnderecoCidade", client.EnderecoCidade);
            cmd.Parameters.AddWithValue("@EnderecoUF", client.EnderecoUF);

            int exec = cmd.ExecuteNonQuery();

            result = exec == 1 ? "Cliente incluído com sucesso." : "Falha ao inserir o cliente";
            
            return result;
        }

        public string Update(Cliente client)
        {
            string result = string.Empty;
            if (client == null)
            {
                throw new ArgumentException("client");
            }
            string stringConnection = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GTIDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection connection = new SqlConnection(stringConnection);
            connection.Open();
            string sql = "CPF=@CPF,Nome=@Nome,RG=@RG,DataExpedicao=@DataExpedicao,OrgaoExpedicao=@OrgaoExpedicao,UF=@UF,DataNascimento=@DataNascimento,Sexo=@Sexo,EstadoCivil=@EstadoCivil,EnderecoCEP=@EnderecoCEP,EnderecoLogradouro=@EnderecoLogradouro,EnderecoNumero=@EnderecoNumero,EnderecoComplemento=@EnderecoComplemento,EnderecoBairro=@EnderecoBairro,EnderecoCidade=@EnderecoCidade,EnderecoUF=@EnderecoUF";
            SqlCommand cmd = new SqlCommand($"update dbo.clientes set {sql} where Id=@Id", connection);
            
            cmd.Parameters.AddWithValue("@Id", client.Id);
            cmd.Parameters.AddWithValue("@CPF", client.CPF);
            cmd.Parameters.AddWithValue("@Nome", client.Nome);
            cmd.Parameters.AddWithValue("@RG", client.RG);
            if (client.DataExpedicao == null)
            {                
                cmd.Parameters.AddWithValue("@DataExpedicao", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@DataExpedicao", client.DataExpedicao);
            }
            cmd.Parameters.AddWithValue("@OrgaoExpedicao", client.OrgaoExpedicao);
            cmd.Parameters.AddWithValue("@UF", client.UF);
            cmd.Parameters.AddWithValue("@DataNascimento", client.DataNascimento);
            cmd.Parameters.AddWithValue("@Sexo", client.Sexo);
            cmd.Parameters.AddWithValue("@EstadoCivil", client.EstadoCivil);
            cmd.Parameters.AddWithValue("@EnderecoCEP", client.EnderecoCEP);
            cmd.Parameters.AddWithValue("@EnderecoLogradouro", client.EnderecoLogradouro);
            cmd.Parameters.AddWithValue("@EnderecoNumero", client.EnderecoNumero);
            cmd.Parameters.AddWithValue("@EnderecoComplemento", client.EnderecoComplemento);
            cmd.Parameters.AddWithValue("@EnderecoBairro", client.EnderecoBairro);
            cmd.Parameters.AddWithValue("@EnderecoCidade", client.EnderecoCidade);
            cmd.Parameters.AddWithValue("@EnderecoUF", client.EnderecoUF);

            int exec = cmd.ExecuteNonQuery();

            result = exec == 1 ? "Cliente alterado com sucesso." : "Falha ao atualizar o cliente";

            return result;
        }

        public string Delete(int Id)
        {
            string result = string.Empty;
            if (Id == 0)
            {
                throw new ArgumentException("client");
            }
            string stringConnection = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GTIDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection connection = new SqlConnection(stringConnection);
            connection.Open();
            SqlCommand cmd = new SqlCommand($"delete dbo.clientes where Id=@Id", connection);
            cmd.Parameters.AddWithValue("@Id", Id);

            int exec = cmd.ExecuteNonQuery();

            result = exec == 1 ? "Cliente excluído com sucesso." : "Falha ao excluir o cliente";

            return result;
        }

        public GetCliente GetInfo()
        {
            GetCliente client = new GetCliente();
            string stringConnection = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GTIDb;Integrated Security=False;Connect Timeout=300;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection connection = new SqlConnection(stringConnection);

            connection.Open();

            string query = "Select * from dbo.clientes";

            SqlCommand command = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable data = new DataTable();
            adapter.Fill(data);
            client.ClienteTable = data;

            return client;
        }

        public DataSet GetClienteRecords()
        {
            DataSet ds = new DataSet();
            try
            {
                string stringConnection = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GTIDb;Integrated Security=False;Connect Timeout=300;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                SqlConnection connection = new SqlConnection(stringConnection);

                connection.Open();

                string query = "Select * from dbo.clientes";

                SqlDataAdapter sda = new SqlDataAdapter(query, connection);
                sda.Fill(ds);
            }
            catch (FaultException ex)
            {
                throw new FaultException<string>("Erro:" + ex);
            }
            return ds;
        }

        public DataSet SearchClient(Cliente cliente)
        {
            DataSet ds = new DataSet();
            try
            {
                string stringConnection = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GTIDb;Integrated Security=False;Connect Timeout=300;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                SqlConnection connection = new SqlConnection(stringConnection);

                connection.Open();

                string query = "Select * from dbo.clientes Where Id=@Id";

                SqlDataAdapter sda = new SqlDataAdapter(query, connection);
                sda.SelectCommand.Parameters.AddWithValue("@Id", cliente.Id);
                sda.Fill(ds);
            }
            catch (FaultException ex)
            {
                throw new FaultException<string>("Erro:" + ex);
            }
            return ds;
        }

        public async Task<string> GetCep(string cep)
        {
            string result = string.Empty;
            try
            {
                string url = $"http://viacep.com.br/ws/{cep}/json";
                HttpClient httpClient = new HttpClient();
                result = await httpClient.GetStringAsync(url);
            }
            catch
            {
                throw;            
            }
            
            return result;
         }
    }
}
