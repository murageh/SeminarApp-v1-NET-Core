using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SeminarIntegration.Models;

namespace SeminarIntegration.Services;

public static class HttpHelper
{
    private static readonly CamelCasePropertyNamesContractResolver CamelCaseOptions = new();

    public static async Task<HttpResponseWrapper<T>> SendPostRequest<T>(string url, object payload)
    {
        try
        {
            var jsonPayload = JsonConvert.SerializeObject(payload,
                new JsonSerializerSettings { ContractResolver = CamelCaseOptions });
            var handler = new HttpClientHandler
            {
                Credentials = Connection.GetCredentials()
            };
            using var httpClient = new HttpClient(handler);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<T>(result);
                return new HttpResponseWrapper<T>(data, true);
            }

            return new HttpResponseWrapper<T>(default, false, response.ReasonPhrase, response.StatusCode);
        }
        catch (Exception ex)
        {
            return new HttpResponseWrapper<T>(default, false, ex.Message);
        }
    }

    public static async Task<HttpResponseWrapper<T>> SendGetRequest<T>(string url)
    {
        try
        {
            var handler = new HttpClientHandler
            {
                Credentials = Connection.GetCredentials()
            };
            using var httpClient = new HttpClient(handler);

            var response = await httpClient.GetAsync(url);

            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<T>(result);
                return new HttpResponseWrapper<T>(data, true);
            }

            return new HttpResponseWrapper<T>(default, false, response.ReasonPhrase, response.StatusCode);
        }
        catch (Exception ex)
        {
            return new HttpResponseWrapper<T>(default, false, ex.Message);
        }
    }
}

public class HttpResponseWrapper<T>
{
    public HttpResponseWrapper(T? data, bool isSuccess, string? errorMessage = null,
        HttpStatusCode? statusCode = null)
    {
        Data = data;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
}