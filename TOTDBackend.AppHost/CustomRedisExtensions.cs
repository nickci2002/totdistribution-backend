using Microsoft.Extensions.DependencyInjection;

public static class CustomRedisExtensions
{
    /// <summary>
    /// Modified code from AddRedis from Aspire.Hosting.Redis.
    /// Adds a Redis resource to the distributed application builder using the latest Redis image.
    /// Configures password handling, health checks, environment variables, and entrypoint arguments.
    /// Handles password parameters to avoid issues with StackExchange.Redis and Azure Dev tooling.
    /// </summary>
    /// <param name="builder">The distributed application builder to add the Redis resource to.</param>
    /// <param name="name">The name of the Redis resource.</param>
    /// <param name="port">The optional port to expose for the Redis resource.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{RedisResource}"/> for further configuration of the Redis resource.
    /// </returns>
    public static IResourceBuilder<RedisResource> AddLatestRedis(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name)
    {
        return builder
            .AddRedis(name, 6379)
            .WithImage("redis/redis-stack-server", "latest")
            .WithDataVolume("aspire-redis-volume");
    }
}
