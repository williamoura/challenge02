using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Services;

namespace Challenge02.Api.Modules
{
    public static class DomainModule
    {
        public static IServiceCollection AddDomain(
             this IServiceCollection services)
        {
            services.AddSingleton<IEmailNormalizationService, EmailNormalizationService>();

            return services;
        }
    }
}
