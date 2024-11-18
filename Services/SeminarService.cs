using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Models;
using SeminarIntegration.Utils;
using static SeminarIntegration.Services.SeminarResponse;

namespace SeminarIntegration.Services;

public class SeminarService(HttpClient httpClient, IConfiguration config, Credentials credentials)
    : ISeminar
{
    private static CamelCasePropertyNamesContractResolver? CamelCaseOptions;

    public async Task<dynamic> CreateSeminar(string Name, int SeminarDuration, int SeminarPrice)
    {
        var functionName = "CreateSeminar";
        var url = GenerateUnboundSeminarUrl(functionName);
        var responseWrapper = await HttpHelper.SendPostRequest<SeminarResponse>(url,
            new
            {
                Name, SeminarDuration, SeminarPrice
            }
        );

        if (!responseWrapper.IsSuccess)
            return new
            {
                success = false,
                message = responseWrapper.ErrorMessage,
                statusCode = responseWrapper.StatusCode,
                data = new { }
            };

        // extract seminar
        SeminarResponseValue? semResponse =
            JsonConvert.DeserializeObject<SeminarResponseValue>(responseWrapper.Data?.value);
        var sem = ExtractSeminarFromResponseWrapper(semResponse);

        return new
        {
            success = true, // there's an inner semResponse.success too
            message = semResponse.message ?? "Operation successful.",
            statusCode = semResponse.status,
            data = sem
        };
    }

    // public async Task<dynamic> CreateSeminar(Seminar seminar)
    // {
    //     var functionName = "CreateSeminar";
    //     var url = GenerateUnboundSeminarUrl(functionName);
    //     var responseWrapper = await HttpHelper.SendPostRequest<SeminarResponse>(url, seminar);
    //
    //     return new
    //     {
    //         success = responseWrapper.IsSuccess,
    //         message = responseWrapper.IsSuccess ? "Operation successful." : responseWrapper.ErrorMessage,
    //         statusCode = responseWrapper.StatusCode,
    //         data = responseWrapper.Data?.value != null
    //             ? JsonConvert.DeserializeObject<SeminarResponse.SeminarResponseValue>(responseWrapper.Data.value)
    //             : null
    //     };
    // }

    public async Task<dynamic> GetSeminars()
    {
        // // unbound actions - does not work since thsi is a GET reques. Unbound actions only accept POST
        // var url = $"{config["AppSettings:PORTALCODEUNIT"]}GetSeminars?company={config["AppSettings:BCOMPANY"]}";
        // var responseWrapper = await HttpHelper.SendGetRequest<SeminarResponse>(url);
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

        var responseWrapper = await HttpHelper.SendGetRequest<SeminarResponse>(url);

        if (!responseWrapper.IsSuccess)
            return new
            {
                success = false,
                message = responseWrapper.ErrorMessage,
                statusCode = responseWrapper.StatusCode,
                data = new { }
            };

        // extract seminars
        JToken? tk = responseWrapper.Data?.value;
        var semList = tk?.ToObject<List<Seminar>>();

        return new
        {
            success = true,
            message = "Operation successful.",
            statusCode = HttpStatusCode.OK,
            data = semList
        };
    }

    public async Task<dynamic> GetSeminar(string seminarNo = "")
    {
        var url = $"{Connection.BaseUri}{Connection.SemListPath}";
        if (string.IsNullOrEmpty(seminarNo))
            return GetSeminars(); // Not sure if this is necessary

        url += FilterHelper.GenerateFilter("No", seminarNo, true);

        var responseWrapper = await HttpHelper.SendGetRequest<SeminarResponse>(url);

        if (!responseWrapper.IsSuccess)
            return new
            {
                success = false,
                message = responseWrapper.ErrorMessage,
                statusCode = responseWrapper.StatusCode,
                data = new { }
            };

        // extract seminar
        JToken? tk = responseWrapper.Data?.value;
        var semList = tk?.ToObject<List<Seminar>>();

        return new
        {
            success = true,
            message = "Operation successful.",
            statusCode = responseWrapper.StatusCode,
            data = semList is { Count: > 0 }
                ? semList[0]
                : null
        };
    }

    // uses SOAP services
    public async Task<dynamic> UpdateSeminarSoap(Seminar seminar)
    {
        try
        {
            var client = credentials.ObjNav();
            var response = await client.UpdateSeminarAsync(seminar.No, seminar.Name, seminar.SeminarDuration,
                seminar.SeminarPrice);
            return new
            {
                success = true,
                statusCode = HttpStatusCode.OK,
                message = $"The seminar with number {seminar.No} has been updated successfully.",
                data = response
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                statusCode = HttpStatusCode.InternalServerError,
                message = $"BCErr => {ex.Message}",
                data = new { }
            };
        }
    }

    public async Task<dynamic> UpdateSeminar(Seminar seminar)
    {
        var AreOptionalFieldsDefined = !string.IsNullOrEmpty(seminar.Gen_Prod_Posting_Group) ||
                                       !string.IsNullOrEmpty(seminar.VAT_Prod_Posting_Group);
        var functionName = AreOptionalFieldsDefined ? "UpdateSeminarWithGroups" : "UpdateSeminar";

        var url = GenerateUnboundSeminarUrl(functionName);
        var responseWrapper = await HttpHelper.SendPostRequest<SeminarResponse>
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
            return new
            {
                success = false,
                message = responseWrapper.ErrorMessage,
                statusCode = responseWrapper.StatusCode,
                data = new { }
            };

        // extract modified seminar
        SeminarResponseValue? semResponse =
            JsonConvert.DeserializeObject<SeminarResponseValue>(responseWrapper.Data?.value);
        var sem = ExtractSeminarFromResponseWrapper(semResponse);

        return new
        {
            success = true,
            message = semResponse.message ?? "Operation successful.",
            statusCode = semResponse.status,
            data = sem
        };
    }

    public async Task<dynamic> DeleteSeminar(string seminarNo)
    {
        var functionName = "DeleteSeminar";
        var url = GenerateUnboundSeminarUrl(functionName);
        var responseWrapper = await HttpHelper.SendPostRequest<SeminarResponse>
        (
            url, new
            {
                No = seminarNo
            }
        );

        if (!responseWrapper.IsSuccess)
            return new
            {
                success = false,
                message = responseWrapper.ErrorMessage,
                statusCode = responseWrapper.StatusCode,
                data = new { }
            };

        SeminarResponseValue? semResponse =
            JsonConvert.DeserializeObject<SeminarResponseValue>(responseWrapper.Data?.value);

        return new
        {
            success = true,
            semResponse.message,
            statusCode = semResponse.status
        };
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

    private string GenerateUnboundSeminarUrl(string functionName)
    {
        return $"{config["AppSettings:PORTALCODEUNIT"]}{functionName}?company={config["AppSettings:BCOMPANY"]}";
    }

    private Seminar? ExtractSeminarFromResponseWrapper(SeminarResponseValue? res)
    {
        if (res == null) return default;
        JToken? tk = res.data;
        return tk?.ToObject<Seminar>() ?? new Seminar();
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

public class SeminarResponse
{
    public dynamic value { get; set; }

    public class SeminarResponseValue
    {
        public int status { get; set; }
        public string message { get; set; }
        public dynamic? data { get; set; }
    }
}