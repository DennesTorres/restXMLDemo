using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace xmltest
{
    public static class Function1
    {
        [FunctionName("Function1")]        
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [Queue("messages",Connection = "AzureWebJobsStorage")] out string message)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            var serializer=new XmlSerializer(typeof(Customer));           
            Customer data = (Customer)serializer.Deserialize(req.Body);
            name = name ?? data?.Name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, data);
            ms.Position= 0;
            StreamReader sr=new StreamReader(ms);
            message=sr.ReadToEnd();

            return new OkObjectResult(responseMessage);
        }
    }
}
