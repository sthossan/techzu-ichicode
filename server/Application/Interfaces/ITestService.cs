using Server.Shared.Models;

namespace Server.Application.Interfaces
{
    public interface ITestService
    {
        Task<ApiResponse> GetAllApplicationsAsync();
    }
}
