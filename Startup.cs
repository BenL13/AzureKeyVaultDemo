using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(AzureKeyVaultDemo.Startup))]

namespace AzureKeyVaultDemo
{
    public class Startup : FunctionsStartup
    {

        public  CosmosClient _cosmosClient;
        public Database database;
        public Container container;
        public string secretValue;
        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", true)
            .AddEnvironmentVariables()
            .Build();

        public override async void Configure(IFunctionsHostBuilder builder)
        {
            
             secretValue = GetVaultValue();
            if (secretValue !=null)
            {
                this._cosmosClient = new CosmosClient(secretValue);
                await this.CreateDatabaseAsync();
                await this.CreateContainerAsync();
            }

        }
        
        private async Task CreateDatabaseAsync()
        {
            this.database = await this._cosmosClient.CreateDatabaseIfNotExistsAsync(Configuration["CosmosDatabaseName"]);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }
        private async Task CreateContainerAsync()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(Configuration["CosmosContainerName"], "/Job");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        public string GetVaultValue()
        {
            var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            string vaultBaseUrl = Configuration["vaultBaseUrl"];
            string secretName = Configuration["secretName"];
            var secret = client.GetSecretAsync(vaultBaseUrl, secretName).GetAwaiter().GetResult();
            return secret.Value;
        }

        private static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
           
            string clientId = Configuration["ClientID"];
            string clientSecret = Configuration["ClientSecret"];

            var credential = new ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, credential).ConfigureAwait(false);   
            return result.AccessToken;
        }

    }
}