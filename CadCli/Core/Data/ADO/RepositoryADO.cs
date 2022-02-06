using CadCli.Core.Contracts;
using CadCli.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CadCli.Core.Data.ADO
{
    public class RepositoryADO : IRepository
    {
        private string _conn;

        public RepositoryADO(IConfiguration config)
        {
            _conn = config.GetConnectionString("CadCliCon");

        }
        public void Add(Cliente cliente)
        {
            throw new NotImplementedException();
        }

        public void Delete(Cliente cliente)
        {
            throw new NotImplementedException();
        }

        public List<Cliente> Get()
        {
            var con = new SqlConnection(_conn);
            con.Open();
            var com = new SqlCommand("select * from Cliente",con);

            var dr = com.ExecuteReader();
            var cli = new List<Cliente>();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    cli.Add(new Cliente() { Id = (int)dr["Id"],
                        Idade = (int)dr["Idade"],
                        Nome = dr["Nome"].ToString()
                    });
                        



                }
            }
            



            con.Close();
            return cli;

        }

        public Cliente Get(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Cliente cliente)
        {
            throw new NotImplementedException();
        }
    }
}
