using Nest;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;
using TaskManager.Shareable.DTOs;

namespace TaskManager.Data.Repositories;

public class ElasticSearchRepository : IElasticSearchRepository
{
    private readonly IElasticClient _elasticClient;

    public ElasticSearchRepository(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task SetTaskAsync(TaskEntity task)
    {
        var response = await _elasticClient.IndexDocumentAsync(task);

        if (!response.IsValid)
        {
            throw new Exception($"Falha ao salvar documento no Elasticsearch: {response.OriginalException?.Message}");
        }

        Console.WriteLine($"Documento salvo com sucesso no Elasticsearch. ID: {response.Id}");
    }

    public async Task<TaskEntity> GetTaskByIdAsync(string taskId)
    {
        var response = await _elasticClient.GetAsync<TaskEntity>(taskId, idx => idx.Index("tasks"));

        return response.Source;
    }

    public async Task<IEnumerable<TaskEntity>> SearchTasksAsync(string search)
    {
        var response = await _elasticClient.SearchAsync<TaskEntity>(s => s
            .Index("tasks")
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Title)
                    .Field(f => f.Description)
                    .Query(search)
                )
            )
        );

        return response.Documents;
    }

    public async Task RemoveTaskAsync(Guid taskId)
    {
        var response = await _elasticClient.DeleteAsync<TaskEntityCreateDTO>(taskId);

        if (!response.IsValid)
        {
            throw new Exception($"Falha ao deletar tarefa no Elasticsearch: {response.OriginalException?.Message}");
        }
    }

    public async Task<IEnumerable<TaskEntity>> SearchTasksAsync(string search, int pageNumber, int pageSize)
    {
        var response = await _elasticClient.SearchAsync<TaskEntity>(s => s
            .Query(q => q.MatchPhrase(m => m.Field(f => f.Title).Query(search)))
            .From((pageNumber - 1) * pageSize)
            .Size(pageSize)
        );

        return response.Documents;
    }

    public async Task<int> GetTotalTaskCountAsync(string search)
    {
        var response = await _elasticClient.CountAsync<TaskEntity>(s => s
            .Query(q => q.Match(m => m.Field(f => f.Title).Query(search)))
        );

        return (int)response.Count;
    }
}