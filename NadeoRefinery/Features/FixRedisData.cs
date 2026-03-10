using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Models.Entities;

namespace TOTDBackend.NadeoRefinery.Features;

internal sealed class FixRedisData
{
    internal sealed class OldRepository(IConnectionMultiplexer redis)
        : IRedisRepositoryComponent<TOTDInfo>
    {
        private readonly IDatabase _db = redis.GetDatabase();

        public async Task StoreDataAsync(TOTDInfo data, TimeSpan? expiry = null)
        {
            throw new NotImplementedException("This repository is only meant to be used for retrieving data, not storing it!");
        }

        public TOTDInfo RetrieveData(string key)
        {
            Log.Information("Retrieving information for type {Type} with key {Key}...", nameof(TOTDInfo), key);
            
            //var fullKey = TOTDInfo.GetKey(key);
            var entries = _db.HashGetAll(key);
            if (entries.Length == 0)
            {
                Log.Error("Key {Key} was not found in the Redis database!", key);
                return TOTDInfo.Empty;
            }

            var redisEntry = TOTDInfo.RecoveryDehashify(entries);
            if (redisEntry == TOTDInfo.Empty)
            {
                Log.Error("Key {Key} was not found in the Redis database!", key);
                return TOTDInfo.Empty;
            }

            Log.Information("Found data with key {Key}!", key);
            Log.Debug("Value found: {Id}", JsonSerializer.Serialize(redisEntry));

            return redisEntry;
        }
    }

    internal sealed class NewRepository(IConnectionMultiplexer redis)
        : IRedisRepositoryComponent<TOTDInfo>
    {
        private readonly IDatabase _db = redis.GetDatabase();

        public async Task StoreDataAsync(TOTDInfo data, TimeSpan? expiry = null)
        {
            var totdId = data.Id.ToString();

            Log.Information("Storing data with key {Id} into the Redis database...", totdId);
            Log.Debug("Value found: {Data}", JsonSerializer.Serialize(data));
            
            var key = TOTDInfo.GetKey(totdId);
            var entries = data.Hashify();
            await _db.HashSetAsync(key, entries);

            if (expiry.HasValue)
                await _db.KeyExpireAsync(key, expiry);

            Log.Information("Successfully stored data with key {Id}!", totdId);
        }

        public TOTDInfo RetrieveData(string key)
        {
            throw new NotImplementedException("This repository is only meant to be used for storing data, not retrieving it!");
        }
    }

    public class TestEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/jobs/fix-redis-data", async ([FromServices] IConnectionMultiplexer redis) =>
            {
                var oldRepo = new OldRepository(redis);
                var newRepo = new NewRepository(redis);
                var db = redis.GetDatabase();

                var server = redis.GetServer(redis.GetEndPoints().First());
                var keys = server.Keys(pattern: "totd:map:*");

                int fixedCount = 0;
                foreach (var key in keys)
                {
                    //var keyString = key.ToString().Replace("totd:", "");
                    var oldData = oldRepo.RetrieveData(key!);
                    
                    if (oldData != TOTDInfo.Empty)
                    {
                        await newRepo.StoreDataAsync(oldData);
                        fixedCount++;
                    }
                }
            });
        }
    }
}
