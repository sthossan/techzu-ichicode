using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entities
{
    [Table("TestEntities")]
    public class TestEntity : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public string? Name { get; set; }
        [ForeignKey("ApplicationType")]
        public Guid ApplicationTypeId { get; set; }
        [ForeignKey("CriticalityLevel")]
        public Guid CriticalityLevelId { get; set; }
        [ForeignKey("OwnerStakeholder")]
        public Guid? OwnerStakeholderId { get; set; }
        public TestEnumList TestEnum { get; set; } = new TestEnumList();

    }
}
