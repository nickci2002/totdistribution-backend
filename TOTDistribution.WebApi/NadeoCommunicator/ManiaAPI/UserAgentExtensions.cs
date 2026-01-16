using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ManiaAPI.NadeoAPI.Extensions.Hosting;

// Modified code from ManiaAPI.NadeoAPI.Extensions.Hosting NuGet package
public static class NadeoAPIUserAgentExtensions
{
    public static void AddNadeoCommunicator(
        this IServiceCollection services,
        IConfiguration config)
    {
        var credentials = new NadeoAPICredentials(
            config.GetValue<string>("Login")!,
            config.GetValue<string>("Password")!,
            AuthorizationMethod.UbisoftAccount);
        
        services.AddKeyedSingleton(nameof(NadeoServices),
                                   new NadeoAPIHandler { PendingCredentials = credentials });
        services.AddKeyedSingleton(nameof(NadeoLiveServices),
                                   new NadeoAPIHandler { PendingCredentials = credentials });

        string userAgent = config.GetValue<string>("UserAgent")!;
        services.AddHttpClient<NadeoServices>().ConfigureHttpClient((client) =>
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent));
        services.AddHttpClient<NadeoLiveServices>().ConfigureHttpClient((client) =>
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent));

        services.AddTransient((provider) =>
        {
            var ns = new NadeoServices
            (
                provider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(NadeoServices)),
                provider.GetRequiredKeyedService<NadeoAPIHandler>(nameof(NadeoServices))
            );
            System.Diagnostics.Debug.WriteLine(ns.Client.DefaultRequestHeaders.UserAgent.ToString());
            //Console.WriteLine(ns.Client.DefaultRequestHeaders.UserAgent.ToString());

            return ns;
        });

        services.AddTransient(provider =>
        {
            var nls = new NadeoLiveServices
            (
                provider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(NadeoLiveServices)),
                provider.GetRequiredKeyedService<NadeoAPIHandler>(nameof(NadeoLiveServices))
            );
            System.Diagnostics.Debug.WriteLine(nls.Client.DefaultRequestHeaders.UserAgent.ToString());
            //Console.WriteLine(nls.Client.DefaultRequestHeaders.UserAgent.ToString());

            return nls;
        });
    }
}