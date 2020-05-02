using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace Varsis.Data.Infrastructure
{
    public class ServiceBase : IService
    {
        private readonly ServiceCollection _serviceCollection;
        private ServiceProvider _container;
        private IServiceScope _scope;

        public ServiceBase()
        {
            _serviceCollection = new ServiceCollection();
        }

        ~ServiceBase()
        {
            if (_scope != null)
            {
                _scope.Dispose();
            }
        }

        public virtual void SetToken(string token)
        {
            throw new NotImplementedException();
        }
        public virtual Task<string> Login(Credentials credentials)
        {
            throw new NotImplementedException();
        }
        public virtual Task Logout()
        {
            throw new NotImplementedException();
        }
        async virtual public Task<bool> Create<T>() where T : EntityBase
        {
            IEntityService<T> service = FindService<T>(typeof(T));
            return await service.Create();
        }

        async virtual public Task Delete<T>(T entity) where T : EntityBase
        {
            IEntityService<T> service = FindService<T>(entity.GetType());
            await service.Delete(entity);
        }

        async virtual public Task Delete<T>(List<Criteria> criterias) where T : EntityBase
        {
            IEntityService<T> service = FindService<T>(typeof(T));
            await service.Delete(criterias);
        }

        async virtual public Task<T> Find<T>(List<Criteria> criterias) where T : EntityBase
        {
            IEntityService<T> service = FindService<T>(typeof(T));
            return await service.Find(criterias);
        }

        async virtual public Task Insert<T>(T entity) where T : EntityBase
        {
            IEntityService<T> service = FindService<T>(entity.GetType());
            await service.Insert(entity);
        }

        async virtual public Task Insert<T>(List<T> entities) where T : EntityBase
        {
            IEntityService<T> service = FindService<T>(typeof(T));
            await service.Insert(entities);
        }

        async virtual public Task<List<T>> List<T>(List<Criteria> criterias) where T : EntityBase
        {
            return await this.List<T>(criterias, -1, -1);
        }

        async virtual public Task<List<T>> List<T>(List<Criteria> criterias, long page, long size) where T : EntityBase
        {
            IEntityService<T> service = FindService<T>(typeof(T));
            return await service.List(criterias, page, size);
        }
        async virtual public Task<Varsis.Data.Infrastructure.Pagination> totalLinhas<T>(long? size, List<Criteria> criterias) where T : EntityBase
        {
            IEntityService<T> service = FindService<T>(typeof(T));
            return await service.TotalLinhas(size, criterias);
        }
        async virtual public Task Update<T>(T entity) where T : EntityBase
        {
            IEntityService<T> service = FindService<T>(typeof(T));
            await service.Update(entity);
        }

        async virtual public Task Update<T>(List<T> entities) where T : EntityBase
        {
            if (entities == null || entities.Count == 0)
            {
                throw new ArgumentNullException();
            }

            IEntityService<T> service = FindService<T>(typeof(T));
            await service.Update(entities);
        }

        private IEntityService<T> DEL_FindService<T>(Type entity) where T : EntityBase
        {
            IEntityService<T> result = null;

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => entity.IsAssignableFrom(p));
            //.Where(p => p.GetCustomAttributesData().Where(a => a.AttributeType == typeof(EntityNameAttribute) && (string)a.ConstructorArguments[0].Value == entityName).Any());

            var entityType = types.FirstOrDefault();

            if (entityType == null)
            {
                throw new NotImplementedException($"Serviço não encontrado - [{entity.FullName}]");
            }
            else
            {
                result = (IEntityService<T>)Activator.CreateInstance(entityType);
            }

            return result;
        }

        public IEntityService<T> FindService<T>() where T : EntityBase
        {
            var type = typeof(T);
            return FindService<T>(type);
        }

        private IEntityService<T> FindService<T>(Type entity) where T : EntityBase
        {
            IEntityService<T> result = null;

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name.StartsWith("varsis", StringComparison.InvariantCultureIgnoreCase))
                .SelectMany(s => s.GetTypes())
                .Where(p => p.GetInterfaces().Where(i => i.GetGenericArguments().Any(x => x.FullName == entity.FullName)).Any());

            var entityType = types.FirstOrDefault();

            if (entityType == null)
            {
                throw new NotImplementedException($"Serviço não encontrado - [{entity.FullName}]");
            }
            else
            {
                result = (IEntityService<T>)_scope.ServiceProvider.GetService(entityType);
            }

            return result;
        }

        public void ConfigureServices(Action<ServiceCollection> services)
        {
            List<Type> servicesList = listServices();

            foreach (var s in servicesList)
            {
                _serviceCollection.AddScoped(s, s);
            }

            services(_serviceCollection);

            _container = _serviceCollection.BuildServiceProvider();

            _scope = _container.CreateScope();
        }

        private List<Type> listServices()
        {
            List<Type> result = new List<Type>();

            Type entity = typeof(IEntityService<EntityBase>);

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name.StartsWith("varsis", StringComparison.InvariantCultureIgnoreCase))
                .SelectMany(s => s.GetTypes()).Where(i => i.GetInterfaces().Where(p => p.IsGenericType && p.GetGenericTypeDefinition().Name == entity.Name).Count() > 0);

            result = types.ToList();

            return result;
        }
    }
}
