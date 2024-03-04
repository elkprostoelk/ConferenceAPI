using ConferenceAPI.Web.Extensions;
using Serilog;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: false, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .CreateLogger();

Log.Information("Starting up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(Log.Logger);
    builder.Services.ConfigureServices(configuration);
    builder.Services.AddHttpClients(configuration);

    var app = builder.Build();
    app.UseExceptionHandler();

    app.UseSwagger();
    app.UseSwaggerUI();

    var origins = configuration["CorsAllowedOrigins"]!.Split(';', StringSplitOptions.RemoveEmptyEntries);
    app.UseHttpsRedirection();
    app.UseCors(policyBuilder => policyBuilder
        .WithOrigins(origins)
        .AllowAnyHeader()
        .AllowAnyMethod());

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception e)
{
    Log.Fatal(e, "An error while starting up!");
    throw;
}
finally
{
    Log.Information("Closing app.");
    await Log.CloseAndFlushAsync();
}