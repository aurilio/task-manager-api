using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Shareable.DTOs;
using TaskManager.Shareable.Requests;
using TaskManager.Shareable.Responses;

namespace TaskManager.Api.Endpoints;

public static class Endpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/tasks", GetAllTasksAsync)
            .WithSummary("Obtém todas as tarefas")
            .WithTags("Tarefas");

        app.MapGet("/tasks/{id}", GetTaskAsync)
            .WithSummary("Obtém uma tarefa pelo ID")
            .WithDescription("Retorna uma tarefa específica com base no ID fornecido.")
            .WithTags("Tarefas");
        
        app.MapGet("/tasks/search", GetTasksWithNameAsync)
            .WithSummary("Obtém uma tarefa pelo ID")
            .WithDescription("Retorna uma tarefa específica com base no ID fornecido.")
            .WithTags("Tarefas");

        app.MapPost("/tasks", CreateTaskAsync)
           .WithSummary("Cria uma nova tarefa")
           .WithTags("Tarefas");

        app.MapPut("/tasks/{id}", UpdateTask)
            .WithSummary("Atualiza um produto existente")
           .WithTags("Tarefas");

        app.MapDelete("/tasks/{id}", Delete)
            .WithSummary("Deleta uma tarefa existente")
           .WithTags("Tarefas");
    }

    private static async Task<GetAllTaskResponse> GetAllTasksAsync(
        [FromServices] IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        return await mediator.Send(new GetAllTasksRequest(pageNumber, pageSize ));
    }

    private static async Task<GetTaskResponse> GetTaskAsync([FromServices] IMediator mediator, [FromRoute] Guid id)
    {
        return await mediator.Send(new GetTaskRequest() { TaskId = id });
    }

    private static async Task<GetTasksBySearchResponse> GetTasksWithNameAsync(
        [FromServices] IMediator mediator,
        [FromQuery] string search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        return await mediator.Send(new GetTasksBySearchRequest(search, pageNumber, pageSize));
    }

    private static async Task<CreateTaskResponse> CreateTaskAsync ([FromServices] IMediator mediator, TaskEntityCreateDTO taskEntityDTO)
    {
        return await mediator.Send(new CreateTaskRequest() { TaskEntityDTO = taskEntityDTO });
    }

    private static async Task<UpdateTaskResponse> UpdateTask([FromServices] IMediator mediator, [FromBody] TaskEntityUpdateDTO taskEntityDTO)
    {
        return await mediator.Send(new UpdateTaskRequest() { TaskEntityDTO = taskEntityDTO });
    }

    private static async Task<DeleteTaskResponse> Delete([FromServices] IMediator mediator, [FromRoute] Guid id)
    {
        return await mediator.Send(new DeleteTaskRequest() { TaskId = id });
    }
}