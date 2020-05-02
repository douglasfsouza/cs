using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Varsis.Data.Infrastructure;
using Varsis.Data.Serviceb1;
using Varsis.Data.Serviceb1.Integration;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Varsis.Data.Model.Integration;

namespace testing
{
    public class CfopToUsageMapTesting
    {
        IConfiguration _configuration;
        Service _service;
        string _token;
        CfopToUsageMap _map;

        [OneTimeSetUp]
        public void Setup()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(".\\appSettingsB1.json");

            ILoggerFactory factory = new LoggerFactory();
            ILogger<Service> logger = factory.CreateLogger<Service>();

            _configuration = builder.Build();
            _service = new Service(_configuration, logger);

            doLogin();

            _map = new CfopToUsageMap()
            {
                Cfop = 1102,
                UsageLegacy = "001",
                DocumentType = CfopToUsageMap.DocumentTypeEnum.PurchInvoice,
                Usage = 10,
                TaxCode ="1102001",
                ServiceItem = "1001"
            };
        }

        [OneTimeTearDown]
        async public Task Logoff()
        {
            await _service.Logout();
        }

        private void doLogin()
        {
            Credentials credentials = new Credentials()
            {
                userName = "manager",
                password = "acesso19",
                database = "SBODEMOBR"
            };

            string token = _service.Login(credentials).Result;

            _token = token;
        }

        [Test]
        public void CT0_CreateTable()
        {
            Assert.That(async () => await _service.Create<CfopToUsageMap>(), Throws.Nothing);
        }

        [Test]
        public void CT1_Insert()
        {
            Assert.That(async () => await _service.Insert(_map), Throws.Nothing);

            Task.Delay(2000);

            CfopToUsageMap map = readMap();
            Assert.IsNotNull(map);
        }

        [Test]
        public void CT2_Update()
        {
            CfopToUsageMap map = readMap();

            map.Usage = 11;

            Assert.That(async () => await _service.Update(map), Throws.Nothing);

            CfopToUsageMap newMap = readMap();

            Assert.IsNotNull(newMap);
            Assert.AreEqual(map.Usage, newMap.Usage);
        }

        [Test]
        public void CT3_Delete()
        {
            CfopToUsageMap map = readMap();

            Assert.That(async () => await _service.Delete(map), Throws.Nothing);

            map = readMap();

            Assert.IsNull(map);
        }

        [Test]
        public void CT4_InsertMany()
        {
            int index = 1;
            int sampleSize = 10;
            int blockSize = 10;

            var stopWatch = new Stopwatch();

            for (int j = 1; j <= (sampleSize / blockSize); j++)
            {
                List<CfopToUsageMap> maps = new List<CfopToUsageMap>();

                for (int i = 1; i <= blockSize; i++)
                {
                    index++;
                    string recId = index.ToString("0000000");

                    maps.Add(new CfopToUsageMap
                    {
                        Cfop = 2101,
                        UsageLegacy = i.ToString("000"),
                        DocumentType = CfopToUsageMap.DocumentTypeEnum.PurchInvoice,
                        Usage = 10,
                        TaxCode = "2101001",
                        ServiceItem = "1001"
                    });
                }

                stopWatch = Stopwatch.StartNew();
                Assert.That(async () => await _service.Insert(maps), Throws.Nothing);
                stopWatch.Stop();
                Console.WriteLine($"{blockSize} records sent - duration: {stopWatch.Elapsed.ToString()}");
            }
        }


        [Test]
        async public Task CT5_DeleteAll()
        {
            var stopWatch = new Stopwatch();

            var criterias = new List<Criteria>();
            criterias.Add(new Criteria()
            {
                Field = "Cfop",
                Operator = "eq",
                Value = "2101"
            });

            stopWatch.Start();
            List<CfopToUsageMap> maps = await _service.List<CfopToUsageMap>(criterias);
            stopWatch.Stop();
            Console.WriteLine($"{maps.Count()} selected to delete - duration: {stopWatch.Elapsed.ToString()}");

            stopWatch.Start();
            foreach (var p in maps)
            {
                Assert.That(async () => await _service.Delete(p), Throws.Nothing);
            }

            stopWatch.Stop();
            Console.WriteLine($"{maps.Count()} records deleted - duration: {stopWatch.Elapsed.ToString()}");
        }

        [Test]
        public void CT6_SelectSummary()
        {
            CfopToUsageMapService service = (CfopToUsageMapService)_service.FindService<CfopToUsageMap>();

            List<Criteria> criterias = new List<Criteria>();

            var lista = service.ListSummary(criterias).Result;

            Assert.IsNotNull(lista);
            Assert.IsTrue(lista.Count != 0);

            lista.ForEach(l =>
            {
                Console.WriteLine($"cfop: {l.cfop}, agenda: {l.usageLegacy}-{l.usageLegacyName}, type: {l.documentType}, utilização: {l.usage}, item: {l.serviceItem}-{l.serviceItemName}");
            });
        }

        private CfopToUsageMap readMap()
        {
            var criteria = new List<Criteria>();

            criteria.Add(new Criteria()
            {
                Field = "Cfop",
                Operator = "eq",
                Value = _map.Cfop.ToString()
            });

            criteria.Add(new Criteria()
            {
                Field = "UsageLegacy",
                Operator = "eq",
                Value = _map.UsageLegacy
            });

            List<CfopToUsageMap> maps = _service.List<CfopToUsageMap>(criteria).Result;

            if (maps != null && maps.Count > 0)
            {
                return maps[0];
            }
            else
            {
                return null;
            }
        }

    }
}
