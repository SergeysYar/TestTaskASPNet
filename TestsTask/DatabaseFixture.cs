using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KIP_test_task;
using Microsoft.EntityFrameworkCore;

namespace TestsTask
{
    public class DatabaseFixture : IDisposable
    {
        public UserStatisticsDbContext DbContext { get; private set; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<UserStatisticsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            DbContext = new UserStatisticsDbContext(options);

            DbContext.UserStatisticsRequests.AddRange(
                new UserStatisticsRequest
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddDays(-5),
                    RequestTime = DateTime.UtcNow,
                    IsCompleted = true,
                    Result = "Test Result 1"
                },
                new UserStatisticsRequest
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    StartDate = DateTime.UtcNow.AddDays(-20),
                    EndDate = DateTime.UtcNow.AddDays(-15),
                    RequestTime = DateTime.UtcNow,
                    IsCompleted = false,
                    Result = null
                }
            );

            DbContext.SaveChanges();
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
