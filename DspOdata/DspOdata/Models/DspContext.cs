using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Dsp.Models
{    
    public class DspContext: DbContext
    {
        public DspContext(DbContextOptions<DspContext> options) : base(options)
        {

        }
        
        public DbSet<Despesas> Despesas { get; set; }

        public List<Despesas> ListarDespesas(DspContext dspContext, List<Criteria> criterias)
        {
            int mes = 0;
            int ano = 0;
            string tipo = null;
            foreach(Criteria c in criterias){
                switch (c.Field.ToLower())
                {
                    case "ano":
                        ano = Convert.ToInt32(c.Value);
                        break;
                    case "mes":
                        mes = Convert.ToInt32(c.Value);
                        break;
                    case "tipo":
                        tipo = c.Value;
                        break;
                }               
            }

            List<Despesas> dsp = new List<Despesas>();

            IQueryable<Despesas> despesas = (from d in dspContext.Despesas
                                            where (ano == 0 || d.Data.Year == ano)
                                               && (mes == 0 || d.Data.Month == mes)
                                               && (tipo == null || d.Tipo == tipo)
                                            select d);
            foreach (Despesas d in despesas)
            {
                dsp.Add(new Despesas()
                {
                    Id = d.Id,
                    Data = d.Data,
                    Tipo = d.Tipo,
                    Descricao = d.Descricao,
                    Valor = d.Valor
                });
            }           
            return dsp;
        }
        public int GetMaxId(DspContext dsp)
        {
            int r = 03;
            var id = from d in Despesas
                     select d;

            if (id.Count() == 0)
                return r;
            else
            {
                int maxId = id.Max(m => m.Id);
                return maxId;
            }           
        }
        public List<Despesas> ListSummary(DspContext dspContext, List<Criteria> criterias)
        {
            int mes = 0;
            int ano = 0;
            string tipo = null;
            foreach (Criteria c in criterias)
            {
                switch (c.Field.ToLower())
                {
                    case "ano":
                        ano = Convert.ToInt32(c.Value);
                        break;
                    case "mes":
                        mes = Convert.ToInt32(c.Value);
                        break;
                    case "tipo":
                        tipo = c.Value;
                        break;
                }
            }

            List<Despesas> dsp = new List<Despesas>();

            IQueryable<Despesas> despesas = (from d in dspContext.Despesas
                                             where (ano == 0 || d.Data.Year == ano)
                                                && (mes == 0 || d.Data.Month == mes)
                                                && (tipo == null || d.Tipo == tipo)
                                             select d);

            var a =
                from b in despesas
                group b by b.Tipo into c
                select new
                {
                    k = c.Key,
                    t = c.Sum(x => x.Valor)
                };

            foreach (var d in a)
            {
                dsp.Add(new Despesas()
                {                  
                    Tipo = d.k,
                    Descricao = "Total",
                    Valor = d.t
                });
            }           
            return dsp;
        }
        public List<Despesas> ListSearch(DspContext dspContext, List<Criteria> criterias)
        {
            int mes = 0;
            int ano = 0;
            string tipo = null;
            string desc = null;
            foreach (Criteria c in criterias)
            {
                switch (c.Field.ToLower())
                {
                    case "ano":
                        ano = Convert.ToInt32(c.Value);
                        break;
                    case "mes":
                        mes = Convert.ToInt32(c.Value);
                        break;
                    case "tipo":
                        tipo = c.Value;
                        break;
                    case "descricao":
                        desc = c.Value.ToLower();
                        break;
                }
            }

            List<Despesas> dsp = new List<Despesas>();

            IQueryable<Despesas> despesas = (from d in dspContext.Despesas
                                             where d.Descricao.ToLower().Contains(desc)
                                             select d);            

            foreach (var d in despesas)
            {
                dsp.Add(new Despesas()
                {
                    Id = d.Id,
                    Data = d.Data,
                    Tipo = d.Tipo,
                    Descricao = d.Descricao,
                    Valor = d.Valor
                });
            }
            return dsp;
        }
    }
}
