
using Despesas.Dominio.Despesas;

namespace Despesas.data
{
    public interface IRepositorioDespesas
    {
        int GravarDespesa(Despesa despesa);
        void ExcluirDespesa(int codigo);
        void AprovarDespesa(int codigo);
        void ReprovarDespesa(int codigo);

    }
}
