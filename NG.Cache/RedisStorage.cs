using NG.Common;
using NG.Extensions;

namespace NG.Cache;

using System.Xml;
using StackExchange.Redis;

public class RedisStorage: IRedisStorage
{

    private readonly Redisconnection _redisConnection;
    private IDatabase writeDatabase => _redisConnection.GetWriteConnection();
    private IDatabase readDatabase => _redisConnection.GetReadConnection();
    private readonly string _redisCacheEnvironment;

    public RedisStorage(Redisconnection redisconnection, string redisCacheEnvironment)
    {
        _redisConnection = redisconnection;
        _redisCacheEnvironment = redisCacheEnvironment;
    }

    private string GetKey(string key) => $"REDIS{_redisCacheEnvironment.AsEmpty()}_{key}";
    
    public async Task<bool> LockTake(string key, string value, TimeSpan timeout)
    {
        key = GetKey(key);
        return await writeDatabase.LockTakeAsync(key, value, timeout);
    }

    public async Task<bool> LockRelease(string key, string value)
    {
        key = GetKey(key);
        return await writeDatabase.LockReleaseAsync(key, value);
    }

    public async Task<string> LockQuery(string key)
    {
        key = GetKey(key);
        return await writeDatabase.LockQueryAsync(key);
    }

    public async Task<bool> StringSet(string key, object value)
    {
        key = GetKey(key);
        RedisValue redisValue = ConvertInput(value);
        return await writeDatabase.StringSetAsync(key, redisValue, null);
    }
    
    public async Task<bool> StringSet((string, object)[] values)
    {
        KeyValuePair<RedisKey, RedisValue>[] redisValues = values
            .Select(p => new KeyValuePair<RedisKey, RedisValue>(GetKey(p.Item1), ConvertInput(p.Item2))).ToArray();
        return await writeDatabase.StringSetAsync(redisValues);
    }

    public async Task<bool> StringSet(string key, object value, TimeSpan timeout)
    {
        key = GetKey(key);
        RedisValue redisValue = ConvertInput(value);
        return await writeDatabase.StringSetAsync(key, redisValue, timeout);
    }

    public async Task<long> StringIncrement(string key, long value, TimeSpan timeout)
    {
        key = GetKey(key);
        long increment = await writeDatabase.StringIncrementAsync(key, value);
        await writeDatabase.KeyExpireAsync(key, timeout);
        return increment;
    }

    public async Task<bool> KeyDelete(string key)
    {
        key = GetKey(key);
        return await writeDatabase.KeyDeleteAsync(key);
    }
    
    public async Task<T> StringGet<T>(string key)
    {
        key = GetKey(key);
        RedisValue value = await readDatabase.StringGetAsync(key);
        return ConvertOutput<T>(value);
    }

    private byte[] ConvertInput(object value)
    {
        return Serialize.ProtoBufSerialize(value);
    }

    private T ConvertOutput<T>(RedisValue redisValue)
    {
        if (redisValue.HasValue)
        {
            return Serialize.ProtoBufDeserialize<T>(redisValue);
        }

        return default(T);
    }
}