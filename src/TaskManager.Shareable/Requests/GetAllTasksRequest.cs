using MediatR;
using TaskManager.Shareable.Responses;

namespace TaskManager.Shareable.Requests
{
    public class GetAllTasksRequest : IRequest<GetAllTaskResponse>
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public GetAllTasksRequest(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 1 ? 20 : pageSize;
        }
    }
}