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
using System.Collections.Generic;

namespace icecream.GetRating
{
    public static class GetRatingByProduct
    {
        [FunctionName("GetRatingByProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "v1/getratingbyproduct/{productid}")] HttpRequest req,
            [CosmosDB(
                databaseName: "icecreamdb",
                collectionName: "jaycontainer",
                ConnectionStringSetting = "icecreamhack_DOCUMENTDB",
                SqlQuery = "SELECT * FROM c where c.productId={productid}")]
                IEnumerable<Rating> ratings,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var retJSON="";
            retJSON = JsonConvert.SerializeObject(ratings);

            return (ActionResult)new OkObjectResult($"{retJSON}");


        }
    }

    public class Rating
    {
        public string docId {get; set;}
        public string userId { get; set; }
        public string productId { get; set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
    }
}
