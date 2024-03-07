using API.Data;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using API.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using API.Helpers;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<DataContext>(opt => 
            {
               opt.UseSqlite(config.GetConnectionString("DefaultConnection"));

             });

            services.AddCors(options => {
                 options.AddPolicy("CorsPolicy",
                  builder =>
                  {
                    builder.WithOrigins("*")
                             .AllowAnyHeader()
                             .AllowAnyMethod();
                   });
            });



            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
      
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //services.AddAutoMapper(typeof(AutoMapperProfiles));
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
           // services.AddHttpContextAccessor();
 
            return services;
        }
        
    }
}