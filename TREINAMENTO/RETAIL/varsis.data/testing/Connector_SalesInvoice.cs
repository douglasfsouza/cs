using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Varsis.Data.Infrastructure;
using Varsis.Data.Serviceb1.Connector;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Varsis.Data.Model.Connector;
using Varsis.Data.Serviceb1;

namespace testing
{
    public class Connector_SalesInvoice
    {
        IConfiguration _configuration;
        Service _service;
        string _token;

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
            Assert.That(async () => await _service.Create<SalesInvoice>(), Throws.Nothing);
        }

        #region Insert
        public static TestCaseData CT1_InsertCenario01
        {
            get
            {
                return new TestCaseData(
                    "Nota fiscal cenário 01",
                    new SalesInvoice()
                    {
                        CodigoCliente = "C99999",
                        CodigoEmpresa = 3,
                        DataLancamento = DateTime.Now.ToString("yyyy-MM-dd"),
                        DataLiberacao = DateTime.Now.ToString("yyyy-MM-dd"),
                        NumeroRP = "00001",
                        NumeroNegociacao = "00001",
                        CodigoCondicaoPagto = "2",
                        Observacao = "NOTA FISCAL DE TESTE",
                        Natureza = "1",
                        TipoPessoa = "1",
                        TipoOrdemFat = "",
                        Itens = new List<SalesInvoiceItem>()
                        {
                            { new SalesInvoiceItem() { CodigoProduto= "0002000002", Quantidade = 1, ValorLiquido = 1234.56 } },
                            { new SalesInvoiceItem() { CodigoProduto= "0003000002", Quantidade = 1, ValorLiquido = 56.67 } }
                        },
                        Parcelas = new List<SalesInvoiceInstallment>()
                        {
                            { new SalesInvoiceInstallment() { DataVencimento=  "2020-06-17", ValorParcela = 1000   } },
                            { new SalesInvoiceInstallment() { DataVencimento=  "2020-06-30", ValorParcela = 291.23   } }
                        }
                    }
                );
            }
        }
        public static IEnumerable<TestCaseData> CT1_InsertTestCaseSource
        {
            get
            {
                yield return CT1_InsertCenario01;
            }
        }

        [Test, TestCaseSource("CT1_InsertTestCaseSource")]
        public void CT1_Insert(string testCaseName, SalesInvoice salesInvoice)
        {
            SalesInvoiceService service = (SalesInvoiceService)_service.FindService<SalesInvoice>();
            IEntityServiceWithReturn<SalesInvoice> serviceWithReturn = service as IEntityServiceWithReturn<SalesInvoice>;

            SalesInvoice response = null;

            Assert.DoesNotThrowAsync(async () => response = await serviceWithReturn.Insert(salesInvoice));

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Response);
            Assert.IsNotEmpty(response.Response.NumeroNotaFiscal);
            Assert.IsNotEmpty(response.Response.SerieNotaFiscal);

            Console.WriteLine($"Nota fiscal gravada com sucesso: {response.Response.NumeroNotaFiscal}-{response.Response.SerieNotaFiscal}");

        }
        #endregion
    }
}
