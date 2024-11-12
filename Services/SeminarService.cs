using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Models;

namespace SeminarIntegration.Services
{
    public class SeminarService(HttpClient httpClient, IConfiguration config, Credentials credentials)
        : ISeminar
    {
        private static CamelCasePropertyNamesContractResolver? CamelCaseOptions;

        public async Task<dynamic> PostData(SeminarData seminar)
        {
            try
            {
                var client = credentials.ObjNav();
                var response = await client.RenameSeminarAsync(seminar.No, seminar.SeminarName, seminar.SeminarDuration,
                    seminar.SeminarPrice);
                return new
                {
                    success = $"The seminar with number {seminar.No} has been updated successfully."
                };
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<dynamic> PostData(SeminarData seminar, string page)
        {
            {
                try
                {
                    CamelCaseOptions ??= new CamelCasePropertyNamesContractResolver();

                    var url = config["AppSettings:PORTALCODEUNIT"] + page + "?company=" +
                              config["AppSettings:BCOMPANY"];
                    // var username = config["AppSettings:W_USER"];
                    // var password = config["AppSettings:W_PWD"];
                    // var domain = config["AppSettings:DOMAIN"];
                    var jsonPayload = JsonConvert.SerializeObject(seminar,
                        new JsonSerializerSettings { ContractResolver = CamelCaseOptions });
                    var handler = new HttpClientHandler();
                    // handler.Credentials = new NetworkCredential(username, password, domain);
                    handler.Credentials = CredentialCache.DefaultNetworkCredentials;
                    var httpClient = new HttpClient(handler);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        return (new
                                {
                                    success = false,
                                    statusCode = response.StatusCode,
                                    message = response.ReasonPhrase
                                }
                            );
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<ModifySeminarResponse>(result);
                    var resObj = JsonConvert.DeserializeObject<ModifySeminarResponse.ModifySeminarResponseValue>(jsonResponse.value);
                    return resObj != null ? resObj : "No response returned.";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }

    public class SeminarData
    {
        public string No { get; set; }
        public string SeminarName { get; set; }
        public int SeminarDuration { get; set; }
        public int SeminarPrice { get; set; }
    }

    public class ModifySeminarResponse
    {
        public string value { get; set; }

        public class ModifySeminarResponseValue
        {
            public int status { get; set; }
            public string message { get; set; }
        }
    }
}