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
  public  class ChartOfAccountsTesting
    {
        IConfiguration _configuration;
        Service _service;
        string _token;
        Varsis.Data.Model.ChartOfAccounts _chartofaccounts;

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

            //Instancia _chartofaccounts
            _chartofaccounts = new Varsis.Data.Model.ChartOfAccounts()
            {
           
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
        public void ListChartOfAccounts()
        {
            List<Criteria> criterias = new List<Criteria>();

            List<Varsis.Data.Model.ChartOfAccounts> result = _service.List<Varsis.Data.Model.ChartOfAccounts>(criterias, 1,0).Result;

           
        }

    }
}
