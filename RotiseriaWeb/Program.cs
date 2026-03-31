using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RotiseriaWeb;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// IMPORTANTE: Revisá que este sea el puerto de tu API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5285/") });

builder.Services.AddMudServices();

await builder.Build().RunAsync();
