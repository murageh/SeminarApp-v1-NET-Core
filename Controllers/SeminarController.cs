using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.DTOs;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Models;
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
    public async Task<IActionResult> GetSeminars()
    {
        var response = await seminar.GetSeminars();
        response.Path = HttpContext.Request.Path; 
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{seminarNo}")]
    [ActionName("Seminar")]
    [EndpointDescription("Fetches a specific seminar")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetSeminar(string seminarNo)
    {
        var response = await seminar.GetSeminar(seminarNo);
        response.Path = HttpContext.Request.Path; 
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("AvailableSeminars")]
    [ActionName("AvailableSeminars")]
    [EndpointDescription("Fetches all available seminars. fetches headers, as opposed to the previous methods.")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetAvailableSeminars()
    {
        var response = await seminar.GetAvailableSeminars();
        response.Path = HttpContext.Request.Path; 
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("AvailableSeminars/{seminarNo}")]
    [ActionName("AvailableSeminar")]
    [EndpointDescription("Fetches a specific available seminar")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetAvailableSeminar(string seminarNo)
    {
        var response = await seminar.GetAvailableSeminar(seminarNo);
        response.Path = HttpContext.Request.Path; 
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Registration")]
    [ActionName("CreateSeminarRegistration")]
    [EndpointDescription("Adds a seminar registration")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> AddSeminarRegistration(SeminarRegistrationDto newSem)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var response = await seminar.CreateSeminarRegistration(newSem.SemNo, newSem.CompanyNo, newSem.ParticipantContactNo, newSem.Confirmed);
        response.Path = HttpContext.Request.Path;
        return StatusCode(response.StatusCode, response);
    }

    [HttpPatch("Registration")]
    [ActionName("UpdateSeminarRegistration")]
    [EndpointDescription("Updates a seminar registration")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> UpdateSeminarRegistration(SeminarUpdateDto reqDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var response = await seminar.UpdateSeminarRegistration(reqDto.SemHeaderNo, reqDto.LineNo, reqDto.Confirmed);
        response.Path = HttpContext.Request.Path; 
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("MyRegistrations")]
    [ActionName("GetMyRegistrations")]
    [EndpointDescription("Fetches seminar registrations for a specific participant and seminar")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetMyRegistrations(string participantContactNo, string? seminarNo="")
    {
        // var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        // if (authHeader == null || !authHeader.StartsWith("Bearer "))
        //     return StatusCode(StatusCodes.Status401Unauthorized, new AppResponse<NormalUserResponse>.ErrorResponse()
        //     {
        //         Title = "get My Registrations",
        //         Message = "Invalid token.",
        //         Path = HttpContext.Request.Path,
        //         StatusCode = (int)HttpStatusCode.Unauthorized
        //     });
        //
        // var token = authHeader.Substring("Bearer ".Length).Trim();
        // var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        // var username = jwtToken?.Id;
        //
        // var user = userService.G

        var response = await seminar.GetSeminarRegistrations(participantContactNo, seminarNo);
        response.Path = HttpContext.Request.Path;
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [ActionName("CreateSeminar")]
    [EndpointDescription("Creates a seminar")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> CreateSeminar(string Name, int SeminarDuration, int SeminarPrice)
    {
        var response = await seminar.CreateSeminar(Name, SeminarDuration, SeminarPrice);
        response.Path = HttpContext.Request.Path; 
        return StatusCode(response.StatusCode, response);
    }

    [HttpPatch]
    [ActionName("UpdateSeminar")]
    [EndpointDescription("Updates an existing Seminar")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> PostDataToBc(Services.Seminar data)
    {
        var response = await seminar.UpdateSeminar(data);
        response.Path = HttpContext.Request.Path; 
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{seminarNo}")]
    [ActionName("DeleteSeminar")]
    [EndpointDescription("basically sets a seminar `Blocked` value to True. Does not actually delete it.")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> DeleteSeminar(string seminarNo)
    {
        var response = await seminar.DeleteSeminar(seminarNo);
        response.Path = HttpContext.Request.Path; 
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("GenProdPostingGroups")]
    [ActionName("GenProdPostingGroups")]
    [EndpointDescription("Fetches all general product posting groups")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetGenProdPostingGroups()
    {
        var response = await seminar.GetGenProdPostingGroups();
        response.Path = HttpContext.Request.Path; 
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("VATProdPostingGroups")]
    [ActionName("VATProdPostingGroups")]
    [EndpointDescription("Fetches all VAT product posting groups")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetVATProdPostingGroups()
    {
        var response = await seminar.GetVATProdPostingGroups();
        response.Path = HttpContext.Request.Path; 
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("Contacts/{companyName}")]
    [ActionName("Contacts")]
    [EndpointDescription("Fetches all contacts for a specific company")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetContacts(string companyName)
    {
        var response = await seminar.GetContacts(companyName);
        response.Path = HttpContext.Request.Path; 
        return StatusCode(response.StatusCode, response);
    }
}