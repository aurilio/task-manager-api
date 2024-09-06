using MediatR;
using Task.Manager.Shareable.Responses;

namespace Task.Manager.Shareable.Requests
{
    public class GetAllTasksRequest : IRequest<GetAllTaskResponse>
    {
    }
}