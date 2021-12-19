using MongoDB.Driver;

namespace NG.EventBus;

public interface IEventStorageRepository
{
    Task Add(EventBusMessage message, EventBusMessage.EventStatusEnum status, string exception);
    Task ChangeStatus(long stt, EventBusMessage.EventStatusEnum status, string exception);
}

public class EventStorageRepository : IEventStorageRepository
{
    private readonly IMongoCollection<EventBusMessage> _collection;

    public EventStorageRepository(string connectionString, string dbName, string tableName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(dbName);
        _collection = database.GetCollection<EventBusMessage>(tableName);
    }
    
    public Task Add(EventBusMessage message, EventBusMessage.EventStatusEnum status, string exception)
    {
        return _collection.InsertOneAsync(message);
    }

    public Task ChangeStatus(long stt, EventBusMessage.EventStatusEnum status, string exception)
    {
        throw new NotImplementedException();
    }
}