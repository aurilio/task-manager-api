using MediatR;
using TaskManager.Shareable.Responses;

namespace TaskManager.Shareable.Requests
{
    public class GetTasksBySearchRequest : IRequest<GetTasksBySearchResponse>
    {
        public string Search { get; set; } = default!;

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public GetTasksBySearchRequest(string search, int pageNumber = 1, int pageSize = 20)
        {
            Search = search;
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 1 ? 20 : pageSize;
        }
    }
}