using app_configuration_dynamic_reload.Models;



var builder = WebApplication.CreateBuilder(args);

// Read configuration
var connectionString = builder.Configuration.GetConnectionString("AppConfig");

// Configure Azure App Configuration
builder.Host.ConfigureAppConfiguration(builder => builder.AddAzureAppConfiguration(options =>
{
    options.Connect(connectionString)
        .ConfigureRefresh(refresh =>
        {
            refresh.Register("TestApp:Settings:Sentinel", refreshAll: true)
                                           .SetCacheExpiration(TimeSpan.FromSeconds(10));
        });

}));

// Add services to the container.
builder.Services.Configure<PageSettings>(builder.Configuration.GetSection("TestApp:Settings"));
builder.Services.AddRazorPages();
builder.Services.AddAzureAppConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAzureAppConfiguration();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
