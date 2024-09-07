using FluentAssertions;
using NSubstitute;
using Serilog;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Handlers;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.DTOs;
using TaskManager.Shareable.Requests;
using Xunit;

namespace TaskManager.Domain.Tests
{
    public class UpdateTaskHandlerTests
    {
        private readonly UpdateTaskHandler _handler;
        private readonly ITaskRepository _taskRepository;
        private readonly ICacheService _cacheService;
        private readonly IMessageBus _messageBus;
        private readonly IElasticSearchRepository _elasticSearchRepository;
        private readonly ILogger _logger;

        public UpdateTaskHandlerTests()
        {
            _taskRepository = Substitute.For<ITaskRepository>();
            _cacheService = Substitute.For<ICacheService>();
            _messageBus = Substitute.For<IMessageBus>();
            _elasticSearchRepository = Substitute.For<IElasticSearchRepository>();
            _logger = Substitute.For<ILogger>();

            _handler = new UpdateTaskHandler(
                _taskRepository,
                _cacheService,
                _messageBus,
                _elasticSearchRepository,
                _logger);
        }

        [Fact]
        public async Task Handle_ShouldUpdateTaskSuccessfully()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var request = new UpdateTaskRequest{
                TaskEntityDTO = new TaskEntityDTO
                {
                    Id = taskId,
                    Title = "Updated Title",
                    Description = "Updated Description",
                    IsCompleted = true
                } 
            };

            var existingTask = new TaskEntity
            {
                Id = taskId,
                Title = "Old Title",
                Description = "Old Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            _taskRepository.GetTaskByIdAsync(taskId).Returns(existingTask);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Success.Should().BeTrue();
            response.Message.Should().Be("Tarefa atualizada com sucesso!");

            await _taskRepository.Received(1).UpdateTaskAsync(Arg.Is<TaskEntity>(t =>
                t.Id == taskId &&
                t.Title == "Updated Title" &&
                t.Description == "Updated Description" &&
                t.IsCompleted == true
            ));

            await _elasticSearchRepository.Received(1).SetTaskAsync(Arg.Is<TaskEntity>(t => t.Id == taskId));
            await _cacheService.Received(1).SetCacheAsync(taskId.ToString(), Arg.Any<string>());
            _messageBus.Received(1).Publish("taskQueue", Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTaskNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var request = new UpdateTaskRequest
            {
                TaskEntityDTO = new TaskEntityDTO
                {
                    Id = taskId,
                    Title = "Updated Title",
                    Description = "Updated Description",
                    IsCompleted = true
                }
            };

            _taskRepository.GetTaskByIdAsync(taskId).Returns((TaskEntity)null);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Tarefa não encontrada.");

            await _taskRepository.DidNotReceive().UpdateTaskAsync(Arg.Any<TaskEntity>());
            await _elasticSearchRepository.DidNotReceive().SetTaskAsync(Arg.Any<TaskEntity>());
            await _cacheService.DidNotReceive().SetCacheAsync(Arg.Any<string>(), Arg.Any<string>());
            _messageBus.DidNotReceive().Publish(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var request = new UpdateTaskRequest
            {
                TaskEntityDTO = new TaskEntityDTO
                {
                    Id = taskId,
                    Title = "Updated Title",
                    Description = "Updated Description",
                    IsCompleted = true
                }
            };

            var existingTask = new TaskEntity
            {
                Id = taskId,
                Title = "Old Title",
                Description = "Old Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            _taskRepository.GetTaskByIdAsync(taskId).Returns(existingTask);
            _taskRepository.When(x => x.UpdateTaskAsync(Arg.Any<TaskEntity>())).Throw(new Exception("Update error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Update error");
            _logger.Received(1).Error(Arg.Any<Exception>(), Arg.Is<string>(s => s.Contains("Erro ao atualizar tarefa com TaskId")), taskId);
        }
    }
}