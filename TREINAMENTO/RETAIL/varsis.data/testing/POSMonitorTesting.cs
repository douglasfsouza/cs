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
    public class POSMonitorTesting
    {
        IConfiguration _configuration;
        Service _service;
        string _token;
        Varsis.Data.Model.Connector.POSMonitor _monitor;

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
            _monitor = new Varsis.Data.Model.Connector.POSMonitor()
            {
                BranchId = 1,
                BranchIdLegacy = "KK",
                Status = Varsis.Data.Model.Connector.IntegrationStatus.Pending,
                TransactionDate = System.DateTime.Now
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
                password = "Varsis@02",
                database = "SBO_COLOMBO"
            };

            string token = _service.Login(credentials).Result;

            _token = token;
        }

        [Test]
        public void CT0_CreateTable()
        {
            Assert.That(async () => await _service.Create<Model.Connector.POSMonitor>(), Throws.Nothing);
        }

        [Test]
        public void CT1_Insert()
        {
            Assert.That(async () => await _service.Insert(_monitor), Throws.Nothing);

            Task.Delay(2000);

            Varsis.Data.Model.Connector.POSMonitor monitor = readMonitor();
            Assert.IsNotNull(monitor);
        }

        [Test]
        public void CT2_Update()
        {
            _monitor.Status = Model.Connector.IntegrationStatus.Imported;

            Assert.That(async () => await _service.Update(_monitor), Throws.Nothing);

            Varsis.Data.Model.Connector.POSMonitor monitor = readMonitor();

            Assert.IsNotNull(monitor);
            Assert.AreEqual(monitor.Status, _monitor.Status);
        }

        [Test]
        public void CT3_Delete()
        {
            Varsis.Data.Model.Connector.POSMonitor monitor = readMonitor();

            Assert.That(async () => await _service.Delete(monitor), Throws.Nothing);

            monitor = readMonitor();

            Assert.IsNull(monitor);
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
                List<Model.Connector.POSMonitor> monitores = new List<Model.Connector.POSMonitor>();

                for (int i = 1; i <= blockSize; i++)
                {
                    index++;
                    string recId = index.ToString("0000000");

                    monitores.Add(new Model.Connector.POSMonitor()
                    {
                        BranchId = 1,
                        BranchIdLegacy = "KK",
                        Status = Model.Connector.IntegrationStatus.Pending,
                        TransactionDate = System.DateTime.Now.AddDays(i)
                    });
                }

                stopWatch = Stopwatch.StartNew();
                Assert.That(async () => await _service.Insert(monitores), Throws.Nothing);
                stopWatch.Stop();
                Console.WriteLine($"{blockSize} records sent - duration: {stopWatch.Elapsed.ToString()}");
            }
        }


        [Test]
        async public Task CT5_DeleteAll()
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            List<Model.Connector.POSMonitor> monitores = await _service.List<Model.Connector.POSMonitor>(new List<Criteria>());
            stopWatch.Stop();
            Console.WriteLine($"{monitores.Count()} selected to delete - duration: {stopWatch.Elapsed.ToString()}");

            stopWatch.Start();
            foreach (var p in monitores)
            {
                Assert.That(async () => await _service.Delete(p), Throws.Nothing);
            }

            stopWatch.Stop();
            Console.WriteLine($"{monitores.Count()} records deleted - duration: {stopWatch.Elapsed.ToString()}");
        }

        [Test]
        public void CT6_InitMonitoring()
        {
            POSMonitorService service = (POSMonitorService)_service.FindService<Model.Connector.POSMonitor>();

            InitMonitorArgs args = new InitMonitorArgs()
            {
                startDate = Convert.ToDateTime("03/03/2020")
            };

            Assert.That(() => service.InitMonitoring(args).Wait(), Throws.Nothing);

            Task.Delay(2000);

            List<Criteria> criterias = new List<Criteria>();
            criterias.Add(new Criteria() { Field = "BranchId", Operator = "eq", Value = "1" });
            criterias.Add(new Criteria() { Field = "TransactionDate", Operator = "eq", Value = args.startDate.Value.ToString("yyyyMMdd") });

            var lista = _service.List<Model.Connector.POSMonitor>(criterias).Result;

            Assert.IsNotNull(lista);
            Assert.IsTrue(lista.Count != 0);
        }

        [Test]
        public void CT6_SelectSummary()
        {
            POSMonitorService service = (POSMonitorService)_service.FindService<Model.Connector.POSMonitor>();

            List<Criteria> criterias = new List<Criteria>();
            criterias.Add(new Criteria() { Field = "status", Operator = "eq", Value = "1" });

            var lista = service.ListSummary(criterias).Result;

            Assert.IsNotNull(lista);
            Assert.IsTrue(lista.Count != 0);

            lista.ForEach(l =>
            {
                Console.WriteLine($"id: {l.RecId}, data: {l.TransactionDate}, filial: {l.BranchId}-{l.BranchName}, tickets: {l.CountTickets}, total: {l.SumTickets}");
            });
        }

        private Model.Connector.POSMonitor readMonitor()
        {
            var criteria = new List<Criteria>();

            criteria.Add(new Criteria()
            {
                Field = "TransactionDate",
                Operator = "eq",
                Value = _monitor.TransactionDate.ToString("yyyyMMdd")
            });

            criteria.Add(new Criteria()
            {
                Field = "BranchId",
                Operator = "eq",
                Value = _monitor.BranchId.ToString()
            });

            List<Model.Connector.POSMonitor> monitores = _service.List<Model.Connector.POSMonitor>(criteria).Result;

            if (monitores != null && monitores.Count > 0)
            {
                return monitores[0];
            }
            else
            {
                return null;
            }
        }

        [Test]
        public void CT7_List()
        {
            POSMonitorService service = (POSMonitorService)_service.FindService<Model.Connector.POSMonitor>();

            List<Criteria> criterias = new List<Criteria>();
            criterias.Add(new Criteria() { Field = "status", Operator = "eq", Value = "1" });
            criterias.Add(new Criteria() { Field = "TransactionDate", Operator = "eq", Value = _monitor.TransactionDate.ToString("dd/MM/yyyy") });

            var lista = service.List(criterias,1,0).Result;

            Assert.IsNotNull(lista);
            Assert.IsTrue(lista.Count != 0);

            lista.ForEach(l =>
            {
                Console.WriteLine($"id: {l.RecId}, data: {l.TransactionDate}, filial: {l.BranchId}-{l.BranchId}");
            });
        }

    }
}
