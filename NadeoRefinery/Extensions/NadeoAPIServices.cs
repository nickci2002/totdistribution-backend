using ManiaAPI.NadeoAPI;
using TOTDBackend.NadeoRefinery.NadeoApi;

namespace TOTDBackend.NadeoRefinery.Extensions;

// Modified code from ManiaAPI.NadeoAPI.Extensions.Hosting NuGet package
public static class NadeoAPIServices
{
    public static void AddNadeoAPIServices(this IServiceCollection services, IConfiguration config)
    {
        var credentials = new NadeoAPICredentials(
            config.GetValue<string>("Login")!,
            config.GetValue<string>("Password")!,
            AuthorizationMethod.UbisoftAccount);

        services.AddKeyedSingleton(nameof(NadeoServices),
                                   new NadeoAPIHandler { PendingCredentials = credentials });
        services.AddKeyedSingleton(nameof(NadeoLiveServices),
                                   new NadeoAPIHandler { PendingCredentials = credentials });

        var userAgent = config.GetValue<string>("UserAgent")!;
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
}