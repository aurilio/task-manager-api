namespace Task.Manager.Domain.Interfaces
{
    public interface ICacheService
    {
        Task<string> GetCacheAsync(string key);

        void SetCacheAsync(string key, string value);

        void RemoveCacheAsync(string key);
    }
}