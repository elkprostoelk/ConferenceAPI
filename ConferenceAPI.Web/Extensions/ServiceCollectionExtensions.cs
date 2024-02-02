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

            services.AddDbContext<ConferenceApiDbContext>(opts => opts.UseNpgsql(configuration.GetConnectionString("ConferenceAPI")));

            services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());
        }

        public static void AddZoomApiClient(this IServiceCollection services, IConfiguration configuration)
        {
            var zoomApiBaseAddress = configuration["ZoomAPI"];
            if (string.IsNullOrEmpty(zoomApiBaseAddress))
            {
                throw new ArgumentException("Zoom API base address is not configured!");
            }

            services.AddHttpClient("ZoomAPI", client => client.BaseAddress = new Uri(zoomApiBaseAddress));
        }
    }
}
