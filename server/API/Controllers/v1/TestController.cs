using Microsoft.AspNetCore.Mvc;
using Server.Application.DTOs;
using Server.Application.Interfaces;
using Server.Shared.Models;

namespace Server.API.Controllers.v1
{
    public class TestController : BaseV1Controller
    {
        private readonly ITestService _applicationService;
        private readonly ILogger<TestController> _logger;

        public TestController(ITestService applicationService, ILogger<TestController> logger)
        {
            _applicationService = applicationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all applications
        /// </summary>
        /// <returns>List of all applications</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllApplications()
        {
            var result = await _applicationService.GetAllApplicationsAsync();
            return Ok(result);
        }
    }
}
