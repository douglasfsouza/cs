using System;
using System.Collections.Generic;
using System.Text;
using Despesas.data;

namespace Despesa.data
{
    public class RepositorioDespesaSQL : IRepositorioDespesas
    {      

        int IRepositorioDespesas.GravarDespesa(Despesa despesa)
        {
            throw new NotImplementedException();
        }

        void IRepositorioDespesas.ExcluirDespesa(int codigo)
        {
            throw new NotImplementedException();
        }

        void IRepositorioDespesas.AprovarDespesa(int codigo)
        {
            throw new NotImplementedException();
        }

        void IRepositorioDespesas.ReprovarDespesa(int codigo)
        {
            throw new NotImplementedException();
        }
    }
}
