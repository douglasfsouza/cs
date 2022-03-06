using dgs.store.Data.EF;
using dgs.store.Data.EF.Repositories;
using dgs.Store.Domain.Contracts.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store.IocDI
{
    public static class Configure
    {
        public static void SetupServices(this IServiceCollection services)
        {           
            services.AddScoped<StoreDbContext>();    
            services.AddTransient<IProdutoRepository, ProdutoRepositoryEF>();
            services.AddTransient<ICategoriaRepository, CategoriaRepositoryEF>();
            services.AddTransient<IUsuarioRepository, UsuarioRepositoryEF>();
            services.AddTransient<IPerfilRepository, PerfilRepositoryEF>();
            services.AddTransient<IUnityOfWork, UnityOfWorkEF>();
        }
    }
}
