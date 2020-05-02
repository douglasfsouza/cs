using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Varsis.Data.Infrastructure
{
    public interface IService 
    {
        public void SetToken(string token);
        public Task<string> Login(Credentials credentials);
        public Task Logout();
        public Task<bool> Create<T>() where T : EntityBase;
        public Task Insert<T>(T entity) where T : EntityBase;
        public Task Insert<T>(List<T> entities) where T : EntityBase;
        public Task Update<T>(T entity) where T : EntityBase;
        public Task Update<T>(List<T> entities) where T : EntityBase;
        public Task Delete<T>(T entity) where T : EntityBase;
        public Task Delete<T>(List<Criteria> criterias) where T : EntityBase;
        public Task<List<T>> List<T>(List<Criteria> criterias) where T : EntityBase;
        public Task<List<T>> List<T>(List<Criteria> criterias, long page, long size) where T : EntityBase;
        public Task<T> Find<T>(List<Criteria> criterias) where T : EntityBase;
        public void ConfigureServices(Action<ServiceCollection> services);
    }
}
