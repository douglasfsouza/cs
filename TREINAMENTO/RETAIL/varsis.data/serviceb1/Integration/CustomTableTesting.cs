using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Varsis.Data.Infrastructure;
using Varsis.Data.Serviceb1;
using Varsis.Data.Model;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Varsis.Data.Model.Integration;
namespace testing
{
    public class CustomTableTesting
    {
        IConfiguration _configuration;
        Service _service;
        string _token;
        Product _product;
        CadEntidade _cadEntidade;
        CadItem _cadItem;
        Invoice _invoice;
        InvoiceItem _invoiceItem;

        [OneTimeSetUp]
        public void Setup()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(".\\appSettingsB1.json");
            _configuration = builder.Build();
            _service = new Service(_configuration);

            doLogin();

            // Instancia produto
            _product = new Product()
            {
                id = "000001",
                fullName = "PRODUTO DE TESTE NOME COMPLETO",
                shortName = "PRODUTO DE TESTE NOME CURTO",
                price = 99999999.99,
                quantity = 9999999,
                priceLastUpdate = DateTime.Now
            };

            _cadEntidade = Populate.PopulateCadEntidade();
            _cadItem = Populate.PopulateCadItem();
            _invoice = Populate.PopulateInvoice();
            _invoiceItem = Populate.PopulateInvoiceItem();
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
            Assert.That(async () => await _service.Create<CadEntidade>(), Throws.Nothing);
        }

        [Test]
        public void CT1_Insert()
        {
            Assert.That(async () => await _service.Insert(_invoiceItem), Throws.Nothing);

            Task.Delay(2000);

            InvoiceItem invoiceItem = readInvoiceItem();
            Assert.IsNotNull(invoiceItem);
        }

        [Test]
        public void CT2_Update()
        {
            _product.fullName = "XPRODUTO DE TESTE NOME COMPLETO";
            _product.shortName = "XPRODUTO DE TESTE NOME CURTO";
            _product.priceLastUpdate = DateTime.Now;

            Assert.That(async () => await _service.Update(_product), Throws.Nothing);

            Product product = readProduct();

            Assert.IsNotNull(product);
            Assert.AreEqual(product.fullName, _product.fullName);
        }

        [Test]
        public void CT3_Delete()
        {
            Assert.That(async () => await _service.Delete(_product), Throws.Nothing);

            Product product = readProduct();

            Assert.IsNull(product);
        }

        [Test]
        public void CT4_InsertMany()
        {
            int index = 1;
            int sampleSize = 10000;
            int blockSize = 500;

            var stopWatch = new Stopwatch();

            for (int j = 1; j <= (sampleSize/blockSize); j++)
            {
                List<Product> products = new List<Product>();

                for (int i = 1; i <= blockSize; i++)
                {
                    index++;
                    string recId = index.ToString("0000000");

                    products.Add(new Product()
                    {
                        id = recId,
                        fullName = $"PRODUTO {recId}",
                        shortName = _product.shortName,
                        quantity = _product.quantity,
                        price = _product.price,
                        priceLastUpdate = _product.priceLastUpdate
                    });
                }

                stopWatch = Stopwatch.StartNew();
                Assert.That(async () => await _service.Insert(products), Throws.Nothing);
                stopWatch.Stop();
                Console.WriteLine($"{blockSize} records sent - duration: {stopWatch.Elapsed.ToString()}");
            }
        }


        [Test]
        async public Task CT5_DeleteAll()
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            List<Product> products = await _service.List<Product>(new List<Criteria>());
            stopWatch.Stop();
            Console.WriteLine($"{products.Count()} selected to delete - duration: {stopWatch.Elapsed.ToString()}");

            stopWatch.Start();
            foreach (var p in products)
            {
                Assert.That(async () => await _service.Delete(p), Throws.Nothing);
            }

            stopWatch.Stop();
            Console.WriteLine($"{products.Count()} records deleted - duration: {stopWatch.Elapsed.ToString()}");
        }

        private Product readProduct()
        {
            Product product = _service.List<Product>(new List<Criteria>()).Result.FirstOrDefault(m => m.id == _product.id);
            return product;
        }
        private CadEntidade readCadEntidade()
        {
            CadEntidade cadEntidade = _service.List<CadEntidade>(new List<Criteria>()).Result.FirstOrDefault(m => m.CODIGO == _cadEntidade.CODIGO);
            return cadEntidade;
        }
        private CadItem readCadItem()
        {
            CadItem cadItem = _service.List<CadItem>(new List<Criteria>()).Result.FirstOrDefault(m => m.COD_ITEM == _cadItem.COD_ITEM);
            return cadItem;
        }
        private Invoice readInvoice()
        {
            Invoice invoice = _service.List<Invoice>(new List<Criteria>()).Result.FirstOrDefault(m => m.ID == _invoice.ID);
            return invoice;
        }
        private InvoiceItem readInvoiceItem()
        {
            InvoiceItem invoiceItem = _service.List<InvoiceItem>(new List<Criteria>()).Result.FirstOrDefault(m => m.IDNOTA == _invoiceItem.IDNOTA);
            return invoiceItem;
        }
    }
}
