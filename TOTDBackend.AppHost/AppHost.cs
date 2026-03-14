var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddLatestRedis("aspire-redis");

// builder.AddExecutable("my-compose", "docker", ".",
//     "compose", "up", "--abort-on-container-exit");

builder.AddProject<Projects.NadeoRefinery>("nadeo-refinery")
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WaitFor(redis);

builder.Build().Run();