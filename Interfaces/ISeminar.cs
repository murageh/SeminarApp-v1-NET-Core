using WeatherApp.Services;

namespace WeatherApp.Interfaces
{
    public interface ISeminar
    {
        Task<dynamic> PostData(SeminarData seminar);
        Task<dynamic> PostData(SeminarData seminar, string page);
    }
}