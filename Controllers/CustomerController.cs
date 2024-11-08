using Microsoft.AspNetCore.Mvc;
using WeatherApp.DTOs;
using WeatherApp.Models;
using WeatherApp.Utils;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomerController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetCustomers(int? top = 5, int? skip = 0)
        {
            List<Customer> customers;
            try
            {
                customers = await Connection.FetchCustomers(null, top, skip);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return StatusCode(500, new
                {
                    success = false,
                    message = e.Message
                });
            }

            return Ok(new
            {
                success = true,
                customers,
                count = customers.Count,
                top,
                skip,
                message = "Customers fetched successfully"
            });
        }

        [HttpGet("{custNo}")]
        public async Task<IActionResult> GetCustomer(string custNo)
        {
            Customer? customer;
            try
            {
                var lst = await Connection.FetchCustomers(custNo);
                customer = lst.FirstOrDefault();
                if (customer == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Customer not found"
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, new
                {
                    success = false,
                    message = e.Message
                });
            }

            return Ok(new
            {
                success = true,
                customer,
                message = "Customer fetched successfully"
            });
        }
    }
}