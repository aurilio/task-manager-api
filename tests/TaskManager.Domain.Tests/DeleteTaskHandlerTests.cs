using FluentAssertions;
using NSubstitute;
using Serilog;
using TaskManager.Domain.Handlers;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.Requests;
using Xunit;

namespace TaskManager.Domain.Tests
{
    public class DeleteTaskHandlerTests
    {
        private readonly DeleteTaskHandler _handler;
        private readonly ITaskRepository _taskRepository;
        private readonly ICacheService _cacheService;
        private readonly IElasticSearchRepository _elasticSearchRepository;
        private readonly ILogger _logger;

        public DeleteTaskHandlerTests()
        {
            _taskRepository = Substitute.For<ITaskRepository>();
            _cacheService = Substitute.For<ICacheService>();
            _elasticSearchRepository = Substitute.For<IElasticSearchRepository>();
            _logger = Substitute.For<ILogger>();

            _handler = new DeleteTaskHandler(
                _taskRepository,
                _cacheService,
                _elasticSearchRepository,
                _logger);
        }

        [Fact]
        public async Task Handle_ShouldDeleteTaskSuccessfully()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var request = new DeleteTaskRequest { TaskId = taskId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            await _taskRepository.Received(1).DeleteTaskAsync(taskId);
            await _elasticSearchRepository.Received(1).RemoveTaskAsync(taskId);
            await _cacheService.Received(1).RemoveCacheAsync(taskId.ToString());
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Tarefa excluída com sucesso!");
        }

        [Fact]
        public async Task Handle_ShouldLogInformationMessages()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var request = new DeleteTaskRequest { TaskId = taskId };

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _logger.Received(1).Information(Arg.Is<string>(s => s.Contains("Iniciando exclusão de dados da Tarefa")), taskId);
            _logger.Received(1).Information(Arg.Is<string>(s => s.Contains("Tarefa com Id: {TaskId} excluída do bando de dados")), taskId);
            _logger.Received(1).Information(Arg.Is<string>(s => s.Contains("Tarefa com TaskId: {TaskId} removida do ElasticSearch")), taskId);
            _logger.Received(1).Information(Arg.Is<string>(s => s.Contains("Tarefa com TaskId: {TaskId} removida do cache Redis")), taskId);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenErrorOccurs()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var request = new DeleteTaskRequest { TaskId = taskId };

            // Simula uma exceção ao tentar deletar a tarefa
            _taskRepository.When(x => x.DeleteTaskAsync(taskId))
                           .Do(x => throw new Exception("Erro ao deletar tarefa"));

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao deletar tarefa");
            _logger.Received(1).Error(Arg.Any<Exception>(), Arg.Is<string>(s => s.Contains("Erro ao deletar Tarefa")), taskId);
        }
    }
}
