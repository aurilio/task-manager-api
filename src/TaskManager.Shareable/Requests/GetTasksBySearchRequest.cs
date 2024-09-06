using MediatR;
using TaskManager.Shareable.Responses;

namespace TaskManager.Shareable.Requests
{
    public class GetTasksBySearchRequest : IRequest<GetTasksBySearchResponse>
    {
        public string Search { get; set; } = default!;
    }
}