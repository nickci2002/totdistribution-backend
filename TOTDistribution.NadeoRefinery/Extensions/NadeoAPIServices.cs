using ManiaAPI.NadeoAPI;
using TOTDistribution.NadeoRefinery.NadeoApi;

namespace TOTDistribution.NadeoRefinery.Extensions;

// Modified code from ManiaAPI.NadeoAPI.Extensions.Hosting NuGet package
public static class NadeoAPIServices
{
    public static void AddNadeoAPIServices(this IServiceCollection services, IConfiguration config)
    {
        var credentials = new NadeoAPICredentials(
            config.GetValue<string>("Login")!,
            config.GetValue<string>("Password")!,
            AuthorizationMethod.UbisoftAccount);

        var userAgent = config.GetValue<string>("UserAgent")!;
        
        // services.AddNadeoAPI(credentials, userAgent, typeof(NadeoServices));
        // services.AddNadeoAPI(credentials, userAgent, typeof(NadeoLiveServices));
        // services.AddNadeoAPI(credentials, userAgent, typeof(ExtendedNadeoLiveServices));
        services.AddKeyedSingleton(nameof(NadeoServices),
                                   new NadeoAPIHandler { PendingCredentials = credentials });
        services.AddKeyedSingleton(nameof(NadeoLiveServices),
                                   new NadeoAPIHandler { PendingCredentials = credentials });

        services.AddHttpClient<NadeoServices>().ConfigureHttpClient((client) =>
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent));
        services.AddHttpClient<ExtendedNadeoLiveServices>().ConfigureHttpClient((client) =>
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent));

        services.AddTransient((provider) => new NadeoServices(
            provider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(NadeoServices)),
            provider.GetRequiredKeyedService<NadeoAPIHandler>(nameof(NadeoServices))));
        services.AddTransient(provider => new ExtendedNadeoLiveServices(
            provider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(ExtendedNadeoLiveServices)),
            provider.GetRequiredKeyedService<NadeoAPIHandler>(nameof(NadeoLiveServices))));
    }

    public static void AddNadeoAPI(this IServiceCollection services, NadeoAPICredentials credentials, string userAgent, Type nadeoServiceType)
    {
        services.AddKeyedSingleton(nadeoServiceType.Name,
                                   new NadeoAPIHandler { PendingCredentials = credentials });
        
        services.AddHttpClient(nadeoServiceType.Name).ConfigureHttpClient((client) =>
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent));

        services.AddTransient(provider => Activator.CreateInstance(nadeoServiceType,
            provider.GetRequiredService<IHttpClientFactory>().CreateClient(nadeoServiceType.Name),
            provider.GetRequiredKeyedService<NadeoAPIHandler>(nadeoServiceType.Name))!);
    }
}