using FluentAssertions;
using NSubstitute;
using Serilog;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Handlers;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.Requests;
using Xunit;

namespace TaskManager.Domain.Tests;

public class GetAllTasksHandlerTests
{
    private readonly GetAllTasksHandler _handler;
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger _logger;

    public GetAllTasksHandlerTests()
    {
        _taskRepository = Substitute.For<ITaskRepository>();
        _logger = Substitute.For<ILogger>();

        _handler = new GetAllTasksHandler(_taskRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnTasksWithPagination()
    {
        // Arrange
        var tasks = new List<TaskEntity>
        {
            new TaskEntity { Id = Guid.NewGuid(), Title = "Task 1", Description = "Description 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new TaskEntity { Id = Guid.NewGuid(), Title = "Task 2", Description = "Description 2", IsCompleted = true, CreatedAt = DateTime.UtcNow }
        };

        var request = new GetAllTasksRequest(1, 10);

        _taskRepository.GetTotalTasksCountAsync().Returns(2);
        _taskRepository.GetTasksWithPaginationAsync(request.PageNumber, request.PageSize).Returns(tasks);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Tasks.Should().HaveCount(2);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(1);
        result.Tasks.First().Title.Should().Be("Task 1");
        result.Tasks.Last().Title.Should().Be("Task 2");

        await _taskRepository.Received(1).GetTotalTasksCountAsync();
        await _taskRepository.Received(1).GetTasksWithPaginationAsync(request.PageNumber, request.PageSize);

        _logger.Received().Information(Arg.Do<string>(msg =>
        {
            Assert.Contains("Iniciando a busca de todas as tarefas com paginação", msg);
            Assert.Contains("Tarefas encontradas no banco de dados", msg);
            Assert.Contains("Busca de todas as tarefas concluída com sucesso", msg);
        }));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyResponse_WhenNoTasksFound()
    {
        // Arrange
        var request = new GetAllTasksRequest(1, 10);

        _taskRepository.GetTotalTasksCountAsync().Returns(0);
        _taskRepository.GetTasksWithPaginationAsync(request.PageNumber, request.PageSize).Returns(new List<TaskEntity>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Tasks.Should().BeEmpty();
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(0);

        await _taskRepository.Received(1).GetTotalTasksCountAsync();
        await _taskRepository.Received(1).GetTasksWithPaginationAsync(request.PageNumber, request.PageSize);

        _logger.Received(1).Warning(Arg.Is<string>(s => s.Contains("Nenhuma tarefa encontrada no banco de dados.")));
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var request = new GetAllTasksRequest(1, 10);

        _taskRepository.When(x => x.GetTasksWithPaginationAsync(request.PageNumber, request.PageSize))
                       .Do(x => throw new Exception("Erro inesperado"));

        // Act
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Erro inesperado");
    }
}
