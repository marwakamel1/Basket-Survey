using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace basketSurvey
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services) {

            services.AddControllers();
            services.AddScoped<IPollService, PollService>();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();
            services.AddSwaggerGen();

            //Add Mapster
            var mappingConfig = TypeAdapterConfig.GlobalSettings;
            mappingConfig.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton<IMapper>(new Mapper(mappingConfig));


            services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }

    }
}
