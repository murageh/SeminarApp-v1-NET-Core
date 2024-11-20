using System.Net;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SeminarIntegration.DTOs;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Models;
using SeminarIntegration.Utils;
using static SeminarIntegration.Services.BcJsonResponse;

namespace SeminarIntegration.Services;

public class SeminarService(HttpClient httpClient, IConfiguration config, Credentials credentials, IMapper mapper)
    : ISeminar
{
    private readonly UrlHelper _urlHelper = new(config);
    private static CamelCasePropertyNamesContractResolver? CamelCaseOptions;

    public async Task<AppResponse<Seminar>.BaseResponse> CreateSeminar(string Name, int SeminarDuration,
        int SeminarPrice)
    {
        var functionName = "CreateSeminar";
        var url = _urlHelper.GenerateUnboundUrl(functionName);
        var responseWrapper = await HttpHelper.SendPostRequest<BcJsonResponse>(url,
            new
            {
                Name,
                SeminarDuration,
                SeminarPrice
            }
        );

        if (!responseWrapper.IsSuccess)
            return AppResponse<Seminar>.Failure(responseWrapper.ErrorMessage, (int)responseWrapper.StatusCode,
                "Create Seminar Failed", url);

        // extract seminar
        SeminarResponseValue? semResponse =
            JsonConvert.DeserializeObject<SeminarResponseValue>(responseWrapper.Data?.value);
        var sem = ExtractSeminarFromResponseWrapper(semResponse);

        return AppResponse<Seminar>.Success(sem, semResponse.message ?? "Operation successful.",
            (int)semResponse.status, "Create Seminar Success", url);
    }

    // public async Task<dynamic> CreateSeminar(Seminar seminar)
    // {
    //     var functionName = "CreateSeminar";
    //     var url = _urlHelper.GenerateUnboundUrl(functionName);
    //     var responseWrapper = await HttpHelper.SendPostRequest<BCSeminarResponse>(url, seminar);
    //
    //     return new
    //     {
    //         success = responseWrapper.IsSuccess,
    //         message = responseWrapper.IsSuccess ? "Operation successful." : responseWrapper.ErrorMessage,
    //         statusCode = responseWrapper.StatusCode,
    //         data = responseWrapper.Data?.value != null
    //             ? JsonConvert.DeserializeObject<BCSeminarResponse.SeminarResponseValue>(responseWrapper.Data.value)
    //             : null
    //     };
    // }

    public async Task<AppResponse<List<Seminar>>.BaseResponse> GetSeminars()
    {
        // // unbound actions - does not work since thsi is a GET reques. Unbound actions only accept POST
        // var url = $"{config["AppSettings:PORTALCODEUNIT"]}GetSeminars?company={config["AppSettings:BCOMPANY"]}";
        // var responseWrapper = await HttpHelper.SendGetRequest<BCSeminarResponse>(url);
        //
        // return new
        // {
        //     success = responseWrapper.IsSuccess,
        //     message = responseWrapper.IsSuccess ? "Operation successful." : responseWrapper.ErrorMessage,
        //     statusCode = responseWrapper.StatusCode,
        //     data = responseWrapper.Data?.value != null
        //         ? JsonConvert.DeserializeObject<List<Seminar>>(responseWrapper.Data.value)
        //         : null
        // };

        // // Services - codeunit
        // try
        // {
        //     var client = credentials.ObjNav();
        //     var response = client.GetSeminars();
        //     var data = JsonConvert.DeserializeObject<List<Seminar>>(response);
        //     return new
        //     {
        //         success = data != null,
        //         message = data != null ? "Operation successful." : "No seminars found.",
        //         statusCode = HttpStatusCode.OK,
        //         data
        //     };
        // }
        // catch (Exception ex)
        // {
        //     return new
        //     {
        //         success = false,
        //         message = $"BCErr => {ex.Message}",
        //         statusCode = HttpStatusCode.InternalServerError,
        //         data = new { }
        //     };
        // }

        // ODatav4 (Page WS)
        var url = $"{Connection.BaseUri}{Connection.SemListPath}";
        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);

        if (!responseWrapper.IsSuccess)
            return AppResponse<List<Seminar>>.Failure(responseWrapper.ErrorMessage, (int)responseWrapper.StatusCode,
                "Get Seminars Failed", url);

        // extract seminars
        JToken? tk = responseWrapper.Data?.value;
        var semList = tk?.ToObject<List<Seminar>>();

        return AppResponse<List<Seminar>>.Success(semList, "Operation successful.", (int)HttpStatusCode.OK,
            "Get Seminars Success", url);
    }

    public async Task<AppResponse<Seminar>.BaseResponse> GetSeminar(string seminarNo = "")
    {
        var url = $"{Connection.BaseUri}{Connection.SemListPath}";
        if (string.IsNullOrEmpty(seminarNo))
            return AppResponse<Seminar>.Failure("Seminar number is required.", (int)HttpStatusCode.BadRequest,
                "Get Seminar Failed", url);

        url += FilterHelper.GenerateFilter("No", seminarNo, true);
        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);

        if (!responseWrapper.IsSuccess)
            return AppResponse<Seminar>.Failure(responseWrapper.ErrorMessage, (int)responseWrapper.StatusCode,
                "Get Seminar Failed", url);

        // extract seminar
        JToken? tk = responseWrapper.Data?.value;
        var semList = tk?.ToObject<List<Seminar>>();

        return AppResponse<Seminar>.Success(semList?.FirstOrDefault(), "Operation successful.",
            (int)responseWrapper.StatusCode, "Get Seminar Success", url);
    }

    // uses SOAP services
    public async Task<AppResponse<object>.BaseResponse> UpdateSeminarSoap(Seminar seminar)
    {
        var url = "SOAP Endpoint URL"; // Replace with actual SOAP endpoint URL
        try
        {
            var client = credentials.ObjNav();
            var response = await client.UpdateSeminarAsync(seminar.No, seminar.Name, seminar.SeminarDuration,
                seminar.SeminarPrice);
            return AppResponse<object>.Success(response,
                $"The seminar with number {seminar.No} has been updated successfully.", (int)HttpStatusCode.OK,
                "Update Seminar Success", url);
        }
        catch (Exception ex)
        {
            return AppResponse<object>.Failure($"BCErr => {ex.Message}", (int)HttpStatusCode.InternalServerError,
                "Update Seminar Failed", url);
        }
    }

    public async Task<AppResponse<Seminar>.BaseResponse> UpdateSeminar(Seminar seminar)
    {
        var AreOptionalFieldsDefined = !string.IsNullOrEmpty(seminar.Gen_Prod_Posting_Group) ||
                                       !string.IsNullOrEmpty(seminar.VAT_Prod_Posting_Group);
        var functionName = AreOptionalFieldsDefined ? "UpdateSeminarWithGroups" : "UpdateSeminar";

        var url = _urlHelper.GenerateUnboundUrl(functionName);
        var responseWrapper = await HttpHelper.SendPostRequest<BcJsonResponse>
        (
            url,
            AreOptionalFieldsDefined
                ? seminar
                : new
                {
                    seminar.No,
                    seminar.Name,
                    seminar.SeminarDuration,
                    seminar.SeminarPrice
                }
        );

        if (!responseWrapper.IsSuccess)
            return AppResponse<Seminar>.Failure(responseWrapper.ErrorMessage, (int)responseWrapper.StatusCode,
                "Update Seminar Failed", url);

        // extract modified seminar
        SeminarResponseValue? semResponse =
            JsonConvert.DeserializeObject<SeminarResponseValue>(responseWrapper.Data?.value);
        var sem = ExtractSeminarFromResponseWrapper(semResponse);

        return AppResponse<Seminar>.Success(sem, semResponse.message ?? "Operation successful.",
            (int)semResponse.status, "Update Seminar Success", url);
    }

    public async Task<AppResponse<object>.BaseResponse> DeleteSeminar(string seminarNo)
    {
        var functionName = "DeleteSeminar";
        var url = _urlHelper.GenerateUnboundUrl(functionName);
        var responseWrapper = await HttpHelper.SendPostRequest<BcJsonResponse>
        (
            url, new
            {
                No = seminarNo
            }
        );

        if (!responseWrapper.IsSuccess)
            return AppResponse<object>.Failure(responseWrapper.ErrorMessage, (int)responseWrapper.StatusCode,
                "Delete Seminar Failed", url);

        SeminarResponseValue? semResponse =
            JsonConvert.DeserializeObject<SeminarResponseValue>(responseWrapper.Data?.value);

        return AppResponse<object>.Success(null, semResponse.message, (int)semResponse.status, "Delete Seminar Success",
            url);
    }

    public Task<dynamic> CreateSeminar(string Name, int SeminarDuration, int SeminarPrice, int MinParticipants,
        int MaxParticipants,
        bool Blocked, string GenProdPostingGroup, string VATProdPostingGroup)
    {
        // var seminar = new Seminar
        // {
        //     Name = Name,
        //     SeminarDuration = SeminarDuration,
        //     SeminarPrice = SeminarPrice,
        //     // MinimumParticipants = MinParticipants,
        //     // MaximumParticipants = MaxParticipants,
        //     // Blocked = Blocked,
        //     Gen_Prod_Posting_Group = GenProdPostingGroup,
        //     VAT_Prod_Posting_Group = VATProdPostingGroup
        // };
        //
        // return CreateSeminar(seminar);

        throw new NotImplementedException();
    }

    public async Task<AppResponse<List<AvailableSeminar>>.BaseResponse> GetAvailableSeminars()
    {
        var url = $"{Connection.BaseUri}{Connection.AvailableSeminarsPath}";
        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);

        if (!responseWrapper.IsSuccess)
            return AppResponse<List<AvailableSeminar>>.Failure(responseWrapper.ErrorMessage,
                (int)responseWrapper.StatusCode, "Get Available Seminars Failed", url);

        // extract available seminars
        JToken? tk = responseWrapper.Data?.value;
        var availableSeminars = tk?.ToObject<List<AvailableSeminar>>();

        return AppResponse<List<AvailableSeminar>>.Success(availableSeminars, "Operation successful.",
            (int)HttpStatusCode.OK, "Get Available Seminars Success", url);
    }

    public async Task<AppResponse<AvailableSeminar>.BaseResponse> GetAvailableSeminar(string seminarNo)
    {
        var url = $"{Connection.BaseUri}{Connection.AvailableSeminarsPath}({seminarNo})";
        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);

        if (!responseWrapper.IsSuccess)
            return AppResponse<AvailableSeminar>.Failure(responseWrapper.ErrorMessage, (int)responseWrapper.StatusCode,
                "Get Available Seminar Failed", url);

        // extract available seminar
        JToken? tk = responseWrapper.Data?.value;
        var availableSeminar = tk?.ToObject<AvailableSeminar>();

        return AppResponse<AvailableSeminar>.Success(availableSeminar, "Operation successful.", (int)HttpStatusCode.OK,
            "Get Available Seminar Success", url);
    }

    public async Task<AppResponse<SeminarRegistrationItem>.BaseResponse> CreateSeminarRegistration(string semNo,
        string companyNo, string participantContactNo, bool? confirmed = false)
    {
        var functionName = "CreateSeminarRegistration";
        var url = _urlHelper.GenerateUnboundUrl(functionName);
        var responseWrapper = await HttpHelper.SendPostRequest<BcJsonResponse>(url,
            new
            {
                semNo,
                companyNo,
                participantContactNo,
                confirmed
            }
        );

        if (!responseWrapper.IsSuccess)
            return AppResponse<SeminarRegistrationItem>.Failure(responseWrapper.ErrorMessage,
                (int)responseWrapper.StatusCode, "Create Seminar Registration Failed", url);

        // extract registration response
        SeminarRegistrationsResponseValue? regResponse =
            JsonConvert.DeserializeObject<SeminarRegistrationsResponseValue>(responseWrapper.Data?.value);
        var registration = ExtractSeminarRegistrationFromResponseWrapper(regResponse);

        return AppResponse<SeminarRegistrationItem>.Success(registration,
            regResponse.message ?? "Operation successful.", (int)regResponse.status,
            "Create Seminar Registration Success", url);
    }

    public async Task<AppResponse<AvailableSeminar>.BaseResponse> UpdateSeminarRegistration(string semNo, int lineNo,
        bool confirmed)
    {
        var functionName = "UpdateSeminarRegistration";
        var url = _urlHelper.GenerateUnboundUrl(functionName);
        var responseWrapper = await HttpHelper.SendPostRequest<BcJsonResponse>(url,
            new
            {
                semNo,
                lineNo,
                confirmed
            }
        );

        if (!responseWrapper.IsSuccess)
            return AppResponse<AvailableSeminar>.Failure(responseWrapper.ErrorMessage, (int)responseWrapper.StatusCode,
                "Update Seminar Registration Failed", url);

        // extract registration response
        SeminarResponseValue? regResponse =
            JsonConvert.DeserializeObject<AvailableSeminarsResponse>(responseWrapper.Data?.value);
        var registration = ExtractAvailableSeminarFromResponseWrapper(regResponse);

        return AppResponse<AvailableSeminar>.Success(registration, regResponse.message ?? "Operation successful.",
            (int)regResponse.status, "Update Seminar Registration Success", url);
    }

    public async Task<AppResponse<List<SeminarRegistrationRespItem>>.BaseResponse> GetSeminarRegistrations(string participantContactNo, string? seminarNo="")
    {
        var url = $"{Connection.BaseUri}{Connection.SeminarRegistrationsPath}";
        url += FilterHelper.GenerateFilter("Participant_Contact_No", participantContactNo, true);
        if (!string.IsNullOrEmpty(seminarNo)) url += FilterHelper.GenerateFilter("Document_No", seminarNo);
        
        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);

        if (!responseWrapper.IsSuccess)
            return AppResponse<List<SeminarRegistrationRespItem>>.Failure(responseWrapper.ErrorMessage, (int)responseWrapper.StatusCode, "Get Seminar Registrations Failed", url);

        // extract registration response
        JToken? tk = responseWrapper.Data?.value;
        var registrationItems = tk?.ToObject<List<SemRegistrationODataItem>>();
        var registrationRespItems = registrationItems?.Select(item => mapper.Map<SeminarRegistrationRespItem>(item)).ToList();

        return AppResponse<List<SeminarRegistrationRespItem>>.Success(registrationRespItems, "Operation successful.", (int)HttpStatusCode.OK, "Get Seminar Registrations Success", url);
    }

    public async Task<AppResponse<List<GenProdPostingGroupDto>>.BaseResponse> GetGenProdPostingGroups()
    {
        var url = $"{Connection.BaseUri}{Connection.GenProdPostingGroupsPath}";
        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);

        if (!responseWrapper.IsSuccess)
            return AppResponse<List<GenProdPostingGroupDto>>.Failure(responseWrapper.ErrorMessage,
                (int)responseWrapper.StatusCode, "Get GenProd Posting Groups Failed", url);

        // extract GenProdPostingGroups
        JToken? tk = responseWrapper.Data?.value;
        var genProdPostingGroups = tk?.ToObject<List<GenProdPostingGroupDto>>();

        return AppResponse<List<GenProdPostingGroupDto>>.Success(genProdPostingGroups, "Operation successful.",
            (int)HttpStatusCode.OK, "Get GenProd Posting Groups Success", url);
    }

    public async Task<AppResponse<List<VATProdPostingGroupDto>>.BaseResponse> GetVATProdPostingGroups()
    {
        var url = $"{Connection.BaseUri}{Connection.VATProdPostingGroupsPath}";
        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);

        if (!responseWrapper.IsSuccess)
            return AppResponse<List<VATProdPostingGroupDto>>.Failure(responseWrapper.ErrorMessage,
                (int)responseWrapper.StatusCode, "Get VAT Prod Posting Groups Failed", url);

        // extract VATProdPostingGroups
        JToken? tk = responseWrapper.Data?.value;
        var vatProdPostingGroups = tk?.ToObject<List<VATProdPostingGroupDto>>();

        return AppResponse<List<VATProdPostingGroupDto>>.Success(vatProdPostingGroups, "Operation successful.",
            (int)HttpStatusCode.OK, "Get VAT Prod Posting Groups Success", url);
    }

    public async Task<AppResponse<List<Contact>>.BaseResponse> GetContacts(string companyName)
    {
        var url = $"{Connection.BaseUri}{Connection.ContactsPath}";
        if (!string.IsNullOrEmpty(companyName))
        {
            url += FilterHelper.GenerateFilter("Company_Name", companyName, true);
        }

        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);

        if (!responseWrapper.IsSuccess)
            return AppResponse<List<Contact>>.Failure(responseWrapper.ErrorMessage, (int)responseWrapper.StatusCode,
                "Get Contacts Failed", url);

        // extract contacts
        JToken? tk = responseWrapper.Data?.value;
        var contacts = tk?.ToObject<List<Contact>>();

        return AppResponse<List<Contact>>.Success(contacts, "Operation successful.", (int)HttpStatusCode.OK,
            "Get Contacts Success", url);
    }

    private Seminar? ExtractSeminarFromResponseWrapper(SeminarResponseValue? res)
    {
        if (res == null) return default;
        JToken? tk = res.data;
        return tk?.ToObject<Seminar>() ?? new Seminar();
    }

    private AvailableSeminar? ExtractAvailableSeminarFromResponseWrapper(SeminarResponseValue? res)
    {
        if (res == null) return default;
        JToken? tk = res.data;
        return tk?.ToObject<AvailableSeminar>() ?? new AvailableSeminar();
    }

    private SeminarRegistrationItem? ExtractSeminarRegistrationFromResponseWrapper(
        SeminarRegistrationsResponseValue? res)
    {
        if (res == null) return default;
        JToken? tk = res.data;
        if (tk?.Type == JTokenType.Object)
        {
            return tk.ToObject<SeminarRegistrationItem>();
        }
        else if (tk?.Type == JTokenType.String)
        {
            return JsonConvert.DeserializeObject<SeminarRegistrationItem>(tk.ToString());
        }

        return new SeminarRegistrationItem();
    }

    private SeminarRegistrationRespItem? ExtractSeminarRegistrationLineResponseWrapper(SeminarRegistrationsResponseValue? res)
    {
        if (res == null) return default;
        JToken? tk = res.data;
        if (tk?.Type == JTokenType.Object)
        {
            var registrationItem = tk.ToObject<SemRegistrationODataItem>();
            return mapper.Map<SeminarRegistrationRespItem>(registrationItem);
        }
        else if (tk?.Type == JTokenType.String)
        {
            var registrationItem = JsonConvert.DeserializeObject<SemRegistrationODataItem>(tk.ToString());
            return mapper.Map<SeminarRegistrationRespItem>(registrationItem);
        }

        return new SeminarRegistrationRespItem();
    }

}

public class Seminar
{
    public string No { get; set; }
    public string Name { get; set; }
    public decimal SeminarDuration { get; set; }

    public decimal SeminarPrice { get; set; }

    // public int MinimumParticipants { get; set; }
    // public int MaximumParticipants { get; set; }
    public bool? Blocked { get; set; }
    public string? Gen_Prod_Posting_Group { get; set; }
    public string? VAT_Prod_Posting_Group { get; set; }
}

public class BcJsonResponse
{
    public dynamic value { get; set; }
}

public class SeminarResponseValue
{
    public int status { get; set; }
    public string message { get; set; }
    public dynamic? data { get; set; }
}

public class GenProdPostingGroupDto
{
    public string Code { get; set; }
    public string Description { get; set; }
    public string Def_VAT_Prod_Posting_Group { get; set; }
    public bool Auto_Insert_Default { get; set; }
}

public class VATProdPostingGroupDto
{
    public string Code { get; set; }
    public string Description { get; set; }
}

public class AvailableSeminar
{
    public string No { get; set; }
    public DateTime Starting_Date { get; set; }
    public string Seminar_No { get; set; }
    public string Seminar_Name { get; set; }
    public string Status { get; set; }
    public int Duration { get; set; }
    public int Maximum_Participants { get; set; }
    public string Room_Resource_No { get; set; }
    public int Registered_Participants { get; set; }
}

public class AvailableSeminarsResponse
{
    public List<AvailableSeminar> value { get; set; }
}

public class SeminarRegistrationItem
{
    public string SeminarNo { get; set; }
    public int LineNo { get; set; }
    public string CompanyNo { get; set; }
    public string ParticipantContactNo { get; set; }
    public string ParticipantName { get; set; }
    public string ConfirmationStatus { get; set; }
    public DateTime ConfirmationDate { get; set; }
}

public class SeminarRegistrationsResponseValue
{
    public int status { get; set; }
    public string message { get; set; }
    public dynamic? data { get; set; }
}