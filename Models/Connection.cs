using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using WeatherApp.DTOs;
using WeatherApp.Utils;

namespace WeatherApp.Models
{
    public class Connection
    {
        private static string _baseUri = "http://murageh:7048/BC240/ODataV4/Company('CRONUS%20International%20Ltd.')/";
        public static string EmpPath = "EmpList";
        public static string CustPath = "CustList";

        public static string GetBaseUri()
        {
            return _baseUri;
        }

        public Connection()
        {
        }

        public static NetworkCredential GetCredentials()
        {
            // if (authenticate)
            //     return new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
            //         ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"]);
            // else
            return CredentialCache.DefaultNetworkCredentials;
        }

        public static async Task<List<Employee>> FetchEmployees(string? empNo)
        {
            List<Employee> employees = new List<Employee>();

            HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.BaseAddress = new Uri(_baseUri);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // generate filters if empNo is not null
            string filter = EmpPath;
            if (!string.IsNullOrEmpty(empNo))
            {
                filter += FilterHelper.GenerateFilter("No", empNo, true);
            }

            HttpResponseMessage response = client.GetAsync(filter).Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var employeesResponse = JsonSerializer.Deserialize<EmployeesResponse>(data);

                if (employeesResponse != null)
                {
                    foreach (var employee in employeesResponse.value)
                    {
                        employees.Add(employee);
                    }
                }
            }

            return employees;
        }

        public static async Task<List<Customer>> FetchCustomers(string custNo=null, int? top = Int32.MaxValue, int? skip=0)
        {
            List<Customer> customers = new List<Customer>();

            HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            client.BaseAddress = new Uri(_baseUri);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // generate filters if custNo is not null
            string filter = CustPath;
            if (!string.IsNullOrEmpty(custNo))
            {
                filter += FilterHelper.GenerateFilter("No", custNo, true);
            }
            
            if (top != Int32.MaxValue && top != null)
            {
                filter += FilterHelper.GenerateTopQuery(top.Value, skip.Value, string.IsNullOrEmpty(custNo));
            }

            HttpResponseMessage response = client.GetAsync(filter).Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var customersResponse = JsonSerializer.Deserialize<CustomerResponse>(data);

                if (customersResponse != null)
                {
                    foreach (var customer in customersResponse.value)
                    {
                        customers.Add(customer);
                    }
                }
            }

            return customers;
        }
    }
}