using Server.Domain.Entities;
using Shared.Enums;

namespace Server.Application.Interfaces
{
    public interface ISponsorshipService
    {
        Task<SponsorshipRequest> CreateRequestAsync(SponsorshipRequest request);
        Task<SponsorshipRequest> UpdateRequestAsync(SponsorshipRequest request);
        Task<SponsorshipRequest?> GetRequestByIdAsync(Guid id);
        Task<IEnumerable<SponsorshipRequest>> GetRequestsByRoleAsync(string userId, UserRole role);
        Task<bool> TransitionStatusAsync(Guid requestId, RequestStatus newStatus, string actionByUserId, string? remarks = null);
        Task<IEnumerable<WorkflowHistory>> GetWorkflowHistoryAsync(Guid requestId);
    }
}
