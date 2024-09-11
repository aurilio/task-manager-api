using NSubstitute;
using Serilog;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Handlers;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.DTOs;
using TaskManager.Shareable.Requests;
using Xunit;


namespace TaskManager.Domain.Tests;

public class CreateTaskHandlerTests
{
    private readonly CreateTaskHandler _handler;
    private readonly ITaskRepository _taskRepository;
    private readonly ICacheService _cacheService;
    private readonly IMessageBus _messageBus;
    private readonly IElasticSearchRepository _elasticSearchRepository;
    private readonly ILogger _logger;

    public CreateTaskHandlerTests()
    {
        _taskRepository = Substitute.For<ITaskRepository>();
        _cacheService = Substitute.For<ICacheService>();
        _messageBus = Substitute.For<IMessageBus>();
        _elasticSearchRepository = Substitute.For<IElasticSearchRepository>();
        _logger = Substitute.For<ILogger>();

        _handler = new CreateTaskHandler(
            _taskRepository,
            _cacheService,
            _messageBus,
            _elasticSearchRepository,
            _logger);
    }

    [Fact]
    public async Task Handle_ShouldCreateTaskSuccessfully()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            TaskEntityDTO = new TaskEntityCreateDTO
            {
                Title = "Custom Title",
                Description = "Custom Description",
                IsCompleted = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        var taskEntity = new TaskEntity
        {
            Title = request.TaskEntityDTO.Title,
            Description = request.TaskEntityDTO.Description,
            IsCompleted = request.TaskEntityDTO.IsCompleted,
            CreatedAt = request.TaskEntityDTO.CreatedAt
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        await _cacheService.Received(1).SetCacheAsync(Arg.Any<string>(), Arg.Any<string>());
        _messageBus.Received(1).Publish(Arg.Any<string>(), Arg.Any<string>());
    }
}
