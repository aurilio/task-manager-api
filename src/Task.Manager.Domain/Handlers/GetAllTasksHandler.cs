﻿using MediatR;
using Task.Manager.Domain.Repositories;
using Task.Manager.Shareable.DTOs;
using Task.Manager.Shareable.Requests;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Domain.Handlers
{
    public class GetAllTasksHandler : IRequestHandler<GetAllTasksRequest, GetAllTaskResponse>
    {
        private readonly ITaskRepository _taskRepository;

        public GetAllTasksHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<GetAllTaskResponse> Handle(GetAllTasksRequest request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            var taskDto = new TaskEntityDTO();
            var listTaskDto = new List<TaskEntityDTO>();

            foreach (var item in tasks)
            {
                taskDto.Id = item.Id;
                taskDto.Title = item.Title;
                taskDto.Description = item.Description;
                taskDto.IsCompleted = item.IsCompleted;
                taskDto.CreatedAt = item.CreatedAt;
                taskDto.UpdatedAt = item.UpdatedAt;

                listTaskDto.Add(taskDto);
            }

            return new GetAllTaskResponse
            {
                Tasks = listTaskDto
            };
        }
    }
}