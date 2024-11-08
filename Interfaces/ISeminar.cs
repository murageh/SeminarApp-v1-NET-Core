using WeatherApp.Services;

namespace WeatherApp.Interfaces
{
    public interface ISeminar
    {
        Task<dynamic> PostData(SeminarData seminar);
    }
}