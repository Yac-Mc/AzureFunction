using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System;
using AzureFunction.UniqueCode.Entities;

namespace AzureFunction.UniqueCode.Functions
{
    public static class GetBoxInfo
    {
        [FunctionName("GetBoxInfo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //var client = new MongoClient(Environment.GetEnvironmentVariable("MongoDBConnString"));
            //var db = client.GetDatabase("WebFlowers");
            //var _boxes = db.GetCollection<Box>("Boxes");
            //var boxes = _boxes.AsQueryable().Where(p => p.Usda == "SB25AO");

            return new OkObjectResult(responseMessage);
        }
    }
}
