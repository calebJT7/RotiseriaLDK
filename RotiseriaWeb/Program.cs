using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services; // Agregá esto
using RotiseriaWeb;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuramos la dirección de tu API (revisá que el puerto sea el tuyo)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5285/") });

// Agregamos los servicios de MudBlazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();
