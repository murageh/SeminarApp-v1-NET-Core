using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.DTOs;
using SeminarIntegration.Models;

namespace SeminarIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmployeeController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            List<Employee> employees;
            try
            {
                employees = await Connection.FetchEmployees(null);
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
        public async Task<IActionResult> GetEmployee(string empNo)
        {
            Employee? employee;
            try
            {
                var lst = await Connection.FetchEmployees(empNo);
                employee = lst.FirstOrDefault();
                if (employee == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Employee not found"
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
                employee,
                message = "Employee fetched successfully"
            });
        }
    }
}