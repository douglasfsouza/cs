using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Varsis.Data.Infrastructure;
using Varsis.Data.Serviceb1;
using Varsis.Data.Model;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace testing
{
    public class Tests
    {
        IConfiguration _configuration;
        Service _service;
        string _token;

        [SetUp]
        public void Setup()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(".\\appSettingsB1.json");

            ILoggerFactory factory = new LoggerFactory();
            ILogger<Service> logger = factory.CreateLogger<Service>();

            _configuration = builder.Build();
            _service = new Service(_configuration, logger);
        }

        [OneTimeTearDown]
        public void Logoff()
        {
            _service.Logout().Wait();
        }

        [Test]
        public void A0_Login()
        {
            Credentials credentials = new Credentials()
            {
                userName = "manager",
                password = "acesso19",
                database = "SBODEMOBR"
            };

            string token = _service.Login(credentials).Result;

            Assert.IsNotNull(token);
            Assert.IsNotEmpty(token);

            TestContext.Out.WriteLine($"Token={token}");

            _token = token;
        }

        [Test]
        public void A1_SetToken()
        {
            _service.SetToken(_token);
        }

        [Test]
        public void B0_ReadSLTable()
        {
            _service.SetToken(_token);

            List<Criteria> criterias = new List<Criteria>();
            List<Varsis.Data.Model.Integration.Store> lista = _service.List<Varsis.Data.Model.Integration.Store>(criterias).Result;

            Assert.IsNotNull(lista);
            Assert.IsTrue(lista.Count > 0);

            foreach(var s in lista)
            {
                //TestContext.Out.WriteLine($"código={s.code}, nome={s.name}");
            }
        }

    }
}