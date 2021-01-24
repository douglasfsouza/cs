using System;
using Oracle.ManagedDataAccess.Client;

namespace dgOracleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.0.117)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=RMS)));User Id=varsis;Password=pwdv4r51s123";
            OracleConnection con = new OracleConnection(connectionString);
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "Select * from carone.AA2CPARA";

            OracleDataReader reader = cmd.ExecuteReader();

            string codigo = "Codigo";
            string acesso = "Acesos";
            string conteudo = "Conteudo";
            Console.WriteLine($"{codigo}\t{acesso}\t{conteudo}\n");            

            while (reader.Read())
            {
                try
                {
                    codigo = reader.GetString(0);
                    acesso = string.IsNullOrEmpty(reader.GetString(1)) ? "Vazio" : reader.GetString(1);
                    conteudo = string.IsNullOrEmpty(reader.GetString(2)) ? "Vazio" : reader.GetString(2);

                    Console.WriteLine($"{codigo}\t{acesso}\t{conteudo}\n");
                }
                catch (Exception e)
                {                    
                }                
            }
            con.Clone();
        }
    }
}
