using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RotiseriaWeb;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var webAddress = new Uri(builder.HostEnvironment.BaseAddress);
var apiPort = webAddress.Scheme == Uri.UriSchemeHttps ? 7148 : 5285;
var apiAddress = new UriBuilder(webAddress.Scheme, webAddress.Host, apiPort).Uri;
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = apiAddress });

builder.Services.AddMudServices();

await builder.Build().RunAsync();
