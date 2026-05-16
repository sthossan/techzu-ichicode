using Server.Domain.Entities.Identity;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities
{
    [Table("WorkflowHistories")]
    public class WorkflowHistory : BaseEntity
    {
        [Required]
        public Guid RequestId { get; set; }
        public virtual SponsorshipRequest? Request { get; set; }

        [Required]
        public RequestStatus FromStatus { get; set; }

        [Required]
        public RequestStatus ToStatus { get; set; }

        [Required]
        public string ActionById { get; set; } = string.Empty;
        public virtual ApplicationUser? ActionBy { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        public string? Remarks { get; set; }
    }
}
