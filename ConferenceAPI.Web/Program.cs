using ConferenceAPI.Web.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Serilog;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(Log.Logger);
    builder.Services.ConfigureServices(configuration);
    builder.Services.AddHttpClients(configuration);

    var app = builder.Build();

    app.UseExceptionHandler(appBuilder => appBuilder.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.GetRequiredFeature<IExceptionHandlerPathFeature>();
        Exception exception = exceptionHandlerPathFeature.Error;
        context.Response.StatusCode = 500;
        Log.Error(exception, "An exception occured while processing the request");
        await context.Response.WriteAsJsonAsync("Internal Server Error");
    }));

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

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