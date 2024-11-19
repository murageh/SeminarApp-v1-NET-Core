using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "User")]
    public async Task<dynamic> GetSeminars()
    {
        var response = await seminar.GetSeminars();
        return Ok(response);
    }

    [HttpGet("{seminarNo}")]
    [ActionName("Seminar")]
    [EndpointDescription("Fetches a specific seminar")]
    [Authorize(Policy = "User")]
    public async Task<dynamic> GetSeminar(string seminarNo)
    {
        var response = await seminar.GetSeminar(seminarNo);
        return Ok(response);
    }

    [HttpGet("AvailableSeminars")]
    [ActionName("AvailableSeminars")]
    [EndpointDescription("Fetches all available seminars. fetches headers, as opposed to the previous methods.")]
    [Authorize(Policy = "User")]
    public async Task<dynamic> GetAvailableSeminars()
    {
        var response = await seminar.GetAvailableSeminars();
        return Ok(response);
    }

    [HttpGet("AvailableSeminars/{seminarNo}")]
    [ActionName("AvailableSeminar")]
    [EndpointDescription("Fetches a specific available seminar")]
    [Authorize(Policy = "User")]
    public async Task<dynamic> GetAvailableSeminar(string seminarNo)
    {
        var response = await seminar.GetAvailableSeminar(seminarNo);
        return Ok(response);
    }

    [HttpPost("Registration")]
    [ActionName("CreateSeminarRegistration")]
    [EndpointDescription("Adds a seminar registration")]
    [Authorize(Policy = "User")]
    public async Task<dynamic> AddSeminarRegistration(string semNo, string companyNo, string participantContactNo, bool confirmed)
    {
        var response = await seminar.CreateSeminarRegistration(semNo, companyNo, participantContactNo, confirmed);
        return Ok(response);
    }

    [HttpPatch("Registration")]
    [ActionName("UpdateSeminarRegistration")]
    [EndpointDescription("Updates a seminar registration")]
    [Authorize(Policy = "User")]
    public async Task<dynamic> UpdateSeminarRegistration(string semNo, int lineNo, bool confirmed)
    {
        var response = await seminar.UpdateSeminarRegistration(semNo, lineNo, confirmed);
        return Ok(response);
    }

    [HttpPost]
    [ActionName("CreateSeminar")]
    [EndpointDescription("Creates a seminar")]
    [Authorize(Policy = "User")]
    public async Task<dynamic> CreateSeminar(string Name, int SeminarDuration, int SeminarPrice)
    {
        var response = await seminar.CreateSeminar(Name, SeminarDuration, SeminarPrice);
        return Ok(response);
    }

    [HttpPatch]
    [ActionName("UpdateSeminar")]
    [EndpointDescription("Updates an existing Seminar")]
    [Authorize(Policy = "User")]
    public async Task<dynamic> PostDataToBc(Seminar data)
    {
        var response = await seminar.UpdateSeminar(data);
        return Ok(response);
    }

    [HttpDelete("{seminarNo}")]
    [ActionName("DeleteSeminar")]
    [EndpointDescription("basically sets a seminar `Blocked` value to True. Does not actually delete it.")]
    [Authorize(Policy = "User")]
    public async Task<dynamic> DeleteSeminar(string seminarNo)
    {
        var response = await seminar.DeleteSeminar(seminarNo);
        return Ok(response);
    }

    [HttpGet("GenProdPostingGroups")]
    [ActionName("GenProdPostingGroups")]
    [EndpointDescription("Fetches all general product posting groups")]
    [Authorize(Policy = "User")]
    public async Task<dynamic> GetGenProdPostingGroups()
    {
        var response = await seminar.GetGenProdPostingGroups();
        return Ok(response);
    }

    [HttpGet("VATProdPostingGroups")]
    [ActionName("VATProdPostingGroups")]
    [EndpointDescription("Fetches all VAT product posting groups")]
    [Authorize(Policy = "User")]
    public async Task<dynamic> GetVATProdPostingGroups()
    {
        var response = await seminar.GetVATProdPostingGroups();
        return Ok(response);
    }

    [HttpGet("Contacts/{companyName}")]
    [ActionName("Contacts")]
    [EndpointDescription("Fetches all contacts for a specific company")]
    [Authorize(Policy = "User")]
    public async Task<dynamic> GetContacts(string companyName)
    {
        var response = await seminar.GetContacts(companyName);
        return Ok(response);
    }
}