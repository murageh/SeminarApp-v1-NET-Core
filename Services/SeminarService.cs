using System.Net;
using WeatherApp.DTOs;
using WeatherApp.Interfaces;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class SeminarService(HttpClient httpClient, IConfiguration configuration, Credentials credentials)
        : ISeminar
    {
        public async Task<dynamic> PostData(SeminarData seminar)
        {
            try
            {
                var client = credentials.ObjNav();
                var response = await client.RenameSeminarAsync(seminar.No, seminar.SeminarName, seminar.SeminarDuration, seminar.SeminarPrice);
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
    }

    public class SeminarData
    {
        public string No { get; set; }
        public string SeminarName { get; set; }
        public int SeminarDuration { get; set; }
        public int SeminarPrice { get; set; }

    }
}
