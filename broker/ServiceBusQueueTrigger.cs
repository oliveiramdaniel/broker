using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace broker
{
    public class ServiceBusQueueTrigger
    {
        [FunctionName("ServiceBusQueueTrigger")]
        public async Task Run([ServiceBusTrigger("queue-filestreaming", Connection = "servicebusdaniel_SERVICEBUS")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            var bytes = System.Convert.FromBase64String(myQueueItem);
            var content = new MemoryStream(bytes);

            try
            {
                var container = new BlobContainerClient(
                    System.Environment.GetEnvironmentVariable("AzureWebJobsStorage"),
                    "content-files"
                );

                var blobName = $"file-{DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm:ss")}";
                await container.UploadBlobAsync(blobName, content);

                log.LogInformation("arquivo inserido com sucesso!");

            } catch (Exception ex)
            {
                log.LogCritical(ex.Message);
            }
        }
    }
}
