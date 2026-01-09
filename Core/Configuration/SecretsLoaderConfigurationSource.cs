using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ServiceDbRestDotnetV4.Core.Configuration
{
    public class SecretsLoaderConfigurationSource : IConfigurationSource
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public SecretsLoaderConfigurationSource(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var logger = _loggerFactory.CreateLogger<SecretsLoaderConfigurationProvider>();
            return new SecretsLoaderConfigurationProvider(_configuration, logger);
        }
    }
}
