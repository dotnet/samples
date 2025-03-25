using GPSTracker.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.AddKeyedAzureTableClient("clustering");
builder.UseOrleansClient();

using var host = builder.Build();
await host.StartAsync();

IHostApplicationLifetime lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
IClusterClient client = host.Services.GetRequiredService<IClusterClient>();

await LoadDriver.DriveLoad(client, 25, lifetime.ApplicationStopping);
await host.StopAsync();
