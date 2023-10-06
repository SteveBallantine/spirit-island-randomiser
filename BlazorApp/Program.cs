using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SiRandomizer;
using SiRandomizer.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.SetMinimumLevel(LogLevel.Warning);
#if(DEBUG)
builder.Logging.AddFilter("SiRandomizer", LogLevel.Debug);
#endif

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<ConfigurationService>();
builder.Services.AddSingleton<SetupGenerator>();
builder.Services.AddSingleton<PresetService>();
builder.Services.AddSingleton<HandelabraLaunchService>();

await builder.Build().RunAsync();
