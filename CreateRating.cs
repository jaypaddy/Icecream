using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs.Host;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

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
                ConnectionStringSetting = "icecreamhack_DOCUMENTDB")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            Rating ratingDoc = JsonConvert.DeserializeObject<Rating>(requestBody as string);

            if (ratingDoc.rating >=0 && ratingDoc.rating <= 5)
            {
                //Check if the Rating for the specific UserId and Product Id already exists
                //If it already exists, then update the document
                //If the combination of UserId and ProductId does not exist, then create the document.

                //Check if the Userid exists
                User u = await GetUserAsync(ratingDoc.userId);
                if (u == null)
                {
                    //User Not Found
                    return new BadRequestObjectResult($"Userid: {ratingDoc.userId} not found");
                }

                //Check if the ProductId exists
                Product p = await GetProductAsync(ratingDoc.productId);
                if (p == null)
                {
                    //User Not Found
                    return new BadRequestObjectResult($"ProductId: {ratingDoc.producId} not found");
                }

                Uri collectionUri = UriFactory.CreateDocumentCollectionUri("icecreamdb", "jaycontainer");
                /*IDocumentQuery<Rating> query = client.CreateDocumentQuery<Rating>(collectionUri)
                    .Where(rDoc => rDoc.userId==ratingDoc.userId && rDoc.productId.Contains(ratingDoc.productId) )
                    .AsDocumentQuery();

                ratingDoc.timestamp = DateTime.Now;
                if (query.HasMoreResults)  //Document Exists
                {
                    foreach (Rating result in await query.ExecuteNextAsync())
                    {
                        ratingDoc.id =  result.id;    
                        break;               
                    }
                    ResourceResponse<Document> response = await client.UpsertDocumentAsync(collectionUri,ratingDoc);
                }*/
                ResourceResponse<Document> response = await client.CreateDocumentAsync(collectionUri,ratingDoc);

            }
            return (ActionResult)new OkObjectResult($"{JsonConvert.SerializeObject(ratingDoc)}");

        }

        static async Task<Product> GetProductAsync(string productid)
        {
            Product product = null;
            String url = "https://serverlessohproduct.trafficmanager.net/api/GetProduct?productid=";
            url = url + productid;
            HttpClient hClient = new HttpClient();
            HttpResponseMessage response = await hClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<Product>();
            }
            return product;
        }


        static async Task<User> GetUserAsync(string userid)
        {
            User user = null;
            String url = "https://serverlessohuser.trafficmanager.net/api/GetUser?userId=";
            url = url + userid;
            HttpClient hClient = new HttpClient();
            HttpResponseMessage response = await hClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                user = await response.Content.ReadAsAsync<User>();
            }
            return user;
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


public class Product
{
    public string productId { get; set; }
    public string productName { get; set; }
    public string productDescription { get; set; }
}
public class User
{
    public string userId { get; set; }
    public string userName { get; set; }
    public string fullName { get; set; }
}
    public class Rating
    {
        public string id {get; set;}
        public string userId { get; set; }
        public string productId { get; set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }

        public DateTime timestamp {get; set;}
    }
}
