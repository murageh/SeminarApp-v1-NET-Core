using SeminarIntegration.Services;

namespace SeminarIntegration.Interfaces
{
    public interface ISeminar
    {
        Task<dynamic> PostData(SeminarData seminar);
        Task<dynamic> PostData(SeminarData seminar, string page);
    }
}