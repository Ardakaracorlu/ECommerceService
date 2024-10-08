using Notification.Api.Extension;
using Notification.Consumer.Consumer;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
#endif
#if RELEASE
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
#endif

// Add services to the container.
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
.Build();

builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
NotificationExtension.RegisterService(builder.Services, configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<EmailConsumer>(); // Arka plan servisini enjekte et
        services.AddHostedService<SmsConsumer>(); // Arka plan servisini enjekte et
    });
