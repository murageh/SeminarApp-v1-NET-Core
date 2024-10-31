using Microsoft.AspNetCore.Mvc;
using WeatherApp.DTOs;
using WeatherApp.Models;
using WeatherApp.Utils;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomersController : Controller
    {
        [HttpGet]
        public async Task<JsonResult> GetCustomers(int? top=5, int? skip=0)
        {
            // Get necessary headers for validation
            var clientId = Request.Headers["client_id"].FirstOrDefault();
            var timestamp = Request.Headers["timestamp"].FirstOrDefault();
            var signature = Request.Headers["X-Signature"].FirstOrDefault();

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(signature))
            {
                return Json(
                    new
                    {
                        success = false,
                        message = "Missing required headers"
                    }
                );
            }

            // Concatenate data passed for hashing
            var data = $"top={top}&skip={skip}";

            var secretKey = AppSecurity.RetrieveClientSecret(clientId);

            if (!AppSecurity.ValidateRequest(clientId, timestamp, data, signature, secretKey))
            {
                return Json(new
                {
                    success = false,
                    message = "Unauthorized request"
                });
            }


            // CLIENT IS AUTHENTICATED
            List<Customer> customers = new List<Customer>();
            try
            {
                customers = await Connection.FetchCustomers(null, top, skip);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                return Json(
                    new
                    {
                        success = false,
                        message = e.Message
                    }
                );
            }

            return Json(
                new
                {
                    success = true,
                    customers,
                    message = "Customers fetched successfully"
                }
            );
        }

        [HttpGet("{custNo}")]
        public async Task<JsonResult> GetCustomer(string custNo)
        {
            Customer customer;
            try
            {
                var lst = await Connection.FetchCustomers(custNo);
                customer = lst.First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(
                    new
                    {
                        success = false,
                        message = e.Message,
                    }
                );
            }

            return Json(
                new
                {
                    success = true,
                    customer,
                    message = "Customer fetched successfully"
                }
            );
        }
    }
}