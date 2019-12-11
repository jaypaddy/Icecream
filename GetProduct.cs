using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace icecream.GetProduct
{
    public static class GetProduct
    {
        [FunctionName("GetProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/getproduct/{productid}")] HttpRequest req,
            [CosmosDB(
                databaseName: "icecreamdb",
                collectionName: "jaycontainer",
                ConnectionStringSetting = "icecreamhack_DOCUMENTDB",
                SqlQuery = "SELECT * FROM c where c.productId={productid}")]
                IEnumerable<Product> products,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var retJSON = JsonConvert.SerializeObject(products);
            IEnumerator<Product> ienum = products.GetEnumerator();
            
            int nCount=0;
            string productJSON="";
            foreach (Product p in products)
            {
                if (nCount==0)
                {
                    //Get the First one....
                    productJSON = JsonConvert.SerializeObject(p);
                }
                nCount++;
                
            }
            if (nCount != 0 )
            {
                return  (ActionResult) new OkObjectResult($"{productJSON}");
            }
            else
            {
                return (ActionResult) new StatusCodeResult(404);
              

            }
        }
    }

    public class Product
    {
        public string id {get; set;}
        public string productId { get; set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
    }
}
