using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using LegislationTimeMachine;
using LegislationTimeMachine.Services; 

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<LegislationTimeMachine.App>("#app");

// Register the HttpClient
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

// Register the State Service for the Timeline
builder.Services.AddSingleton<LegislationStateService>();

//await builder.Build().RunAsync();

var host = builder.Build();

var stateService = host.Services.GetRequiredService<LegislationStateService>();
await stateService.InitializeAsync();

await host.RunAsync();
