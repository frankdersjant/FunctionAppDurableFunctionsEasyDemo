using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using FunctionAppDurableFunctionsEasyDemo.Domain;

namespace FunctionAppDurableFunctionsEasyDemo
{
    public static class StarterFunction
    {
        [FunctionName("StarterFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log,
             [DurableClient] IDurableOrchestrationClient starter)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(requestBody);

            log.LogInformation("Starting the orcestration");

            //starts orchestration
            //orchestrationId is unique and will be returned immediately - process might still be running...
            var orchestrationId = await starter.StartNewAsync("O_ProcessOrder", order);


            //helper function which returns the orchestrationid
            return starter.CreateCheckStatusResponse(req, orchestrationId);
        }
    }
}
