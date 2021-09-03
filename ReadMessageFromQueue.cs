using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureKeyVaultDemo
{
    public class ReadMessageFromQueue
    {
        
        [FunctionName("ReadMessageFromQueue")]
        public async Task Run([ServiceBusTrigger("myqueue")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            var response = JsonConvert.DeserializeObject<Family>(myQueueItem);
            try
            {
                StoreData sd = new StoreData();
                await sd.AddItemsToContainerAsync(response);
            }
            catch(Exception ex)
            {
                log.LogError("Error Occurred inserting db records " + ex.Message);
            }
        }
    }
}
