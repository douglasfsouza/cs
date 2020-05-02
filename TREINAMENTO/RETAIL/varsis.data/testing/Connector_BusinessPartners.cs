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
    public class Connector_BusinessPartners
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

        #region List
        public static IEnumerable<TestCaseData> CT0_ListTestCaseSource
        {
            get
            {
                yield return new TestCaseData(
                    "Retorna um fornecedor",
                    new List<Criteria>()
                    {
                        new Criteria() {Field = "code", Operator = "eq", Value = "1210"}
                    }
                );
            }
        }

        [Test, TestCaseSource("CT0_ListTestCaseSource")]
        public void CT0_List(string testCaseName, List<Criteria> criteria)
        {
            BusinessPartnersService service = (BusinessPartnersService)_service.FindService<BusinessPartners>();
            List<BusinessPartners> lista = null;

            Assert.DoesNotThrowAsync(async () => lista = await _service.List<BusinessPartners>(criteria));

            Assert.IsNotNull(lista);
            Assert.IsTrue(lista.Count != 0);
        }
        #endregion

        #region Insert
        public static TestCaseData CT1_InsertCustomer
        {
            get
            {
                return new TestCaseData(
                    "Cliente CTST01",
                    new BusinessPartners()
                    {
                        Codigo = "CTST01",
                        Tipo = "C",
                        RazaoSocial = "CLIENTE DE TESTE",
                        NomeFantasia = "NOME FANTASIA TESTE",
                        CodigoGrupo = 100,
                        ContaFinanceira = "1.01.02.01.01",
                        ContaAdiantamento = "2.01.01.02.01",
                        ContaBoleto = "1.01.02.01.01",
                        ContaEmAberto = "1.01.02.01.01",
                        CodigoGrupoImpostoRetido = "001",
                        FoneFixo = "9999-9999",
                        DDDFoneFixo = "11",
                        Email = "teste@teste.com.br",
                        TipoLogradouro = "Rua",
                        Logradouro = "Rua do Teste",
                        NumeroLogradouro = "1234",
                        ComplementoLogradouro = "1o. andar",
                        CEP = "00000-000",
                        Bairro = "Vila Teste",
                        Municipio = "Barueri",
                        Estado = "SP",
                        Pais = "BR",
                        CNPJ = "60.957.578/0001-62",
                        InscricaoEstadual = "111.111.111.111",
                        InscricaoMunicipal = "89829211",
                        CPF = "111.111.111-11",
                        IdEstrangeiro = "99999"
                    }
                );
            }
        }
        public static TestCaseData CT1_InsertVendor
        {
            get
            {
                return new TestCaseData(
                    "Fornecedor FTST01",
                    new BusinessPartners()
                    {
                        Codigo = "FTST01",
                        Tipo = "F",
                        RazaoSocial = "FORNECEDOR DE TESTE",
                        NomeFantasia = "NOME FANTASIA TESTE",
                        CodigoGrupo = 101,
                        ContaFinanceira = "2.01.01.01.01",
                        ContaAdiantamento = "1.01.02.04.01",
                        ContaBoleto = "2.01.01.01.01",
                        ContaEmAberto = "2.01.01.01.01",
                        CodigoGrupoImpostoRetido = "001",
                        FoneFixo = "9999-9999",
                        DDDFoneFixo = "11",
                        Email = "teste@teste.com.br",
                        TipoLogradouro = "Rua",
                        Logradouro = "Rua do Teste",
                        NumeroLogradouro = "1234",
                        ComplementoLogradouro = "1o. andar",
                        CEP = "00000-000",
                        Bairro = "Vila Teste",
                        Municipio = "Barueri",
                        Estado = "SP",
                        Pais = "BR",
                        CNPJ = "60.957.578/0002-62",
                        InscricaoEstadual = "111.111.111.111",
                        InscricaoMunicipal = "89829211",
                        CPF = "222.222.222.222",
                        IdEstrangeiro = "99999"
                    }
                );
            }
        }

        public static IEnumerable<TestCaseData> CT1_InsertTestCaseSource
        {
            get
            {
                yield return CT1_InsertCustomer;
                yield return CT1_InsertVendor;
            }
        }

        [Test, TestCaseSource("CT1_InsertTestCaseSource")]
        public void CT1_Insert(string testCaseName, BusinessPartners businessPartners)
        {
            Assert.DoesNotThrowAsync(async () => await _service.Insert(businessPartners));

            List<Criteria> criterias = new List<Criteria>()
            {
                {new Criteria() {Field = "Codigo", Operator = "eq", Value = businessPartners.Codigo} }
            };

            BusinessPartners record = null;

            Assert.DoesNotThrowAsync(async () => record = await _service.Find<BusinessPartners>(criterias));

            Assert.IsNotNull(record);
            Assert.IsTrue(record.Codigo == businessPartners.Codigo);
        }
        #endregion


    }
}
