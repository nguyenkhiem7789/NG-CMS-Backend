namespace NG.Cache;

public interface IRedisStorage
{
    Task<bool> LockTake(string key, string value, TimeSpan timeout);
    
    Task<bool> LockRelease(string key, string value);
}