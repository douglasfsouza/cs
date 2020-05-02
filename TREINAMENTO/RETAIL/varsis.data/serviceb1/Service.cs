using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Varsis.Data.Serviceb1
{
    public class Service : ServiceBase
    {
        readonly IConfiguration _appConfiguration;
        readonly ServiceLayerConnector _serviceLayerConnector;
        readonly ILogger<Service> _logger;

        string _token;

        public Service(IConfiguration appConfiguration, ILogger<Service> logger)
        {
            _appConfiguration = appConfiguration;
            _serviceLayerConnector = new ServiceLayerConnector(appConfiguration, logger);
            _logger = logger;

            this.ConfigureServices((services) =>
            {
                services.AddSingleton(_serviceLayerConnector);
            });
        }

        public override void SetToken(string token)
        {
            _token = token;
            _serviceLayerConnector.SetToken(_token);
        }

        async public override Task<string> Login(Credentials credentials)
        {
            _token = await _serviceLayerConnector.Login(credentials);
            return _token;
        }

        async public override Task Logout()
        {
            await _serviceLayerConnector.Logout();
        }
    }
}
