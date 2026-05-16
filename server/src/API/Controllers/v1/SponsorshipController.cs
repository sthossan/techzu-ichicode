using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.DTOs;
using Server.Application.Interfaces;
using Server.Domain.Entities;
using Server.Shared.Models;
using Shared.Enums;
using System.Security.Claims;

namespace Server.API.Controllers
{
    [Authorize]
    public class SponsorshipController : BaseV1Controller
    {
        private readonly ISponsorshipService _sponsorshipService;

        public SponsorshipController(ISponsorshipService sponsorshipService)
        {
            _sponsorshipService = sponsorshipService;
        }

        [HttpPost]
        [Authorize(Roles = "Requestor,SystemAdmin")]
        public async Task<IActionResult> Create([FromForm] CreateSponsorshipRequestDto dto, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Error(message: $"Validation failed: {errors}"));
            }

            if (string.IsNullOrEmpty(UserId))
                return Unauthorized(ApiResponse.Error(message: "User ID not found in token."));

            try
            {
                string? fileUrl = null;
                if (file != null && file.Length > 0)
                {
                    var env = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
                    var rootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
                    var uploadsFolder = Path.Combine(rootPath, "uploads");
                    
                    if (!Directory.Exists(uploadsFolder)) 
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    fileUrl = $"/uploads/{fileName}";
                }

                // Force UTC for Postgres
                var eventDateUtc = DateTime.SpecifyKind(dto.EventDate, DateTimeKind.Utc);

                var request = new SponsorshipRequest
                {
                    Title = dto.Title,
                    RequestorId = UserId!,
                    Department = dto.Department,
                    Type = dto.Type,
                    EventName = dto.EventName,
                    EventDate = eventDateUtc,
                    RequestedAmount = dto.RequestedAmount,
                    Purpose = dto.Purpose,
                    DocumentUrl = fileUrl,
                    ExpectedBusinessBenefit = dto.ExpectedBusinessBenefit,
                    Status = dto.Status
                };

                var result = await _sponsorshipService.CreateRequestAsync(request);
                return Ok(ApiResponse.Single(result, message: "Request created successfully."));
            }
            catch (Exception ex)
            {
                // Detailed error logging
                var errorMsg = $"Error creating sponsorship: {ex.Message}";
                if (ex.InnerException != null) errorMsg += $" | Inner: {ex.InnerException.Message}";
                
                Console.WriteLine($"[CRITICAL ERROR]: {errorMsg}");
                Console.WriteLine(ex.StackTrace);
                
                return StatusCode(500, ApiResponse.Error(message: errorMsg));
            }
        }

        [HttpGet("my-requests")]
        [Authorize(Roles = "Requestor,SystemAdmin")]
        public async Task<IActionResult> GetMyRequests()
        {
            var result = await _sponsorshipService.GetRequestsByRoleAsync(UserId, UserRole.Requestor);
            var dtos = result.Select(r => new SponsorshipRequestDto
            {
                Id = r.Id,
                Title = r.Title,
                RequestorName = r.Requestor?.Email ?? "Unknown",
                Department = r.Department,
                Type = r.Type,
                EventName = r.EventName,
                EventDate = r.EventDate,
                RequestedAmount = r.RequestedAmount,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            });
            return Ok(ApiResponse.Single(dtos));
        }

        [HttpGet("pending-approvals")]
        [Authorize(Roles = "Manager,FinanceAdmin,SystemAdmin")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            var roleStr = User.FindFirstValue(ClaimTypes.Role)!;
            var role = Enum.Parse<UserRole>(roleStr);
            
            var result = await _sponsorshipService.GetRequestsByRoleAsync(UserId, role);
            var dtos = result.Select(r => new SponsorshipRequestDto
            {
                Id = r.Id,
                Title = r.Title,
                RequestorName = r.Requestor?.Email ?? "Unknown",
                Department = r.Department,
                Type = r.Type,
                EventName = r.EventName,
                EventDate = r.EventDate,
                RequestedAmount = r.RequestedAmount,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            });
            return Ok(ApiResponse.Single(dtos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sponsorshipService.GetRequestByIdAsync(id);
            if (result == null) return NotFound(ApiResponse.Error(message: "Request not found.", statusCode: 404));
            
            var dto = new SponsorshipRequestDto
            {
                Id = result.Id,
                Title = result.Title,
                RequestorName = result.Requestor?.Email ?? "Unknown",
                Department = result.Department,
                Type = result.Type,
                EventName = result.EventName,
                EventDate = result.EventDate,
                RequestedAmount = result.RequestedAmount,
                Purpose = result.Purpose,
                DocumentUrl = result.DocumentUrl,
                ExpectedBusinessBenefit = result.ExpectedBusinessBenefit,
                Status = result.Status,
                CreatedAt = result.CreatedAt
            };
            
            return Ok(ApiResponse.Single(dto));
        }

        [HttpPost("{id}/transition")]
        public async Task<IActionResult> Transition(Guid id, TransitionRequestDto dto)
        {
            await _sponsorshipService.TransitionStatusAsync(id, dto.NewStatus, UserId, dto.Remarks);
            return Ok(ApiResponse.Single(null, message: $"Status updated to {dto.NewStatus}"));
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetHistory(Guid id)
        {
            var histories = await _sponsorshipService.GetWorkflowHistoryAsync(id);
            var result = histories.Select(h => new WorkflowHistoryDto
            {
                Id = h.Id,
                FromStatus = h.FromStatus,
                ToStatus = h.ToStatus,
                ActionByName = h.ActionBy?.Email ?? "System", // Or DisplayName if available
                ActionDate = h.ActionDate,
                Remarks = h.Remarks
            });
            return Ok(ApiResponse.Single(result));
        }
    }
}
