using Microsoft.AspNetCore.Mvc;
using WeatherApp.DTOs;
using WeatherApp.Models;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmployeesController : Controller
    {
        [HttpGet]
        public async Task<JsonResult> GetEmployees()
        {
            List<Employee> employees;
            try
            {
                employees = await Connection.FetchEmployees(null);
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
                    employees,
                    count = employees.Count,
                    message = "Employees fetched successfully"
                }
            );
        }

        [HttpGet("{empNo}")]
        public async Task<JsonResult> GetEmployee(string empNo)
        {
            Employee employee;
            try
            {
                var lst = await Connection.FetchEmployees(empNo);
                employee = lst.First();
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
                    employee,
                    message = "Employee fetched successfully"
                }
            );
        }
    }
}