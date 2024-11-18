using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Services;

namespace SeminarIntegration.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SeminarController(ISeminar seminar) : Controller
{
    [HttpGet]
    [ActionName("allSeminars")]
    [EndpointDescription("Fetches all seminars")]
    public async Task<dynamic> GetSeminars()
    {
        var response = await seminar.GetSeminars();
        return Ok(response);
    }

    [HttpGet("{seminarNo}")]
    [ActionName("Seminar")]
    [EndpointDescription("Fetches a specific seminar")]
    public async Task<dynamic> GetSeminar(string seminarNo)
    {
        var response = await seminar.GetSeminar(seminarNo);
        return Ok(response);
    }

    [HttpPost]
    [ActionName("CreateSeminar")]
    [EndpointDescription("Creates a seminar")]
    public async Task<dynamic> CreateSeminar(string Name, int SeminarDuration, int SeminarPrice)
    {
        var response = await seminar.CreateSeminar(Name, SeminarDuration, SeminarPrice);
        return Ok(response);
    }

    [HttpPatch]
    [ActionName("UpdateSeminar")]
    [EndpointDescription("Updates an existing Seminar")]
    public async Task<dynamic> PostDataToBc(Seminar data)
    {
        var response = await seminar.UpdateSeminar(data);
        return Ok(response);
    }

    [HttpDelete("{seminarNo}")]
    [ActionName("DeleteSeminar")]
    [EndpointDescription("basically sets a seminar `Blocked` value to True. Does not actually delete it.")]
    public async Task<dynamic> DeleteSeminar(string seminarNo)
    {
        var response = await seminar.DeleteSeminar(seminarNo);
        return Ok(response);
    }
}