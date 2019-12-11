using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;



namespace icecream.CreateRating
{
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/createrating")] HttpRequest req,
            [CosmosDB(
                databaseName: "icecreamdb",
                collectionName: "jaycontainer",
                ConnectionStringSetting = "icecreamhack_DOCUMENTDB")]IAsyncCollector<Rating> ratingsDoc,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            Rating ratingDoc = JsonConvert.DeserializeObject<Rating>(requestBody as string);
            ratingDoc.timestamp = DateTime.Now;

            await ratingsDoc.AddAsync(ratingDoc);
            return (ActionResult)new OkObjectResult($"{JsonConvert.SerializeObject(ratingDoc)}");

        }
    }

/*
{
  "userId": "cc20a6fb-a91f-4192-874d-132493685376",
  "productId": "4c25613a-a3c2-4ef3-8e02-9c335eb23204",
  "locationName": "Sample ice cream shop",
  "rating": 5,
  "userNotes": "I love the subtle notes of orange in this ice cream!"
}
*/
    public class Rating
    {
        public string userId { get; set; }
        public string productId { get; set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }

        public DateTime timestamp {get; set;}
    }
}
