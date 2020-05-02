using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Varsis.Data.Infrastructure;
using Varsis.Data.Serviceb1;
using Varsis.Data.Serviceb1.Connector;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Model = Varsis.Data.Model;

namespace testing
{
    public class POSBranchConfig
    {
        IConfiguration _configuration;
        Service _service;
        string _token;
        Varsis.Data.Model.Connector.POSBranchConfig _config;

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

            //Instancia produto
            _config = new Varsis.Data.Model.Connector.POSBranchConfig()
            {
                BranchId = 1,
                BranchIdLegacy = "KK",
                DefaultCustomer = "C99999",
                UsageCode = 10,
                OpeningDate = Convert.ToDateTime("03/03/2020"),
                CashAccount = "00.001.002",
                CreditCardId = 1,
                DebitCardId = 2
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
            Assert.That(async () => await _service.Create<Model.Connector.POSBranchConfig>(), Throws.Nothing);
        }

        [Test]
        public void CT1_Insert()
        {
            Assert.That(async () => await _service.Insert(_config), Throws.Nothing);

            Task.Delay(2000);

            Varsis.Data.Model.Connector.POSBranchConfig monitor = readConfig();
            Assert.IsNotNull(monitor);
        }

        [Test]
        public void CT2_Update()
        {
            Model.Connector.POSBranchConfig config = readConfig();

            config.BranchIdLegacy = "KK-alt";

            Assert.That(async () => await _service.Update(config), Throws.Nothing);

            Varsis.Data.Model.Connector.POSBranchConfig configUpdated = readConfig();

            Assert.IsNotNull(config);
            Assert.AreEqual(configUpdated.BranchIdLegacy, config.BranchIdLegacy);
        }

        [Test]
        public void CT3_Delete()
        {
            Varsis.Data.Model.Connector.POSBranchConfig config = readConfig();

            Assert.That(async () => await _service.Delete(config), Throws.Nothing);

            config = readConfig();

            Assert.IsNull(config);
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
                List<Model.Connector.POSBranchConfig> configs = new List<Model.Connector.POSBranchConfig>();

                for (int i = 1; i <= blockSize; i++)
                {
                    index++;

                    configs.Add(new Model.Connector.POSBranchConfig()
                    {
                        BranchId = i,
                        BranchIdLegacy = $"{_config.BranchIdLegacy}{i}",
                        DefaultCustomer = _config.DefaultCustomer,
                        UsageCode = _config.UsageCode
                    });
                }

                stopWatch = Stopwatch.StartNew();
                Assert.That(async () => await _service.Insert(configs), Throws.Nothing);
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
                Field = "BranchIdLegacy",
                Operator = "startswith",
                Value = _config.BranchIdLegacy
            });

            stopWatch.Start();
            List<Model.Connector.POSBranchConfig> configs = await _service.List<Model.Connector.POSBranchConfig>(criterias);
            stopWatch.Stop();
            Console.WriteLine($"{configs.Count()} selected to delete - duration: {stopWatch.Elapsed.ToString()}");

            stopWatch.Start();
            foreach (var p in configs)
            {
                Assert.That(async () => await _service.Delete(p), Throws.Nothing);
            }

            stopWatch.Stop();
            Console.WriteLine($"{configs.Count()} records deleted - duration: {stopWatch.Elapsed.ToString()}");
        }

        [Test]
        public void CT6_ListSummary()
        {
            POSBranchConfigService service = (POSBranchConfigService)_service.FindService<Model.Connector.POSBranchConfig>();

            List<Criteria> criterias = new List<Criteria>();

            var lista = service.ListSummary(criterias).Result;

            Assert.IsNotNull(lista);
            Assert.IsTrue(lista.Count != 0);

            lista.ForEach(l =>
            {
                Console.WriteLine($"id: {l.RecId}, filial: {l.BranchId}-{l.BranchName}, cliente: {l.DefaultCustomer}-{l.DefaultCustomerName}, utilização: {l.UsageCode}, data {l.OpeningDate}");
            });
        }

        [Test]
        public void CT6_GetList()
        {
            POSBranchConfigService service = (POSBranchConfigService)_service.FindService<Model.Connector.POSBranchConfig>();

            List<Criteria> criterias = new List<Criteria>();

            var lista = service.List(null, 1, 0).Result;

            Assert.IsNotNull(lista);
            Assert.IsTrue(lista.Count != 0);

            lista.ForEach(l =>
            {
                Console.WriteLine($"id: {l.RecId}, filial: {l.BranchId}-{l.BranchId}, cliente: {l.DefaultCustomer}-{l.DefaultCustomer}, utilização: {l.UsageCode}, data {l.OpeningDate}");
            });
        }



        private Model.Connector.POSBranchConfig readConfig()
        {
            var criteria = new List<Criteria>();

            criteria.Add(new Criteria()
            {
                Field = "BranchId",
                Operator = "eq",
                Value = _config.BranchId.ToString()
            });

            List<Model.Connector.POSBranchConfig> configs = _service.List<Model.Connector.POSBranchConfig>(criteria).Result;

            if (configs != null && configs.Count > 0)
            {
                return configs[0];
            }
            else
            {
                return null;
            }
        }

    }
}
