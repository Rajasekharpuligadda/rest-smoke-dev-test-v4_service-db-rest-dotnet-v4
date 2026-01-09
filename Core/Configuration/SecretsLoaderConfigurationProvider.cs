using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ServiceDbRestDotnetV4.Core.Configuration
{
    public class SecretsLoaderConfigurationProvider : ConfigurationProvider
    {
        private const string SecretsDirectoryProperty = "Secrets:Directory:Path";
        private const string SecretsProviderEnabledProperty = "Secrets:Provider:Enabled";
        
        private readonly IConfiguration _configuration;
        private readonly ILogger<SecretsLoaderConfigurationProvider> _logger;

        public SecretsLoaderConfigurationProvider(IConfiguration configuration, ILogger<SecretsLoaderConfigurationProvider> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public override void Load()
        {
            _logger.LogInformation("Started Loading SecretsLoaderConfigurationProvider");
            
            var secretsProviderEnabledValue = _configuration.GetValue<string>(SecretsProviderEnabledProperty);
            var secretsProviderEnabled = bool.Parse(secretsProviderEnabledValue ?? "false");
            
            _logger.LogInformation("Secrets Provider: {Property}:{Value}", SecretsProviderEnabledProperty, secretsProviderEnabledValue);

            if (!secretsProviderEnabled)
            {
                _logger.LogInformation("Secrets provider is disabled");
                return;
            }

            var secretsDirectory = _configuration.GetValue<string>(SecretsDirectoryProperty);
            if (string.IsNullOrEmpty(secretsDirectory))
            {
                throw new InvalidOperationException($"Secrets directory path is not configured. Set {SecretsDirectoryProperty}");
            }

            var secretsPath = new DirectoryInfo(secretsDirectory);
            VerifySecretsDirectory(secretsPath);

            _logger.LogInformation("Reading secrets from directory: {Directory}", secretsDirectory);
            
            try
            {
                var mountedProperties = new Dictionary<string, string?>();
                
                var files = secretsPath.GetFiles("*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    var (secretName, secretValue) = ProcessSecretFile(file);
                    var replacedSecretKey = secretName.Replace("-", "_");
                    mountedProperties[replacedSecretKey] = secretValue;
                }

                foreach (var property in mountedProperties)
                {
                    Data[property.Key] = property.Value;
                }

                _logger.LogInformation("Secrets successfully loaded. Count: {Count}", mountedProperties.Count);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error reading secrets from directory", ex);
            }
        }

        private (string SecretName, string SecretValue) ProcessSecretFile(FileInfo secretFile)
        {
            try
            {
                var secretName = secretFile.Name;
                var secretValue = File.ReadAllText(secretFile.FullName).Trim();
                return (secretName, secretValue);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to read secret from file: {secretFile.FullName}", ex);
            }
        }

        private void VerifySecretsDirectory(DirectoryInfo secretsPath)
        {
            if (!secretsPath.Exists)
            {
                throw new DirectoryNotFoundException($"Secrets path is not a directory: {secretsPath.FullName}");
            }

            var files = secretsPath.GetFiles("*", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                throw new InvalidOperationException($"Secrets directory is empty: {secretsPath.FullName}");
            }
        }
    }
}
