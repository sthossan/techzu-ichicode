using Shared.Enums;

namespace Server.Application.DTOs
{
    public class SponsorshipRequestDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string RequestorName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public SponsorshipType Type { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }
        public string? ExpectedBusinessBenefit { get; set; }
        public string? Remarks { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSponsorshipRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public SponsorshipType Type { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }
        public string? ExpectedBusinessBenefit { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Draft;
    }

    public class TransitionRequestDto
    {
        public RequestStatus NewStatus { get; set; }
        public string? Remarks { get; set; }
    }

    public class WorkflowHistoryDto
    {
        public Guid Id { get; set; }
        public RequestStatus FromStatus { get; set; }
        public RequestStatus ToStatus { get; set; }
        public string ActionByName { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; }
        public string? Remarks { get; set; }
    }
}
