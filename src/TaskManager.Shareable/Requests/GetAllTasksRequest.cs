using MediatR;
using TaskManager.Shareable.Responses;

namespace TaskManager.Shareable.Requests
{
    public class GetAllTasksRequest : IRequest<GetAllTaskResponse>
    {
    }
}