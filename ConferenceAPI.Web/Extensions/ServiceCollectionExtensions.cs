using ConferenceAPI.Core.Interfaces;
using ConferenceAPI.Core.Services;
using ConferenceAPI.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ConferenceAPI.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddMemoryCache();

            services.AddDbContext<ConferenceApiDbContext>(opts => opts.UseNpgsql(configuration.GetConnectionString("ConferenceAPI")));

            services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());
            services.AddScoped<IZoomApiService, ZoomApiService>();
        }

        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var zoomApiBaseAddress = configuration["ZoomApiBaseUrl"];
            if (string.IsNullOrEmpty(zoomApiBaseAddress))
            {
                throw new ArgumentException("Zoom API base address is not configured!");
            }
            var zoomAuthAddress = configuration["ZoomAuthUrl"];
            if (string.IsNullOrEmpty(zoomAuthAddress))
            {
                throw new ArgumentException("Zoom Auth address is not configured!");
            }
            var clientId = configuration["ZoomApp:ClientId"];
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("Zoom client ID is not configured!");
            }
            var clientSecret = configuration["ZoomApp:ClientSecret"];
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException("Zoom client secret is not configured!");
            }

            services.AddHttpClient("ZoomAPI", client => client.BaseAddress = new Uri(zoomApiBaseAddress));
            services.AddHttpClient("ZoomAuth", client =>
            {
                client.BaseAddress = new Uri(zoomAuthAddress);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"))
                    );
            });
        }
    }
}
