using Server.Application.DTOs;
using Server.Application.Interfaces;
using Server.Application.Interfaces.Core;
using Server.Shared.Models;
using Server.Shared.Exceptions;
using AutoMapper;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Server.Application.Services
{

    public class TestService : ITestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TestService> _logger;

        public TestService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TestService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse> GetAllApplicationsAsync()
        {
            try
            {
                var applications = await _unitOfWork.BaseRepository<Server.Domain.Entities.TestEntity>().GetAllAsync();
                var applicationDtos = _mapper.Map<IEnumerable<TestDto>>(applications);
                return ApiResponse.Single(applicationDtos, 200, "Applications retrieved successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}