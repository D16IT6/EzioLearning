using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


namespace EzioLearning.Wasm;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        ApiConstants.BaseUrl =
            builder.Configuration[nameof(ApiConstants.BaseUrl)]
            ??
            throw new KeyNotFoundException("Cannot found baseurl");

        await builder.Services.ConfigureServices(builder.Configuration);
     

        var host = builder.Build();

        await host.LoadCurrentCulture();

        await host.RunAsync();
    }
}