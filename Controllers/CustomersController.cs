using Microsoft.AspNetCore.Mvc;
using WeatherApp.DTOs;
using WeatherApp.Models;

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