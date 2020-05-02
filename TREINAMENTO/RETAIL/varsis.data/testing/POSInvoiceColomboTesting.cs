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
    public class POSInvoiceColomboTesting
    {
        IConfiguration _configuration;
        Service _service;
        string _token;
        Varsis.Data.Model.Connector.POSInvoice _invoice;

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
            _invoice = new Varsis.Data.Model.Connector.POSInvoice()
            {
                DocumentType = "dDocument_Items",
                DocumentDate = new DateTime(2020, 02, 07),
                DocumentTime = new DateTime(2020, 02, 07, 14, 43, 0),
                DueDate = new DateTime(2020, 02, 07),
                CustomerId = "C99999",
                SalesPersonId = -1,
                BranchId = 3,
                InvoiceId = 9987,
                InvoiceSeries = "1",
                InvoiceModel = "53",
                FiscalKey = "3588888888888"
            };

            _invoice.Items = new List<Model.Connector.POSInvoiceItem>();

            _invoice.Items.Add(new Model.Connector.POSInvoiceItem()
            {
                LineSequence = 0,
                ItemId  = "0001000002",
                Quantity = 1,
                Price = 235.0,
                Usage = 10
            });
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
        public void CT1_Insert()
        {
            Assert.That(async () => await _service.Insert(_invoice), Throws.Nothing);

            Task.Delay(2000);

            Varsis.Data.Model.Connector.POSInvoice invoice = readInvoice();
            Assert.IsNotNull(invoice);
        }

        [Test]
        public void CT2_Find()
        {
            var invoice = this.readInvoice();

            Assert.IsNotNull(invoice);
            Assert.AreEqual(invoice.InvoiceId, _invoice.InvoiceId);
            Assert.AreEqual(invoice.InvoiceSeries, _invoice.InvoiceSeries);
            Assert.AreEqual(invoice.InvoiceModel, _invoice.InvoiceModel);

            Assert.IsNotNull(invoice.Items);
            Assert.AreEqual(invoice.Items.Count, _invoice.Items.Count);
        }

        private Model.Connector.POSInvoice readInvoice()
        {
            var criteria = new List<Criteria>();

            criteria.Add(new Criteria()
            {
                Field = "InvoiceId",
                Operator = "eq",
                Value = _invoice.InvoiceId.ToString()
            });

            criteria.Add(new Criteria()
            {
                Field = "BranchId",
                Operator = "eq",
                Value = _invoice.BranchId.ToString()
            });

            criteria.Add(new Criteria()
            {
                Field = "InvoiceSeries",
                Operator = "eq",
                Value = _invoice.InvoiceSeries
            });

            criteria.Add(new Criteria()
            {
                Field = "InvoiceModel",
                Operator = "eq",
                Value = _invoice.InvoiceModel
            });

            Model.Connector.POSInvoice invoice = _service.Find<Model.Connector.POSInvoice>(criteria).Result;

            return invoice;
        }

    }
}
