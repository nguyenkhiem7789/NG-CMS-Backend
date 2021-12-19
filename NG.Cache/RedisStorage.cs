using NG.Common;
using NG.Extensions;

namespace NG.Cache;

using System.Xml;
using StackExchange.Redis;

public class RedisStorage : IRedisStorage
    {
        private readonly RedisConnection _redisConnection;
        private IDatabase WriteDatabase => _redisConnection.GetWriteConnection();
        private IDatabase ReadDatabase => _redisConnection.GetReadConnection();
        private readonly string _redisCacheEnvironment;

        public RedisStorage(RedisConnection redisConnection, string redisCacheEnvironment)
        {
            _redisConnection = redisConnection;
            _redisCacheEnvironment = redisCacheEnvironment;
        }

        private string GetKey(string key) => $"REDIS{_redisCacheEnvironment.AsEmpty()}_{key}";

        public async Task<bool> LockTake(string key, string value, TimeSpan timeout)
        {
            key = GetKey(key);
            return await WriteDatabase.LockTakeAsync(key, value, timeout);
        }

        public async Task<bool> LockRelease(string key, string value)
        {
            key = GetKey(key);
            return await WriteDatabase.LockReleaseAsync(key, value);
        }

        public async Task<string> LockQuery(string key)
        {
            key = GetKey(key);
            return await WriteDatabase.LockQueryAsync(key);
        }

        public async Task<bool> StringSet(string key, object value)
        {
            key = GetKey(key);
            RedisValue redisValue = ConvertInput(value);
            return await WriteDatabase.StringSetAsync(key, redisValue, null);
        }

        public async Task<bool[]> StringSet((string, object)[] values)
        {
            List<Task<bool>> tasks = new List<Task<bool>>();
            foreach (var value in values)
            {
                var task = StringSet(value.Item1, value.Item2);
                tasks.Add(task);
            }

            var results = await Task.WhenAll(tasks);
            return results;
            // KeyValuePair<RedisKey, RedisValue>[] redisValues = values
            //     .Select(p => new KeyValuePair<RedisKey, RedisValue>(GetKey(p.Item1), ConvertInput(p.Item2))).ToArray();
            // return await WriteDatabase.StringSetAsync(redisValues);
        }

        public async Task<bool> StringSet(string key, object value, TimeSpan timeout)
        {
            key = GetKey(key);
            RedisValue redisValue = ConvertInput(value);
            return await WriteDatabase.StringSetAsync(key, redisValue, timeout);
        }

        public async Task<long> StringIncrement(string key, long value, TimeSpan timeout)
        {
            key = GetKey(key);
            long increment = await WriteDatabase.StringIncrementAsync(key, value);
            await WriteDatabase.KeyExpireAsync(key, timeout);
            return increment;
        }

        public async Task<bool> KeyDelete(string key)
        {
            key = GetKey(key);
            return await WriteDatabase.KeyDeleteAsync(key);
        }

        public async Task<T> StringGet<T>(string key)
        {
            key = GetKey(key);
            RedisValue value = await ReadDatabase.StringGetAsync(key);
            return ConvertOutput<T>(value);
        }

        public async Task<T[]> StringGet<T>(string[] keys)
        {
            var tasks = new Task<RedisValue>[keys.Length];
            for (var i = 0; i < keys.Length; i++)
            {
                var key = GetKey(keys[i]);
                var t = ReadDatabase.StringGetAsync(key);
                tasks[i] = t;
            }

            var values = await Task.WhenAll(tasks);
            // RedisKey[] redisKeys = new RedisKey[keys.Length];
            // for (int i = 0; i < keys.Length; i++)
            // {
            //     redisKeys[i] = GetKey(keys[i]);
            // }
            //
            // RedisValue[] values = await ReadDatabase.StringGetAsync(redisKeys);
            T[] results = new T[keys.Length];
            int j = 0;
            foreach (var redisValue in values)
            {
                if (redisValue.HasValue)
                {
                    var obj = ConvertOutput<T>(redisValue);
                    results[j] = obj;
                }

                j++;
            }

            return results;
        }

        public async Task<long> ListLeftPush(string key, object value)
        {
            key = GetKey(key);
            return await WriteDatabase.ListLeftPushAsync(key, ConvertInput(value));
        }

        public async Task<bool> HashSet(string key, string field, object value)
        {
            key = GetKey(key);
            RedisValue redisValue = ConvertInput(value);
            var result = await WriteDatabase.HashSetAsync(key, field, redisValue);
            return result;
        }

        public async Task HashSet(string key, (string, object)[] items)
        {
            key = GetKey(key);
            HashEntry[] entries = items.Select(p => new HashEntry(p.Item1, ConvertInput(p.Item2))).ToArray();
            await WriteDatabase.HashSetAsync(key, entries);
        }

        public async Task<long> HashIncrement(string key, string field, long value)
        {
            key = GetKey(key);
            return await WriteDatabase.HashIncrementAsync(key, field, value);
        }

        public async Task<long?> HashIncrement(string key, string field, long value, long minValue)
        {
            key = GetKey(key);
            try
            {
                string luaScript =
                    @"local curentValue = tonumber(redis.call('HGET',@key,@field)); local value = tonumber(@value);
                                  if  curentValue == nil then redis.call('HSET',@key,@field, 0); curentValue = 0;  end 
                                  if (curentValue +  value) >= tonumber(@minvalue) then return redis.call('HINCRBY',@key,@field, @value) 
                                  else return nil end";
                var prepared = LuaScript.Prepare(luaScript);
                var val = await prepared.EvaluateAsync(WriteDatabase, new { key, field, value, minvalue = minValue });
                if (val.IsNull)
                {
                    return null;
                }

                return val.AsLong();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<KeyValuePair<string, KeyValuePair<string, T>[]>[]> HashGetAll<T>(string[] keys)
        {
            keys = keys.Select(GetKey).ToArray();
            string luaScript = @"
local function splitString(inputstr, sep)
                                        if sep == nil then
                                                sep = '%s'
                                        end
                                        local t={{}} ; local i=1
                                        for str in string.gmatch(inputstr, '([^'..sep..']+)') do
                                                t[i] = str;
                                                i = i + 1;
                                        end
                                        return t
                                end
local resultKeys = splitString(@keys, '@');
local r = {}
                                    for _, v in pairs(resultKeys) do
                                      r[#r+1] = redis.call('HGETALL', v)
                                    end

                                    return r";
            var redisKeys = string.Join("@", keys);
            var prepared = LuaScript.Prepare(luaScript);
            var vals = await prepared.EvaluateAsync(ReadDatabase, new { keys = redisKeys });
            List<KeyValuePair<string, KeyValuePair<string, T>[]>> valuePairs =
                new List<KeyValuePair<string, KeyValuePair<string, T>[]>>();
            if (!vals.IsNull)
            {
                var results = (RedisResult[])vals;

                int j = 0;
                foreach (var redisResult in results)
                {
                    List<KeyValuePair<string, T>> valuePairsByKey = new List<KeyValuePair<string, T>>();
                    if (redisResult.IsNull)
                    {
                        continue;
                    }

                    var resultsByKey = (RedisResult[])redisResult;
                    for (int i = 0; i < resultsByKey.Length; i += 2)
                    {
                        var key = resultsByKey[i];
                        var value = resultsByKey[i + 1];
                        if (value != null)
                        {
                            var valueObject = ConvertOutput<T>((byte[])value);
                            valuePairsByKey.Add(new KeyValuePair<string, T>((string)key, valueObject));
                        }
                        else
                        {
                            valuePairsByKey.Add(new KeyValuePair<string, T>((string)key, default(T)));
                        }
                    }

                    valuePairs.Add(
                        new KeyValuePair<string, KeyValuePair<string, T>[]>(keys[j], valuePairsByKey.ToArray()));
                    j++;
                }
            }

            return valuePairs.ToArray();
        }

        public async Task<KeyValuePair<string, KeyValuePair<string, long>[]>[]> HashGetAll(string[] keys)
        {
            keys = keys.Select(GetKey).ToArray();
            string luaScript = @"
local function splitString(inputstr, sep)
                                        if sep == nil then
                                                sep = '%s'
                                        end
                                        local t={{}} ; local i=1
                                        for str in string.gmatch(inputstr, '([^'..sep..']+)') do
                                                t[i] = str;
                                                i = i + 1;
                                        end
                                        return t
                                end
local resultKeys = splitString(@keys, '@');
local r = {}
                                    for _, v in pairs(resultKeys) do
                                      r[#r+1] = redis.call('HGETALL', v)
                                    end

                                    return r";
            var redisKeys = string.Join("@", keys);
            var prepared = LuaScript.Prepare(luaScript);
            var vals = await prepared.EvaluateAsync(ReadDatabase, new { keys = redisKeys });
            List<KeyValuePair<string, KeyValuePair<string, long>[]>> valuePairs =
                new List<KeyValuePair<string, KeyValuePair<string, long>[]>>();
            if (!vals.IsNull)
            {
                var results = (RedisResult[])vals;

                int j = 0;
                foreach (var redisResult in results)
                {
                    List<KeyValuePair<string, long>> valuePairsByKey = new List<KeyValuePair<string, long>>();
                    if (redisResult.IsNull)
                    {
                        continue;
                    }

                    var resultsByKey = (RedisResult[])redisResult;
                    for (int i = 0; i < resultsByKey.Length; i += 2)
                    {
                        var key = resultsByKey[i];
                        var value = resultsByKey[i + 1];
                        if (value != null)
                        {
                            valuePairsByKey.Add(new KeyValuePair<string, long>((string)key, (long)value));
                        }
                        else
                        {
                            valuePairsByKey.Add(new KeyValuePair<string, long>((string)key, 0));
                        }
                    }

                    valuePairs.Add(
                        new KeyValuePair<string, KeyValuePair<string, long>[]>(keys[j], valuePairsByKey.ToArray()));
                    j++;
                }
            }

            return valuePairs.ToArray();
        }

        public async Task<KeyValuePair<string, long>[]> HashGetAll(string key)
        {
            key = GetKey(key);
            HashEntry[] hashEntries = await ReadDatabase.HashGetAllAsync(key);
            return hashEntries.Select(p => new KeyValuePair<string, long>(p.Name, (long)p.Value)).ToArray();
        }

        public async Task<KeyValuePair<string, T>[]> HashGetAll<T>(string key)
        {
            key = GetKey(key);
            HashEntry[] hashEntries = await ReadDatabase.HashGetAllAsync(key);
            if (hashEntries.Length > 0)
            {
                KeyValuePair<string, T>[] results = new KeyValuePair<string, T>[hashEntries.Length];
                int i = 0;
                foreach (var hashEntry in hashEntries)
                {
                    results[i] = new KeyValuePair<string, T>(hashEntry.Name, ConvertOutput<T>(hashEntry.Value));
                    i++;
                }

                return results;
            }

            return default(KeyValuePair<string, T>[]);
        }

        public async Task<T> HashGet<T>(string key, string field)
        {
            key = GetKey(key);
            RedisValue redisValue = await ReadDatabase.HashGetAsync(key, field);
            return ConvertOutput<T>(redisValue);
        }

        public async Task<T[]> HashGet<T>(string key, string[] fields)
        {
            key = GetKey(key);
            RedisValue[] redisValues = fields.Select(p => (RedisValue)p).ToArray();
            var values = await ReadDatabase.HashGetAsync(key, redisValues);
            T[] results = new T[values.Length];
            int i = 0;
            foreach (var redisValue in values)
            {
                if (redisValue.HasValue)
                {
                    var obj = ConvertOutput<T>(redisValue);
                    results[i] = obj;
                }

                i++;
            }

            return results;
        }

        public async Task<bool> HashDelete(string key, string field)
        {
            key = GetKey(key);
            return await WriteDatabase.HashDeleteAsync(key, field);
        }

        public async Task<bool> KeyExist(string key)
        {
            key = GetKey(key);
            return await ReadDatabase.KeyExistsAsync(key);
        }

        public async Task<bool> HashExists(string key, string field)
        {
            key = GetKey(key);
            return await WriteDatabase.HashExistsAsync(key, field);
        }

        public async Task<bool> SortedSetAdd(string key, string member, double score)
        {
            key = GetKey(key);
            var result = await WriteDatabase.SortedSetAddAsync(key, member, score);
            return result;
        }

        public async Task<long> SortedSetAdd(string key, (string, double)[] members)
        {
            key = GetKey(key);
            SortedSetEntry[] entries = members.Select(p => new SortedSetEntry(p.Item1, p.Item2)).ToArray();
            var result = await WriteDatabase.SortedSetAddAsync(key, entries);
            return result;
        }

        public async Task<SortedSetEntry[]> SortedSetRangeByScoreWithScores(string key,
            double start = double.NegativeInfinity,
            double stop = double.PositiveInfinity,
            Order order = Order.Ascending,
            long skip = 0,
            long take = -1)
        {
            key = GetKey(key);
            var result =
                await WriteDatabase.SortedSetRangeByScoreWithScoresAsync(key, start, stop, order: order, skip: skip,
                    take: take);
            return result;
        }

        private byte[] ConvertInput(object value)
        {
            return Serialize.ProtoBufSerialize(value);
        }

        private T? ConvertOutput<T>(RedisValue redisValue)
        {
            if (redisValue.HasValue)
            {
                return Serialize.ProtoBufDeserialize<T>(redisValue);
            }

            return default(T);
        }
    }