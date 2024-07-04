using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using KIP_test_task;
using Xunit;
using KIP_test_task.Controllers;

namespace TestsTask
{
    public class UserStatisticsControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly UserStatisticsDbContext _dbContext;
        private readonly UserStatisticsController _controller;

        public UserStatisticsControllerTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _controller = new UserStatisticsController(_dbContext);
        }

        [Fact]
        public async Task GetRequestInfo_ReturnsCorrectResult()
        {

            var request = new UserStatisticsRequest
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(-5),
                RequestTime = DateTime.UtcNow,
                IsCompleted = true,
                Result = "Test Result"
            };

            _dbContext.UserStatisticsRequests.Add(request);
            await _dbContext.SaveChangesAsync();


            await Task.Delay(1000);


            var result = await _controller.GetRequestInfo(request.Id);


            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            Assert.NotNull(response);


            dynamic dynamicResponse = response;
            Assert.Equal(request.Id, dynamicResponse.Query);
            Assert.InRange(dynamicResponse.Percent, 0, 100);

            if (dynamicResponse.Percent == 100)
            {
                Assert.NotNull(dynamicResponse.Result);
                Assert.Equal(request.UserId, dynamicResponse.Result.UserId);
                Assert.Equal(12, dynamicResponse.Result.CountSignIn);
            }
            else
            {
                Assert.Null(dynamicResponse.Result);
            }
        }

        [Fact]
        public async Task CreateUserStatisticsRequest_ReturnsNewGuid()
        {

            var userId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(-10);
            var endDate = DateTime.UtcNow.AddDays(-5);


            var result = await _controller.CreateUserStatisticsRequest(userId, startDate, endDate);


            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedGuid = Assert.IsType<Guid>(okResult.Value);

            var request = await _dbContext.UserStatisticsRequests.FindAsync(returnedGuid);

            Assert.NotNull(request);
            Assert.Equal(userId, request.UserId);
            Assert.Equal(startDate, request.StartDate);
            Assert.Equal(endDate, request.EndDate);
            Assert.False(request.IsCompleted);  
            Assert.Null(request.Result);  
        }
    }
}
