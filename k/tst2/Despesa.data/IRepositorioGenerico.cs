using Despesas.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Despesa.data
{
    public interface IRepositorioGenerico<T> where T : BaseEntity
    {
        void Insert();
        void Delete();
        void Update();
        IEnumerable<T> GetAll();
        T GetById(int id);

    }
}
