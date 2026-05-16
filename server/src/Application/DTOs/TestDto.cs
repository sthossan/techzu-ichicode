using AutoMapper;
using Server.Application.Mappings;
using Server.Domain.Entities;
using Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;


namespace Server.Application.DTOs
{
    public class TestDto : IMapFrom<TestEntity>
    {
        public Guid Id { get; set; }
        public string? CompanyName { get; set; }
        public string Name { get; set; } = default!;
        public Guid ApplicationTypeId { get; set; }
        public string? ApplicationTypeName { get; set; }
        public Guid CriticalityLevelId { get; set; }
        public string? CriticalityLevelName { get; set; }
        public Guid? OwnerStakeholderId { get; set; }
        public string? OwnerDisplayName { get; set; }
        public TestEnumList TestEnum { get; set; } = new TestEnumList();
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = default!;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public void Mapping(Profile profile)
        {
            
        }
    }

    public class CreateApplicationDto : IMapFrom<TestEntity>
    {
        [Required(ErrorMessage = "Application name is required")]
        [StringLength(200, ErrorMessage = "Application name cannot exceed 200 characters")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Application type is required")]
        public Guid ApplicationTypeId { get; set; }

        [Required(ErrorMessage = "Criticality level is required")]
        public Guid CriticalityLevelId { get; set; }

        public Guid? OwnerStakeholderId { get; set; }

        [StringLength(100, ErrorMessage = "Owner display name cannot exceed 100 characters")]
        public string? OwnerDisplayName { get; set; }
    }

    public class UpdateApplicationDto : IMapFrom<TestEntity>
    {
        [StringLength(200, ErrorMessage = "Application name cannot exceed 200 characters")]
        public string? Name { get; set; }

        public Guid? ApplicationTypeId { get; set; }
        public Guid? CriticalityLevelId { get; set; }
        public Guid? OwnerStakeholderId { get; set; }

        [StringLength(100, ErrorMessage = "Owner display name cannot exceed 100 characters")]
        public string? OwnerDisplayName { get; set; }
    }

    public class ApplicationDetailsDto : TestDto, IMapFrom<TestEntity>
    {
        public int StakeholderCount { get; set; }
    }
}
