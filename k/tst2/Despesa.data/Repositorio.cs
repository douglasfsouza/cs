using Despesas.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Despesa.data
{
    public class Repositorio<T> : IRepositorioGenerico<T> where T : BaseEntity

    {
        public void Delete()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Projeto> GetAll()
        {
            throw new NotImplementedException();
        }

        public Projeto GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Insert()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        IEnumerable<T> IRepositorioGenerico<T>.GetAll()
        {
            throw new NotImplementedException();
        }

        T IRepositorioGenerico<T>.GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
