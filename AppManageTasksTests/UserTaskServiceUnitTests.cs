using AppManageTasks.DTOs;
using AppManageTasks.Repository;
using AppManageTasks.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace AppManageTasksTests
{
    public class UserTaskServiceUnitTests
    {
        private readonly Mock<IUserTaskRepository> _mockRepository;
        private readonly Mock<ILogger<UserTaskService>> _mockLogger;
        private readonly UserTaskService _service;

        public UserTaskServiceUnitTests()
        {
            _mockRepository = new Mock<IUserTaskRepository>();
            _mockLogger = new Mock<ILogger<UserTaskService>>();
            _service = new UserTaskService(_mockRepository.Object, _mockLogger.Object);
        }

        public class GetUserTaskByIdAsyncTests : UserTaskServiceUnitTests
        {
            [Fact]
            public async Task GetUserTaskByIdAsync_ValidId_ReturnsTask()
            {
                // Arrange
                var taskId = Guid.NewGuid();
                var expectedTask = new UserTaskDetail
                {
                    Id = taskId,
                    Title = "Test Task",
                    Description = "Test Description"
                };

                _mockRepository.Setup(repo => repo.GetUserTaskByIdAsync(taskId))
                    .ReturnsAsync(expectedTask);

                // Act
                var result = await _service.GetUserTaskByIdAsync(taskId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(taskId, result.Id);
                Assert.Equal("Test Task", result.Title);
                _mockRepository.Verify(repo => repo.GetUserTaskByIdAsync(taskId), Times.Once);
            }

            [Fact]
            public async Task GetUserTaskByIdAsync_EmptyId_ThrowsArgumentException()
            {
                // Arrange
                var emptyId = Guid.Empty;

                // Act & Assert
                await Assert.ThrowsAsync<ArgumentException>(() =>
                    _service.GetUserTaskByIdAsync(emptyId));

                _mockLogger.Verify(log => log.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("пустым GUID")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                    Times.Once);
            }

            [Fact]
            public async Task GetUserTaskByIdAsync_NotFound_ThrowsKeyNotFoundException()
            {
                // Arrange
                var taskId = Guid.NewGuid();
                _mockRepository.Setup(repo => repo.GetUserTaskByIdAsync(taskId))
                    .ReturnsAsync((UserTaskDetail?)null);

                // Act & Assert
                await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                    _service.GetUserTaskByIdAsync(taskId));

                _mockLogger.Verify(log => log.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("не найдена")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                    Times.Once);
            }
        }

    }
}