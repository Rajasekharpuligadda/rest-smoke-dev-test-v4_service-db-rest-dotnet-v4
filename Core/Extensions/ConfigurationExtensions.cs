using ServiceDbRestDotnetV4.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ServiceDbRestDotnetV4.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds secrets loader configuration source that reads secrets from mounted directory
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="loggerFactory">Logger factory for logging</param>
        /// <returns>Configuration builder for chaining</returns>
        public static IConfigurationBuilder AddSecretsLoader(this IConfigurationBuilder builder, ILoggerFactory? loggerFactory = null)
        {
            // Build temporary configuration to access existing settings
            var tempConfiguration = builder.Build();
            
            // Use provided logger factory or create a simple one
            var factory = loggerFactory ?? LoggerFactory.Create(loggingBuilder => 
                loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Information));
            
            var source = new SecretsLoaderConfigurationSource(tempConfiguration, factory);
            return builder.Add(source);
        }
    }
}
