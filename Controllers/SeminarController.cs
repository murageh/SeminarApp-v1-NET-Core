using Microsoft.AspNetCore.Mvc;
using WeatherApp.DTOs;
using WeatherApp.Interfaces;
using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Utils;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SeminarController (ISeminar seminar) : Controller
    {
        [HttpPatch]
        public async Task<dynamic> PostDataToBc(SeminarData seminarData)
        {
            var response = await seminar.PostData(seminarData);
            return Ok(response);
        }
    }
}