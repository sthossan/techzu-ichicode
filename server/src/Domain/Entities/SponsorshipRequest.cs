using Server.Domain.Entities.Identity;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities
{
    [Table("SponsorshipRequests")]
    public class SponsorshipRequest : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string RequestorId { get; set; } = string.Empty;
        public virtual ApplicationUser? Requestor { get; set; }

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        public SponsorshipType Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal RequestedAmount { get; set; }

        [Required]
        public string Purpose { get; set; } = string.Empty;

        public string? DocumentUrl { get; set; }
        public string? ExpectedBusinessBenefit { get; set; }
        public string? Remarks { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Draft;

        public string? ManagerId { get; set; }
        public virtual ApplicationUser? Manager { get; set; }

        public string? FinanceAdminId { get; set; }
        public virtual ApplicationUser? FinanceAdmin { get; set; }

        public virtual ICollection<WorkflowHistory> Histories { get; set; } = new List<WorkflowHistory>();
    }
}
