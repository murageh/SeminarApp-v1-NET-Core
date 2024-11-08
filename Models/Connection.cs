using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using WeatherApp.DTOs;
using WeatherApp.Utils;

namespace WeatherApp.Models
{
    public class Connection
    {
        private const string BaseUri = "http://murageh:7048/BC240/ODataV4/Company('CRONUS%20International%20Ltd.')/";
        private const string EmpPath = "EmpList";
        private const string CustPath = "CustList";

        public static string GetBaseUri()
        {
            return BaseUri;
        }

        public static NetworkCredential GetCredentials()
        {
            // if (authenticate)
            //     return new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
            //         ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"]);
            // else
            return CredentialCache.DefaultNetworkCredentials;
        }

        public static Task<List<Employee>> FetchEmployees(string? empNo)
        {
            List<Employee> employees = [];

            HttpClient client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });
            client.BaseAddress = new Uri(BaseUri);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // generate filters if empNo is not null
            string filter = EmpPath;
            if (!string.IsNullOrEmpty(empNo))
            {
                filter += FilterHelper.GenerateFilter("No", empNo, true);
            }

            HttpResponseMessage response = client.GetAsync(filter).Result;

            // early return
            if (!response.IsSuccessStatusCode) return Task.FromResult(employees);

            var data = response.Content.ReadAsStringAsync().Result;
            var employeesResponse = JsonSerializer.Deserialize<EmployeesResponse>(data);

            if (employeesResponse != null)
            {
                employees.AddRange(employeesResponse.value);
            }

            return Task.FromResult(employees);
        }

        public static Task<List<Customer>> FetchCustomers(string? custNo = null, int? top = int.MaxValue, int? skip = 0)
        {
            List<Customer> customers = [];

            HttpClient client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });
            client.BaseAddress = new Uri(BaseUri);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // generate filters if custNo is not null
            string filter = CustPath;
            if (!string.IsNullOrEmpty(custNo))
            {
                filter += FilterHelper.GenerateFilter("No", custNo, true);
            }

            if (top != int.MaxValue && top != null)
            {
                skip ??= 0;
                filter += FilterHelper.GenerateTopQuery(top.Value, skip.Value, string.IsNullOrEmpty(custNo));
            }

            HttpResponseMessage response = client.GetAsync(filter).Result;

            // early return
            if (!response.IsSuccessStatusCode) return Task.FromResult(customers);

            var data = response.Content.ReadAsStringAsync().Result;
            var customersResponse = JsonSerializer.Deserialize<CustomerResponse>(data);

            if (customersResponse == null) return Task.FromResult(customers);

            customers.AddRange(customersResponse.value);

            return Task.FromResult(customers);
        }

        public static async Task<KCB> GetKCBToken()
        {
            var kcbInstance = new KCB();
            var client = new HttpClient();
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://uat.buni.kcbgroup.com/token?grant_type=client_credentials"
            );
            request.Headers.Add(
                "Authorization",
                "Basic YWVPRV90eWxYT2themFFZTdWTHRhTkk2bnhvYTpfTUR4OGtBc1h5aHRUN0VreWoxd"
            );
            request.Headers.Add(
                "Cookie",
                "4b1f380494b4bbde9d5435be5996a54d=ca8dddc54d876b7eda4e66af2077dbd8"
            );
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var kcbSuccessRes = JsonSerializer.Deserialize<KCB.SuccessRes>(data);

                kcbInstance.success = true;
                kcbInstance.successRes = kcbSuccessRes;
            }
            else
            {
                var data = await response.Content.ReadAsStringAsync();
                var kcbErrorRes = JsonSerializer.Deserialize<KCB.ErrorRes>(data);

                kcbInstance.success = false;
                kcbInstance.errorRes = kcbErrorRes;
            }

            return kcbInstance;
        }
    }
}