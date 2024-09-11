using FluentAssertions;
using NSubstitute;
using Serilog;
using System.Text.Json;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Handlers;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.DTOs;
using TaskManager.Shareable.Requests;
using TaskManager.Shareable.Responses;
using Xunit;

namespace TaskManager.Domain.Tests;

public class GetTasksHandlerTests
{
    private readonly GetTasksHandler _handler;
    private readonly ITaskRepository _taskRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;

    public GetTasksHandlerTests()
    {
        _taskRepository = Substitute.For<ITaskRepository>();
        _cacheService = Substitute.For<ICacheService>();
        _logger = Substitute.For<ILogger>();

        _handler = new GetTasksHandler(
            _taskRepository,
            _cacheService,
            _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnTaskFromCache_WhenCacheContainsData()
    {
        // Arrange
        var request = new GetTaskRequest { TaskId = Guid.NewGuid() };
        var cachedData = JsonSerializer.Serialize(new TaskEntityGetDTO
        {
            Id = request.TaskId,
            Title = "Cached Title",
            Description = "Cached Description",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        _cacheService.GetCacheAsync(request.TaskId.ToString()).Returns(cachedData);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.TaskEntityGetDto.Should().NotBeNull();
        response.TaskEntityGetDto.Title.Should().Be("Cached Title");
    }

    [Fact]
    public async Task Handle_ShouldReturnTaskFromDatabase_WhenNotFoundInCache()
    {
        // Arrange
        var request = new GetTaskRequest { TaskId = Guid.NewGuid() };
        var taskFromDatabase = new TaskEntity
        {
            Id = request.TaskId,
            Title = "DB Title",
            Description = "DB Description",
            IsCompleted = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _cacheService.GetCacheAsync(request.TaskId.ToString()).Returns((string)null);
        _taskRepository.GetTaskByIdAsync(request.TaskId).Returns(taskFromDatabase);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.TaskEntityGetDto.Should().NotBeNull();
        response.TaskEntityGetDto.Title.Should().Be("DB Title");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenTaskNotFoundInCacheOrDatabase()
    {
        // Arrange
        var request = new GetTaskRequest { TaskId = Guid.NewGuid() };

        _cacheService.GetCacheAsync(request.TaskId.ToString()).Returns((string)null);
        _taskRepository.GetTaskByIdAsync(request.TaskId).Returns((TaskEntity)null);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.TaskEntityGetDto.Id.Equals(Guid.Empty);
    }
}
