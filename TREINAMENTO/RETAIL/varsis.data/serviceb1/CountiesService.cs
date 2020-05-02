using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.IO;

namespace Varsis.Data.Serviceb1
{
    public class CountiesService : IEntityService<Counties>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;

        readonly List<Counties> DataSource;


        public CountiesService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;

            DataSource = this.LoadDataSource().Result;
        }

        async private Task<List<Counties>> LoadDataSource()
        {
            var provider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), "Varsis.Data.Serviceb1");
            List<Counties> result = new List<Counties>();

            using (var stream = provider.GetFileInfo("DataFile/OCNT.data").CreateReadStream())
            using(StreamReader reader = new StreamReader(stream, Encoding.Default))
            {
                // Ignora a linha do cabeçalho
                string line = await reader.ReadLineAsync();

                while (!reader.EndOfStream)
                {
                    line = await reader.ReadLineAsync();
                    if (line.Trim() != string.Empty)
                    {
                        string[] row = line.Split(';');

                        try
                        {
                            Counties c = new Counties()
                            {
                                AbsId = Convert.ToInt64(row[1].Trim().Replace(".","")),
                                Code = row[2]?.Trim() ?? string.Empty,
                                Country = row[3]?.Trim() ?? string.Empty,
                                State = row[4]?.Trim() ?? string.Empty,
                                Name = row[5]?.Trim() ?? string.Empty,
                                TaxZone = row[6]?.Trim() ?? string.Empty,
                                IBGECode = row[7]?.Trim() ?? string.Empty,
                                GIACode = row[8]?.Trim() ?? string.Empty
                            };

                            result.Add(c);
                        }
                        catch(Exception ex)
                        {
                            var msg = ex.Message;
                        }
                    }
                }
            }

            return result;
        }

        public Task<bool> Create()
        {
            throw new NotSupportedException();
        }

        public Task Delete(Counties entity)
        {
            throw new NotSupportedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotSupportedException();
        }

        async public Task<Counties> Find(List<Criteria> criterias)
        {
            Counties entity = null;

            IEnumerable<Counties> q;

            switch(criterias[0].Field.ToLower())
            {
                case "name":
                    q = from p in DataSource where p.Name == criterias[0].Value select p;
                    break;

                case "code":
                default:
                    q = from p in DataSource where p.Code == criterias[0].Value select p;
                    break;

            }

            entity = q.First();

            return entity;
        }

        public Task Insert(Counties entity)
        {
            throw new NotSupportedException();
        }

        public Task Insert(List<Counties> entities)
        {
            throw new NotSupportedException();
        }

        async public Task<Varsis.Data.Infrastructure.Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            return new Varsis.Data.Infrastructure.Pagination();
        }

        async public Task<List<Counties>> List(List<Criteria> criterias, long page, long size)
        {
            return DataSource;
        }

        public Task Update(Counties entity)
        {
            throw new NotSupportedException();
        }

        public Task Update(List<Counties> entities)
        {
            throw new NotSupportedException();
        }
    }
}
