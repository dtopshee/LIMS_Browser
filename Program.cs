using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LegislationTimeMachine;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Register our State Service for the Timeline
builder.Services.AddSingleton<LegislationStateService>();

// Configure HttpClient to allow CORS-friendly fetches or local assets
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

await builder.Build().RunAsync();
