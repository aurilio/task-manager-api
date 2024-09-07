using FluentAssertions;
using NSubstitute;
using Serilog;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Handlers;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.Requests;
using Xunit;

namespace TaskManager.Domain.Tests;

public class GetTasksBySearchHandlerTests
{
    private readonly GetTasksBySearchHandler _handler;
    private readonly IElasticSearchRepository _elasticSearchRepository;
    private readonly ILogger _logger;

    public GetTasksBySearchHandlerTests()
    {
        _elasticSearchRepository = Substitute.For<IElasticSearchRepository>();
        _logger = Substitute.For<ILogger>();
        _handler = new GetTasksBySearchHandler(_elasticSearchRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnTasks_WhenTasksAreFound()
    {
        // Arrange
        var request = new GetTasksBySearchRequest("test", 1, 10);

        var tasks = new List<TaskEntity>
        {
            new TaskEntity { Id = Guid.NewGuid(), Title = "Test Task", Description = "Description", IsCompleted = false, CreatedAt = DateTime.UtcNow }
        };

        _elasticSearchRepository.GetTotalTaskCountAsync(request.Search).Returns(1);
        _elasticSearchRepository.SearchTasksAsync(request.Search, request.PageNumber, request.PageSize).Returns(tasks);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Tasks.Should().HaveCount(1);
        response.Tasks.First().Title.Should().Be("Test Task");
        response.TotalPages.Should().Be(1);
        _logger.Received(1).Information(
            Arg.Is<string>(msg => msg.Contains("[GetTasksBySearchHandler] Busca de tarefas concluída com sucesso")),
            Arg.Any<int>());
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyTasks_WhenNoTasksAreFound()
    {
        // Arrange
        var request = new GetTasksBySearchRequest("test", 1, 10);

        var tasks = new List<TaskEntity>();

        _elasticSearchRepository.GetTotalTaskCountAsync(request.Search).Returns(0);
        _elasticSearchRepository.SearchTasksAsync(request.Search, request.PageNumber, request.PageSize).Returns(tasks);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Tasks.Should().BeEmpty();
        response.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var request = new GetTasksBySearchRequest("test", 1, 10);

        var exception = new Exception("Elasticsearch error");
        _elasticSearchRepository.When(x => x.SearchTasksAsync(request.Search, request.PageNumber, request.PageSize))
                                .Throw(exception);

        // Act
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Elasticsearch error");
    }
}