namespace TaskManager.Domain.Interfaces
{
    public interface ICacheService
    {
        Task<string> GetCacheAsync(string key);

        Task SetCacheAsync(string key, string value);

        Task RemoveCacheAsync(string key);
    }
}