using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace AzureKeyVaultDemo
{
    class StoreData:Startup
    {
        private Container _container;
        
        public StoreData()
        {
            Startup start = new Startup();

            CosmosClient cosmosClient = new CosmosClient(start.GetVaultValue());
            this._container = cosmosClient.GetContainer(Environment.GetEnvironmentVariable("CosmosDatabaseName"), Environment.GetEnvironmentVariable("CosmosContainerName"));
        }
        public async Task AddItemsToContainerAsync(Family familyResponse)
        {
            try
            {
                ItemResponse<Family> itemResponse = await this._container.CreateItemAsync<Family>(familyResponse, new PartitionKey(familyResponse.Job));

            }
            catch (CosmosException ex) 
            {

                Console.WriteLine("Error While creating Item"+ex.Message);
            }
        }

    }
}
