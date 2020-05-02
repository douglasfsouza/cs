using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Varsis.Data.Infrastructure;
using Varsis.Data.Serviceb1;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Model = Varsis.Data.Model;

namespace testing
{
    public class POSMonitorDetailTesting
    {
        IConfiguration _configuration;
        Service _service;
        string _token;
        Varsis.Data.Model.Connector.POSMonitor _monitor;
        Varsis.Data.Model.Connector.POSMonitorDetail _detail;

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

            _monitor = new Varsis.Data.Model.Connector.POSMonitor()
            {
                BranchId = 1,
                BranchIdLegacy = "KK",
                Status = Varsis.Data.Model.Connector.IntegrationStatus.Pending,
                TransactionDate = System.DateTime.Now
            };

            _detail = new Model.Connector.POSMonitorDetail()
            {
                POSMonitor = _monitor.RecId,
                POSId = "0001",
                TransactionTime = System.DateTime.Now,
                InvoiceId = "012345",
                totalAmount = 789.98,
                itemsCount = 5
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
            Assert.That(async () => await _service.Create<Model.Connector.POSMonitorDetail>(), Throws.Nothing);
        }

        [Test]
        public void CT1_Insert()
        {
            Assert.That(async () => await _service.Insert(_monitor), Throws.Nothing);
            Assert.That(async () => await _service.Insert(_detail), Throws.Nothing);

            Task.Delay(2000);

            Model.Connector.POSMonitor monitor = readMonitor();
            Assert.IsNotNull(monitor);

            Model.Connector.POSMonitorDetail detail = readDetail(monitor);
            Assert.IsNotNull(detail);
        }

        [Test]
        public void CT2_Update()
        {
            Model.Connector.POSMonitor monitor = readMonitor();
            Model.Connector.POSMonitorDetail detail = readDetail(monitor);

            detail.itemsCount = 11;

            Assert.That(async () => await _service.Update(detail), Throws.Nothing);

            Varsis.Data.Model.Connector.POSMonitorDetail newDetail = readDetail(monitor);

            Assert.IsNotNull(detail);
            Assert.AreEqual(newDetail.itemsCount, detail.itemsCount);
        }

        [Test]
        public void CT3_Delete()
        {
            Model.Connector.POSMonitor monitor = readMonitor();
            Model.Connector.POSMonitorDetail detail = readDetail(monitor);

            Assert.That(async () => await _service.Delete(monitor), Throws.Nothing);
            Assert.That(async () => await _service.Delete(detail), Throws.Nothing);

            monitor = readMonitor();
            detail = readDetail(monitor);

            Assert.IsNull(monitor);
            Assert.IsNull(detail);
        }

        [Test]
        public void CT4_InsertMany()
        {
            int index = 1;
            int sampleSize = 10;
            int blockSize = 10;

            var stopWatch = new Stopwatch();

            for (int j = 1; j <= (sampleSize/blockSize); j++)
            {
                List<Model.Connector.POSMonitorDetail> details = new List<Model.Connector.POSMonitorDetail>();

                Random rnd = new Random(j);

                for (int i = 1; i <= blockSize; i++)
                {
                    index++;
                    string recId = index.ToString("0000000");

                    details.Add(new Model.Connector.POSMonitorDetail()
                    {
                        POSMonitor = _monitor.RecId,
                        POSId = "0001",
                        TransactionTime = System.DateTime.Now,
                        InvoiceId = rnd.Next(1000, 2000).ToString("000000"),
                        totalAmount = 789.98 * (rnd.NextDouble() * 10),
                        itemsCount = 5
                    });
                }

                stopWatch = Stopwatch.StartNew();
                Assert.That(async () => await _service.Insert(details), Throws.Nothing);
                stopWatch.Stop();
                Console.WriteLine($"{blockSize} records sent - duration: {stopWatch.Elapsed.ToString()}");
            }
        }


        [Test]
        async public Task CT5_DeleteAll()
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            List<Criteria> criterias = new List<Criteria>();
            criterias.Add(new Criteria()
            {
                Field = "POSMonitor",
                Operator = "eq",
                Value = _monitor.RecId.ToString()
            });
            List<Model.Connector.POSMonitorDetail> details = await _service.List<Model.Connector.POSMonitorDetail>(criterias);
            stopWatch.Stop();
            Console.WriteLine($"{details.Count()} selected to delete - duration: {stopWatch.Elapsed.ToString()}");

            stopWatch.Start();
            foreach (var p in details)
            {
                Assert.That(async () => await _service.Delete(p), Throws.Nothing);
            }

            stopWatch.Stop();
            Console.WriteLine($"{details.Count()} records deleted - duration: {stopWatch.Elapsed.ToString()}");
        }

        //87683cbc-dd9e-426a-994e-c50bd6e9812f

        [Test]
        public void CT6_GetDetails()
        {
            var criteria = new List<Criteria>();

            //criteria.Add(new Criteria()
            //{
            //    Field = "POSMonitor",
            //    Operator = "eq",
            //    Value = "87683cbc-dd9e-426a-994e-c50bd6e9812f"
            //});

            var details = _service.List<Model.Connector.POSMonitorDetail>(criteria, 1, 1000).Result;

            Assert.IsNotNull(details);
            Assert.IsTrue(details.Count > 0);
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

        private Model.Connector.POSMonitorDetail readDetail(Model.Connector.POSMonitor monitor)
        {
            var criteria = new List<Criteria>();

            criteria.Add(new Criteria()
            {
                Field = "POSMonitor",
                Operator = "eq",
                Value = monitor.RecId.ToString()
            });

            List<Model.Connector.POSMonitorDetail> details = _service.List<Model.Connector.POSMonitorDetail>(criteria).Result;

            if (details != null && details.Count > 0)
            {
                return details[0];
            }
            else
            {
                return null;
            }
        }
    }
}
